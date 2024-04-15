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
            this.driver = WebDriverSetup.GetWebDriver("chrome");
            var sparkReporter = new ExtentSparkReporter(GlobalValues.testReportPath);
            extent = new ExtentReports();
            extent.AttachReporter(sparkReporter);

            test = extent.CreateTest("verifyEndToEndCheckoutProcess");
            homePage = new HomePage(this.driver, test);
            productPage = new ProductPage(this.driver, test);
            cartPage = new CartPage(this.driver, test);
                    }
        private static IEnumerable<TestCaseData> GetTestCases()
        {
            string filePath = GlobalValues.e2eTestCaseDataPath;
            return FileTestDataReader.CreateTestCases(filePath);
        }

        [Test]
        [TestCaseSource(nameof(GetTestCases))]
        public void verifyEndToEndCheckoutProcess( string validUsername,
                                                   string validPassword,
                                                   string phonesCategory,
                                                   string nokiaPhone,
                                                   string laptopsCategory,
                                                   string appleLaptop,
                                                   string name,
                                                   string country,
                                                   string city,
                                                   string creditCard,
                                                   string month,
                                                   string year,
                                                   string conformationMessage
                                                 )
        {
            // Start the test
            test.Log(Status.Info, "Starting the End-to-End Checkout Process");

            // Validate HomePage and LogIn
            test.Log(Status.Info, "Filling login form with valid credentials");
            homePage.fillLoginForm(validUsername, validPassword);
            test.Log(Status.Info, "Validate Home Page Content");
            homePage.validatePageContent(validUsername);

            // Validate ProductPage and add item to cart
            test.Log(Status.Info, $"Navigating to {phonesCategory} and selecting {nokiaPhone}");
            homePage.clickOnProduct(phonesCategory, nokiaPhone);
            test.Log(Status.Info, "Validate Product Page Content");
            productPage.validatePageContent(validUsername, nokiaPhone);
            productPage.addToCart();


            // Navigate back to HomePage
            productPage.navigateBack();

            // Validate ProductPage and add item to cart
            test.Log(Status.Info, $"Navigating to {laptopsCategory} and selecting {appleLaptop}");
            homePage.clickOnProduct(laptopsCategory, appleLaptop);
            test.Log(Status.Info, "Validate Product Page Content");
            productPage.validatePageContent(validUsername, appleLaptop);
            productPage.addToCart();

            // Navigate to the CartPage 
            productPage.goToCart();

            // Validate CartPage, fill out the order form 
            test.Log(Status.Info, "Validate Cart Page Content");
            cartPage.validatePageContent(validUsername);            
            cartPage.clickPlaceOrderButton();
            test.Log(Status.Info, "Filling out the order form");
            cartPage.fillPlaceOrderForm(name, country, city, creditCard, month, year);
            test.Log(Status.Info, "Validating Purchase Confirmation");
            cartPage.validatePurchaseConformationForm(creditCard, name, conformationMessage);
            
            // LogOut
            homePage.logOut();
            test.Log(Status.Pass, "Test passed");

        }

        [TearDown]
        public void TearDown()
        {
            extent.Flush();
            this.driver.Quit();
        }


    }
}
