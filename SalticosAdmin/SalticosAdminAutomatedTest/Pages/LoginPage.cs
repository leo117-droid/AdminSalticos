using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest.Pages
{
    public class LoginPage : Base
    {

        By EmailLocator = By.Id("Input_Email");
        By PasswordLocator = By.Id("Input_Password");
        By IniciarSesionBtnLocator = By.XPath("//*[@id=\"account\"]/div[4]/button");


        public LoginPage(IWebDriver driver) : base(driver)
        {

        }

        public void Login(string email, string password)
        { 

            Type(email, EmailLocator);

            Type(password, PasswordLocator);

            Click(IniciarSesionBtnLocator);
        }


    }
}
