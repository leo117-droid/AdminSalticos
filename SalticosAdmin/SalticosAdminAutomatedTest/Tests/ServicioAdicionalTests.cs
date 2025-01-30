using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class ServicioAdicionalTests
    {
        private IWebDriver driver;

        private ServicioAdicionalPage servicioAdicionalPage;

        private readonly String imagenPath = @"C:\Users\camiu\Downloads\Imagenes\karaoke.jpg";

        [SetUp]
        public void SetUp()
        {
            servicioAdicionalPage = new ServicioAdicionalPage(driver);
            driver = servicioAdicionalPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            servicioAdicionalPage.Visit("https://localhost:7033/");

            servicioAdicionalPage.IniciarSesion("camiulatech@gmail.com", "Hola321!");
        }


        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }


        [Test, Order(1)]
        public void CrearNuevoServicioAdicionConDatosValidos_EsRegistrado()
        {
            servicioAdicionalPage.GestionServicioAdicional();
            Thread.Sleep(2000);
            servicioAdicionalPage.
                CrearServicioAdicional("Máquina de karaoke", 
                "Máquina de karaoke con micrófono incluido", imagenPath, "5", "2000");
            Thread.Sleep(2000);
            Assert.IsTrue(servicioAdicionalPage.ServicioAdicionalEstaRegistrado("Máquina de karaoke"));
        }


        [Test, Order(2)]
        public void CrearNuevoServicioAdicionalConNombreRepetido_MuestraError()
        {
            servicioAdicionalPage.GestionServicioAdicional();
            Thread.Sleep(1500);
            servicioAdicionalPage.CrearServicioAdicionalConNombreRepetido("Máquina de karaoke");
            Thread.Sleep(1500);
            Assert.IsTrue(servicioAdicionalPage.MensajeErrorNombreRepetidoDesplegado());
        }

        [Test, Order(3)]
        public void ActualizarServicioAdicional_EsActualizado()
        {
            servicioAdicionalPage.GestionServicioAdicional();
            Thread.Sleep(1500);
            servicioAdicionalPage.ActualizarInventarioServicioAdicional("Máquina de karaoke", "9");
            Thread.Sleep(1500);
            Assert.IsTrue(servicioAdicionalPage.InventarioServicioAdicionalActualizado("Máquina de karaoke","9"));
        }


        [Test, Order(4)]
        public void EliminarServicioAdicional_EsEliminado()
        {
            servicioAdicionalPage.GestionServicioAdicional();
            Thread.Sleep(1500);
            servicioAdicionalPage.EliminarServicioAdicional("Máquina de karaoke");
            Thread.Sleep(1500);
            Assert.IsTrue(servicioAdicionalPage.ServicioAdicionalEliminado());
        }
    }
}
