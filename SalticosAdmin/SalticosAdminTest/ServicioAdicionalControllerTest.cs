using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SalticosAdmin.Areas.Admin.Controllers;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SalticosAdmin.Utilidades;
using System.Linq.Expressions;
using Microsoft.Extensions.Primitives;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class ServicioAdicionalControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private ServicioAdicionalController _controller;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _controller = new ServicioAdicionalController(_unidadTrabajoMock.Object, _webHostEnvironmentMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            {
                [DS.Exitosa] = "Success message",
                [DS.Error] = "Error message"
            };
            _controller.TempData = tempData;

            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
        }

        // Verifica que el método Index retorne una vista correctamente.
        [Test]
        public void Index_RetornaVista()
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
            var modelo = (ServicioAdicional)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0));
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var servicioAdicionalId = 1;
            var servicioAdicional = new ServicioAdicional
            {
                Id = servicioAdicionalId,
                Nombre = "Máquina de burbujas",
                Descripcion = "Servicio 30 minutos de máquina de burbujas",
                Precio = 10000,
                Inventario = 10
            };
            _unidadTrabajoMock.Setup(u => u.ServicioAdicional.Obtener(servicioAdicionalId)).ReturnsAsync(servicioAdicional);

            var resultado = await _controller.Upsert(servicioAdicionalId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (ServicioAdicional)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(servicioAdicionalId));
        }

        // Verifica que el método Upsert cree un nuevo servicio adicional cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaNuevoServicioAdicionalConImagen()
        {
            var servicioAdicional = new ServicioAdicional
            {
                Id = 0,
                Nombre = "Planta generadora de energía",
                Descripcion = "Planta portatil generadora de energía con combustible",
                Precio = 40000,
                Inventario = 3
            };

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("imagen.jpg");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var files = new FormFileCollection { fileMock.Object };
            _controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), files);

            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string servicioAdicionalPath = Path.Combine(webRootPath, "imagenes", "servicioAdicional");

            if (!Directory.Exists(servicioAdicionalPath))
            {
                Directory.CreateDirectory(servicioAdicionalPath);
            }

            _webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(webRootPath);

            _unidadTrabajoMock.Setup(u => u.ServicioAdicional.Agregar(It.IsAny<ServicioAdicional>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(servicioAdicional);

            _unidadTrabajoMock.Verify(u => u.ServicioAdicional.Agregar(It.IsAny<ServicioAdicional>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once);
            Assert.That(_controller.TempData[DS.Exitosa], Is.EqualTo("Servicio Adicional creado Exitosamente"));
        }


        // Verifica que el método Upsert actualice un servicio adicional y actualice la nueva imagen.
        [Test]
        public async Task Upsert_Post_ActualizaServicioAdicionalConImagen()
        {
            var servicioAdicionalExistente = new ServicioAdicional

            {
                Id = 1,
                Nombre = "Trampolín",
                ImageUrl = "imagen_anterior.jpg",
                Descripcion = "Trampolín para niños menores de 12 años",
                Precio = 35000,
                Inventario = 2
            };

            var servicioAdicionalActualizado = new ServicioAdicional
            {
                Id = 1,
                Nombre = "Trampolín",
                Descripcion = "Trampolín para niños menores de 12 años",
                Precio = 36500,
                Inventario = 3
            };

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("nueva_imagen.jpg");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var files = new FormFileCollection { fileMock.Object };
            _controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), files);

            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string servicioAdicionalPath = Path.Combine(webRootPath, "imagenes", "servicioAdicional");

            if (!Directory.Exists(servicioAdicionalPath))
            {
                Directory.CreateDirectory(servicioAdicionalPath);
            }

            _webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(webRootPath);

            _unidadTrabajoMock.Setup(u => u.ServicioAdicional.ObtenerPrimero(It.IsAny<Expression<Func<ServicioAdicional, bool>>>(), null, false))
                              .ReturnsAsync(servicioAdicionalExistente); 
            _unidadTrabajoMock.Setup(u => u.ServicioAdicional.Actualizar(It.IsAny<ServicioAdicional>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(servicioAdicionalActualizado);

            _unidadTrabajoMock.Verify(u => u.ServicioAdicional.Actualizar(It.IsAny<ServicioAdicional>()), Times.Once); 
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once); 

            Assert.That(_controller.TempData[DS.Exitosa], Is.EqualTo("Servicio adicional actualizado Exitosamente"));
        }

        // Verifica que el método Upsert actualice un servicio adicional.
        [Test]
        public async Task Upsert_Post_ActualizaServicioAdicional_NoActualizarImagen()
        {
            var servicioAdicionalExistente = new ServicioAdicional
            {
                Id = 1,
                Nombre = "Tira al blanco",
                ImageUrl = "tiroBlanco.jpg",
                Descripcion = "Tiro blanco inflable con tres pelotas",
                Precio = 12000,
                Inventario = 2
            };

            var servicioAdicionalActualizado = new ServicioAdicional
            {
                Id = 1,
                Nombre = "Tira al blanco",
                ImageUrl = "tiroBlanco.jpg",
                Descripcion = "Tiro blanco inflable con tres pelotas",
                Precio = 12000,
                Inventario = 4
            };

            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            _webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(webRootPath);

            _unidadTrabajoMock.Setup(u => u.ServicioAdicional.ObtenerPrimero(It.IsAny<Expression<Func<ServicioAdicional, bool>>>(), null, false))
                              .ReturnsAsync(servicioAdicionalExistente); 
            _unidadTrabajoMock.Setup(u => u.ServicioAdicional.Actualizar(It.IsAny<ServicioAdicional>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var formFileCollection = new FormFileCollection(); 
            _controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), formFileCollection);

            var resultado = await _controller.Upsert(servicioAdicionalActualizado);

            _unidadTrabajoMock.Verify(u => u.ServicioAdicional.Actualizar(It.IsAny<ServicioAdicional>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once); 

            Assert.That(servicioAdicionalExistente.ImageUrl, Is.EqualTo("tiroBlanco.jpg"));

            Assert.That(_controller.TempData[DS.Exitosa], Is.EqualTo("Servicio adicional actualizado Exitosamente"));
        }

        // Verifica que un servicio adicional sin evento asociado sea eliminada correctamente.
        [Test]
        public async Task Delete_RetornaExito_CuandoElServicioAdicionalNoTieneEventosAsociados()
        {
            var servicioAdicionalId = 1;
            var servicioAdicionalBd = new ServicioAdicional
            {
                Id = servicioAdicionalId,
                Nombre = "Hombre al agua",
                ImageUrl = "hombreAgua.jpg",
                Descripcion = "Juego hombre al agua, incluye manguera de 20 metros",
                Precio = 18000,
                Inventario = 2
            };

            _unidadTrabajoMock.Setup(u => u.ServicioAdicional.Obtener(servicioAdicionalId)).ReturnsAsync(servicioAdicionalBd);
            _unidadTrabajoMock.Setup(u => u.ServicioAdicional.Remover(It.IsAny<ServicioAdicional>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(servicioAdicionalId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);
            Assert.That(contenido["success"].ToString(), Is.EqualTo("True")); 
            _unidadTrabajoMock.Verify(u => u.ServicioAdicional.Remover(It.IsAny<ServicioAdicional>()), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar un servicio adicional que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoElServicioAdicionalNoExiste()
        {
            var servicioAdicionalId = 999;
            ServicioAdicional servicioAdicionalBd = null;

            _unidadTrabajoMock.Setup(u => u.ServicioAdicional.Obtener(servicioAdicionalId)).ReturnsAsync(servicioAdicionalBd);

            var resultado = await _controller.Delete(servicioAdicionalId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Servicio Adicional"));
        }

        // Verifica que el método ValidarNombre retorne false si el nombre no está duplicado.
        [Test]
        public async Task ValidarNombre_NombreNoDuplicado_RetornaFalse()
        {
            var nombreNuevo = "Sky dancer";
            var listaServicioAdicional= new List<ServicioAdicional>
            {
                new ServicioAdicional { Id = 1, Nombre = "Hombre al agua" },
                new ServicioAdicional { Id = 2, Nombre = "Máquina de burbujas" }
            };

            _unidadTrabajoMock.Setup(u => u.ServicioAdicional.ObtenerTodos(It.IsAny<Expression<Func<ServicioAdicional, bool>>>(), null, null, true))
                .ReturnsAsync(listaServicioAdicional);

            var resultado = await _controller.ValidarNombre(nombreNuevo);

            var jsonResult = resultado as JsonResult;
            var dataPropiedad = jsonResult.Value.GetType().GetProperty("data");
            var dataValor = dataPropiedad.GetValue(jsonResult.Value);
            Assert.That(dataValor, Is.False, "El valor de 'data' no es false.");
        }


        // Verifica que el método ValidarNombre retorne true si el nombre está duplicado.
        [Test]
        public async Task ValidarNombre_NombreDuplicado_RetornaTrue()
        {
            var nombreNuevo = "Sky dancer";
            var listaServicioAdicional = new List<ServicioAdicional>
            {
                new ServicioAdicional { Id = 1, Nombre = "Sky dancer" },
                new ServicioAdicional { Id = 2, Nombre = "Parlante de música" }
            };

            _unidadTrabajoMock.Setup(u => u.ServicioAdicional.ObtenerTodos(It.IsAny<Expression<Func<ServicioAdicional, bool>>>(), null, null, true))
                .ReturnsAsync(listaServicioAdicional);

            var resultado = await _controller.ValidarNombre(nombreNuevo);

            var jsonResult = resultado as JsonResult;
            var dataPropiedad = jsonResult.Value.GetType().GetProperty("data");
            var dataValor = dataPropiedad.GetValue(jsonResult.Value);
            Assert.That(dataValor, Is.True, "El valor de 'data' no es true.");
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
