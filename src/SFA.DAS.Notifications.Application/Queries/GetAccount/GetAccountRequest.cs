using System;
using MediatR;

namespace SFA.DAS.Notifications.Application.Queries.GetAccount
{
    public class GetAccountRequest : IAsyncRequest<GetAccountResponse>
    {
        public int AccountId { get; set; }
    }
}
