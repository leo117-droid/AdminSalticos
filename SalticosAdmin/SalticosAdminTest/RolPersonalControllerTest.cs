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
    public class RolPersonalControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private RolPersonalController _controller;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new RolPersonalController(_unidadTrabajoMock.Object);
            
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
            var modelo = (RolPersonal)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0));
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var rolId = 1;
            var rol = new RolPersonal
            {
                Id = rolId,
                Nombre = "Chofer"
            };
            _unidadTrabajoMock.Setup(u => u.RolPersonal.Obtener(rolId)).ReturnsAsync(rol);

            var resultado = await _controller.Upsert(rolId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (RolPersonal)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(rolId));
        }

        // Verifica que el método Upsert cree un nuevo rol de personal cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaNuevoRolPersonal()
        {
            var nuevoRol = new RolPersonal
            {
                Id = 0,
                Nombre = "Asistente"
            };

            _unidadTrabajoMock.Setup(u => u.RolPersonal.Agregar(It.IsAny<RolPersonal>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(nuevoRol);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.RolPersonal.Agregar(It.IsAny<RolPersonal>()), Times.Once);
        }

        // Verifica que el método Upsert actualice un rol de personal existente.
        [Test]
        public async Task Upsert_Post_ActualizaRolPersonal()
        {
            var rolPersonalId = 1;
            var rolPersonalExistente = new RolPersonal
            {
                Id = rolPersonalId,
                Nombre = "Asistente"
            };

            var rolPersonalActualizado = new RolPersonal
            {
                Id = rolPersonalId,
                Nombre = "Asistente"
            };

            _unidadTrabajoMock.Setup(u => u.RolPersonal.Obtener(rolPersonalId)).ReturnsAsync(rolPersonalExistente);
            _unidadTrabajoMock.Setup(u => u.RolPersonal.Actualizar(It.IsAny<RolPersonal>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(rolPersonalActualizado);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.RolPersonal.Actualizar(It.IsAny<RolPersonal>()), Times.Once);
        }

        // Verifica que el método Delete elimine correctamente un rol de personal.
        [Test]
        public async Task Delete_RetornaExito_CuandoRolSeEliminaCorrectamente()
        {
            var rolId = 1;
            var rol = new RolPersonal
            {
                Id = rolId,
                Nombre = "Auditor",
            };

            _unidadTrabajoMock.Setup(u => u.RolPersonal.Obtener(rolId)).ReturnsAsync(rol);
            _unidadTrabajoMock.Setup(u => u.RolPersonal.Remover(It.IsAny<RolPersonal>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(rolId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            _unidadTrabajoMock.Verify(u => u.RolPersonal.Remover(It.IsAny<RolPersonal>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar un rol de personal que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoRolNoExiste()
        {
            var rolId = 999;

            _unidadTrabajoMock.Setup(u => u.RolPersonal.Obtener(rolId)).ReturnsAsync((RolPersonal)null);

            var resultado = await _controller.Delete(rolId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar rol de personal"));
        }

        // Verifica que el método ValidarNombre retorne false si el nombre del rol no está duplicado.
        [Test]
        public async Task ValidarNombreRol_NombreNoDuplicado_RetornaFalse()
        {
            var nombreNuevo = "Chofer";
            var listaRoles = new List<RolPersonal>
            {
                new RolPersonal { Id = 1, Nombre = "Animador" },
                new RolPersonal { Id = 2, Nombre = "Asistente" }
            };

            _unidadTrabajoMock.Setup(u => u.RolPersonal.ObtenerTodos(It.IsAny<Expression<Func<RolPersonal, bool>>>(), null, null, true))
                .ReturnsAsync(listaRoles);

            var resultado = await _controller.ValidarNombre(nombreNuevo);

            var jsonResult = resultado as JsonResult;
            var dataPropiedad = jsonResult.Value.GetType().GetProperty("data");
            var dataValor = dataPropiedad.GetValue(jsonResult.Value);
            Assert.That(dataValor, Is.False, "El valor de 'data' no es false.");
        }

        // Verifica que el método ValidarNombre retorne true si el nombre del rol está duplicado.
        [Test]
        public async Task ValidarNombreRol_NombreDuplicado_RetornaTrue()
        {
            var nombreNuevo = "Chofer";
            var listaRoles = new List<RolPersonal>
            {
                new RolPersonal { Id = 1, Nombre = "Chef" },
                new RolPersonal { Id = 2, Nombre = "Chofer" }
            };

            _unidadTrabajoMock.Setup(u => u.RolPersonal.ObtenerTodos(It.IsAny<Expression<Func<RolPersonal, bool>>>(), null, null, true))
                .ReturnsAsync(listaRoles);

            var resultado = await _controller.ValidarNombre(nombreNuevo);

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

