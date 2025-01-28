using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class MobiliarioPage : Base
    {

        By IrServiciosBtnLocator = By.XPath("/html/body/div/main/section/div/div[1]/div[3]/div/div/a");
        By IrMobiliarioBtnLocator = By.XPath("/html/body/div/main/section/div/div[1]/div[2]/div/div/a");

        By CrearNuevoMobiliarioLocator = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By NombreInputLocator = By.Id("nombre");
        By DescripcionInputLocator = By.Id("Descripcion");
        By DimensionesInputLocator = By.Id("Dimensiones");
        By ImagenInputLocator = By.Id("imagenId");
        By PrecioInputLocator = By.Id("Precio");
        By InventarioInputLocator = By.Id("Inventario");
        By CrearBtnLocator = By.XPath("//*[@id=\"formMobiliario\"]/div/div[2]/div[1]/div/div[7]/div/button");

        By BuscarInputLocator = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By NombreRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[1]");

        By EditarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[6]/div/a[1]/i");
        By GuardarCambiosLocator = By.XPath("//*[@id=\"formMobiliario\"]/div/div[2]/div[1]/div/div[7]/div/button");
        By DescripcionRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[2]");

        By EliminarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[6]/div/a[2]/i");
        By EliminarConfirmacionBtnLocator = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By NingunRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");


        public MobiliarioPage(IWebDriver driver) : base(driver)
        {

        }


        public void IniciarSesion(string usuario, string contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }


        public void GestionMobiliario()
        {
            ClickElementUsingJS(IrServiciosBtnLocator);
            Thread.Sleep(1500);
            ClickElementUsingJS(IrMobiliarioBtnLocator);
            Thread.Sleep(1500);
        }


        public void CrearMobiliario(string nombre, string descripcion, string dimensiones, string imagen,
            string precio, string inventario)
        {
            Click(CrearNuevoMobiliarioLocator);
            Thread.Sleep(1500);
            Type(nombre, NombreInputLocator);
            Type(descripcion, DescripcionInputLocator);
            Type(dimensiones, DimensionesInputLocator);
            Type(imagen, ImagenInputLocator);
            Clear(PrecioInputLocator);
            Type(precio, PrecioInputLocator);
            Clear(InventarioInputLocator);
            Type(inventario, InventarioInputLocator);
            Click(CrearBtnLocator);
        }


        public bool MobiliarioEstaRegistrado(string nombre)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            return IsDisplayed(NombreRegistroLocator);
        }


        public void ActualizarDescripcionMobiliario(string nombre, string nuevaDescripcion)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            Click(EditarBtnLocator);
            Thread.Sleep(1500);
            Clear(DescripcionInputLocator);
            Type(nuevaDescripcion, DescripcionInputLocator);
            Click(GuardarCambiosLocator);
        }


        public bool DescripcionMobiliarioActualizada(string nombre, string nuevaDescripcion)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            return IsDisplayed(DescripcionRegistroLocator) 
                && GetText(DescripcionRegistroLocator) == nuevaDescripcion;
        }


        public void EliminarMobiliario(string nombre)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            Click(EliminarBtnLocator);
            Thread.Sleep(1500);
            Click(EliminarConfirmacionBtnLocator);
        }

        public bool MobiliarioEliminado()
        {
            return IsDisplayed(NingunRegistroLocator);
        }

    }
}
