using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class ProveedoresTests
    {
        private IWebDriver driver;

        ProveedoresPage proveedoresPage;

        [SetUp]
        public void SetUp()
        {
            proveedoresPage = new ProveedoresPage(driver);
            driver = proveedoresPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            proveedoresPage.Visit("https://localhost:7033/");

            proveedoresPage.IniciarSesion("camiulatech@gmail.com", "Hola321!");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        [Test, Order(1)]
        public void CrearProveedoresConDatosValidos_EsRegistrado()
        {
            proveedoresPage.GestionProveedores();
            Thread.Sleep(2000);
            proveedoresPage.CrearProveedor("EPA", "EpaEpa", "85858585", "Epa@gmail.com", "Belen", "Herramientas y otros", "Herramientas");
            Thread.Sleep(2000);
            Assert.IsTrue(proveedoresPage.ProveedorEstaRegistrado("EPA"));
        }
        [Test, Order(2)]
        public void ActualizarPrecioTarifa_EsActualizado()
        {
            proveedoresPage.GestionProveedores();
            Thread.Sleep(2000);
            proveedoresPage.ActualizarTelefonoYCorreoProveedor("80808080", "Epa22@gmail.com", "EPA");
            Thread.Sleep(2000);
            Assert.IsTrue(proveedoresPage.TelefonoYCorreoActualizado("80808080", "EPA"));
        }
        [Test, Order(3)]
        public void EliminarTarifa_EsEliminada()
        {
            proveedoresPage.GestionProveedores();
            Thread.Sleep(2000);
            proveedoresPage.EliminarProveedor("EPA");
            Thread.Sleep(2000);
            Assert.IsTrue(proveedoresPage.ProveedorEliminado());
        }

    }
}
