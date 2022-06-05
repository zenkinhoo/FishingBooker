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
using Hooking.Models;

namespace IntegrationTests
{
    class IntegrationTest7
    {
        private IWebDriver _webDriver;
        private WebDriverWait _wait;
        private int _timeoutInSeconds = 30;
        [SetUp]
        public void SetUp()
        {
            _webDriver = new ChromeDriver("C:\\chromedriver_win32\\");
            _wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(_timeoutInSeconds));
        }
        [Test]
        public void  Test()
        {
            LoginDTO loginDTO = new LoginDTO("zenky1337@gmail.com", "Lazar1231.");

            var results = LogInUser(_webDriver, _wait, loginDTO);
            Assert.IsTrue(results);
        }

        private bool LogInUser(IWebDriver webDriver, WebDriverWait wait, LoginDTO loginDTO)
        {
            bool result = false;
            try
            {
                webDriver.Navigate().GoToUrl("https://localhost:44306/Identity/Account/Login");
                webDriver.Manage().Window.Maximize();
                IWebElement webElement;
                // Username
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("email_input")));
                webElement = webDriver.FindElement(By.Id("email_input"));
                webElement.Clear();
                webDriver.FindElement(By.Id("email_input")).SendKeys(loginDTO.email);

                // password
                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("password_input")));
                webElement = webDriver.FindElement(By.Id("password_input"));
                webElement.Clear();
                webDriver.FindElement(By.Id("password_input")).SendKeys(loginDTO.password);

                webDriver.FindElement(By.Id("submit_login")).Click();


                wait.Until(ExpectedConditions.ElementExists(By.Id("home")));
                webElement = webDriver.FindElement(By.Id("home"));
                webElement.Click();

                wait.Until(ExpectedConditions.ElementExists(By.Id("cottages")));
                webElement = webDriver.FindElement(By.Id("cottages"));
                webElement.Click();

                wait.Until(ExpectedConditions.ElementExists(By.Id("boats")));
                webElement = webDriver.FindElement(By.Id("boats"));
                webElement.Click();

                wait.Until(ExpectedConditions.ElementExists(By.Id("instructors")));
                webElement = webDriver.FindElement(By.Id("instructors"));
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
    }
}
