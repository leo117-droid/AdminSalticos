using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class VehiculoTests
    {
        private IWebDriver driver;

        VehiculoPage vehiculoPage;

        [SetUp]
        public void SetUp()
        {
            vehiculoPage = new VehiculoPage(driver);
            driver = vehiculoPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            vehiculoPage.Visit("http://localhost:5270/");

            vehiculoPage.IniciarSesion("yoswi200210@gmail.com", "Hola321!");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        [Test, Order(1)]
        public void CrearVehiculoConDatosValidos_EsRegistrado()
        {
            vehiculoPage.GestionVehiculos();
            Thread.Sleep(2000);
            vehiculoPage.AgregarVehiculo("Toyota", "2007", "010203", "Supra");
            Thread.Sleep(2000);
            Assert.IsTrue(vehiculoPage.VehiculoEstaRegistrado("010203"));
        }

        [Test, Order(2)]
        public void CrearVehiculoConCedulaExistente_MuestraMensajeDeError()
        {
            vehiculoPage.GestionVehiculos();
            Thread.Sleep(2000);
            vehiculoPage.RegistrarVehiculoConPlacaRepetida("Toyota", "2007", "010203");
            Thread.Sleep(2000);
            Assert.IsTrue(vehiculoPage.MensajeErrorPlacaRepetidaDesplegado());
        }


        [Test, Order(3)]
        public void ActualizarModeloVehiculo_EsActualizado()
        {
            vehiculoPage.GestionVehiculos();
            Thread.Sleep(2000);
            vehiculoPage.ActualizarModeloVehiculo("2010", "010203");
            Thread.Sleep(2000);
            Assert.IsTrue(vehiculoPage.ModeloVehiculoActualizado("2010", "010203"));
        }

        [Test, Order(4)]
        public void EliminarVehiculo_EsEliminado()
        {
            vehiculoPage.GestionVehiculos();
            Thread.Sleep(2000);
            vehiculoPage.EliminarVehiculo("010203");
            Thread.Sleep(2000);
            Assert.IsTrue(vehiculoPage.EstaVehiculoEliminado());
        }

    }
}
