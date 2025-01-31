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
            proveedoresPage.Visit("http://localhost:5270/");

            proveedoresPage.IniciarSesion("yoswi200210@gmail.com", "Hola321!");
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
            proveedoresPage.CrearProveedor("Pricesmart", "Yurgen Cambronero", "62358809", "yurgen@gmail.com", "Belen", "Alimentos y otros", "Alimentación");
            Thread.Sleep(2000);
            Assert.IsTrue(proveedoresPage.ProveedorEstaRegistrado("Pricesmart"));
        }
        [Test, Order(2)]
        public void ActualizarPrecioTarifa_EsActualizado()
        {
            proveedoresPage.GestionProveedores();
            Thread.Sleep(2000);
            proveedoresPage.ActualizarTelefonoYCorreoProveedor("62998721", "yurgencambronero@gmail.com", "Pricesmart");
            Thread.Sleep(2000);
            Assert.IsTrue(proveedoresPage.TelefonoYCorreoActualizado("62998721", "Pricesmart"));
        }
        [Test, Order(3)]
        public void EliminarTarifa_EsEliminada()
        {
            proveedoresPage.GestionProveedores();
            Thread.Sleep(2000);
            proveedoresPage.EliminarProveedor("Pricesmart");
            Thread.Sleep(2000);
            Assert.IsTrue(proveedoresPage.ProveedorEliminado());
        }

        [Test, Order(4)]
        public void CrearProveedorConTelefonoMenorA8Caracteres_MensajeError()
        {
            proveedoresPage.GestionProveedores();
            Thread.Sleep(1500);
            proveedoresPage.CrearProveedor("EPA", "Karlina Chaves", "6235", "karchav@gmail.com", 
                "Río Segundo, Alajuela", "Herramientas y otros", "Herramientas");
            Thread.Sleep(1500);
            Assert.IsFalse(proveedoresPage.ValidacionDesplegada());

        }


    }
}
