using OpenQA.Selenium;
using Demoblaze_Selenium.utils;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using AventStack.ExtentReports;

namespace Demoblaze_Selenium.pages
{
    internal class CartPage
    {
        private IWebDriver driver;
        private ExtentTest test;
        private string actualUrl => driver.Url;
        private IWebElement homePageHeader => driver.FindElement(By.Id("nava"));
        private IWebElement loggedInUserEmailHeader => driver.FindElement(By.Id("nameofuser"));
        private IWebElement calculatedTotalPrice => driver.FindElement(By.Id("totalp"));
        private IWebElement placeOrderButton => driver.FindElement(By.CssSelector("button[class*='btn-success']"));
        private IWebElement placeOrderNameField => driver.FindElement(By.Id("name"));
        private IWebElement placeOrderCountryField => driver.FindElement(By.Id("country"));
        private IWebElement placeOrderCityField => driver.FindElement(By.Id("city"));
        private IWebElement placeOrderCreditCardField => driver.FindElement(By.Id("card"));
        private IWebElement placeOrderMonthField => driver.FindElement(By.Id("month"));
        private IWebElement placeOrderYearField => driver.FindElement(By.Id("year"));
        private IWebElement purchaseButton => driver.FindElement(By.XPath("//button[contains(text(), 'Purchase')]"));
        private IWebElement conformationMessage => driver.FindElement(By.XPath("//h2[normalize-space()='Thank you for your purchase!']"));
        private IWebElement purchaseOrderUserInformation => driver.FindElement(By.CssSelector(".lead.text-muted"));
        private IWebElement conformationMessageOkButton => driver.FindElement(By.CssSelector(".confirm.btn.btn-lg.btn-primary"));

        private IReadOnlyCollection<IWebElement> productsInCart;
        private bool productsInCartInitialized = false;
        private double actualTotalPrice;

        public CartPage(IWebDriver driver, ExtentTest test)
        {
            this.driver = driver;
            this.test = test;
        }

        public void ValidatePageContent(string expectedEmail)
        {
            string expectedPageUrl = GlobalValues.CART_PAGE_URL;
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

            bool areProductInCart = ValidateProductNamesInCart(GlobalValues.allProducts);
            Assert.That(areProductInCart, Is.True, ValidationMessage.VALIDATE_PRODUCTS_IN_CART);
            test.Log(Status.Pass, "Validate cart content.");

            bool isTotalPriceCalculatedRight = ValidateTotalPrice();
            Assert.That(isTotalPriceCalculatedRight, Is.True, ValidationMessage.VALIDATE_TOTAL_PRICE_VALUE);
            test.Log(Status.Pass, "Validate cart total price.");
        }

        public void ClickPlaceOrderButton()
        {
            placeOrderButton.Click();
            test.Log(Status.Pass, "Click on place order button.");
        }

        public void FillPlaceOrderForm(string name,
                                        string country,
                                        string city,
                                        string creditCard,
                                        string month,
                                        string year
                                       )
        {
            placeOrderNameField.SendKeys(name);
            test.Log(Status.Pass, "Entered name successfully.");
            placeOrderCountryField.SendKeys(country);
            test.Log(Status.Pass, "Entered country successfully.");
            placeOrderCityField.SendKeys(city);
            test.Log(Status.Pass, "Entered city successfully.");
            placeOrderCreditCardField.SendKeys(creditCard);
            test.Log(Status.Pass, "Entered credit card successfully.");
            placeOrderMonthField.SendKeys(month);
            test.Log(Status.Pass, "Entered motnh successfully.");
            placeOrderYearField.SendKeys(year);
            test.Log(Status.Pass, "Entered year successfully.");
            purchaseButton.Click();
            test.Log(Status.Pass, "Click on pruchase button.");
        }

        public void ValidatePurchaseConformationForm(string expectedCreditCard,
                                                      string expectedName,
                                                      string expectedConformationMessage
                                                      )
        {
            string purchaseOrderUserInformationText = purchaseOrderUserInformation.Text;
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            string[] singleRow = purchaseOrderUserInformationText.Split('\n');

            foreach (string line in singleRow)
            {
                string[] parts = line.Split(":");
                string key = parts[0].Trim();
                string value = parts[1].Trim();

                keyValuePairs[key] = value;

            }

            Assert.That(keyValuePairs["Card Number"], Is.EqualTo(expectedCreditCard), ValidationMessage.VALIDATE_CARD_NUMBER_IS_PRESENT_IN_PURCHASE_FORM);
            test.Log(Status.Pass, "Validate card number value in purchase form.");
            Assert.That(keyValuePairs["Name"], Is.EqualTo(expectedName), ValidationMessage.VALIDATE_NAME_IS_PRESENT_IN_PURCHASE_FORM);
            test.Log(Status.Pass, "Validate name value in purchase form.");
            Assert.That(keyValuePairs["Amount"], Is.EqualTo(actualTotalPrice.ToString() + " USD"), ValidationMessage.VALIDATE_AMOUNT_IS_PRESENT_IN_PURCHASE_FORM);
            test.Log(Status.Pass, "Validate amount value in purchase form.");
            Assert.That(conformationMessage.Text, Is.EqualTo(expectedConformationMessage), ValidationMessage.VALIDATE_CONFORMATION_MESSAGE_IS_PRESENT_IN_PURCHASE_FORM);
            test.Log(Status.Pass, "Validate conformation message in purchase form.");
            conformationMessageOkButton.Click();
        }

        public void DeleteAllProductsFromCart()
        {
            foreach (var productRow in GetProductsInCart())
            {
                var deleteButton = productRow.FindElement(By.CssSelector("td:nth-child(4) a"));
                deleteButton.Click();
            }
        }

        private IReadOnlyCollection<IWebElement> GetProductsInCart()
        {
            if (!productsInCartInitialized)
            {
                productsInCart = WaitForProductRows(5);
                productsInCartInitialized = true;
            }
            return productsInCart;
        }

        private IReadOnlyCollection<IWebElement> WaitForProductRows(int timeoutInSeconds)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#tbodyid tr.success")));
            return driver.FindElements(By.CssSelector("#tbodyid tr.success"));
        }

        private bool ValidateProductNamesInCart(string[] productNames)
        {
            foreach (string productName in productNames)
            {
                bool found = false;
                foreach (var product in GetProductsInCart())
                {
                    string productText = product.FindElement(By.XPath("./td[2]")).Text.ToLower();

                    if (productText.Contains(productName.ToLower()))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return false;
                }
            }
            return true;
        }

        private bool ValidateTotalPrice()
        {
            actualTotalPrice = 0;
            foreach (var product in GetProductsInCart())
            {
                string priceText = product.FindElement(By.XPath("./td[3]")).Text;
                double price = double.Parse(priceText.Replace("$", ""));
                this.actualTotalPrice += price;
            }

            double totalPrice = double.Parse(calculatedTotalPrice.Text);
            if (actualTotalPrice == totalPrice)
            {
                return true;
            }
            return false;
        }
    }
}
