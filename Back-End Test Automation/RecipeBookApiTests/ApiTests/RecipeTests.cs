using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace ApiTests
{
    [TestFixture]
    public class RecipeTests : IDisposable
    {
        private RestClient client;
        private string token;
        private Random random;
        private string title;

        [SetUp]
        public void Setup()
        {
            client = new RestClient(GlobalConstants.BaseUrl);
            token = GlobalConstants.AuthenticateUser("john.doe@example.com", "password123");

            Assert.That(token, Is.Not.Null.Or.Empty, "Authentication token should not be null or empty");
            random = new Random();
        }

        [Test, Order(1)]
        public void Test_GetAllRecipes()
        {
            //Arrange
            var request = new RestRequest("/recipe", Method.Get);

            //Act
            var response = client.Execute(request);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                    "The response does not have the correct status code");

                Assert.That(response.Content, Is.Not.Null.Or.Empty,
                    "Response body/content is not as expected");

                var recipies = JArray.Parse(response.Content);

                Assert.That(recipies.Type, Is.EqualTo(JTokenType.Array),
                    "The response content is not array");

                Assert.That(recipies.Count, Is.GreaterThan(0),
                    "Recipies count is below 1");

                foreach (var recipe in recipies)
                {
                    Assert.That(recipe["title"]?.ToString(), Is.Not.Null.Or.Empty,
                        "Title is not as expected");

                    Assert.That(recipe["ingredients"], Is.Not.Null.Or.Empty,
                        "Ingredients is not as expected");

                    Assert.That(recipe["instructions"], Is.Not.Null.Or.Empty,
                        "Instructions is not as expected");

                    Assert.That(recipe["cookingTime"], Is.Not.Null.Or.Empty,
                        "cookingTme is not as expected");

                    Assert.That(recipe["servings"], Is.Not.Null.Or.Empty,
                        "servings is not as expected");

                    Assert.That(recipe["category"], Is.Not.Null.Or.Empty,
                        "category is not as expected");

                    Assert.That(recipe["ingredients"]?.Type, Is.EqualTo
                        (JTokenType.Array), "ingredients does not have the correct type");

                    Assert.That(recipe["instructions"]?.Type, Is.EqualTo
                        (JTokenType.Array), "instructions does not have the correct type");
                }
            });
        }

        [Test, Order(2)]
        public void Test_GetRecipeByTitle()
        {
            //Arrange
            //Get request for all recipes
            var expectedCookingTime = 25;
            var expectedServings = 24;
            var expectedIngredientsCount = 9;
            var expectedInstructionsCount = 7;
            var titleToGet = "Chocolate Chip Cookies";
            var getRequest = new RestRequest("/recipe", Method.Get);

            //Act
            var response = client.Execute(getRequest);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo
                    (HttpStatusCode.OK), "Response code is not correct");

                Assert.That(response.Content, Is.Not.Null.Or.Empty,
                    "Response content is not as expected");

                var recipies = JArray.Parse(response.Content);
                var recipe = recipies.FirstOrDefault(r => r["title"]?.ToString() == titleToGet);

                Assert.That(recipe, Is.Not.Null, $"Recipe with title {titleToGet} does not exist");
                Assert.That(recipe["cookingTime"].Value<int>(), Is.EqualTo(expectedCookingTime),
                    "cookingTime is not as expected");

                Assert.That(recipe["servings"].Value<int>(), Is.EqualTo(expectedServings),
                    "cookingTime is not as expected");

                Assert.That(recipe["ingredients"].Count(), Is.EqualTo(expectedIngredientsCount),
                    "ingredients count is not as expected");

                Assert.That(recipe["instructions"].Count(), Is.EqualTo(expectedInstructionsCount),
                    "instructions count is not as expected");
            });
        }

        [Test, Order(3)]
        public void Test_AddRecipe()
        {
            //Arrange
            //Get All categories
            var getAllCategories = new RestRequest("/category", Method.Get);

            var getAllCategoriesResponse = client.Execute(getAllCategories);

            Assert.Multiple(() =>
            {
                Assert.That(getAllCategoriesResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                    "get categories status code is not as expected");

                Assert.That(getAllCategoriesResponse.Content, Is.Not.Null.Or.Empty,
                    "response content is not as expected");
            });

            var categories = JArray.Parse(getAllCategoriesResponse.Content);

            //Extract the first category id
            var categoryId = categories.First()["_id"]?.ToString();

            //Create request for creating recipe
            //Arrange
            var createRecipeRequest = new RestRequest("/recipe", Method.Post);
            createRecipeRequest.AddHeader("Authorization", $"Bearer {token}");
            title = $"recipeTitle_{random.Next(999, 9999)}";
            var cookingTime = 50;
            var servings = 4;
            var ingredients = new[] { new { name = "Test", quantity = "10g" } };
            var instructions = new[] { new { step = "test" } };

            createRecipeRequest.AddBody(new
            {
                title = title,
                cookingTime,
                servings,
                ingredients,
                instructions,
                category = categoryId,
            });

            //Act
            var addResponse = client.Execute(createRecipeRequest);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(addResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(addResponse.Content, Is.Not.Null.Or.Empty);
            });

            var recipe = JObject.Parse(addResponse.Content);
            var recipeId = recipe["_id"]?.ToString();

            //Get request for getting by id
            var getbyIdRequest = new RestRequest($"/recipe/{recipeId}", Method.Get);

            var getbyIdResponse = client.Execute(getbyIdRequest);

            Assert.Multiple(() =>
            {
                Assert.That(getbyIdResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(getbyIdResponse.Content, Is.Not.Null.Or.Empty);

                var createdRecipe = JObject.Parse(getbyIdResponse.Content);

                Assert.That(createdRecipe["title"].ToString(), Is.EqualTo(title));

                Assert.That(createdRecipe["cookingTime"].Value<int>, Is.EqualTo(cookingTime));

                Assert.That(createdRecipe["servings"].Value<int>, Is.EqualTo(servings));

                Assert.That(createdRecipe["category"]?["_id"]?.ToString(), Is.EqualTo(categoryId));

                //Asserts for ingredients
                Assert.That(createdRecipe["ingredients"].Type, Is.EqualTo(JTokenType.Array));

                Assert.That(createdRecipe["ingredients"].Count(), Is.EqualTo(ingredients.Count()));

                Assert.That(createdRecipe["ingredients"]?[0]["name"]?.ToString(),
                    Is.EqualTo(ingredients[0].name));

                Assert.That(createdRecipe["ingredients"]?[0]["quantity"]?.ToString(),
                    Is.EqualTo(ingredients[0].quantity));

                //Asserts for instructions
                Assert.That(createdRecipe["instructions"].Type, Is.EqualTo(JTokenType.Array));

                Assert.That(createdRecipe["instructions"].Count(), Is.EqualTo(instructions.Count()));

                Assert.That(createdRecipe["instructions"]?[0]["step"]?.ToString(),
                    Is.EqualTo(instructions[0].step));
            });
        }

        [Test, Order(4)]
        public void Test_UpdateRecipe()
        {
            //Arrange
            //Get by title
            var getRequest = new RestRequest("/recipe", Method.Get);

            //Act
            var getResponse = client.Execute(getRequest);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(getResponse.StatusCode, Is.EqualTo
                    (HttpStatusCode.OK), "Response code is not correct");

                Assert.That(getResponse.Content, Is.Not.Null.Or.Empty,
                    "Response content is not as expected");
            });

            var recipies = JArray.Parse(getResponse.Content);
            var recipe = recipies.FirstOrDefault(r => r["title"]?.ToString() == title);

            Assert.That(recipe, Is.Not.Null, $"Recipe with title {title} does not exist");

            var recipeId = recipe["_id"].ToString();

            //Create update request
            var updateRequest = new RestRequest($"/recipe/{recipeId}", Method.Put);
            updateRequest.AddHeader("Authorization", $"Bearer {token}");
            title = title + "updated";
            var updatedServings = 30;
            updateRequest.AddJsonBody(new 
            {
                title = title,
                servings = updatedServings
            });

            //Act
            var updateResponse = client.Execute(updateRequest);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                    "The response does not have the correct status code");

                Assert.That(updateResponse.Content, Is.Not.Null.Or.Empty,
                    "Response body/content is not as expected");

                var updatedRecipe = JObject.Parse(updateResponse.Content);

                Assert.That(updatedRecipe["title"]?.ToString(), Is.EqualTo(title));

                Assert.That(updatedRecipe["servings"]?.Value<int>(), Is.EqualTo(updatedServings));
            });
        }

        [Test, Order(5)]
        public void Test_DeleteRecipe()
        {
            //Arrange
            //Get by title
            var getRequest = new RestRequest("/recipe", Method.Get);

            //Act
            var getResponse = client.Execute(getRequest);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(getResponse.StatusCode, Is.EqualTo
                    (HttpStatusCode.OK), "Response code is not correct");

                Assert.That(getResponse.Content, Is.Not.Null.Or.Empty,
                    "Response content is not as expected");
            });

            var recipies = JArray.Parse(getResponse.Content);
            var recipe = recipies.FirstOrDefault(r => r["title"]?.ToString() == title);

            Assert.That(recipe, Is.Not.Null, $"Recipe with title {title} does not exist");

            var recipeId = recipe["_id"].ToString();

            //Create delete request
            var deleteRequest = new RestRequest($"/recipe/{recipeId}", Method.Delete);
            deleteRequest.AddHeader("Authorization", $"Bearer {token}");

            //Act
            var deleteResponse = client.Execute(deleteRequest);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                //Get request by id
                var verifyRequest = new RestRequest($"/recipe/{recipeId}", Method.Get);

                var verifyResponse = client.Execute(verifyRequest);

                Assert.That(verifyResponse.Content, Is.EqualTo("null"));
            });
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
