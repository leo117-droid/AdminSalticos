using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class CotizacionTests
    {

        private IWebDriver driver;

        CotizacionPage cotizacionPage;

        private readonly String downloadPath = @"C:\Users\yaira\Downloads\Cotizaciones";


        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions();
            options.AddUserProfilePreference("download.default_directory", downloadPath);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            options.AddUserProfilePreference("download.directory_upgrade", true);
            options.AddUserProfilePreference("safebrowsing.enabled", true);

            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();

            
            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }



            cotizacionPage = new CotizacionPage(driver, downloadPath);
            cotizacionPage.Visit("http://localhost:5270/");

            cotizacionPage.IniciarSesion("yoswi200210@gmail.com", "Hola321!");
        }


        [TearDown]
        public void TearDown()
        {
            driver.Quit();

            foreach (var file in Directory.GetFiles(downloadPath))
            {
                File.Delete(file);
            }
        }



        [Test, Order(1)]
        public void GenerarNuevaCotizacion()
        {
            cotizacionPage.GestionCotizacion();
            Thread.Sleep(2000);
            cotizacionPage.CrearCotizacion();
            Assert.IsTrue(cotizacionPage.EsCotizacionGenerada());
        }


        [Test, Order(2)]
        public void DescargarCotizacion()
        {
            cotizacionPage.GestionCotizacion();
            Thread.Sleep(2000);
            cotizacionPage.CrearCotizacion();
            cotizacionPage.DescargarPdf();
            Assert.IsTrue(cotizacionPage.VerificarArhivoDescargado("Cotizacion.pdf"));
        }

    }
}
