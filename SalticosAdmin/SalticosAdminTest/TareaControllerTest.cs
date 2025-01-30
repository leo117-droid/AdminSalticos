using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SalticosAdmin.Areas.Admin.Controllers;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SalticosAdmin.Utilidades;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class TareaControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private TareaController _controller;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new TareaController(_unidadTrabajoMock.Object);

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
            var modelo = (Tarea)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0));
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var tareaId = 1;
            var tarea = new Tarea
            {
                Id = tareaId,
                Titulo = "Llamar al mecánico",
                Descripcion = "Llamar al mecánico antes de las 10am",
                Estado = "Pendiente",
                Prioridad = "Alta",
                Fecha = DateTime.Now.AddDays(-1)
            };
            _unidadTrabajoMock.Setup(u => u.Tareas.Obtener(tareaId)).ReturnsAsync(tarea);

            var resultado = await _controller.Upsert(tareaId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Tarea)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(tareaId));
        }

        // Verifica que el método Upsert cree un nueva tarea cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaTarea()
        {
            var tarea = new Tarea
            {
                Id = 0,
                Titulo = "Recoger paquete en el correo",
                Descripcion = "Recoger paquete de tips en el correo",
                Estado = "Pendiente",
                Prioridad = "Alta",
                Fecha = DateTime.Now
            };

            _unidadTrabajoMock.Setup(u => u.Tareas.Agregar(It.IsAny<Tarea>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Upsert(tarea);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.Tareas.Agregar(It.IsAny<Tarea>()), Times.Once);
        }

        // Verifica que el método Upsert actualiza una tarea existente.
        [Test]
        public async Task Upsert_Post_ActualizaTarea()
        {
            var tareaId = 1;
            var tareaExistente = new Tarea
            {
                Id = tareaId,
                Titulo = "Recoger camisas negras para empleados",
                Descripcion = "Recoger 3 camisas tallas M y 1 talla L",
                Estado = "Pendiente",
                Prioridad = "Media",
                Fecha = DateTime.Now.AddDays(-1)
            };

            var tareaActualizada = new Tarea
            {
                Id = tareaId,
                Titulo = "Recoger camisas negras para empleados",
                Descripcion = "Recoger 3 camisas tallas M y 2 talla L",
                Estado = "En Progreso",
                Prioridad = "Alta",
                Fecha = DateTime.Now
            };

            _unidadTrabajoMock.Setup(u => u.Tareas.Obtener(tareaId)).ReturnsAsync(tareaExistente);
            _unidadTrabajoMock.Setup(u => u.Tareas.Actualizar(It.IsAny<Tarea>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Upsert(tareaActualizada);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.Tareas.Actualizar(It.IsAny<Tarea>()), Times.Once);
        }

        // Verifica que el método Delete elimine correctamente una tarea.
        [Test]
        public async Task Delete_RetornaExito_CuandoLaTareaExiste()
        {
            var tareaId = 1;
            var tareaBd = new Tarea
            {
                Id = tareaId,
                Titulo = "Llamar a la contadora"
            };

            _unidadTrabajoMock.Setup(u => u.Tareas.Obtener(tareaId)).ReturnsAsync(tareaBd);
            _unidadTrabajoMock.Setup(u => u.Tareas.Remover(It.IsAny<Tarea>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(tareaId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("true"));
            _unidadTrabajoMock.Verify(u => u.Tareas.Remover(It.IsAny<Tarea>()), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar una tarea que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoLaTareaNoExiste()
        {
            var tareaId = 999;
            Tarea tareaBd = null;

            _unidadTrabajoMock.Setup(u => u.Tareas.Obtener(tareaId)).ReturnsAsync(tareaBd);

            var resultado = await _controller.Delete(tareaId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Tarea no encontrada"));
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
