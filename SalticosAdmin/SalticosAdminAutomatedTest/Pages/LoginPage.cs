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
        By MensajeErrorInicioSesion = By.XPath("//*[@id=\"account\"]/div[1]/ul/li");
        By BannerPagPrincipal = By.XPath("/html/body/div/main/div/img");


        public LoginPage(IWebDriver driver) : base(driver)
        {

        }

        public void Login(string email, string password)
        { 

            Type(email, EmailLocator);

            Type(password, PasswordLocator);

            Click(IniciarSesionBtnLocator);
        }


        public Boolean MensajeErrorInicioSesionDesplegado()
        {
            return IsDisplayed(MensajeErrorInicioSesion);
        }

        public Boolean InicioDeSesionCorrecto()
        {
            return IsDisplayed(BannerPagPrincipal);
        }
    }
}
