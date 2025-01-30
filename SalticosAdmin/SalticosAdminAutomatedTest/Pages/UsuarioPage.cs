using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class UsuarioPage : Base
    {

        By IrOtrosBtnLocator = By.XPath("/html/body/div/main/section/div/div[2]/div[2]/div/div/a");
        By IrUsuariosBtnLocator = By.XPath("/html/body/div/main/section/div/div/div[6]/div/div/a");

        By CrearNuevoUsuarioBtnLocator = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By NombreInputLocator = By.Id("Input_Nombre");
        By ApellidoInputLocator = By.Id("Input_Apellido");
        By EmailInputLocator = By.Id("Input_Email");
        By InputContrasennaLocator = By.Id("Input_Password");
        By ConfirmarContrasennaLocator = By.Id("Input_ConfirmPassword");

        By RegistrarBtnLocator = By.Id("registerSubmit");

        By BuscarInputLocator = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By EmailRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[3]");

        By EditarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[4]/div/a[1]/i");
        By EditarCorreoInputLocator = By.XPath("//*[@id=\"Email\"]");
        By GuardarCambiosBtnLocator = By.XPath("/html/body/div/main/form/div/div[6]/div/button");


        By EliminarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[4]/div/a[2]");
        By ConfirmacionBorrarBtnLocator = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By NingunRegistroMsgLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");

        public UsuarioPage(IWebDriver driver) : base(driver)
        {
        }


        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }



        public void GestionUsuario()
        {
            Click(IrOtrosBtnLocator);
            Thread.Sleep(1500);
            Click(IrUsuariosBtnLocator);
            Thread.Sleep(1500);

        }

        public void CrearUsuario(string nombre, string apellido, string email, string contrasenna, string confirmarContrasenna)
        {
            Click(CrearNuevoUsuarioBtnLocator);
            Thread.Sleep(1500);
            Type(nombre, NombreInputLocator);
            Type(apellido, ApellidoInputLocator);
            Type(email, EmailInputLocator);
            Type(contrasenna, InputContrasennaLocator);
            Type(confirmarContrasenna, ConfirmarContrasennaLocator);
            Click(RegistrarBtnLocator);
        }


        public bool UsuarioEstaRegistrado(string email)
        {
            Type(email, BuscarInputLocator);
            Thread.Sleep(1500);
            return IsDisplayed(EmailRegistroLocator) && GetText(EmailRegistroLocator) == email;
        }


        public void EditarEmailUsuario(string email, string nuevoEmail)
        {
            Type(email, BuscarInputLocator);
            Thread.Sleep(1500);
            Click(EditarBtnLocator);
            Thread.Sleep(1500);
            Clear(EditarCorreoInputLocator);
            Type(nuevoEmail, EditarCorreoInputLocator);
            Click(GuardarCambiosBtnLocator);
            Thread.Sleep(1500);
        }


        public void EliminarUsuario(string email)
        {
            Type(email, BuscarInputLocator);
            Thread.Sleep(1500);
            Click(EliminarBtnLocator);
            Thread.Sleep(1500);
            Click(ConfirmacionBorrarBtnLocator);
            Thread.Sleep(1500);
        }

        public bool UsuarioEliminado()
        {
            return IsDisplayed(NingunRegistroMsgLocator);
        }



    }
    
   }

