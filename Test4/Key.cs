using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testfortanixapi
{
    public class Key
    {
        public DateTime activationDT { get; set; }
        private string _activation_date;
        public DateTime creationDT { get; set; }
        private string _created_at;

        public string acct_id { get; set; }

        public string activation_date
        {
            get=>_activation_date;
            set
            {
                _activation_date = value;
                activationDT=DateTime.ParseExact(value,"yyyyMMddTHHmmssK",null);
            }
        }

        public string created_at
        {
            get=>_created_at;
            set
            {
                _created_at = value;
                creationDT = DateTime.ParseExact(value, "yyyyMMddTHHmmssK", null);
            }
        }
        public bool enabled { get; set; }
        public int key_size { get; set; }
        public string kid { get; set; }
        public string name { get; set; }
        public string obj_type { get; set; }
        public string state { get; set; }
        public string transient_key { get; set; }

    }
}
