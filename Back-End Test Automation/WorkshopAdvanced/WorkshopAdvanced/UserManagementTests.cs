using Newtonsoft.Json.Linq;
using RestSharp;

namespace WorkshopAdvanced
{
    [TestFixture]
    public class UserManagementTests
    {
        private RestClient restClient;
        private string adminToken;
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
            random = new Random();
        }

        [Test]
        public void UserLoginTest()
        {
            //Check login, registration and password reset functionalities
            var loginRequest = new RestRequest("/user/login", Method.Post);
            loginRequest.AddJsonBody(new
            {
                email = "john.doe@example.com",
                password = "password123"
            });

            var loginResponse = restClient.Execute(loginRequest);

            Assert.That(loginResponse.IsSuccessful, Is.True, "Login failed");
            Assert.That(loginResponse.Content, Is.Not.Null, "Login response data is null");

            //exctract user token
            string userToken = JObject.Parse(loginResponse.Content)["token"]?.ToString();
            Assert.That(userToken, Is.Not.Null.And.Not.Empty, "Login token is null or empty");

            //Register random user
            var randomUserEmail = $"ivanov{+random.Next(999, 9999)}@example.com";
            var signupRequest = new RestRequest("/user/register", Method.Post);
            signupRequest.AddJsonBody(new
            {
                Firstname = "Ivan" + random.Next(999, 9999),
                Lastname = "Ivanov",
                Password = "password123",
                Mobile = $"+111{random.Next(999, 9999)}",
                Email = randomUserEmail
            });

            var signupResponse = restClient.Execute(signupRequest);
            Assert.That(signupResponse.IsSuccessful, Is.True, "Signup failed");

            //Password reset request
            var forgotPasswordRequest = new RestRequest("/user/forgot-password-token", Method.Post);
            forgotPasswordRequest.AddJsonBody(new
            {
                Email = randomUserEmail
            });

            var forgorPasswordResponse = restClient.Execute(forgotPasswordRequest);

            Assert.That(forgorPasswordResponse.IsSuccessful, Is.True,
                "Forgot password request failed");

            Assert.That(forgorPasswordResponse.Content, Is.Not.Null,
                "Forgot password response data is null");
        }

        [Test]
        public void userSignupLoginUpdateAndDeleteTest()
        {
            //Signup new random user
            var randomUserEmail = $"petrov{+random.Next(999, 9999)}@example.com";
            var signupRequest = new RestRequest("/user/register", Method.Post);
            signupRequest.AddBody(new
            {
                Firstname = "Petar",
                Lastname = "Petrov",
                Password = "password123",
                Mobile = $"+1234567891",
                Email = randomUserEmail
            });

            var signupResponse = restClient.Execute(signupRequest);

            Assert.That(signupResponse.IsSuccessful, Is.True,
                "Signup failed");

            Assert.That(signupResponse.Content, Is.Not.Null,
                "Signup response data is null");

            //login the newly created user
            var loginRequest = new RestRequest("/user/login", Method.Post);
            loginRequest.AddBody(new
            {
                Email = randomUserEmail,
                Password = "password123"
            });

            var loginResponse = restClient.Execute(loginRequest);
            Assert.That(loginResponse.IsSuccessful, Is.True,
                "Login failed");
            Assert.That(loginResponse.Content, Is.Not.Null,
                "Login response data is null");

            //exctract user token and userid
            var userToken = JObject.Parse(loginResponse.Content)["token"]?.ToString();
            Assert.That(userToken, Is.Not.Null.And.Not.Empty, "Login token is null or empty");

            var userId = JObject.Parse(loginResponse.Content)["_id"]?.ToString();
            Assert.That(userId, Is.Not.Null.And.Not.Empty, "User id is null or empty");

            //udpate the created user
            var updatedUserEmail = $"ivanova{random.Next(999, 9999)}@example.com";
            var updateUserRequest = new RestRequest("/user/edit-user", Method.Put);
            updateUserRequest.AddHeader("Authorization", $"Bearer {userToken}");
            updateUserRequest.AddJsonBody(new
            {
                Firstname = "Ivana",
                Lastname = "Ivanova",
                Password = "password123",
                Mobile = $"+123456",
                Email = updatedUserEmail
            });

            var updatedUserResponse = restClient.Execute(updateUserRequest);

            Assert.That(updatedUserResponse.IsSuccessful, Is.True, "User update failed");
            Assert.That(updatedUserResponse.Content, Is.Not.Null.And.Not.Empty, "User update failed");

            //Delete the user
            var deleteUserRequest = new RestRequest($"/user/{userId}", Method.Delete);
            updateUserRequest.AddHeader("Authorization", $"Bearer {userToken}");

            var deleteUserResponse = restClient.Execute(deleteUserRequest);

            Assert.That(deleteUserResponse.IsSuccessful, Is.True, "User deletion failed");
        }

        [Test]
        public void ProductAndUserCartTest()
        {
            //Create new product with random title
            var productTitle = "ProductTitle" + random.Next(999, 9999).ToString();
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
            Assert.That(createProductResponse.IsSuccessful, Is.True, "Product creation failed");

            var productContent = JObject.Parse(createProductResponse.Content);
            var productId = productContent["_id"]?.ToString();

            Assert.That(productId, Is.Not.Null.And.Not.Empty);

            //login a user and get user token
            var loginRequest = new RestRequest("/user/login", Method.Post);
            loginRequest.AddBody(new
            {
                Email = "john.doe@example.com",
                Password = "password123"
            });

            var loginResponse = restClient.Execute(loginRequest);

            Assert.That(loginResponse.IsSuccessful, Is.True,
                "Login failed");
            Assert.That(loginResponse.Content, Is.Not.Null,
                "Login response data is null");

            var userToken = JObject.Parse(loginResponse.Content)["token"]?.ToString();
            Assert.That(userToken, Is.Not.Null.And.Not.Empty, "Login token is null or empty");

            //Add created product to the user's card
            var addCardRequest = new RestRequest("/user/cart", Method.Post);
            addCardRequest.AddHeader("Authorization", $"Bearer {userToken}");
            addCardRequest.AddJsonBody(new
            {
                cart = new[]
                {
                    new { _id = productId, count = 1, color = "Red"}
                }
            });

            var addCardResponse = restClient.Execute(addCardRequest);

            Assert.That(addCardResponse.IsSuccessful, Is.True, "Adding product to cart failed");

            //Apply coupon to the cart
            var addCouponRequest = new RestRequest("/user/cart/applycoupon", Method.Post);
            addCouponRequest.AddHeader("Authorization", $"Bearer {userToken}");
            addCouponRequest.AddJsonBody(new
            {
                Coupon = "BLACKFRIDAY"
            });

            var addCouponResponse = restClient.Execute(addCouponRequest);

            Assert.That(addCouponResponse.IsSuccessful, Is.True, "Adding coupon to cart failed");

            //Delete the product
            var deleteProductRequest = new RestRequest($"/product/{productId}", Method.Delete);
            deleteProductRequest.AddHeader("Authorization", $"Bearer {adminToken}");

            var deleteProductResponse = restClient.Execute(deleteProductRequest);
            Assert.That(deleteProductResponse.IsSuccessful, Is.True, "Delete product failed");
        }
    }
}
