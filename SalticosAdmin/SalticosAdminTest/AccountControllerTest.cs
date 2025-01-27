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
            _controller = new AccountController();
        }

        // Verifica que el método Index redirija correctamente al Login
        [Test]
        public void Index_RedireccionaCorrectamenteAlLogin()
        {
            // Act
            var resultado = _controller.Index();

            // Assert
            Assert.That(resultado, Is.InstanceOf<RedirectResult>());
            var redirectResult = resultado as RedirectResult;
            Assert.That(redirectResult.Url, Is.EqualTo("/Identity/Account/Login"));
        }

        [TearDown]
        public void TearDown()
        {
            if (_controller != null)
            {
                _controller.Dispose(); // Asegúrate de que el método Dispose es llamado
                _controller = null;
            }
        }
    }
}
