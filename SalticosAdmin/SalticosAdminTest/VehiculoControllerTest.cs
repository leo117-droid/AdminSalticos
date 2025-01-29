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
    public class VehiculoControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private VehiculoController _controller;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new VehiculoController(_unidadTrabajoMock.Object);

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
            var modelo = (Vehiculo)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0));
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var vehiculoId = 1;
            var vehiculo = new Vehiculo
            {
                Id = vehiculoId,
                Placa = "ABC123",
                Marca = "Toyota",
                Modelo = "Corolla"

            };
            _unidadTrabajoMock.Setup(u => u.Vehiculo.Obtener(vehiculoId)).ReturnsAsync(vehiculo);

            var resultado = await _controller.Upsert(vehiculoId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Vehiculo)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(vehiculoId));
        }

        // Verifica que el método Upsert cree un nuevo vehículo cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaVehiculo()
        {
            var vehiculo = new Vehiculo
            {
                Id = 0,
                Placa = "DEF456",
                Marca = "Honda",
                Modelo = "Civic"

            };

            _unidadTrabajoMock.Setup(u => u.Vehiculo.Agregar(It.IsAny<Vehiculo>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Upsert(vehiculo);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.Vehiculo.Agregar(It.IsAny<Vehiculo>()), Times.Once);
        }

        // Verifica que el método Upsert actualice un vehículo existente.
        [Test]
        public async Task Upsert_Post_ActualizaVehiculo()
        {
            var vehiculoId = 1;
            var vehiculoExistente = new Vehiculo
            {
                Id = vehiculoId,
                Placa = "ABC123",
                Marca = "Toyota",
                Modelo = "Corolla"

            };

            var vehiculoActualizado = new Vehiculo
            {
                Id = vehiculoId,
                Placa = "XYZ789",
                Marca = "Toyota",
                Modelo = "Corolla"

            };

            _unidadTrabajoMock.Setup(u => u.Vehiculo.Obtener(vehiculoId)).ReturnsAsync(vehiculoExistente);
            _unidadTrabajoMock.Setup(u => u.Vehiculo.Actualizar(It.IsAny<Vehiculo>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Upsert(vehiculoActualizado);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.Vehiculo.Actualizar(It.IsAny<Vehiculo>()), Times.Once);
        }

        // Verifica que el método Delete elimine correctamente un vehiculo.
        [Test]
        public async Task Delete_RetornaExito_CuandoElVehiculoExiste()
        {
            var vehiculoId = 1;
            var vehiculoBd = new Vehiculo
            {
                Id = vehiculoId,
                Placa = "ABC123",
                Marca = "Toyota",
                Modelo = "Corolla"

            };

            _unidadTrabajoMock.Setup(u => u.Vehiculo.Obtener(vehiculoId)).ReturnsAsync(vehiculoBd);
            _unidadTrabajoMock.Setup(u => u.Vehiculo.Remover(It.IsAny<Vehiculo>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(vehiculoId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("true"));
            _unidadTrabajoMock.Verify(u => u.Vehiculo.Remover(It.IsAny<Vehiculo>()), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar un vehiculo que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoElVehiculoNoExiste()
        {
            var vehiculoId = 999;
            Vehiculo vehiculoBd = null;

            _unidadTrabajoMock.Setup(u => u.Vehiculo.Obtener(vehiculoId)).ReturnsAsync(vehiculoBd);

            var resultado = await _controller.Delete(vehiculoId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Vehiculo"));
        }

        // Verifica que el método ValidarPlaca retorne false si la placa no está duplicada.
        [Test]
        public async Task ValidarPlaca_PlacaNoDuplicada_RetornaFalse()
        {
            var placaNueva = "DEF456";
            var listaVehiculos = new List<Vehiculo>
            {
                new Vehiculo { Id = 1, Placa = "ABC123" },
                new Vehiculo { Id = 2, Placa = "GHI789" }
            };

            _unidadTrabajoMock.Setup(u => u.Vehiculo.ObtenerTodos(It.IsAny<Expression<Func<Vehiculo, bool>>>(), null, null, true))
                .ReturnsAsync(listaVehiculos);

            var resultado = await _controller.ValidarPlaca(placaNueva);

            var jsonResult = resultado as JsonResult;
            var dataPropiedad = jsonResult.Value.GetType().GetProperty("data");
            var dataValor = dataPropiedad.GetValue(jsonResult.Value);
            Assert.That(dataValor, Is.False, "El valor de 'data' no es false.");
        }

        // Verifica que el método ValidarPlaca retorne true si la placa está duplicada.
        [Test]
        public async Task ValidarPlaca_PlacaDuplicada_RetornaTrue()
        {
            var placaNueva = "GHI789";
            var listaVehiculos = new List<Vehiculo>
            {
                new Vehiculo { Id = 1, Placa = "ABC123" },
                new Vehiculo { Id = 2, Placa = "GHI789" }
            };

            _unidadTrabajoMock.Setup(u => u.Vehiculo.ObtenerTodos(It.IsAny<Expression<Func<Vehiculo, bool>>>(), null, null, true))
                .ReturnsAsync(listaVehiculos);

            var resultado = await _controller.ValidarPlaca(placaNueva);

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
