using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace WorkshopAdvanced
{
    public static class GlobalConstants
    {
        public const string BaseUrl = "http://localhost:5000/api";

        public static string AuthenticateUser(string email, string password)
        {
            string resourse = "";
            if(email == "admin@gmail.com")
            {
                resourse = "user/admin-login";
            }
            else
            {
                resourse = "user/login";
            }

            var restClient = new RestClient(BaseUrl);
            var request = new RestRequest(resourse, Method.Post);
            request.AddJsonBody(new { email, password });

            var response = restClient.Post(request);

            if(response.StatusCode != HttpStatusCode.OK)
            {
                Assert.Fail($"Authentication failed. Status code: " +
                    $"{response.StatusCode} With Content: {response.Content}");
            }

            var content = JObject.Parse(response.Content);

            return content["token"]?.ToString();
        }
    }
}
