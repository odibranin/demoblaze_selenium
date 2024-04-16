using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Demoblaze_Selenium.utils
{
    public class FileTestDataReader
    {
        public static IEnumerable<TestCaseData> CreateTestCases(string filePath)
        {
            var testDataList = ReadTestData(filePath);

            foreach (var data in testDataList)
            {
                var testCaseData = new TestCaseData(data);
                yield return testCaseData;
            }
        }

        private static IEnumerable<TestDataModel> ReadTestData(string filePath)
        {
            var testDataList = new List<TestDataModel>();

            using (var fs = File.OpenRead(filePath))
            using (var sr = new StreamReader(fs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] keyValuePairs = line.Split(',');
                    var testDataModel = new TestDataModel();

                    PropertyInfo[] properties = typeof(TestDataModel).GetProperties();

                    for (int i = 0; i < properties.Length && i < keyValuePairs.Length; i++)
                    {
                        properties[i].SetValue(testDataModel, keyValuePairs[i]);
                    }

                    testDataList.Add(testDataModel);
                }
            }

            return testDataList;
        }
    }
}
