using NUnit.Framework;

namespace SFA.DAS.Notifications.Infrastructure.UnitTests.ConfigurationTests.TemplateConfigurationServiceTests
{
    public class WhenGettingAsync : TemplateConfigurationServiceTestBase
    {
        [SetUp]
        protected override void Arrange()
        {
            base.Arrange();
        }

        [Test]
        public override void ThenItShouldReturnAnInstanceOfTemplateConfiguration()
        {
            // Act
            var actual = Service.GetAsync().Result;

            // Assert
            AssertItHasReturnedAnInstanceOfTemplateConfiguration(actual);
        }

        [Test]
        public override void ThenItShouldReturnCorrectlyDeserializeValues()
        {
            // Act
            var actual = Service.GetAsync().Result;

            // Assert
            AssertItHasReturnedCorrectlyDeserializeValues(actual);
        }
    }
}
