namespace ClientApp
{
    using System;
    using System.Net.Http;
    using Newtonsoft.Json;
    
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Clear();
            client.BaseAddress = new Uri("http://localhost:62236");

            // 1. without access_token will not access the service
            //    and return 401 .
            var resWithoutToken = client.GetAsync("/customers").Result;

            Console.WriteLine($"Sending Request to /customers , without token.");
            Console.WriteLine($"Result : {resWithoutToken.StatusCode}");

            //2. with access_token will access the service
            //   and return result.
            client.DefaultRequestHeaders.Clear();
            Console.WriteLine("\nBegin Auth....");
            var jwt = GetJwt();
            Console.WriteLine("End Auth....");
            if (string.IsNullOrWhiteSpace(jwt))
            {
                Console.WriteLine("Auth failed. Set DEMO_USER and DEMO_PASSWORD, and fill AuthServer plus APIGateway appsettings.");
                return;
            }
            Console.WriteLine("Received an access token.");

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwt}");
            var resWithToken = client.GetAsync("/customers").Result;

            Console.WriteLine($"\nSend Request to /customers , with token.");
            Console.WriteLine($"Result : {resWithToken.StatusCode}");
            Console.WriteLine(resWithToken.Content.ReadAsStringAsync().Result);

            //3. visit no auth service 
            Console.WriteLine("\nNo Auth Service Here ");
            client.DefaultRequestHeaders.Clear();
            var res = client.GetAsync("/customers/1").Result;

            Console.WriteLine($"Send Request to /customers/1");
            Console.WriteLine($"Result : {res.StatusCode}");
            Console.WriteLine(res.Content.ReadAsStringAsync().Result);

            Console.Read();
        }


        private static string GetJwt()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:62236");
            client.DefaultRequestHeaders.Clear();

            var user = Environment.GetEnvironmentVariable("DEMO_USER");
            var password = Environment.GetEnvironmentVariable("DEMO_PASSWORD");
            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Set DEMO_USER and DEMO_PASSWORD before requesting a token.");
                return null;
            }

            var res2 = client.GetAsync($"/api/auth?name={Uri.EscapeDataString(user)}&pwd={Uri.EscapeDataString(password)}").Result;
            if (!res2.IsSuccessStatusCode)
            {
                Console.WriteLine($"Auth request failed: {(int)res2.StatusCode}");
                return null;
            }

            dynamic jwt = JsonConvert.DeserializeObject(res2.Content.ReadAsStringAsync().Result);
            return jwt?.access_token;
        }
    }
}
