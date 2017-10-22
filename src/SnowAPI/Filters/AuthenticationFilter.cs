using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SnowAPI.Filters
{
    /// <summary>
    /// Authentication filter
    /// </summary>
    public class AuthenticationFilter : AuthorizeFilter
    {
        public AuthenticationFilter(AuthorizationPolicy policy) : base(policy)
        {

        }

        public AuthenticationFilter(IAuthorizationPolicyProvider policyProvider, IEnumerable<IAuthorizeData> authorizeData) : base(policyProvider, authorizeData)
        {
        }

        public override Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var a = base.OnAuthorizationAsync(context);
            Task.WaitAll(a);


            return a;
        }
    }
}
