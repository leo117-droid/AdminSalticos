using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class MobiliarioTests
    {

        private IWebDriver driver;

        MobiliarioPage mobiliarioPage;

        private readonly String imagenPath = @"C:\Users\yaira\Downloads\Imagenes\mesa_conferencia.jpg";


        [SetUp]
        public void SetUp()
        {
            mobiliarioPage = new MobiliarioPage(driver);
            driver = mobiliarioPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            mobiliarioPage.Visit("http://localhost:5270/");

            mobiliarioPage.IniciarSesion("yoswi200210@gmail.com", "Hola321!");
        }


        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }


        [Test, Order(1)]
        public void CrearNuevoMobilairioConDatosValidos_EsRegistrado()
        {
            mobiliarioPage.GestionMobiliario();
            mobiliarioPage.CrearMobiliario("Mesa de conferencias", "Mesa grande de madera para conferencias", "2m x 1m x 1.5m",
                imagenPath, "1000", "10");
            Thread.Sleep(1500);

            Assert.IsTrue(mobiliarioPage.MobiliarioEstaRegistrado("Mesa de conferencias"));
        }


        [Test, Order(2)]
        public void ActualizarDescripcionMobiliario_EsActualizado()
        {
            mobiliarioPage.GestionMobiliario();
            mobiliarioPage.ActualizarDescripcionMobiliario("Mesa de conferencias", "Mesa grande de plástico para conferencias");
            Thread.Sleep(1500);

            Assert.IsTrue(mobiliarioPage.DescripcionMobiliarioActualizada("Mesa de conferencias", "Mesa grande de plástico para conferencias"));
        }


        [Test, Order(3)]
        public void EliminarMobiliario_EsEliminado()
        {
            mobiliarioPage.GestionMobiliario();
            mobiliarioPage.EliminarMobiliario("Mesa de conferencias");
            Thread.Sleep(1500);

            Assert.IsTrue(mobiliarioPage.MobiliarioEliminado());
        }
    }
}
