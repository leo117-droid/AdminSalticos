using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class LoginTests
    {
        private IWebDriver driver;
        LoginPage loginPage;

        [SetUp]
        public void SetUp()
        {
            loginPage = new LoginPage(driver);
            driver = loginPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            loginPage.Visit("https://localhost:7033/");
        }

        [Test, Order(1)]
        public void IniciarSesion_UsuarioValido()
        {
            Thread.Sleep(2000);
            loginPage.Login("camiulatech@gmail.com", "Hola321!");
            Thread.Sleep(2000);
            Assert.IsTrue(loginPage.InicioDeSesionCorrecto());
        }

        [Test, Order(2)]
        public void IniciarSesion_UsuarioNoValido() 
        {
            loginPage.Login("camiulatech@gmail.com", "Hola1234!");
            Thread.Sleep(2000);
            Assert.IsTrue(loginPage.MensajeErrorInicioSesionDesplegado());
        
        }
    }
}
