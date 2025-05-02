using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace WorkshopAdvanced
{
    [TestFixture]
    public class ProductManagementTests
    {
        private RestClient restClient;
        private string adminToken;
        private string userToken;
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
            adminToken = GlobalConstants.AuthenticateUser("admin@gmail.com", "admin@gmail.com");
            userToken = GlobalConstants.AuthenticateUser("john.doe@example.com", "password123");
            random = new Random();
        }

        [Test]
        public void ProductLifecycleTest()
        {
            //Create poduct with random title
            var productTitle = "Test Product" + random.Next(999, 9999).ToString();
            var createProductRequest = new RestRequest("/product", Method.Post);
            createProductRequest.AddHeader("Authorization", $"Bearer {adminToken}");
            createProductRequest.AddJsonBody(new
            {
                Title = productTitle,
                Description = "This is test product",
                Slug = "test-product",
                Price = 9.99,
                Category = "Electronics",
                Brand = "Apple",
                Quantity = 10
            });

            var createProductResponse = restClient.Execute(createProductRequest);
            Assert.That(createProductResponse.IsSuccessful, Is.True,
                "Product creation failed");

            var productId = JObject.Parse(createProductResponse.Content)["_id"]?.ToString();
            Assert.That(productId, Is.Not.Null.Or.Empty,
                "Product ID should not be null or empty");

            //Get the created product
            var getProductRequest = new RestRequest($"/product/{productId}", Method.Get);
            var getProductResponse = restClient.Execute(getProductRequest);
            Assert.That(getProductResponse.IsSuccessful, Is.True,
                "Failed to retrieve product details");
            Assert.That(getProductResponse.Content, Is.Not.Null.Or.Empty,
                "Product details are null");

            //Update the created product
            var updateProductTitle = "Updated Product" + random.Next(999, 9999).ToString();
            var updateProductRequest = new RestRequest($"/product/{productId}", Method.Put);
            updateProductRequest.AddHeader("Authorization", $"Bearer {adminToken}");
            updateProductRequest.AddJsonBody(new
            {
                Name = updateProductTitle,
                Description = "Updated product description",
                Price = 39.99
            });

            var updateProductResponse = restClient.Execute(updateProductRequest);
            Assert.That(updateProductResponse.IsSuccessful, Is.True, "Product update failed");

            //Delete the created product
            var deleteProductRequest = new RestRequest($"/product/{productId}", Method.Delete);
            deleteProductRequest.AddHeader("Authorization", $"Bearer {adminToken}");
            var deleteProductResponse = restClient.Execute(deleteProductRequest);
            Assert.That(deleteProductResponse.IsSuccessful, Is.True, "Product deletion failed");

            //Verify the product was delete by doing get request
            var verifyDeleteRequest = new RestRequest($"/product/{productId}", Method.Get);
            var verifyDeleteResponse = restClient.Execute(verifyDeleteRequest);
            Assert.That(verifyDeleteResponse.Content, Is.Null.Or.EqualTo("null"),
                "Product still exists after deletion");
        }

        [Test]
        public void productRatingLifecycleTest()
        {
            //Retriving a random product ID from the products list
            var getProductListRequest = new RestRequest("/product", Method.Get);
            var getProductListResponse = restClient.Execute(getProductListRequest);
            Assert.That(getProductListResponse.IsSuccessful, Is.True, "Fail to retrive product list");

            var products = JArray.Parse(getProductListResponse.Content);
            Assert.That(products.Count, Is.GreaterThan(0), "No products found");

            var randomProduct = products[new Random().Next(products.Count)];
            var productId = randomProduct["_id"]?.ToString();
            Assert.That(productId, Is.Not.Null.And.Not.EqualTo("null"),
                "Products Id should not be empty or null");

            //Adding review to randomly selected product
            var addReviewRequest = new RestRequest("/product/rating", Method.Put);
            addReviewRequest.AddHeader("Authorization", $"Bearer {userToken}");
            addReviewRequest.AddJsonBody(new
            {
                star = 5,
                prodId = productId,
                comment = "Very nice product! If my API tests pass!"
            });

            var addReviewResponse = restClient.Execute(addReviewRequest);

            Assert.That(addReviewResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Adding rating failed");

            //Adding wished product to the users wishlist
            var addToWishlistRequest = new RestRequest("/product/wishlist", Method.Put);
            addToWishlistRequest.AddHeader("Authorization", $"Bearer {userToken}");
            addToWishlistRequest.AddJsonBody(new
            {
                prodId = productId
            });

            var addToWishListResponse = restClient.Execute(addToWishlistRequest);

            Assert.That(addToWishListResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), 
                "Adding product to wishlist failed");
        }

        [Test]
        public void complexProductInteractionTests()
        {
            //Fething products and extracting its ID
            var getProductListRequest = new RestRequest("/product", Method.Get);
            var getProductListResponse = restClient.Execute(getProductListRequest);

            Assert.That(getProductListResponse.IsSuccessful, Is.True,
                "Failed to retrive product list");

            var products = JArray.Parse(getProductListResponse.Content);
            Assert.That(products.Count, Is.GreaterThan(0), "No products found");

            var randomProduct = products[new Random().Next(products.Count)];
            var productId = randomProduct["_id"]?.ToString();

            Assert.That(productId, Is.Not.Null.And.Not.Empty,
                "Product id should not be null or empty");

            //Add selected product to the wishlist 
            var addToWishlistRequest = new RestRequest("/product/wishlist", Method.Put);
            addToWishlistRequest.AddHeader("Authorization", $"Bearer {userToken}");
            addToWishlistRequest.AddJsonBody(new
            {
                prodId = productId
            });

            var addToWishListResponse = restClient.Execute(addToWishlistRequest);

            Assert.That(addToWishListResponse.IsSuccessful, Is.True, 
                "Adding product to wishlist failed");

            //Upload new photo for selected product
            var uploadPhotoRequest = new RestRequest($"/product/upload/{productId}", Method.Put);
            uploadPhotoRequest.AddHeader("Authorization", $"Bearer {adminToken}");
            uploadPhotoRequest.AddJsonBody(new
            {
                images = new[]
                {
                    "https://example.com/image1.jpg",
                    "https://example.com/image2.jpg"
                }
            });

            var uploadPhotoResponse = restClient.Execute(uploadPhotoRequest);
            Assert.That(uploadPhotoResponse.IsSuccessful, Is.True,
                "Uploading photo failed");

            //Add 5 star rating and review for selected product
            var addRatingRequest = new RestRequest("/product/rating", Method.Put);
            addRatingRequest.AddHeader("Authorization", $"Bearer {userToken}");
            addRatingRequest.AddJsonBody(new
            {
                star = 5,
                prodId = productId,
                comment = "Excellent product! Test!"
            });

            var addRatingResponse = restClient.Execute(addRatingRequest);
            Assert.That(addRatingResponse.IsSuccessful, Is.True, "Adding rating failed");

            //Remove products from wishlist
            var removeFromWishlistRequest = new RestRequest("/product/wishlist", Method.Put);
            removeFromWishlistRequest.AddHeader("Authorization", $"Bearer {userToken}");
            removeFromWishlistRequest.AddJsonBody(new
            {
                prodId = productId
            });

            var removeFromWishlistResponse = restClient.Execute(removeFromWishlistRequest);
            Assert.That(removeFromWishlistResponse.IsSuccessful, Is.True,
                "Removing product from wishlist fail");
        }
    }
}
