using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SalticosAdmin.Areas.Admin.Controllers;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SalticosAdmin.Utilidades;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminTest
{

    [TestFixture]
    public class EventoPersonalControllerTest
    {

        private Mock<IUnidadTrabajo> _mockUnidadTrabajo;
        private EventoPersonalController _controller;


        [SetUp]
        public void SetUp()
        {
            _mockUnidadTrabajo = new Mock<IUnidadTrabajo>();
            _controller = new EventoPersonalController(_mockUnidadTrabajo.Object);

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

            var mockPersonalList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Isabel Arias", Value = "1" },
                new SelectListItem { Text = "Jimena Madrigal", Value = "2" }
            };

            _mockUnidadTrabajo
                .Setup(u => u.EventoPersonal.ObtenerPersonal("Personal", eventoID))
                .Returns(mockPersonalList);

            
            var result = await _controller.Upsert(eventoID, relacionId) as ViewResult;
            var model = result.Model as EventoPersonalVM;

            
            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(eventoID, Is.EqualTo(model.IdEvento));
            Assert.That(mockPersonalList, Is.EqualTo(model.ListaPersonal));
            
        }


        [Test]
        public async Task Upsert_PostModeloInvalido_ReturnsVistaConErrores()
        {
            
            var viewModel = new EventoPersonalVM { IdEvento = 1 };
            _controller.ModelState.AddModelError("IdPersonal", "El campo es requerido.");

            var result = await _controller.Upsert(viewModel) as ViewResult;

            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(viewModel));
            Assert.That(_controller.ModelState.IsValid, Is.False);
        }



        [Test]
        public async Task Upsert_PostModeloValido_CreaNuevoEventoPersonal()
        {

            var eventoPersonalVM = new EventoPersonalVM
            {
                IdEvento = 1,
                IdRelacion = 0,
                IdPersonal = 1
            };
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("fake-url");
            _controller.Url = urlHelperMock.Object;
            _mockUnidadTrabajo
                .Setup(u => u.EventoPersonal.ObtenerPrimero(It.IsAny<Expression<Func<EventoPersonal, bool>>>(), null, false))
                .ReturnsAsync((EventoPersonal)null);

            _mockUnidadTrabajo
                .Setup(u => u.Evento.ObtenerPrimero(It.IsAny<Expression<Func<Evento, bool>>>(), null, true))
                .ReturnsAsync(new Evento
                {
                    Id = 1,
                    Fecha = DateTime.Now,
                    ClienteId = 2
                });
            _mockUnidadTrabajo
                .Setup(u => u.Personal.Obtener(It.IsAny<int>()))
                .ReturnsAsync(new Personal
                {
                    Id = 1,
                    Nombre = "Ian Mora"
                });

            _mockUnidadTrabajo
                .Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            var tempDataMock = new Mock<ITempDataDictionary>();
            _controller.TempData = tempDataMock.Object;
            var result = await _controller.Upsert(eventoPersonalVM);
            tempDataMock.VerifySet(tempData => tempData[DS.Exitosa] = "Evento agregado exitosamente", Times.Once);
            _mockUnidadTrabajo.Verify(u => u.EventoPersonal.Agregar(It.IsAny<EventoPersonal>()), Times.Once);
            _mockUnidadTrabajo.Verify(u => u.Guardar(), Times.Once);
            Assert.That(result, Is.InstanceOf<RedirectResult>());
        }


        [Test]
        public async Task Delete_Post_EliminaEventoPersonal_Exito()
        {
            int idEventoPersonal = 1;
            var eventoPersonal = new EventoPersonal
            {
                Id = idEventoPersonal,
                IdEvento = 1,
                IdPersonal = 1
            };

            var evento = new Evento
            {
                Id = 1,
                Fecha = DateTime.Now,
                HoraInicio = DateTime.Now.TimeOfDay,
                ClienteId = 2
            };

            var personal = new Personal
            {
                Id = 1,
                Nombre = "Alejandro Tellez"
            };


            _mockUnidadTrabajo
                .Setup(u => u.EventoPersonal.Obtener(idEventoPersonal))
                .ReturnsAsync(eventoPersonal);

            
            _mockUnidadTrabajo
                .Setup(u => u.EventoPersonal.Remover(It.IsAny<EventoPersonal>()))
                .Verifiable();

            _mockUnidadTrabajo.Setup(u => u.Guardar()).Returns(Task.CompletedTask);


            _mockUnidadTrabajo
                .Setup(u => u.Evento.Obtener(It.IsAny<int>()))
                .ReturnsAsync(evento);

            _mockUnidadTrabajo
                .Setup(u => u.Personal.Obtener(It.IsAny<int>()))
                .ReturnsAsync(personal);

            _mockUnidadTrabajo
                .Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);


            var result = await _controller.Delete(idEventoPersonal);

            _mockUnidadTrabajo.Verify(u => u.EventoPersonal.Remover(It.IsAny<EventoPersonal>()), Times.Once);
            _mockUnidadTrabajo.Verify(u => u.Guardar(), Times.Once);

            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("true"));
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Personal borrado del evento exitosamente"));

        }



        [Test]
        public async Task Delete_CuandoEventoPersonalNoExiste_RetornaError()
        {
            
            int id = 1;
            _mockUnidadTrabajo
                .Setup(u => u.EventoPersonal.Obtener(id))
                .ReturnsAsync((EventoPersonal)null);

            var result = await _controller.Delete(id);

            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("false"));
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar personal del evento"));
        }


    }
}
