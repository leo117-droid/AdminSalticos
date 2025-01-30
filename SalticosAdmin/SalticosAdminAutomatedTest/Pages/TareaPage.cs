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
    public class TareaPage : Base
    {
        By BtnTareaPagInicioLocator = By.XPath("/html/body/header/nav/div/div/ul[1]/li[3]/a");
        By BtnCrearNuevaTarea = By.XPath("/html/body/div/main/div/div[1]/div[2]/a");
        By TituloTarea = By.XPath("//*[@id=\"tarea-5\"]/div/div/h5");
        By CuadroTareaDisplay = By.XPath("//*[@id=\"tarea-8\"]/div/div");
        By BtnMarcarTareaCompletada = By.XPath("//*[@id=\"tarea-8\"]/div/div/div/button");
        By BtnEditarTarea = By.XPath("//*[@id=\"tarea-8\"]/div/div/div/a");
        By BtnEliminarTarea = By.XPath("/html/body/div/main/div/div[2]/div[1]/div/div/button");

        By BtnMensajeEliminarTarea = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");

        By TituloListaTareas = By.XPath("/html/body/div[1]/main/div/div[1]/div[1]/h2");


        //Botones Upsert
        By EspacioTituloTarea = By.Id("titulo");
        By EspacioDescripcionTarea = By.Id("Descripcion");
        By DropdownPrioridadTarea = By.Id("prioridad");
        By DropdownEstadoTarea = By.Id("estado");
        By SeleccionFechaTarea = By.Id("Fecha");
        By SeleccionHoraTarea = By.Id("Hora");

        By BtnCrearTarea = By.XPath("//*[@id=\"formTareas\"]/div/div[9]/div/button");

        By BtnGuardarCambiosTarea = By.XPath("//*[@id=\"formTareas\"]/div/div[9]/div/button");
        
        public TareaPage(IWebDriver driver) : base(driver)
        {

        }

        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }

        public void GestionTareas()
        {
            ClickElementUsingJS(BtnTareaPagInicioLocator);
            Thread.Sleep(2000);
        }

        public void CrearTarea(String titulo, String descripcion, String prioridad, String estado, string fecha, string hora)
        {
            Click(BtnCrearNuevaTarea);
            Thread.Sleep(2000);
            Type(titulo, EspacioTituloTarea);
            Type(descripcion, EspacioDescripcionTarea);
            SelectByVisibleTextUsingJS(prioridad, DropdownPrioridadTarea);
            SelectByVisibleTextUsingJS(estado, DropdownEstadoTarea);
            SetDateUsingJS(fecha, SeleccionFechaTarea);
            Type(hora, SeleccionHoraTarea);
            Click(BtnCrearTarea);
        }
        public void ActualizarDescripcionTarea(String descripcion)
        {

            Click(BtnEditarTarea);
            Thread.Sleep(2000);
            Clear(EspacioDescripcionTarea);
            Type(descripcion, EspacioDescripcionTarea);
            Click(BtnGuardarCambiosTarea);

        }

        public void EliminarTarea()
        {
            Click(BtnEliminarTarea);
            Thread.Sleep(2000);
            Click(BtnMensajeEliminarTarea);

        }
        /*public Boolean EstadoActualizado(String estado, String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            return GetText(EspacioDuracionCapacitacion) == duracion;

        }*/
        public Boolean TareaEstaRegistrada()
        {
            return IsDisplayed(TituloListaTareas);

        }
    }
}
