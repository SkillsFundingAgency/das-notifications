using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Notifications.Api.Client.UnitTests2
{
    public class FakeResponseHandler : DelegatingHandler
    {
        private readonly Dictionary<TestRequest, HttpResponseMessage> _fakeResponses = new Dictionary<TestRequest, HttpResponseMessage>();

        public void AddFakeResponse(TestRequest request, HttpResponseMessage responseMessage)
        {
            _fakeResponses.Add(request, responseMessage);
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var testRequest = new TestRequest(request.RequestUri, await request.Content.ReadAsStringAsync());

            if (_fakeResponses.ContainsKey(testRequest))
            {
                return _fakeResponses[testRequest];
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    RequestMessage = request,
                    Content = new StringContent(string.Empty)
                };
            }
        }
    }

    public class TestRequest
    {
        public TestRequest(Uri uri, string requestContent)
        {
            Uri = uri;
            RequestContent = requestContent;
        }

        public Uri Uri { get; private set; }
        public string RequestContent { get; private set; }

        public override int GetHashCode()
        {
            return Uri.ToString().GetHashCode() ^ RequestContent.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            TestRequest otherRequest;
            otherRequest = (TestRequest)obj;

            return (obj.GetHashCode() == otherRequest.GetHashCode());
        }
    }
}
