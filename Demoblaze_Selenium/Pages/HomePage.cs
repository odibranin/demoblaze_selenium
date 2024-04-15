using OpenQA.Selenium;
using Demoblaze_Selenium.utils;
using OpenQA.Selenium.Support.UI;
using AventStack.ExtentReports;
using ICSharpCode.SharpZipLib.Core;

namespace Demoblaze_Selenium.Pages
{
    public class HomePage
    {
        private IWebDriver driver;
        private ExtentTest test;
        private IReadOnlyCollection<IWebElement> CATEGORIES => driver.FindElements(By.CssSelector(".list-group-item:not(#cat)"));
        private IReadOnlyCollection<IWebElement> PHONES => driver.FindElements(By.CssSelector(".card-title a"));
        private IReadOnlyCollection<IWebElement> LAPOTOPS => driver.FindElements(By.CssSelector(".card-block .card-title a"));
        private IReadOnlyCollection<IWebElement> MONITORS => driver.FindElements(By.CssSelector(".card-block .card-title a"));
        private IWebElement HOME_PAGE_HEADER => driver.FindElement(By.Id("nava"));
        private IWebElement SIGNUP_HEADER_BUTTON => driver.FindElement(By.Id("signin2"));
        private IWebElement LOGIN_HEADER_BUTTON => driver.FindElement(By.Id("login2"));
        private IWebElement SIGNUP_FORM => driver.FindElement(By.Id("signInModal"));
        private IWebElement LOGIN_FORM => driver.FindElement(By.Id("logInModal"));
        private IWebElement SIGNUP_FORM_USERNAME_FIELD => driver.FindElement(By.Id("sign-username"));
        private IWebElement SIGNUP_FORM_PASSWORD_FIELD => driver.FindElement(By.Id("sign-password"));
        private IWebElement LOGIN_FORM_USERNAME_FIELD => driver.FindElement(By.Id("loginusername"));
        private IWebElement LOGIN_FORM_PASSWORD_FIELD => driver.FindElement(By.Id("loginpassword"));
        private IWebElement SIGNUP_FORM_SIGNUP_BUTTON => driver.FindElement(By.CssSelector("button[onclick='register()']"));
        private IWebElement LOGIN_FORM_LOGIN_BUTTON => driver.FindElement(By.CssSelector("button[onclick='logIn()']"));
        private IWebElement LOGGEDIN_USER_EMAIL_HEADER => driver.FindElement(By.Id("nameofuser"));
        private IWebElement ORDER_FORM_CLOSE_BUTTON => driver.FindElement(By.CssSelector("div[id='orderModal'] div[class='modal-footer'] button:nth-child(1)"));
        private IWebElement LOG_OUT_BUTTON => driver.FindElement(By.Id("logout2"));
        public HomePage(IWebDriver driver, ExtentTest test)
        {
            this.driver = driver;
            this.test = test;
        }
        public void validatePageContent(String expectedEmail)
        {
            string actualPageUrl = driver.Url;
            string EXPECTED_PAGE_URL = GlobalValues .HOME_PAGE_URL;
            Assert.That(actualPageUrl, Is.EqualTo(EXPECTED_PAGE_URL), ValidationMessage.VALIDATE_PAGE_URL);
            test.Log(Status.Pass, "Validate home page url.");

            bool isHeaderDisplayed = HOME_PAGE_HEADER.Displayed;
            Assert.That(isHeaderDisplayed, Is.True, ValidationMessage.VALIDATE_HEADER_DISPLAY);
            test.Log(Status.Pass, "Validate page header is displayed.");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(d => LOGGEDIN_USER_EMAIL_HEADER.Displayed);

            string userWelcomeHeaderText = LOGGEDIN_USER_EMAIL_HEADER.GetAttribute("innerText");
            string[] partsOfMessage = userWelcomeHeaderText.Split(" ");
            string extractedUserEmail = partsOfMessage[1];
            Assert.That(extractedUserEmail, Is.EqualTo(expectedEmail), ValidationMessage.VALIDATE_LOGGEDIN_USER_EMAIL);
            test.Log(Status.Pass, "Validate correct user is logged in.");
        }
        public void fillSignUpForm(string username, string password)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(d => SIGNUP_HEADER_BUTTON.Displayed);
            SIGNUP_HEADER_BUTTON.Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(d => SIGNUP_FORM.Displayed);
            validateSignUpFormContent();
            SIGNUP_FORM_USERNAME_FIELD.SendKeys(username);
            test.Log(Status.Pass, "Entered email successfully.");
            SIGNUP_FORM_PASSWORD_FIELD.SendKeys(password);
            test.Log(Status.Pass, "Entered password successfully.");
            SIGNUP_FORM_SIGNUP_BUTTON.Click();
            test.Log(Status.Pass, "Click on signup button.");
            alert();
        }

