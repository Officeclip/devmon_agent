using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dev_web_api.BusinessLayer
{
    public class User
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        //public DateTime LastSent { get; set; }
        public string Password { get; set; }
    }
}