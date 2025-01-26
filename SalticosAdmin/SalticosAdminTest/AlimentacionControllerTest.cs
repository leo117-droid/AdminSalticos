using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SalticosAdmin.AccesoDeDatos.Repositorio.IRepositorio;
using SalticosAdmin.Areas.Admin.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace SalticosAdmin.Tests
{
    [TestFixture]
    public class AlimentacionControllerTests
    {
        private Mock<IUnidadTrabajo> _unidadTrabajoMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        //private AlimentacionController _controller;

        [SetUp]
        public void Setup()
        {
            _unidadTrabajoMock = new Mock<IUnidadTrabajo>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
        //    _controller = new AlimentacionController(_unidadTrabajoMock.Object, _webHostEnvironmentMock.Object);
        }

        /*
        [Test]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.That(result, Is.TypeOf<ViewResult>()); // Verifica que el resultado es una vista
            var viewResult = result as ViewResult;
            Assert.That(viewResult.ViewName, Is.Null.Or.EqualTo("Index")); // Verifica que la vista retornada es "Index"
        }*/
    }
}
