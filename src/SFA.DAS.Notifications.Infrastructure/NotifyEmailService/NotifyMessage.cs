using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.Notifications.Infrastructure.NotifyEmailService
{
    public class NotifyMessage
    {
        [JsonProperty(PropertyName = "to")]
        public string To { get; set; }
        [JsonProperty(PropertyName = "template")]
        public string Template { get; set; }
        [JsonProperty(PropertyName = "personalisation")]
        public Dictionary<string, string> Personalisation { get; set; }
        [JsonProperty(PropertyName = "reference")]
        public object Reference { get; set; }
        [JsonProperty(PropertyName = "systemId")]
        public string SystemId { get; set; }
    }
}