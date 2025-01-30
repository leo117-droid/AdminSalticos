using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class EventoPage : Base
    {
        By BtnIrEvento = By.XPath("/html/body/div/main/section/div/div[1]/div[1]/div/div/a");
        By BtnIrEventos = By.XPath("/html/body/div/main/section/div/div/div[1]/div/div/a");
        By BtnCrearNuevoEvento = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By BtnEditarEvento = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[7]/div/a[1]");
        By BtnEliminarEvento = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[7]/div/a[2]");
        By BtnAccionesEvento = By.XPath("//*[@id=\"dropdownMenuButton\"]");
        By BtnOpcionAlimentacionAcciones = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[7]/div/div/ul/li[1]/a");
        By BtnOpcionVehiculoAcciones = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[7]/div/div/ul/li[2]/a");
        By BtnOpcionMobiliarioAcciones = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[7]/div/div/ul/li[3]/a");
        By BtnOpcionPersonalAcciones = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[7]/div/div/ul/li[4]/a");
        By BtnOpcionInflableAcciones = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[7]/div/div/ul/li[5]/a");
        By BtnOpcionServiciosAdicionalesAcciones = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[7]/div/div/ul/li[6]/a");
        By InputBarraBusqueda = By.XPath("//*[@id=\"tblDatos_filter\"]/label/input");
        By EspacioNombreClienteTablaEvento = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[6]");

        public EventoPage(IWebDriver driver) : base(driver)
        {


        }
    }
}
