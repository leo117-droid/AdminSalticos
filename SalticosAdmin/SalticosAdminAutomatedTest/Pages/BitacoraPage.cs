using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class BitacoraPage : Base
    {

        By IrOtrosBtnLocator = By.XPath("/html/body/div/main/section/div/div[2]/div[2]/div/div/a");
        By IrBitacoraBtnLocator = By.XPath("/html/body/div/main/section/div/div/div[1]/div/div/a");

        By FechaInicioInputLocator = By.Id("fechaInicio");
        By FechaFinInputLocator = By.Id("fechaFin");
        By BuscarBtnLocator = By.Id("btnBuscar");

        By RegistrosLocator = By.TagName("tr");

        public BitacoraPage(IWebDriver driver) : base(driver)
        {

        }


        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }

        public void GestionBitacora()
        {
            ClickElementUsingJS(IrOtrosBtnLocator);
            Click(IrBitacoraBtnLocator);
        }

        public void FiltrarBitacora(String fechaInicio, String fechaFin)
        {
            Type(fechaInicio, FechaInicioInputLocator);
            Type(fechaFin, FechaFinInputLocator);
            Click(BuscarBtnLocator);
        }

        public Boolean FiltraResultados(String fechaInicio, String fechaFin)
        {
            DateTime inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            List<IWebElement> registros = FindElements(RegistrosLocator);
            for (int i = 1; i < registros.Count; i++)
            {
                List<IWebElement> columnas = registros[i].FindElements(By.TagName("td")).ToList();

                if (columnas.Count > 0)
                {
                    string fechaTexto = columnas[0].Text;
                    DateTime fecha = DateTime.ParseExact(fechaTexto, "dd MMM yyyy", new CultureInfo("es-ES"));

                    if (fecha < inicio || fecha > fin)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        
    }
}
