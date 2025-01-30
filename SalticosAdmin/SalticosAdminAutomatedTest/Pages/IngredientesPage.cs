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
    public class IngredientesPage : Base
    {
        //Ruta para ir a Gestion de Ingredientes
        By BtnIrServicios = By.XPath("/html/body/div/main/section/div/div[1]/div[3]/div/div/a");
        By BtnIrAlimentacion = By.XPath("/html/body/div/main/section/div/div[2]/div[1]/div/div/a");
        By BtnIrIngredientes = By.XPath("/html/body/div/main/section/div/div/div[2]/div/div/a");


        //Pagina Principal tabla de datos
        By BtnCrearNuevoIngrediente = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By BtnEditarIngrediente = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[4]/div/a[1]");
        By BtnEliminarIngrediente = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[4]/div/a[2]");
        By EspacioNombreIngredienteTabla = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[1]");
        By EspacioDescripcionIngredienteTabla = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[2]");
        By InputBarraBusqueda = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By NingunRegistroTablaIngrediente = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");

        //Formulario ingrediente 
        By NombreEspacioFormularioIngrediente = By.Id("nombre");
        By DescripcionEspacioFormularioIngrediente = By.Id("Descripcion");
        By PrecioEspacioFormularioIngrediente = By.Id("Precio");
        By BtnCrearIngrediente = By.XPath("//*[@id=\"formIngrediente\"]/div/div[6]/div/button");
        By BtnGuardarCambios = By.XPath("//*[@id=\"formIngrediente\"]/div/div[6]/div/button");

        By BtnConfirmarEliminarIngrediente = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");



        public IngredientesPage(IWebDriver driver) : base(driver)
        {

        }
        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }

        public Boolean IngredienteEstaRegistrado(String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            return IsDisplayed(EspacioNombreIngredienteTabla) && GetText(EspacioNombreIngredienteTabla) == nombre;
        }

        public void GestionIngredientes()
        {
            ClickElementUsingJS(BtnIrServicios);
            Thread.Sleep(2000);
            ClickElementUsingJS(BtnIrAlimentacion);
            Thread.Sleep(2000);
            Click(BtnIrIngredientes);
        }
        public void CrearIngredientes(String nombre, String descripcion, String precio)
        {
            Click(BtnCrearNuevoIngrediente);
            Thread.Sleep(2000);
            Type(nombre, NombreEspacioFormularioIngrediente);
            Type(descripcion, DescripcionEspacioFormularioIngrediente);
            Type(precio, PrecioEspacioFormularioIngrediente);
            Click(BtnCrearIngrediente);
        }

        public void ActualizarDescripcionIngrediente(String descripcion, String nombre)
        {
            Type(nombre, InputBarraBusqueda);

            Click(BtnEditarIngrediente);
            Thread.Sleep(2000);
            Clear(DescripcionEspacioFormularioIngrediente);
            Type(descripcion, DescripcionEspacioFormularioIngrediente);
            Click(BtnGuardarCambios);

        }

        public void EliminarIngrediente(String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnEliminarIngrediente);
            Thread.Sleep(2000);
            Click(BtnConfirmarEliminarIngrediente);
            Thread.Sleep(2000);

        }
        public Boolean DescripcionIngredienteActualizado(String nombre, String descripcion)
        {
            Type(nombre, InputBarraBusqueda);
            return GetText(EspacioDescripcionIngredienteTabla) == descripcion;
        }
        public Boolean EstaIngredienteEliminado()
        {
            return IsDisplayed(NingunRegistroTablaIngrediente);
        }
    }
}
