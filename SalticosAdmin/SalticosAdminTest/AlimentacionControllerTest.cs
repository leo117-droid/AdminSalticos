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

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class AlimentacionControllerTests
    {
        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private AlimentacionController _controller;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _controller = new AlimentacionController(_unidadTrabajoMock.Object, _webHostEnvironmentMock.Object);

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
            var modelo = (Alimentacion)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0));
        }

        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var alimentacionId = 1;
            var alimentacion = new Alimentacion
            {
                Id = alimentacionId,
                Nombre = "Test Alimentacion"
            };
            _unidadTrabajoMock.Setup(u => u.Alimentacion.Obtener(alimentacionId)).ReturnsAsync(alimentacion);

            var resultado = await _controller.Upsert(alimentacionId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Alimentacion)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(alimentacionId));
        }

        

        [Test]
        public async Task Delete_RetornaExito_CuandoLaAlimentacionNoTieneEventosAsociados()
        {
            var alimentacionId = 1;
            var alimentacionBd = new Alimentacion
            {
                Id = alimentacionId,
                Nombre = "Alimentacion para borrar",
                ImageUrl = "testimage.jpg"
            };

            _unidadTrabajoMock.Setup(u => u.Alimentacion.Obtener(alimentacionId)).ReturnsAsync(alimentacionBd);
            _unidadTrabajoMock.Setup(u => u.Alimentacion.Remover(It.IsAny<Alimentacion>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(alimentacionId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);
            Assert.That(contenido["success"].ToString(), Is.EqualTo("True")); // Aquí se añade la comparación insensible a mayúsculas y minúsculas
            _unidadTrabajoMock.Verify(u => u.Alimentacion.Remover(It.IsAny<Alimentacion>()), Times.Once);
        }


        [Test]
        public async Task Delete_RetornaError_CuandoLaAlimentacionNoExiste()
        {
            var alimentacionId = 999;
            Alimentacion alimentacionBd = null;

            _unidadTrabajoMock.Setup(u => u.Alimentacion.Obtener(alimentacionId)).ReturnsAsync(alimentacionBd);

            var resultado = await _controller.Delete(alimentacionId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Alimentacion"));
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
