using NUnit.Framework;
using RestSharp;
using Newtonsoft.Json;
using ProfileStudioAPI.PageObjects;
using ProfileStudioAPI.Drivers;
using System.Net;
using Newtonsoft.Json.Linq;
using ProfileStudioAPI.support;
using AventStack.ExtentReports;
using Serilog;
using AventStack.ExtentReports.Gherkin.Model;
using ExtentReportHooks;
using ProfileStudioAPI.Utilities;

namespace ProfileStudioAPI.StepDefinitions
{
    [Binding]
    public class CategoryE2E
    {
        public RestClient restClient;
        private readonly Hooks _extentReport;
        public RestRequest restRequest;
        public RestResponse restResponse;
        public CategoryPostData postData;
        public string categoryName;
        public int categoryId;
        public string categoryData;
        string apiUrl = ApiUrl.BaseUrl;
        public string updatedCategoryData;
        public string updatedName;
        public string requestDetails;
        public string responseDetails;
        public static ExtentReports extent;
        public static ExtentTest feature;
        public static ExtentTest scenario, step;
        public ScenarioContext _scenarioContext;
        public string requestJson;
        public string responseJson;

        public CategoryE2E(Hooks extentReport)
        {
            _extentReport = extentReport;
            restClient = new RestClient(ApiUrl.BaseUrl);
        }

        public CategoryE2E()
        {
            restClient = new RestClient(ApiUrl.BaseUrl);
        }

        // Do a POST request and validate the ID and Name

        [Given(@"the user sends the post request with url")]
        public void GivenTheUserSendsThePostRequestWithUrl()
        {
            try
            {
                categoryName = TestDataGenerator.GenerateRandomCategoryName();
                categoryId = TestDataGenerator.GenerateRandomCategoryId();

                categoryData = CategoryRequestBuilder.GenerateCategoryData(categoryName, categoryId);

                var request = CategoryRequestBuilder.CategoryPostRequest(categoryData);

                requestJson = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody)?.Value?.ToString();
                _extentReport.SetRequestJson(requestJson);

                restResponse = (RestResponse)restClient.Execute(request);

                responseJson = restResponse.Content;
                _extentReport.SetResponseJson(responseJson);

                JObject obs = JObject.Parse(restResponse.Content);
                Assert.That(obs["name"].ToString(), Is.EqualTo(categoryName), "Category name is not matching");

                postData = JsonConvert.DeserializeObject<CategoryPostData>(restResponse.Content);

                ScenarioPassingMethods.SetCategoryInfo(postData.categoryId, postData.name);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during POST request: " + ex.Message);
                throw;
            }
        }

            [Then(@"user gets a success response")]
            public void ThenUserGetsASuccessResponse()
            {
                Assert.True(restResponse.IsSuccessful);
                _extentReport.ReportLog("The response indicates success (HTTP 201 Created).");
                Assert.AreEqual(HttpStatusCode.Created, restResponse.StatusCode);


            }

            [Then(@"the response contains the correct name")]
            public void ThenTheResponseContainsTheCorrectNameAndID()
            {
                Assert.AreEqual(postData.name, categoryName);
                Assert.AreEqual(postData.categoryId, categoryId);

            }


        // Do an GET to validate the information

        [Given(@"the user sends a GET request with the category ID")]
        public void GivenTheUserSendsAGETRequestWithTheCategoryID()
        {
            try
            {
                int categoryId = ScenarioPassingMethods.GetCategoryId();
                var request = CategoryRequestBuilder.CategoryGetRequest(apiUrl, categoryId);
                
                restResponse = (RestResponse)restClient.Execute(request);
                responseJson = restResponse.Content;
                _extentReport.SetResponseJson(responseJson);

                Assert.True(restResponse.IsSuccessful);
                Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during GET request: " + ex.Message);
                throw;
            }
        }

        [Then(@"the Get response contains the correct name and category ID")]
        public void ThenTheGetResponseContainsTheCorrectNameAndID()
        {
            string categoryName = ScenarioPassingMethods.GetCategoryName();
            var obs = JObject.Parse(restResponse.Content);
            Assert.That(obs["name"].ToString(), Is.EqualTo(categoryName), "Category name is not matching");
        }


        // Update the name and GET the ID to validate

