using System;

namespace ProfileStudioAPI.StepDefinitions
{
    public static class CategoryTestDataGenerator
    {
        public static string GenerateRandomCategoryName()
        {
            string baseName = "Category_";
            int randomNumber = new Random().Next(99999, 999999999);
            return $"{baseName}{randomNumber}";
        }

        public static int GenerateRandomCategoryId()
        {
            int randomNumber = new Random().Next(99999, 999999999);
            return randomNumber;
        }

    }
}