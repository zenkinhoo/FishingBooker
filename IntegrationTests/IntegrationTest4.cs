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
    class IntegrationTest4
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
        [Test]
        public async Task Test()
        {
            LoginDTO loginDTO = new LoginDTO("randomemail@gmail.com", "Asdf123.");
            var results = LogInInstructor(_webDriver, _wait, loginDTO);
            Assert.IsTrue(results);
        }

        private bool LogInInstructor(IWebDriver webDriver, WebDriverWait wait, LoginDTO loginDTO)
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

            wait.Until(ExpectedConditions.ElementExists(By.Id("reservation-history")));
            webElement = webDriver.FindElement(By.Id("reservation-history"));
            webElement.Click();

            wait.Until(ExpectedConditions.ElementExists(By.Id("reservations")));
            webElement = webDriver.FindElement(By.Id("reservations"));
            webElement.Click();

            wait.Until(ExpectedConditions.ElementExists(By.Id("adventures")));
            webElement = webDriver.FindElement(By.Id("adventures"));
            webElement.Click();

            wait.Until(ExpectedConditions.ElementExists(By.Id("calendar")));
            webElement = webDriver.FindElement(By.Id("calendar"));
            webElement.Click();

            wait.Until(ExpectedConditions.ElementExists(By.Id("special-offers")));
            webElement = webDriver.FindElement(By.Id("special-offers"));
            if (webElement == null)
            {
                return false;
            }
            return true;
        }
    }
}
