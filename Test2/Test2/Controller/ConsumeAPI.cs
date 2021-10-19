using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Test2.Client.Models;

namespace Test2
{
    public class ConsumeAPI
    {
        private string _loginDataPath { get; }

        private LoginData _loginData { get; set; }

        private static AuthResponse _authResponse;

        private SessionData _sessionData;
        private List<KeyObject> _keyList { get; set; }

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
                Console.WriteLine("API is available. Connecting to API...");
                Thread.Sleep(300);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("API is NOT available. Try again later");
            }
        }

        public async Task ConnectToAPI()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _loginData.apiKey);

            var response = await _httpClient.PostAsync("sys/v1/session/auth", null);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            //Trenger ikke denne da...
            //_authResponse = JsonSerializer.Deserialize<AuthResponse>(responseBody, 
            //    new JsonSerializerOptions
            //    {
            //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            //    });

            _sessionData = JsonSerializer.Deserialize<SessionData>(responseBody,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });


            //Console.WriteLine(_authResponse);
            //Console.WriteLine(_authResponse.Access_token);
            //Console.WriteLine(_authResponse.Entity_id);
            //Console.WriteLine(_authResponse.Expires_in);

            Console.WriteLine("Successful connection to API\n");

            //Trenger denne for å fortsette videre..
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _sessionData.access_token);
        }

        public bool CheckSession()
        {
            var timeValue = DateTime.Compare(DateTime.Now, _sessionData.sessionEnd);
            if (timeValue < 0)
            {
                Console.WriteLine("Session active until:" + _sessionData.sessionEnd + "\n");
                return true;
            }
            Console.WriteLine("Session expired: " + _sessionData.sessionEnd + "\n");
            return false;
        }

        //public async Task RefreshSession()
        //{
        //    await _httpClient.PostAsync("/sys/v1/session/refresh", null);
        //    _sessionData.sessionStart = DateTime.Now;
        //}

        //public async Task TerminateSession()
        //{

        //}


        public async Task GetAllKeys()
        {
            var response = await _httpClient.GetAsync("/crypto/v1/keys");
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                _keyList = JsonSerializer.Deserialize<List<KeyObject>>(responseString);

                Console.WriteLine("Number of keys found: " + _keyList.Count);
                int count = 0;
                foreach (var key in _keyList)
                {
                    count++;
                    Console.WriteLine(count + ". Name: " + key.Name + " - Description: " + key.Description);
                }
                Console.WriteLine("\n");

            }
            else
            {
                var errormessage = await response.Content.ReadAsStringAsync();
                var errorcode = response.StatusCode;
                Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
            }
        }

        //Trenger kanskje ikke..
        //public async Task GetKeyByName(string keyname)
        //{
        //    KeyObject returnkey = null;
        //    foreach (var key in _keyList)
        //        if (key.name == keyname)
        //        {
        //            await GetKeyByID(key.kid);
        //            return;
        //        }

        //    //getactiveKey = returnkey;
        //}

        //private async Task GetKeyByID(string kid)
        //{
        //    KeyObject returnkey = null;
        //    if (kid.Length == 32 || kid.Length == 36)
        //    {
        //        var response = await _httpClient.GetAsync("/crypto/v1/keys/" + kid);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var responseBody = await response.Content.ReadAsStringAsync();
        //            returnkey = JsonSerializer.Deserialize<KeyObject>(responseBody);
        //            Console.WriteLine(string.Format($"The key with the name {returnkey.Name} has been found"));
        //        }
        //        else
        //        {
        //            var errormessage = await response.Content.ReadAsStringAsync();
        //            var errorcode = response.StatusCode;
        //            Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
        //        }
        //    }
        //}
    }

}