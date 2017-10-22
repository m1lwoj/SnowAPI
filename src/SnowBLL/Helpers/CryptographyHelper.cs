using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace SnowBLL.Helpers
{
    public class CryptographyHelper
    {

        public static string Hash(string value, string salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                  password: value,
                  salt: Encoding.UTF8.GetBytes(salt),
                  prf: KeyDerivationPrf.HMACSHA1,
                  iterationCount: 10000,
                  numBytesRequested: 256 / 8));

            return hashed;
        }
    }
}
