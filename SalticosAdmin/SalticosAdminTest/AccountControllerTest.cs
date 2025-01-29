using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SalticosAdmin.Areas.Admin.Controllers;

namespace SalticosAdmin.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class AccountControllerTests
    {
        private AccountController _controller;

        [SetUp]
        public void SetUp()
        {
            // Configuración inicial antes de cada prueba

            _controller = new AccountController();
        }

        // Verifica que el método Index redirija correctamente al Login
        [Test]
        public void Index_RedireccionaCorrectamenteAlLogin()
        {
            var resultado = _controller.Index();

            Assert.That(resultado, Is.InstanceOf<RedirectResult>());
            var redirectResult = resultado as RedirectResult;
            Assert.That(redirectResult.Url, Is.EqualTo("/Identity/Account/Login"));
        }

        [TearDown]
        public void TearDown()
        {
            if (_controller != null)
            {
                _controller.Dispose(); 
                _controller = null;
            }
        }
    }
}
