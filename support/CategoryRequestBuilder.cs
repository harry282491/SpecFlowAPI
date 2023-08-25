using Newtonsoft.Json;
using RestSharp;

namespace ProfileStudioAPI.support
{
    public static class CategoryRequestBuilder
    {
        public static string GenerateCategoryData(string categoryName, int categoryId)
        {
            var categoryData = new
            {
                name = categoryName,
                categoryId,
            };

            return JsonConvert.SerializeObject(categoryData);
        }

        public static string GenerateUpdatedCategoryData(string UpdatedcategoryName, int categoryId)
        {
            var updatedCategoryData = new
            {
                name = UpdatedcategoryName,
                categoryId
            };

            return JsonConvert.SerializeObject(updatedCategoryData);
        }

        public static RestRequest CategoryPostRequest(string categoryData)
        {
            var request = new RestRequest("/Category", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(categoryData);
            return request;
        }

        public static RestRequest CategoryGetRequest(string apiUrl, int categoryId)
        {
            var getCategoryByIdUrl = $"{apiUrl}/Category/{categoryId}";
            var request = new RestRequest(getCategoryByIdUrl, Method.Get);
            return request;
        }

        public static RestRequest CategoryPutRequest(string categoryData)
        {
            var request = new RestRequest("/Category", Method.Put);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(categoryData);
            return request;
        }
        public static RestRequest CategoryDeleteRequest(string apiUrl, int categoryId)
        {
            var deleteCategoryByIdUrl = $"{apiUrl}/Category/{categoryId}";
            var request = new RestRequest(deleteCategoryByIdUrl, Method.Delete);
            return request;
        }

        public static RestRequest CategoryGetAllRequest(string apiUrl)
        {
            var getCategoryAllByIdUrl = $"{apiUrl}/Category/";
            var request = new RestRequest(getCategoryAllByIdUrl, Method.Get);
            return request;
        }


    }
}