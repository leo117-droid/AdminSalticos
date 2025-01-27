using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class ServicioAdicionalPage : Base
    {

        By IrServiciosBtnLocator = By.XPath("/html/body/div/main/section/div/div[1]/div[3]/div/div/a");
        By IrServicioAdicionalBtnLocator = By.XPath("/html/body/div/main/section/div/div[2]/div[2]/div/div/a");

        By CrearNuevoServicioLocator = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By NombreInputLocator = By.Id("nombre");
        By DescripcionInputLocator = By.XPath("//*[@id=\"Descripcion\"]");
        By ImagenInputLocator = By.Id("imagenId");
        By InventarioInputLocator = By.Id("Inventario");
        By PrecioInputLocator = By.Id("Precio");

        By CrearBtnLocator = By.XPath("//*[@id=\"formServicioAdicional\"]/div/div[2]/div[1]/div/div[6]/div/button");

        By BuscarInputLocator = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By NombreRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[1]");

        By NombreRepetidoMsgLocator = By.XPath("/html/body/div[2]/div/div[3]");

        By EditarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[5]/div/a[1]");
        By GuardarCambiosLocator = By.XPath("//*[@id=\"formServicioAdicional\"]/div/div[2]/div[1]/div/div[6]/div/button");

        By InventarioRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[3]");

        By EliminarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[5]/div/a[2]");
        By ElimnarConfirmacionBtnLocator = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By NingunRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");

        public ServicioAdicionalPage(IWebDriver driver) : base(driver)
        {

        }


        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }


        public void GestionServicioAdicional()
        {
            ClickElementUsingJS(IrServiciosBtnLocator);
            Thread.Sleep(1500);
            ClickElementUsingJS(IrServicioAdicionalBtnLocator);
            Thread.Sleep(1500);
        }


        public void CrearServicioAdicional(String nombre, String descripcion, String imagen, String inventario, String precio)
        {
            Click(CrearNuevoServicioLocator);
            Thread.Sleep(1500);
            Type(nombre, NombreInputLocator);
            Type(descripcion, DescripcionInputLocator);
            Type(imagen, ImagenInputLocator);
            Clear(InventarioInputLocator);
            Type(inventario, InventarioInputLocator);
            Clear(PrecioInputLocator);
            Type(precio, PrecioInputLocator);
            Click(CrearBtnLocator);
        }


        public Boolean ServicioAdicionalEstaRegistrado(String nombre)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            return IsDisplayed(NombreRegistroLocator);
        }

        public void CrearServicioAdicionalConNombreRepetido(String nombre)
        {
            Click(CrearNuevoServicioLocator);
            Thread.Sleep(1500);
            Type(nombre + "\n", NombreInputLocator);
        }

        public Boolean MensajeErrorNombreRepetidoDesplegado()
        {
            return IsDisplayed(NombreRepetidoMsgLocator);
        }

        public void ActualizarInventarioServicioAdicional(String nombre, String inventario)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            Click(EditarBtnLocator);
            Thread.Sleep(1500);
            Clear(InventarioInputLocator);
            Type(inventario, InventarioInputLocator);
            Click(GuardarCambiosLocator);
        } 


        public Boolean InventarioServicioAdicionalActualizado(String nombre, String inventario)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            return GetText(NombreRegistroLocator) == nombre 
                && GetText(InventarioRegistroLocator) == inventario;
        }



        public void EliminarServicioAdicional(String nombre)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            Click(EliminarBtnLocator);
            Thread.Sleep(1500);
            Click(ElimnarConfirmacionBtnLocator);

        }


        public Boolean ServicioAdicionalEliminado()
        {
            return IsDisplayed(NingunRegistroLocator);
        }




    }
}
