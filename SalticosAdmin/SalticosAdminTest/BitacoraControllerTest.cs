using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SalticosAdmin.Areas.Admin.Controllers;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SalticosAdmin.Utilidades;
using System.Security.Claims;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class BitacoraControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private BitacoraController _controller;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new BitacoraController(_unidadTrabajoMock.Object);

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
        }

        // Verifica que el método Index retorne una vista correctamente.
        [Test]
        public void Index_RetornaVista()
        {
            var resultado = _controller.Index();

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
        }

        // Verifica que el método Intermedia retorne una vista correctamente.
        [Test]
        public void Intermedia_RetornaVista()
        {
            var resultado = _controller.Intermedia();

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
        }

        // Verifica que el método ObtenerTodos retorna un Json con los datos ordenados.
        [Test]
        public async Task ObtenerTodos_RetornaJsonConDatosOrdenados()
        {
            var bitacoraLista = new List<Bitacora>
            {
                new Bitacora { Fecha = DateTime.Now.AddDays(-1) },
                new Bitacora { Fecha = DateTime.Now }
            };

            _unidadTrabajoMock.Setup(u => u.Bitacora.ObtenerTodos(
                It.IsAny<Expression<Func<Bitacora, bool>>>(),
                It.IsAny<Func<IQueryable<Bitacora>, IOrderedQueryable<Bitacora>>>(),
                It.IsAny<string>(),
                true
            )).ReturnsAsync(bitacoraLista);

            var resultado = await _controller.ObtenerTodos();

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);
            var datos = contenido["data"].ToObject<List<Bitacora>>();

            Assert.That(datos, Is.Not.Null);
            Assert.That(datos.Count, Is.EqualTo(bitacoraLista.Count));
            Assert.That(datos[0].Fecha, Is.GreaterThan(datos[1].Fecha));
        }

        // Verifica que el método ConsultarConFiltro retorna un Json con los datos correctamente filtrados.
        [Test]
        public async Task ConsultarConFiltro_RetornaJsonConDatosFiltrados()
        {
            var fechainicial = DateTime.Now.AddDays(-3);
            var fechafinal = DateTime.Now;

            var bitacoraLista = new List<Bitacora>
            {
                new Bitacora { Fecha = DateTime.Now.AddDays(-2) },
                new Bitacora { Fecha = DateTime.Now.AddDays(-1) }
            };

            _unidadTrabajoMock.Setup(u => u.Bitacora.ObtenerEntreFechas(fechainicial, fechafinal)).ReturnsAsync(bitacoraLista);

            var resultado = await _controller.ConsultarConFiltro(fechainicial, fechafinal);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);
            var datos = contenido["data"].ToObject<List<Bitacora>>();

            Assert.That(datos, Is.Not.Null);
            Assert.That(datos.Count, Is.EqualTo(bitacoraLista.Count));
        }

        // Verifica que el método ConsultarConFiltro retorna un Json con los datos correctamente en caso de no seleccionar fechas para el filtro.
        [Test]
        public async Task ConsultarConFiltro_RetornaJsonConTodosLosDatosSiFechasSonMinValue()
        {
            var bitacoraLista = new List<Bitacora>
            {
                new Bitacora { Fecha = DateTime.Now.AddDays(-2) },
                new Bitacora { Fecha = DateTime.Now.AddDays(-1) }
            };

            _unidadTrabajoMock.Setup(u => u.Bitacora.ObtenerTodos(
                It.IsAny<Expression<Func<Bitacora, bool>>>(),
                It.IsAny<Func<IQueryable<Bitacora>, IOrderedQueryable<Bitacora>>>(),
                It.IsAny<string>(),
                true
            )).ReturnsAsync(bitacoraLista);

            var resultado = await _controller.ConsultarConFiltro(DateTime.MinValue, DateTime.MinValue);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);
            var datos = contenido["data"].ToObject<List<Bitacora>>();

            Assert.That(datos, Is.Not.Null);
            Assert.That(datos.Count, Is.EqualTo(bitacoraLista.Count));
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
