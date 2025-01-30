using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;

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

        public void ScrollToElement(By elementLocator)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var element = driver.FindElement(elementLocator);
            js.ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        public void ClickElementUsingJS(By elementLocator)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var element = driver.FindElement(elementLocator);
            js.ExecuteScript("arguments[0].click();", element);
        }


        public void TypeUsingJS(string inputText, By elementLocator)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var element = driver.FindElement(elementLocator);

            js.ExecuteScript("arguments[0].value = arguments[1];", element, inputText);

            
            js.ExecuteScript(@"
                var event = new Event('input', { bubbles: true });
                arguments[0].dispatchEvent(event);
            ", element);
        }


        public void SelectByVisibleTextUsingJS(string visibleText, By elementLocator)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var dropdown = driver.FindElement(elementLocator);

            js.ExecuteScript(@"
                Array.from(arguments[0].options).forEach(option => {
                    if (option.text === arguments[1]) {
                        option.selected = true;
                    }
                });
                var event = new Event('change', { bubbles: true });
                arguments[0].dispatchEvent(event);
            ", dropdown, visibleText);
        }

        public void SetDateUsingJS(string date, By elementLocator)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var dateInput = driver.FindElement(elementLocator);

            js.ExecuteScript(@"
        arguments[0].value = arguments[1];
        var event = new Event('input', { bubbles: true });
        arguments[0].dispatchEvent(event);
    ", dateInput, date);
        }


        public void SelectByText(String text, By locator)
        {
            IWebElement element = driver.FindElement(locator);
            SelectElement selectElement = new SelectElement(element);
            selectElement.SelectByText(text);
        }

    }
}
