using OpenQA.Selenium;
using SalticosAdminAutomatedTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Tests
{
    public class VehiculoTests
    {
        private IWebDriver driver;

        VehiculoPage vehiculoPage;

        [SetUp]
        public void SetUp()
        {
            vehiculoPage = new VehiculoPage(driver);
            driver = vehiculoPage.ChromeDriverConnection();
            driver.Manage().Window.Maximize();
            vehiculoPage.Visit("http: //localhost:5232/");

            //vehiculoPage.EnsureLoggedIn("admin@gmail.com", "Admin@123");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }


    }
}
