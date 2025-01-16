using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace ApiTests
{
    [TestFixture]
    public class DestinationTests : IDisposable
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
        public void Test_GetAllDestinations()
        {
            //Arrange
            var getRequest = new RestRequest("destination", Method.Get);

            //Act
            var getResponse = client.Execute(getRequest);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(getResponse.StatusCode, Is.EqualTo
                    (HttpStatusCode.OK), "Expected status code is not OK");

                Assert.That(getResponse.Content, Is.Not.Null.Or.Empty,
                    "Response content is null or empty");

                var destinations = JArray.Parse(getResponse.Content);

                Assert.That(destinations.Type, Is.EqualTo(JTokenType.Array),      //проверяваме че сървъра връща дестинацийте в JSON Array формат
                    "The response content is not JSON Array");

                Assert.That(destinations.Count, Is.GreaterThan(0),
                    "Expected destination is less than zero");

                foreach (var dest in destinations)
                {
                    Assert.That(dest["name"]?.ToString(), Is.Not.Null.Or.Empty,
                        "Property name is not as expected");

                    Assert.That(dest["location"]?.ToString(), Is.Not.Null.Or.Empty,
                        "Property location is not as expected");

                    Assert.That(dest["description"]?.ToString(), Is.Not.Null.Or.Empty,
                        "Property description is not as expected");

                    Assert.That(dest["category"]?.ToString(), Is.Not.Null.Or.Empty,
                        "Property category is not as expected");

                    Assert.That(dest["attractions"]?.Type, Is.EqualTo(JTokenType.Array),
                        "Attraction property is not array");

                    Assert.That(dest["bestTimeToVisit"]?.ToString(), Is.Not.Null.Or.Empty,
                        "Property bestTimeToVisit is not as expected");
                }
            });

        }

        [Test]
        public void Test_GetDestinationByName()
        {
            //Arrange
            var getRequest = new RestRequest("destination", Method.Get);

            //Act
            var getResponse = client.Execute(getRequest);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK)
                    , "Response status code is not as expected");

                Assert.That(getResponse.Content, Is.Not.Null.Or.Empty
                    , "Response content is not as expected");

                var destinations = JArray.Parse(getResponse.Content);
                var destination = destinations.FirstOrDefault(d => d["name"]?.ToString() == "New York City");

                Assert.That(destination["location"]?.ToString(), Is.EqualTo("New York, USA"),
                    "Location property does not have the correct value");

                Assert.That(destination["description"]?.ToString(),
                    Is.EqualTo("The largest city in the USA, known for its skyscrapers, culture, and entertainment."),
                    "Description property does not have the correct value");
            });
        }

        [Test]
        public void Test_AddDestination()
        {
            //Arrange
            //Get all categories and extract first category id
            var getCategoriesRequest = new RestRequest("category", Method.Get);

            var getCategoriesResponse = client.Execute(getCategoriesRequest);

            var categories = JArray.Parse(getCategoriesResponse.Content);
            var firstCategory = categories.First();
            var categoryId = firstCategory["_id"]?.ToString();

            //Create new destination
            var addRequst = new RestRequest("destination", Method.Post);
            addRequst.AddHeader("Authorization", $"Bearer {token}");
            var name = "Random name";
            var location = "New location";
            var description = "New description";
            var bestTimeToVisit = "April";
            var attractions = new[] { "Attraction1", "Attraction2", "Attraction3" };
            addRequst.AddJsonBody(new
            {
                name,
                location,
                description,
                bestTimeToVisit,
                attractions,
                category = categoryId
            });

            //Act
            var addResponse = client.Execute(addRequst);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(addResponse.StatusCode, Is.EqualTo
                    (HttpStatusCode.OK), "Response status code is not as expected");

                Assert.That(addResponse.Content, Is.Not.Empty.Or.Null,
                    "Response content is not as expected");
            });

            var createdDestination = JObject.Parse(addResponse.Content);
            Assert.That(createdDestination["_id"]?.ToString(), Is.Not.Empty.Or.Null,
                "Property id is not as expected");

            var createdDestinationId = createdDestination["_id"]?.ToString();

            //Get destination by id
            var getDestinationRequest = new RestRequest($"/destination/{createdDestinationId}", Method.Get);

            var getResponse = client.Execute(getDestinationRequest);

            Assert.Multiple(() =>
            {
                Assert.That(getResponse.StatusCode, Is.EqualTo
                    (HttpStatusCode.OK), "Response status code is not as expected");

                Assert.That(getResponse.Content, Is.Not.Empty.Or.Null,
                    "Response content is not as expected");

                var destination = JObject.Parse(getResponse.Content);

                Assert.That(destination["name"]?.ToString(), Is.EqualTo(name));
                Assert.That(destination["location"]?.ToString(), Is.EqualTo(location));
                Assert.That(destination["description"]?.ToString(), Is.EqualTo(description));
                Assert.That(destination["bestTimeToVisit"]?.ToString(), Is.EqualTo(bestTimeToVisit));

                Assert.That(destination["category"]?.ToString(), Is.Not.Empty.Or.Null);
                Assert.That(destination["category"]["_id"]?.ToString(), Is.EqualTo(categoryId));

                Assert.That(destination["attractions"].Count, Is.EqualTo(3));
                Assert.That(destination["attractions"]?.Type, Is.EqualTo(JTokenType.Array));

                Assert.That(destination["attractions"][0]?.ToString(), Is.EqualTo("Attraction1"));
                Assert.That(destination["attractions"][1]?.ToString(), Is.EqualTo("Attraction2"));
                Assert.That(destination["attractions"][2]?.ToString(), Is.EqualTo("Attraction3"));
            });

        }

        [Test]
        public void Test_UpdateDestination()
        {
            //Arrange
            //Get all destinations and extract with name Machu Picchu
            var getRequest = new RestRequest("destination", Method.Get);
            var getResponse = client.Execute(getRequest);

            Assert.That(getResponse.StatusCode, Is.EqualTo
                   (HttpStatusCode.OK), "Expected status code is not as expected");

            Assert.That(getResponse.Content, Is.Not.Null.Or.Empty,
                "Response content is not as expected");

            var destinations = JArray.Parse(getResponse.Content);
            var destinationToUpdate = destinations.FirstOrDefault
                (d => d["name"]?.ToString() == "Machu Picchu");

            Assert.That(destinationToUpdate, Is.Not.Null);

            var destinationId = destinationToUpdate["_id"]?.ToString();

            //Create update request
            var updateRequest = new RestRequest($"destination/{destinationId}", Method.Put);
            updateRequest.AddHeader("Authorization", $"Bearer {token}");
            updateRequest.AddJsonBody(new
            {
                name = "UpdatedName",
                bestTimeToVisit = "Winter"
            });

            //Act
            var updateResponse = client.Execute(updateRequest);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(updateResponse.StatusCode, Is.EqualTo
                   (HttpStatusCode.OK), "Expected status code is not as expected");

                Assert.That(updateResponse.Content, Is.Not.Null.Or.Empty,
                    "Response content is not as expected");

                var updatedDestination = JObject.Parse(updateResponse.Content);

                Assert.That(updatedDestination["name"]?.ToString(), Is.EqualTo("UpdatedName"));
                Assert.That(updatedDestination["bestTimeToVisit"]?.ToString(), Is.EqualTo("Winter"));
            });
        }

        [Test]
        public void Test_DeleteDestination()
        {
            //Arrange
            //Get all destinations and extract with name Yellowstone National Park
            var getRequest = new RestRequest("destination", Method.Get);
            var getResponse = client.Execute(getRequest);

            Assert.That(getResponse.StatusCode, Is.EqualTo
                   (HttpStatusCode.OK), "Expected status code is not as expected");

            Assert.That(getResponse.Content, Is.Not.Null.Or.Empty,
                "Response content is not as expected");

            var destinations = JArray.Parse(getResponse.Content);
            var destinationToDelete = destinations.FirstOrDefault
                (d => d["name"]?.ToString() == "Yellowstone National Park");

            Assert.That(destinationToDelete, Is.Not.Null);

            var destinationId = destinationToDelete["_id"]?.ToString();

            //Create delete request
            var deleteRequest = new RestRequest($"destination/{destinationId}", Method.Delete);
            deleteRequest.AddHeader("Authorization", $"Bearer {token}");

            //Act
            var deleteResponse = client.Execute(deleteRequest);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(deleteResponse.StatusCode, Is.EqualTo
                        (HttpStatusCode.OK), "Expected status code is not OK");

                //Get request to get the destination that we deleted
                var verifyRequest = new RestRequest($"destination/{destinationId}", Method.Get);

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
