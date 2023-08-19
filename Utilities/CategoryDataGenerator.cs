using AventStack.ExtentReports.Model;
using Newtonsoft.Json;

namespace ProfileStudioAPI.Utilities
{
    public static class CategoryDataGenerator
    {
        public static string GenerateCategoryData(string categoryName, int categoryId)
        {
            var categoryData = new
            {
                name = categoryName,
                categoryId = categoryId
            };

            return JsonConvert.SerializeObject(categoryData);
        }

        public static string GenerateUpdatedCategoryData(string UpdatedcategoryName, int categoryId)
        {
            var updatedCategoryData = new
            {
                name = UpdatedcategoryName,
                categoryId = categoryId
            };

            return JsonConvert.SerializeObject(updatedCategoryData);
        }
    }
       

}