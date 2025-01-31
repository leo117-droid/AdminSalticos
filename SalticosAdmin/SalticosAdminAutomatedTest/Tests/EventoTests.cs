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

            eventoPage.IniciarSesion("camiulatech@gmail.com", "Hola321!");
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
            eventoPage.CrearEvento("10/02/2025", "13:00", "16:00", "La Ceiba", "Alajuela", "Jose Chaves Arce - 101289043", "jose@gmail.com");
            Thread.Sleep(2000);
            Assert.IsTrue(eventoPage.EventoEstaRegistrado("Jose Chaves Arce"));
        }

        [Test, Order(2)]
        public void ActualizarFecha_EsActualizado()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.ActualizarFechaEvento("Jose Chaves Arce", "03/03/2025");
            Thread.Sleep(2000);
            Assert.IsTrue(eventoPage.FechaActualizada("03/03/2025", "Jose Chaves Arce"));
        }

        [Test, Order(3)]
        public void AgregarInflableAEvento() 
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.SeleccionarInflable("Cono Escalador", "Jose Chaves Arce");
            eventoPage.InflableActualizado("Cono Escalador");
        }
        [Test, Order(4)]
        public void EliminarInflableEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.EliminarInflable("Jose Chaves Arce");
            eventoPage.InflableEliminado();
        }
        [Test, Order(5)]
        public void AgregarPersonalAEvento() 
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.SeleccionarPersonal("Raúl Bolaños Días - 102671235", "Jose Chaves Arce");
            eventoPage.PersonalActualizado("102671235");
        }
        [Test, Order(6)]
        public void EliminarPersonalEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.EliminarPersonal("Jose Chaves Arce");
            eventoPage.PersonalEliminado();
        }
        [Test, Order(7)]
        public void AgregarVehiculoAEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.SeleccionarVehiculo("Hyundai Porter - 321880", "Jose Chaves Arce");
            eventoPage.VehiculoActualizado("321880");
        }
        [Test, Order(8)]
        public void EliminarVehiculoEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.EliminarVehiculo("Jose Chaves Arce");
            eventoPage.VehiculoEliminado();
        }
        [Test, Order(9)]
        public void AgregarMobiliarioAEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.SeleccionarMobiliario("Mesa", "6", "Jose Chaves Arce");
            eventoPage.MobiliarioActualizado("Mesa");
        }
        [Test, Order(10)]
        public void EliminarMobiliarioEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.EliminarMobiliario("Jose Chaves Arce");
            eventoPage.MobiliarioEliminado();
        }

        [Test, Order(11)]
        public void AgregarAlimentacionAEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.SeleccionarAlimentacion("Hot dog básico", "50", "Jose Chaves Arce");
            eventoPage.AlimentacionActualizada("Hot dog básico");
        }

        [Test, Order(12)]
        public void EliminarAlimentacionEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.EliminarAlimentacion("Jose Chaves Arce");
            eventoPage.AlimentacionEliminada();
        }

        [Test, Order(13)]
        public void AgregarServicioAEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.SeleccionarServicioAdicional("Planta", "1", "Jose Chaves Arce");
            eventoPage.ServicioActualizado("Planta");
        }

        [Test, Order(14)]
        public void EliminarServicioEvento()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.EliminarServicioAdicional("Jose Chaves Arce");
            eventoPage.ServicioEliminado();
        }
        [Test, Order(15)]
        public void EliminarEvento_EsEliminado()
        {
            eventoPage.GestionEvento();
            Thread.Sleep(2000);
            eventoPage.EliminarEvento("Jose Chaves Arce");
            Thread.Sleep(2000);
            Assert.IsTrue(eventoPage.EventoEliminado());
        }
    }
}
