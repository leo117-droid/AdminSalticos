using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SalticosAdmin.Areas.Admin.Controllers;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SalticosAdmin.Utilidades;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;


namespace SalticosAdminTest
{

    [TestFixture]
    public class EventoInflableControllerTest
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _mockUnidadTrabajo;
        private EventoInflableController _controller;


        [SetUp]
        public void SetUp()
        {
            _mockUnidadTrabajo = new Mock<IUnidadTrabajo>();
            _controller = new EventoInflableController(_mockUnidadTrabajo.Object);

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

            _mockUnidadTrabajo.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

        }

        // Verifica que el método Index retorne una vista correctamente.
        [Test]
        public void Index_ConId_RetornaVista()
        {
            
            int? id = 1;

            var result = _controller.Index(id) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(id, Is.EqualTo(result.ViewData["Id"]));
        }

        //Verifica el controlador sigue funciona correctamente y proporciona la lista de inflable esperada en la vista.
        [Test]
        public async Task Upsert_ConRelacionIdEsNull_RetornVistaConViewModel()
        {
            
            int eventoID = 1;
            int? relacionId = null;

            var mockInflableList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Tsunami", Value = "1" },
                new SelectListItem { Text = "Gladiador", Value = "2" }
            };

            _mockUnidadTrabajo
                .Setup(u => u.EventoInflable.ObtenerInflable("Inflable", eventoID))
                .Returns(mockInflableList);

            
            var result = await _controller.Upsert(eventoID, relacionId) as ViewResult;
            var model = result.Model as EventoInflableVM;

            
            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(eventoID, Is.EqualTo(model.IdEvento));
            Assert.That(mockInflableList, Is.EqualTo(model.ListaInflable));
            
        }

        //Asegura que cuando el modelo es inválido, el controlador responde correctamente, mostrando la vista con los errores de validación
        [Test]
        public async Task Upsert_PostModeloInvalido_ReturnsVistaConErrores()
        {
            
            var viewModel = new EventoInflableVM { IdEvento = 1 };
            _controller.ModelState.AddModelError("IdInflable", "El campo es requerido.");

            var result = await _controller.Upsert(viewModel) as ViewResult;
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(viewModel));
            Assert.That(_controller.ModelState.IsValid, Is.False);
        }

        // Verifica que el método Upsert cree un nuevo evento inflable cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_PostModeloValido_CreaNuevoEventoInflable()
        {

            var eventoInflableVM = new EventoInflableVM
            {
                IdEvento = 1,
                IdRelacion = 0,
                IdInflable = 1
            };
            
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("fake-url");
            _controller.Url = urlHelperMock.Object;

            _mockUnidadTrabajo
                .Setup(u => u.EventoInflable.ObtenerPrimero(It.IsAny<Expression<Func<EventoInflable, bool>>>(), null, false))
                .ReturnsAsync((EventoInflable)null);

            _mockUnidadTrabajo
                .Setup(u => u.Evento.ObtenerPrimero(It.IsAny<Expression<Func<Evento, bool>>>(), null, true))
                .ReturnsAsync(new Evento
                {
                    Id = 1,
                    Fecha = DateTime.Now,
                    ClienteId = 2
                });

            _mockUnidadTrabajo
                .Setup(u => u.Inflable.Obtener(It.IsAny<int>()))
                .ReturnsAsync(new Inflable
                {
                    Id = 1,
                    Nombre = "Avengers"
                });

            _mockUnidadTrabajo.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var tempDataMock = new Mock<ITempDataDictionary>();
            _controller.TempData = tempDataMock.Object;

            var result = await _controller.Upsert(eventoInflableVM);

            tempDataMock.VerifySet(tempData => tempData[DS.Exitosa] = "Inflable agregado exitosamente", Times.Once);
            _mockUnidadTrabajo.Verify(u => u.EventoInflable.Agregar(It.IsAny<EventoInflable>()), Times.Once);
            _mockUnidadTrabajo.Verify(u => u.Guardar(), Times.Once);
            Assert.That(result, Is.InstanceOf<RedirectResult>());
        }


        // Verifica que el método Delete elimine correctamente un producto de inflable de un evento.
        [Test]
        public async Task Delete_Post_EliminaEventoInflable_Exito()
        {
            int idEventoInflable = 1;
            var eventoInflable = new EventoInflable
            {
                Id = idEventoInflable,
                IdEvento = 1,
                IdInflable = 1
            };

            var evento = new Evento
            {
                Id = 1,
                Fecha = DateTime.Now,
                HoraInicio = DateTime.Now.TimeOfDay,
                ClienteId = 2
            };

            var inflable = new Inflable
            {
                Id = 1,
                Nombre = "Casa club"
            };


            _mockUnidadTrabajo
                .Setup(u => u.EventoInflable.Obtener(idEventoInflable))
                .ReturnsAsync(eventoInflable);

            
            _mockUnidadTrabajo
                .Setup(u => u.EventoInflable.Remover(It.IsAny<EventoInflable>()))
                .Verifiable();

            _mockUnidadTrabajo.Setup(u => u.Guardar()).Returns(Task.CompletedTask);


            _mockUnidadTrabajo
                .Setup(u => u.Evento.Obtener(It.IsAny<int>()))
                .ReturnsAsync(evento);

            _mockUnidadTrabajo
                .Setup(u => u.Inflable.Obtener(It.IsAny<int>()))
                .ReturnsAsync(inflable);

            _mockUnidadTrabajo
                .Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);


            var result = await _controller.Delete(idEventoInflable);

            _mockUnidadTrabajo.Verify(u => u.EventoInflable.Remover(It.IsAny<EventoInflable>()), Times.Once);
            _mockUnidadTrabajo.Verify(u => u.Guardar(), Times.Once);

            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("true"));
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Inflable borrado del evento exitosamente"));

        }

        // Verifica que se retorne un error cuando se intenta eliminar un evento alimentación que no existe.
        [Test]
        public async Task Delete_CuandoEventoInflableNoExiste_RetornaError()
        {
            
            int id = 999;

            _mockUnidadTrabajo.Setup(u => u.EventoInflable.Obtener(id)).ReturnsAsync((EventoInflable)null);

            var result = await _controller.Delete(id);

            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("false"));
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar inflable del evento"));
        }

        [TearDown]
        public void TearDown()
        {
            // Limpia las configuraciones y recursos después de cada prueba.

            _mockUnidadTrabajo.Reset();
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
