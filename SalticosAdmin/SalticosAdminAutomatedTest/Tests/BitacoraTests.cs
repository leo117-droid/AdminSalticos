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
            bitacoraPage.Visit("http://localhost:5270/");

            bitacoraPage.IniciarSesion("yoswi200210@gmail.com", "Hola321!");
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
            bitacoraPage.FiltrarBitacora("22/01/2025", "23/01/2025");
            Thread.Sleep(1500);
            Assert.IsTrue(bitacoraPage.FiltraResultados("22/01/2025", "23/01/2025"));
        }
    }
}
