using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class TarifasDeTransportePage : Base
    {
        By BtnIrTransporteLocator = By.XPath("/html/body/div/main/section/div/div[2]/div[1]/div/div/a");
        By BtnIrTarifasTransporte = By.XPath("/html/body/div/main/section/div/div/div[2]/div/div/a");
        By EspacioProvinciaLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[1]");

        //Pagina Gestion Tarifas

        By BtnCrearTarifaTransporte = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By BtnEditarTarifaTransporte = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[3]/div/a[1]");
        By BtnEliminarTarifaTransporte = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[3]/div/a[2]");
        By InputBarraBusqueda = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By NingunRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");

        By MensajePrecioNegativo = By.XPath("//*[@id=\"formTarifa\"]/div/div[4]/div/span");

        By BtnConfirmarEliminarTarifa = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");

        By EspacioPrecioTablaTarifas = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[2]");
        //Pagina Crear Tarifa 

        By DropdownProvincia = By.Id("provincia");
        By EspacioPrecioTarifa = By.Id("Precio");
        By BtnCrearTarifa = By.XPath("//*[@id=\"formTarifa\"]/div/div[5]/div/button");
        By BtnGuardarCambios = By.XPath("//*[@id=\"formTarifa\"]/div/div[5]/div/button");

        public TarifasDeTransportePage(IWebDriver driver) : base(driver)
        {

        }

        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }

        public Boolean TarifaEstaRegistrada(String provincia)
        {
            Type(provincia, InputBarraBusqueda);
            return IsDisplayed(EspacioProvinciaLocator) && GetText(EspacioProvinciaLocator) == provincia;
        }

        public void GestionTarifasDeTransporte()
        {
            ClickElementUsingJS(BtnIrTransporteLocator);
            Thread.Sleep(2000);
            ClickElementUsingJS(BtnIrTarifasTransporte);
        }

        public void CrearTarifa(String provincia, String precio)
        {
            Click(BtnCrearTarifaTransporte);
            Thread.Sleep(2000);
            SelectByVisibleTextUsingJS(provincia, DropdownProvincia);
            Type(precio, EspacioPrecioTarifa);
            Click(BtnCrearTarifa);
        }
        public void ActualizarPrecioTarifa(String precio, String provincia)
        {
            Type(provincia, InputBarraBusqueda);

            Click(BtnEditarTarifaTransporte);
            Thread.Sleep(2000);
            Clear(EspacioPrecioTarifa);
            Type(precio, EspacioPrecioTarifa);
            Click(BtnGuardarCambios);

        }

        public void CrearTarifaConPrecioNegativo(String provincia, String precio)
        {
            Click(BtnCrearTarifaTransporte);
            Thread.Sleep(2000);
            SelectByVisibleTextUsingJS(provincia, DropdownProvincia);
            Type(precio + "\n", EspacioPrecioTarifa);


        }
        public void EliminarTarifa(String provincia)
        {
            Type(provincia, InputBarraBusqueda);
            Click(BtnEliminarTarifaTransporte);
            Thread.Sleep(2000);
            Click(BtnConfirmarEliminarTarifa);
            Thread.Sleep(2000);

        }

        public bool PrecioNegativoMsgDesplegado()
        {
            return IsDisplayed(MensajePrecioNegativo);
        }

        public bool TarifaEliminada()
        {
            return IsDisplayed(NingunRegistroLocator);
        }

        public Boolean PrecioTarifaActualizado(String precio, String provincia)
        {
            Type(provincia, InputBarraBusqueda);
            return GetText(EspacioPrecioTablaTarifas) == precio;
        }
    }
}