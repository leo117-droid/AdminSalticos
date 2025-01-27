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
    public class IngredienteControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private IngredienteController _controller;
        private Mock<HttpContext> _httpContextMock;
        private Mock<ITempDataDictionary> _tempDataMock;


        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new IngredienteController(_unidadTrabajoMock.Object);
            
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
            var modelo = (Ingrediente)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0)); 
        }

        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var ingredienteId = 1;
            var ingrediente = new Ingrediente 
            { 
                Id = ingredienteId,
                Nombre = "Aceite",
                Descripcion = "Botella aceite de girasol de 2 litros",
                Precio = 2100
            };
            _unidadTrabajoMock.Setup(u => u.Ingrediente.Obtener(ingredienteId)).ReturnsAsync(ingrediente);

            var resultado = await _controller.Upsert(ingredienteId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Ingrediente)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);  
            Assert.That(modelo.Id, Is.EqualTo(ingredienteId)); 
        }


        // Verifica que el método Upsert cree un nuevo ingrediente cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaIngrediente()
        {
            var ingredienteId = 0;
            var ingrediente = new Ingrediente
            {
                Id = ingredienteId,
                Nombre = "Salchichas",
                Descripcion = "Paquete de 12 salchichas de pollo",
                Precio = 3750
            };

            _unidadTrabajoMock.Setup(u => u.Ingrediente.Agregar(It.IsAny<Ingrediente>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(ingrediente);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); 
            _unidadTrabajoMock.Verify(u => u.Ingrediente.Agregar(It.IsAny<Ingrediente>()), Times.Once);
        }

        // Verifica que el método Upsert actualice un ingrediente existente.
        [Test]
        public async Task Upsert_Post_ActualizaIngrediente()
        {
            var ingredienteId = 1;
            var ingredienteExistente = new Ingrediente
            {
                Id = ingredienteId,
                Nombre = "Colorante",
                Descripcion = "500 gramos de colorante comestible color rosado",
                Precio = 5400
            };

            var ingredienteActualizada = new Ingrediente
            {
                Id = ingredienteId,
                Nombre = "Colorante",
                Descripcion = "380 gramos de colorante comestible color rosado",
                Precio = 4200
            };

            _unidadTrabajoMock.Setup(u => u.Ingrediente.Obtener(ingredienteId)).ReturnsAsync(ingredienteExistente);
            _unidadTrabajoMock.Setup(u => u.Ingrediente.Actualizar(It.IsAny<Ingrediente>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(ingredienteActualizada);

            var redirectResult = resultado as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); 
            _unidadTrabajoMock.Verify(u => u.Ingrediente.Actualizar(It.IsAny<Ingrediente>()), Times.Once); 
        }

        // Verifica que el método Delete elimine correctamente un ingrediente.
        [Test]
        public async Task Delete_RetornaExito_CuandoElIngredienteSeEliminaCorrectamente()
        {
            var ingredienteId = 1;
            var ingrediente = new Ingrediente
            {
                Id = ingredienteId,
                Nombre = "Maiz",
                Descripcion = "10kg de maiz para palomitas",
                Precio = 7100
            };

            _unidadTrabajoMock.Setup(u => u.Ingrediente.Obtener(ingredienteId)).ReturnsAsync(ingrediente);

            _unidadTrabajoMock.Setup(u => u.Ingrediente.Remover(It.IsAny<Ingrediente>())).Verifiable();

            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(ingredienteId);

            _unidadTrabajoMock.Setup(u => u.Ingrediente.Obtener(ingredienteId)).ReturnsAsync((Ingrediente)null);

            var ingredienteEliminado = await _unidadTrabajoMock.Object.Ingrediente.Obtener(ingredienteId);

            Assert.That(ingredienteEliminado, Is.Null);
            _unidadTrabajoMock.Verify(u => u.Ingrediente.Remover(It.IsAny<Ingrediente>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar un ingrediente que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoElIngredienteNoExiste()
        {
            var ingredienteId = 999;
            Ingrediente ingredienteBd = null;

            _unidadTrabajoMock.Setup(u => u.Ingrediente.Obtener(ingredienteId)).ReturnsAsync(ingredienteBd);

            var resultado = await _controller.Delete(ingredienteId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;

            var contenido = JObject.FromObject(jsonResult.Value);

            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Ingrediente"));
        }

        // Verifica que el método ValidarNombre retorne false si el nombre no está duplicado.
        [Test]
        public async Task ValidarNombre_NombreNoDuplicado_RetornaFalse()
        {
            var nombreNuevo = "Papas Tostadas";
            var listaIngredientes = new List<Ingrediente>
            {
                new Ingrediente { Id = 1, Nombre = "Azúcar", Descripcion = "8kg de azúcar blanca", Precio = 6000 },
                new Ingrediente { Id = 1, Nombre = "Mayonesa", Descripcion = "Bolsa 500 gramos de mayonesa", Precio = 1000}
            };

            _unidadTrabajoMock.Setup(u => u.Ingrediente.ObtenerTodos(It.IsAny<Expression<Func<Ingrediente, bool>>>(), null, null, true))
                .ReturnsAsync(listaIngredientes);

            var resultado = await _controller.ValidarNombre(nombreNuevo);

            var jsonResult = resultado as JsonResult;
            var dataPropiedad = jsonResult.Value.GetType().GetProperty("data");
            var dataValor = dataPropiedad.GetValue(jsonResult.Value);
            Assert.That(dataValor, Is.False, "El valor de 'data' no es false.");
        }

        // Verifica que el método ValidarNombre retorne true si el nombre está duplicado.
        public async Task ValidarNombre_NombreDuplicado_RetornaTrue()
        {
            var nombreNuevo = "Papas Tostadas";
            var listaIngredientes = new List<Ingrediente>
            {
                new Ingrediente { Id = 1, Nombre = "Papas Tostadas", Descripcion = "700 gramos de papas tostadas con sal", Precio = 1300 },
                new Ingrediente { Id = 1, Nombre = "Mayonesa", Descripcion = "Bolsa 500 gramos de mayonesa", Precio = 1000}
            };

            _unidadTrabajoMock.Setup(u => u.Ingrediente.ObtenerTodos(It.IsAny<Expression<Func<Ingrediente, bool>>>(), null, null, true))
                .ReturnsAsync(listaIngredientes);

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

