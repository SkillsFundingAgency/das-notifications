using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.Notifications.Domain.Configuration;

namespace SFA.DAS.Notifications.Application.Queries.GetEmailServiceTemplateId
{
    public class GetEmailServiceTemplateIdQueryHandler : IAsyncRequestHandler<GetEmailServiceTemplateIdQuery, GetEmailServiceTemplateIdQueryResponse>
    {
        private readonly ITemplateConfigurationService _templateConfigurationService;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public GetEmailServiceTemplateIdQueryHandler(ITemplateConfigurationService templateConfigurationService)
        {
            _templateConfigurationService = templateConfigurationService;
        }

        public async Task<GetEmailServiceTemplateIdQueryResponse> Handle(GetEmailServiceTemplateIdQuery message)
        {
            if (IsGuid(message.TemplateId))
            {
                _logger.Info($"Request to send template {message.TemplateId} received using email service id");
                return new GetEmailServiceTemplateIdQueryResponse
                {
                    EmailServiceTemplateId = message.TemplateId
                };
            }

            var templateConfiguration = await _templateConfigurationService.GetAsync();
            var emailServiceTemplateId = templateConfiguration.EmailServiceTemplates.SingleOrDefault(
                    t => t.Id.Equals(message.TemplateId, StringComparison.CurrentCultureIgnoreCase))?.EmailServiceId;
            return new GetEmailServiceTemplateIdQueryResponse
            {
                EmailServiceTemplateId = emailServiceTemplateId
            };
        }


        private bool IsGuid(string value)
        {
            Guid x;
            return Guid.TryParse(value, out x);
        }
    }
}
