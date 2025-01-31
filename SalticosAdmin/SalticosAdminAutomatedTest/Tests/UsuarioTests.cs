using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class UsuarioTests
    {


        private IWebDriver driver;
        private UsuarioPage usuarioPage;


        [SetUp]
        public void SetUp()
        {
            usuarioPage = new UsuarioPage(driver);
            driver = usuarioPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            usuarioPage.Visit("https://localhost:7033/");

            usuarioPage.IniciarSesion("camiulatech@gmail.com", "Hola321!");
        }


        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }



        [Test, Order(1)]
        public void CrearNuevoUsuarioConDatosValidos_EsRegistrado()
        {
            usuarioPage.GestionUsuario();
            usuarioPage.CrearUsuario("Jesús", "Calvo", "jesus@gmail.com", "Jesmusic24!", "Jesmusic24!");
            Thread.Sleep(1500);
            Assert.IsTrue(usuarioPage.UsuarioEstaRegistrado("jesus@gmail.com"));
        }

        [Test, Order(2)]
        public void ActualizarCorreoUsuario_EsActualizado()
        {
            usuarioPage.GestionUsuario();
            Thread.Sleep(1500);
            usuarioPage.EditarEmailUsuario("jesus@gmail.com", "jcalvo@gmail.com");
            Thread.Sleep(1500);
            Assert.IsTrue(usuarioPage.UsuarioEstaRegistrado("jcalvo@gmail.com"));
        }


        [Test, Order(3)]
        public void EliminarUsuario_EsEliminado()
        {
            usuarioPage.GestionUsuario();
            Thread.Sleep(1500);
            usuarioPage.EliminarUsuario("jcalvo@gmail.com");
            Thread.Sleep(1500);
            Assert.IsTrue(usuarioPage.UsuarioEliminado());
        }
    }
}
