using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class SegurosTests
    {
        private IWebDriver driver;
        SegurosPage segurosPage;

        [SetUp]
        public void SetUp()
        {
            segurosPage = new SegurosPage(driver);
            driver = segurosPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            segurosPage.Visit("https://localhost:7033/");

            segurosPage.IniciarSesion("camiulatech@gmail.com", "Hola321!");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        [Test, Order(1)]
        public void CrearSeguroConDatosValidos_EsRegistrado()
        {
            segurosPage.GestionSeguros();
            Thread.Sleep(2000);
            segurosPage.CrearSeguro("Seguro Personal", "INS", "220", "2025-10-28", "2026-10-28", "Vigente");
            Thread.Sleep(2000);
            Assert.IsTrue(segurosPage.SeguroEstaRegistrado("2200"));
        }

        [Test, Order(2)]
        public void CrearSeguroDatosVacios_MensajeDeError()
        {
            segurosPage.GestionSeguros();
            Thread.Sleep(2000);
            segurosPage.CrearSeguroSinDatos();
            Thread.Sleep(2000);
            Assert.IsTrue(segurosPage.MensajeErrorNoDatos());
        }

        [Test, Order(3)]
        public void ActualizarFechaVencimiento() 
        {
            segurosPage.GestionSeguros();
            Thread.Sleep(2000);
            segurosPage.ActualizarFechaVencimiento("2200", "2028-08-22");
            Thread.Sleep(2000);
            Assert.IsTrue(segurosPage.FechaActualizada("22/08/2028", "2200"));
        }

        [Test, Order(4)]
        public void EliminarSeguro_EsEliminado()
        {
            segurosPage.GestionSeguros();
            Thread.Sleep(2000);
            segurosPage.EliminarSeguro("2200");
            Thread.Sleep(2000);
            Assert.IsTrue(segurosPage.EstaSeguroEliminado());
        }
    }
}
