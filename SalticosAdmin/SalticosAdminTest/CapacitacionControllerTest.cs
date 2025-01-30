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
using System.Linq.Expressions;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class CapacitacionControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private CapacitacionController _controller;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new CapacitacionController(_unidadTrabajoMock.Object);
            
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
            var modelo = (Capacitacion)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0)); 
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var capacitacionId = 1;
            var capacitacion = new Capacitacion 
            { 
                Id = capacitacionId,
                Fecha = DateTime.Today.AddDays(1),
                Tema = "Limpieza de las máquinas de burbujas",
                Duracion = "2 horas" 
            };
            _unidadTrabajoMock.Setup(u => u.Capacitacion.Obtener(capacitacionId)).ReturnsAsync(capacitacion);

            var resultado = await _controller.Upsert(capacitacionId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Capacitacion)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);  
            Assert.That(modelo.Id, Is.EqualTo(capacitacionId)); 
        }


        // Verifica que el método Upsert cree una nueva capacitación cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaCapacitacion()
        {
            var capacitacion = new Capacitacion
            {
                Id = 0, 
                Fecha = DateTime.Today.AddDays(2), 
                Tema = "Arreglos para los motores de los inflables",
                Duracion = "3 horas"
            };

            _unidadTrabajoMock.Setup(u => u.Capacitacion.Agregar(It.IsAny<Capacitacion>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Upsert(capacitacion);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); 
            _unidadTrabajoMock.Verify(u => u.Capacitacion.Agregar(It.IsAny<Capacitacion>()), Times.Once);
        }

        // Verifica que el método Upsert actualice una capacitación existente.
        [Test]
        public async Task Upsert_Post_ActualizaCapacitacion()
        {
            var capacitacionId = 1;
            var capacitacionExistente = new Capacitacion
            {
                Id = capacitacionId,
                Fecha = DateTime.Today.AddDays(2),
                Tema = "Cambio de llanta",
                Duracion = "2 horas"
            };

            var capacitacionActualizada = new Capacitacion
            {
                Id = capacitacionId,
                Fecha = DateTime.Today.AddDays(2), 
                Tema = "Cambio de llanta",
                Duracion = "2 horas y media"
            };

            _unidadTrabajoMock.Setup(u => u.Capacitacion.Obtener(capacitacionId)).ReturnsAsync(capacitacionExistente);
            _unidadTrabajoMock.Setup(u => u.Capacitacion.Actualizar(It.IsAny<Capacitacion>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Upsert(capacitacionActualizada);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); 
            _unidadTrabajoMock.Verify(u => u.Capacitacion.Actualizar(It.IsAny<Capacitacion>()), Times.Once); 
        }

        // Verifica que una capacitación sin personal asociado sea eliminada correctamente.
        [Test]
        public async Task Delete_RetornaExito_CuandoLaCapacitacionNoTienePersonalAsociado()
        {
            var capacitacionId = 1;  
            var capacitacionBd = new Capacitacion
            {
                Fecha = DateTime.Today.AddDays(7),
                Tema = "Cambio de aceite en camiones pequeños",
                Duracion = "1 hora"
            };

            _unidadTrabajoMock.Setup(u => u.Capacitacion.Obtener(capacitacionId)).ReturnsAsync(capacitacionBd);

            _unidadTrabajoMock.Setup(u => u.CapacitacionPersonal.ObtenerTodos(It.IsAny<Expression<Func<CapacitacionPersonal, bool>>>(), null, null, true))
                .ReturnsAsync(new List<CapacitacionPersonal>());

            _unidadTrabajoMock.Setup(u => u.Capacitacion.Remover(It.IsAny<Capacitacion>())).Verifiable();

            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(capacitacionId);

            _unidadTrabajoMock.Setup(u => u.Capacitacion.Obtener(capacitacionId)).ReturnsAsync((Capacitacion)null);
            var capacitacionEliminada = await _unidadTrabajoMock.Object.Capacitacion.Obtener(capacitacionId);
            Assert.That(capacitacionEliminada, Is.Null);  
        }

        // Verifica que se retorne un error cuando se intenta eliminar una capacitación que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoLaCapacitacionNoExiste()
        {
            var capacitacionId = 999;  
            Capacitacion capacitacionBd = null;  

            _unidadTrabajoMock.Setup(u => u.Capacitacion.Obtener(capacitacionId)).ReturnsAsync(capacitacionBd);

            var resultado = await _controller.Delete(capacitacionId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());  
            var jsonResult = resultado as JsonResult;

            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar capacitacion"));  // Verifica que el mensaje sea el esperado
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

