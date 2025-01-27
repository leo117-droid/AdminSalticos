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
    public class ClienteControllerTests
    {
        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private ClienteController _controller;
        private Mock<HttpContext> _httpContextMock;
        private Mock<ITempDataDictionary> _tempDataMock;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new ClienteController(_unidadTrabajoMock.Object);

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
            var modelo = (Cliente)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0));
        }

        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var clienteId = 1;
            var cliente = new Cliente
            {
                Id = clienteId,
                Nombre = "Juan",
                Apellidos = "Perez",
                Cedula = "123456789"
            };
            _unidadTrabajoMock.Setup(u => u.Cliente.Obtener(clienteId)).ReturnsAsync(cliente);

            var resultado = await _controller.Upsert(clienteId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Cliente)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(clienteId));
        }

        [Test]
        public async Task Upsert_Post_CreaCliente()
        {
            var cliente = new Cliente
            {
                Id = 0,
                Nombre = "Maria",
                Apellidos = "Gomez",
                Cedula = "987654321"
            };

            _unidadTrabajoMock.Setup(u => u.Cliente.Agregar(It.IsAny<Cliente>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Upsert(cliente);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.Cliente.Agregar(It.IsAny<Cliente>()), Times.Once);
        }

        [Test]
        public async Task Upsert_Post_ActualizaCliente()
        {
            var clienteId = 1;
            var clienteExistente = new Cliente
            {
                Id = clienteId,
                Nombre = "Carlos",
                Apellidos = "Rojas",
                Cedula = "456123789"
            };

            var clienteActualizado = new Cliente
            {
                Id = clienteId,
                Nombre = "Carlos",
                Apellidos = "Rojas",
                Cedula = "456123789"
            };

            _unidadTrabajoMock.Setup(u => u.Cliente.Obtener(clienteId)).ReturnsAsync(clienteExistente);
            _unidadTrabajoMock.Setup(u => u.Cliente.Actualizar(It.IsAny<Cliente>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Upsert(clienteActualizado);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.Cliente.Actualizar(It.IsAny<Cliente>()), Times.Once);
        }

        [Test]
        public async Task Delete_RetornaExito_CuandoElClienteExiste()
        {
            var clienteId = 1;
            var clienteBd = new Cliente
            {
                Id = clienteId,
                Nombre = "Luis",
                Apellidos = "Ramirez",
                Cedula = "321654987"
            };

            _unidadTrabajoMock.Setup(u => u.Cliente.Obtener(clienteId)).ReturnsAsync(clienteBd);
            _unidadTrabajoMock.Setup(u => u.Cliente.Remover(It.IsAny<Cliente>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(clienteId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString(), Is.EqualTo("True"));
            _unidadTrabajoMock.Verify(u => u.Cliente.Remover(It.IsAny<Cliente>()), Times.Once);
        }

        [Test]
        public async Task Delete_RetornaError_CuandoElClienteNoExiste()
        {
            var clienteId = 999;
            Cliente clienteBd = null;

            _unidadTrabajoMock.Setup(u => u.Cliente.Obtener(clienteId)).ReturnsAsync(clienteBd);

            var resultado = await _controller.Delete(clienteId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Cliente"));
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
