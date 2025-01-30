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
    public class InformeTests
    {

        private IWebDriver driver;

        InformePage informePage;

        private readonly String downloadPath = @"C:\Users\camiu\Downloads\Informes";

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



            informePage = new InformePage(driver, downloadPath);
            informePage.Visit("http://localhost:7033/");

            informePage.IniciarSesion("camiulatech@gmail.com", "Hola321!");
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
        public void DescargarInformeInflablesPDF()
        {
            informePage.GestionInforme();
            Thread.Sleep(1500);
            informePage.DescargarPdfInformeInflable();
            Assert.IsTrue(informePage.VerificarArhivoDescargado("InformeInflablesSolicitados.pdf"));
        }


        [Test, Order(2)]
        public void DescargarInformeAlimentacionPDF()
        {
            informePage.GestionInforme();
            Thread.Sleep(1500);
            informePage.DescargarPdfInformeAlimentacion();
            Assert.IsTrue(informePage.VerificarArhivoDescargado("InformeAlimentosSolicitados.pdf"));
        }

        [Test, Order(3)]
        public void DescargarInformeMobiliarioPDF()
        {
            informePage.GestionInforme();
            Thread.Sleep(1500);
            informePage.DescargarPdfInformeMobiliario();
            Assert.IsTrue(informePage.VerificarArhivoDescargado("InformeMobiliariosSolicitados.pdf"));
        }
    }
}
