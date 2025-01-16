using Newtonsoft.Json.Linq;
using NUnit.Framework.Internal;
using RestSharp;
using System.Net;

namespace ApiTests
{
    [TestFixture]
    public class CategoryTests : IDisposable
    {
        private RestClient client;
        private string token;

        [SetUp]
        public void Setup()
        {
            client = new RestClient(GlobalConstants.BaseUrl);
            token = GlobalConstants.AuthenticateUser("john.doe@example.com", "password123");

            Assert.That(token, Is.Not.Null.Or.Empty, "Authentication token should not be null or empty");
        }

        [Test]
        public void Test_CategoryLifecycle()
        {
            // Step 1: Create a new category
            var createCategoryRequest = new RestRequest("/category", Method.Post);
            createCategoryRequest.AddHeader("Authorization", $"Bearer {token}");
            createCategoryRequest.AddJsonBody(new
            {
                name = "Test Category"
            });

            var createResponse = client.Execute(createCategoryRequest);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Status code is not as expected");

            var createdCategory = JObject.Parse(createResponse.Content);        //достъпваме обекта category
            var categoryId = createdCategory["_id"]?.ToString();                //достъпваме ID category

            Assert.That(categoryId, Is.Not.Null.Or.Empty);

            // Step 2: Get all categories
            var getAllCategoriesRequest = new RestRequest("category", Method.Get);

            var getAllCategoriesResponse = client.Execute(getAllCategoriesRequest);

            Assert.Multiple(() =>
            {
                Assert.That(getAllCategoriesResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                    "Status code is not as expected");

                Assert.That(getAllCategoriesResponse.Content, Is.Not.Empty,
                    "Response content is not as expected");

                var categories = JArray.Parse(getAllCategoriesResponse.Content);

                Assert.That(categories.Type, Is.EqualTo(JTokenType.Array),
                    "Category is not JSON Array");

                Assert.That(categories.Count, Is.GreaterThan(0),
                    "Categories count is empty");

                var createdCategory = categories.FirstOrDefault(c => c
                    ["name"]?.ToString() == "Test Category");

                Assert.That(createdCategory, Is.Not.Null,
                    "Created category is not as expected");
            });

            // Step 3: Get category by ID
            var getCategoryByIdRequest = new RestRequest($"category/{categoryId}", Method.Get);

            var getCategoryByIdResponse = client.Execute(getCategoryByIdRequest);

            Assert.Multiple(() =>
            {
                Assert.That(getCategoryByIdResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                    "Status code is not as expected");

                Assert.That(getCategoryByIdResponse.Content, Is.Not.Empty,
                    "Response content is not as expected");

                var category = JObject.Parse(getCategoryByIdResponse.Content);          //достъпваме обекта

                Assert.That(category["_id"]?.ToString(), Is.EqualTo(categoryId));           //достъпваме пропъртито от обекта
                Assert.That(category["name"]?.ToString(), Is.EqualTo("Test Category"));
            });

            // Step 4: Edit the category
            var editRequest = new RestRequest($"category/{categoryId}", Method.Put);
            editRequest.AddHeader("Authorization", $"Bearer {token}");
            editRequest.AddJsonBody( new 
            {
                name = "Updated Test Category"
            });

            var editResponse = client.Execute(editRequest);

            Assert.That(editResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Status code is not as expected");

            // Step 5: Verify Edit
            var getEditCategoryRequest = new RestRequest($"category/{categoryId}", Method.Get);

            var getEditedCategoryResponse = client.Execute(getEditCategoryRequest);

            Assert.Multiple(() =>
            {
                Assert.That(getEditedCategoryResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                    "Status code is not as expected");

                Assert.That(getEditedCategoryResponse.Content, Is.Not.Empty,
                    "Response content is not as expected");

                var updatedCategoryGet = JObject.Parse(getEditedCategoryResponse.Content);

                Assert.That(updatedCategoryGet["name"]?.ToString(), Is.EqualTo("Updated Test Category"),
                "Updated category name is not as expected");
            });

            // Step 6: Delete the category
            var deleteCategory = new RestRequest($"category/{categoryId}" , Method.Delete);
            deleteCategory.AddHeader("Authorization", $"Bearer {token}");

            var deleteResponse = client.Execute(deleteCategory);

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Status code is not as expected");

            // Step 7: Verify that the deleted category cannot be found
            var verifyDeleteRequest = new RestRequest($"category/{categoryId}" , Method.Get);

            var verifyResponse = client.Execute(verifyDeleteRequest);

            Assert.That(verifyResponse.Content, Is.EqualTo("null"));
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
