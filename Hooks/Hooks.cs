using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;
using relativePathUtility = ProfileStudioAPI.Utilities.RelativePathUtility;
using Serilog.Core;
using Serilog;
using Serilog.Formatting.Json;
using Log = Serilog.Log;
using ProfileStudioAPI.Drivers;
using AventStack.ExtentReports.Gherkin.Model;
using TechTalk.SpecFlow.Bindings;

namespace ExtentReportHooks
{
    [Binding]
    public class Hooks
    {

        public static ExtentReports extent;
        public static ExtentTest feature;
        public static ExtentTest scenario, step;
        public ExtentTest _test;
        public ScenarioContext _scenarioContext;


        public string requestJson;
        public string responseJson;

        public string requestUrl; // Add this variable

        public void SetRequestUrl(string url) // Add this method
        {
            requestUrl = url;
        }

        public void SetRequestJson(string json)
        {
            requestJson = json;
        }

        public void SetResponseJson(string json)
        {
            responseJson = json;
        }

        public void ReportLog(string message)
        {
            if (_test != null)
            {
                _test.Info(message);
            }
            Log.Information(message);
            Console.WriteLine(message);
        }


        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {

            string reportpath = Path.Combine(relativePathUtility.AssemblyDirectory, @"..\..\..\Reports\" + "ProfileStudioAPI_" + DateTime.Now.ToString("yyyyMMd-HHmmss") + "\\");
            extent = new ExtentReports();

            ExtentHtmlReporter htmlReporter = new ExtentHtmlReporter(reportpath);
            htmlReporter.Config.Theme = Theme.Dark;
            htmlReporter.Config.DocumentTitle = "ProfileStudio_API Test Case Report";
            htmlReporter.Config.ReportName = "ProfileStudio_API Test Case Report";
            extent.AttachReporter(htmlReporter);

            LoggingLevelSwitch levelSwitch = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Debug);
            Log.Logger = new LoggerConfiguration().MinimumLevel.ControlledBy(levelSwitch).
                WriteTo.File(new JsonFormatter(), reportpath + @"..\..\Logs\ProfileStudiologfile_",
                rollingInterval: RollingInterval.Day).CreateLogger();        // generate logs in json format

        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext context)
        {
            feature = extent.CreateTest(context.FeatureInfo.Title);
            Log.Information("Select feature file {0} to run", context.FeatureInfo.Title);
        }

        [BeforeScenario]
        public void BeforeScenario(ScenarioContext context)
        {
            scenario = feature.CreateNode(context.ScenarioInfo.Title);
            _test = scenario;
            Log.Information("Select scenario {0} to run", context.ScenarioInfo.Title);
        }

        [BeforeStep]
        public void BeforeStep()
        {
            step = scenario; // Initialize step if it's null
        }
        [AfterStep]
        public void AfterStep()
        {
            if (step != null)
            {
                if (_scenarioContext.TestError == null)
                {
                    var stepInfo = _scenarioContext.StepContext.StepInfo;
                    switch (stepInfo.StepDefinitionType)
                    {
                        case StepDefinitionType.Given:
                            step = scenario.CreateNode<Given>(stepInfo.Text);
                            break;
                        case StepDefinitionType.When:
                            scenario.CreateNode<When>(stepInfo.Text);
                            break;
                        case StepDefinitionType.Then:
                            scenario.CreateNode<Then>(stepInfo.Text);
                            break;
                        default:
                            break;
                    }
                    LogRequestAndResponse();
                }
                else
                {
                    var stepInfo = _scenarioContext.StepContext.StepInfo;
                    switch (stepInfo.StepDefinitionType)
                    {
                        case StepDefinitionType.Given:
                            step = scenario.CreateNode<Given>(stepInfo.Text).Fail(_scenarioContext.TestError.Message);
                            break;
                        case StepDefinitionType.When:
                            step = scenario.CreateNode<When>(stepInfo.Text).Fail(_scenarioContext.TestError.Message);
                            break;
                        case StepDefinitionType.Then:
                            step = scenario.CreateNode<Then>(stepInfo.Text).Fail(_scenarioContext.TestError.Message);
                            break;
                        default:
                            break;
                    }
                }
            }
            step = null;
        }

        private void LogRequestAndResponse()
        {
            step.Info("Request JSON: " + requestJson);
            step.Info("Response JSON: " + responseJson);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            //TODO: implement logic that has to run after executing each scenario
        }

        [AfterFeature]
        public static void AfterFeature()
        {
            extent.Flush();
        }
    }
}
