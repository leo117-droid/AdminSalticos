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
using Microsoft.Extensions.FileProviders;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class AlimentacionControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private AlimentacionController _controller;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _controller = new AlimentacionController(_unidadTrabajoMock.Object, _webHostEnvironmentMock.Object);

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
            var modelo = (Alimentacion)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0));
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var alimentacionId = 1;
            var alimentacion = new Alimentacion
            {
                Id = alimentacionId,
                Nombre = "Palomitas",
                Descripcion = "Bolsa de papel con palomitas saladas",
                Precio = 750
            };
            _unidadTrabajoMock.Setup(u => u.Alimentacion.Obtener(alimentacionId)).ReturnsAsync(alimentacion);

            var resultado = await _controller.Upsert(alimentacionId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Alimentacion)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(alimentacionId));
        }

        // Verifica que el método Upsert cree una nuevo producto de alimentación cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaNuevaAlimentacionConImagen()
        {
            var nuevaAlimentacion = new Alimentacion
            {
                Id = 0,
                Nombre = "Churro",
                Descripcion = "Bolsa de papel con un churro relleno",
                Precio = 750
            };

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("imagen.jpg");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var files = new FormFileCollection { fileMock.Object };
            _controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), files);

            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string alimentacionPath = Path.Combine(webRootPath, "imagenes", "alimentacion");

            if (!Directory.Exists(alimentacionPath))
            {
                Directory.CreateDirectory(alimentacionPath);
            }

            _webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(webRootPath);

            _unidadTrabajoMock.Setup(u => u.Alimentacion.Agregar(It.IsAny<Alimentacion>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(nuevaAlimentacion);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            _unidadTrabajoMock.Verify(u => u.Alimentacion.Agregar(It.IsAny<Alimentacion>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once);
            Assert.That(_controller.TempData[DS.Exitosa], Is.EqualTo("Alimentacion creado Exitosamente"));
        }

        // Verifica que el método Upsert actualice un producto de alimentación y actualice la nueva imagen.
        [Test]
        public async Task Upsert_Post_ActualizaAlimentacionConImagen()
        {
            var alimentacionExistente = new Alimentacion
            {
                Id = 1, 
                Nombre = "Galleta suiza",
                ImageUrl = "imagen_anterior.jpg",
                Descripcion = "Galleta suiza con leche condensada",
                Precio = 800
            };

            var alimentacion = new Alimentacion
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
            string alimentacionPath = Path.Combine(webRootPath, "imagenes", "alimentacion");

            if (!Directory.Exists(alimentacionPath))
            {
                Directory.CreateDirectory(alimentacionPath);
            }

            _webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(webRootPath);

            _unidadTrabajoMock.Setup(u => u.Alimentacion.ObtenerPrimero(It.IsAny<Expression<Func<Alimentacion, bool>>>(), null, false))
                              .ReturnsAsync(alimentacionExistente); 
            _unidadTrabajoMock.Setup(u => u.Alimentacion.Actualizar(It.IsAny<Alimentacion>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(alimentacion);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            _unidadTrabajoMock.Verify(u => u.Alimentacion.Actualizar(It.IsAny<Alimentacion>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once); 
            Assert.That(_controller.TempData[DS.Exitosa], Is.EqualTo("Alimentacion actualizado Exitosamente"));
        }

        // Verifica que el método Upsert actualice un producto de alimentación.
        [Test]
        public async Task Upsert_Post_ActualizaAlimentacion_NoActualizarImagen()
        {
            var alimentacionExistente = new Alimentacion
            {
                Id = 1, 
                Nombre = "Granizado pequeño",
                ImageUrl = "granizado.jpg",
                Descripcion = "Granizado con dos leches",
                Precio = 800
            };

            var alimentacionActualizada = new Alimentacion
            {
                Id = 1,
                Nombre = "Granizado grande",
                ImageUrl = "granizado.jpg",
                Descripcion = "Granizado con dos leches",
                Precio = 800
            };

            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            _webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(webRootPath);

            _unidadTrabajoMock.Setup(u => u.Alimentacion.ObtenerPrimero(It.IsAny<Expression<Func<Alimentacion, bool>>>(), null, false))
                              .ReturnsAsync(alimentacionExistente); 
            _unidadTrabajoMock.Setup(u => u.Alimentacion.Actualizar(It.IsAny<Alimentacion>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var formFileCollection = new FormFileCollection(); 
            _controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), formFileCollection);

            var resultado = await _controller.Upsert(alimentacionActualizada);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            _unidadTrabajoMock.Verify(u => u.Alimentacion.Actualizar(It.IsAny<Alimentacion>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once); 

            Assert.That(alimentacionExistente.ImageUrl, Is.EqualTo("granizado.jpg"));

            Assert.That(_controller.TempData[DS.Exitosa], Is.EqualTo("Alimentacion actualizado Exitosamente"));
        }

        // Verifica que un producto de alimentación sin evento asociado sea eliminada correctamente.
        [Test]
        public async Task Delete_RetornaExito_CuandoLaAlimentacionNoTieneEventosAsociados()
        {
            var alimentacionId = 1;
            var alimentacionBd = new Alimentacion
            {
                Id = alimentacionId,
                Nombre = "Hot Dog",
                ImageUrl = "hotdog.jpg",
                Descripcion = "Hot dog con salsas y papas tostadas",
                Precio = 800
            };

            _unidadTrabajoMock.Setup(u => u.Alimentacion.Obtener(alimentacionId)).ReturnsAsync(alimentacionBd);
            _unidadTrabajoMock.Setup(u => u.Alimentacion.Remover(It.IsAny<Alimentacion>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(alimentacionId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);
            Assert.That(contenido["success"].ToString(), Is.EqualTo("True")); 
            _unidadTrabajoMock.Verify(u => u.Alimentacion.Remover(It.IsAny<Alimentacion>()), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar una producto de alimentación que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoLaAlimentacionNoExiste()
        {
            var alimentacionId = 999;
            Alimentacion alimentacionBd = null;

            _unidadTrabajoMock.Setup(u => u.Alimentacion.Obtener(alimentacionId)).ReturnsAsync(alimentacionBd);

            var resultado = await _controller.Delete(alimentacionId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Alimentacion"));
        }

        // Verifica que el método ValidarNombre retorne false si el nombre no está duplicado.
        [Test]
        public async Task ValidarNombre_NombreNoDuplicado_RetornaFalse()
        {
            var nombreNuevo = "Palomitas";
            var listaAlimentacion = new List<Alimentacion>
            {
                new Alimentacion { Id = 1, Nombre = "Hot Dog" },
                new Alimentacion { Id = 2, Nombre = "Churro" }
            };

            _unidadTrabajoMock.Setup(u => u.Alimentacion.ObtenerTodos(It.IsAny<Expression<Func<Alimentacion, bool>>>(), null, null, true))
                .ReturnsAsync(listaAlimentacion);

            var resultado = await _controller.ValidarNombre(nombreNuevo);

            var jsonResult = resultado as JsonResult;
            var dataPropiedad = jsonResult.Value.GetType().GetProperty("data");
            var dataValor = dataPropiedad.GetValue(jsonResult.Value);
            Assert.That(dataValor, Is.False, "El valor de 'data' no es false.");
        }

        // Verifica que el método ValidarNombre retorne true si el nombre no está duplicado.
        [Test]
        public async Task ValidarNombre_NombreDuplicado_RetornaTrue()
        {
            var nombreNuevo = "Churro";
            var listaAlimentacion = new List<Alimentacion>
            {
                new Alimentacion { Id = 1, Nombre = "Granizado" },
                new Alimentacion { Id = 2, Nombre = "Churro" }
            };

            _unidadTrabajoMock.Setup(u => u.Alimentacion.ObtenerTodos(It.IsAny<Expression<Func<Alimentacion, bool>>>(), null, null, true))
                .ReturnsAsync(listaAlimentacion);

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
