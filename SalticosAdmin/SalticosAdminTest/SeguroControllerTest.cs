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

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class SeguroControllerTests
    {
        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private SeguroController _controller;
        private Mock<HttpContext> _httpContextMock;
        private Mock<ITempDataDictionary> _tempDataMock;

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

        [Test]
        public void Index_RetornaVista()
        {
            var resultado = _controller.Index();

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloVacioCuandoIdEsNull()
        {
            var resultado = await _controller.Upsert((int?)null);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Seguro)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0));
        }

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

        [TearDown]
        public void TearDown()
        {
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
