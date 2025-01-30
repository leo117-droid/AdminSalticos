using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class IngredientesTests
    {
        private IWebDriver driver;
        IngredientesPage ingredientesPage;

        [SetUp]
        public void SetUp()
        {
            ingredientesPage = new IngredientesPage(driver);
            driver = ingredientesPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            ingredientesPage.Visit("https://localhost:7033/");

            ingredientesPage.IniciarSesion("leomora641@gmail.com", "Hola123.");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        [Test, Order(1)]
        public void CrearIngredientesConDatosValidos_EsRegistrado()
        {
            ingredientesPage.GestionIngredientes();
            Thread.Sleep(2000);
            ingredientesPage.CrearIngredientes("Carne", "Carne Angus", "1000");
            Thread.Sleep(2000);
            Assert.IsTrue(ingredientesPage.IngredienteEstaRegistrado("Carne"));
        }

        [Test, Order(2)]
        public void ActualizarDescripcionIngrediente_EsActualizado()
        {
            ingredientesPage.GestionIngredientes();
            Thread.Sleep(2000);
            ingredientesPage.ActualizarDescripcionIngrediente("Carne bistec", "Carne");
            Thread.Sleep(2000);
            Assert.IsTrue(ingredientesPage.DescripcionIngredienteActualizado("Carne ", "Carne bistec"));
        }

        [Test, Order(3)]
        public void EliminarIngrediente_EsEliminado()
        {
            ingredientesPage.GestionIngredientes();
            Thread.Sleep(2000);
            ingredientesPage.EliminarIngrediente("Carne");
            Thread.Sleep(2000);
            Assert.IsTrue(ingredientesPage.EstaIngredienteEliminado());
        }
    }
}
