using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocaWebAPI.Models
{
    public class LWListasContatos
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Contact
        {
            public string email { get; set; }
            public CustomFields custom_fields { get; set; }
        }

        public class CustomFields
        {
            public string name { get; set; }
        }

        public class List
        {
            public List<Contact> contacts { get; set; }
            public bool overwriteattributes { get; set; }
        }

        public class ListasContatos
        {
            public List list { get; set; }
        }


    }
}