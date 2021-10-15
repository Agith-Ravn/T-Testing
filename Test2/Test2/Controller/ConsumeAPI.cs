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
        private string _loginDataPath { get; }

        private LoginData _loginData { get; set; }

        private static AuthResponse _authResponse;

        private HttpClient _httpClient { get; }

        public ConsumeAPI(string loginDataPath)
        {
            _loginDataPath = loginDataPath;
            _loginData = GetLoginData(loginDataPath);
            _httpClient = new HttpClient();
            _httpClient.Timeout = new TimeSpan(0, 0, 300);
            _httpClient.BaseAddress = new Uri(_loginData.baseURI);
            _authResponse = new AuthResponse();
        }

        private LoginData GetLoginData(string loginDataPath)
        {
            string jsonString = System.IO.File.ReadAllText(loginDataPath);
            LoginData loginData = JsonSerializer.Deserialize<LoginData>(jsonString);
            return loginData;
        }

        public async Task CheckAPIStatus()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("sys/v1/health");
                Console.WriteLine("API is available");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("API is NOT available");
            }
        }

        public async Task ConnectToAPI()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _loginData.apiKey);

            var response = await _httpClient.PostAsync("sys/v1/session/auth", null);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            _authResponse = JsonSerializer.Deserialize<AuthResponse>(responseBody, 
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            Console.WriteLine(_authResponse);

            Console.WriteLine(_authResponse.Access_token);
            Console.WriteLine(_authResponse.Entity_id);
            Console.WriteLine(_authResponse.Expires_in);


            
        }

        private async Task LogIntoApi()
        {


        }

    }
}