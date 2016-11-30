using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Notifications.Application.Queries.GetEmailServiceTemplateId;
using SFA.DAS.Notifications.Domain.Configuration;

namespace SFA.DAS.Notifications.Application.UnitTests.QueriesTests.GetEmailServiceTemplateIdTests.GetEmailServiceTemplateIdQueryHandlerTests
{
    public class WhenHandling
    {
        private const string MyTemplateId = "MyTemplate";
        private const string MyTemplateEmailServiceId = "c752ddef-e3f1-4f62-9d5d-7dd7ed5f77e5";

        private Mock<ITemplateConfigurationService> _templateConfigurationService;
        private GetEmailServiceTemplateIdQueryHandler _handler;
        private GetEmailServiceTemplateIdQuery _query;

        [SetUp]
        public void Arrange()
        {
            _templateConfigurationService = new Mock<ITemplateConfigurationService>();
            _templateConfigurationService.Setup(s => s.GetAsync())
                .ReturnsAsync(new TemplateConfiguration
                {
                    EmailServiceTemplates = new System.Collections.Generic.List<Template>
                    {
                        new Template {Id = MyTemplateId, EmailServiceId = MyTemplateEmailServiceId},
                        new Template {Id = "Not" + MyTemplateId, EmailServiceId = "fffb72dd-ef2d-4fcd-9d41-12a23801a5ea"}
                    }
                });

            _handler = new GetEmailServiceTemplateIdQueryHandler(_templateConfigurationService.Object);

            _query = new GetEmailServiceTemplateIdQuery
            {
                TemplateId = MyTemplateId
            };
        }

        [Test]
        public async Task ThenItShouldReturnAnInstanceOfGetEmailServiceTemplateIdQueryResponse()
        {
            // Act
            var actual = await _handler.Handle(_query);

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenItShouldReturnTheRequestedTemplateIdIfItParsesAsAGuid()
        {
            // Arrange
            var templateId = Guid.NewGuid().ToString();

            // Act
            var actual = await _handler.Handle(new GetEmailServiceTemplateIdQuery { TemplateId = templateId });

            // Assert
            Assert.AreEqual(templateId, actual.EmailServiceTemplateId);
        }

        [Test]
        public async Task ThenItShouldReturnEmailServiceTemplateIdFromServiceForRequestedTemplate()
        {
            // Act
            var actual = await _handler.Handle(_query);

            // Assert
            Assert.AreEqual(MyTemplateEmailServiceId, actual.EmailServiceTemplateId);
        }

        [Test]
        public async Task ThenItShouldReturnAnNullEmailServiceTemplateIdIfNoTemplateFound()
        {
            // Act
            var actual = await _handler.Handle(new GetEmailServiceTemplateIdQuery { TemplateId = "NoSuchTemplate" });

            // Assert
            Assert.IsNull(actual.EmailServiceTemplateId);
        }
    }
}
