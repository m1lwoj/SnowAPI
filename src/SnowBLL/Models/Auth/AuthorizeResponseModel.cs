using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Models.Auth
{
    public class AuthorizeResponseModel
    {
        public string AccessToken { get; set; }
        public int Expires { get; set; }
    }
}
