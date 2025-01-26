using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SalticosAdmin.Areas.Admin.Controllers;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SalticosAdmin.Utilidades;
using System.Linq.Expressions;
using SalticosAdmin.AccesoDeDatos.Repositorio;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class ProveedorControllerTest
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private ProveedorController _controller;
        private Mock<HttpContext> _httpContextMock;
        private Mock<ITempDataDictionary> _tempDataMock;


        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new ProveedorController(_unidadTrabajoMock.Object);
            
            //Simula el login y creacion de un usuario
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));

            //Simula el HTTP necesario para el funcionamiento de la prueba
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            //Simula la existencia del TempData 
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            {
                [DS.Exitosa] = "Success message",
                [DS.Error] = "Error message"
            };
            _controller.TempData = tempData;

            // Simula los metodo para Bitacora 
            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
        }


        // Verifica que el método Index retorne una vista correctamente.
        [Test]
        public async Task Index_RetornaVista()
        {
            var resultado = _controller.Index();

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
        }

        // Verifica que, cuando el ID es null, el método Upsert retorne una vista con un modelo vacío.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloVacioCuandoIdEsNull()
        {
            var resultado = await _controller.Upsert((int?)null); 

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Proveedor)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0)); 
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var proveedorId = 1;
            var proveedor = new Proveedor
            { 
                Id = proveedorId,
                NombreEmpresa = "Pricesmart",
                Contacto = "Leonardo Mora",
                Telefono = "88723468",
                Correo = "leomora@gmail.com",
                Direccion = "Alajuela",
                Descripcion = "Proveedor de las sillas y mesas para adultos",
                TipoProveedor = "Mobiliario"

            };
            _unidadTrabajoMock.Setup(u => u.Proveedor.Obtener(proveedorId)).ReturnsAsync(proveedor);

            var resultado = await _controller.Upsert(proveedorId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Proveedor)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);  
            Assert.That(modelo.Id, Is.EqualTo(proveedorId)); 
        }


        // Verifica que el método Upsert cree un nuevo proveedor cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaProveedor()
        {
            var proveedorId = 0;
            var proveedor = new Proveedor
            {
                Id = proveedorId,
                NombreEmpresa = "Tips",
                Contacto = "Jimena Solorzano",
                Telefono = "65429081",
                Correo = "solorzano@gmail.com",
                Direccion = "San José",
                Descripcion = "Proveedor de los colorantes comestibles en variedad de colores",
                TipoProveedor = "Alimentación"

            };

            _unidadTrabajoMock.Setup(u => u.Proveedor.Agregar(It.IsAny<Proveedor>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(proveedor);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); 
            _unidadTrabajoMock.Verify(u => u.Proveedor.Agregar(It.IsAny<Proveedor>()), Times.Once);
        }

        // Verifica que el método Upsert actualice un´proveedor existente.
        [Test]
        public async Task Upsert_Post_ActualizaProveedor()
        {
            var proveedorId = 1;
            var proveedorExistente = new Proveedor
            {
                Id = proveedorId,
                NombreEmpresa = "Walmart",
                Contacto = "Jose Chaves",
                Telefono = "88765443",
                Correo = "josech@gmail.com",
                Direccion = "Heredia",
                Descripcion = "Proveedor de los parlantes",
                TipoProveedor = "Servicios Adicionales"
            };

            var proveedorActualizado = new Proveedor
            {
                Id = proveedorId,
                NombreEmpresa = "Walmart",
                Contacto = "Jose Chaves",
                Telefono = "88765443",
                Correo = "josech@gmail.com",
                Direccion = "Alajuela",
                Descripcion = "Proveedor de los parlantes",
                TipoProveedor = "Servicios Adicionales"
            };

            _unidadTrabajoMock.Setup(u => u.Proveedor.Obtener(proveedorId)).ReturnsAsync(proveedorExistente);
            _unidadTrabajoMock.Setup(u => u.Proveedor.Actualizar(It.IsAny<Proveedor>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(proveedorActualizado);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); 
            _unidadTrabajoMock.Verify(u => u.Proveedor.Actualizar(It.IsAny<Proveedor>()), Times.Once); 
        }

        // Verifica que el método Delete elimine correctamente un proveedor.
        [Test]
        public async Task Delete_RetornaExito_CuandoElProveedorSeEliminaCorrectamente()
        {
            var proveedorId = 0;
            var proveedor = new Proveedor
            {
                Id = proveedorId,
                NombreEmpresa = "EPA",
                Contacto = "Gabriel Garcia",
                Telefono = "87651021",
                Correo = "garcia@gmail.com",
                Direccion = "Heredia",
                Descripcion = "Proveedor de las herramientas",
                TipoProveedor = "Otros"

            };

            _unidadTrabajoMock.Setup(u => u.Proveedor.Obtener(proveedorId)).ReturnsAsync(proveedor);

            _unidadTrabajoMock.Setup(u => u.Proveedor.Remover(It.IsAny<Proveedor>())).Verifiable();

            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(proveedorId);

            _unidadTrabajoMock.Setup(u => u.Proveedor.Obtener(proveedorId)).ReturnsAsync((Proveedor)null);

            var proveedorEliminado = await _unidadTrabajoMock.Object.Proveedor.Obtener(proveedorId);

            Assert.That(proveedorEliminado, Is.Null);
            _unidadTrabajoMock.Verify(u => u.Proveedor.Remover(It.IsAny<Proveedor>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar un proveedor que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoElProveedorNoExiste()
        {
            var proveedorId = 999;
            Proveedor proveedorBd = null;

            _unidadTrabajoMock.Setup(u => u.Proveedor.Obtener(proveedorId)).ReturnsAsync(proveedorBd);

            var resultado = await _controller.Delete(proveedorId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;

            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar proveedor"));  
        }

        [TearDown]
        public void TearDown()
        {
            // Limpia las configuraciones y recursos después de cada prueba.
            _unidadTrabajoMock.Reset();

            if (_controller != null)
            {
                _controller.TempData?.Clear();

                if (_controller is IDisposable disposableController)
                {
                    disposableController.Dispose();
                }

                _controller = null;
            }
        }

    }
}

