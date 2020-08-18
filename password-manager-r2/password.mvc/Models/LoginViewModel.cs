using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace password.mvc.Models
{
    public class LoginViewModel
    {
        public string Site { get; set; }
        [Display(Name = "Username")]
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}