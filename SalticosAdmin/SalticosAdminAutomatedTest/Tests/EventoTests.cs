using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class EventoTests
    {
        private IWebDriver driver;
        EventoPage eventoPage;

        [SetUp]
        public void SetUp()
        {
            eventoPage = new EventoPage(driver);
            driver = eventoPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            eventoPage.Visit("https://localhost:7033/");

            eventoPage.IniciarSesion("leomora641@gmail.com", "Hola123.");
        }
        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
        [Test, Order(1)]
        public void CrearEventoConDatosValidos_EsRegistrado()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.CrearEvento("10/02/2025", "12:48", "22:00", "La Ceiba", "Alajuela", "Yowi Garcia - 112233", "yowi@gmail.com");
            Thread.Sleep(2000);
            Assert.IsTrue(eventoPage.EventoEstaRegistrado("Yowi Garcia"));
        }

        [Test, Order(2)]
        public void ActualizarFecha_EsActualizado()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.ActualizarFechaEvento("Yowi Garcia", "15/02/2025");
            Thread.Sleep(2000);
            Assert.IsTrue(eventoPage.FechaActualizada("15/02/2025", "Yowi Garcia"));
        }
        [Test, Order(3)]
        public void AgregarInflableAEvento() 
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.SeleccionarInflable("el torilllo", "Yowi Garcia");
            eventoPage.InflableActualizado("el torilllo");
        }
        [Test, Order(4)]
        public void EliminarInflableAEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.EliminarInflable("Yowi Garcia");
            eventoPage.InflableEliminado();
        }
        [Test, Order(5)]
        public void EliminarEvento_EsEliminado()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.EliminarEvento("Yowi Garcia");
            Thread.Sleep(2000);
            Assert.IsTrue(eventoPage.EventoEliminado());
        }
    }
}
