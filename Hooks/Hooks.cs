using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using System.Reflection;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;
using System;
using System.IO;
using RestSharp;

namespace HooksForAll
{
    [Binding]
    public class Hooks
    {
        private static int categoryId;
        private static string categoryName;

        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        private static ExtentReports _extentReports;
        private static ExtentTest _scenario;
        private static string requestDetails;
        private static string responseDetails;
        private static string validationDetails;


        public Hooks(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
        }


        [BeforeTestRun]
        public static void InitializeExtentReports()
        {
            string reportsFolderPath = @"C:\SpecFlowProject-API\Reports\";
            var extentReportPath = Path.Combine(reportsFolderPath, "ExtentReport.html");

            _extentReports = new ExtentReports();
            var spark = new ExtentSparkReporter(extentReportPath);
            _extentReports.AttachReporter(spark);
        }

        [BeforeScenario()]
        public void ProfileStudioReport()
        {
            var feature = _extentReports.CreateTest<Feature>(_featureContext.FeatureInfo.Title);
            _scenario = feature.CreateNode<Scenario>(_scenarioContext.ScenarioInfo.Title);
        }

        [AfterStep]
        public void AfterStep()
        {
            if (_scenarioContext.TestError == null)
                switch (_scenarioContext.StepContext.StepInfo.StepDefinitionType)
                {
                    case StepDefinitionType.Given:
                        _scenario.CreateNode<Given>(_scenarioContext.StepContext.StepInfo.Text);
                        break;
                    case StepDefinitionType.When:
                        _scenario.CreateNode<When>(_scenarioContext.StepContext.StepInfo.Text);
                        break;
                    case StepDefinitionType.Then:
                        _scenario.CreateNode<Then>(_scenarioContext.StepContext.StepInfo.Text);
                        break;
                    default:
                        break;
                }
            else
                switch (_scenarioContext.StepContext.StepInfo.StepDefinitionType)
                {
                    case StepDefinitionType.Given:
                        _scenario.CreateNode<Given>(_scenarioContext.StepContext.StepInfo.Text).Fail();
                        break;
                    case StepDefinitionType.When:
                        _scenario.CreateNode<When>(_scenarioContext.StepContext.StepInfo.Text).Fail();
                        break;
                    case StepDefinitionType.Then:
                        _scenario.CreateNode<Then>(_scenarioContext.StepContext.StepInfo.Text).Fail();
                        break;
                    default:
                        break;
                }

            // Include request and response details if they are available
            if (!string.IsNullOrEmpty(requestDetails))
            {
                _scenario.CreateNode<And>("Request and Response Details").Info(requestDetails + "\n" + responseDetails);
                requestDetails = null; // Reset the request details after including them in the report
            }
        }


        [AfterTestRun]
        public static void CloseExtentReports()
        {
            _extentReports.Flush();
        }


        public static void SetCategoryInfo(int id, string name) // Update this method
        {
            categoryId = id;
            categoryName = name;
        }

        public static void AddRequestAndResponseDetails(string requestDetails, string responseDetails)
        {
            // Get the current scenario node
            ExtentTest scenarioNode = _scenario;

            // Add request and response details to the scenario node
            scenarioNode.Info(requestDetails);
            scenarioNode.Info(responseDetails);
        }

        public static int GetCategoryId()
        {
            return categoryId;
        }

        public static string GetCategoryName() // Add this method
        {
            return categoryName;
        }
    }
}
