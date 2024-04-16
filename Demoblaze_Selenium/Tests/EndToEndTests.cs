using com.example.swaglabs.utils;
using Demoblaze_Selenium.pages;
using Demoblaze_Selenium.Pages;
using Demoblaze_Selenium.utils;
using OpenQA.Selenium;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;


namespace Demoblaze_Selenium.Tests
{
    [TestFixture()]
    public class EndToEndTests
    {
        private IWebDriver driver;
        private ExtentReports extent;
        private ExtentTest test;
        private HomePage homePage;
        private ProductPage productPage;
        private CartPage cartPage;
        
        [SetUp]
        public void Setup()
        {
            driver = WebDriverSetup.GetWebDriver("chrome");
            var sparkReporter = new ExtentSparkReporter(GlobalValues.testReportPath);
            extent = new ExtentReports();
            extent.AttachReporter(sparkReporter);

            test = extent.CreateTest("verifyEndToEndCheckoutProcess");
            homePage = new HomePage(driver, test);
            productPage = new ProductPage(driver, test);
            cartPage = new CartPage(driver, test);
        }
        private static IEnumerable<TestCaseData> GetTestCases()
        {
            return FileTestDataReader.CreateTestCases(GlobalValues.e2eTestCaseDataPath);
        }

        [Test]
        [TestCaseSource(nameof(GetTestCases))]
        public void VerifyEndToEndCheckoutProcess(TestDataModel testDataModel)
        {
            string validUsername = testDataModel.ValidUsername;
            string validPassword = testDataModel.ValidPassword;
            string phonesCategory = testDataModel.PhonesCategory;
            string nokiaPhone = testDataModel.NokiaPhone;
            string laptopsCategory = testDataModel.LaptopsCategory;
            string appleLaptop = testDataModel.AppleLaptop;
            string name = testDataModel.Name;
            string country = testDataModel.Country;
            string city = testDataModel.City;
            string creditCard = testDataModel.CreditCard;
            string month = testDataModel.Month;
            string year = testDataModel.Year;
            string conformationMessage = testDataModel.ConformationMessage;

            // Start the test
            test.Log(Status.Info, "Starting the End-to-End Checkout Process");

            // Validate HomePage and LogIn
            test.Log(Status.Info, "Filling login form with valid credentials");
            homePage.FillLoginForm(validUsername, validPassword);
            test.Log(Status.Info, "Validate Home Page Content");
            homePage.ValidatePageContent(validUsername);

            // Validate ProductPage and add item to cart
            test.Log(Status.Info, $"Navigating to {phonesCategory} and selecting {nokiaPhone}");
            homePage.ClickOnProduct(phonesCategory, nokiaPhone);
            test.Log(Status.Info, "Validate Product Page Content");
            productPage.ValidatePageContent(validUsername, nokiaPhone);
            productPage.AddToCart();


            // Navigate back to HomePage
            productPage.NavigateBack();

            // Validate ProductPage and add item to cart
            test.Log(Status.Info, $"Navigating to {laptopsCategory} and selecting {appleLaptop}");
            homePage.ClickOnProduct(laptopsCategory, appleLaptop);
            test.Log(Status.Info, "Validate Product Page Content");
            productPage.ValidatePageContent(validUsername, appleLaptop);
            productPage.AddToCart();

            // Navigate to the CartPage 
            productPage.GoToCart();

            // Validate CartPage, fill out the order form 
            test.Log(Status.Info, "Validate Cart Page Content");
            cartPage.ValidatePageContent(validUsername);
            cartPage.ClickPlaceOrderButton();
            test.Log(Status.Info, "Filling out the order form");
            cartPage.FillPlaceOrderForm(name, country, city, creditCard, month, year);
            test.Log(Status.Info, "Validating Purchase Confirmation");
            cartPage.ValidatePurchaseConformationForm(creditCard, name, conformationMessage);

            // LogOut
            homePage.LogOut();
            test.Log(Status.Pass, "Test passed");
        }

        [TearDown]
        public void TearDown()
        {
            extent.Flush();
            driver.Quit();
        }
    }
}
