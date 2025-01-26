using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class ContactoTests
    {
        private IWebDriver driver;

        ContactoPage contactoPage;

        [SetUp]
        public void SetUp()
        {
            contactoPage = new ContactoPage(driver);
            driver = contactoPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            contactoPage.Visit("https://localhost:7033/");

            contactoPage.IniciarSesion("leomora641@gmail.com", "Hola123.");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        [Test]
        public void CrearContactoConDatosValidos_EsRegistrado()
        {
            contactoPage.GestionContactos();
            Thread.Sleep(2000);
            contactoPage.CrearContacto("Yowi", "Garcia", "Mantenimiento", "Belen", "89996655", "yowi@gmail.com");
            Thread.Sleep(2000);
            Assert.IsTrue(contactoPage.ContactoEstaRegistrado("yowi@gmail.com"));
        }

    }
}
