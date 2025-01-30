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
    public class HerramientasPage : Base
    {
        //Ir a gestion
        By BtnIrOtros = By.XPath("/html/body/div/main/section/div/div[2]/div[2]/div/div/a");
        By BtnIrHerramientas = By.XPath("/html/body/div/main/section/div/div/div[3]/div/div/a");

        //Pagina Principal
        By EspacioNombreTablaHerramientas = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[1]");
        By EspacioDescripcionTablaHerramientas = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[2]");
        By BtnEditarHerramienta = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[4]/div/a[1]");
        By BtnEliminarHerramienta = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[4]/div/a[2]");
        By InputBarraBusqueda = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By BtnCrearNuevaHerramienta = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By NingunRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");


        //Formulario
        By NombreEspacioFormularioHerramientas = By.Id("nombre");
        By DescripcionEspacioFormularioHerramientas = By.Id("Descripcion");
        By CantidadEspacioFormularioHerramientas = By.Id("Cantidad");
        By BtnCrearHerramienta = By.XPath("//*[@id=\"formHerramienta\"]/div/div[6]/div/button");
        By BtnGuardarCambios = By.XPath("//*[@id=\"formHerramienta\"]/div/div[6]/div/button");
        By MensajeNombreRepetidoHerramienta = By.XPath("/html/body/div[2]/div");

        By BtnConfirmarEliminarHerramienta = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");



        public HerramientasPage(IWebDriver driver) : base(driver)
        {

        }
        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }

        public Boolean HerramientaEstaRegistrado(String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            return IsDisplayed(EspacioNombreTablaHerramientas) && GetText(EspacioNombreTablaHerramientas) == nombre;
        }

        public void GestionHerramientas()
        {
            ClickElementUsingJS(BtnIrOtros);
            Thread.Sleep(2000);
            ClickElementUsingJS(BtnIrHerramientas);
        }

        public void CrearHerramienta(String nombre, String descripcion, String cantidad)
        {
            Click(BtnCrearNuevaHerramienta);
            Thread.Sleep(2000);
            Type(nombre, NombreEspacioFormularioHerramientas);
            Type(descripcion, DescripcionEspacioFormularioHerramientas);
            Type(cantidad, CantidadEspacioFormularioHerramientas);
            Click(BtnCrearHerramienta);
        }

        public void ActualizarDescripcionIngrediente(String descripcion, String nombre)
        {
            Type(nombre, InputBarraBusqueda);

            Click(BtnEditarHerramienta);
            Thread.Sleep(2000);
            Clear(DescripcionEspacioFormularioHerramientas);
            Type(descripcion, DescripcionEspacioFormularioHerramientas);
            Click(BtnGuardarCambios);

        }
        public void EliminarHerramienta(String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnEliminarHerramienta);
            Thread.Sleep(2000);
            Click(BtnConfirmarEliminarHerramienta);
            Thread.Sleep(2000);

        }
        public Boolean DescripcionHerramientaActualizado(String nombre, String descripcion)
        {
            Type(nombre, InputBarraBusqueda);
            return GetText(EspacioDescripcionTablaHerramientas) == descripcion;
        }

        public void RegistrarHerramientaConNombreRepetida(String nombre)
        {
            Click(BtnCrearNuevaHerramienta);
            Thread.Sleep(2000);
            Type(nombre + "\n", NombreEspacioFormularioHerramientas);


        }

        public Boolean MensajeErrorNombreRepetidoDesplegado()
        {
            return IsDisplayed(MensajeNombreRepetidoHerramienta);
        }
        public Boolean EstaHerramientaEliminada()
        {
            return IsDisplayed(NingunRegistroLocator);
        }
    }
}
