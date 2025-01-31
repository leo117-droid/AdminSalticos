using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SalticosAdminAutomatedTest.Pages
{
    public class CapacitacionesPage : Base
    {
        By BtnIrPersonal = By.XPath("/html/body/div/main/section/div/div[1]/div[2]/div/div/a");
        By BtnIrCapacitaciones = By.XPath("/html/body/div/main/section/div/div/div[3]/div/div/a");

        By BtnCrearNuevaCapacitacion = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By BtnEditarCapacitacion = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[4]/div/a[1]");
        By BtnEliminarCapacitacion = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[4]/div/a[3]");
        By BtnParticipantesCapacitacion = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[4]/div/a[2]");
        By InputBarraBusqueda = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By MensajeNingunRegistroCapacitacion = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");
        By BtnCrearCapacitacion = By.XPath("//*[@id=\"formCapacitacion\"]/div/div[6]/div/button");
        By BtnGuardarCambios = By.XPath("//*[@id=\"formCapacitacion\"]/div/div[6]/div/button");

        
        By AgregarNuevoPersonalCapacitacion = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By DropdownPersonalCapacitacion = By.Id("IdPersonal");
        By BtnAgregarPersonalACapacitacion = By.XPath("//*[@id=\"formCapacitacionPersonal\"]/div/div[3]/button");
        By EspacioCedulaPersonalCapacitacion = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[3]");
        By BtnConfirmarEliminacionCapacitacion = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By MensajeDatosRequeridos = By.XPath("//*[@id=\"formCapacitacion\"]/div/div[4]/div/span");

        By SelectFechaCapacitacion = By.Id("Fecha");
        By lblTemaCapacitacion = By.Id("Tema");
        By lblDuracionCapacitacion = By.Id("Duracion");

        By EspacioNombreCapacitacion = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[2]");
        By EspacioDuracionCapacitacion = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[3]");

        By PersonalDeCapacitacionBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[4]/div/a[2]");
        By AgregarNuevoPersonalBtnLocator = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By DropdownPersonalLocator = By.Id("IdPersonal");
        By CrearBtnLocator = By.XPath("//*[@id=\"formCapacitacionPersonal\"]/div/div[3]/button");
        By NingunRegistroMsgLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");
        By EliminarPersonalCapacitacionBtnLocator = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[4]/div/a/i");
        By EliminarPersonalCapacitacionConfirmacionBtnLocator = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");

        public CapacitacionesPage(IWebDriver driver) : base(driver)
        {

        }

        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }

        public Boolean CapacitacionEstaRegistrada(String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            return IsDisplayed(EspacioNombreCapacitacion) && GetText(EspacioNombreCapacitacion) == nombre;

        }

        public void GestionCapacitaciones()
        {
            ClickElementUsingJS(BtnIrPersonal);
            Thread.Sleep(2000);
            ClickElementUsingJS(BtnIrCapacitaciones);
        }

        public void CrearCapacitacion(String Fecha, String tema, String duracion)
        {
            Click(BtnCrearNuevaCapacitacion);
            Thread.Sleep(2000);
            SetDateUsingJS(Fecha, SelectFechaCapacitacion);
            Type(tema, lblTemaCapacitacion);
            Type(duracion, lblDuracionCapacitacion);
            Click(BtnCrearCapacitacion);
        }

        public void ActualizarDuracion(String nombre, String duracion)
        {
            Type(nombre, InputBarraBusqueda);

            Click(BtnEditarCapacitacion);
            Thread.Sleep(2000);
            SetDateUsingJS(duracion, lblDuracionCapacitacion);
            Click(BtnGuardarCambios);

        }
        public void EliminarCapacitacion(String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnEliminarCapacitacion);
            Thread.Sleep(2000);
            Click(BtnConfirmarEliminacionCapacitacion);
            Thread.Sleep(2000);

        }

        public Boolean DuracionActualizada(String duracion, String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            return GetText(EspacioDuracionCapacitacion) == duracion;

        }
        public bool CapacitacionEliminada()
        {
            return IsDisplayed(MensajeNingunRegistroCapacitacion);
        }

        public void CrearCapacitacionSinDatos()
        {
            Click(BtnCrearNuevaCapacitacion);
            Thread.Sleep(2000);
            Click(BtnCrearCapacitacion);
        }

        public Boolean MensajeErrorNoDatos()
        {
            return IsDisplayed(MensajeDatosRequeridos);
        }

        public Boolean EstaCapacitacionEliminada()
        {
            return IsDisplayed(MensajeNingunRegistroCapacitacion);
        }

        public void AgregarPersonalACapacitacion(String nombreCapacitacion)
        {
            Type(nombreCapacitacion, InputBarraBusqueda);
            Thread.Sleep(1500);
            Click(PersonalDeCapacitacionBtnLocator);
            Thread.Sleep(1500);
            Click(AgregarNuevoPersonalBtnLocator);
            Thread.Sleep(1500);
            SeleccionarPrimeraOpcionEnSelect(DropdownPersonalLocator);
            Click(CrearBtnLocator);
        }

        public bool PersonalEsAsignadoACapacitacion()
        {
            return IsDisplayed(MensajeNingunRegistroCapacitacion) &&
                GetText(MensajeNingunRegistroCapacitacion) == "Ningún registro"; ;
        }

        public void EliminarPersonalDeCapacitacion(String nombreCapacitacion)
        {
            Type(nombreCapacitacion, InputBarraBusqueda);
            Thread.Sleep(1500);
            Click(PersonalDeCapacitacionBtnLocator);
            Thread.Sleep(1500);
            Click(EliminarPersonalCapacitacionBtnLocator);
            Thread.Sleep(1500);
            Click(EliminarPersonalCapacitacionConfirmacionBtnLocator);

        }

        public bool PersonalEliminadoCapacitacion()
        {
            return IsDisplayed(NingunRegistroMsgLocator);
        }

        

    }
}

