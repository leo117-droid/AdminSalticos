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
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SalticosAdmin.Utilidades;
using System.Linq.Expressions;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class HerramientaControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private HerramientaController _controller;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new HerramientaController(_unidadTrabajoMock.Object);

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
            var modelo = (Herramienta)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0));
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.

        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var herramientaId = 1;
            var herramienta = new Herramienta
            {
                Id = herramientaId,
                Nombre = "Martillo",
                Cantidad = 5
            };
            _unidadTrabajoMock.Setup(u => u.Herramienta.Obtener(herramientaId)).ReturnsAsync(herramienta);

            var resultado = await _controller.Upsert(herramientaId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Herramienta)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(herramientaId));
        }

        // Verifica que el método Upsert cree un nueva herramienta cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaHerramienta()
        {
            var herramienta = new Herramienta
            {
                Id = 0,
                Nombre = "Desarmador",
                Cantidad = 10
            };

            _unidadTrabajoMock.Setup(u => u.Herramienta.Agregar(It.IsAny<Herramienta>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Upsert(herramienta);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.Herramienta.Agregar(It.IsAny<Herramienta>()), Times.Once);
        }

        // Verifica que el método Upsert actualice una herramienta existente.

        [Test]
        public async Task Upsert_Post_ActualizaHerramienta()
        {
            var herramientaId = 1;
            var herramientaExistente = new Herramienta
            {
                Id = herramientaId,
                Nombre = "Taladro",
                Cantidad = 3
            };

            var herramientaActualizada = new Herramienta
            {
                Id = herramientaId,
                Nombre = "Taladro",
                Cantidad = 4
            };

            _unidadTrabajoMock.Setup(u => u.Herramienta.Obtener(herramientaId)).ReturnsAsync(herramientaExistente);
            _unidadTrabajoMock.Setup(u => u.Herramienta.Actualizar(It.IsAny<Herramienta>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Upsert(herramientaActualizada);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.Herramienta.Actualizar(It.IsAny<Herramienta>()), Times.Once);
        }

        // Verifica que el método Delete elimine correctamente un herramiento.
        [Test]
        public async Task Delete_RetornaExito_CuandoLaHerramientaExiste()
        {
            var herramientaId = 1;
            var herramientaBd = new Herramienta
            {
                Id = herramientaId,
                Nombre = "Sierra",
                Cantidad = 2
            };

            _unidadTrabajoMock.Setup(u => u.Herramienta.Obtener(herramientaId)).ReturnsAsync(herramientaBd);
            _unidadTrabajoMock.Setup(u => u.Herramienta.Remover(It.IsAny<Herramienta>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(herramientaId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("true"));
            _unidadTrabajoMock.Verify(u => u.Herramienta.Remover(It.IsAny<Herramienta>()), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar una herramienta que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoLaHerramientaNoExiste()
        {
            var herramientaId = 999;
            Herramienta herramientaBd = null;

            _unidadTrabajoMock.Setup(u => u.Herramienta.Obtener(herramientaId)).ReturnsAsync(herramientaBd);

            var resultado = await _controller.Delete(herramientaId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Herramienta"));
        }

        [Test]
        public async Task ValidarNombre_NombreNoDuplicado_RetornaFalse()
        {
            var nombreNuevo = "Taladro";
            var listaHerramientas = new List<Herramienta>
            {
                new Herramienta { Id = 1, Nombre = "Martillo" },
                new Herramienta { Id = 2, Nombre = "Sierra" }
            };

            _unidadTrabajoMock.Setup(u => u.Herramienta.ObtenerTodos(It.IsAny<Expression<Func<Herramienta, bool>>>(), null, null, true))
                .ReturnsAsync(listaHerramientas);

            var resultado = await _controller.ValidarNombre(nombreNuevo);

            var jsonResult = resultado as JsonResult;
            var dataPropiedad = jsonResult.Value.GetType().GetProperty("data");
            var dataValor = dataPropiedad.GetValue(jsonResult.Value);
            Assert.That(dataValor, Is.False, "El valor de 'data' no es false.");
        }

        [Test]
        public async Task ValidarNombre_NombreDuplicado_RetornaTrue()
        {
            var nombreNuevo = "Sierra";
            var listaHerramientas = new List<Herramienta>
            {
                new Herramienta { Id = 1, Nombre = "Martillo" },
                new Herramienta { Id = 2, Nombre = "Sierra" }
            };

            _unidadTrabajoMock.Setup(u => u.Herramienta.ObtenerTodos(It.IsAny<Expression<Func<Herramienta, bool>>>(), null, null, true))
                .ReturnsAsync(listaHerramientas);

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