        [Given(@"the user sends a PUT request with the category ID")]
        public void GivenTheUserSendsAPUTRequestWithTheCategoryID()
        {
            try
            {
                categoryId = ScenarioPassingMethods.GetCategoryId();
                updatedName = TestDataGenerator.GenerateRandomCategoryName();

                categoryData = CategoryRequestBuilder.GenerateUpdatedCategoryData(updatedName, categoryId);

                var request = CategoryRequestBuilder.CategoryPutRequest(categoryData);
                request.Resource += "/" + categoryId;
                requestJson = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody)?.Value?.ToString();
                _extentReport.SetRequestJson(requestJson);

                restResponse = (RestResponse)restClient.Execute(request);

                Assert.True(restResponse.IsSuccessful);
                Assert.AreEqual(HttpStatusCode.NoContent, restResponse.StatusCode);

                ScenarioPassingMethods.SetCategoryInfo(categoryId, updatedName); // Use categoryName here

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during PUT request: " + ex.Message);
                throw;
            }
        }

        [Then(@"the user does a GET response after update by ID and validates the name")]
        public void ThenTheUserDoesAGETResponseAfterUpdateByIDAndValidatesTheName()
        {
            try
            {
                int categoryId = ScenarioPassingMethods.GetCategoryId();
                string updatedName = ScenarioPassingMethods.GetCategoryName();

                var request = CategoryRequestBuilder.CategoryGetRequest(apiUrl, categoryId);

                restResponse = (RestResponse)restClient.Execute(request);
                responseJson = restResponse.Content;
                _extentReport.SetResponseJson(responseJson);

                Assert.True(restResponse.IsSuccessful);
                Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode);

                var obs = JObject.Parse(restResponse.Content);
                Assert.That(obs["name"].ToString(), Is.EqualTo(updatedName), "Category name is not matching");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during GET request: " + ex.Message);
                throw;
            }
        }

        //DELETE and GET the ID to validate

        [Given(@"the user sends a Delete request with the category ID and do an GET to confirm")]
        public void GivenTheUserSendsADeleteRequestWithTheCategoryIDAndDoAnGETToConfirm()
        {
            int categoryId = ScenarioPassingMethods.GetCategoryId();

            restRequest = CategoryRequestBuilder.CategoryDeleteRequest(apiUrl, categoryId);

            restResponse = (RestResponse)restClient.Execute(restRequest);

            Assert.True(restResponse.IsSuccessful);
            Assert.AreEqual(HttpStatusCode.NoContent, restResponse.StatusCode);

            restRequest = CategoryRequestBuilder.CategoryGetRequest(apiUrl, categoryId);

            restResponse = (RestResponse)restClient.Execute(restRequest);

            Assert.True(restResponse.IsSuccessful);
            Assert.AreEqual(HttpStatusCode.NoContent, restResponse.StatusCode);


        }

        // NOT OK Case - DELETE the non exisitng Category

        [Given(@"the user sends a Delete request with the non exisiting category ID  and validate")]
        public void GivenTheUserSendsADeleteRequestWithTheNonExisitingCategoryIDAndValidate()
        {
            int categoryId = ScenarioPassingMethods.GetCategoryId();

            restRequest = CategoryRequestBuilder.CategoryDeleteRequest(apiUrl, categoryId);
           
            restResponse = (RestResponse)restClient.Execute(restRequest);
            responseJson = restResponse.Content;
            _extentReport.SetResponseJson(responseJson);

            Assert.False(restResponse.IsSuccessful);
            Assert.AreEqual(HttpStatusCode.NotFound, restResponse.StatusCode);
        }

        // NOT OK Case - CREATE a Cateogry using an exisitng Category name and Validate

        [Given(@"the user sends a Pose request with the an exisiting category Name and validate")]
        public void GivenTheUserSendsAPoseRequestWithTheAnExisitingCategoryNameAndValidate()
        {
            try
            {
                categoryName = "Schneider Electric";
                categoryId = TestDataGenerator.GenerateRandomCategoryId();

                categoryData = CategoryRequestBuilder.GenerateCategoryData(categoryName, categoryId);

                var request = CategoryRequestBuilder.CategoryPostRequest(categoryData);

                requestJson = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody)?.Value?.ToString();
                _extentReport.SetRequestJson(requestJson);

                restResponse = (RestResponse)restClient.Execute(request);

                //responseJson = restResponse.Content;
                //_extentReport.SetResponseJson(responseJson);

                Assert.AreEqual(HttpStatusCode.Conflict, restResponse.StatusCode);

                JObject responseJson = JObject.Parse(restResponse.Content);

                string errorMessage = (string)responseJson["detail"];
                string expectedErrorMessage = "CATEGORY.ADD_ERROR_DUPLICATE_CURRENT";

                Assert.AreEqual(errorMessage, expectedErrorMessage);


            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during POST request: " + ex.Message);
                throw;
            }
        }


        [Given(@"the user sends a get all request and validate the datatypes")]
        public void GivenTheUserSendsAGetAllRequestAndValidateTheDatatypes()
        {
            try
            {
                var request = CategoryRequestBuilder.CategoryGetAllRequest(apiUrl);

                restResponse = (RestResponse)restClient.Execute(request);
                responseJson = restResponse.Content;
                _extentReport.SetResponseJson(responseJson);


                Assert.True(restResponse.IsSuccessful);
                Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode);

                JArray categoriesArray = JArray.Parse(restResponse.Content);

                foreach (JObject category in categoriesArray)
                {
                    try
                    {
                        int categoryId = (int)category["categoryId"];
                        string name = (string)category["name"];
                        bool locked = (bool)category["locked"];

                        // Validation is successful for this category.
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        Assert.Fail("Validation failed for category.");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during GET request: " + ex.Message);
                throw;
            }
        }




    }
}
