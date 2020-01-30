using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SFA.DAS.Notifications.Api.UnitTests.ControllerTests
{
    public static class TestHelpers
    {
        public static ControllerContext CreateControllerContextWithUser(string name = "UnitTestUser", string nameIdentifier = "1")
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.NameIdentifier, nameIdentifier)
            }));

            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}
