using System.Collections.Generic;
using System.IO;

namespace Demoblaze_Selenium.utils
{
    public class FileTestDataReader
    {
        public static List<TestCaseData> CreateTestCases(string filePath)
        {
            var testCases = new List<TestCaseData>();

            using (var fs = File.OpenRead(filePath))
            using (var sr = new StreamReader(fs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] tests = line.Split(',');
                    var testCase = new TestCaseData(tests);
                    testCases.Add(testCase);
                }
            }

            return testCases;
        }
    }
}