        public void fillLoginForm(String username, String password)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(d => LOGIN_HEADER_BUTTON.Displayed);
            LOGIN_HEADER_BUTTON.Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(d => LOGIN_FORM.Displayed);
            validateLoginFormContent();
            LOGIN_FORM_USERNAME_FIELD.SendKeys(username);
            test.Log(Status.Pass, "Entered email successfully.");
            LOGIN_FORM_PASSWORD_FIELD.SendKeys(password);
            test.Log(Status.Pass, "Entered password successfully.");
            LOGIN_FORM_LOGIN_BUTTON.Click();
            test.Log(Status.Pass, "Click on login button.");
        }

        public void clickOnProduct(String productType, String productName)
        {
            foreach (var product in CATEGORIES)
            {
                string category = product.GetAttribute("innerText").ToLower();

                if (category.Equals(productType.ToLower()))
                {
                    product.Click();
                    test.Log(Status.Pass, $"Clicked on {productType} category.");
                    break;
                }
            }

            IReadOnlyCollection<IWebElement> productsToChoose;
            switch (productType)
            {
                case "Phones":
                    productsToChoose = PHONES;
                    break;
                case "Laptops":
                    productsToChoose = LAPOTOPS;
                    break;
                case "Monitors":
                    productsToChoose = MONITORS;
                    break;
                default:
                    productsToChoose = PHONES;
                    break;
            }

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            
            foreach (var device in productsToChoose)
            {
                try
                {
                    string product = device.GetAttribute("innerText").ToLower();

                    if (product.Equals(productName.ToLower()))
                    {
                        device.Click();
                        test.Log(Status.Pass, $"Clicked on {productName} product.");
                        break;
                    }
                }
                catch (StaleElementReferenceException)
                {
                    clickOnProduct(productName);
                    test.Log(Status.Pass, $"Clicked on {productName} product.");
                    break;

                }
            }
        }

        public void logOut()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
                wait.Until(d => LOG_OUT_BUTTON.Displayed);
                LOG_OUT_BUTTON.Click();
                test.Log(Status.Pass, "Click on logout button.");
            }
            catch (ElementClickInterceptedException)
            {
                ORDER_FORM_CLOSE_BUTTON.Click();
                LOG_OUT_BUTTON.Click();
                test.Log(Status.Pass, "Click on logout button.");
            }
        }

        private void clickOnProduct(string name)
        {
            driver.FindElement(By.LinkText(name)).Click();
        }

        private void validateSignUpFormContent()
        {
            bool isSignUpFormDisplayed = SIGNUP_FORM.Displayed;
            Assert.That(isSignUpFormDisplayed, ValidationMessage.VALIDATE_SIGNUP_FORM);
            test.Log(Status.Pass, "Validate signup form is displayed.");

            bool isSignUpFormUsernameFieldDisplayed = SIGNUP_FORM_USERNAME_FIELD.Displayed;
            Assert.That(isSignUpFormUsernameFieldDisplayed, ValidationMessage.VALIDATE_SIGNUP_FORM_USERNAME_FIELD);
            test.Log(Status.Pass, "Validate signup username field is displayed");

            bool isSignUpFormPasswordFieldDisplayed = SIGNUP_FORM_PASSWORD_FIELD.Displayed;
            Assert.That(isSignUpFormPasswordFieldDisplayed, ValidationMessage.VALIDATE_SIGNUP_FORM_PASSWORD_FIELD);
            test.Log(Status.Pass, "Validate signup password field is displayed");
        }

        private void validateLoginFormContent()
        {
            bool isLoginFormDisplayed = LOGIN_FORM.Displayed;
            Assert.That(isLoginFormDisplayed, ValidationMessage.VALIDATE_LOGIN_FORM);
            test.Log(Status.Pass, "Validate login form is displayed.");

            bool isloginFormUsernameFieldDisplayed = LOGIN_FORM_USERNAME_FIELD.Displayed;
            Assert.That(isloginFormUsernameFieldDisplayed, ValidationMessage.VALIDATE_LOGIN_FORM_USERNAME_FIELD);
            test.Log(Status.Pass, "Validate login username field is displayed");

            bool isLoginFormPasswordFieldDisplayed = LOGIN_FORM_PASSWORD_FIELD.Displayed;
            Assert.That(isLoginFormPasswordFieldDisplayed, ValidationMessage.VALIDATE_LOGIN_FORM_PASSWORD_FIELD);
            test.Log(Status.Pass, "Validate login password field is displayed");
        }
        private void alert()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            IAlert alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
            alert.Accept();
        }
    }
}
