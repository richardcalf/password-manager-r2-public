using Microsoft.Ajax.Utilities;
using password.model;
using password.mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace password.mvc.Extensions
{
    public static class ModelConverterExtension
    {
        public static LoginViewModel ToViewModel(this Login login)
        {
            return new LoginViewModel
            {
                Password = login.Password,
                Site = login.Site,
                UserName = login.UserName
            };
        }
    }
}