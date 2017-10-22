using Microsoft.Extensions.Options;
using Moq;
using SnowBLL.Helpers;
using SnowBLL.Models;
using SnowBLL.Models.Auth;
using SnowBLL.Models.Users;
using SnowBLL.Service.Concrete;
using SnowBLL.Service.Interfaces;
using SnowDAL.DBModels;
using SnowDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SnowTests.Unit
{
    public class AuthorizationTests
    {
        [Fact]
        public void SamePasswordsHash()
        {
            var result1 = CryptographyHelper.Hash("password123", "salt123");
            var result2 = CryptographyHelper.Hash("password123", "salt123");

            Assert.Equal(result1, result2);
        }
    }
}
