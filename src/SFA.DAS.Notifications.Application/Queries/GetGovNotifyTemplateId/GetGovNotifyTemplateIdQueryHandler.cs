using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.Notifications.Domain.Configuration;

namespace SFA.DAS.Notifications.Application.Queries.GetGovNotifyTemplateId
{
    public class GetGovNotifyTemplateIdQueryHandler : IAsyncRequestHandler<GetGovNotifyTemplateIdQuery, GetGovNotifyTemplateIdQueryResponse>
    {
        private readonly ITemplateConfigurationService _templateConfigurationService;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public GetGovNotifyTemplateIdQueryHandler(ITemplateConfigurationService templateConfigurationService)
        {
            _templateConfigurationService = templateConfigurationService;
        }

        public async Task<GetGovNotifyTemplateIdQueryResponse> Handle(GetGovNotifyTemplateIdQuery message)
        {
            if (IsGuid(message.TemplateId))
            {
                _logger.Info($"Request to send template {message.TemplateId} received using GOV Notify id");
                return new GetGovNotifyTemplateIdQueryResponse
                {
                    GovNotifyTemplateId = message.TemplateId
                };
            }

            var templateConfiguration = await _templateConfigurationService.GetAsync();
            var govNotifyTemplateId = templateConfiguration.GovNotifyTemplates.SingleOrDefault(
                    t => t.Id.Equals(message.TemplateId, StringComparison.CurrentCultureIgnoreCase))?.GovNotifyId;
            return new GetGovNotifyTemplateIdQueryResponse
            {
                GovNotifyTemplateId = govNotifyTemplateId
            };
        }


        private bool IsGuid(string value)
        {
            Guid x;
            return Guid.TryParse(value, out x);
        }
    }
}
