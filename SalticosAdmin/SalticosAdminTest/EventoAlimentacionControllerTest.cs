using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Newtonsoft.Json.Linq;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Areas.Admin.Controllers;
using SalticosAdmin.Modelos.ViewModels;
using SalticosAdmin.Modelos;
using SalticosAdmin.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SalticosAdminTest
{
    public class EventoAlimentacionControllerTest
    {

        private Mock<IUnidadTrabajo> _mockUnidadTrabajo;
        private EventoAlimentacionController _controller;


        [SetUp]
        public void SetUp()
        {
            _mockUnidadTrabajo = new Mock<IUnidadTrabajo>();
            _controller = new EventoAlimentacionController(_mockUnidadTrabajo.Object);

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

        [Test]
        public void Index_ConId_RetornaVista()
        {
            int? id = 1;

            var result = _controller.Index(id) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(id, Is.EqualTo(result.ViewData["Id"]));
        }

        [Test]
        public async Task Upsert_ConRelacionIdEsNull_RetornVistaConViewModel()
        {
            int eventoID = 1;
            int? relacionId = null;

            var mockeventoList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Algodón de azúcar", Value = "1" },
                new SelectListItem { Text = "Churro", Value = "2" }
            };

            _mockUnidadTrabajo
                .Setup(u => u.EventoAlimentacion.ObtenerAlimentacion("Alimentacion", eventoID))
                .Returns(mockeventoList);

            var result = await _controller.Upsert(eventoID, relacionId) as ViewResult;
            var model = result.Model as EventoAlimentacionVM;

            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(eventoID, Is.EqualTo(model.IdEvento));
            Assert.That(mockeventoList, Is.EqualTo(model.ListaAlimentacion));
        }

        [Test]
        public async Task Upsert_PostModeloInvalido_ReturnsVistaConErrores()
        {
            var viewModel = new EventoAlimentacionVM { IdEvento = 1 };
            _controller.ModelState.AddModelError("IdAlimentacion", "El campo es requerido.");

            var result = await _controller.Upsert(viewModel) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(viewModel));
            Assert.That(_controller.ModelState.IsValid, Is.False);
        }

        [Test]
        public async Task Upsert_PostModeloValido_CreaNuevoEventoAlimentacion()
        {
            var eventoAlimentacionVM = new EventoAlimentacionVM
            {
                IdEvento = 1,
                IdRelacion = 0,
                IdAlimentacion = 1,
                Cantidad = 2
            };

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("fake-url");
            _controller.Url = urlHelperMock.Object;

            _mockUnidadTrabajo
                .Setup(u => u.Evento.ObtenerPrimero(It.IsAny<Expression<Func<Evento, bool>>>(), null, true))
                .ReturnsAsync(new Evento { Id = 1, Fecha = DateTime.Now });
            _mockUnidadTrabajo
                .Setup(u => u.Alimentacion.ObtenerPrimero(It.IsAny<Expression<Func<Alimentacion, bool>>>(), null, true))
                .ReturnsAsync(new Alimentacion { Id = 1, Nombre = "Palomitas" });

            _mockUnidadTrabajo
                .Setup(u => u.Alimentacion.Obtener(It.IsAny<int>()))
                .ReturnsAsync(new Alimentacion { Id = 1, Nombre = "Palomitas" });

            _mockUnidadTrabajo
                .Setup(u => u.EventoAlimentacion.ObtenerPrimero(It.IsAny<Expression<Func<EventoAlimentacion, bool>>>(), null, true))
                .ReturnsAsync((EventoAlimentacion)null);

            _mockUnidadTrabajo.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);


            var tempDataMock = new Mock<ITempDataDictionary>();
            _controller.TempData = tempDataMock.Object;

            var result = await _controller.Upsert(eventoAlimentacionVM);

            tempDataMock.VerifySet(tempData => tempData[DS.Exitosa] = "Alimentación agregado exitosamente", Times.Once);
            _mockUnidadTrabajo.Verify(u => u.EventoAlimentacion.Agregar(It.IsAny<EventoAlimentacion>()), Times.Once);
            _mockUnidadTrabajo.Verify(u => u.Guardar(), Times.Once);
            Assert.That(result, Is.InstanceOf<RedirectResult>());
        }



        [Test]
        public async Task Upsert_PostModeloValido_ActualizaEventoAlimentacionExistente()
        {
            var eventoAlimentacionVM = new EventoAlimentacionVM
            {
                IdEvento = 1,
                IdRelacion = 5,
                IdAlimentacion = 1,
                Cantidad = 3
            };

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("fake-url");
            _controller.Url = urlHelperMock.Object;

            _mockUnidadTrabajo
                .Setup(u => u.Evento.ObtenerPrimero(It.IsAny<Expression<Func<Evento, bool>>>(), null, true))
                .ReturnsAsync(new Evento
                {
                    Id = 1,
                    Fecha = DateTime.Now,
                    HoraInicio = DateTime.Now.TimeOfDay,
                });

            _mockUnidadTrabajo
                .Setup(u => u.Alimentacion.ObtenerPrimero(It.IsAny<Expression<Func<Alimentacion, bool>>>(), null, true))
                .ReturnsAsync(new Alimentacion { Id = 1, Nombre = "Pizza"});


            _mockUnidadTrabajo
                .Setup(u => u.Alimentacion.Obtener(It.IsAny<int>()))
                .ReturnsAsync(new Alimentacion { Id = 1, Nombre = "Pizza"});

            _mockUnidadTrabajo
                .Setup(u => u.EventoAlimentacion.ObtenerPrimero(It.IsAny<Expression<Func<EventoAlimentacion, bool>>>(), null, true))
                .ReturnsAsync(new EventoAlimentacion { Id = 5, IdEvento = 1, IdAlimentacion = 1, Cantidad = 2 });

            _mockUnidadTrabajo
                .Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var tempDataMock = new Mock<ITempDataDictionary>();
            _controller.TempData = tempDataMock.Object;

            var result = await _controller.Upsert(eventoAlimentacionVM);

            tempDataMock.VerifySet(tempData => tempData[DS.Exitosa] = "Evento actualizado exitosamente", Times.Once);
            _mockUnidadTrabajo.Verify(u => u.EventoAlimentacion.Actualizar(It.IsAny<EventoAlimentacion>()), Times.Once);
            _mockUnidadTrabajo.Verify(u => u.Guardar(), Times.Once);
            Assert.That(result, Is.InstanceOf<RedirectResult>());
        }






        [Test]
        public async Task Delete_Post_EliminaEventoAlimentacion_Exito()
        {
            int IdEventoAlimentacion = 1;
            var eventoAlimentacion = new EventoAlimentacion 
            {
                Id = IdEventoAlimentacion, 
                IdEvento = 1, 
                IdAlimentacion = 1 
            };


            var evento = new Evento
            {
                Id = 1,
                Fecha = DateTime.Now,
                HoraInicio = DateTime.Now.TimeOfDay,
                ClienteId = 2
            };

            var alimentacion = new Alimentacion
            {
                Id = 1,
                Nombre = "Hot dog"
            };


            _mockUnidadTrabajo
                .Setup(u => u.EventoAlimentacion.Obtener(IdEventoAlimentacion))
                .ReturnsAsync(eventoAlimentacion);

            _mockUnidadTrabajo
                .Setup(u => u.EventoAlimentacion.Remover(It.IsAny<EventoAlimentacion>()))
                .Verifiable();


            _mockUnidadTrabajo
                .Setup(u => u.Evento.Obtener(It.IsAny<int>()))
                .ReturnsAsync(evento);

            _mockUnidadTrabajo
                .Setup(u => u.Alimentacion.Obtener(It.IsAny<int>()))
                .ReturnsAsync(alimentacion);

            _mockUnidadTrabajo
                .Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);


            _mockUnidadTrabajo.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Delete(IdEventoAlimentacion);

            _mockUnidadTrabajo.Verify(u => u.EventoAlimentacion.Remover(It.IsAny<EventoAlimentacion>()), Times.Once);
            _mockUnidadTrabajo.Verify(u => u.Guardar(), Times.Once);

            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("true"));
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Alimentación borrado del evento exitosamente"));
        }

        [Test]
        public async Task Delete_CuandoEventoAlimentacionNoExiste_RetornaError()
        {
            int id = 999;

            _mockUnidadTrabajo.Setup(u => u.EventoAlimentacion.Obtener(id)).ReturnsAsync((EventoAlimentacion)null);

            var result = await _controller.Delete(id);

            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("false"));
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar alimentación del evento"));
        }


        [TearDown]
        public void TearDown()
        {
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
