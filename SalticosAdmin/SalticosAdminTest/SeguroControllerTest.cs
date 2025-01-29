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
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SalticosAdmin.Utilidades;
using System.Linq.Expressions;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class SeguroControllerTests
    {        
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private SeguroController _controller;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new SeguroController(_unidadTrabajoMock.Object);

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
        public void Index_RetornaVista()
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
            var modelo = (Seguro)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0));
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var seguroId = 1;
            var seguro = new Seguro
            {
                Id = seguroId,
                NumeroPoliza = 12345,
                FechaInicio = DateTime.Now.AddDays(-30),
                FechaVencimiento = DateTime.Now.AddDays(30)
            };
            _unidadTrabajoMock.Setup(u => u.Seguros.Obtener(seguroId)).ReturnsAsync(seguro);

            var resultado = await _controller.Upsert(seguroId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Seguro)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(seguroId));
        }

        // Verifica que el método Upsert cree un nuevo seguro cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaSeguro()
        {
            var seguro = new Seguro
            {
                Id = 0,
                NumeroPoliza = 67890,
                FechaInicio = DateTime.Now.AddDays(-30),
                FechaVencimiento = DateTime.Now.AddDays(60)
            };

            _unidadTrabajoMock.Setup(u => u.Seguros.Agregar(It.IsAny<Seguro>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Upsert(seguro);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.Seguros.Agregar(It.IsAny<Seguro>()), Times.Once);
        }

        // Verifica que el método Upsert actualice un seguro existente.
        [Test]
        public async Task Upsert_Post_ActualizaSeguro()
        {
            var seguroId = 1;
            var seguroExistente = new Seguro
            {
                Id = seguroId,
                NumeroPoliza = 12345,
                FechaInicio = DateTime.Now.AddDays(-30),
                FechaVencimiento = DateTime.Now.AddDays(30)
            };

            var seguroActualizado = new Seguro
            {
                Id = seguroId,
                NumeroPoliza = 12345,
                FechaInicio = DateTime.Now.AddDays(-20),
                FechaVencimiento = DateTime.Now.AddDays(40)
            };

            _unidadTrabajoMock.Setup(u => u.Seguros.Obtener(seguroId)).ReturnsAsync(seguroExistente);
            _unidadTrabajoMock.Setup(u => u.Seguros.Actualizar(It.IsAny<Seguro>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Upsert(seguroActualizado);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.Seguros.Actualizar(It.IsAny<Seguro>()), Times.Once);
        }

        // Verifica que el método Delete elimine correctamente seguro.
        [Test]
        public async Task Delete_RetornaExito_CuandoElSeguroExiste()
        {
            var seguroId = 1;
            var seguroBd = new Seguro
            {
                Id = seguroId,
                NumeroPoliza = 12345
            };

            _unidadTrabajoMock.Setup(u => u.Seguros.Obtener(seguroId)).ReturnsAsync(seguroBd);
            _unidadTrabajoMock.Setup(u => u.Seguros.Remover(It.IsAny<Seguro>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(seguroId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("true"));
            _unidadTrabajoMock.Verify(u => u.Seguros.Remover(It.IsAny<Seguro>()), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar un seguro que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoElSeguroNoExiste()
        {
            var seguroId = 999;
            Seguro seguroBd = null;

            _unidadTrabajoMock.Setup(u => u.Seguros.Obtener(seguroId)).ReturnsAsync(seguroBd);

            var resultado = await _controller.Delete(seguroId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar seguro"));
        }

        [Test]
        public async Task ValidarPoliza_PolizaNoDuplicada_RetornaFalse()
        {
            var polizaNueva = 67890;
            var listaSeguros = new List<Seguro>
            {
                new Seguro { Id = 1, NumeroPoliza = 12345 },
                new Seguro { Id = 2, NumeroPoliza = 54321 }
            };

            _unidadTrabajoMock.Setup(u => u.Seguros.ObtenerTodos(It.IsAny<Expression<Func<Seguro, bool>>>(), null, null, true))
                .ReturnsAsync(listaSeguros);

            var resultado = await _controller.ValidarPoliza(polizaNueva);

            var jsonResult = resultado as JsonResult;
            var dataPropiedad = jsonResult.Value.GetType().GetProperty("data");
            var dataValor = dataPropiedad.GetValue(jsonResult.Value);
            Assert.That(dataValor, Is.False, "El valor de 'data' no es false.");
        }

        // Verifica que el método ValidarPoliza retorne true si el número de póliza está duplicado.
        [Test]
        public async Task ValidarPoliza_PolizaDuplicada_RetornaTrue()
        {
            var polizaNueva = 54321;
            var listaSeguros = new List<Seguro>
            {
                new Seguro { Id = 1, NumeroPoliza = 12345 },
                new Seguro { Id = 2, NumeroPoliza = 54321 }
            };

            _unidadTrabajoMock.Setup(u => u.Seguros.ObtenerTodos(It.IsAny<Expression<Func<Seguro, bool>>>(), null, null, true))
                .ReturnsAsync(listaSeguros);

            var resultado = await _controller.ValidarPoliza(polizaNueva);

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
