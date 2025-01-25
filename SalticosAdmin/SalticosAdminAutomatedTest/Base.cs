using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalticosAdminAutomatedTest
{
    public class Base
    {

        protected IWebDriver driver;

        public Base(IWebDriver driver)
        {
            this.driver = driver;
        }

        public IWebDriver ChromeDriverConnection()
        {
            driver = new ChromeDriver();
            return driver;
        }

        public IWebElement FindElement(By locator)
        {
            return driver.FindElement(locator);
        }


        public List<IWebElement> FindElements(By locator)
        {
            return driver.FindElements(locator).ToList();
        }

        public IWebElement FindElementOnList(By locator, int pos)
        {
            return driver.FindElements(locator).Take(pos).Last();
        }

        public String GetText(IWebElement element)
        {
            return element.Text;
        }

        public String GetText(By locator)
        {
            return driver.FindElement(locator).Text;
        }

        public void Type(String inputText, By locator)
        {
            driver.FindElement(locator).SendKeys(inputText);
        }

        public void Click(By locator)
        {
            driver.FindElement(locator).Click();
        }

        public void Click(IWebElement element)
        {
            element.Click();
        }

        public Boolean IsDisplayed(By locator)
        {
            try
            {
                return driver.FindElement(locator).Displayed;
            }
            catch (NoSuchElementException e)
            {
                return false;
            }

        }

        public void Visit(String url)
        {
            driver.Navigate().GoToUrl(url);
        }

        public String GetUrl()
        {
            return driver.Url;
        }

        public void Clear(By locator)
        {
            driver.FindElement(locator).Clear();
        }

    }
}
