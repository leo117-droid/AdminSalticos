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
    public class ClientePage : Base
    {

        By BtnIrClientes = By.XPath("/html/body/div/main/section/div/div[1]/div[1]/div/div/a");
        By BtnIrGestionClientes = By.XPath("/html/body/div/main/section/div/div/div[2]/div/div/a");

        //Pagina principal GestionClientes

        By BtnCrearNuevoCliente = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By InputBarraBusqueda = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By BtnEditarCliente = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[6]/div/a[1]");
        By BtnEliminarCliente = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[6]/div/a[2]");
        By CampoCedulaTablaClientes = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[3]");
        By CampoNombreTablaClientes = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[1]");
        By NingunRegistroTablaClientes = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");


        //Botones CrearCliente

        By NombreEspacioCrearCliente = By.Id("nombre");
        By ApellidoEspacioCrearCliente = By.Id("Apellidos");
        By CedulaEspacioCrearCliente = By.Id("cedula");
        By TelefonoEspacioCrearCliente = By.Id("Telefono");
        By CorreoEspacioCrearCliente = By.Id("Correo");
        By BtnCrearCliente = By.XPath("//*[@id=\"formCliente\"]/div/div[8]/div/button");
        By MensajeCedulaYaExistente = By.XPath("/html/body/div[2]/div");

        //Botones ActualizarCliente 
        
        By NombreEspacioActualizarCliente = By.Id("nombre");
        By ApellidoEspacioActualizarCliente = By.Id("Apellidos");
        By CedulaEspacioActualizarCliente = By.Id("cedula");
        By TelefonoEspacioActualizarCliente = By.Id("Telefono");
        By CorreoEspacioActualizarCliente = By.Id("Correo");
        By BtnGuardarCambios = By.XPath("//*[@id=\"formCliente\"]/div/div[8]/div/button");

        //Btn Eliminar Cliente

        By BtnEliminarClienteConfirmacion = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        public ClientePage(IWebDriver driver) : base(driver)
        {

        }
        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }
        public Boolean ClienteEstaRegistrado(String cedula)
        {
            Type(cedula, InputBarraBusqueda);
            return IsDisplayed(CampoCedulaTablaClientes) && GetText(CampoCedulaTablaClientes) == cedula;
        }

        public void GestionClientes()
        {
            ClickElementUsingJS(BtnIrClientes);
            Thread.Sleep(2000);
            Click(BtnIrGestionClientes);
        }

        public void CrearCliente(String nombre, String apellido, String cedula, string telefono, string correo)
        {
            Click(BtnCrearNuevoCliente);
            Thread.Sleep(2000);
            Type(nombre, NombreEspacioCrearCliente);
            Type(apellido, ApellidoEspacioCrearCliente);
            Type(cedula, CedulaEspacioCrearCliente);
            Type(telefono, TelefonoEspacioCrearCliente);
            Type(correo, CorreoEspacioCrearCliente);
            ClickElementUsingJS(BtnCrearCliente);

        }

        public void RegistrarClienteConCedulaRepetida(String nombre, String apellido, String cedula)
        {
            Click(BtnCrearNuevoCliente);
            Thread.Sleep(2000);
            Type(nombre, NombreEspacioCrearCliente);
            Type(apellido, ApellidoEspacioCrearCliente);
            Type(cedula + "\n", CedulaEspacioCrearCliente);


        }
        public void ActualizarNombreCliente(String nombre, String cedula)
        {
            Type(cedula, InputBarraBusqueda);

            Click(BtnEditarCliente);
            Thread.Sleep(2000);
            Clear(NombreEspacioActualizarCliente);
            Type(nombre, NombreEspacioActualizarCliente);
            ClickElementUsingJS(BtnGuardarCambios);

        }

        public void EliminarCliente(String cedula)
        {
            Type(cedula, InputBarraBusqueda);
            Click(BtnEliminarCliente);
            Thread.Sleep(2000);
            Click(BtnEliminarClienteConfirmacion);
            Thread.Sleep(2000);

        }

        public Boolean NombreClienteActualizado(String nombre, String cedula)
        {
            Type(cedula, InputBarraBusqueda);
            return GetText(CampoNombreTablaClientes) == nombre;
        }

        public Boolean MensajeErrorCedulaRepetidaDesplegado()
        {
            return IsDisplayed(MensajeCedulaYaExistente);
        }
        public Boolean EstaClienteEliminado()
        {
            return IsDisplayed(NingunRegistroTablaClientes);
        }

        

    }
}
