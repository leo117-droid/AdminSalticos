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
    public class AlimentacionIngredienteControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private AlimentacionIngredienteController _controller;
        private Mock<HttpContext> _httpContextMock;
        private Mock<ITempDataDictionary> _tempDataMock;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new AlimentacionIngredienteController(_unidadTrabajoMock.Object);

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
            _unidadTrabajoMock.Setup(u => u.AlimentacionIngrediente.ObtenerIngrediente(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<SelectListItem>());

            var resultado = await _controller.Upsert(1, (int?)null);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (AlimentacionIngredienteVM)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.IdRelacion, Is.EqualTo(0));
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var relacionId = 1;
            var alimentacionIngrediente = new AlimentacionIngrediente
            {
                Id = relacionId,
                IdAlimentacion = 1,
                IdIngrediente = 2
            };

            _unidadTrabajoMock.Setup(u => u.AlimentacionIngrediente.ObtenerPrimero(It.IsAny<Expression<Func<AlimentacionIngrediente, bool>>>(), null, true))
                .ReturnsAsync(alimentacionIngrediente);

            var resultado = await _controller.Upsert(1, relacionId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (AlimentacionIngredienteVM)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.IdRelacion, Is.EqualTo(relacionId));
        }

        // Verifica que el método Upsert cree una nuevo ingrediente en producto de alimentación cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_AgregarNuevoIngrediente_Exito()
        {
            // Arrange
            var alimentacionIngredienteVM = new AlimentacionIngredienteVM
            {
                IdAlimentacion = 1,
                IdRelacion = 0,
                IdIngrediente = 2
            };

            // Mock de IUrlHelper
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("fake-url"); 

            _controller.Url = urlHelperMock.Object; // Asigna el mock al controlador.

            _unidadTrabajoMock.Setup(u => u.Ingrediente.ObtenerPrimero(
                It.IsAny<Expression<Func<Ingrediente, bool>>>(),
                null,
                true))
                .ReturnsAsync(new Ingrediente { Id = 2, Nombre = "Leche condensada" });

            _unidadTrabajoMock.Setup(u => u.Alimentacion.ObtenerPrimero(
                It.IsAny<Expression<Func<Alimentacion, bool>>>(),
                null,
                true))
                .ReturnsAsync(new Alimentacion { Id = 1, Nombre = "Granizado" });

            _unidadTrabajoMock.Setup(u => u.AlimentacionIngrediente.Agregar(It.IsAny<AlimentacionIngrediente>()))
                .Verifiable();

            var resultado = await _controller.Upsert(alimentacionIngredienteVM);

            Assert.That(_controller.TempData[DS.Exitosa], Is.EqualTo("Ingrediente agregado exitosamente"));
            _unidadTrabajoMock.Verify(u => u.AlimentacionIngrediente.Agregar(It.IsAny<AlimentacionIngrediente>()), Times.Once);
        }

        // Verifica que un ingrediente asociado a un produucto de alimentación sea eliminado correctamente del producto de alimentación.
        [Test]
        public async Task Delete_RetornaExito_CuandoElAlimentacionIngredienteExiste()
        {
            var alimentacionIngredienteId = 1;
            var alimentacionIngredienteBd = new AlimentacionIngrediente
            {
                Id = alimentacionIngredienteId,
                IdAlimentacion = 1,
                IdIngrediente = 2
            };

            var ingrediente = new Ingrediente
            {
                Id = 2,
                Nombre = "Aceite"
            };

            var alimentacion = new Alimentacion
            {
                Id = 1,
                Nombre = "Palomitas"
            };

            _unidadTrabajoMock.Setup(u => u.AlimentacionIngrediente.Obtener(alimentacionIngredienteId)).ReturnsAsync(alimentacionIngredienteBd);
            _unidadTrabajoMock.Setup(u => u.AlimentacionIngrediente.Remover(It.IsAny<AlimentacionIngrediente>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Ingrediente.ObtenerPrimero(It.IsAny<Expression<Func<Ingrediente, bool>>>(), null, true))
                              .ReturnsAsync(ingrediente);
            _unidadTrabajoMock.Setup(u => u.Alimentacion.ObtenerPrimero(It.IsAny<Expression<Func<Alimentacion, bool>>>(), null, true))
                              .ReturnsAsync(alimentacion);

            var resultado = await _controller.Delete(alimentacionIngredienteId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("true"));
            _unidadTrabajoMock.Verify(u => u.AlimentacionIngrediente.Remover(It.IsAny<AlimentacionIngrediente>()), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar una ingrediente de un producto de alimentación que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoElAlimentacionIngredienteNoExiste()
        {
            var alimentacionIngredienteId = 999;
            AlimentacionIngrediente alimentacionIngredienteBd = null;

            _unidadTrabajoMock.Setup(u => u.AlimentacionIngrediente.Obtener(alimentacionIngredienteId)).ReturnsAsync(alimentacionIngredienteBd);

            var resultado = await _controller.Delete(alimentacionIngredienteId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Ingrediente de producto de Alimentación"));
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