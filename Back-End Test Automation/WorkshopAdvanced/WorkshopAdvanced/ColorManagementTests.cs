using Newtonsoft.Json.Linq;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Net;

namespace WorkshopAdvanced
{
    [TestFixture]
    public class ColorManagementTests
    {
        private RestClient restClient;
        private string token;
        private Random random;

        [TearDown]
        public void Dispose()
        {
            restClient.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            restClient = new RestClient(GlobalConstants.BaseUrl);
            token = GlobalConstants.AuthenticateUser("admin@gmail.com", "admin@gmail.com");
            random = new Random();
        }

        [Test]
        public void ColorLifecycleTest()
        {
            //Make post request to create a color with random title
            var addColorRequest = new RestRequest("/color", Method.Post);
            addColorRequest.AddHeader("Authorization", $"Bearer {token}");
            addColorRequest.AddJsonBody(new
            {
                title = $"Color_{random.Next(999, 9999)}"
            });

            var addColorResponse = restClient.Execute(addColorRequest);

            Assert.That(addColorResponse.IsSuccessful, Is.True, "The creation of the color failed");

            //Extract color id and make get request by id
            var colorId = JObject.Parse(addColorResponse.Content)["_id"]?.ToString();
            Assert.That(colorId, Is.Not.Null.Or.Empty);

            var getColorRequest = new RestRequest($"/color/{colorId}", Method.Get);

            var getColorResponse = restClient.Execute(getColorRequest);

            Assert.IsTrue(getColorResponse.IsSuccessful);

            //Delete color
            var deleteColorRequest = new RestRequest($"/color/{colorId}", Method.Delete);

            deleteColorRequest.AddHeader("Authorization", $"Bearer {token}");

            var deleteResponse = restClient.Execute(deleteColorRequest);

            Assert.IsTrue(deleteResponse.IsSuccessful);

            //Make get request by it to validate the color is deleted
            var verifyRequest = new RestRequest($"/color/{colorId}", Method.Get);

            var verifyResponse = restClient.Execute(verifyRequest);

            Assert.That(verifyResponse.Content, Is.Null.Or.EqualTo("null"));
        }

        [Test]
        public void ColorLifecycleNegativeTest()
        {
            //Make post request with invalid token
            var invalidToken = "invalidToken";

            var addColorRequest = new RestRequest("/color", Method.Post);
            addColorRequest.AddHeader("Authorization", $"Bearer {invalidToken}");
            addColorRequest.AddJsonBody(new
            {
                title = $"Color_{random.Next(999, 9999)}"
            });

            var addColorResponse = restClient.Execute(addColorRequest);

            Assert.IsFalse(addColorResponse.IsSuccessful, "Expected POST request to fail with invalid token.");
            Assert.That(addColorResponse.StatusCode, Is.EqualTo
               (HttpStatusCode.InternalServerError));

            //Get color with invalid id
            var invalidColorId = "invalidID";
            var getColorRequest = new RestRequest($"/color/{invalidColorId}", Method.Get);

            var getColorResponse = restClient.Execute(getColorRequest);

            Assert.IsFalse(getColorResponse.IsSuccessful);
            Assert.That(getColorResponse.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));

            //Delete color with invalid color id
            var deleteColorRequest = new RestRequest($"/color/{invalidColorId}", Method.Delete);

            deleteColorRequest.AddHeader("Authorization", $"Bearer {invalidToken}");

            var deleteResponse = restClient.Execute(deleteColorRequest);

            Assert.IsFalse(deleteResponse.IsSuccessful,
                "Deleting color should fail with an invalid token");
            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError), 
                "Expected internal server error status when using invalid token to delete color");
        }
    }
}
