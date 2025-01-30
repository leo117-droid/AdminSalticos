using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SalticosAdmin.Areas.Admin.Controllers;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SalticosAdmin.Utilidades;
using Microsoft.AspNetCore.Hosting;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;


namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class EventoControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private EventoController _controller;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new EventoController(_unidadTrabajoMock.Object, Mock.Of<IWebHostEnvironment>());

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
            _unidadTrabajoMock.Setup(u => u.Evento.ObtenerTodos(It.IsAny<Expression<Func<Evento, bool>>>(), null, null,true)).ReturnsAsync(new List<Evento>());

            var resultado = await _controller.Index();

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (EventoVM)((ViewResult)resultado).Model;
            Assert.That(modelo.EventoLista, Is.Not.Null);
        }

        // Verifica que, cuando el ID es null, el método Upsert retorne una vista con un modelo vacío.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloVacioCuandoIdEsNull()
        {
            _unidadTrabajoMock.Setup(u => u.Evento.ObtenerTodosDropdownLista("Cliente")).Returns(new List<SelectListItem>());

            var resultado = await _controller.Upsert((int?)null);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (EventoVM)((ViewResult)resultado).Model;
            Assert.That(modelo.Evento.Id, Is.EqualTo(0));
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var eventoId = 1;
            var evento = new Evento { Id = eventoId, ClienteId = 1, HoraInicio = DateTime.Now.TimeOfDay, HoraFinal = DateTime.Now.AddHours(1).TimeOfDay };
            _unidadTrabajoMock.Setup(u => u.Evento.Obtener(eventoId)).ReturnsAsync(evento);
            _unidadTrabajoMock.Setup(u => u.Evento.ObtenerTodosDropdownLista("Cliente")).Returns(new List<SelectListItem>());

            var resultado = await _controller.Upsert(eventoId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (EventoVM)((ViewResult)resultado).Model;
            Assert.That(modelo.Evento.Id, Is.EqualTo(eventoId));
        }

        // Verifica que el método Upsert cree un nuevo evento cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaEvento()
        {
            var eventoVM = new EventoVM
            {
                Evento = new Evento
                {
                    Id = 0,
                    ClienteId = 1,
                    Fecha = DateTime.Now,
                    HoraInicio = DateTime.Now.TimeOfDay, 
                    HoraFinal = DateTime.Now.AddHours(1).TimeOfDay 
                },
                ClienteLista = new List<SelectListItem>()
            };

            var cliente = new Cliente { Id = 1, Nombre = "Samira", Apellidos = "Rodriguez" };

            _unidadTrabajoMock.Setup(u => u.Evento.Agregar(It.IsAny<Evento>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Evento.ObtenerTodosDropdownLista("Cliente")).Returns(new List<SelectListItem>());
            _unidadTrabajoMock.Setup(u => u.Cliente.Obtener(eventoVM.Evento.ClienteId)).ReturnsAsync(cliente);

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("fake-url");

            _controller.Url = urlHelperMock.Object;

            var resultado = await _controller.Upsert(eventoVM);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.Evento.Agregar(It.IsAny<Evento>()), Times.Once);
        }

        // Verifica que el método Upsert actualice un evento existente.
        [Test]
        public async Task Upsert_Post_ActualizaEvento()
        {
            var eventoId = 1;
            var eventoExistente = new Evento
            {
                Id = eventoId,
                ClienteId = 1,
                Fecha = DateTime.Now,
                HoraInicio = DateTime.Now.TimeOfDay, 
                HoraFinal = DateTime.Now.AddHours(1).TimeOfDay
            };

            var eventoVM = new EventoVM
            {
                Evento = eventoExistente,
                ClienteLista = new List<SelectListItem>()
            };

            var cliente = new Cliente { Id = 1, Nombre = "Nombre", Apellidos = "Apellidos" };

            _unidadTrabajoMock.Setup(u => u.Evento.Obtener(eventoId)).ReturnsAsync(eventoExistente);
            _unidadTrabajoMock.Setup(u => u.Evento.ObtenerTodosDropdownLista("Cliente")).Returns(new List<SelectListItem>());
            _unidadTrabajoMock.Setup(u => u.Cliente.Obtener(eventoVM.Evento.ClienteId)).ReturnsAsync(cliente);
            _unidadTrabajoMock.Setup(u => u.Evento.Actualizar(It.IsAny<Evento>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(eventoVM);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.Evento.Actualizar(It.IsAny<Evento>()), Times.Once);
        }

        // Verifica que el método Delete elimine correctamente un evento.
        [Test]
        public async Task Delete_RetornaExito_CuandoElEventoExiste()
        {
            var eventoId = 1;
            var evento = new Evento { Id = eventoId, ClienteId = 1, Fecha = System.DateTime.Now };
            var cliente = new Cliente { Id = 1, Nombre = "Sara", Apellidos = "Medina" };

            _unidadTrabajoMock.Setup(u => u.Evento.Obtener(eventoId)).ReturnsAsync(evento);
            _unidadTrabajoMock.Setup(u => u.Cliente.Obtener(evento.ClienteId)).ReturnsAsync(cliente);

            _unidadTrabajoMock.Setup(u => u.EventoAlimentacion.ObtenerTodos(It.IsAny<Expression<Func<EventoAlimentacion, bool>>>(), null, null, true)).ReturnsAsync(new List<EventoAlimentacion>());
            _unidadTrabajoMock.Setup(u => u.EventoVehiculo.ObtenerTodos(It.IsAny<Expression<Func<EventoVehiculo, bool>>>(),null,null,true)).ReturnsAsync(new List<EventoVehiculo>());
            _unidadTrabajoMock.Setup(u => u.EventoMobiliario.ObtenerTodos(It.IsAny<Expression<Func<EventoMobiliario, bool>>>(),null,null,true)).ReturnsAsync(new List<EventoMobiliario>());
            _unidadTrabajoMock.Setup(u => u.EventoPersonal.ObtenerTodos(It.IsAny<Expression<Func<EventoPersonal, bool>>>(),null,null,true)).ReturnsAsync(new List<EventoPersonal>());
            _unidadTrabajoMock.Setup(u => u.EventoInflable.ObtenerTodos(It.IsAny<Expression<Func<EventoInflable, bool>>>(),null,null,true)).ReturnsAsync(new List<EventoInflable>());
            _unidadTrabajoMock.Setup(u => u.EventoServicioAdicional.ObtenerTodos(It.IsAny<Expression<Func<EventoServicioAdicional, bool>>>(),null,null,true)).ReturnsAsync(new List<EventoServicioAdicional>());

            _unidadTrabajoMock.Setup(u => u.Evento.Remover(It.IsAny<Evento>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(eventoId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString(), Is.EqualTo("True"));
            _unidadTrabajoMock.Verify(u => u.Evento.Remover(It.IsAny<Evento>()), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar un evento que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoElEventoNoExiste()
        {
            var eventoId = 999;
            Evento evento = null;

            _unidadTrabajoMock.Setup(u => u.Evento.Obtener(eventoId)).ReturnsAsync(evento);

            var resultado = await _controller.Delete(eventoId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Evento"));
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
