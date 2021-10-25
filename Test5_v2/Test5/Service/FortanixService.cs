using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Test5.Model;

namespace Test5
{
    public class FortanixService
    {
        private LoginData _loginData { get; set; }
        private SessionData _sessionData { get; set; }
        private HttpClient _httpClient;

        public FortanixService(string filePath)
        {
            _loginData = JsonToObject(filePath);
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_loginData.baseURI);
            
        }

        public LoginData JsonToObject(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            var loginData = JsonSerializer.Deserialize<LoginData>(jsonString);
            Console.WriteLine(loginData);
            return loginData;
        }

        public async Task CheckConnection()
        {
            if (_sessionData == null)
            {
                await ConnectToAPI();
            }
            else
            {
                var sessionStatus = _sessionData.sessionEnd.CompareTo(DateTime.Now);
                var secondsRemaining = (_sessionData.sessionEnd - _sessionData.sessionStart).TotalSeconds;

                //if time has expired
                if (sessionStatus <= 0)
                //if (_sessionData != null)
                {
                    await ReconnectToAPI();
                    return;
                }

                //if time hasn't expired and will expire within 100 seconds
                if (sessionStatus > 0 && secondsRemaining < 100)
                //if (_sessionData != null)
                {
                    await RefreshConnection();
                }
            }
        }

        private async Task ConnectToAPI()
        {
            Console.WriteLine("Connecting to API..");
            Thread.Sleep(300);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _loginData.apiKey);

            var request = new HttpRequestMessage(HttpMethod.Post, "/sys/v1/session/auth");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.Clear();
                Console.WriteLine("Connection Successful!");
                var content = await response.Content.ReadAsStringAsync();
                _sessionData = JsonSerializer.Deserialize<SessionData>(content);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sessionData.access_token);
            }
            else
            {
                var errormessage = await response.Content.ReadAsStringAsync();
                var errorcode = response.StatusCode;
                Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
            }
        }

        private async Task ReconnectToAPI()
        {
            Console.WriteLine("Time expired. reconnecting to API..");
            var request = new HttpRequestMessage(HttpMethod.Post, "/sys/v1/session/reauth");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Console.WriteLine("FØR:");
            //Console.WriteLine(_sessionData.sessionStart);
            //Console.WriteLine(_sessionData.sessionEnd);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.Clear();
                Console.WriteLine("Connection Successful!");
                var content = await response.Content.ReadAsStringAsync();
                _sessionData = JsonSerializer.Deserialize<SessionData>(content);

                //Console.WriteLine("ETTER:");
                //Console.WriteLine(_sessionData.sessionStart);
                //Console.WriteLine(_sessionData.sessionEnd);

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sessionData.access_token);

            }
            else
            {
                var errormessage = await response.Content.ReadAsStringAsync();
                var errorcode = response.StatusCode;
                Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
            }
        }

        private async Task RefreshConnection()
        {
            Console.WriteLine("Refreshing connection..");
            var request = new HttpRequestMessage(HttpMethod.Post, "/sys/v1/session/refresh");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Console.WriteLine("FØR:");
            //Console.WriteLine(_sessionData.sessionStart);
            //Console.WriteLine(_sessionData.sessionEnd);


            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.Clear();
                Console.WriteLine("Refresh Successful!");
                _sessionData.sessionEnd = DateTime.Now.AddSeconds(600);

                //Console.WriteLine("ETTER:");
                //Console.WriteLine(_sessionData.sessionStart);
                //Console.WriteLine(_sessionData.sessionEnd);
            }
            else
            {
                var errormessage = await response.Content.ReadAsStringAsync();
                var errorcode = response.StatusCode;
                Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
            }
        }

        public async Task<Key> CreateNewKey(string newKeyName)
        {
            await CheckConnection();

            Key keyRequest = new Key(newKeyName, 256, true);
            string keyToCreate = JsonSerializer.Serialize(keyRequest);

            var request = new HttpRequestMessage(HttpMethod.Post, "/crypto/v1/keys");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(keyToCreate);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Key newKey = JsonSerializer.Deserialize<Key>(content);
                return newKey;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                var errorCode = response.StatusCode;
                Console.WriteLine("Error: " + errorCode + "\nMessage: " + errorMessage);
                return new Key();
            }

        }

        //public async Task<Payload> Encrypt(string keyName, string data)
        //{
        //    /* 1. Make a POST request to /crypto/v1/encrypt
        //     *      - Send required information
        //     *      - Save important information
        //     *      
        //     * 2. Return a payload object with encrypted data
        //     *  
        //     */
        //}

        //public async Task<Payload> Decrypt(Payload payload)
        //{
        //    /* 1. 
        //     *      
        //     * 2. 
        //     * 
        //     */
        //}

    }

}