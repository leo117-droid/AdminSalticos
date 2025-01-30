using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class ClienteTests
    {
        private IWebDriver driver;
        ClientePage clientePage;

        [SetUp]

        public void SetUp() 
        {
            clientePage = new ClientePage(driver);
            driver = clientePage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            clientePage.Visit("https://localhost:7033/");

            clientePage.IniciarSesion("leomora641@gmail.com", "Hola123.");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        [Test, Order(1)]
        public void CrearClienteConDatosValidos_EsRegistrado()
        {
            clientePage.GestionClientes();
            Thread.Sleep(2000);
            clientePage.CrearCliente("Yowi", "Garcia", "112233", "89996655", "yowi@gmail.com");
            Thread.Sleep(2000);
            Assert.IsTrue(clientePage.ClienteEstaRegistrado("112233"));
        }

        [Test, Order(2)]

        public void CrearClienteConCedulaExistente_MuestraMensajeDeError()
        {
            clientePage.GestionClientes();
            Thread.Sleep(2000);
            clientePage.RegistrarClienteConCedulaRepetida("Maria", "Alvarez", "112233");
            Thread.Sleep(2000);
            Assert.IsTrue(clientePage.MensajeErrorCedulaRepetidaDesplegado());
        }


        [Test, Order(3)]
        public void ActualizarNombreCliente_EsActualizado()
        {
            clientePage.GestionClientes();
            Thread.Sleep(2000);
            clientePage.ActualizarNombreCliente("Yosward", "112233");
            Thread.Sleep(2000);
            Assert.IsTrue(clientePage.NombreClienteActualizado("Yosward", "112233"));
        }

        [Test, Order(4)]
        public void EliminarCliente_EsEliminado()
        {
            clientePage.GestionClientes();
            Thread.Sleep(2000);
            clientePage.EliminarCliente("112233");
            Thread.Sleep(2000);
            Assert.IsTrue(clientePage.EstaClienteEliminado());
        }
    }
}
