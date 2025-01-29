using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SalticosAdmin.Areas.Admin.Controllers;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Modelos;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SalticosAdmin.Utilidades;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class CategoriaEdadControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private CategoriasEdadController _controller;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new CategoriasEdadController(_unidadTrabajoMock.Object);
            
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
            var modelo = (CategoriasEdad)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0)); 
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var categoriasEdadId = 1;
            var categoriasEdad = new CategoriasEdad 
            { 
                Id = categoriasEdadId,
                Nombre = "Menores 8 años" 
            };
            _unidadTrabajoMock.Setup(u => u.CategoriasEdad.Obtener(categoriasEdadId)).ReturnsAsync(categoriasEdad);

            var resultado = await _controller.Upsert(categoriasEdadId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (CategoriasEdad)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);  
            Assert.That(modelo.Id, Is.EqualTo(categoriasEdadId)); 
        }


        // Verifica que el método Upsert cree una nueva categoría por edad cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaCategoriaEdad()
        {
            var categoriasEdadId = 0;
            var categoriasEdad = new CategoriasEdad
            {
                Id = categoriasEdadId,
                Nombre = "Menores 10 años"
            };

            _unidadTrabajoMock.Setup(u => u.CategoriasEdad.Agregar(It.IsAny<CategoriasEdad>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(categoriasEdad);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); 
            _unidadTrabajoMock.Verify(u => u.CategoriasEdad.Agregar(It.IsAny<CategoriasEdad>()), Times.Once);
        }

        // Verifica que el método Upsert actualice una categoría por edad existente.
        [Test]
        public async Task Upsert_Post_ActualizaCategoriasEdad()
        {
            var categoriasEdadId = 1;
            var categoriasEdadExistente = new CategoriasEdad
            {
                Id = 1,
                Nombre = "Todas las edades"
            };

            var categoriasEdadActualizada = new CategoriasEdad
            {
                Id = categoriasEdadId,
                Nombre = "Todas las edades"
            };

            _unidadTrabajoMock.Setup(u => u.CategoriasEdad.Obtener(categoriasEdadId)).ReturnsAsync(categoriasEdadExistente);
            _unidadTrabajoMock.Setup(u => u.CategoriasEdad.Actualizar(It.IsAny<CategoriasEdad>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(categoriasEdadActualizada);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); 
            _unidadTrabajoMock.Verify(u => u.CategoriasEdad.Actualizar(It.IsAny<CategoriasEdad>()), Times.Once); 
        }

        // Verifica que el método Delete elimine correctamente una categoría por edad.
        [Test]
        public async Task Delete_RetornaExito_CuandoLaCategoriaEdadSeEliminaCorrectamente()
        {
            var categoriaEdadId = 1;
            var categoriaEdad = new CategoriasEdad
            {
                Id = categoriaEdadId,
                Nombre = "Menores 10 años"
            };

            _unidadTrabajoMock.Setup(u => u.CategoriasEdad.Obtener(categoriaEdadId)).ReturnsAsync(categoriaEdad);

            _unidadTrabajoMock.Setup(u => u.CategoriasEdad.Remover(It.IsAny<CategoriasEdad>())).Verifiable();

            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(categoriaEdadId);

            _unidadTrabajoMock.Setup(u => u.CategoriasEdad.Obtener(categoriaEdadId)).ReturnsAsync((CategoriasEdad)null);

            var categoriaEdadEliminada = await _unidadTrabajoMock.Object.CategoriasEdad.Obtener(categoriaEdadId);

            Assert.That(categoriaEdadEliminada, Is.Null);
            _unidadTrabajoMock.Verify(u => u.CategoriasEdad.Remover(It.IsAny<CategoriasEdad>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once);
        }


        // Verifica que se retorne un error cuando se intenta eliminar una categoría por edad que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoLaCategoriaEdadNoExiste()
        {
            var categoriaEdadId = 999;
            CategoriasEdad categoriasEdadBd = null;

            _unidadTrabajoMock.Setup(u => u.CategoriasEdad.Obtener(categoriaEdadId)).ReturnsAsync(categoriasEdadBd);

            var resultado = await _controller.Delete(categoriaEdadId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Categoría por edad"));
        }

        // Verifica que el método ValidarNombre retorne false si el nombre no está duplicado.
        [Test]
        public async Task ValidarNombre_NombreNoDuplicado_RetornaFalse()
        {
            var nombreNuevo = "Menores 8 años";
            var listaCategorias = new List<CategoriasEdad>
            {
                new CategoriasEdad { Id = 1, Nombre = "Todas las edades" },
                new CategoriasEdad { Id = 2, Nombre = "Menores 5 años" }
            };

            _unidadTrabajoMock.Setup(u => u.CategoriasEdad.ObtenerTodos(It.IsAny<Expression<Func<CategoriasEdad, bool>>>(), null, null, true))
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
            var nombreNuevo = "Todas las edades";
            var listaCategorias = new List<CategoriasEdad>
            {
                new CategoriasEdad { Id = 1, Nombre = "Todas las edades" },
                new CategoriasEdad { Id = 2, Nombre = "Menores 5 años" }
            };

            _unidadTrabajoMock.Setup(u => u.CategoriasEdad.ObtenerTodos(It.IsAny<Expression<Func<CategoriasEdad, bool>>>(), null, null, true))
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

