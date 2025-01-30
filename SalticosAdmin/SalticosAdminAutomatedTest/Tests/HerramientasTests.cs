using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SalticosAdminAutomatedTest.Tests
{
    public class HerramientasTests
    {
        private IWebDriver driver;

        HerramientasPage herramientasPage;
        [SetUp]
        public void SetUp()
        {
            herramientasPage = new HerramientasPage(driver);
            driver = herramientasPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            herramientasPage.Visit("https://localhost:7033/");

            herramientasPage.IniciarSesion("camiulatech@gmail.com", "Hola321!");
        }
        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        [Test, Order(1)]
        public void CrearHerramientaConDatosValidos_EsRegistrado()
        {
            herramientasPage.GestionHerramientas();
            Thread.Sleep(2000);
            herramientasPage.CrearHerramienta("Martillo", "Martillo Hammer", "5");
            Thread.Sleep(2000);
            Assert.IsTrue(herramientasPage.HerramientaEstaRegistrado("Martillo"));
        }

        [Test, Order(2)]
        public void CrearHerramientaConNombreExistente_MuestraMensajeDeError()
        {
            herramientasPage.GestionHerramientas();
            Thread.Sleep(2000);
            herramientasPage.RegistrarHerramientaConNombreRepetida("Martillo");
            Thread.Sleep(2000);
            Assert.IsTrue(herramientasPage.MensajeErrorNombreRepetidoDesplegado());
        }

        [Test, Order(3)]
        public void ActualizarDescripcionHerramienta_EsActualizado()
        {
            herramientasPage.GestionHerramientas();
            Thread.Sleep(2000);
            herramientasPage.ActualizarDescripcionIngrediente("Martillo automatico", "Martillo");
            Thread.Sleep(2000);
            Assert.IsTrue(herramientasPage.DescripcionHerramientaActualizado("Martillo", "Martillo automatico"));
        }

        [Test, Order(4)]
        public void EliminarHerramienta_EsEliminada()
        {
            herramientasPage.GestionHerramientas();
            Thread.Sleep(2000);
            herramientasPage.EliminarHerramienta("Martillo");
            Thread.Sleep(2000);
            Assert.IsTrue(herramientasPage.EstaHerramientaEliminada());
        }

    }
}
