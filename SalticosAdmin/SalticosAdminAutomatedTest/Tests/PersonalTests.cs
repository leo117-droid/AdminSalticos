using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class PersonalTests
    {
        private IWebDriver driver;
        PersonalPage personalPage;

        [SetUp]
        public void SetUp()
        {
            personalPage = new PersonalPage(driver);
            driver = personalPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            personalPage.Visit("https://localhost:7033/");

            personalPage.IniciarSesion("leomora641@gmail.com", "Hola123.");
        }
        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
        [Test, Order(1)]
        public void CrearPersonalConDatosValidos_EsRegistrado()
        {
            personalPage.GestionPersonal();
            Thread.Sleep(2000);
            personalPage.CrearPersonal("Ian", "Calvo", "88225588", "IanCalvo@gmail.com", "2058", "2003-04-24", "2025-04-24", "Conductor");
            Thread.Sleep(2000);
            Assert.IsTrue(personalPage.PersonalEstaRegistrado("2058"));
        }

        [Test, Order(2)]
        public void CrearPersonal_CedulaYaExistente()
        {
            personalPage.GestionPersonal();
            Thread.Sleep(2000);
            personalPage.CrearPersonal_CedulaRepetida("Leo", "Mora", "88225588", "IanCalvoo@gmail.com", "2058");
            Thread.Sleep(2000);
            Assert.IsTrue(personalPage.MensajeDespliegaCedulaRepetida());
        }

        [Test, Order(3)]
        public void ActualizarCorreo_EsActualizado()
        {
            personalPage.GestionPersonal();
            Thread.Sleep(2000);
            personalPage.ActualizarCorreoPersonal("2058", "Ian@gmail.com");
            Thread.Sleep(2000);
            Assert.IsTrue(personalPage.CorreoActualizado("2058", "Ian@gmail.com"));
        }
        [Test, Order(4)]
        public void FiltrarPersonal_PorRol() 
        {
            personalPage.GestionPersonal();
            Thread.Sleep(2000);
            personalPage.FiltrarPersonal("Ingeniero");
            Thread.Sleep(2000);
            Assert.IsTrue(personalPage.FiltroCorreocto("Ingeniero"));
        }
        [Test, Order(5)]
        public void EliminarPersonal_EsEliminado()
        {
            personalPage.GestionPersonal();
            Thread.Sleep(2000);
            personalPage.EliminarPersonal("2058");
            Thread.Sleep(2000);
            Assert.IsTrue(personalPage.PersonalEliminado());
        }
    }
}
