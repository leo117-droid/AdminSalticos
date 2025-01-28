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
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.FileProviders;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class MobiliarioControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private MobiliarioController _controller;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _controller = new MobiliarioController(_unidadTrabajoMock.Object, _webHostEnvironmentMock.Object);

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
            var modelo = (Mobiliario)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0));
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var mobiliarioId = 1;
            var mobiliario = new Mobiliario
            {
                Id = mobiliarioId,
                Nombre = "Sillas",
                Descripcion = "Sillas de metal",
                Precio = 375
            };
            _unidadTrabajoMock.Setup(u => u.Mobiliario.Obtener(mobiliarioId)).ReturnsAsync(mobiliario);

            var resultado = await _controller.Upsert(mobiliarioId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Mobiliario)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(mobiliarioId));
        }

        // Verifica que el método Upsert cree un nuevo mobiliario cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaNuevoMobiliarioConImagen()
        {
            var mobiliario = new Mobiliario
            {
                Id = 0,
                Nombre = "Mesa niños",
                Descripcion = "mesa cuadrada pequeña para niños",
                Precio = 450
            };

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("imagen.jpg");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var files = new FormFileCollection { fileMock.Object };
            _controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), files);

            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string mobiliarioPath = Path.Combine(webRootPath, "imagenes", "mobiliario");

            if (!Directory.Exists(mobiliarioPath))
            {
                Directory.CreateDirectory(mobiliarioPath);
            }

            _webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(webRootPath);

            _unidadTrabajoMock.Setup(u => u.Mobiliario.Agregar(It.IsAny<Mobiliario>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(mobiliario);

            _unidadTrabajoMock.Verify(u => u.Mobiliario.Agregar(It.IsAny<Mobiliario>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once);
            Assert.That(_controller.TempData[DS.Exitosa], Is.EqualTo("Mobiliario creado Exitosamente"));
        }


        // Verifica que el método Upsert actualice un mobiliario y actualice la nueva imagen.
        [Test]
        public async Task Upsert_Post_ActualizaMobiliarioConImagen()
        {
            var mobiliarioExistente = new Mobiliario
            {
                Id = 1, 
                Nombre = "Mesa",
                ImageUrl = "imagen_anterior.jpg",
                Descripcion = "Mesa grande para adultos",
                Precio = 800
            };

            var mobiliarioActualizado = new Mobiliario
            {
                Id = 1,
                Nombre = "Galleta suiza",
                Descripcion = "Galleta suiza con dulce de leche",
                Precio = 800
            };

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("nueva_imagen.jpg");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var files = new FormFileCollection { fileMock.Object };
            _controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), files);

            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string mobiliarioPath = Path.Combine(webRootPath, "imagenes", "mobiliario");

            if (!Directory.Exists(mobiliarioPath))
            {
                Directory.CreateDirectory(mobiliarioPath);
            }

            _webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(webRootPath);

            _unidadTrabajoMock.Setup(u => u.Mobiliario.ObtenerPrimero(It.IsAny<Expression<Func<Mobiliario, bool>>>(), null, false))
                              .ReturnsAsync(mobiliarioExistente); 
            _unidadTrabajoMock.Setup(u => u.Mobiliario.Actualizar(It.IsAny<Mobiliario>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(mobiliarioActualizado);

            _unidadTrabajoMock.Verify(u => u.Mobiliario.Actualizar(It.IsAny<Mobiliario>()), Times.Once); 
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once); 

            Assert.That(_controller.TempData[DS.Exitosa], Is.EqualTo("Mobiliario actualizado Exitosamente"));
        }

        // Verifica que el método Upsert actualice un mobiliario
        [Test]
        public async Task Upsert_Post_ActualizaMobiliario_NoActualizarImagen()
        {
            var mobiliarioExistente = new Mobiliario
            {
                Id = 1,
                Nombre = "Mesa",
                ImageUrl = "mesa.jpg",
                Descripcion = "Mesa grande para adultos",
                Precio = 800
            };

            var mobiliarioActualizado = new Mobiliario
            {
                Id = 1,
                Nombre = "Mesa",
                ImageUrl = "mesa.jpg",
                Descripcion = "Mesa grande para adultos",
                Precio = 800
            };

            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            _webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(webRootPath);

            _unidadTrabajoMock.Setup(u => u.Mobiliario.ObtenerPrimero(It.IsAny<Expression<Func<Mobiliario, bool>>>(), null, false))
                              .ReturnsAsync(mobiliarioExistente); 
            _unidadTrabajoMock.Setup(u => u.Mobiliario.Actualizar(It.IsAny<Mobiliario>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var formFileCollection = new FormFileCollection(); 
            _controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), formFileCollection);

            var resultado = await _controller.Upsert(mobiliarioActualizado);

            _unidadTrabajoMock.Verify(u => u.Mobiliario.Actualizar(It.IsAny<Mobiliario>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once); 

            Assert.That(mobiliarioExistente.ImageUrl, Is.EqualTo("mesa.jpg"));

            Assert.That(_controller.TempData[DS.Exitosa], Is.EqualTo("Mobiliario actualizado Exitosamente"));
        }

        // Verifica que un mobiliario sin evento asociado sea eliminada correctamente.
        [Test]
        public async Task Delete_RetornaExito_CuandoElMobiliarioNoTieneEventosAsociados()
        {
            var mobiliarioId = 1;
            var mobiliarioBd = new Mobiliario
            {
                Id = mobiliarioId,
                Nombre = "Silla niños",
                ImageUrl = "silla.jpg",
                Descripcion = "Silla pequeña para niños menores de 10 años",
                Precio = 300
            };

            _unidadTrabajoMock.Setup(u => u.Mobiliario.Obtener(mobiliarioId)).ReturnsAsync(mobiliarioBd);
            _unidadTrabajoMock.Setup(u => u.Mobiliario.Remover(It.IsAny<Mobiliario>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(mobiliarioId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);
            Assert.That(contenido["success"].ToString(), Is.EqualTo("True")); 
            _unidadTrabajoMock.Verify(u => u.Mobiliario.Remover(It.IsAny<Mobiliario>()), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar un mobiliario que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoElMobiliarioNoExiste()
        {
            var mobiliarioId = 999;
            Mobiliario mobiliarioBd = null;

            _unidadTrabajoMock.Setup(u => u.Mobiliario.Obtener(mobiliarioId)).ReturnsAsync(mobiliarioBd);

            var resultado = await _controller.Delete(mobiliarioId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Mobiliario"));
        }

        // Verifica que el método ValidarNombre retorne false si el nombre no está duplicado.
        [Test]
        public async Task ValidarNombre_NombreNoDuplicado_RetornaFalse()
        {
            var nombreNuevo = "Silla";
            var listaMobiliario = new List<Mobiliario>
            {
                new Mobiliario { Id = 1, Nombre = "Mesa" },
                new Mobiliario { Id = 2, Nombre = "Mantel" }
            };

            _unidadTrabajoMock.Setup(u => u.Mobiliario.ObtenerTodos(It.IsAny<Expression<Func<Mobiliario, bool>>>(), null, null, true))
                .ReturnsAsync(listaMobiliario);

            var resultado = await _controller.ValidarNombre(nombreNuevo);

            var jsonResult = resultado as JsonResult;
            var dataPropiedad = jsonResult.Value.GetType().GetProperty("data");
            var dataValor = dataPropiedad.GetValue(jsonResult.Value);
            Assert.That(dataValor, Is.False, "El valor de 'data' no es false.");
        }


        // Verifica que el método ValidarNombre retorne true si el nombre está duplicado.
        [Test]
        public async Task ValidarNombre_NombreNoDuplicado_RetornaTrue()
        {
            var nombreNuevo = "Mantel";
            var listaMobiliario = new List<Mobiliario>
            {
                new Mobiliario { Id = 1, Nombre = "Mesa" },
                new Mobiliario { Id = 2, Nombre = "Mantel" }
            };

            _unidadTrabajoMock.Setup(u => u.Mobiliario.ObtenerTodos(It.IsAny<Expression<Func<Mobiliario, bool>>>(), null, null, true))
                .ReturnsAsync(listaMobiliario);

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
