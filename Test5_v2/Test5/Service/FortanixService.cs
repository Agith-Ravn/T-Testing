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

        public FortanixService(string path)
        {
            /1. Må konverte innhold fra json fil til object
        }

        public async Task CheckConnection()
        {

        }

        public async Task<Key> CreateNewKey(string newKeyName)
        {
            /* 1. You should check if you're logged in
             *      - If not > Log in
             *      - If the time is expired > Reauth/Refresh
             */
            await CheckConnection();

            
             /* 2. Create new key
             */
               
        }

        public async Task<Payload> Encrypt(string keyName, string data)
        {
            /* 1. Make a POST request to /crypto/v1/encrypt
             *      - Send required information
             *      - Save important information
             *      
             * 2. Return a payload object with encrypted data
             *  
             */
        }

        public async Task<Payload> Decrypt(Payload payload)
        {
            /* 1. 
             *      
             * 2. 
             * 
             */
        }

    }

}