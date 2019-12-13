using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.Notifications.Api2.Security
{
    public class LegacySupportingAuthorizeAttribute : AuthorizeAttribute
    {
        public LegacySupportingAuthorizeAttribute(string roles)
        {
            Roles = Roles;
        }
    }
}
