using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SalticosAdmin.Areas.Admin.Controllers;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using SalticosAdmin.Modelos.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SalticosAdmin.Utilidades;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class CapacitacionPersonalControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private CapacitacionPersonalController _controller;
        private Mock<HttpContext> _httpContextMock;
        private Mock<ITempDataDictionary> _tempDataMock;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new CapacitacionPersonalController(_unidadTrabajoMock.Object);

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
            var resultado = _controller.Index(null);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
        }

        // Verifica que, cuando el ID es null, el método Upsert retorne una vista con un modelo vacío.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloVacioCuandoIdEsNull()
        {
            _unidadTrabajoMock.Setup(u => u.CapacitacionPersonal.ObtenerPersonal(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<SelectListItem>());

            var resultado = await _controller.Upsert(1, (int?)null);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (CapacitacionPersonalVM)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.IdRelacion, Is.EqualTo(0));
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var relacionId = 1;
            var capacitacionPersonal = new CapacitacionPersonal
            {
                Id = relacionId,
                IdCapacitacion = 1,
                IdPersonal = 2
            };

            _unidadTrabajoMock.Setup(u => u.CapacitacionPersonal.ObtenerPrimero(It.IsAny<Expression<Func<CapacitacionPersonal, bool>>>(), null, true))
                .ReturnsAsync(capacitacionPersonal);

            var resultado = await _controller.Upsert(1, relacionId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (CapacitacionPersonalVM)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.IdRelacion, Is.EqualTo(relacionId));
        }

        // Verifica que el método Upsert cree una nuevo personal en capacitación cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_AgregarNuevoPersonal_Exito()
        {
            // Arrange
            var capacitacionPersonalVM = new CapacitacionPersonalVM
            {
                IdPersonal = 1,
                IdRelacion = 0,
                IdCapacitacion = 2
            };

            // Mock de IUrlHelper
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("fake-url"); 

            _controller.Url = urlHelperMock.Object; // Asigna el mock al controlador.

            _unidadTrabajoMock.Setup(u => u.Personal.ObtenerPrimero(
                It.IsAny<Expression<Func<Personal, bool>>>(),
                null,
                true))
                .ReturnsAsync(new Personal { Id = 2, Nombre = "Ian Calvo" });

            _unidadTrabajoMock.Setup(u => u.Capacitacion.ObtenerPrimero(
                It.IsAny<Expression<Func<Capacitacion, bool>>>(),
                null,
                true))
                .ReturnsAsync(new Capacitacion { Id = 1, Tema = "Limpieza máquina de burbujas" });

            _unidadTrabajoMock.Setup(u => u.CapacitacionPersonal.Agregar(It.IsAny<CapacitacionPersonal>()))
                .Verifiable();

            var resultado = await _controller.Upsert(capacitacionPersonalVM);

            Assert.That(_controller.TempData[DS.Exitosa], Is.EqualTo("Personal agregado exitosamente"));
            _unidadTrabajoMock.Verify(u => u.CapacitacionPersonal.Agregar(It.IsAny<CapacitacionPersonal>()), Times.Once);
        }

        // Verifica que un personal asociado a una capacitación sea eliminado correctamente del producto de alimentación.
        [Test]
        public async Task Delete_RetornaExito_CuandoElCapacitacionPersonalExiste()
        {
            var capacitacionPersonalId = 1;
            var capacitacionPersonalBd = new CapacitacionPersonal
            {
                Id = capacitacionPersonalId,
                IdCapacitacion = 1,
                IdPersonal = 2
            };

            var personal = new Personal
            {
                Id = 2,
                Nombre = "Gabriela Chaves"
            };

            var capacitacion = new Capacitacion
            {
                Id = 1,
                Tema = "Mantenimiento de los motores"
            };

            _unidadTrabajoMock.Setup(u => u.CapacitacionPersonal.Obtener(capacitacionPersonalId)).ReturnsAsync(capacitacionPersonalBd);
            _unidadTrabajoMock.Setup(u => u.CapacitacionPersonal.Remover(It.IsAny<CapacitacionPersonal>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Personal.ObtenerPrimero(It.IsAny<Expression<Func<Personal, bool>>>(), null, true))
                              .ReturnsAsync(personal);
            _unidadTrabajoMock.Setup(u => u.Capacitacion.ObtenerPrimero(It.IsAny<Expression<Func<Capacitacion, bool>>>(), null, true))
                              .ReturnsAsync(capacitacion);

            var resultado = await _controller.Delete(capacitacionPersonalId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("true"));
            _unidadTrabajoMock.Verify(u => u.CapacitacionPersonal.Remover(It.IsAny<CapacitacionPersonal>()), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar un personal a una capacitación que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoElCapacitacionPersonalNoExiste()
        {
            var capacitacionPersonalId = 999;
            CapacitacionPersonal capacitacionPersonalBd = null;

            _unidadTrabajoMock.Setup(u => u.CapacitacionPersonal.Obtener(capacitacionPersonalId)).ReturnsAsync(capacitacionPersonalBd);

            var resultado = await _controller.Delete(capacitacionPersonalId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar personal de una capacitación"));
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