using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VaultTest1.Model;

namespace VaultTest1
{
    class VaultService
    {
        private LoginData _loginData;
        public bool login { get; set; }
        private HttpClient _httpClient { get; set; }
        public SessionData _sessionData { get; private set; }

        public VaultService(string filepath)
        {
            _loginData = JsonToObject(filepath);
            login = false;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_loginData.baseURI);

        }

        public LoginData JsonToObject(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            var loginData = JsonSerializer.Deserialize<LoginData>(jsonString);
            return loginData;
        }

        public async Task<SessionData> Login()
        {
            var loginData = new LoginData()
            {
                role_id = _loginData.role_id,
                secret_id = _loginData.secret_id
            };

            var serializedLoginData = JsonSerializer.Serialize(loginData);

            var request = new HttpRequestMessage(HttpMethod.Post, "/v1/auth/approle/login");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(serializedLoginData);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                SessionData _sessionData = JsonSerializer.Deserialize<SessionData>(content);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sessionData.auth.client_token);
                login = true;
                return _sessionData;
            }
            else
            {
                var errormessage = await response.Content.ReadAsStringAsync();
                var errorcode = response.StatusCode;
                Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
                return _sessionData = new SessionData();
            }
        }

        public async Task<Key> CreateNewKey(string keyName)
        {

            var key = new Key()
            {
                exportable = true,
                type = "aes256-gcm96"
                //May need to change this later..
            };

            var serializedKey = JsonSerializer.Serialize(key);

            var request = new HttpRequestMessage(HttpMethod.Post, "/v1/transit/keys/" + keyName);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(serializedKey);
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
                var errormessage = await response.Content.ReadAsStringAsync();
                var errorcode = response.StatusCode;
                Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
                return new Key();
            }
        }

    }
}
