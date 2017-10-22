using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Validators.Users
{
    public class UserMessages
    {
        public static string ID_NOTEMPTY = "Id cannot be empty";
        public static string NAME_NOTEMPTY = "Name cannot be empty";
        public static string PASSWORD_NOTEMPTY = "Password cannot be empty";
        public static string PASSWORD_LENGTH = "Password should have at least 6 characters";
        public static string EMAIL_NOTEMPTY = "Email cannot be empty";
        public static string EMAIL_VALIDFORMAT = "Email invalid format";
        public static string CODE_NOTEMPTY = "Code cannot be empty";
        public static string CODE_4CHARACTERS = "Code should have 4 charcaters";
        public static string CONFIRMATIONPASSWORD_EQUAL = "Passwords should be the same";
    }
}
