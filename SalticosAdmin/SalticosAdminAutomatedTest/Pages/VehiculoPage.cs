using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class VehiculoPage : Base
    {

        By IrBtnLocator = By.XPath("/html/body/div/main/section/div/div[2]/div[1]/div/div/a");
        By IrVehiculosBtnLocator = By.XPath("/html/body/div/main/section/div/div/div[1]/div/div/a");
        By CrearNuevoVehiculoBtnLocator = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By MarcaLocator = By.Id("marca");
        By ModeloLocator = By.Id("Modelo");
        By PlacaLocator = By.Id("placa");
        By TipoVehiculoLocator = By.Id("TipoVehiculo");
        By CrearBtnLocator = By.XPath("//*[@id=\"formVehiculo\"]/div/div[7]/button");

        By BuscarInputLocator = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By EliminarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[5]/div/a[2]");
        By MensajeEliminarLocator = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By NingunRegistroMsgLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");

        By EditarBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[5]/div/a[1]/i");
        By GuardarCambiosBtnLocator = By.XPath("//*[@id=\"formVehiculo\"]/div/div[7]/button");

        By ModeloRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[2]");
        By PlacaRegistroLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[3]");

        By ErrorPlacaMsgLocator = By.XPath("/html/body/div[2]/div/div[3]");
        public VehiculoPage(IWebDriver driver) : base(driver)
        {
        }

        //public void EnsureLoggedIn(string username, string password)
        //{
        //    LoginPage loginPage = new LoginPage(driver);
        //    loginPage.Login(username, password);
        //}

        public void GestionVehiculos()
        {
            Click(IrBtnLocator);
            Click(IrVehiculosBtnLocator);
        }

        public void AgregarVehiculo(String marca, String modelo, String placa, String tipoVehiculo)
        {
            Click(CrearNuevoVehiculoBtnLocator); 
            Thread.Sleep(2000);
            Type(marca, MarcaLocator);
            Type(modelo, ModeloLocator);
            Type(placa, PlacaLocator);
            Type(tipoVehiculo, TipoVehiculoLocator);
            Click(CrearBtnLocator);
        }

        public Boolean VehiculoEstaRegistrado(String placa)
        {
            Type(placa, BuscarInputLocator);
            return IsDisplayed(PlacaRegistroLocator) && GetText(PlacaRegistroLocator) == placa;
        }

        public void ActualizarModeloVehiculo(String modelo, String placa)
        {
            Type(placa, BuscarInputLocator);
            
            Click(EditarBtnLocator);
            Thread.Sleep(2000);
            Clear(ModeloLocator);
            Type(modelo, ModeloLocator);
            Click(GuardarCambiosBtnLocator);

        }

        public Boolean ModeloVehiculoActualizado(String modelo, String placa)
        {
            Type(placa, BuscarInputLocator);
            return GetText(ModeloRegistroLocator) == modelo;
        }

        public void EliminarVehiculo(String placa)
        {
            Type(placa, BuscarInputLocator);
            Click(EliminarBtnLocator);
            Click(MensajeEliminarLocator);
            Thread.Sleep(2000);

        }
        public void RegistrarVehiculoConPlacaRepetida(String marca, String modelo, String placa, String tipoVehiculo)
        {
            Click(CrearNuevoVehiculoBtnLocator);
            Thread.Sleep(2000);
            Type(marca, MarcaLocator);
            Type(modelo, ModeloLocator);
            Type(placa, PlacaLocator);
        }

        public Boolean MensajeErrorPlacaRepetidaDesplegado()
        {
            return IsDisplayed(ErrorPlacaMsgLocator);
        }

        public Boolean EstaVehiculoEliminado()
        {
            return IsDisplayed(NingunRegistroMsgLocator);
        }
    }
}
