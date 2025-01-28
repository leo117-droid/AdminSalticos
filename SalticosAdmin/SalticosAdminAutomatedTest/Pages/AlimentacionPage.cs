using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class AlimentacionPage : Base
    {

        By IrServiciosBtnLocator = By.XPath("/html/body/div/main/section/div/div[1]/div[3]/div/div/a");
        By IrAlimentacionIntermediaBtnLocator = By.XPath("/html/body/div/main/section/div/div[2]/div[1]/div/div/a");
        By IrAlimentacionBtnLocator = By.XPath("/html/body/div/main/section/div/div/div[1]/div/div/a");

        By CrearNuevoAlimentoLocator = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By NombreInputLocator = By.Id("nombre");
        By DescripcionInputLocator = By.Id("Descripcion");
        By ImagenInputLocator = By.Id("imagenId");
        By PrecioInputLocator = By.Id("Precio");
        By CreartBtnLocator = By.XPath("//*[@id=\"formAlimentacion\"]/div/div[2]/div[1]/div/div[5]/div/button/i");

        By BuscarInputLocator = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By NombreRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[1]");

        By EditarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[4]/div/a[1]/i");
        By GuardarCambiosBtnLocator = By.XPath("//*[@id=\"formAlimentacion\"]/div/div[2]/div[1]/div/div[5]/div/button/i");
        By PrecioRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[3]");

        By EliminarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[4]/div/a[3]/i");
        By EliminarConfirmacionBtnLocator = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By NingunRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");

        //Alimentos ingredientes
        By IrAlimentosIngredientesBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[4]/div/a[2]");
        By AgregarNuevoIngredienteBtnLocator = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By IngredienteSelectLocator = By.XPath("//*[@id=\"IdIngrediente\"]");
        By IngredienteCrearBtnLocator = By.XPath("//*[@id=\"formAlimentacionIngrediente\"]/div/div[3]/button");
        By IngredienteNombreRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[1]");

        By EliminarIngredienteBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[4]/div/a/i");
        By EliminarIngredienteConfirmacionBtnLocator = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");





        public AlimentacionPage(IWebDriver driver) : base(driver)
        {

        }

        public void IniciarSesion(string usuario, string contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }


        public void GestionAlimentacion()
        {
            ClickElementUsingJS(IrServiciosBtnLocator);
            Thread.Sleep(1500);
            ClickElementUsingJS(IrAlimentacionIntermediaBtnLocator);
            Thread.Sleep(1500);
            Click(IrAlimentacionBtnLocator);
            Thread.Sleep(1500);
        }


        public void CrearAlimento(string nombre, string descripcion, string imagen, string precio)
        {
            Click(CrearNuevoAlimentoLocator);
            Thread.Sleep(1500);
            Type(nombre, NombreInputLocator);
            Type(descripcion, DescripcionInputLocator);
            Type(imagen, ImagenInputLocator);
            Clear(PrecioInputLocator);
            Type(precio, PrecioInputLocator);
            Click(CreartBtnLocator);
            Thread.Sleep(1500);
        }

        public bool AlimentoCreado(string nombre)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            return IsDisplayed(NombreRegistroLocator) && 
                GetText(NombreRegistroLocator) == nombre;
        }

        public void ActualizarPrecioAlimento(string nombre, string nuevoPrecio)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            Click(EditarBtnLocator);
            Thread.Sleep(1500);
            Clear(PrecioInputLocator);
            Type(nuevoPrecio, PrecioInputLocator);
            Click(GuardarCambiosBtnLocator);
            Thread.Sleep(1500);
        }

        public bool PrecioActualizado(string nombre, string precio)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            return GetText(NombreRegistroLocator) == nombre &&
                GetText(PrecioRegistroLocator) == precio;
        }


        public void AgregarIngrediente(string nombre, string ingrediente)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            Click(IrAlimentosIngredientesBtnLocator);
            Thread.Sleep(1500);
            Click(AgregarNuevoIngredienteBtnLocator);
            Thread.Sleep(1500);
            SelectByText(ingrediente, IngredienteSelectLocator);
            Click(IngredienteCrearBtnLocator);
            Thread.Sleep(1500);
        }

        public bool IngredienteAgregado(string ingrediente)
        {
            Type(ingrediente, BuscarInputLocator);
            Thread.Sleep(1500);
            return IsDisplayed(IngredienteNombreRegistroLocator) && 
                GetText(IngredienteNombreRegistroLocator) == ingrediente;
        }


        public void EliminarIngrediente(string nombre, string ingrediente)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);

            Click(IrAlimentosIngredientesBtnLocator);
            Thread.Sleep(1500);

            Type(ingrediente, BuscarInputLocator);
            Thread.Sleep(1500);

            Click(EliminarIngredienteBtnLocator);
            Thread.Sleep(1500);

            Click(EliminarIngredienteConfirmacionBtnLocator);
            Thread.Sleep(1500);
        }


        public bool IngredienteEliminado()
        {
            return IsDisplayed(NingunRegistroLocator);
        }



        public void EliminarAlimento(string alimento)
        {
            Type(alimento, BuscarInputLocator);
            Thread.Sleep(1500);
            Click(EliminarBtnLocator);
            Thread.Sleep(1500);
            Click(EliminarConfirmacionBtnLocator);
        }


        public bool AlimentoEliminado()
        {
            return IsDisplayed(NingunRegistroLocator);
        }

    }
}
