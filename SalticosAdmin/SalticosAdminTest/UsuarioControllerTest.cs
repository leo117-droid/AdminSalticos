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
using SalticosAdmin.AccesoDeDatos.Data;
using Microsoft.EntityFrameworkCore;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class UsuarioControllerTests
    {
        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private UsuarioController _controller;
        private Mock<HttpContext> _httpContextMock;
        private Mock<ITempDataDictionary> _tempDataMock;
        private Mock<ApplicationDbContext> _dbContextMock;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _dbContextMock = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _controller = new UsuarioController(_unidadTrabajoMock.Object, _dbContextMock.Object);

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
        public async Task Edit_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var usuarioId = "1";
            var usuario = new Usuario
            {
                Id = usuarioId,
                Nombre = "John",
                Apellido = "Doe",
                Email = "john.doe@example.com"
            };
            _unidadTrabajoMock.Setup(u => u.Usuario.ObtenerPorIdAsync(usuarioId)).ReturnsAsync(usuario);

            var resultado = await _controller.Edit(usuarioId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Usuario)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(usuarioId));
        }

        [Test]
        public async Task Edit_Post_ActualizaUsuario()
        {
            var usuario = new Usuario
            {
                Id = "1",
                Nombre = "John",
                Apellido = "Doe",
                Email = "john.doe@example.com"
            };

            _unidadTrabajoMock.Setup(u => u.Usuario.Actualizar(It.IsAny<Usuario>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Edit(usuario);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.Usuario.Actualizar(It.IsAny<Usuario>()), Times.Once);
        }

        [Test]
        public async Task Delete_RetornaExito_CuandoElUsuarioExiste()
        {
            var usuarioId = "1";
            var usuarioBd = new Usuario
            {
                Id = usuarioId,
                Nombre = "John",
                Apellido = "Doe"
            };

            _unidadTrabajoMock.Setup(u => u.Usuario.ObtenerPorIdAsync(usuarioId)).ReturnsAsync(usuarioBd);
            _unidadTrabajoMock.Setup(u => u.Usuario.Remover(It.IsAny<Usuario>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(usuarioId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("true"));
            _unidadTrabajoMock.Verify(u => u.Usuario.Remover(It.IsAny<Usuario>()), Times.Once);
        }

        [Test]
        public async Task Delete_RetornaError_CuandoElUsuarioNoExiste()
        {
            var usuarioId = "999";
            Usuario usuarioBd = null;

            _unidadTrabajoMock.Setup(u => u.Usuario.ObtenerPorIdAsync(usuarioId)).ReturnsAsync(usuarioBd);

            var resultado = await _controller.Delete(usuarioId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Usuario"));
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
