using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class InflablePage : Base
    {

        By IrServiciosBtnLocator = By.XPath("/html/body/div/main/section/div/div[1]/div[3]/div/div/a");
        By IrInflableIntermedioBtnLocator = By.XPath("/html/body/div/main/section/div/div[1]/div[1]/div/div/a");
        By IrInflableBtnLocator = By.XPath("/html/body/div/main/section/div/div/div[1]/div/div/a");

        By CrearNuevoInflableLocator = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By NombreInputLocator = By.Id("nombre");
        By DescripcionInputLocator = By.Id("Inflable_Descripcion");
        By DimensionesInputLocator = By.Id("Inflable_Dimensiones");
        By ImagenInputLocator = By.Id("imagenId");
        By PrecioInputLocator = By.Id("Inflable_Precio");
        By PrecioHoraAdicionalInputLocator = By.Id("Inflable_PrecioHoraAdicional");
        By CategoriaTamannoSelectLocator = By.XPath("//*[@id=\"Inflable_CategoriaTamannoId\"]");
        By CategoriaEdadSelectLocator = By.XPath("//*[@id=\"Inflable_CategoriaEdadId\"]");
        By CrearBtnLocator = By.XPath("//*[@id=\"formInflable\"]/div/div[2]/div[1]/div/div[10]/div/button");

        By BuscarInputLocator = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By NombreRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[1]");

        By DescripcionRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[2]");
        By EditarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[9]/div/a[1]");
        By GuardarCambiosBtnLocator = By.XPath("//*[@id=\"formInflable\"]/div/div[2]/div[1]/div/div[10]/div/button");

        By PrecioNegativoMsgLocator = By.XPath("//*[@id=\"Inflable_Precio-error\"]");

        By EliminarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[9]/div/a[2]/i");
        By EliminarConfirmacionBtnLocator = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By NingunRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");

        By TamannoFiltroSelectLocator = By.XPath("//*[@id=\"categoriaTamannoSelect\"]");
        By EdadFiltroSelectLocator = By.XPath("//*[@id=\"categoriaEdadSelect\"]");
        By BuscarBtnLocator = By.XPath("//*[@id=\"formFiltrar\"]/div/div[3]/button");

        By RegistrosLocator = By.TagName("tr");



        public InflablePage(IWebDriver driver) : base(driver)
        {


        }


        public void IniciarSesion(string usuario, string contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }


        public void GestionInflable()
        {
            ClickElementUsingJS(IrServiciosBtnLocator);
            Thread.Sleep(1500);
            ClickElementUsingJS(IrInflableIntermedioBtnLocator);
            Thread.Sleep(1500);
            ClickElementUsingJS(IrInflableBtnLocator);
            Thread.Sleep(1500);
        }


        public void CrearInflable(string nombre, string descripcion, string dimensiones, string imagen, 
            string precio, string precioAdicional, string categoriaTamanno, string categoriaEdad)
        {
            Click(CrearNuevoInflableLocator);
            Thread.Sleep(1500);
            Type(nombre, NombreInputLocator);
            Type(descripcion, DescripcionInputLocator);
            Type(dimensiones, DimensionesInputLocator);
            Type(imagen, ImagenInputLocator);
            Clear(PrecioInputLocator);
            Type(precio, PrecioInputLocator);
            TypeUsingJS(precioAdicional, PrecioHoraAdicionalInputLocator);
            SelectByVisibleTextUsingJS(categoriaTamanno, CategoriaTamannoSelectLocator);
            SelectByVisibleTextUsingJS(categoriaEdad, CategoriaEdadSelectLocator);
            ClickElementUsingJS(CrearBtnLocator);
        }

        public bool InflableEstaRegistrado(string nombre)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            return IsDisplayed(NombreRegistroLocator) && 
                GetText(NombreRegistroLocator) == nombre;
        }

        public void ActualizarPrecioNegativo(string nombre, string precioNegativo)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            Click(EditarBtnLocator);
            Thread.Sleep(1500);
            Clear(PrecioInputLocator);
            Type(precioNegativo + "\n", PrecioInputLocator);
        }


        public bool PrecioNegativoMsgDesplegado()
        {
            return IsDisplayed(PrecioNegativoMsgLocator);
        }


        public void ActualizarDescripcionInflable(string nombre, string nuevaDescripcion)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            Click(EditarBtnLocator);
            Thread.Sleep(1500);
            Clear(DescripcionInputLocator);
            Type(nuevaDescripcion, DescripcionInputLocator);
            ClickElementUsingJS(GuardarCambiosBtnLocator);
        }

        public bool DescripcionInflableActualizada(string nombre, string nuevaDescripcion)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            return IsDisplayed(DescripcionRegistroLocator) &&
                GetText(DescripcionRegistroLocator) == nuevaDescripcion;
        }


        public void EliminarInflable(string nombre)
        {
            Type(nombre, BuscarInputLocator);
            Thread.Sleep(1500);
            Click(EliminarBtnLocator);
            Thread.Sleep(1500);
            Click(EliminarConfirmacionBtnLocator);
        }


        public bool InflableEliminado()
        {
            return IsDisplayed(NingunRegistroLocator);
        }

        public void FiltrarInflable(string categoriaTamanno, string categoriaEdad)
        {
            SelectByVisibleTextUsingJS(categoriaTamanno, TamannoFiltroSelectLocator);
            SelectByVisibleTextUsingJS(categoriaEdad, EdadFiltroSelectLocator);
            ClickElementUsingJS(BuscarBtnLocator);
            Thread.Sleep(1500);
        }   


        public bool FiltroAplicado(string categoriaTamanno, string categoriaEdad    )
        {
            List<IWebElement> registros = FindElements(RegistrosLocator);
            for (int i = 1; i < registros.Count; i++)
            {
                List<IWebElement> columnas = registros[i].FindElements(By.TagName("td")).ToList();

                if (columnas.Count > 0)
                {
                    string tamanno = columnas[6].Text;
                    string edad = columnas[7].Text;
                    if (tamanno != categoriaTamanno || edad != categoriaEdad)
                    {
                        return false;
                    }
                }

            }

            return true;
        }

    }
}
