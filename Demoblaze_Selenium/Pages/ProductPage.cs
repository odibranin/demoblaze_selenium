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
        private IWebElement homeHeaderButton => driver.FindElement(By.CssSelector(".nav-link[href='index.html']"));
        private IWebElement loggedInUserEmailHeader => driver.FindElement(By.Id("nameofuser"));
        private IWebElement homePageHeader => driver.FindElement(By.Id("nava"));
        private IWebElement productImage => driver.FindElement(By.CssSelector(".product-image"));
        private string actualUrl => driver.Url;
        private IWebElement addToCartButton => driver.FindElement(By.XPath("//a[normalize-space()='Add to cart']"));
        private IWebElement itemName => driver.FindElement(By.CssSelector(".name"));
        private IWebElement itemPrice => driver.FindElement(By.CssSelector("h3[class='price-container'] small"));
        private IWebElement itemDescription => driver.FindElement(By.XPath("//div[@id='more-information']"));
        private IWebElement cartButton => driver.FindElement(By.Id("cartur"));

        public ProductPage(IWebDriver driver, ExtentTest test)
        {
            this.driver = driver;
            this.test = test;
        }

        public void ValidatePageContent(string expectedEmail, string productName)
        {
            string expectedPageUrl = $"https://www.demoblaze.com/prod.html?idp_={ExtractProductId()}";
            Assert.That(actualUrl, Is.EqualTo(expectedPageUrl), ValidationMessage.VALIDATE_PAGE_URL);
            test.Log(Status.Pass, "Validate product page url.");

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

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(d => productImage.Displayed);
            bool isProductPictureDisplayed = productImage.Displayed;
            Assert.That(isProductPictureDisplayed, Is.True, ValidationMessage.VALIDATE_PRODUCT_PICTURE_PRESENCE);
            test.Log(Status.Pass, $"Validate {productName} picture is displayed.");

            wait.Until(d => itemName.Displayed);
            bool isProductNameDisplayed = itemName.Displayed;
            Assert.That(isProductNameDisplayed, Is.True, ValidationMessage.VALIDATE_PRODUCT_NAME_PRESENCE);
            test.Log(Status.Pass, $"Validate {productName} name is displayed.");

            bool isProductPriceDisplayed = itemPrice.Displayed;
            Assert.That(isProductPriceDisplayed, Is.True, ValidationMessage.VALIDATE_PRODUCT_PRICE_PRESENCE);
            test.Log(Status.Pass, $"Validate {productName} price is displayed.");

            bool isProductDescriptionDisplayed = itemDescription.Displayed;
            Assert.That(isProductDescriptionDisplayed, Is.True, ValidationMessage.VALIDATE_PRODUCT_NAME_PRESENCE);
            test.Log(Status.Pass, $"Validate {productName} description is displayed.");

            string actualProductName = itemName.GetAttribute("innerText");
            Assert.That(actualProductName, Is.EqualTo(productName), ValidationMessage.VALIDATE_PRODUCT_NAME_VALUE);
            test.Log(Status.Pass, $"Validate {productName} name is correct.");

            bool isProductNameInDescription = ContainsProductInDescription(productName);
            Assert.That(isProductNameInDescription, Is.True, ValidationMessage.VALIDATE_PRODUCT_NAME_IN_DESCRIPTION);
            test.Log(Status.Pass, $"Validate {productName} description is correct.");
        }

        public void AddToCart()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(d => addToCartButton.Displayed);
            addToCartButton.Click();
            test.Log(Status.Pass, "Add product to cart");
            Alert();
        }

        public void GoToCart()
        {
            cartButton.Click();
            test.Log(Status.Pass, "Click on cart button.");
        }

        public void NavigateBack()
        {
            homeHeaderButton.Click();
            test.Log(Status.Pass, "Navigate to the home page");
        }

        private string ExtractProductId()
        {
            int index = actualUrl.LastIndexOf('=');
            string productId = actualUrl.Substring(index + 1);
            return productId;
        }

        private bool ContainsProductInDescription(string productName)
        {
            string actualProductDescription = itemDescription.Text.ToLower();
            if (actualProductDescription.Contains(productName.ToLower()))
            {
                return true;
            }
            return false;
        }

        private void Alert()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            IAlert alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
            alert.Accept();
        }
    }
}
