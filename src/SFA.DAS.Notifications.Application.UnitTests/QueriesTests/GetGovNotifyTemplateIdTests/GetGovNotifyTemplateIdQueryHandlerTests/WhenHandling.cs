using System;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Notifications.Application.Queries.GetGovNotifyTemplateId;
using SFA.DAS.Notifications.Domain.Configuration;

namespace SFA.DAS.Notifications.Application.UnitTests.QueriesTests.GetGovNotifyTemplateIdTests.GetGovNotifyTemplateIdQueryHandlerTests
{
    public class WhenHandling
    {
        private const string MyTemplateId = "MyTemplate";
        private const string MyTemplateGovNotifyId = "c752ddef-e3f1-4f62-9d5d-7dd7ed5f77e5";

        private Mock<ITemplateConfigurationService> _templateConfigurationService;
        private Mock<ILogger> _logger;
        private GetGovNotifyTemplateIdQueryHandler _handler;
        private GetGovNotifyTemplateIdQuery _query;

        [SetUp]
        public void Arrange()
        {
            _templateConfigurationService = new Mock<ITemplateConfigurationService>();
            _templateConfigurationService.Setup(s => s.GetAsync())
                .ReturnsAsync(new TemplateConfiguration
                {
                    GovNotifyTemplates = new System.Collections.Generic.List<Template>
                    {
                        new Template {Id = MyTemplateId, GovNotifyId = MyTemplateGovNotifyId},
                        new Template {Id = "Not" + MyTemplateId, GovNotifyId = "fffb72dd-ef2d-4fcd-9d41-12a23801a5ea"}
                    }
                });

            _logger = new Mock<ILogger>();

            _handler = new GetGovNotifyTemplateIdQueryHandler(_templateConfigurationService.Object, _logger.Object);

            _query = new GetGovNotifyTemplateIdQuery
            {
                TemplateId = MyTemplateId
            };
        }

        [Test]
        public async Task ThenItShouldReturnAnInstanceOfGetGovNotifyTemplateIdQueryResponse()
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
            var actual = await _handler.Handle(new GetGovNotifyTemplateIdQuery { TemplateId = templateId });

            // Assert
            Assert.AreEqual(templateId, actual.GovNotifyTemplateId);
        }

        [Test]
        public async Task ThenItShouldLogWhenAGuidTemplateIdIsUsed()
        {
            // Arrange
            var templateId = Guid.NewGuid().ToString();

            // Act
            var actual = await _handler.Handle(new GetGovNotifyTemplateIdQuery { TemplateId = templateId });

            // Assert
            var expectedLogMessage = $"Request to send template {templateId} received using GOV Notify id";
            _logger.Verify(l => l.Info(expectedLogMessage), Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnGovNotifyTemplateIdFromServiceForRequestedTemplate()
        {
            // Act
            var actual = await _handler.Handle(_query);

            // Assert
            Assert.AreEqual(MyTemplateGovNotifyId, actual.GovNotifyTemplateId);
        }

        [Test]
        public async Task ThenItShouldReturnAnNullGovNotifyTemplateIdIfNoTemplateFound()
        {
            // Act
            var actual = await _handler.Handle(new GetGovNotifyTemplateIdQuery { TemplateId = "NoSuchTemplate" });

            // Assert
            Assert.IsNull(actual.GovNotifyTemplateId);
        }
    }
}
