using Moq;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Areas.Admin.Controllers;
using SalticosAdmin.Modelos.ViewModels;
using SalticosAdmin.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using SalticosAdmin.Utilidades;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SalticosAdminTest
{

    [TestFixture]
    public class CotizacionControllerTest
    {
        private Mock<IUnidadTrabajo> _mockUnidadTrabajo;
        private Mock<IServiceProvider> _mockServiceProvider;
        private CotizacionController _controller;

        private Mock<IFormCollection> _formMock;

        [SetUp]
        public void SetUp()
        {
            _mockUnidadTrabajo = new Mock<IUnidadTrabajo>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _formMock = new Mock<IFormCollection>();

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            {
                [DS.Exitosa] = "Success message",
                [DS.Error] = "Error message"
            };
            


            _controller = new CotizacionController(_mockUnidadTrabajo.Object, _mockServiceProvider.Object);
            _controller.TempData = tempData;
        }



        // Valida que cuando se llama al método Index, se retorne una vista con un modelo de CotizacionVM
        [Test]
        public async Task Index_DebeRetornarConCotizacionVM_CuandoHayDatosDisponibles()
        {
            var mockInflables = new List<Inflable> { new Inflable { Id = 1, Nombre = "Avengers" } };
            var mockMobiliarios = new List<Mobiliario> { new Mobiliario { Id = 1, Nombre = "Sillas" } };
            var mockServicios = new List<ServicioAdicional> { new ServicioAdicional { Id = 1, Nombre = "Máquina de burbujas" } };
            var mockAlimentacion = new List<Alimentacion> { new Alimentacion { Id = 1, Nombre = "Hot dog" } };
            var mockTarifas = new List<TarifasTransporte> { new TarifasTransporte { Id = 1, Provincia = "Heredia" } };

            _mockUnidadTrabajo.Setup(x => x.Inflable.ObtenerTodos(It.IsAny<Expression<Func<Inflable, bool>>>(), null, null, true)).ReturnsAsync(mockInflables);
            _mockUnidadTrabajo.Setup(x => x.Mobiliario.ObtenerTodos(It.IsAny<Expression<Func<Mobiliario, bool>>>(), null, null, true)).ReturnsAsync(mockMobiliarios);
            _mockUnidadTrabajo.Setup(x => x.ServicioAdicional.ObtenerTodos(It.IsAny<Expression<Func<ServicioAdicional, bool>>>(), null, null, true)).ReturnsAsync(mockServicios);
            _mockUnidadTrabajo.Setup(x => x.Alimentacion.ObtenerTodos(It.IsAny<Expression<Func<Alimentacion, bool>>>(), null, null, true)).ReturnsAsync(mockAlimentacion);
            _mockUnidadTrabajo.Setup(x => x.TarifasTransporte.ObtenerTodos(It.IsAny<Expression<Func<TarifasTransporte, bool>>>(), null, null, true)).ReturnsAsync(mockTarifas);

            var result = await _controller.Index() as ViewResult;

            Assert.That(result, Is.Not.Null);
            var model = result.Model as CotizacionVM;
            Assert.That(model, Is.Not.Null);
            Assert.That(1, Is.EqualTo(model.Inflables.Count));
            Assert.That(1, Is.EqualTo(model.Mobiliarios.Count));
            Assert.That(1, Is.EqualTo(model.ServiciosAdicionales.Count));
            Assert.That(1, Is.EqualTo(model.Alimentacion.Count));
            Assert.That(1, Is.EqualTo(model.TarifasTransporte.Count));
        }


        // Valida que cuando se llama el métoddo de generar cotizacion,
        // se retorne un mensaje de error cuando no se selecciona ningun elemento
        [Test]
        public async Task GenerarCotizacion_RetornaError_CuandoNoSeSeleccionaNingunElemento()
        {
            
            var result = await _controller.GenerarCotizacion(
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                _formMock.Object
            );

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            Assert.That(_controller.TempData["Error"], Is.EqualTo("Debe seleccionar al menos un elemento."));
        }


        // Valida que cuando se llama el método de generar cotizacion,
        // se retorne una vista con un modelo de CotizacionVM con el monto total correcto
        [Test]
        public async Task GenerarCotizacion_RetornaVista_ConCotizacionCorrecta_CuandoSeSeleccionanItems()
        {
            var inflables = new List<Inflable>
            {
                new Inflable { Id = 1, Precio = 110000, PrecioHoraAdicional = 20000 }
            };
            var mobiliarios = new List<Mobiliario>
            {
                new Mobiliario { Id = 1, Precio = 325 }
            };
            var servicios = new List<ServicioAdicional>
            {
                new ServicioAdicional { Id = 1, Precio = 12500 }
            };
            var alimentacion = new List<Alimentacion>
            {
                new Alimentacion { Id = 1, Precio = 850 }
            };
            var transportes = new List<TarifasTransporte>
            {
                new TarifasTransporte { Id = 1, Precio = 3500 }
            };

            _mockUnidadTrabajo.Setup(u => u.Inflable.ObtenerTodos(It.IsAny<Expression<Func<Inflable, bool>>>(), null, null, true)).ReturnsAsync(inflables);
            _mockUnidadTrabajo.Setup(u => u.Mobiliario.ObtenerTodos(It.IsAny<Expression<Func<Mobiliario, bool>>>(), null, null, true)).ReturnsAsync(mobiliarios);
            _mockUnidadTrabajo.Setup(u => u.ServicioAdicional.ObtenerTodos(It.IsAny<Expression<Func<ServicioAdicional, bool>>>(), null, null, true)).ReturnsAsync(servicios);
            _mockUnidadTrabajo.Setup(u => u.Alimentacion.ObtenerTodos(It.IsAny<Expression<Func<Alimentacion, bool>>>(), null, null, true)).ReturnsAsync(alimentacion);
            _mockUnidadTrabajo.Setup(u => u.TarifasTransporte.ObtenerTodos(It.IsAny<Expression<Func<TarifasTransporte, bool>>>(), null, null, true)).ReturnsAsync(transportes);

            _formMock.Setup(f => f["inflableHorasAdicionales_1"]).Returns("2");
            _formMock.Setup(f => f["mobiliarioCantidades_1"]).Returns("3");
            _formMock.Setup(f => f["servicioCantidades_1"]).Returns("1");
            _formMock.Setup(f => f["alimentacionCantidades_1"]).Returns("4");

            var result = await _controller.GenerarCotizacion(
                new List<int> { 1 },
                new List<int> { 2 },
                new List<int> { 1 },
                new List<int> { 1 },
                new List<int> { 1 },
                new List<int> { 1 },
                new List<int> { 1 },
                new List<int> { 1 },
                new List<int> { 1 },
                _formMock.Object
            );

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            var cotizacion = viewResult.Model as CotizacionVM;
            Assert.That(cotizacion, Is.Not.Null);
            Assert.That(cotizacion.MontoTotal, Is.EqualTo(100 + (20 * 2) + 50 * 3 + 30 + 15 * 4 + 200));
        }



        [TearDown]
        public void TearDown()
        {
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
