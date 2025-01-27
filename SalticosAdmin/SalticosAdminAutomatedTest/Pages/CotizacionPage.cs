using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class CotizacionPage : Base
    {

        By CotizacionBtnLocator = By.XPath("/html/body/header/nav/div/div/ul[1]/li[1]/a");
        By InflableCheckLocator = By.XPath("/html/body/div/main/div/form/table[1]/tbody/tr[1]/td[1]/input");
        By MobiliarioCheckLocator = By.XPath("/html/body/div/main/div/form/table[2]/tbody/tr[1]/td[1]/input");
        By ServicioCheckLocator = By.XPath("/html/body/div/main/div/form/table[3]/tbody/tr[1]/td[1]/input");
        By AlimentacionCheckLocator = By.XPath("/html/body/div/main/div/form/table[4]/tbody/tr[1]/td[1]/input");
        By ProvinciaSelectLocator = By.XPath("//*[@id=\"transporteIds\"]");

        By GenerarCotizacionBtnLocator = By.XPath("/html/body/div/main/div/form/div[2]/button");

        By MontoTotalLocator = By.XPath("/html/body/div/main/h3[1]");

        By DescargarPdfBtnLocator = By.XPath("/html/body/div/main/div/div/div[2]/form/button");


        private readonly String downloadPath;

        public CotizacionPage(IWebDriver driver, String downloadPath) : base(driver)
        {
            this.downloadPath = downloadPath;
        }


        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }


        public void SeleccionarProvincia()
        {
            
            var provinciaElement = driver.FindElement(By.Id("transporteIds")); 

            var jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript(@"
                arguments[0].selectedIndex = 0; // Cambia el índice aquí
                const event = new Event('change'); // Crea el evento 'change'
                arguments[0].dispatchEvent(event); // Dispara el evento 'change'
            ", provinciaElement);
        }

        public void GestionCotizacion()
        {
            Click(CotizacionBtnLocator);

        }

        public void CrearCotizacion()
        {
            ClickElementUsingJS(InflableCheckLocator);
            ClickElementUsingJS(MobiliarioCheckLocator);
            ClickElementUsingJS(ServicioCheckLocator);
            ClickElementUsingJS(AlimentacionCheckLocator);
            SeleccionarProvincia();
            ClickElementUsingJS(GenerarCotizacionBtnLocator);
        }

        public Boolean EsCotizacionGenerada()
        {
            return IsDisplayed(MontoTotalLocator);
        }


        public void DescargarPdf()
        {
            ClickElementUsingJS(DescargarPdfBtnLocator);
        }


        public bool VerificarArhivoDescargado(String nombreArchivo, int tiempoEspera=10)
        {
            var filePath = Path.Combine(downloadPath, nombreArchivo);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            while (stopwatch.Elapsed.TotalSeconds < tiempoEspera)
            {
                if (File.Exists(filePath))
                {
                    return true;
                }
                Thread.Sleep(500); // Esperar un poco antes de volver a verificar
            }
            return false;
        }

    }
}
