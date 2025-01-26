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
    public class CategoriaTamannoControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private CategoriaTamannoController _controller;
        private Mock<HttpContext> _httpContextMock;
        private Mock<ITempDataDictionary> _tempDataMock;


        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new CategoriaTamannoController(_unidadTrabajoMock.Object);
            
            //Simula el login y creacion de un usuario
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));

            //Simula el HTTP necesario para el funcionamiento de la prueba
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            //Simula la existencia del TempData 
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            {
                [DS.Exitosa] = "Success message",
                [DS.Error] = "Error message"
            };
            _controller.TempData = tempData;

            // Simula los metodo para Bitacora 
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
            var modelo = (CategoriaTamanno)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0)); 
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var categoriasTamañoId = 1;
            var categoriaTamanno = new CategoriaTamanno
            { 
                Id = categoriasTamañoId,
                Nombre = "Mediano" 
            };
            _unidadTrabajoMock.Setup(u => u.CategoriaTamanno.Obtener(categoriasTamañoId)).ReturnsAsync(categoriaTamanno);

            var resultado = await _controller.Upsert(categoriasTamañoId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (CategoriaTamanno)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);  
            Assert.That(modelo.Id, Is.EqualTo(categoriasTamañoId)); 
        }


        // Verifica que el método Upsert cree una nueva categoría por tamaño cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaCategoriaTamanno()
        {
            var categoriaTamannoId = 0;
            var categoriaTamanno = new CategoriaTamanno
            {
                Id = categoriaTamannoId,
                Nombre = "Grandes"
            };

            _unidadTrabajoMock.Setup(u => u.CategoriaTamanno.Agregar(It.IsAny<CategoriaTamanno>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(categoriaTamanno);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); 
            _unidadTrabajoMock.Verify(u => u.CategoriaTamanno.Agregar(It.IsAny<CategoriaTamanno>()), Times.Once);
        }

        // Verifica que el método Upsert actualice una categoría por tamaño existente.
        [Test]
        public async Task Upsert_Post_ActualizaCategoriaTamanno()
        {
            var categoriaTamannoId = 1;
            var categoriaTamannoExistente = new CategoriaTamanno
            {
                Id = categoriaTamannoId,
                Nombre = "Mediano"
            };

            var categoriaTamannoActualizada = new CategoriaTamanno
            {
                Id = categoriaTamannoId,
                Nombre = "Pequeño"
            };

            _unidadTrabajoMock.Setup(u => u.CategoriaTamanno.Obtener(categoriaTamannoId)).ReturnsAsync(categoriaTamannoExistente);
            _unidadTrabajoMock.Setup(u => u.CategoriaTamanno.Actualizar(It.IsAny<CategoriaTamanno>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(categoriaTamannoActualizada);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); 
            _unidadTrabajoMock.Verify(u => u.CategoriaTamanno.Actualizar(It.IsAny<CategoriaTamanno>()), Times.Once); 
        }

        // Verifica que el método Delete elimine correctamente una categoría por tamaño.
        [Test]
        public async Task Delete_RetornaExito_CuandoLaCategoriaTamannoSeEliminaCorrectamente()
        {
            var categoriaTamannoId = 1;
            var categoriaTamanno = new CategoriaTamanno
            {
                Id = categoriaTamannoId,
                Nombre = "Pequeño"
            };

            _unidadTrabajoMock.Setup(u => u.CategoriaTamanno.Obtener(categoriaTamannoId)).ReturnsAsync(categoriaTamanno);

            _unidadTrabajoMock.Setup(u => u.CategoriaTamanno.Remover(It.IsAny<CategoriaTamanno>())).Verifiable();

            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(categoriaTamannoId);

            _unidadTrabajoMock.Setup(u => u.CategoriaTamanno.Obtener(categoriaTamannoId)).ReturnsAsync((CategoriaTamanno)null);

            var categoriaTamannoEliminada = await _unidadTrabajoMock.Object.CategoriaTamanno.Obtener(categoriaTamannoId);

            Assert.That(categoriaTamannoEliminada, Is.Null);
            _unidadTrabajoMock.Verify(u => u.CategoriaTamanno.Remover(It.IsAny<CategoriaTamanno>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once);
        }

        // Verifica que el método ValidarNombre retorne false si el nombre no está duplicado.
        [Test]
        public async Task ValidarNombre_NombreNoDuplicado_RetornaFalse()
        {
            var nombreNuevo = "Mediano";
            var listaCategorias = new List<CategoriaTamanno>
            {
                new CategoriaTamanno { Id = 1, Nombre = "Grande" },
                new CategoriaTamanno { Id = 2, Nombre = "Pequeño  " }
            };

            _unidadTrabajoMock.Setup(u => u.CategoriaTamanno.ObtenerTodos(It.IsAny<Expression<Func<CategoriaTamanno, bool>>>(), null, null, true))
                .ReturnsAsync(listaCategorias);

            var resultado = await _controller.ValidarNombre(nombreNuevo);

            var jsonResult = resultado as JsonResult;
            var dataPropiedad = jsonResult.Value.GetType().GetProperty("data");
            var dataValor = dataPropiedad.GetValue(jsonResult.Value);
            Assert.That(dataValor, Is.False, "El valor de 'data' no es false.");
        }

        // Verifica que el método ValidarNombre retorne true si el nombre está duplicado.
        [Test]
        public async Task ValidarNombre_NombreDuplicado_RetornaTrue()
        {
            var nombreNuevo = "Pequeño";
            var listaCategorias = new List<CategoriaTamanno>
            {
                new CategoriaTamanno { Id = 1, Nombre = "Pequeño" },
                new CategoriaTamanno { Id = 2, Nombre = "Grande" }
            };

            _unidadTrabajoMock.Setup(u => u.CategoriaTamanno.ObtenerTodos(It.IsAny<Expression<Func<CategoriaTamanno, bool>>>(), null, null, true))
                .ReturnsAsync(listaCategorias);

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

