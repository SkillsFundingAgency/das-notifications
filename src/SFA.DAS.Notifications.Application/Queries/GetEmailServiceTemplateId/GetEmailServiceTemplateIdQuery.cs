using MediatR;

namespace SFA.DAS.Notifications.Application.Queries.GetEmailServiceTemplateId
{
    public class GetEmailServiceTemplateIdQuery : IAsyncRequest<GetEmailServiceTemplateIdQueryResponse>
    {
        public string TemplateId { get; set; }
    }
}
