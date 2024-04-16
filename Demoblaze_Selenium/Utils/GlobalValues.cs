using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demoblaze_Selenium.utils
{
    internal class GlobalValues
    {
        // Pages URLs
        public static string HOME_PAGE_URL = "https://www.demoblaze.com/index.html";
        public static string CART_PAGE_URL = "https://www.demoblaze.com/cart.html";

        // All products
        public static string[] allProducts = { "Nokia Lumia 1520", "MacBook Pro" };
    
        // Data file Path
        public static string e2eTestCaseDataPath = @"..\..\Data\e2eTestData.csv";

        // Test Reports Path
        public static string testReportPath = @"..\..\Reports\EndToEndTestReport.html";
    }
}


