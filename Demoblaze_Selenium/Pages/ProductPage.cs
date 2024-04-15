using OpenQA.Selenium;
using Demoblaze_Selenium.utils;
using OpenQA.Selenium.Support.UI;
using AventStack.ExtentReports;

namespace Demoblaze_Selenium.pages
{
    public class ProductPage
    {
        private IWebDriver driver;
        private ExtentTest test;
        private IWebElement HOME_HEADER_BUTTON => driver.FindElement(By.CssSelector(".nav-link[href='index.html']"));
        private IWebElement LOGGEDIN_USER_EMAIL_HEADER => driver.FindElement(By.Id("nameofuser"));
        private IWebElement HOME_PAGE_HEADER => driver.FindElement(By.Id("nava"));
        private IWebElement PRODUCT_IMAGE => driver.FindElement(By.CssSelector(".product-image"));
        private string ACTUAL_URL => driver.Url;
        private IWebElement ADD_TO_CART_BUTTON => driver.FindElement(By.XPath("//a[normalize-space()='Add to cart']"));
        private IWebElement ITEM_NAME => driver.FindElement(By.CssSelector(".name"));
        private IWebElement ITEM_PRICE => driver.FindElement(By.CssSelector("h3[class='price-container'] small"));
        private IWebElement ITEM_DESCRIPTION => driver.FindElement(By.XPath("//div[@id='more-information']"));
        private IWebElement CART_BUTTON => driver.FindElement(By.Id("cartur"));
        public ProductPage(IWebDriver driver, ExtentTest test)
        {
            this.driver = driver;
            this.test = test;
        }

        public void validatePageContent(string expectedEmail, string productName)
        {
            string EXPECTED_PAGE_URL = $"https://www.demoblaze.com/prod.html?idp_={extractProductId()}";
            Assert.That(ACTUAL_URL, Is.EqualTo(EXPECTED_PAGE_URL), ValidationMessage.VALIDATE_PAGE_URL);
            test.Log(Status.Pass, "Validate product page url.");

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

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(d => PRODUCT_IMAGE.Displayed);
            bool isProductPictureDisplayed = PRODUCT_IMAGE.Displayed;
            Assert.That(isProductPictureDisplayed, Is.True, ValidationMessage.VALIDATE_PRODUCT_PICTURE_PRESENCE);
            test.Log(Status.Pass, $"Validate {productName} picture is displayed.");

            wait.Until(d => ITEM_NAME.Displayed);
            bool isProductNameDisplayed = ITEM_NAME.Displayed;
            Assert.That(isProductNameDisplayed, Is.True, ValidationMessage.VALIDATE_PRODUCT_NAME_PRESENCE);
            test.Log(Status.Pass, $"Validate {productName} name is displayed.");

            bool isProductPriceDisplayed = ITEM_PRICE.Displayed;
            Assert.That(isProductPriceDisplayed, Is.True, ValidationMessage.VALIDATE_PRODUCT_PRICE_PRESENCE);
            test.Log(Status.Pass, $"Validate {productName} price is displayed.");

            bool isProductDescriptionDisplayed = ITEM_DESCRIPTION.Displayed;
            Assert.That(isProductDescriptionDisplayed, Is.True, ValidationMessage.VALIDATE_PRODUCT_NAME_PRESENCE);
            test.Log(Status.Pass, $"Validate {productName} description is displayed.");

            string actualProductName = ITEM_NAME.GetAttribute("innerText");
            Assert.That(actualProductName, Is.EqualTo(productName), ValidationMessage.VALIDATE_PRODUCT_NAME_VALUE);
            test.Log(Status.Pass, $"Validate {productName} name is correct.");

            bool isProductNameInDescription = containsProductInDescription(productName);
            Assert.That(isProductNameInDescription, Is.True, ValidationMessage.VALIDATE_PRODUCT_NAME_IN_DESCRIPTION);
            test.Log(Status.Pass, $"Validate {productName} description is correct.");
        }

        public void addToCart()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(d => ADD_TO_CART_BUTTON.Displayed);
            ADD_TO_CART_BUTTON.Click();
            test.Log(Status.Pass, "Add product to cart");
            alert();
        }
        public void goToCart()
        {
            CART_BUTTON.Click();
            test.Log(Status.Pass, "Click on cart button.");
        }

        public void navigateBack()
        {              
            HOME_HEADER_BUTTON.Click();
            test.Log(Status.Pass, "Navigate to the home page");
        }

        private string extractProductId()
        {
            int index = ACTUAL_URL.LastIndexOf('=');
            string productId = ACTUAL_URL.Substring(index + 1);
            return productId;
        }

        private bool containsProductInDescription(string productName)
        {
            string actualProductDescription = ITEM_DESCRIPTION.Text.ToLower();
            if (actualProductDescription.Contains(productName.ToLower()))
            {
                return true;
            }
            return false;
        }

        private void alert()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            IAlert alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
            alert.Accept();
        }
    }
}
