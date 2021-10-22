using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
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
            //else
            //{                
            //    var value = _sessionData.sessionEnd.CompareTo(DateTime.Now);

            //    if (value <= 0)
            //    {
            //        await ConnectToAPI();
            //    }
            //    //else
            //    //{
            //    //    await ReconnectToAPI();
            //    //}
            //}
        }

        private async Task ConnectToAPI()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _loginData.apiKey);
            var request = new HttpRequestMessage(HttpMethod.Post, "/sys/v1/session/auth");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                _sessionData = JsonSerializer.Deserialize<SessionData>(content);
            }
            else
            {
                var errormessage = await response.Content.ReadAsStringAsync();
                var errorcode = response.StatusCode;
                Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
            }


        }
            private Task ReconnectToAPI()
            {
                throw new NotImplementedException();
            }

        public async Task<Key> CreateNewKey(string newKeyName)
        {
            await CheckConnection();
            return new Key();
            
             /* 2. Create new key
             */
               
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