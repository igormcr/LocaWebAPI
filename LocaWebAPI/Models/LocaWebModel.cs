using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocaWebAPI.Models
{
    public class LocaWebModel
    {
        public class Lista
        {
            public int contacts_count { get; set; }
            public string id { get; set; }
            public string created_at { get; set; }
            public string description { get; set; }
            public string updated_at { get; set; }
            public string name { get; set; }
            public Page page { get; set; }
            public List<Item> items { get; set; }

        }

        public class Item
        {
            public int contacts_count { get; set; }
            public string id { get; set; }
            public string description { get; set; }
            public string name { get; set; }
        }

        public class Page
        {
            public int current { get; set; }
            public int total { get; set; }
        }

        public class Contact
        {
            public string email { get; set; }
            public CustomFields custom_fields { get; set; }
        }

        public class CustomFields
        {
            public string nasc { get; set; }
            public string city { get; set; }
            public string name { get; set; }
        }

        public class List
        {            
            public List<Contact> contacts { get; set; }
            public bool overwriteattributes { get; set; }
            public string name { get; set; }
            public string description { get; set; }

        }

        public class ListContacts
        {
            public List<Contact> contacts { get; set; }
            public List list { get; set; }
        }

        public class Message
        {
            public string campaign_id { get; set; }
            public string domain_id { get; set; }
            public string html_body { get; set; }
            public string text_body { get; set; }
            public List<string> list_ids { get; set; }
            public string name { get; set; }
            public string sender_name { get; set; }
            public string sender { get; set; }
            public string subject { get; set; }
            public string scheduled_to { get; set; }
        }

        public class Envio
        {
            public Message message { get; set; }
        }


        public class Raiz
        {
            public List list { get; set; }
            public Message message { get; set; }
        }


    }
}