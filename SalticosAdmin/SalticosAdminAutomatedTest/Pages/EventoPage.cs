﻿using OpenQA.Selenium;
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
        By EspacioDireccionClienteTablaEvento = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[4]");
        By SeleccionFechaFormularioEvento = By.Id("Evento_Fecha");
        By IngresarHoraInicioFormularioEvento = By.Id("Evento_HoraInicio");
        By IngresarHoraFinalFormularioEvento = By.Id("Evento_HoraFinal");
        By lblDireccionFormularioEvento = By.Id("Evento_Direccion");
        By DropdownProvinciaEvento = By.Id("provincia");
        By DropdownClienteEvento = By.Id("Evento_ClienteId");
        By lblCorreoRecordatorioEvento = By.Id("Evento_Correo");
        By BtnCrearEvento = By.XPath("//*[@id=\"formEvento\"]/div/div[9]/div/button");
        By BtnGuardarCambios = By.XPath("//*[@id=\"formEvento\"]/div/div[9]/div/button");
        By BtnConfirmarEliminacion = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By EspacioFechaEventoTabla = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr[1]/td[1]");
        By BtnAgregarNuevoInflable = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By MensajeNingunRegistro = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");
        By OpcionesInflables = By.XPath("//*[@id=\"IdInflable\"]");
        By BtnCrearInflable = By.XPath("//*[@id=\"formEventoInflable\"]/div/div[3]/button");

        By EspacioNombreInflable = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[1]"); 
        By BtnEliminarInflable = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[3]/div/a");
        By BtnConfirmarEliminacionInflable = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[3]/div/a");
        By NingunRegistroMensaje = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td");
        By BtnCrearPersonal = By.XPath("//*[@id=\"formEventoPersonal\"]/div/div[3]/button");
        By OpcionesPersonal = By.XPath("//*[@id=\"IdPersonal\"]");
        By BtnAnadirPersonal = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By EspacioCedulaPersonal = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[3]");
        By BtnEliminarPersonal = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[4]/div/a");
        By BtnConfirmarEliminarPersonal = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By BtnAnadirVehiculo = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By OpcionesVehiculo = By.XPath("//*[@id=\"IdVehiculo\"]");
        By BtnCrearVehiculo = By.XPath("//*[@id=\"formEventoVehiculo\"]/div/div[3]/button");
        By BtnEliminarVehiculo = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[3]/div/a");
        By BtnConfirmarEliminarVehiculo = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By EspacioPlacaVehiculo = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[2]");
        By BtnAnadirMobiliario = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By OpcionesMobiliario = By.XPath("//*[@id=\"IdMobiliario\"]");
        By CantidadMobiliarioInput = By.XPath("//*[@id=\"Cantidad\"]");
        By BtnCrearMobiliario = By.XPath("//*[@id=\"formEventoMobiliario\"]/div/div[4]/button");
        By BtnEliminarMobiliario = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[3]/div/a[2]");
        By BtnConfirmarEliminarMobiliario = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By EspacioNombreMobiliario = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[1]");
        By BtnAnadirAlimentacion = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By OpcionesAlimentacion = By.XPath("//*[@id=\"IdAlimentacion\"]");
        By CantidadAlimentacionInput = By.XPath("//*[@id=\"Cantidad\"]");
        By BtnCrearAlimentacion = By.XPath("//*[@id=\"formEventoAlimentacion\"]/div/div[4]/button");
        By BtnEliminarAlimentacion = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[3]/div/a[2]");
        By BtnConfirmarEliminarAlimentacion = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");
        By EspacioNombreAlimentacion = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[1]");
        By BtnAnadirServicioAdicional = By.XPath("/html/body/div/main/div[1]/div[2]/a");
        By OpcionesServicioAdicional = By.XPath("//*[@id=\"IdServicioAdicional\"]");
        By CantidadServicioAdicionalInput = By.XPath("//*[@id=\"Cantidad\"]");
        By BtnCrearServicioAdicional = By.XPath("//*[@id=\"formEventoServicioAdicional\"]/div/div[4]/button");
        By BtnEliminarServicioAdicional = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[3]/div/a[2]");
        By EspacioNombreServicioAdicional = By.XPath("//*[@id=\"tblDatos\"]/tbody/tr/td[1]");
        By BtnConfirmarEliminarServicioAdicional = By.XPath("/html/body/div[2]/div/div[4]/div[2]/button");


        public EventoPage(IWebDriver driver) : base(driver)
        {

        }
        public void IniciarSesion(String usuario, String contrasenna)
        {
            LoginPage loginPage = new LoginPage(driver);
            loginPage.Login(usuario, contrasenna);

        }

        public void GestionEvento()
        {
            ClickElementUsingJS(BtnIrEvento);
            Thread.Sleep(1500);
            ClickElementUsingJS(BtnIrEventos);
            Thread.Sleep(1500);
        }

        public Boolean EventoEstaRegistrado(String cliente)
        {
            Type(cliente, InputBarraBusqueda);
            return IsDisplayed(EspacioNombreClienteTablaEvento) && GetText(EspacioNombreClienteTablaEvento) == cliente;

        }

        public void CrearEvento(String Fecha, String horaInicio, String horaFinal, String direccion, String provincia, String Cliente, String correoConfirmacion)
        {
            Click(BtnCrearNuevoEvento);
            Thread.Sleep(2000);
            Type(Fecha, SeleccionFechaFormularioEvento);
            Type(horaInicio, IngresarHoraInicioFormularioEvento);
            Type(horaFinal, IngresarHoraFinalFormularioEvento);
            Type(direccion, lblDireccionFormularioEvento);
            SelectByVisibleTextUsingJS(provincia, DropdownProvinciaEvento);
            SelectByVisibleTextUsingJS(Cliente, DropdownClienteEvento);
            Type(correoConfirmacion, lblCorreoRecordatorioEvento);
            Click(BtnCrearEvento);
        }
        public void ActualizarFechaEvento(String nombre, String Fecha)
        {
            Type(nombre, InputBarraBusqueda);

            Click(BtnEditarEvento);
            Thread.Sleep(2000);
            Clear(SeleccionFechaFormularioEvento);
            Type(Fecha, SeleccionFechaFormularioEvento);
            Thread.Sleep(3000);
            Click(BtnGuardarCambios);

        }

        public void EliminarEvento(String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnEliminarEvento);
            Thread.Sleep(2000);
            Click(BtnConfirmarEliminacion);
            Thread.Sleep(2000);

        }

        public void SeleccionarInflable(String nombreInflable, String nombre) 
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnAccionesEvento);
            Click(BtnOpcionInflableAcciones);
            Thread.Sleep(1500);
            Click(BtnAgregarNuevoInflable);
            SelectByVisibleTextUsingJS(nombreInflable, OpcionesInflables);
            Click(BtnCrearInflable);
            
        }
        public void SeleccionarPersonal(String nombrePersonal, String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnAccionesEvento);
            Click(BtnOpcionPersonalAcciones);
            Thread.Sleep(1500);
            Click(BtnAnadirPersonal);
            SelectByVisibleTextUsingJS(nombrePersonal, OpcionesPersonal);
            Click(BtnCrearPersonal);
        }
        public void SeleccionarVehiculo(String vehiculo, String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnAccionesEvento);
            Click(BtnOpcionVehiculoAcciones);
            Thread.Sleep(1500);
            Click(BtnAnadirVehiculo);
            SelectByVisibleTextUsingJS(vehiculo, OpcionesVehiculo);
            Click(BtnCrearVehiculo);

        }

        public void SeleccionarMobiliario(String mobiliario, String cantidad, String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnAccionesEvento);
            Click(BtnOpcionMobiliarioAcciones);
            Thread.Sleep(1500);
            Click(BtnAnadirMobiliario);
            SelectByVisibleTextUsingJS(mobiliario, OpcionesMobiliario);
            Clear(CantidadMobiliarioInput);
            Type(cantidad, CantidadMobiliarioInput);
            Click(BtnCrearMobiliario);

        }

        public void SeleccionarAlimentacion(String alimentacion, String cantidad, String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnAccionesEvento);
            Click(BtnOpcionAlimentacionAcciones);
            Thread.Sleep(1500);
            Click(BtnAnadirAlimentacion);
            SelectByVisibleTextUsingJS(alimentacion, OpcionesAlimentacion);
            Clear(CantidadAlimentacionInput);
            Type(cantidad, CantidadAlimentacionInput);
            Click(BtnCrearAlimentacion);

        }
        public void SeleccionarServicioAdicional(String servicio, String cantidad, String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnAccionesEvento);
            Click(BtnOpcionServiciosAdicionalesAcciones);
            Thread.Sleep(1500);
            Click(BtnAnadirServicioAdicional);
            SelectByVisibleTextUsingJS(servicio, OpcionesServicioAdicional);
            Clear(CantidadServicioAdicionalInput);
            Type(cantidad, CantidadServicioAdicionalInput);
            Click(BtnCrearServicioAdicional);

        }
        public void EliminarInflable(String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnAccionesEvento);
            Click(BtnOpcionInflableAcciones);
            Thread.Sleep(1500);
            Click(BtnEliminarInflable);
            Click(BtnConfirmarEliminacionInflable);
        }
        public void EliminarPersonal(String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnAccionesEvento);
            Click(BtnOpcionPersonalAcciones);
            Thread.Sleep(1500);
            Click(BtnEliminarPersonal);
            Click(BtnConfirmarEliminarPersonal);
        }

        public void EliminarVehiculo(String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnAccionesEvento);
            Click(BtnOpcionVehiculoAcciones);
            Thread.Sleep(1500);
            Click(BtnEliminarVehiculo);
            Click(BtnConfirmarEliminarVehiculo);
        }

        public void EliminarMobiliario(String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnAccionesEvento);
            Click(BtnOpcionMobiliarioAcciones);
            Thread.Sleep(1500);
            Click(BtnEliminarMobiliario);
            Click(BtnConfirmarEliminarMobiliario);
        }
        public void EliminarAlimentacion(String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnAccionesEvento);
            Click(BtnOpcionAlimentacionAcciones);
            Thread.Sleep(1500);
            Click(BtnEliminarAlimentacion);
            Click(BtnConfirmarEliminarAlimentacion);
        }
        public void EliminarServicioAdicional(String nombre)
        {
            Type(nombre, InputBarraBusqueda);
            Click(BtnAccionesEvento);
            Click(BtnOpcionServiciosAdicionalesAcciones);
            Thread.Sleep(1500);
            Click(BtnEliminarServicioAdicional);
            Click(BtnConfirmarEliminarServicioAdicional);
        }
        public Boolean FechaActualizada(String fecha, String cliente)
        {
            Type(cliente, InputBarraBusqueda);
            return GetText(EspacioFechaEventoTabla) == fecha;

        }

        public bool EventoEliminado()
        {
            return IsDisplayed(MensajeNingunRegistro);
        }

        public Boolean InflableActualizado(String inflable) 
        {
            return GetText(EspacioNombreInflable) == inflable;
        }

        public Boolean MobiliarioActualizado(String mobiliario)
        {
            return GetText(EspacioNombreMobiliario) == mobiliario;
        }
        public Boolean PersonalActualizado(String cedula) 
        {
            return GetText(EspacioCedulaPersonal) == cedula;
        }

        public Boolean VehiculoActualizado(String placa)
        {
            return GetText(EspacioPlacaVehiculo) == placa;
        }

        public Boolean AlimentacionActualizada(String alimentacion) 
        {
            return GetText(EspacioNombreAlimentacion) == alimentacion;
        }

        public Boolean ServicioActualizado(String servicio)
        {
            return GetText(EspacioPlacaVehiculo) == servicio;
        }
        public Boolean InflableEliminado() 
        {
            return IsDisplayed(NingunRegistroMensaje);
        }

        public Boolean MobiliarioEliminado()
        {
            return IsDisplayed(NingunRegistroMensaje);
        }
        public Boolean PersonalEliminado()
        {
            return IsDisplayed(NingunRegistroMensaje);
        }

        public Boolean VehiculoEliminado()
        {
            return IsDisplayed(NingunRegistroMensaje);
        }

        public Boolean AlimentacionEliminada()
        {
            return IsDisplayed(NingunRegistroMensaje);
        }

        public Boolean ServicioEliminado()
        {
            return IsDisplayed(NingunRegistroMensaje);
        }
    }
}
