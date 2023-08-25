
namespace ProfileStudioAPI.Utilities
{
    [Binding]
    public class ScenarioPassingMethods
    {
        private static int categoryId;
        private static string categoryName;


        public static void SetCategoryInfo(int id, string name)
        {
            categoryId = id;
            categoryName = name;
        }

        public static int GetCategoryId()
        {
            return categoryId;
        }

        public static string GetCategoryName()
        {
            return categoryName;
        }
    }
}
