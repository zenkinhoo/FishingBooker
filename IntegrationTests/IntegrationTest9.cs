using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Hooking;
using Hooking.Models.DTO;
using System.Web.Http;
using SeleniumExtras.WaitHelpers;

namespace IntegrationTests
{
    class IntegrationTest9
    {
        private IWebDriver _webDriver;
        private WebDriverWait _wait;
        private int _timeoutInSeconds = 30;
        public LoginDTO loginDTO;
        [SetUp]
        public void SetUp()
        {
            _webDriver = new ChromeDriver("C:\\chromedriver_win32\\");
            _wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(_timeoutInSeconds));
        }
        #region Integration Test
        public bool SendUserDeleteRequest(IWebDriver webDriver, WebDriverWait wait, LoginDTO loginDTO)
        {
            bool result = false;
            try
            {
                webDriver.Navigate().GoToUrl("https://localhost:44306/Identity/Account/Login");
                webDriver.Manage().Window.Maximize();
                IWebElement webElement;
                
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email_input")));
                webElement = webDriver.FindElement(By.Id("email_input"));
                webElement.Clear();
                webDriver.FindElement(By.Id("email_input")).SendKeys(loginDTO.email);

                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("password_input")));
                webElement = webDriver.FindElement(By.Id("password_input"));
                webElement.Clear();
                webDriver.FindElement(By.Id("password_input")).SendKeys(loginDTO.password);

                webDriver.FindElement(By.Id("submit_login")).Click();

                webDriver.Navigate().GoToUrl("https://localhost:44306/Identity/Account/Manage");

                wait.Until(ExpectedConditions.ElementExists(By.Id("delete-request")));
                webElement = webDriver.FindElement(By.Id("delete-request"));
                webElement.Click();

                wait.Until(ExpectedConditions.ElementExists(By.Id("input")));
                webElement = webDriver.FindElement(By.Id("input"));
                webElement.SendKeys("integracioni test zahtev");
                webElement.Click();

                wait.Until(ExpectedConditions.ElementExists(By.Id("submit")));
                webElement = webDriver.FindElement(By.Id("submit"));
                webElement.Click();



                if (webElement == null)
                {
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Source + " - " + ex.Message + " - " + ex.StackTrace);
                return result;
            }
        }
        #endregion
        [Test]
        public void Test()
        {
            LoginDTO loginDTO = new LoginDTO("zenky1337@gmail.com", "Lazar1231.");
            var results = SendUserDeleteRequest(_webDriver, _wait, loginDTO);
            Assert.IsTrue(results);
        }
    }
}
