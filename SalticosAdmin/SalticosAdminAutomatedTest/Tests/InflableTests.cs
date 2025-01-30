using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class InflableTests 
    {

        private IWebDriver driver;

        InflablePage inflablePage;

        private readonly String imagenPath = @"C:\Users\camiu\Downloads\Imagenes\super_tobogan.png";


        [SetUp]
        public void SetUp()
        {
            inflablePage = new InflablePage(driver);
            driver = inflablePage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            inflablePage.Visit("http://localhost:7033/");

            inflablePage.IniciarSesion("camiulatech@gmail.com", "Hola321!");
        }


        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }


        [Test, Order(1)]
        public void CrearNuevoInflableConDatosValidos_EsRegistrado()
        {
            inflablePage.GestionInflable();
            inflablePage.CrearInflable("Super Tobogán", "Se puede usar con agua o en seco y con palmeras a los lados",
                "10m x 4m x 4.5m", imagenPath, "110000", "22000", 
                "Grande", "Todas las edades");
            Thread.Sleep(1500);

            Assert.IsTrue(inflablePage.InflableEstaRegistrado("Super Tobogán"));
        }

        [Test, Order(2)]
        public void ActualizarDescripcionInflable_EsActualizado()
        {

            string nuevaDescripcion = "Inflable grande de 10m de alto con tobogán de agua";
            inflablePage.GestionInflable();
            inflablePage.ActualizarDescripcionInflable("Super Tobogán", nuevaDescripcion);
            Thread.Sleep(1500);

            Assert.IsTrue(inflablePage.DescripcionInflableActualizada("Super Tobogán", nuevaDescripcion));
        }

        [Test, Order(3)]
        public void ActualizarPrecioInflableConPrecioNegativo_NoEsActualizado()
        {

            string nuevoPrecio = "-22";
            inflablePage.GestionInflable();
            inflablePage.ActualizarPrecioNegativo("Super Tobogán", nuevoPrecio);
            Thread.Sleep(1500);

            Assert.IsTrue(inflablePage.PrecioNegativoMsgDesplegado());
        }


        [Test, Order(4)]
        public void FiltroTamannoYEdad_ResultadoFiltrado()
        {
            inflablePage.GestionInflable();
            inflablePage.FiltrarInflable("Grande", "Todas las edades");
            Thread.Sleep(1500);
            Assert.IsTrue(inflablePage.FiltroAplicado("Grande", "Todas las edades"));
        }
    }
}
