using Newtonsoft.Json.Linq;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ApiTests
{
    [TestFixture]
    public class CategoryTests : IDisposable
    {
        private RestClient client;
        private string token;
        private Random random;
        private string name;

        [SetUp]
        public void Setup()
        {
            client = new RestClient(GlobalConstants.BaseUrl);
            token = GlobalConstants.AuthenticateUser("john.doe@example.com", "password123");

            Assert.That(token, Is.Not.Null.Or.Empty, "Authentication token should not be null or empty");
            random = new Random();
        }

        [Test]
        public void Test_CategoryLifecycle_RecipeBook()
        {
            // Step 1: Create a new category
            name = $"categoryName_{random.Next(999, 9999)}";
            var createRequst = new RestRequest("/category", Method.Post);
            createRequst.AddHeader("Authorization", $"Bearer {token}");
            createRequst.AddJsonBody(new { name });

            var createResponse = client.Execute(createRequst);

            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var createdCategory = JObject.Parse(createResponse.Content);

            Assert.That(createdCategory["_id"]?.ToString(), Is.Not.Null.Or.Empty);

            // Step 2: Get all categories and verify new category is included
            var getAllCategoriesRequest = new RestRequest("/category", Method.Get);

            var getAllResponse = client.Execute(getAllCategoriesRequest);

            Assert.Multiple(() =>
            {
                Assert.That(getAllResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(getAllResponse.Content, Is.Not.Null.Or.Empty);

                var categories = JArray.Parse(getAllResponse.Content);

                Assert.That(categories?.Type, Is.EqualTo(JTokenType.Array));
                Assert.That(categories.Count(), Is.GreaterThan(0));
            });

            // Step 3: Get category by ID
            var categoryId = createdCategory["_id"]?.ToString();

            var getByIdRequest = new RestRequest($"/category/{categoryId}", Method.Get);

            var getByIdResponse = client.Execute(getByIdRequest);

            Assert.Multiple(() =>
            {
                Assert.That(getByIdResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(getByIdResponse.Content, Is.Not.Null.Or.Empty);

                var categoryById = JObject.Parse(getByIdResponse.Content);

                Assert.That(categoryById["_id"]?.ToString(), Is.EqualTo(categoryId));
                Assert.That(categoryById["name"]?.ToString(), Is.EqualTo(name));
            });

            // Step 4: Edit the category 
            var editCategoryRequest = new RestRequest($"/category/{categoryId}", Method.Put);
            editCategoryRequest.AddHeader("Authorization", $"Bearer {token}");
            name = name + "_updated";
            editCategoryRequest.AddJsonBody(new { name });

            var editCategoryResponse = client.Execute(editCategoryRequest);

            Assert.That(editCategoryResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // Step 5: Verify update
            var getUpdatedCategory = new RestRequest($"/category/{categoryId}", Method.Get);

            var verifyResponse = client.Execute(getUpdatedCategory);

            Assert.That(verifyResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(verifyResponse.Content, Is.Not.Null.Or.Empty);

            var verifyCategory = JObject.Parse(verifyResponse.Content);

            Assert.That(verifyCategory["name"]?.ToString(), Is.EqualTo(name));

            // Step 6: Delete the category
            var deleteRequest = new RestRequest($"/category/{categoryId}", Method.Delete);
            deleteRequest.AddHeader("Authorization", $"Bearer {token}");

            var deleteResponse = client.Execute(deleteRequest);

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // Step 7: Verify category is deleted
            var verifyDeleteRequest = new RestRequest($"/category/{categoryId}", Method.Get);

            var verifyDeleteResponse = client.Execute(verifyDeleteRequest);

            Assert.That(verifyDeleteResponse.Content, Is.EqualTo("null"));
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
