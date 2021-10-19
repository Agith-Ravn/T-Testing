using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testfortanixapi
{
    class LoginData
    {
        public string apiKey { get; set; }
        public string bearertoken { get; set; }
        public DateTime sessionstart { get; set; }
        //The duration of the session in ms
        public int maxduration { get; set; }
        public string baseURI { get; set; }

    }
}
