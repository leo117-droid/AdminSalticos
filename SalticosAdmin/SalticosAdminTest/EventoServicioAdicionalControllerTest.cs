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
    public class EventoServicioAdicionalControllerTest
    {

        private Mock<IUnidadTrabajo> _mockUnidadTrabajo;
        private EventoServicioAdicionalController _controller;


        [SetUp]
        public void SetUp()
        {
            _mockUnidadTrabajo = new Mock<IUnidadTrabajo>();
            _controller = new EventoServicioAdicionalController(_mockUnidadTrabajo.Object);

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

            var mockServicioAdicionalList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Máquina de burbujas", Value = "1" },
                new SelectListItem { Text = "Sky dancer", Value = "2" }
            };

            _mockUnidadTrabajo
                .Setup(u => u.EventoServicioAdicional.ObtenerServicioAdicional("ServicioAdicional", eventoID))
                .Returns(mockServicioAdicionalList);

            var result = await _controller.Upsert(eventoID, relacionId) as ViewResult;
            var model = result.Model as EventoServicioAdicionalVM;

            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(eventoID, Is.EqualTo(model.IdEvento));
            Assert.That(mockServicioAdicionalList, Is.EqualTo(model.ListaServicioAdicional));
        }

        [Test]
        public async Task Upsert_PostModeloInvalido_ReturnsVistaConErrores()
        {
            var viewModel = new EventoServicioAdicionalVM { IdEvento = 1 };
            _controller.ModelState.AddModelError("IdServicioAdicional", "El campo es requerido.");

            var result = await _controller.Upsert(viewModel) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(viewModel));
            Assert.That(_controller.ModelState.IsValid, Is.False);
        }

        [Test]
        public async Task Upsert_PostModeloValido_CreaNuevoEventoServicioAdicional()
        {
            var eventoServicioAdicionalVM = new EventoServicioAdicionalVM
            {
                IdEvento = 1,
                IdRelacion = 0,
                IdServicioAdicional = 1,
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
                .Setup(u => u.ServicioAdicional.ObtenerPrimero(It.IsAny<Expression<Func<ServicioAdicional, bool>>>(), null, true))
                .ReturnsAsync(new ServicioAdicional { Id = 1, Nombre = "Parlante", Inventario = 10 });

            _mockUnidadTrabajo
                .Setup(u => u.ServicioAdicional.Obtener(It.IsAny<int>()))
                .ReturnsAsync(new ServicioAdicional { Id = 1, Nombre = "Parlante", Inventario = 10});

            _mockUnidadTrabajo
                .Setup(u => u.EventoServicioAdicional.ObtenerPrimero(It.IsAny<Expression<Func<EventoServicioAdicional, bool>>>(), null, true))
                .ReturnsAsync((EventoServicioAdicional)null);

            _mockUnidadTrabajo.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);


            var tempDataMock = new Mock<ITempDataDictionary>();
            _controller.TempData = tempDataMock.Object;

            var result = await _controller.Upsert(eventoServicioAdicionalVM);

            tempDataMock.VerifySet(tempData => tempData[DS.Exitosa] = "Servicio adicional agregado exitosamente", Times.Once);
            _mockUnidadTrabajo.Verify(u => u.EventoServicioAdicional.Agregar(It.IsAny<EventoServicioAdicional>()), Times.Once);
            _mockUnidadTrabajo.Verify(u => u.Guardar(), Times.Once);
            Assert.That(result, Is.InstanceOf<RedirectResult>());
        }

        [Test]
        public async Task Delete_Post_EliminaEventoEventoServicioAdicional_Exito()
        {
            int IdEventoServicioAdicional = 1;
            var eventoServicioAdicional = new EventoServicioAdicional
            {
                Id = IdEventoServicioAdicional, 
                IdEvento = 1, 
                IdServicioAdicional = 1 
            };


            var evento = new Evento
            {
                Id = 1,
                Fecha = DateTime.Now,
                HoraInicio = DateTime.Now.TimeOfDay,
                ClienteId = 2
            };

            var servicioAdicional = new ServicioAdicional
            {
                Id = 1,
                Nombre = "Basket"
            };


            _mockUnidadTrabajo
                .Setup(u => u.EventoServicioAdicional.Obtener(IdEventoServicioAdicional))
                .ReturnsAsync(eventoServicioAdicional);

            _mockUnidadTrabajo
                .Setup(u => u.EventoServicioAdicional.Remover(It.IsAny<EventoServicioAdicional>()))
                .Verifiable();


            _mockUnidadTrabajo
                .Setup(u => u.Evento.Obtener(It.IsAny<int>()))
                .ReturnsAsync(evento);

            _mockUnidadTrabajo
                .Setup(u => u.ServicioAdicional.Obtener(It.IsAny<int>()))
                .ReturnsAsync(servicioAdicional);

            _mockUnidadTrabajo
                .Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);


            _mockUnidadTrabajo.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Delete(IdEventoServicioAdicional);

            _mockUnidadTrabajo.Verify(u => u.EventoServicioAdicional.Remover(It.IsAny<EventoServicioAdicional>()), Times.Once);
            _mockUnidadTrabajo.Verify(u => u.Guardar(), Times.Once);

            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("true"));
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Servicio adicional borrado del evento exitosamente"));
        }

        [Test]
        public async Task Delete_CuandoEventoServicioAdicionalNoExiste_RetornaError()
        {
            int id = 999;

            _mockUnidadTrabajo.Setup(u => u.EventoServicioAdicional.Obtener(id)).ReturnsAsync((EventoServicioAdicional)null);

            var result = await _controller.Delete(id);

            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("false"));
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar servicio adicional del evento"));
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
