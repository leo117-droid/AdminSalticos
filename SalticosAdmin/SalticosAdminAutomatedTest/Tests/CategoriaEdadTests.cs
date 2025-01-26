using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class CategoriaEdadTests
    {

        private IWebDriver driver;

        CategoriaEdadPage categoriaEdadPage;

        [SetUp]
        public void SetUp()
        {
            categoriaEdadPage = new CategoriaEdadPage(driver);
            driver = categoriaEdadPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            categoriaEdadPage.Visit("http://localhost:5270/");

            categoriaEdadPage.IniciarSesion("yoswi200210@gmail.com", "Hola321!");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }



        [Test, Order(1)]
        public void CrearCategoriaEdadConDatosValidos_EsRegistrado()
        {
            categoriaEdadPage.GestionCategoriaEdad();
            Thread.Sleep(2000);
            categoriaEdadPage.AgregarCategoriaEdad("Mayores de 10 años");
            Thread.Sleep(2000);
            Assert.IsTrue(categoriaEdadPage.CategoriaEdadEstaRegistrada("Mayores de 10 años"));
        }


        [Test, Order(2)]
        public void CrearCategoriaEdadConNombreExistente_MuestraMensajeDeError()
        {
            categoriaEdadPage.GestionCategoriaEdad();
            Thread.Sleep(2000);
            categoriaEdadPage.RegistrarCategoriaEdadConNombreRepetido("Mayores de 10 años");
            Thread.Sleep(2000);
            Assert.IsTrue(categoriaEdadPage.MensajeErrorNombreRepetidoDesplegado());
        }


        [Test, Order(3)]
        public void ActualizarNombreCategoriaEdad_EsActualizado()
        {
            categoriaEdadPage.GestionCategoriaEdad();
            Thread.Sleep(2000);
            categoriaEdadPage.ActualizarNombreCategoriaEdad("Mayores de 10 años", "Mayores de 12 años");
            Thread.Sleep(2000);
            Assert.IsTrue(categoriaEdadPage.NombreCategoriaEdadActualizado("Mayores de 12 años"));
        }


        [Test, Order(4)]
        public void EliminarCategoriaEdad_EsEliminado()
        {
            categoriaEdadPage.GestionCategoriaEdad();
            Thread.Sleep(2000);
            categoriaEdadPage.EliminarCategoriaEdad("Mayores de 12 años");
            Thread.Sleep(2000);
            Assert.IsTrue(categoriaEdadPage.EstaCategoriaEdadEliminado());
        }
    }
}
