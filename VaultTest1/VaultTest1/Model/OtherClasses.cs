using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultTest1.Model
{
    public class SessionData
    {
        public string request_id { get; set; }
        public Auth auth { get; set; }
    }

    public class Auth
    {
        public string client_token { get; set; }
        public string accessor { get; set; }
        public int lease_duration { get; set; }
    }

    public class Key
    {
        public bool exportable { get; set; }
        public string type { get; set; }
    }
}
