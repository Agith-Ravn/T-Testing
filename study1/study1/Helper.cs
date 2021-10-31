using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace study1
{
    public class Payload
    {
        public string Data { get; set; }
        public string KeyName { get; set; }

        public Payload(string keyName, string data)
        {
            Data = data;
            KeyName = keyName;
        }

    }

    public class LoginData
    {
        public string ApiKey { get; set; }
        public string BearerToken { get; set; }
        public DateTime SessionStart { get; set; }
        public int MaxDuration { get; set; }
        public string BaseUri { get; set; }
    }

    public class SessionData
    {
        public string access_token { get; set; }
        public string entity_id { get; set; }
        private double _expires_in;
        public DateTime sessionend { get; set; }
        public DateTime sessionstart { get; set; }

        public double expires_in
        {
            get => _expires_in;
            set
            {
                _expires_in = value;
                sessionstart = DateTime.Now;
                sessionend = sessionstart.AddSeconds(value);
            }
        }



    }

    public class Key
    {
        public string kid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int key_size { get; set; }
        public string acct_id { get; set; }
        public string obj_type { get; set; }
        public List<string> key_ops { get; set; }
        public bool enabled { get; set; }
        public string value { get; set; }

        public Key() { }
        public Key(string Name, int KeySize)
        {
            name = Name;
            obj_type = "AES";
            key_size = KeySize;
            key_ops = new List<string> { "ENCRYPT", "DECRYPT", "WRAPKEY", "UNWRAPKEY", "EXPORT", "APPMANAGEABLE" };
        }

        public Key(string Name, int KeySize, bool Enabled)
        {
            name = Name;
            obj_type = "AES";
            key_size = KeySize;
            key_ops = new List<string> { "ENCRYPT", "DECRYPT", "WRAPKEY", "UNWRAPKEY", "EXPORT", "APPMANAGEABLE" };
            enabled = Enabled;
        }
    }

    public class SObjectDescriptor
    {
        public string kid { get; set; }

        public SObjectDescriptor(string keyId)
        {
            kid = keyId;
        }
    }
    public class EncryptionCall
    {
        public SObjectDescriptor key { get; set; }
        public string alg { get; set; }
        public string plain { get; set; }
        public string mode { get; set; }
        public string iv { get; set; }

        public EncryptionCall(SObjectDescriptor keydeDescriptor, string plaintext, string IV)
        {
            key = keydeDescriptor;
            plain = plaintext;
            iv = IV;
            alg = "AES";
            mode = "CBC";
        }
    }

    public class DecryptionCall
    {
        public SObjectDescriptor key { get; set; }
        public string alg { get; set; }
        public string cipher { get; set; }
        public string mode { get; set; }
        public string iv { get; set; }

        public DecryptionCall(SObjectDescriptor keyDescriptor, string Cipher, string IV)
        {
            key = keyDescriptor;
            cipher = Cipher;
            iv = IV;
            alg = "AES";
            mode = "CBC";
        }
    }

    public class EncryptionResponse
    {
        public string kid { get; set; }
        public string cipher { get; set; }
        public string iv { get; set; }
        public string tag { get; set; }
    }

    public class DecryptionResponse
    {
        public string kid { get; set; }
        public string plain { get; set; }
    }

    public class RotateKeyCall
    {
        public string name { get; set; }
        public string obj_type { get; set; }
        public List<string> key_ops { get; set; }

        public RotateKeyCall(string Name)
        {
            name = Name;
            obj_type = "AES";
            key_ops = new List<string> { "ENCRYPT", "DECRYPT", "WRAPKEY", "UNWRAPKEY", "EXPORT", "APPMANAGEABLE" };
            ;
        }
    }

    public class WrappedKey
    {
        public string wrapped_key { get; set; }
        public string IV { get; set; }
        public string tag { get; set; }
    }

    public class WrapKeyRequest
    {
        public string alg { get; set; }
        public string kid { get; set; }
        public string mode { get; set; }
        public string iv { get; set; }

        public WrapKeyRequest(string keybeingwrappedid, string IV)
        {
            alg = "AES";
            kid = keybeingwrappedid;
            mode = "CBC";
            iv = IV;

        }

    }
}
