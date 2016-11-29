using NUnit.Framework;

namespace SFA.DAS.Notifications.Infrastructure.UnitTests.ConfigurationTests.TemplateConfigurationServiceTests
{
    public class WhenGetting : TemplateConfigurationServiceTestBase
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
            var actual = Service.Get();

            // Assert
            AssertItHasReturnedAnInstanceOfTemplateConfiguration(actual);
        }

        [Test]
        public override void ThenItShouldReturnCorrectlyDeserializeValues()
        {
            // Act
            var actual = Service.Get();

            // Assert
            AssertItHasReturnedCorrectlyDeserializeValues(actual);
        }
    }
}
