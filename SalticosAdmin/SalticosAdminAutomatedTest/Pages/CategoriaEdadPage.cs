using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class CategoriaEdadPage : Base
    {
        // Entrar gestión categoría edad
        By IrServiciosBtnLocator = By.XPath("/html/body/div/main/section/div/div[1]/div[3]/div/div/a");
        By IrInflablesBtnLocator = By.XPath("/html/body/div/main/section/div/div[1]/div[1]/div/div/a");
        By IrCategoriaEdadBtnLocator = By.XPath("/html/body/div/main/section/div/div/div[2]/div/div/a");

        // Crear categoría edad
        By CrearNuevaCategoriaEdadBtnLocator = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By NombreInputLocator = By.Id("nombre");
        By CrearBtnLocator = By.XPath("/html/body/div/main/form/div/div[4]/div/button");

        By BuscarInputLocator = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By NombreRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[1]");

        By MsgNombreRepetidoLocator = By.XPath("/html/body/div[2]/div/div[3]");

        // Actualizar categoría edad
        By EditarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[2]/div/a[1]/i");
        By GuardarCambiosBtnLocator = By.XPath("/html/body/div/main/form/div/div[4]/div/button");

        // Eliminar
        By EliminarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[2]/div/a[2]/i");
        By MensajeEliminarBtnLocator = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By NingunRegistroMsgLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");



        public CategoriaEdadPage(IWebDriver driver) : base(driver)
        {

        }

        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }


        public void GestionCategoriaEdad()
        {
            ClickElementUsingJS(IrServiciosBtnLocator);
            Thread.Sleep(2000);
            Click(IrInflablesBtnLocator);
            Thread.Sleep(2000);
            Click(IrCategoriaEdadBtnLocator);
        }


        public void AgregarCategoriaEdad(String nombre)
        {
            Click(CrearNuevaCategoriaEdadBtnLocator);
            Thread.Sleep(2000);
            Type(nombre, NombreInputLocator);
            Thread.Sleep(2000);

            Click(CrearBtnLocator);
        }


        public Boolean CategoriaEdadEstaRegistrada(String nombre)
        {
            Type(nombre, BuscarInputLocator);
            return IsDisplayed(NombreRegistroLocator) && GetText(NombreRegistroLocator) == nombre;
        }


        public void RegistrarCategoriaEdadConNombreRepetido(String nombre)
        {
            Click(CrearNuevaCategoriaEdadBtnLocator);
            Thread.Sleep(2000);
            Type(nombre + "\n", NombreInputLocator);

        }

        public Boolean MensajeErrorNombreRepetidoDesplegado()
        {
            return IsDisplayed(MsgNombreRepetidoLocator);
        }



        public void ActualizarNombreCategoriaEdad(String nombre, String nuevoNombre)
        {
            Type(nombre, BuscarInputLocator);

            Click(EditarBtnLocator);
            Thread.Sleep(2000);
            Clear(NombreInputLocator);
            Type(nuevoNombre, NombreInputLocator);
            Click(GuardarCambiosBtnLocator);

        }

        public Boolean NombreCategoriaEdadActualizado(String nombre)
        {
            Type(nombre, BuscarInputLocator);
            return GetText(NombreRegistroLocator) == nombre;
        }


        public void EliminarCategoriaEdad(String nombre)
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
