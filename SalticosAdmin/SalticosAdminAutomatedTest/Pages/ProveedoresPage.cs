using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class ProveedoresPage : Base
    {
        By BtnIrOtros = By.XPath("/html/body/div/main/section/div/div[2]/div[2]/div/div/a");
        By BtnIrProveedores = By.XPath("/html/body/div/main/section/div/div/div[4]/div/div/a");
        By BtnCrearNuevoProveedor = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By EspacioCorreoTablaProveedores = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By BtnEditarProveedor = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[6]/div/a[1]");
        By BtnEliminarProveedor = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[6]/div/a[2]");
        By BtnConfirmarEliminacion = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By InputBarraBusqueda = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By EspacioNombreTablaProveedores = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[1]");
        By EspacioTelefonoTablaProveedores = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[3]");
        By MensajeSeguroRequerido = By.XPath("//*[@id=\"formSeguro\"]/div/div[3]/div/span");
        By MensajeAseguradoraRequerida = By.XPath("//*[@id=\"formSeguro\"]/div/div[4]/div/span");

        //Formulario

        By lblNombreProveedor = By.Id("NombreEmpresa");
        By lblContactoProveedor = By.Id("Contacto");
        By lblTelefonoProveedor = By.Id("Telefono");
        By lblCorreoProveedor = By.Id("Correo");
        By lblDireccionProveedor = By.Id("Direccion");
        By lblDescripcionProveedor = By.Id("Descripcion");
        By DropdownTipoProveedor = By.Id("tipoProveedor");
        By BtnCrearProveedor = By.XPath("//*[@id=\"formProveedor\"]/div/div[10]/div/button");
        By BtnGuardarCambios = By.XPath("//*[@id=\"formProveedor\"]/div/div[10]/div/button");

        By NingunRegistroMensaje = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");

        //Formulario


        public ProveedoresPage(IWebDriver driver) : base(driver)
        {

        }

        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }

        public Boolean ProveedorEstaRegistrado(String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            return IsDisplayed(EspacioNombreTablaProveedores) && GetText(EspacioNombreTablaProveedores) == nombre;
        }
        public void GestionProveedores()
        {
            ClickElementUsingJS(BtnIrOtros);
            Thread.Sleep(2000);
            ClickElementUsingJS(BtnIrProveedores);
        }
        public void CrearProveedor(String nombre, String contacto, String telefono, String correo, String direccion, String descripcion, String Tipo)
        {
            Click(BtnCrearNuevoProveedor);
            Thread.Sleep(2000);
            Type(nombre, lblNombreProveedor);
            Type(contacto, lblContactoProveedor);
            Type(telefono, lblTelefonoProveedor);
            Type(correo, lblCorreoProveedor);
            Type(direccion, lblDireccionProveedor);
            Type(descripcion, lblDescripcionProveedor);
            SelectByVisibleTextUsingJS(Tipo, DropdownTipoProveedor);
            Click(BtnCrearProveedor);
        }

        public void ActualizarTelefonoYCorreoProveedor(String telefono, String correo, String nombre)
        {
            Type(nombre, InputBarraBusqueda);

            Click(BtnEditarProveedor);
            Thread.Sleep(2000);
            Clear(lblTelefonoProveedor);
            Clear(lblCorreoProveedor);
            Type(telefono, lblTelefonoProveedor);
            Type(correo, lblCorreoProveedor);
            Click(BtnGuardarCambios);

        }

        public void EliminarProveedor(String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnEliminarProveedor);
            Thread.Sleep(2000);
            Click(BtnConfirmarEliminacion);
            Thread.Sleep(2000);

        }

        public Boolean TelefonoYCorreoActualizado(String telefono, String correo, String nombre)
        {
            Type(telefono, InputBarraBusqueda);
            return GetText(EspacioTelefonoTablaProveedores) == telefono && GetText(EspacioCorreoTablaProveedores) == correo;

        }
        public bool ProveedorEliminado()
        {
            return IsDisplayed(NingunRegistroMensaje);
        }

    }
}
