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
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.FileProviders;
using SalticosAdmin.Modelos.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class InflableControllerTests
    {
        // Configuración inicial antes de cada prueba

        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private InflableController _controller;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;

        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _controller = new InflableController(_unidadTrabajoMock.Object, _webHostEnvironmentMock.Object);

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
        public async Task Index_RetornaVistaConModeloCorrecto()
        {
            var inflableLista = new List<Inflable>
            {
                new Inflable { Id = 1, Nombre = "Tsunami" },
                new Inflable { Id = 2, Nombre = "Gran Tobogán" }
            };

                    var categoriaTamannoLista = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Grande" },
                new SelectListItem { Value = "2", Text = "Mediano" }
            };

                    var categoriaEdadLista = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Menores 8 años" },
                new SelectListItem { Value = "2", Text = "Todas las edades" }
            };

            _unidadTrabajoMock.Setup(u => u.Inflable.ObtenerTodosDropdownLista("CategoriaTamanno"))
                              .Returns(categoriaTamannoLista);
            _unidadTrabajoMock.Setup(u => u.Inflable.ObtenerTodosDropdownLista("CategoriaEdad"))
                              .Returns(categoriaEdadLista);
            _unidadTrabajoMock.Setup(u => u.Inflable.ObtenerTodos(
                    null,
                    null,
                    "CategoriaTamanno,CategoriasEdad",
                    true))
                .ReturnsAsync(inflableLista);

            var resultado = await _controller.Index();

            Assert.That(resultado, Is.InstanceOf<ViewResult>()); 

            var vistaResultado = resultado as ViewResult;
            Assert.That(vistaResultado.Model, Is.InstanceOf<InflableVM>()); 

            var modelo = vistaResultado.Model as InflableVM;
            Assert.That(modelo, Is.Not.Null); 
            Assert.That(modelo.InflableLista.Count, Is.EqualTo(2)); 
            Assert.That(modelo.CategoriaTamannoLista.Count, Is.EqualTo(2));
            Assert.That(modelo.CategoriaEdadLista.Count, Is.EqualTo(2)); 
        }



        // Verifica que el método Intermedia retorne una vista correctamente.

        [Test]
        public void Intermedia_RetornaVista()
        {
            var resultado = _controller.Intermedia();

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
        }

        // Verifica que, cuando el ID es null, el método Upsert retorne una vista con un modelo vacío.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloVacioCuandoIdEsNull()
        {
        var categoriaTamannoLista = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Grande" },
            new SelectListItem { Value = "2", Text = "Pequeño" }
        };

        var categoriaEdadLista = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Todas las edades" },
            new SelectListItem { Value = "2", Text = "Menores de 10 años" }
        };

            _unidadTrabajoMock.Setup(u => u.Inflable.ObtenerTodosDropdownLista("CategoriaTamanno"))
                              .Returns(categoriaTamannoLista);
            _unidadTrabajoMock.Setup(u => u.Inflable.ObtenerTodosDropdownLista("CategoriaEdad"))
                              .Returns(categoriaEdadLista);

            var resultado = await _controller.Upsert((int?)null);

            Assert.That(resultado, Is.InstanceOf<ViewResult>(), "Se esperaba un ViewResult");

            var vistaResultado = resultado as ViewResult;
            Assert.That(vistaResultado.Model, Is.InstanceOf<InflableVM>(), "El modelo debería ser del tipo InflableVM");

            var modelo = vistaResultado.Model as InflableVM;
            Assert.That(modelo, Is.Not.Null, "El modelo no debería ser nulo");
            Assert.That(modelo.Inflable.Id, Is.EqualTo(0), "El ID del modelo Inflable debería ser 0");
            Assert.That(modelo.Inflable.Estado, Is.True, "El estado del modelo Inflable debería ser True");
            Assert.That(modelo.CategoriaTamannoLista, Is.EqualTo(categoriaTamannoLista), "La lista de categorías de tamaño no coincide");
            Assert.That(modelo.CategoriaEdadLista, Is.EqualTo(categoriaEdadLista), "La lista de categorías de edad no coincide");
        }


        // Verifica que, cuando se proporciona un ID válido, el método Upsert retorne una vista con el modelo correspondiente.
        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            var inflableId = 1;
            var inflable = new Inflable
            {
                Id = inflableId,
                Nombre = "Castillito",
                Descripcion = "Inflable en forma de castillo pequeño",
                Precio = 45000,
            };

            _unidadTrabajoMock.Setup(u => u.Inflable.Obtener(inflableId)).ReturnsAsync(inflable);

            var resultado = await _controller.Upsert(inflableId);

            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var vistaResultado = (ViewResult)resultado;

            Assert.That(vistaResultado.Model, Is.InstanceOf<InflableVM>());
            var modelo = (InflableVM)vistaResultado.Model;

            Assert.That(modelo.Inflable, Is.Not.Null);
            Assert.That(modelo.Inflable.Id, Is.EqualTo(inflableId));
            Assert.That(modelo.Inflable.Nombre, Is.EqualTo("Castillito"));
            Assert.That(modelo.Inflable.Descripcion, Is.EqualTo("Inflable en forma de castillo pequeño"));
            Assert.That(modelo.Inflable.Precio, Is.EqualTo(45000));
        }


        // Verifica que el método Upsert cree un nuevo inflable cuando el modelo no tiene un ID.
        [Test]
        public async Task Upsert_Post_CreaInflableMobiliarioConImagen()
        {
            var inflableVM = new InflableVM
            {
                Inflable = new Inflable
                {
                    Id = 0, 
                    Nombre = "Tsunami",
                    Descripcion = "Inflable acuático",
                    Precio = 160000,
                }
            };

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("imagen.jpg");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var files = new FormFileCollection { fileMock.Object };

            var formCollection = new FormCollection(new Dictionary<string, StringValues>(), files);
            _controller.ControllerContext.HttpContext.Request.Form = formCollection;

            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string inflablePath = Path.Combine(webRootPath, "imagenes", "inflable");
            Directory.CreateDirectory(inflablePath);

            _webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(webRootPath);

            _unidadTrabajoMock.Setup(u => u.Inflable.Agregar(It.IsAny<Inflable>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(inflableVM);
            _unidadTrabajoMock.Verify(u => u.Inflable.Agregar(It.IsAny<Inflable>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once);

            Assert.That(_controller.TempData[DS.Exitosa], Is.EqualTo("Inflable creado Exitosamente"));
            var redirectResult = (RedirectToActionResult)resultado;
        }


        // Verifica que el método Upsert actualice un inflable y actualice la nueva imagen.
        [Test]
        public async Task Upsert_Post_ActualizaInflableConImagen()
        {
            var inflableExistente = new Inflable
            {
                Id = 1,
                Nombre = "Casa Juegos",
                ImageUrl = "casaJuegos.jpg",
                Descripcion = "Inflable con obstáculos internos",
                Precio = 65000
            };

            var inflableActualizadoVM = new InflableVM
            {
                Inflable = new Inflable
                {
                    Id = 1,
                    Nombre = "Casa Juegos",
                    Descripcion = "Inflable con obstáculos internos",
                    Precio = 65000
                }
            };

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("nueva_imagen.jpg");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var files = new FormFileCollection { fileMock.Object };
            _controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), files);

            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string inflablesPath = Path.Combine(webRootPath, "imagenes", "inflables");

            if (!Directory.Exists(inflablesPath))
            {
                Directory.CreateDirectory(inflablesPath);
            }

            _webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(webRootPath);

            _unidadTrabajoMock.Setup(u => u.Inflable.ObtenerPrimero(It.IsAny<Expression<Func<Inflable, bool>>>(), null, false))
                              .ReturnsAsync(inflableExistente);

            _unidadTrabajoMock.Setup(u => u.Inflable.Actualizar(It.IsAny<Inflable>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Upsert(inflableActualizadoVM);

            _unidadTrabajoMock.Verify(u => u.Inflable.Actualizar(It.IsAny<Inflable>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once);

            Assert.That(_controller.TempData[DS.Exitosa], Is.EqualTo("Inflable actualizado Exitosamente"));
            var redirectResult = resultado as RedirectToActionResult;
        }

        // Verifica que el método Upsert actualice un inflable
        [Test]
        public async Task Upsert_Post_ActualizaInflable_NoActualizarImagen()
        {
            var inflableExistente = new Inflable
            {
                Id = 1,
                Nombre = "Bob Esponja",
                ImageUrl = "bobEsponja.jpg",
                Descripcion = "Inflable temático de Bob Esponja",
                Precio = 45000
            };

            var inflableActualizadoVM = new InflableVM
            {
                Inflable = new Inflable
                {
                    Id = 1,
                    Nombre = "Bob esponja",
                    ImageUrl = "bobEsponja.jpg",
                    Descripcion = "Inflable temático de Bob Esponja",
                    Precio = 50000
                }
            };

            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            _webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(webRootPath);

            _unidadTrabajoMock.Setup(u => u.Inflable.ObtenerPrimero(It.IsAny<Expression<Func<Inflable, bool>>>(), null, false))
                              .ReturnsAsync(inflableExistente);

            _unidadTrabajoMock.Setup(u => u.Inflable.Actualizar(It.IsAny<Inflable>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var formFileCollection = new FormFileCollection();
            _controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), formFileCollection);

            var resultado = await _controller.Upsert(inflableActualizadoVM);

            _unidadTrabajoMock.Verify(u => u.Inflable.Actualizar(It.IsAny<Inflable>()), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Guardar(), Times.Once);
            Assert.That(inflableExistente.ImageUrl, Is.EqualTo("bobEsponja.jpg")); // Verificar que la URL de la imagen no cambia

            Assert.That(_controller.TempData[DS.Exitosa], Is.EqualTo("Inflable actualizado Exitosamente"));
            var redirectResult = resultado as RedirectToActionResult;
        }

        // Verifica que el método ConsultarConFiltro retorne los inflables filtrados cuando se proporcionan los filtros de categoría de tamaño y edad.
        [Test]
        public async Task ConsultarConFiltro_FiltraPorCategorias_CuandoHayFiltros()
        {
            int? categoriaTamannoId = 1;
            int? categoriaEdadId = 2;

            var inflablesFiltrados = new List<Inflable>
            {
                new Inflable { Id = 1, Nombre = "Frozen" },
                new Inflable { Id = 2, Nombre = "Avengers" }
            };

            var categoriasTamannoLista = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Grande" },
                new SelectListItem { Value = "2", Text = "Pequeño" }
            };

            var categoriasEdadLista = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Todas las edades" },
                new SelectListItem { Value = "2", Text = "Menores 10 años" }
            };

            _unidadTrabajoMock.Setup(u => u.Inflable.FiltrarPorCategorias(categoriaTamannoId, categoriaEdadId))
                .ReturnsAsync(inflablesFiltrados);
            _unidadTrabajoMock.Setup(u => u.Inflable.ObtenerTodosDropdownLista("CategoriaTamanno"))
                .Returns(categoriasTamannoLista);
            _unidadTrabajoMock.Setup(u => u.Inflable.ObtenerTodosDropdownLista("CategoriaEdad"))
                .Returns(categoriasEdadLista);

            var resultado = await _controller.ConsultarConFiltro(categoriaTamannoId, categoriaEdadId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResultado = (JsonResult)resultado;

            dynamic data = jsonResultado.Value;

            Assert.That(data, Is.Not.Null);
        }

        // Verifica que el método ConsultarConFiltro retorne los inflables filtrados cuando no se proporcionan los filtros de categoría de tamaño y edad.
        [Test]
        public async Task ConsultarConFiltro_ObtieneTodos_CuandoNoHayFiltros()
        {
            int? categoriaTamannoId = null;
            int? categoriaEdadId = null;

            var todosLosInflables = new List<Inflable>
            {
                new Inflable { Id = 1, Nombre = "Futbolín Humano" },
                new Inflable { Id = 2, Nombre = "Bungee trampolín" }
            };

            _unidadTrabajoMock.Setup(u => u.Inflable.ObtenerTodos(
                    null, 
                    null, 
                    "CategoriaTamanno,CategoriasEdad", 
                    true)) 
                .ReturnsAsync(todosLosInflables);

            var resultado = await _controller.ConsultarConFiltro(categoriaTamannoId, categoriaEdadId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResultado = (JsonResult)resultado;
            var data = jsonResultado.Value as dynamic;

            Assert.That(data, Is.Not.Null);
        }


        // Verifica que se muestren correctamente las opciones de cada categoría respectivamente
        [Test]
        public async Task ConsultarConFiltro_CargaListasDesplegablesCorrectamente()
        {
            var categoriasTamannoLista = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Grande" },
                new SelectListItem { Value = "2", Text = "Pequeño" }
            };

            var categoriasEdadLista = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Todas las edades" },
                new SelectListItem { Value = "2", Text = "Menores 8 años" }
            };

            _unidadTrabajoMock.Setup(u => u.Inflable.ObtenerTodosDropdownLista("CategoriaTamanno"))
                .Returns(categoriasTamannoLista);
            _unidadTrabajoMock.Setup(u => u.Inflable.ObtenerTodosDropdownLista("CategoriaEdad"))
                .Returns(categoriasEdadLista);

            var resultado = await _controller.ConsultarConFiltro(null, null);

            _unidadTrabajoMock.Verify(u => u.Inflable.ObtenerTodosDropdownLista("CategoriaTamanno"), Times.Once);
            _unidadTrabajoMock.Verify(u => u.Inflable.ObtenerTodosDropdownLista("CategoriaEdad"), Times.Once);
        }


        // Verifica que un inflable sin evento asociado sea eliminada correctamente.
        [Test]
        public async Task Delete_RetornaExito_CuandoElInflableNoTieneEventosAsociados()
        {
            var inflableId = 1;
            var inflableBd = new Inflable
            {
                Id = inflableId,
                Nombre = "Frozen",
                ImageUrl = "frozen.jpg",
                Descripcion = "Inflable con temática de la película animada Frozen",
                Precio = 65000,
            };

            _unidadTrabajoMock.Setup(u => u.Inflable.Obtener(inflableId)).ReturnsAsync(inflableBd);
            _unidadTrabajoMock.Setup(u => u.Inflable.Remover(It.IsAny<Inflable>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            var resultado = await _controller.Delete(inflableId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);
            Assert.That(contenido["success"].ToString(), Is.EqualTo("True")); 
            _unidadTrabajoMock.Verify(u => u.Inflable.Remover(It.IsAny<Inflable>()), Times.Once);
        }

        // Verifica que se retorne un error cuando se intenta eliminar un inflable que no existe.
        [Test]
        public async Task Delete_RetornaError_CuandoElInflableoNoExiste()
        {
            var inflableId = 999;
            Inflable inflableBd = null;

            _unidadTrabajoMock.Setup(u => u.Inflable.Obtener(inflableId)).ReturnsAsync(inflableBd);

            var resultado = await _controller.Delete(inflableId);

            Assert.That(resultado, Is.InstanceOf<JsonResult>());
            var jsonResult = resultado as JsonResult;
            var contenido = JObject.FromObject(jsonResult.Value);
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar Inflable"));
        }

        // Verifica que el método ValidarNombre retorne false si el nombre no está duplicado.
        [Test]
        public async Task ValidarNombre_NombreNoDuplicado_RetornaFalse()
        {
            var nombreNuevo = "Avengers";
            var listaInflables = new List<Inflable>
            {
                new Inflable { Id = 1, Nombre = "Cono escalador" },
                new Inflable { Id = 2, Nombre = "Casa club" }
            };

            _unidadTrabajoMock.Setup(u => u.Inflable.ObtenerTodos(It.IsAny<Expression<Func<Inflable, bool>>>(), null, null, true))
                .ReturnsAsync(listaInflables);

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
            var nombreNuevo = "Gladiador";
            var listaInflables = new List<Inflable>
            {
                new Inflable { Id = 1, Nombre = "Gladiador" },
                new Inflable { Id = 2, Nombre = "Liga de la Justicia" }
            };

            _unidadTrabajoMock.Setup(u => u.Inflable.ObtenerTodos(It.IsAny<Expression<Func<Inflable, bool>>>(), null, null, true))
                .ReturnsAsync(listaInflables);

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
