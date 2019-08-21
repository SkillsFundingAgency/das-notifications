using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Notifications.Api.Client.Configuration;

namespace SFA.DAS.Notifications.Api.Client.UnitTests.NotificationsApiClientConfigurationTests
{
    [TestFixture]
    public class AttributeTests
    {
        [Test]
        public void ClientTokenDoesNotHaveAnObsoleteAttribute()
        {
            typeof(NotificationsApiClientConfiguration).GetProperty("ClientToken").Should().NotBeDecoratedWith<ObsoleteAttribute>();
        }


        [TestCase("ClientId")]
        [TestCase("ClientSecret")]
        [TestCase("IdentifierUri")]
        [TestCase("Tenant")]
        public void AADPropertyHasObsoleteAttribute(string propertyName)
        {
            typeof(NotificationsApiClientConfiguration).GetProperty(propertyName).Should().BeDecoratedWith<ObsoleteAttribute>();
        }
    }
}