using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using Demoblaze_Selenium.utils;

namespace com.example.swaglabs.utils
{
    public class WebDriverSetup
    {
        public static IWebDriver GetWebDriver(string browserName)
        {
            IWebDriver driver = browserName.ToLower() switch
            {
                "chrome" => GetChromeDriver(),
                "firefox" => GetFirefoxDriver(),
                "edge" => GetEdgeDriver(),
                _ => throw new ArgumentException("Browser configuration not found for the following browser: " + browserName)
            };

            ConfigureDriver(driver);
            return driver;
        }

        private static IWebDriver GetChromeDriver()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            return new ChromeDriver();
        }

        private static IWebDriver GetFirefoxDriver()
        {
            new DriverManager().SetUpDriver(new FirefoxConfig());
            return new FirefoxDriver();
        }

        private static IWebDriver GetEdgeDriver()
        {
            new DriverManager().SetUpDriver(new EdgeConfig());
            return new EdgeDriver();
        }

        private static void ConfigureDriver(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(GlobalValues.HOME_PAGE_URL);
            driver.Manage().Window.Maximize();

        }
    }
}
