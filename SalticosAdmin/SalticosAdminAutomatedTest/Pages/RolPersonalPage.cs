using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class RolPersonalPage : Base
    {

        By IrPersonalBtnLocator = By.XPath("/html/body/div/main/section/div/div[1]/div[2]/div/div/a");
        By IrRolPersonalBtnLocator = By.XPath("/html/body/div/main/section/div/div/div[2]/div/div/a");

        By CrearNuevoRolPersonalBtnLocator = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By NombreInputLocator = By.Id("nombre");
        By CrearBtnLocator = By.XPath("//*[@id=\"formRolPersonal\"]/div/div[4]/div/button");

        By BuscarInputLocator = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By NombreRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[1]");

        By EditarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[2]/div/a[1]");
        By GuardarCambiosBtnLocator = By.XPath("//*[@id=\"formRolPersonal\"]/div/div[4]/div/button");

        By EliminarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[2]/div/a[2]");
        By MensajeEliminarBtnLocator = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By NingunRegistroMsgLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");

        By NombreExisteMsgLocator = By.XPath("/html/body/div[2]/div/div[3]");

        public RolPersonalPage(IWebDriver driver) : base(driver)
        {

        }


        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }


        public void GestionRolPersonal()
        {
            ClickElementUsingJS(IrPersonalBtnLocator);
            Thread.Sleep(1500);
            Click(IrRolPersonalBtnLocator);
            Thread.Sleep(1500);
        }

        public void AgregarRolPersonal(String nombre)
        {
            Click(CrearNuevoRolPersonalBtnLocator);
            Thread.Sleep(2000);
            Type(nombre, NombreInputLocator);
            Thread.Sleep(2000);

            Click(CrearBtnLocator);
        }


        public Boolean RolPersonalEstaRegistrada(String nombre)
        {
            Type(nombre, BuscarInputLocator);
            return IsDisplayed(NombreRegistroLocator) && GetText(NombreRegistroLocator) == nombre;
        }



        public void RegistrarRolPersonalConNombreRepetido(String nombre)
        {
            Click(CrearNuevoRolPersonalBtnLocator);
            Thread.Sleep(2000);
            Type(nombre + "\n", NombreInputLocator);

        }

        public Boolean MensajeErrorNombreRepetidoDesplegado()
        {
            return IsDisplayed(NombreExisteMsgLocator);
        }



        public void ActualizarNombreRolPersonal(String nombre, String nuevoNombre)
        {
            Type(nombre, BuscarInputLocator);

            Click(EditarBtnLocator);
            Thread.Sleep(2000);
            Clear(NombreInputLocator);
            Type(nuevoNombre, NombreInputLocator);
            Click(GuardarCambiosBtnLocator);

        }

        public Boolean NombreRolPersonalActualizado(String nombre)
        {
            Type(nombre, BuscarInputLocator);
            return GetText(NombreRegistroLocator) == nombre;
        }


        public void EliminarRolPersonal(String nombre)
        {
            Type(nombre, BuscarInputLocator);
            Click(EliminarBtnLocator);
            Click(MensajeEliminarBtnLocator);
            Thread.Sleep(2000);

        }



        public Boolean EstaCategoriaEdadEliminado()
        {
            return IsDisplayed(NingunRegistroMsgLocator);
        }


    }
}
