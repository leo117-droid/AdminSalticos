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
    public class EventoVehiculoControllerTest
    {

        private Mock<IUnidadTrabajo> _mockUnidadTrabajo;
        private EventoVehiculoController _controller;


        [SetUp]
        public void SetUp()
        {
            _mockUnidadTrabajo = new Mock<IUnidadTrabajo>();
            _controller = new EventoVehiculoController(_mockUnidadTrabajo.Object);

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

        //Verifica el controlador sigue funciona correctamente y proporciona la lista de servicio adicional esperada en la vista.
        [Test]
        public async Task Upsert_ConRelacionIdEsNull_RetornVistaConViewModel()
        {
            
            int eventoID = 1;
            int? relacionId = null;

            var mockVehiculoList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Chevrolet", Value = "1" },
                new SelectListItem { Text = "Toyota", Value = "2" }
            };

            _mockUnidadTrabajo
                .Setup(u => u.EventoVehiculo.ObtenerVehiculo("Vehiculo", eventoID))
                .Returns(mockVehiculoList);

            
            var result = await _controller.Upsert(eventoID, relacionId) as ViewResult;
            var model = result.Model as EventoVehiculoVM;

            
            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(eventoID, Is.EqualTo(model.IdEvento));
            Assert.That(mockVehiculoList, Is.EqualTo(model.ListaVehiculo));
            
        }

        //Asegura que cuando el modelo es inválido, el controlador responde correctamente, mostrando la vista con los errores de validación
        [Test]
        public async Task Upsert_PostModeloInvalido_ReturnsVistaConErrores()
        {         
            var viewModel = new EventoVehiculoVM { IdEvento = 1 };
            _controller.ModelState.AddModelError("IdVehiculo", "El campo es requerido.");

            var result = await _controller.Upsert(viewModel) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(viewModel));
            Assert.That(_controller.ModelState.IsValid, Is.False);
        }

        // Verifica que el método Upsert cree un nuevo evento vehiculo cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_PostModeloValido_CreaNuevoEventoVehiculo()
        {
            
            var eventoVehiculoVM = new EventoVehiculoVM
            {
                IdEvento = 1,
                IdRelacion = 0,
                IdVehiculo = 1
            };

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("fake-url");

            _controller.Url = urlHelperMock.Object; 
            _mockUnidadTrabajo
                .Setup(u => u.EventoVehiculo.ObtenerPrimero(It.IsAny<Expression<Func<EventoVehiculo, bool>>>(), null, false))
                .ReturnsAsync((EventoVehiculo)null);

            
            _mockUnidadTrabajo
                .Setup(u => u.Evento.ObtenerPrimero(It.IsAny<Expression<Func<Evento, bool>>>(), null, true))
                .ReturnsAsync(new Evento
                {
                    Id = 1,
                    Fecha = DateTime.Now,
                    HoraInicio = DateTime.Now.TimeOfDay,
                    ClienteId = 2
                });

            _mockUnidadTrabajo
                .Setup(u => u.Vehiculo.Obtener(It.IsAny<int>()))
                .ReturnsAsync(new Vehiculo
                {
                    Id = 1,
                    Placa = "990812"
                });

            _mockUnidadTrabajo
                .Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var tempDataMock = new Mock<ITempDataDictionary>();
            _controller.TempData = tempDataMock.Object;

            var result = await _controller.Upsert(eventoVehiculoVM);

            tempDataMock.VerifySet(tempData => tempData[DS.Exitosa] = "Vehiculo agregado exitosamente", Times.Once);
            _mockUnidadTrabajo.Verify(u => u.EventoVehiculo.Agregar(It.IsAny<EventoVehiculo>()), Times.Once);
            _mockUnidadTrabajo.Verify(u => u.Guardar(), Times.Once);
            Assert.That(result, Is.InstanceOf<RedirectResult>());
        }


        // Verifica que el método Delete elimine correctamente un vehiculo de un evento.
        [Test]
        public async Task Delete_Post_EliminaEventoVehiculo_Exito()
        {
            int idEventoVehiculo = 1;
            var eventoVehiculo = new EventoVehiculo
            {
                Id = idEventoVehiculo,
                IdEvento = 1,
                IdVehiculo = 1
            };

            var evento = new Evento
            {
                Id = 1,
                Fecha = DateTime.Now,
                HoraInicio = DateTime.Now.TimeOfDay,
                ClienteId = 2
            };

            var vehiculo = new Vehiculo
            {
                Id = 1,
                Placa = "203018"
            };


            _mockUnidadTrabajo
                .Setup(u => u.EventoVehiculo.Obtener(idEventoVehiculo))
                .ReturnsAsync(eventoVehiculo);

            
            _mockUnidadTrabajo
                .Setup(u => u.EventoVehiculo.Remover(It.IsAny<EventoVehiculo>()))
                .Verifiable();

            _mockUnidadTrabajo.Setup(u => u.Guardar()).Returns(Task.CompletedTask);


            _mockUnidadTrabajo
                .Setup(u => u.Evento.Obtener(It.IsAny<int>()))
                .ReturnsAsync(evento);

            _mockUnidadTrabajo
                .Setup(u => u.Vehiculo.Obtener(It.IsAny<int>()))
                .ReturnsAsync(vehiculo);

            _mockUnidadTrabajo
                .Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);


            var result = await _controller.Delete(idEventoVehiculo);

            _mockUnidadTrabajo.Verify(u => u.EventoVehiculo.Remover(It.IsAny<EventoVehiculo>()), Times.Once);
            _mockUnidadTrabajo.Verify(u => u.Guardar(), Times.Once);

            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("true"));
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Vehiculo borrado del evento exitosamente"));

        }

        // Verifica que se retorne un error cuando se intenta eliminar un evento vehiculo que no existe.
        [Test]
        public async Task Delete_CuandoEventoVehiculoNoExiste_RetornaError()
        {
            
            int id = 1;
            _mockUnidadTrabajo
                .Setup(u => u.EventoVehiculo.Obtener(id))
                .ReturnsAsync((EventoVehiculo)null);

            var result = await _controller.Delete(id);

            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;

            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("false"));
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar vehiculo del evento"));
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
