using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SalticosAdminAutomatedTest.Pages
{
    public class SegurosPage : Base
    {
        By BtnIrOtros = By.XPath("/html/body/div/main/section/div/div[2]/div[2]/div/div/a");
        By BtnIrSeguros = By.XPath("/html/body/div/main/section/div/div/div[5]/div/div/a");
        By BtnCrearNuevoSeguro = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By InputBarraBusqueda = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By EspacioNPolizaTablaSeguros = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[3]");
        By EspacioFechaVencimiento = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[5]");
        By BtnEditarSeguro = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[7]/div/a[1]");
        By BtnEliminarSeguro = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[7]/div/a[2]");

        By lblTipoSeguro = By.Id("TipoSeguro");
        By lblNombreAseguradora = By.Id("NombreAseguradora");
        By lblNumeroPoliza = By.Id("poliza");
        By SelectFechaInicio = By.Id("FechaInicio");
        By SelectFechaVencimiento = By.Id("FechaVencimiento");
        By DropdownEstado = By.Id("Estado");

        By BtnCrearSeguro = By.XPath("//*[@id=\"formSeguro\"]/div/div[9]/div/button");
        By BtnGuardarCambios = By.XPath("//*[@id=\"formSeguro\"]/div/div[9]/div/button");

        By BtnConfirmarEliminarSeguro = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");

        By NingunRegistroMensaje = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");
        By TipoRequeridoSeguro = By.XPath("//*[@id=\"formSeguro\"]/div/div[3]/div/span");





        public SegurosPage(IWebDriver driver) : base(driver)
        {

        }

        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }

        public Boolean SeguroEstaRegistrado(String poliza)
        {
            Type(poliza, InputBarraBusqueda);
            return IsDisplayed(EspacioNPolizaTablaSeguros) && GetText(EspacioNPolizaTablaSeguros) == poliza;
        }

        public void GestionSeguros()
        {
            ClickElementUsingJS(BtnIrOtros);
            Thread.Sleep(2000);
            ClickElementUsingJS(BtnIrSeguros);
        }

        public void CrearSeguro(String tipo, String aseguradora, String poliza, String FechaInicio, String FechaFinal, String Estado)
        {
            Click(BtnCrearNuevoSeguro);
            Thread.Sleep(2000);
            Type(tipo, lblTipoSeguro);
            Type(aseguradora, lblNombreAseguradora);
            Type(poliza, lblNumeroPoliza);
            SetDateUsingJS(FechaInicio, SelectFechaInicio);
            SetDateUsingJS(FechaFinal, SelectFechaVencimiento);
            SelectByVisibleTextUsingJS(Estado, DropdownEstado);
            Click(BtnCrearSeguro);
        }

        public void ActualizarFechaVencimiento(String poliza, String FechaVencimiento) 
        {
            Type(poliza, InputBarraBusqueda);

            Click(BtnEditarSeguro);
            Thread.Sleep(2000);
            SetDateUsingJS(FechaVencimiento, SelectFechaVencimiento);
            Click(BtnGuardarCambios);

        }

        public void EliminarSeguro(String poliza)
        {
            Type(poliza, InputBarraBusqueda);
            Click(BtnEliminarSeguro);
            Thread.Sleep(2000);
            Click(BtnConfirmarEliminarSeguro);
            Thread.Sleep(2000);

        }

        public Boolean FechaActualizada(String Fecha, String poliza)
        {
            Type(poliza, InputBarraBusqueda);
            return GetText(EspacioFechaVencimiento) == Fecha;

        }
        public bool SeguroEliminado()
        {
            return IsDisplayed(NingunRegistroMensaje);
        }

        public void CrearSeguroSinDatos()
        {
            Click(BtnCrearNuevoSeguro);
            Thread.Sleep(2000);
            Click(BtnCrearSeguro);
        }

        public Boolean MensajeErrorNoDatos() 
        {
            return IsDisplayed(TipoRequeridoSeguro);
        }

        public Boolean EstaSeguroEliminado()
        {
            return IsDisplayed(NingunRegistroMensaje);
        }
    }
}

