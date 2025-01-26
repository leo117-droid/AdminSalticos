using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class RolPersonalTests
    {

        private IWebDriver driver;

        private RolPersonalPage rolPersonalPage;


        [SetUp]
        public void SetUp()
        {
            rolPersonalPage = new RolPersonalPage(driver);
            driver = rolPersonalPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            rolPersonalPage.Visit("http://localhost:5270/");

            rolPersonalPage.IniciarSesion("yoswi200210@gmail.com", "Hola321!");
        }


        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }


        [Test, Order(1)]
        public void CrearRolPersonalConDatosValidos_EsRegistrado()
        {
            rolPersonalPage.GestionRolPersonal();
            Thread.Sleep(2000);
            rolPersonalPage.AgregarRolPersonal("Conductor");
            Thread.Sleep(2000);
            Assert.IsTrue(rolPersonalPage.RolPersonalEstaRegistrada("Conductor"));
        }

        [Test, Order(2)]
        public void CrearRolPersonalConNombreExistente_MuestraMensajeDeError()
        {
            rolPersonalPage.GestionRolPersonal();
            Thread.Sleep(2000);
            rolPersonalPage.RegistrarRolPersonalConNombreRepetido("Conductor");
            Thread.Sleep(1500);
            Assert.IsTrue(rolPersonalPage.MensajeErrorNombreRepetidoDesplegado());
        }


        [Test, Order(3)]
        public void ActualizarNombreRolPersonal_EsActualizado()
        {
            rolPersonalPage.GestionRolPersonal();
            Thread.Sleep(2000);
            rolPersonalPage.ActualizarNombreRolPersonal("Conductor", "Asistente");
            Thread.Sleep(2000);
            Assert.IsTrue(rolPersonalPage.NombreRolPersonalActualizado("Asistente"));
        }


        [Test, Order(4)]
        public void EliminarRolPersonal_EsEliminado()
        {
            rolPersonalPage.GestionRolPersonal();
            Thread.Sleep(2000);
            rolPersonalPage.EliminarRolPersonal("Asistente");
            Thread.Sleep(2000);
            Assert.IsTrue(rolPersonalPage.EstaCategoriaEdadEliminado());
        }

    }
}
