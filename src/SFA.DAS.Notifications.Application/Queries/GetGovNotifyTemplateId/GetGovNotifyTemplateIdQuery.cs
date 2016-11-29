using MediatR;

namespace SFA.DAS.Notifications.Application.Queries.GetGovNotifyTemplateId
{
    public class GetGovNotifyTemplateIdQuery : IAsyncRequest<GetGovNotifyTemplateIdQueryResponse>
    {
        public string TemplateId { get; set; }
    }
}
