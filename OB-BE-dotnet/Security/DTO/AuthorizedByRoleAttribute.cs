using Microsoft.AspNetCore.Authorization;
using System;

namespace Security.DTO
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuthorizedByRoleAttribute : AuthorizeAttribute
    {
        public AuthorizedByRoleAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }
    }
}
