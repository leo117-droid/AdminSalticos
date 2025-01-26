using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SalticosAdmin.Areas.Admin.Controllers;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SalticosAdmin.Utilidades;
using System.Linq.Expressions;
using SalticosAdmin.AccesoDeDatos.Repositorio;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class TarifasTransporteControllerTests
    {
        // Configuración inicial antes de cada prueba
        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private TarifasTransporteController _controller;
        private Mock<HttpContext> _httpContextMock;
        private Mock<ITempDataDictionary> _tempDataMock;


        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new TarifasTransporteController(_unidadTrabajoMock.Object);
            
            //Simula el login y creacion de un usuario
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));

            //Simula el HTTP necesario para el funcionamiento de la prueba
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            //Simula la existencia del TempData 
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            {
                [DS.Exitosa] = "Success message",
                [DS.Error] = "Error message"
            };
            _controller.TempData = tempData;

            // Simula los metodo para Bitacora 
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
            var modelo = (TarifasTransporte)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0)); 
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var tarifasTransporteId = 1;
            var tarifasTransporte = new TarifasTransporte
            { 
                Id = tarifasTransporteId,
                Provincia = "Heredia" 
            };
            _unidadTrabajoMock.Setup(u => u.TarifasTransporte.Obtener(tarifasTransporteId)).ReturnsAsync(tarifasTransporte);

            var resultado = await _controller.Upsert(tarifasTransporteId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (TarifasTransporte)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);  
            Assert.That(modelo.Id, Is.EqualTo(tarifasTransporteId)); 
        }


        // Verifica que el método Upsert cree una nueva tarifa de transporte cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaTarifasTransporte()
        {
            var tarifasTransporteId = 0;
            var tarifasTransporte = new TarifasTransporte
            {
                Id = tarifasTransporteId,
                Provincia = "Cartago"
            };

            _unidadTrabajoMock.Setup(u => u.TarifasTransporte.Agregar(It.IsAny<TarifasTransporte>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(tarifasTransporte);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); 
            _unidadTrabajoMock.Verify(u => u.TarifasTransporte.Agregar(It.IsAny<TarifasTransporte>()), Times.Once);
        }

        // Verifica que el método Upsert actualice una tarifa transporte existente.
        [Test]
        public async Task Upsert_Post_ActualizaTarifasTransporte()
        {
            var tarifasTransporteId = 1;
            var tarifasTransporteExistente = new TarifasTransporte
            {
                Id = tarifasTransporteId,
                Provincia = "Alajuela"
            };

            var tarifasTransporteActualizada = new TarifasTransporte
            {
                Id = tarifasTransporteId,
                Provincia = "Puntarenas"
            };

            _unidadTrabajoMock.Setup(u => u.TarifasTransporte.Obtener(tarifasTransporteId)).ReturnsAsync(tarifasTransporteExistente);
            _unidadTrabajoMock.Setup(u => u.TarifasTransporte.Actualizar(It.IsAny<TarifasTransporte>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(tarifasTransporteActualizada);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); 
            _unidadTrabajoMock.Verify(u => u.TarifasTransporte.Actualizar(It.IsAny<TarifasTransporte>()), Times.Once); 
        }

        // Verifica que el método Delete elimine correctamente una tarifa de transporte.
        [Test]
        public async Task Delete_RetornaExito_CuandoLaTarifaTransporteSeEliminaCorrectamente()
        {
            var tarifasTransporteId = 1;
            var tarifasTransporte = new TarifasTransporte
            {
                Id = tarifasTransporteId,
                Provincia = "San José"
            };

            _unidadTrabajoMock.Setup(u => u.TarifasTransporte.Obtener(tarifasTransporteId)).ReturnsAsync(tarifasTransporte);

            _unidadTrabajoMock.Setup(u => u.TarifasTransporte.Remover(It.IsAny<TarifasTransporte>())).Verifiable();

            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(tarifasTransporteId);

            _unidadTrabajoMock.Setup(u => u.TarifasTransporte.Obtener(tarifasTransporteId)).ReturnsAsync((TarifasTransporte)null);

            var tarifasTransporteEliminada = await _unidadTrabajoMock.Object.TarifasTransporte.Obtener(tarifasTransporteId);

            Assert.That(tarifasTransporteEliminada, Is.Null);
            _unidadTrabajoMock.Verify(u => u.TarifasTransporte.Remover(It.IsAny<TarifasTransporte>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar una tarifa de transporte que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoLaTarifaTransporteNoExiste()
        {
            var tarifaTransporteId = 999;
            TarifasTransporte tarifasTransporteBd = null;

            _unidadTrabajoMock.Setup(u => u.TarifasTransporte.Obtener(tarifaTransporteId)).ReturnsAsync(tarifasTransporteBd);

            var resultado = await _controller.Delete(tarifaTransporteId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;

            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Tarifa de Transporte"));
        }

        // Verifica que el método ValidarProvincia retorne false si la provincia no está duplicada.
        [Test]
        public async Task ValidarProvincia_ProvinciaNoDuplicada_RetornaFalse()
        {
            var provinciaNueva = "Limón";
            var listaProvincias = new List<TarifasTransporte>
            {
                new TarifasTransporte { Id = 1, Provincia = "Alajuela" },
                new TarifasTransporte { Id = 2, Provincia = "Guanacaste" }
            };

            _unidadTrabajoMock.Setup(u => u.TarifasTransporte.ObtenerTodos(It.IsAny<Expression<Func<TarifasTransporte, bool>>>(), null, null, true))
                .ReturnsAsync(listaProvincias);

            var resultado = await _controller.ValidarProvincia(provinciaNueva);

            var jsonResult = resultado as JsonResult;
            var dataPropiedad = jsonResult.Value.GetType().GetProperty("data");
            var dataValor = dataPropiedad.GetValue(jsonResult.Value);
            Assert.That(dataValor, Is.False, "El valor de 'data' no es false.");
        }

        // Verifica que el método ValidarProvincia retorne true si la provincia está duplicada.
        [Test]
        public async Task ValidarProvincia_ProvinciaEsDuplicada_RetornaTrue()
        {
            var provinciaNueva = "Heredia";
            var listaProvincias = new List<TarifasTransporte>
            {
                new TarifasTransporte { Id = 1, Provincia = "Heredia" },
                new TarifasTransporte { Id = 2, Provincia = "Guanacaste" }
            };

            _unidadTrabajoMock.Setup(u => u.TarifasTransporte.ObtenerTodos(It.IsAny<Expression<Func<TarifasTransporte, bool>>>(), null, null, true))
                .ReturnsAsync(listaProvincias);

            var resultado = await _controller.ValidarProvincia(provinciaNueva);

            var jsonResult = resultado as JsonResult;
            var dataPropiedad = jsonResult.Value.GetType().GetProperty("data");
            var dataValor = dataPropiedad.GetValue(jsonResult.Value);
            Assert.That(dataValor, Is.True, "El valor de 'data' no es true.");
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

