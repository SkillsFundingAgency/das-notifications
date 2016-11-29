using System.Collections.Generic;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Configuration;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Infrastructure.Configuration;

namespace SFA.DAS.Notifications.Infrastructure.UnitTests.ConfigurationTests.TemplateConfigurationServiceTests
{
    public abstract class TemplateConfigurationServiceTestBase
    {
        protected const string ServiceName = "SFA.DAS.Notifications-Templates";
        protected const string EnvironmentName = "UNITTEST";
        protected const string Version = "1.0";

        protected const string Template1Id = "Template1";
        protected const string Template1GovNotifyId = "bd1502ed-27f7-4d94-b96c-c40c0e3eeabe";
        protected const string Template2Id = "Template2";
        protected const string Template2GovNotifyId = "6aaa019f-00fc-447f-a933-0313a787e96d";

        protected string RepositoryResult;
        protected Mock<IConfigurationRepository> ConfigurationRepository;
        protected TemplateConfigurationService Service;

        protected virtual void Arrange()
        {
            RepositoryResult = JsonConvert.SerializeObject(new TemplateConfiguration
            {
                GovNotifyTemplates = new List<Template>
                {
                    new Template
                    {
                         Id = Template1Id,
                         GovNotifyId = Template1GovNotifyId
                    },
                    new Template
                    {
                         Id = Template2Id,
                         GovNotifyId = Template2GovNotifyId
                    }
                }
            });

            ConfigurationRepository = new Mock<IConfigurationRepository>();
            ConfigurationRepository.Setup(r => r.Get(ServiceName, EnvironmentName, Version))
                .Returns(RepositoryResult);
            ConfigurationRepository.Setup(r => r.GetAsync(ServiceName, EnvironmentName, Version))
                .ReturnsAsync(RepositoryResult);

            Service = new TemplateConfigurationService(ConfigurationRepository.Object, EnvironmentName);
        }

        public abstract void ThenItShouldReturnAnInstanceOfTemplateConfiguration();
        public abstract void ThenItShouldReturnCorrectlyDeserializeValues();


        protected void AssertItHasReturnedAnInstanceOfTemplateConfiguration(TemplateConfiguration actual)
        {
            Assert.IsNotNull(actual);
        }
        protected void AssertItHasReturnedCorrectlyDeserializeValues(TemplateConfiguration actual)
        {
            Assert.IsNotNull(actual.GovNotifyTemplates);
            Assert.AreEqual(2, actual.GovNotifyTemplates.Count);

            var template1 = actual.GovNotifyTemplates[0];
            Assert.AreEqual(Template1Id, template1.Id);
            Assert.AreEqual(Template1GovNotifyId, template1.GovNotifyId);

            var template2 = actual.GovNotifyTemplates[1];
            Assert.AreEqual(Template2Id, template2.Id);
            Assert.AreEqual(Template2GovNotifyId, template2.GovNotifyId);
        }
    }
}
