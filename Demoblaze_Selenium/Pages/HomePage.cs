using OpenQA.Selenium;
using Demoblaze_Selenium.utils;
using OpenQA.Selenium.Support.UI;
using AventStack.ExtentReports;

namespace Demoblaze_Selenium.Pages
{
    public class HomePage
    {
        private IWebDriver driver;
        private ExtentTest test;
        private IReadOnlyCollection<IWebElement> categories => driver.FindElements(By.CssSelector(".list-group-item:not(#cat)"));
        private IReadOnlyCollection<IWebElement> phones => driver.FindElements(By.CssSelector(".card-title a"));
        private IReadOnlyCollection<IWebElement> laptops => driver.FindElements(By.CssSelector(".card-block .card-title a"));
        private IReadOnlyCollection<IWebElement> monitors => driver.FindElements(By.CssSelector(".card-block .card-title a"));
        private IWebElement homePageHeader => driver.FindElement(By.Id("nava"));
        private IWebElement signupHeaderButton => driver.FindElement(By.Id("signin2"));
        private IWebElement loginHeaderButton => driver.FindElement(By.Id("login2"));
        private IWebElement signupForm => driver.FindElement(By.Id("signInModal"));
        private IWebElement loginForm => driver.FindElement(By.Id("logInModal"));
        private IWebElement signupFormUsernameField => driver.FindElement(By.Id("sign-username"));
        private IWebElement signupFormPasswordField => driver.FindElement(By.Id("sign-password"));
        private IWebElement loginFormUsernameField => driver.FindElement(By.Id("loginusername"));
        private IWebElement loginFormPasswordField => driver.FindElement(By.Id("loginpassword"));
        private IWebElement signupFormSignupButton => driver.FindElement(By.CssSelector("button[onclick='register()']"));
        private IWebElement loginFormLoginButton => driver.FindElement(By.CssSelector("button[onclick='logIn()']"));
        private IWebElement loggedInUserEmailHeader => driver.FindElement(By.Id("nameofuser"));
        private IWebElement orderFormCloseButton => driver.FindElement(By.CssSelector("div[id='orderModal'] div[class='modal-footer'] button:nth-child(1)"));
        private IWebElement logoutButton => driver.FindElement(By.Id("logout2"));

        public HomePage(IWebDriver driver, ExtentTest test)
        {
            this.driver = driver;
            this.test = test;
        }

        public void ValidatePageContent(String expectedEmail)
        {
            string actualPageUrl = driver.Url;
            string expectedPageUrl = GlobalValues.HOME_PAGE_URL;
            Assert.That(actualPageUrl, Is.EqualTo(expectedPageUrl), ValidationMessage.VALIDATE_PAGE_URL);
            test.Log(Status.Pass, "Validate home page url.");

            bool isHeaderDisplayed = homePageHeader.Displayed;
            Assert.That(isHeaderDisplayed, Is.True, ValidationMessage.VALIDATE_HEADER_DISPLAY);
            test.Log(Status.Pass, "Validate page header is displayed.");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(d => loggedInUserEmailHeader.Displayed);

            string userWelcomeHeaderText = loggedInUserEmailHeader.GetAttribute("innerText");
            string[] partsOfMessage = userWelcomeHeaderText.Split(" ");
            string extractedUserEmail = partsOfMessage[1];
            Assert.That(extractedUserEmail, Is.EqualTo(expectedEmail), ValidationMessage.VALIDATE_LOGGEDIN_USER_EMAIL);
            test.Log(Status.Pass, "Validate correct user is logged in.");
        }

        public void FillSignUpForm(string username, string password)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(d => signupHeaderButton.Displayed);
            signupHeaderButton.Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(d => signupForm.Displayed);
            ValidateSignUpFormContent();
            signupFormUsernameField.SendKeys(username);
            test.Log(Status.Pass, "Entered email successfully.");
            signupFormPasswordField.SendKeys(password);
            test.Log(Status.Pass, "Entered password successfully.");
            signupFormSignupButton.Click();
            test.Log(Status.Pass, "Click on signup button.");
            Alert();
        }

        public void FillLoginForm(String username, String password)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(d => loginHeaderButton.Displayed);
            loginHeaderButton.Click();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(d => loginForm.Displayed);
            ValidateLoginFormContent();
            loginFormUsernameField.SendKeys(username);
            test.Log(Status.Pass, "Entered email successfully.");
            loginFormPasswordField.SendKeys(password);
            test.Log(Status.Pass, "Entered password successfully.");
            loginFormLoginButton.Click();
            test.Log(Status.Pass, "Click on login button.");
        }

        public void ClickOnProduct(String productType, String productName)
        {
            foreach (var product in categories)
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
                    productsToChoose = phones;
                    break;
                case "Laptops":
                    productsToChoose = laptops;
                    break;
                case "Monitors":
                    productsToChoose = monitors;
                    break;
                default:
                    productsToChoose = phones;
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
                    ClickOnProduct(productName);
                    test.Log(Status.Pass, $"Clicked on {productName} product.");
                    break;

                }
            }
        }

        public void LogOut()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
                wait.Until(d => logoutButton.Displayed);
                logoutButton.Click();
                test.Log(Status.Pass, "Click on logout button.");
            }
            catch (ElementClickInterceptedException)
            {
                orderFormCloseButton.Click();
                logoutButton.Click();
                test.Log(Status.Pass, "Click on logout button.");
            }
        }

        private void ClickOnProduct(string name)
        {
            driver.FindElement(By.LinkText(name)).Click();
        }

        private void ValidateSignUpFormContent()
        {
            bool isSignUpFormDisplayed = signupForm.Displayed;
            Assert.That(isSignUpFormDisplayed, ValidationMessage.VALIDATE_SIGNUP_FORM);
            test.Log(Status.Pass, "Validate signup form is displayed.");

            bool isSignUpFormUsernameFieldDisplayed = signupFormUsernameField.Displayed;
            Assert.That(isSignUpFormUsernameFieldDisplayed, ValidationMessage.VALIDATE_SIGNUP_FORM_USERNAME_FIELD);
            test.Log(Status.Pass, "Validate signup username field is displayed");

            bool isSignUpFormPasswordFieldDisplayed = signupFormPasswordField.Displayed;
            Assert.That(isSignUpFormPasswordFieldDisplayed, ValidationMessage.VALIDATE_SIGNUP_FORM_PASSWORD_FIELD);
            test.Log(Status.Pass, "Validate signup password field is displayed");
        }

        private void ValidateLoginFormContent()
        {
            bool isLoginFormDisplayed = loginForm.Displayed;
            Assert.That(isLoginFormDisplayed, ValidationMessage.VALIDATE_LOGIN_FORM);
            test.Log(Status.Pass, "Validate login form is displayed.");

            bool isLoginFormUsernameFieldDisplayed = loginFormUsernameField.Displayed;
            Assert.That(isLoginFormUsernameFieldDisplayed, ValidationMessage.VALIDATE_LOGIN_FORM_USERNAME_FIELD);
            test.Log(Status.Pass, "Validate login username field is displayed");

            bool isLoginFormPasswordFieldDisplayed = loginFormPasswordField.Displayed;
            Assert.That(isLoginFormPasswordFieldDisplayed, ValidationMessage.VALIDATE_LOGIN_FORM_PASSWORD_FIELD);
            test.Log(Status.Pass, "Validate login password field is displayed");
        }

        private void Alert()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            IAlert alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
            alert.Accept();
        }
    }
}
