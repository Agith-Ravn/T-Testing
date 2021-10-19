using System;

namespace Test2.Client.Models
{
    public class KeyObjectData
    {
        public string name { get; set; }
        public string description { get; set; }
        public int acct_id { get; set; }
        public CreatorType creatorType { get; set; }
        public string obj_type { get; set; }



    }

    //Temporary placement (make a own class for this later)
    //public class CreatorType
    //{
    //    public string app { get; set; }
    //    public string user { get; set; }
    //}

    enum Obj_type
    {
        
    }
}