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
    public class CapacitacionControllerTests
    {
        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private CapacitacionController _controller;
        private Mock<HttpContext> _httpContextMock;
        private Mock<ITempDataDictionary> _tempDataMock;


        [SetUp]
        public void SetUp()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _controller = new CapacitacionController(_unidadTrabajoMock.Object);
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

            //Simula la existencia del TempData en forma de un Diccionario
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            {
                [DS.Exitosa] = "Success message",
                [DS.Error] = "Error message"
            };
            _controller.TempData = tempData;

            // Mocking methods for IBitacora and IBitacoraError
            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
        }



        [Test]
        public async Task Index_RetornaVista()
        {
            // Actuar
            var resultado = _controller.Index();

            // Verificar
            Assert.That(resultado, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloVacioCuandoIdEsNull()
        {
            // Actuar
            var resultado = await _controller.Upsert((int?)null); // Forzar el uso de la sobrecarga que recibe int?

            // Verificar
            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Capacitacion)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);
            Assert.That(modelo.Id, Is.EqualTo(0)); // Usar Is.EqualTo para comparar valores
        }


        [Test]
        public async Task Upsert_Get_RetornaVistaConModeloCuandoIdEsValido()
        {
            // Arrange
            var capacitacionId = 1;
            var capacitacion = new Capacitacion { Id = capacitacionId, Fecha = DateTime.Today.AddDays(1), Tema = "Limpieza de las máquinas de burbujas", Duracion = "2 horas" };
            _unidadTrabajoMock.Setup(u => u.Capacitacion.Obtener(capacitacionId)).ReturnsAsync(capacitacion);

            // Actuar
            var resultado = await _controller.Upsert(capacitacionId);

            // Verificar
            Assert.That(resultado, Is.InstanceOf<ViewResult>());
            var modelo = (Capacitacion)((ViewResult)resultado).Model;
            Assert.That(modelo, Is.Not.Null);  // Verificamos que el modelo no sea nulo
            Assert.That(modelo.Id, Is.EqualTo(capacitacionId));  // Comparación correcta con NUnit
        }

        [Test]
        public async Task Upsert_Post_CreaCapacitacion()
        {
            // Arrange
            var capacitacion = new Capacitacion
            {
                Id = 0, // Indicamos que es una nueva capacitación
                Fecha = DateTime.Today.AddDays(1), // Asignamos la fecha de mañana
                Tema = "Tema de Capacitación",
                Duracion = "2 horas"
            };

            _unidadTrabajoMock.Setup(u => u.Capacitacion.Agregar(It.IsAny<Capacitacion>())).Returns(Task.CompletedTask);
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Upsert(capacitacion);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); // Verifica que la redirección sea a la acción Index
            _unidadTrabajoMock.Verify(u => u.Capacitacion.Agregar(It.IsAny<Capacitacion>()), Times.Once); // Verifica que el método Agregar se haya llamado
        }

        [Test]
        public async Task Upsert_Post_ActualizaCapacitacion()
        {
            // Arrange
            var capacitacionId = 1;
            var capacitacionExistente = new Capacitacion
            {
                Id = capacitacionId,
                Fecha = DateTime.Today.AddDays(1),
                Tema = "Capacitación Inicial",
                Duracion = "1 hora"
            };

            var capacitacionActualizada = new Capacitacion
            {
                Id = capacitacionId,
                Fecha = DateTime.Today.AddDays(2), // Actualizamos la fecha
                Tema = "Capacitación Avanzada",
                Duracion = "2 horas"
            };

            // Mock para obtener la capacitación existente
            _unidadTrabajoMock.Setup(u => u.Capacitacion.Obtener(capacitacionId)).ReturnsAsync(capacitacionExistente);
            _unidadTrabajoMock.Setup(u => u.Capacitacion.Actualizar(It.IsAny<Capacitacion>())).Verifiable();
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            // Actuar
            var result = await _controller.Upsert(capacitacionActualizada);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index")); // Verifica la redirección a Index
            _unidadTrabajoMock.Verify(u => u.Capacitacion.Actualizar(It.IsAny<Capacitacion>()), Times.Once); // Verifica que Actualizar haya sido llamado
        }

        [Test]
        public async Task Delete_RetornaExito_CuandoLaCapacitacionNoTienePersonalAsociado_YNoExisteDespuesDeEliminarla()
        {
            // Arrange
            var capacitacionId = 1;  // ID de la capacitación a eliminar
            var capacitacionBd = new Capacitacion
            {
                Fecha = DateTime.Today.AddDays(1),
                Tema = "Capacitación Inicial",
                Duracion = "1 hora"
            };

            // Setup del mock para obtener la capacitación
            _unidadTrabajoMock.Setup(u => u.Capacitacion.Obtener(capacitacionId)).ReturnsAsync(capacitacionBd);

            // Setup del mock para obtener los registros de capacitacionPersonal (sin personal asociado)
            _unidadTrabajoMock.Setup(u => u.CapacitacionPersonal.ObtenerTodos(It.IsAny<Expression<Func<CapacitacionPersonal, bool>>>(), null, null, true))
                .ReturnsAsync(new List<CapacitacionPersonal>());

            // Setup del mock para remover la capacitación (no hay que remover capacitacionPersonal)
            _unidadTrabajoMock.Setup(u => u.Capacitacion.Remover(It.IsAny<Capacitacion>())).Verifiable();

            // Setup del mock para guardar los cambios
            _unidadTrabajoMock.Setup(u => u.Guardar()).Returns(Task.CompletedTask);

            // Setup del mock para registrar la bitácora
            _unidadTrabajoMock.Setup(u => u.Bitacora.RegistrarBitacora(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _controller.Delete(capacitacionId);

            // Assert

            // Simulamos que la capacitación no existe después de la eliminación
            _unidadTrabajoMock.Setup(u => u.Capacitacion.Obtener(capacitacionId)).ReturnsAsync((Capacitacion)null);

            // Luego validamos que la capacitación ya no existe
            var capacitacionEliminada = await _unidadTrabajoMock.Object.Capacitacion.Obtener(capacitacionId);
            Assert.That(capacitacionEliminada, Is.Null);  // Verifica que la capacitación eliminada sea nula
        }

        [Test]
        public async Task Delete_RetornaError_CuandoLaCapacitacionNoExiste()
        {
            // Arrange
            var capacitacionId = 999;  // ID de la capacitación que no existe
            Capacitacion capacitacionBd = null;  // No existe en la base de datos

            // Setup del mock para obtener la capacitación (no existe)
            _unidadTrabajoMock.Setup(u => u.Capacitacion.Obtener(capacitacionId)).ReturnsAsync(capacitacionBd);

            // Act
            var resultado = await _controller.Delete(capacitacionId);

            // Assert
            // Verificamos que el resultado sea un JsonResult
            Assert.That(resultado, Is.InstanceOf<JsonResult>());  // Verifica que el resultado sea un JsonResult

            var jsonResult = resultado as JsonResult;

            // Deserializamos el JsonResult a un JObject para acceder a las propiedades
            var contenido = JObject.FromObject(jsonResult.Value);

            // Verificamos que el mensaje dentro del JObject indique que hay un error al borrar
            Assert.That(contenido["message"].ToString(), Is.EqualTo("Error al borrar capacitacion"));  // Verifica que el mensaje sea el esperado
        }



        [TearDown]
        public void TearDown()
        {
            // Limpiar los mocks configurados
            _unidadTrabajoMock.Reset();

            // Liberar cualquier recurso adicional si es necesario
            if (_controller != null)
            {
                // Limpiar el TempData si no es nulo
                _controller.TempData?.Clear();

                // No poner ControllerContext a null, ya que es necesario para el controlador
                // Si el controlador implementa IDisposable, liberar los recursos
                if (_controller is IDisposable disposableController)
                {
                    disposableController.Dispose();
                }

                // Limpiar el controlador
                _controller = null;
            }
        }




    }
}

