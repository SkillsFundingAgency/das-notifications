using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Notifications.Application.Interfaces;
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
            var storedMessage = await _messageRepository.Get(message.MessageType, message.MessageId);

            return new GetMessageQueryResponse
            {
                MessageType = storedMessage.MessageType,
                MessageId = storedMessage.MessageId,
                Content = storedMessage.Content
            };
        }
    }
}