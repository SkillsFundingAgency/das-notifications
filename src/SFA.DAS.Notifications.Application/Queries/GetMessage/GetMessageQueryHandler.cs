using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Notifications.Domain.Repositories;

namespace SFA.DAS.Notifications.Application.Queries.GetMessage
{
    public class GetMessageQueryHandler : IAsyncRequestHandler<GetMessageQueryRequest, GetMessageQueryResponse>
    {
        private readonly INotificationsRepository _messageRepository;

        public GetMessageQueryHandler(INotificationsRepository messageRepository)
        {
            if (messageRepository == null)
                throw new ArgumentNullException(nameof(messageRepository));

            _messageRepository = messageRepository;
        }

        public async Task<GetMessageQueryResponse> Handle(GetMessageQueryRequest message)
        {
            var notification = await _messageRepository.Get(message.Format, message.MessageId);

            return new GetMessageQueryResponse
            {
                Notification = notification
            };
        }
    }
}
