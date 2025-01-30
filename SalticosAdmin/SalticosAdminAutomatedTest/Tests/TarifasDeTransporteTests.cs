using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class TarifasDeTransporteTests
    {
        private IWebDriver driver;

        TarifasDeTransportePage tarifasDeTransportePage;

        [SetUp]
        public void SetUp()
        {
            tarifasDeTransportePage = new TarifasDeTransportePage(driver);
            driver = tarifasDeTransportePage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            tarifasDeTransportePage.Visit("https://localhost:7033/");

            tarifasDeTransportePage.IniciarSesion("camiulatech@gmail.com", "Hola321!");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        [Test, Order(1)]
        public void CrearTarifaConDatosValidos_EsRegistrado()
        {
            tarifasDeTransportePage.GestionTarifasDeTransporte();
            Thread.Sleep(2000);
            tarifasDeTransportePage.CrearTarifa("Alajuela", "3000");
            Thread.Sleep(2000);
            Assert.IsTrue(tarifasDeTransportePage.TarifaEstaRegistrada("Alajuela"));
        }


        [Test, Order(2)]
        public void CrearTarifaConPrecioNegativo_MuestraMensajeDeError()
        {
            tarifasDeTransportePage.GestionTarifasDeTransporte();
            Thread.Sleep(2000);
            tarifasDeTransportePage.CrearTarifaConPrecioNegativo("Alajuela", "-2000");
            Thread.Sleep(2000);
            Assert.IsTrue(tarifasDeTransportePage.PrecioNegativoMsgDesplegado());
        }
        [Test, Order(3)]
        public void ActualizarPrecioTarifa_EsActualizado()
        {
            tarifasDeTransportePage.GestionTarifasDeTransporte();
            Thread.Sleep(2000);
            tarifasDeTransportePage.ActualizarPrecioTarifa("2000", "Alajuela");
            Thread.Sleep(2000);
            Assert.IsTrue(tarifasDeTransportePage.PrecioTarifaActualizado("₡2,000.00", "Alajuela"));
        }

        [Test, Order(4)]
        public void EliminarTarifa_EsEliminada()
        {
            tarifasDeTransportePage.GestionTarifasDeTransporte();
            Thread.Sleep(2000);
            tarifasDeTransportePage.EliminarTarifa("Alajuela");
            Thread.Sleep(2000);
            Assert.IsTrue(tarifasDeTransportePage.TarifaEliminada());
        }

    }
}
