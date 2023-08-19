using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using System;
using System.IO;
using TechTalk.SpecFlow;

namespace SpecFlowBDDAutomationFramework.Utility
{
    public class ExtentReport
    {
        public static ExtentReports _extentReports;
        public static ExtentTest _feature;
        public static ExtentTest _scenario;

        public static string dir = AppDomain.CurrentDomain.BaseDirectory;

        public static void ExtentReportInit()
        {
            string reportsFolderPath = @"C:\SpecFlowProject-API\Reports\";
            var extentReportPath = Path.Combine(reportsFolderPath, "ExtentReport.html");

            _extentReports = new ExtentReports();
            var spark = new ExtentSparkReporter(extentReportPath);
            _extentReports.AttachReporter(spark);
            _extentReports.AddSystemInfo("Application", "Your API");
        }

        public static void ExtentReportTearDown()
        {
            _extentReports.Flush();
        }

    }
}
