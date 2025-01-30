using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class BitacoraTests
    {

        private IWebDriver driver;

        BitacoraPage bitacoraPage;

        [SetUp]
        public void SetUp()
        {
            bitacoraPage = new BitacoraPage(driver);
            driver = bitacoraPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            bitacoraPage.Visit("https://localhost:7033/");

            bitacoraPage.IniciarSesion("camiulatech@gmail.com", "Hola321!");
        }


        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }


        [Test, Order(1)]
        public void FiltrarBitacoraConFechasValidas_EsFiltrado()
        {
            bitacoraPage.GestionBitacora();
            Thread.Sleep(1500);
            bitacoraPage.FiltrarBitacora("01/01/2025", "02/02/2025");
            Thread.Sleep(1500);
            Assert.IsTrue(bitacoraPage.FiltraResultados("01/01/2025", "02/02/2025"));
        }
    }
}
