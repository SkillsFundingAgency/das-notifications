using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
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
        protected const string Template1EmailServiceId = "bd1502ed-27f7-4d94-b96c-c40c0e3eeabe";
        protected const string Template2Id = "Template2";
        protected const string Template2EmailServiceId = "6aaa019f-00fc-447f-a933-0313a787e96d";

        protected string RepositoryResult;
        protected Mock<IConfiguration> Configuration;
        protected TemplateConfigurationService Service;

        protected virtual void Arrange()
        {
            RepositoryResult = JsonConvert.SerializeObject(new TemplateConfiguration
            {
                EmailServiceTemplates = new List<Template>
                {
                    new Template
                    {
                         Id = Template1Id,
                         EmailServiceId = Template1EmailServiceId
                    },
                    new Template
                    {
                         Id = Template2Id,
                         EmailServiceId = Template2EmailServiceId
                    }
                }
            });

            Configuration = new Mock<IConfiguration>();
            Configuration.Setup(c => c[$"{ServiceName}_{Version}"]).Returns(RepositoryResult);

            Service = new TemplateConfigurationService(EnvironmentName, Configuration.Object);
        }

        public abstract void ThenItShouldReturnAnInstanceOfTemplateConfiguration();
        public abstract void ThenItShouldReturnCorrectlyDeserializeValues();


        protected void AssertItHasReturnedAnInstanceOfTemplateConfiguration(TemplateConfiguration actual)
        {
            Assert.IsNotNull(actual);
        }
        protected void AssertItHasReturnedCorrectlyDeserializeValues(TemplateConfiguration actual)
        {
            Assert.IsNotNull(actual.EmailServiceTemplates);
            Assert.AreEqual(2, actual.EmailServiceTemplates.Count);

            var template1 = actual.EmailServiceTemplates[0];
            Assert.AreEqual(Template1Id, template1.Id);
            Assert.AreEqual(Template1EmailServiceId, template1.EmailServiceId);

            var template2 = actual.EmailServiceTemplates[1];
            Assert.AreEqual(Template2Id, template2.Id);
            Assert.AreEqual(Template2EmailServiceId, template2.EmailServiceId);
        }
    }
}
