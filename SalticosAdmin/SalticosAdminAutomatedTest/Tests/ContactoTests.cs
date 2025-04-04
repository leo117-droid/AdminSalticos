﻿using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class ContactoTests
    {
        private IWebDriver driver;

        ContactoPage contactoPage;

        [SetUp]
        public void SetUp()
        {
            contactoPage = new ContactoPage(driver);
            driver = contactoPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            contactoPage.Visit("https://localhost:7033/");

            contactoPage.IniciarSesion("camiulatech@gmail.com", "Hola321!");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        [Test, Order(1)]
        public void CrearContactoConDatosValidos_EsRegistrado()
        {
            contactoPage.GestionContactos();
            Thread.Sleep(2000);
            contactoPage.CrearContacto("Yowi", "Garcia", "Mantenimiento", "Belen", "89996655", "yowi@gmail.com");
            Thread.Sleep(2000);
            Assert.IsTrue(contactoPage.ContactoEstaRegistrado("yowi@gmail.com"));
        }

        [Test, Order(2)]
        public void ActualizarNombreContacto_EsActualizado()
        {
            contactoPage.GestionContactos();
            Thread.Sleep(2000);
            contactoPage.ActualizarNombreContacto("Yosward", "yowi@gmail.com");
            Thread.Sleep(2000);
            Assert.IsTrue(contactoPage.NombreContactoActualizado("Yosward", "yowi@gmail.com"));
        }
        [Test, Order(3)]

        public void EliminarContacto_EsEliminado()
        {
            contactoPage.GestionContactos();
            Thread.Sleep(2000);
            contactoPage.EliminarContacto("yowi@gmail.com");
            Thread.Sleep(2000);
            Assert.IsTrue(contactoPage.EstaContactoEliminado());
        }


    }
}
