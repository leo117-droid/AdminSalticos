using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class TareaTests
    {
        private IWebDriver driver;
        TareaPage tareaPage;

        [SetUp]
        public void SetUp()
        {
            tareaPage = new TareaPage(driver);
            driver = tareaPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            tareaPage.Visit("https://localhost:7033/");

            tareaPage.IniciarSesion("camiulatech@gmail.com", "Hola321!");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        [Test, Order(1)]
        public void CrearTareaConDatosValidos_EsRegistrado()
        {
            tareaPage.GestionTareas();
            Thread.Sleep(2000);
            tareaPage.CrearTarea("Llamar a Yosward", "Llamar al veneco", "Alta", "Pendiente", "2026-10-28", "17:05");
            Thread.Sleep(2000);
            Assert.IsTrue(tareaPage.TareaEstaRegistrada());
        }
        [Test, Order(2)]
        public void EliminarTarea_SeMuestraVista() 
        {
            tareaPage.GestionTareas();
            Thread.Sleep(2000);
            tareaPage.EliminarTarea();
            Thread.Sleep(2000);
            Assert.IsTrue(tareaPage.TareaEstaRegistrada());
        }
    }
}
