using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class CategoriaTamannoTests
    {
        private IWebDriver driver;

        CategoriaTamannoPage categoriaTamannoPage;

        [SetUp]
        public void SetUp()
        {
            categoriaTamannoPage = new CategoriaTamannoPage(driver);
            driver = categoriaTamannoPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            categoriaTamannoPage.Visit("https://localhost:7033/");

            categoriaTamannoPage.IniciarSesion("camiulatech@gmail.com", "Hola321!");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }


        [Test, Order(1)]
        public void CrearCategoriaTamannoConDatosValidos_EsRegistrado()
        {
            categoriaTamannoPage.GestionCategoriaTamanno();
            Thread.Sleep(2000);
            categoriaTamannoPage.AgregarCategoriaTamanno("Muy pequeño");
            Thread.Sleep(2000);
            Assert.IsTrue(categoriaTamannoPage.CategoriaTamannoEstaRegistrada("Muy pequeño"));
        }


        [Test, Order(2)]
        public void CrearCategoriaTamannoConNombreExistente_MuestraMensajeDeError()
        {
            categoriaTamannoPage.GestionCategoriaTamanno();
            Thread.Sleep(2000);
            categoriaTamannoPage.RegistrarCategoriaTamannoConNombreRepetido("Mediano");
            Thread.Sleep(2000);
            Assert.IsTrue(categoriaTamannoPage.MensajeErrorNombreRepetidoDesplegado());
        }


        [Test, Order(3)]
        public void ActualizarNombreCategoriaTamanno_EsActualizado()
        {
            categoriaTamannoPage.GestionCategoriaTamanno();
            Thread.Sleep(2000);
            categoriaTamannoPage.ActualizarNombreCategoriaTamanno("Muy pequeño", "Muy grande");
            Thread.Sleep(2000);
            Assert.IsTrue(categoriaTamannoPage.NombreCategoriaTamannoActualizado("Muy grande"));
        }


        [Test, Order(4)]
        public void EliminarCategoriaTamanno_EsEliminado()
        {
            categoriaTamannoPage.GestionCategoriaTamanno();
            Thread.Sleep(2000);
            categoriaTamannoPage.EliminarCategoriaTamanno("Muy grande");
            Thread.Sleep(2000);
            Assert.IsTrue(categoriaTamannoPage.EstaCategoriaTamannoEliminado());
        }
    }
}
