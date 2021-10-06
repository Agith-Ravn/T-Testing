using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Test2
{
    public class TestClass
    {
        static readonly HttpClient client = new HttpClient();
        public static async Task Test1()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("https://apps.sdkms.fortanix.com/sys/v1/version");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            
        }
    }
}