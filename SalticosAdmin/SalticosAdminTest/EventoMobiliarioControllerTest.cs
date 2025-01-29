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
    public class EventoMobiliarioControllerTest
    {

        private Mock<IUnidadTrabajo> _mockUnidadTrabajo;
        private EventoMobiliarioController _controller;


        [SetUp]
        public void SetUp()
        {
            _mockUnidadTrabajo = new Mock<IUnidadTrabajo>();
            _controller = new EventoMobiliarioController(_mockUnidadTrabajo.Object);

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

            var mockMobiliarioList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Mesa", Value = "1" },
                new SelectListItem { Text = "Silla", Value = "2" }
            };

            _mockUnidadTrabajo
                .Setup(u => u.EventoMobiliario.ObtenerMobiliario("Mobiliario", eventoID))
                .Returns(mockMobiliarioList);

            var result = await _controller.Upsert(eventoID, relacionId) as ViewResult;
            var model = result.Model as EventoMobiliarioVM;

            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(eventoID, Is.EqualTo(model.IdEvento));
            Assert.That(mockMobiliarioList, Is.EqualTo(model.ListaMobiliario));
        }

        [Test]
        public async Task Upsert_PostModeloInvalido_ReturnsVistaConErrores()
        {
            var viewModel = new EventoMobiliarioVM { IdEvento = 1 };
            _controller.ModelState.AddModelError("IdMobiliario", "El campo es requerido.");

            var result = await _controller.Upsert(viewModel) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(viewModel));
            Assert.That(_controller.ModelState.IsValid, Is.False);
        }

        [Test]
        public async Task Upsert_PostModeloValido_CreaNuevoEventoMobiliario()
        {
            var eventoMobiliarioVM = new EventoMobiliarioVM
            {
                IdEvento = 1,
                IdRelacion = 0,
                IdMobiliario = 1,
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
                .Setup(u => u.Mobiliario.ObtenerPrimero(It.IsAny<Expression<Func<Mobiliario, bool>>>(), null, true))
                .ReturnsAsync(new Mobiliario { Id = 1, Nombre = "Silla", Inventario = 10 });

            _mockUnidadTrabajo
                .Setup(u => u.Mobiliario.Obtener(It.IsAny<int>()))
                .ReturnsAsync(new Mobiliario { Id = 1, Nombre = "Silla", Inventario = 10 });

            _mockUnidadTrabajo
                .Setup(u => u.EventoMobiliario.ObtenerPrimero(It.IsAny<Expression<Func<EventoMobiliario, bool>>>(), null, true))
                .ReturnsAsync((EventoMobiliario)null);

            _mockUnidadTrabajo.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);


            var tempDataMock = new Mock<ITempDataDictionary>();
            _controller.TempData = tempDataMock.Object;

            var result = await _controller.Upsert(eventoMobiliarioVM);

            tempDataMock.VerifySet(tempData => tempData[DS.Exitosa] = "Mobiliario agregado exitosamente", Times.Once);
            _mockUnidadTrabajo.Verify(u => u.EventoMobiliario.Agregar(It.IsAny<EventoMobiliario>()), Times.Once);
            _mockUnidadTrabajo.Verify(u => u.Guardar(), Times.Once);
            Assert.That(result, Is.InstanceOf<RedirectResult>());
        }

        [Test]
        public async Task Upsert_PostModeloValido_ActualizaEventoMobiliarioExistente()
        {
            var eventoMobiliarioVM = new EventoMobiliarioVM
            {
                IdEvento = 1,
                IdRelacion = 5,
                IdMobiliario = 1,
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
                .Setup(u => u.Mobiliario.ObtenerPrimero(It.IsAny<Expression<Func<Mobiliario, bool>>>(), null, true))
                .ReturnsAsync(new Mobiliario { Id = 1, Nombre = "Mesa", Inventario = 10 });


            _mockUnidadTrabajo
                .Setup(u => u.Mobiliario.Obtener(It.IsAny<int>()))
                .ReturnsAsync(new Mobiliario { Id = 1, Nombre = "Mesa", Inventario = 10 });

            _mockUnidadTrabajo
                .Setup(u => u.EventoMobiliario.ObtenerPrimero(It.IsAny<Expression<Func<EventoMobiliario, bool>>>(), null, true))
                .ReturnsAsync(new EventoMobiliario { Id = 5, IdEvento = 1, IdMobiliario = 1, Cantidad = 2 });

            _mockUnidadTrabajo
                .Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var tempDataMock = new Mock<ITempDataDictionary>();
            _controller.TempData = tempDataMock.Object;

            var result = await _controller.Upsert(eventoMobiliarioVM);

            tempDataMock.VerifySet(tempData => tempData[DS.Exitosa] = "Evento actualizado exitosamente", Times.Once);
            _mockUnidadTrabajo.Verify(u => u.EventoMobiliario.Actualizar(It.IsAny<EventoMobiliario>()), Times.Once);
            _mockUnidadTrabajo.Verify(u => u.Guardar(), Times.Once);
            Assert.That(result, Is.InstanceOf<RedirectResult>());
        }


        [Test]
        public async Task Upsert_PostModeloValido_ConInventarioInsuficiente_NoActualizaYRetornaError()
        {
            var eventoMobiliarioVM = new EventoMobiliarioVM
            {
                IdEvento = 1,
                IdRelacion = 5,
                IdMobiliario = 1,
                Cantidad = 15
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
                .Setup(u => u.Mobiliario.ObtenerPrimero(It.IsAny<Expression<Func<Mobiliario, bool>>>(), null, true))
                .ReturnsAsync(new Mobiliario { Id = 1, Nombre = "Mesa", Inventario = 10 }); // Inventario insuficiente


            _mockUnidadTrabajo
                .Setup(u => u.Mobiliario.Obtener(It.IsAny<int>()))
                .ReturnsAsync(new Mobiliario { Id = 1, Nombre = "Mesa", Inventario = 10 });

            _mockUnidadTrabajo
                .Setup(u => u.EventoMobiliario.ObtenerPrimero(It.IsAny<Expression<Func<EventoMobiliario, bool>>>(), null, true))
                .ReturnsAsync(new EventoMobiliario { Id = 5, IdEvento = 1, IdMobiliario = 1, Cantidad = 2 });

            _mockUnidadTrabajo
                .Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());


            var result = await _controller.Upsert(eventoMobiliarioVM);

            Assert.That(_controller.TempData[DS.Error].ToString(), Does.Contain("No hay suficiente mobiliario disponible"),
                "El mensaje de error no contiene el texto esperado.");
            _mockUnidadTrabajo.Verify(u => u.EventoMobiliario.Actualizar(It.IsAny<EventoMobiliario>()), Times.Never);
            _mockUnidadTrabajo.Verify(u => u.Guardar(), Times.Never);
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }


        [Test]
        public async Task Delete_Post_EliminaEventoMobiliario_Exito()
        {
            int idEventoMobiliario = 1;
            var eventoMobiliario = new EventoMobiliario { Id = idEventoMobiliario, IdEvento = 1, IdMobiliario = 1 };


            var evento = new Evento
            {
                Id = 1,
                Fecha = DateTime.Now,
                HoraInicio = DateTime.Now.TimeOfDay,
                ClienteId = 2
            };

            var inflable = new Mobiliario
            {
                Id = 1,
                Nombre = "Silla"
            };


            _mockUnidadTrabajo
                .Setup(u => u.EventoMobiliario.Obtener(idEventoMobiliario))
                .ReturnsAsync(eventoMobiliario);

            _mockUnidadTrabajo
                .Setup(u => u.EventoMobiliario.Remover(It.IsAny<EventoMobiliario>()))
                .Verifiable();


            _mockUnidadTrabajo
                .Setup(u => u.Evento.Obtener(It.IsAny<int>()))
                .ReturnsAsync(evento);

            _mockUnidadTrabajo
                .Setup(u => u.Mobiliario.Obtener(It.IsAny<int>()))
                .ReturnsAsync(inflable);

            _mockUnidadTrabajo
                .Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);


            _mockUnidadTrabajo.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Delete(idEventoMobiliario);

            _mockUnidadTrabajo.Verify(u => u.EventoMobiliario.Remover(It.IsAny<EventoMobiliario>()), Times.Once);
            _mockUnidadTrabajo.Verify(u => u.Guardar(), Times.Once);

            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("true"));
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Mobiliario borrado del evento exitosamente"));
        }

        [Test]
        public async Task Delete_CuandoEventoMobiliarioNoExiste_RetornaError()
        {
            int id = 999;

            _mockUnidadTrabajo.Setup(u => u.EventoMobiliario.Obtener(id)).ReturnsAsync((EventoMobiliario)null);

            var result = await _controller.Delete(id);

            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("false"));
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar mobiliario del evento"));
        }



    }
}
