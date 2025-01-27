using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class InformePage : Base
    {

        By InformeBtnLocator = By.XPath("/html/body/header/nav/div/div/ul[1]/li[2]/a");
        By DescargarInformeInflableBtnLocator = By.XPath("/html/body/div/main/div/div[1]/div[2]/form/button");
        By DescargarInformeAlimentacionBtnLocator = By.XPath("/html/body/div/main/div/div[2]/div[2]/form/button");
        By DescargarInformeMobiliarioBtnLocator = By.XPath("/html/body/div/main/div/div[3]/div[2]/form/button");

        private readonly String downloadPath;


        public InformePage(IWebDriver driver, string downloadPath) : base(driver)
        {
            this.downloadPath = downloadPath;
        }


        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }


        public void GestionInforme()
        {
            Click(InformeBtnLocator);
        }

        public void DescargarPdfInformeInflable()
        {
            ClickElementUsingJS(DescargarInformeInflableBtnLocator);
        }

        public void DescargarPdfInformeAlimentacion()
        {
            ClickElementUsingJS(DescargarInformeAlimentacionBtnLocator);
        }

        public void DescargarPdfInformeMobiliario()
        {
            ClickElementUsingJS(DescargarInformeMobiliarioBtnLocator);
        }


        public bool VerificarArhivoDescargado(String nombreArchivo, int tiempoEspera = 10)
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
