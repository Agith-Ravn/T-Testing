using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Test2.Client.Models;

namespace Test2
{
    public class ConsumeAPI
    {
        //Lager object av HttpClient class
        private readonly HttpClient httpClient = new HttpClient();
        private string apiKey { get; set; }

        private static AuthResponse authResponse;

        public ConsumeAPI()
        {
            apiKey = "NDc1Y2EzNzEtZjQxNy00YTYzLWExOGEtMzM5MTU1N2M3YTNiOmVhTHN3TC11WlREeEpYeWIxTXJsRGRXcXdreW5Zc3FpZnhTTUNRRnE2TTV2VU50WElDZzhyNXZZdDA3QXZDM3hJZl9GSHNYU0IzYmJqU1BZM0tHcktB";
            authResponse = new AuthResponse();
        }

        public async Task ConnectToAPI()
        {
            //Legger til baseAdress i object
            httpClient.BaseAddress = new Uri("https://eu.smartkey.io/");
            httpClient.Timeout = new TimeSpan(0, 0, 30);
            httpClient.DefaultRequestHeaders.Clear();

            //Console.WriteLine("Test");
            await LogIntoApi();
            //await CheckAPIHealth();
        }

        private async Task CheckAPIHealth()
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync("sys/v1/health");
                Console.WriteLine("API is available");
            }
            catch (HttpRequestException e)
            {
                Console.Write("API is NOT available \nMessage: " + e);

            }
        }

        private async Task LogIntoApi()
        {

            //Legger til en verdi (basic + Apikey) i Request header > Authorization
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", apiKey);

            try
            {

                //Post request + sender med null i content
                var response = await httpClient.PostAsync("sys/v1/session/auth", null);
                var responseBody = await response.Content.ReadAsStringAsync();
                authResponse = JsonSerializer.Deserialize<AuthResponse>(responseBody);

                Console.WriteLine(authResponse.Access_token);

                //httpClient.DefaultRequestHeaders.Authorization =
                //    new AuthenticationHeaderValue("Bearer", authResponse.Access_token);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }



        }




    }
}