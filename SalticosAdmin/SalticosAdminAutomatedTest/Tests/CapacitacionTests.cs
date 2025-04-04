﻿using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class CapacitacionTests
    {
        private IWebDriver driver;
        CapacitacionesPage capacitacionesPage;

        [SetUp]
        public void SetUp()
        {
            capacitacionesPage = new CapacitacionesPage(driver);
            driver = capacitacionesPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            capacitacionesPage.Visit("https://localhost:7033/");

            capacitacionesPage.IniciarSesion("camiulatech@gmail.com", "Hola321!");
        }
        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        [Test, Order(1)]
        public void CrearCapacitacionConDatosValidos_EsRegistrado()
        {
            capacitacionesPage.GestionCapacitaciones();
            Thread.Sleep(2000);
            capacitacionesPage.CrearCapacitacion("2025-10-28", "Uso de equipos", "2 días");
            Thread.Sleep(2000);
            Assert.IsTrue(capacitacionesPage.CapacitacionEstaRegistrada("Uso de equipos"));
        }

        [Test, Order(2)]
        public void CrearCapacitacionDatosVacios_MensajeDeError()
        {
            capacitacionesPage.GestionCapacitaciones();
            Thread.Sleep(2000);
            capacitacionesPage.CrearCapacitacionSinDatos();
            Thread.Sleep(2000);
            Assert.IsTrue(capacitacionesPage.MensajeErrorNoDatos());
        }

        [Test, Order(3)]
        public void ActualizarDuracion()
        {
            capacitacionesPage.GestionCapacitaciones();
            Thread.Sleep(2000);
            capacitacionesPage.ActualizarDuracion("Uso de equipos", "4 días");
            Thread.Sleep(2000);
            Assert.IsTrue(capacitacionesPage.DuracionActualizada("4 días", "Uso de equipos"));
        }



        [Test, Order(4)]
        public void AsignarPersonalACapacitacion_EsAsignado()
        {
            capacitacionesPage.GestionCapacitaciones();
            Thread.Sleep(1500);
            capacitacionesPage.AgregarPersonalACapacitacion("Uso de equipos");
            Thread.Sleep(1500);
            Assert.IsFalse(capacitacionesPage.PersonalEsAsignadoACapacitacion());
        }


        [Test, Order(5)]
        public void EliminarPersonalDeCapacitacion_EsEliminado()
        {
            capacitacionesPage.GestionCapacitaciones();
            Thread.Sleep(1500);
            capacitacionesPage.EliminarPersonalDeCapacitacion("Uso de equipos");
            Thread.Sleep(1500);
            Assert.IsTrue(capacitacionesPage.PersonalEliminadoCapacitacion());
        }

        [Test, Order(6)]
        public void EliminarCapacitacion_EsEliminado()
        {
            capacitacionesPage.GestionCapacitaciones();
            Thread.Sleep(2000);
            capacitacionesPage.EliminarCapacitacion("Uso de equipos");
            Thread.Sleep(2000);
            Assert.IsTrue(capacitacionesPage.EstaCapacitacionEliminada());
        }
    }
}
