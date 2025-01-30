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
        public void EliminarInflableEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.EliminarInflable("Yowi Garcia");
            eventoPage.InflableEliminado();
        }
        [Test, Order(5)]
        public void AgregarPersonalAEvento() 
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.SeleccionarPersonal("Leo Mora - 1", "Yowi Garcia");
            eventoPage.PersonalActualizado("1");
        }
        [Test, Order(6)]
        public void EliminarPersonalEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.EliminarPersonal("Yowi Garcia");
            eventoPage.PersonalEliminado();
        }
        [Test, Order(7)]
        public void AgregarVehiculoAEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.SeleccionarVehiculo("Camion - 111111", "Yowi Garcia");
            eventoPage.VehiculoActualizado("111111");
        }
        [Test, Order(8)]
        public void EliminarVehiculoEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.EliminarVehiculo("Yowi Garcia");
            eventoPage.VehiculoEliminado();
        }
        [Test, Order(9)]
        public void AgregarMobiliarioAEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.SeleccionarMobiliario("Mob1", "01", "Yowi Garcia");
            eventoPage.MobiliarioActualizado("Mob1");
        }
        [Test, Order(10)]
        public void EliminarMobiliarioEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.EliminarMobiliario("Yowi Garcia");
            eventoPage.MobiliarioEliminado();
        }

        [Test, Order(11)]
        public void AgregarAlimentacionAEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.SeleccionarAlimentacion("vorgesita", "1", "Yowi Garcia");
            eventoPage.AlimentacionActualizada("vorgesita");
        }

        [Test, Order(12)]
        public void EliminarAlimentacionEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.EliminarAlimentacion("Yowi Garcia");
            eventoPage.AlimentacionEliminada();
        }

        [Test, Order(13)]
        public void AgregarServicioAEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.SeleccionarServicioAdicional("Leo", "1", "Yowi Garcia");
            eventoPage.ServicioActualizado("Leo");
        }

        [Test, Order(14)]
        public void EliminarServicioEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.EliminarServicioAdicional("Yowi Garcia");
            eventoPage.ServicioEliminado();
        }
        [Test, Order(15)]
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
