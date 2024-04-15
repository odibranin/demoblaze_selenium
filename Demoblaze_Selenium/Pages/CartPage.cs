using OpenQA.Selenium;
using Demoblaze_Selenium.utils;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using AventStack.ExtentReports;

namespace Demoblaze_Selenium.pages
{
    internal class CartPage
    {
        private IWebDriver driver;
        private ExtentTest test;
        private string ACTUAL_URL => driver.Url;
        private IWebElement HOME_PAGE_HEADER => driver.FindElement(By.Id("nava"));
        private IWebElement LOGGEDIN_USER_EMAIL_HEADER => driver.FindElement(By.Id("nameofuser"));
        private IWebElement TOTAL_PRICE => driver.FindElement(By.Id("totalp"));
        private IWebElement PLACE_ORDER_BUTTON => driver.FindElement(By.CssSelector("button[class*='btn-success']"));
        private IWebElement PLACE_ORDER_NAME_FIELD => driver.FindElement(By.Id("name"));
        private IWebElement PLACE_ORDER_COUNTRY_FIELD => driver.FindElement(By.Id("country"));
        private IWebElement PLACE_ORDER_CITY_FIELD => driver.FindElement(By.Id("city"));
        private IWebElement PLACE_ORDER_CREDIT_CARD_FIELD => driver.FindElement(By.Id("card"));
        private IWebElement PLACE_ORDER_MONTH_FIELD => driver.FindElement(By.Id("month"));
        private IWebElement PLACE_ORDER_YEAR_FIELD => driver.FindElement(By.Id("year"));
        private IWebElement PURCHASE_BUTTON => driver.FindElement(By.XPath("//button[contains(text(), 'Purchase')]"));
        private IWebElement CONFORMATION_MESSAGE => driver.FindElement(By.XPath("//h2[normalize-space()='Thank you for your purchase!']"));
        private IWebElement PURCHASE_ORDER_USER_INFORMATION => driver.FindElement(By.CssSelector(".lead.text-muted"));
        private IWebElement CONFORMATION_MESSAGE_OK_BUTTON => driver.FindElement(By.CssSelector(".confirm.btn.btn-lg.btn-primary"));

        private IReadOnlyCollection<IWebElement> PRODUCTS_IN_CART;
        private bool productsInCartInitialized = false;
        private double actualTotalPrice;

        public CartPage(IWebDriver driver, ExtentTest test)
        {
            this.driver = driver;
            this.test = test;
        }

        public void validatePageContent(string expectedEmail)
        {
            string EXPECTED_PAGE_URL = GlobalValues.CART_PAGE_URL;
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

            bool areProductInCart = validateProductNamesInCart(GlobalValues.allProducts);
            Assert.That(areProductInCart, Is.True, ValidationMessage.VALIDATE_PRODUCTS_IN_CART);
            test.Log(Status.Pass, "Validate cart content.");

            bool isTotalPriceCalculatedRight = validateTotalPrice();
            Assert.That(isTotalPriceCalculatedRight, Is.True, ValidationMessage.VALIDATE_TOTAL_PRICE_VALUE);
            test.Log(Status.Pass, "Validate cart total price.");
        }

        public void clickPlaceOrderButton()
        {
            PLACE_ORDER_BUTTON.Click();
            test.Log(Status.Pass, "Click on place order button.");
        }

        public void fillPlaceOrderForm(string name,
                                        string country,
                                        string city,
                                        string creditCard,
                                        string month,
                                        string year
                                       )
        {
            PLACE_ORDER_NAME_FIELD.SendKeys(name);
            test.Log(Status.Pass, "Entered name successfully.");
            PLACE_ORDER_COUNTRY_FIELD.SendKeys(country);
            test.Log(Status.Pass, "Entered country successfully.");
            PLACE_ORDER_CITY_FIELD.SendKeys(city);
            test.Log(Status.Pass, "Entered city successfully.");
            PLACE_ORDER_CREDIT_CARD_FIELD.SendKeys(creditCard);
            test.Log(Status.Pass, "Entered credit card successfully.");
            PLACE_ORDER_MONTH_FIELD.SendKeys(month);
            test.Log(Status.Pass, "Entered motnh successfully.");
            PLACE_ORDER_YEAR_FIELD.SendKeys(year);
            test.Log(Status.Pass, "Entered year successfully.");
            PURCHASE_BUTTON.Click();
            test.Log(Status.Pass, "Click on pruchase button.");
        }

        public void validatePurchaseConformationForm( string expectedCreditCard,
                                                      string expectedName,
                                                      string expectedConformationMessage
                                                      )
        {
            string purchaseOrderUserInformationText = PURCHASE_ORDER_USER_INFORMATION.Text;
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
            Assert.That(CONFORMATION_MESSAGE.Text, Is.EqualTo(expectedConformationMessage), ValidationMessage.VALIDATE_CONFORMATION_MESSAGE_IS_PRESENT_IN_PURCHASE_FORM);
            test.Log(Status.Pass, "Validate conformation message in purchase form.");
            CONFORMATION_MESSAGE_OK_BUTTON.Click();
        }

        public void deleteAllProductsFromCart()
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
                PRODUCTS_IN_CART = WaitForProductRows(5);
                productsInCartInitialized = true;
            }
            return PRODUCTS_IN_CART;
        }

        private IReadOnlyCollection<IWebElement> WaitForProductRows(int timeoutInSeconds)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#tbodyid tr.success")));
            return driver.FindElements(By.CssSelector("#tbodyid tr.success"));
        }

        private bool validateProductNamesInCart(string[] productNames)
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

        private bool validateTotalPrice()
        {
            actualTotalPrice = 0;
            foreach (var product in GetProductsInCart())
            {
                string priceText = product.FindElement(By.XPath("./td[3]")).Text;
                double price = double.Parse(priceText.Replace("$", ""));
                this.actualTotalPrice += price;
            }

            double totalPrice = double.Parse(TOTAL_PRICE.Text);
            if (actualTotalPrice == totalPrice)
            {
                return true;
            }
            return false;
        }
    }
}
