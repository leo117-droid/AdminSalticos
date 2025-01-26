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
    public class ContactoControllerTest
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private ContactoController _controller;
        private Mock<HttpContext> _httpContextMock;
        private Mock<ITempDataDictionary> _tempDataMock;


        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new ContactoController(_unidadTrabajoMock.Object);
            
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
            var modelo = (Contacto)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0)); 
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var contactoId = 1;
            var contacto = new Contacto
            { 
                Id = contactoId,
                Nombre = "Ian",
                Apellido = "Calvo",
                TipoServicio = "Mecánico",
                Direccion = "Santa Ana, San José",
                Telefono = "89765469",
                Correo = "ian@gmail.com"
            };
            _unidadTrabajoMock.Setup(u => u.Contacto.Obtener(contactoId)).ReturnsAsync(contacto);

            var resultado = await _controller.Upsert(contactoId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Contacto)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);  
            Assert.That(modelo.Id, Is.EqualTo(contactoId)); 
        }


        // Verifica que el método Upsert cree un nuevo proveedor cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CrearContacto()
        {
            var contactoId = 0;
            var contacto = new Contacto
            {
                Id = contactoId,
                Nombre = "Alejandro",
                Apellido = "Campos",
                TipoServicio = "Abogado",
                Direccion = "Hacienda Rivera Belén",
                Telefono = "65517892",
                Correo = "alecampos@gmail.com"
            };

            _unidadTrabajoMock.Setup(u => u.Contacto.Agregar(It.IsAny<Contacto>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(contacto);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); 
            _unidadTrabajoMock.Verify(u => u.Contacto.Agregar(It.IsAny<Contacto>()), Times.Once);
        }

        // Verifica que el método Upsert actualice un proveedor existente.
        [Test]
        public async Task Upsert_Post_ActualizarContacto()
        {
            var contactoId = 1;
            var contactoExistente = new Contacto
            {
                Id = contactoId,
                Nombre = "Fabiana",
                Apellido = "Rosales",
                TipoServicio = "Contadora",
                Direccion = "Condominio Agua Clara",
                Telefono = "88904287",
                Correo = "fabianarosales@gmail.com"
            };

            var contactoActualizado = new Contacto
            {
                Id = contactoId,
                Nombre = "Fabiana",
                Apellido = "Rosales",
                TipoServicio = "Contadora",
                Direccion = "Condominio Agua Clara",
                Telefono = "67890532",
                Correo = "fabianarosales@gmail.com"
            };

            _unidadTrabajoMock.Setup(u => u.Contacto.Obtener(contactoId)).ReturnsAsync(contactoExistente);
            _unidadTrabajoMock.Setup(u => u.Contacto.Actualizar(It.IsAny<Contacto>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(contactoActualizado);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); 
            _unidadTrabajoMock.Verify(u => u.Contacto.Actualizar(It.IsAny<Contacto>()), Times.Once); 
        }

        // Verifica que el método Delete elimine correctamente un proveedor.
        [Test]
        public async Task Delete_RetornaExito_CuandoElContactoSeEliminaCorrectamente()
        {
            var contactoId = 1;
            var contacto = new Contacto
            {
                Id = contactoId,
                Nombre = "Gabriela",
                Apellido = "Chaves",
                TipoServicio = "Diseñadora gráfica",
                Direccion = "La Trinidad, Alajuela",
                Telefono = "82309123",
                Correo = "gabychaves.com"
            };

            _unidadTrabajoMock.Setup(u => u.Contacto.Obtener(contactoId)).ReturnsAsync(contacto);

            _unidadTrabajoMock.Setup(u => u.Contacto.Remover(It.IsAny<Contacto>())).Verifiable();

            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(contactoId);

            _unidadTrabajoMock.Setup(u => u.Contacto.Obtener(contactoId)).ReturnsAsync((Contacto)null);

            var contactoEliminado = await _unidadTrabajoMock.Object.Contacto.Obtener(contactoId);

            Assert.That(contactoEliminado, Is.Null);
            _unidadTrabajoMock.Verify(u => u.Contacto.Remover(It.IsAny<Contacto>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar un proveedor que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoElProveedorNoExiste()
        {
            var contactoId = 999;
            Contacto contactoBd = null;

            _unidadTrabajoMock.Setup(u => u.Contacto.Obtener(contactoId)).ReturnsAsync(contactoBd);

            var resultado = await _controller.Delete(contactoId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;

            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Contacto"));  
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

