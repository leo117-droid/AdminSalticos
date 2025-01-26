using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class ContactoPage : Base
    {

        By BtnActualizarContacto = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[7]/div/a[1]");
        By BtnEliminarContacto = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[7]/div/a[2]");
        By InputBarraBusqueda = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By BtnIrOtros = By.XPath("/html/body/div/main/section/div/div[2]/div[2]/div/div/a");
        By BtnIrContactos = By.XPath("/html/body/div/main/section/div/div/div[2]/div/div/a");
        By CorreoTablaLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[6]");

        //Boton de Upsert Locators
        By BtnCrearNuevoContacto = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By NombreLocator = By.Id("nombre");
        By ApellidoLocator = By.Id("Apellido");
        By TipoServicioLocator = By.Id("TipoServicio");
        By DireccionLocator = By.Id("Direccion");
        By TelefonoLocator = By.Id("Telefono");
        By CorreoLocator = By.Id("Correo");
        By BtnCrearContactoLocator = By.XPath("/html/body/div/main/form/div/div[9]/div/button");

        //Mensaje Eliminar Contacto

        By BtnMensajeEliminarContacto = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");


        public ContactoPage(IWebDriver driver) : base(driver)
        {
        }

        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }

        public Boolean ContactoEstaRegistrado(String correo)
        {
            Type(correo, InputBarraBusqueda);
            return IsDisplayed(CorreoTablaLocator) && GetText(CorreoTablaLocator) == correo;
        }

        public void GestionContactos()
        {
            Click(BtnIrOtros);
            Click(BtnIrContactos);
        }

        public void CrearContacto(String nombre, String apellido, String tipoServicio, String direccion, string telefono, string correo) 
        {
            Click(BtnCrearNuevoContacto);
            Thread.Sleep(2000);
            Type(nombre, NombreLocator);
            Type(apellido, ApellidoLocator);
            Type(tipoServicio, TipoServicioLocator);
            Type(direccion, DireccionLocator);
            Type(telefono, TelefonoLocator);
            Type(correo, CorreoLocator);
            Click(BtnCrearContactoLocator);
        }

        public void EliminarContacto(String contacto)
        {
            Type(contacto, InputBarraBusqueda);
            Click(BtnEliminarContacto);
            Click(BtnMensajeEliminarContacto);
            Thread.Sleep(2000);

        }
    }
}
