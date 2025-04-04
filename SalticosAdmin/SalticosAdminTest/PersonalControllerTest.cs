﻿using Microsoft.AspNetCore.Mvc;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class PersonalControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private PersonalController _controller;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _controller = new PersonalController(_unidadTrabajoMock.Object, _webHostEnvironmentMock.Object);

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
        //[Test]
        //public void Index_RetornaVista()
        //{
        //    var resultado = _controller.Index();

        //    Assert.That(resultado, Is.InstanceOf<ViewResult>());
        //}

        // Verifica que, cuando el ID es null, el método Upsert retorne una vista con un modelo vacío.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloVacioCuandoIdEsNull()
        {
            _unidadTrabajoMock.Setup(u => u.Personal.ObtenerTodosDropdownLista("RolPersonal"))
                .Returns(new List<SelectListItem>());

            var resultado = await _controller.Upsert((int?)null);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (PersonalVM)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Personal.Id, Is.EqualTo(0));
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var personalId = 1;
            var personal = new Personal
            {
                Id = personalId,
                Nombre = "John",
                Apellidos = "Doe",
                FechaNacimiento = DateTime.Now.AddYears(-25),
                RolPersonalId = 1
            };
            _unidadTrabajoMock.Setup(u => u.Personal.Obtener(personalId)).ReturnsAsync(personal);

            var resultado = await _controller.Upsert(personalId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (PersonalVM)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Personal.Id, Is.EqualTo(personalId));
        }

        // Verifica que el método Upsert cree un nuevo personal cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaPersonal()
        {
            var personalVM = new PersonalVM
            {
                Personal = new Personal
                {
                    Id = 0,
                    Nombre = "Alejandro",
                    Apellidos = "Campos",
                    FechaNacimiento = DateTime.Now.AddYears(-25),
                    RolPersonalId = 1
                }
            };

            _unidadTrabajoMock.Setup(u => u.Personal.Agregar(It.IsAny<Personal>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Upsert(personalVM);

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.Personal.Agregar(It.IsAny<Personal>()), Times.Once);
        }

        // Verifica que el método Upsert actualice un personal existente.
        [Test]
        public async Task Upsert_Post_ActualizaPersonal()
        {
            var personalId = 1;
            var personalExistente = new Personal
            {
                Id = personalId,
                Nombre = "Alejandro",
                Apellidos = "Campos",
                FechaNacimiento = DateTime.Now.AddYears(-25),
                RolPersonalId = 1
            };

            var personalActualizado = new Personal
            {
                Id = personalId,
                Nombre = "Alejandro",
                Apellidos = "Campos Paredes",
                FechaNacimiento = DateTime.Now.AddYears(-25),
                RolPersonalId = 1
            };

            _unidadTrabajoMock.Setup(u => u.Personal.Obtener(personalId)).ReturnsAsync(personalExistente);
            _unidadTrabajoMock.Setup(u => u.Personal.Actualizar(It.IsAny<Personal>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var result = await _controller.Upsert(new PersonalVM { Personal = personalActualizado });

            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            _unidadTrabajoMock.Verify(u => u.Personal.Actualizar(It.IsAny<Personal>()), Times.Once);
        }

        // Verifica que el método Delete elimine correctamente un ingrediente.
        [Test]
        public async Task Delete_RetornaExito_CuandoElPersonalExiste()
        {
            var personalId = 1;
            var personalBd = new Personal
            {
                Id = personalId,
                Nombre = "Daniela",
                Apellidos = "Chaves"
            };

            _unidadTrabajoMock.Setup(u => u.Personal.Obtener(personalId)).ReturnsAsync(personalBd);
            _unidadTrabajoMock.Setup(u => u.Personal.Remover(It.IsAny<Personal>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(personalId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["success"].ToString().ToLower(), Is.EqualTo("true"));
            _unidadTrabajoMock.Verify(u => u.Personal.Remover(It.IsAny<Personal>()), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar un personal que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoElPersonalNoExiste()
        {
            var personalId = 999;
            Personal personalBd = null;

            _unidadTrabajoMock.Setup(u => u.Personal.Obtener(personalId)).ReturnsAsync(personalBd);

            var resultado = await _controller.Delete(personalId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Personal"));
        }

        // Verifica que el método ValidarCedula retorne fase si el cédula no está duplicado.
        [Test]
        public async Task ValidarCedula_CedulaNoDuplicada_RetornaFalse()
        {
            var cedulaNueva = "123456789";
            var listaPersonal = new List<Personal>
            {
                new Personal { Id = 1, Cedula = "987654321" },
                new Personal { Id = 2, Cedula = "456789123" }
            };

            _unidadTrabajoMock.Setup(u => u.Personal.ObtenerTodos(It.IsAny<Expression<Func<Personal, bool>>>(), null, null, true))
                .ReturnsAsync(listaPersonal);

            var resultado = await _controller.ValidarCedula(cedulaNueva);

            var jsonResult = resultado as JsonResult;
            var dataPropiedad = jsonResult.Value.GetType().GetProperty("data");
            var dataValor = dataPropiedad.GetValue(jsonResult.Value);
            Assert.That(dataValor, Is.False, "El valor de 'data' no es false.");
        }

        // Verifica que el método ValidarCedula retorne true si la cédula está duplicada.
        [Test]
        public async Task ValidarCedula_CedulaDuplicada_RetornaTrue()
        {
            var cedulaNueva = "456789123";
            var listaPersonal = new List<Personal>
            {
                new Personal { Id = 1, Cedula = "123456789" },
                new Personal { Id = 2, Cedula = "456789123" }
            };

            _unidadTrabajoMock.Setup(u => u.Personal.ObtenerTodos(It.IsAny<Expression<Func<Personal, bool>>>(), null, null, true))
                .ReturnsAsync(listaPersonal);

            var resultado = await _controller.ValidarCedula(cedulaNueva);

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
