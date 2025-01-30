using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class PersonalPage : Base
    {
        By BtnIrPersonal = By.XPath("/html/body/div/main/section/div/div[1]/div[2]/div/div/a");
        By BtnIrPersonal2 = By.XPath("/html/body/div/main/section/div/div/div[1]/div/div/a");
        By BtnCrearNuevoPersonal = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By BtnCrearPersonal = By.XPath("//*[@id=\"formPersonal\"]/div/div[11]/div/button");
        By InputBarraBusqueda = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By BtnEditarPersonal = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[9]/div/a[1]");
        By BtnEliminarPersonal = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[9]/div/a[2]");
        By EspacioCedulaTablaPersonal = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[5]");
        By BtnConfirmarEliminarPersonal = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By EspacioCorreoTablaPersonal = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[4]");
        By lblNombreFormulario = By.Id("Personal_Nombre");
        By lblApellidoFormulario = By.Id("Personal_Apellidos");
        By lblTelefonoFormulario = By.Id("Personal_Telefono");
        By lblCorreoFormulario = By.Id("Personal_Correo");
        By lblCedulaFormulario = By.Id("cedula");
        By SelectFechaNacimiento = By.Id("Personal_FechaNacimiento");
        By SelectFechaEntrada = By.Id("Personal_FechaEntrada");
        By DropdownRol = By.XPath("//*[@id=\"Personal_RolPersonalId\"]");
        By BtnGuardarCambios = By.XPath("//*[@id=\"formPersonal\"]/div/div[11]/div/button");
        By MensajeNingunRegistro = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");
        By MensajeCedulaRepetida = By.XPath("/html/body/div[2]/div");

        public PersonalPage(IWebDriver driver) : base(driver)
        {

        }

        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }

        public Boolean PersonalEstaRegistrado(String cedula)
        {
            Type(cedula, InputBarraBusqueda);
            return IsDisplayed(EspacioCedulaTablaPersonal) && GetText(EspacioCedulaTablaPersonal) == cedula;

        }

        public void GestionPersonal()
        {
            ClickElementUsingJS(BtnIrPersonal);
            Thread.Sleep(2000);
            ClickElementUsingJS(BtnIrPersonal2);
        }

        public void CrearPersonal(String nombre, String apellido, String telefono, String correo, String cedula, String FechaNacimiento, String FechaEntrada, String Rol)
        {
            Click(BtnCrearNuevoPersonal);
            Thread.Sleep(2000);
            Type(nombre, lblNombreFormulario);
            Type(apellido, lblApellidoFormulario);
            Type(telefono, lblTelefonoFormulario);
            Type(correo, lblCorreoFormulario);
            Type(cedula, lblCedulaFormulario);
            SetDateUsingJS(FechaNacimiento, SelectFechaNacimiento);
            SetDateUsingJS(FechaEntrada, SelectFechaEntrada);
            SelectByVisibleTextUsingJS(Rol, DropdownRol);
            ClickElementUsingJS(BtnCrearPersonal);
        }

        public void CrearPersonal_CedulaRepetida(String nombre, String apellido, String telefono, String correo, String cedula) 
        {
            Click(BtnCrearNuevoPersonal);
            Thread.Sleep(2000);
            Type(nombre, lblNombreFormulario);
            Type(apellido, lblApellidoFormulario);
            Type(telefono, lblTelefonoFormulario);
            Type(correo, lblCorreoFormulario);
            Type(cedula + "\n", lblCedulaFormulario);
            
        }
        public void ActualizarCorreoPersonal(String cedula, String correo) 
        {
            Type(cedula, InputBarraBusqueda);

            Click(BtnEditarPersonal);
            Thread.Sleep(2000);
            Clear(lblCorreoFormulario);
            Type(correo, lblCorreoFormulario);
            ClickElementUsingJS(BtnGuardarCambios);
        }

        public void EliminarPersonal(String cedula)
        {
            Type(cedula, InputBarraBusqueda);
            Click(BtnEliminarPersonal);
            Thread.Sleep(2000);
            Click(BtnConfirmarEliminarPersonal);
            Thread.Sleep(2000);

        }
        public Boolean CorreoActualizado(String cedula, String correo)
        {
            Type(cedula, InputBarraBusqueda);
            return GetText(EspacioCorreoTablaPersonal) == correo;

        }
        public bool PersonalEliminado()
        {
            return IsDisplayed(MensajeNingunRegistro);
        }

        public bool MensajeDespliegaCedulaRepetida() 
        {
            return IsDisplayed(MensajeCedulaRepetida);
        }
    }
}
