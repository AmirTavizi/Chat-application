using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace CrossChat.Domain.Common.CustomClass
{
    public class CurrentUser
    {
        public static UserClaimsInfo Get(ClaimsPrincipal User)
        {
            return new UserClaimsInfo()
            {
                RoleCode = User?.FindFirst(ClaimTypes.GivenName).Value,
                UserId = User?.FindFirst(ClaimTypes.Surname).Value
            };
        }
    }
}
