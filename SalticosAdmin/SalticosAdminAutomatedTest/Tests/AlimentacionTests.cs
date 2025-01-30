using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class AlimentacionTests
    {

        private IWebDriver driver;

        AlimentacionPage alimentacionPage;

        private readonly string imagenPath = @"C:\Users\camiu\Downloads\Imagenes\pizza.jpg";


        [SetUp]
        public void SetUp()
        {
            alimentacionPage = new AlimentacionPage(driver);
            driver = alimentacionPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            alimentacionPage.Visit("https://localhost:7033/");

            alimentacionPage.IniciarSesion("camiulatech@gmail.com", "Hola321!");
        }


        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }


        [Test, Order(1)]
        public void CrearNuevoAlimentoConDatosValidos_EsRegistrado()
        {
            alimentacionPage.GestionAlimentacion();
            alimentacionPage.CrearAlimento("Pizza", "Pizza con queso y pepperoni", imagenPath, "1500");
            Thread.Sleep(1500);
            Assert.IsTrue(alimentacionPage.AlimentoCreado("Pizza"));
        }


        [Test, Order(2)]
        public void ActualizarPrecioAlimento_EsActualizado()
        {
            alimentacionPage.GestionAlimentacion();
            alimentacionPage.ActualizarPrecioAlimento("Pizza", "2000");
            Thread.Sleep(1500);
            Assert.IsTrue(alimentacionPage.PrecioActualizado("Pizza", "₡2,000.00"));
        }


        [Test,Order(3)]
        public void AgregarIngredienteAlimento_EsAgregado()
        {
            alimentacionPage.GestionAlimentacion();
            alimentacionPage.AgregarIngrediente("Pizza", "Queso mozzarella");
            Thread.Sleep(1500);
            Assert.IsTrue(alimentacionPage.IngredienteAgregado("Queso mozzarella"));
        }


        [Test, Order(4)]
        public void EliminarIngredienteAlimento_EsEliminado()
        {
            alimentacionPage.GestionAlimentacion();
            alimentacionPage.EliminarIngrediente("Pizza", "Queso mozzarella");
            Thread.Sleep(1500);
            Assert.IsTrue(alimentacionPage.IngredienteEliminado());
        }




        [Test,Order(5)]
        public void EliminarAlimento_EsEliminado()
        {
            alimentacionPage.GestionAlimentacion();
            alimentacionPage.EliminarAlimento("Pizza");
            Thread.Sleep(1500);
            Assert.IsTrue(alimentacionPage.AlimentoEliminado());
        }
    }
}
