﻿using System;
using Newtonsoft.Json;

namespace SFA.DAS.Notifications.Application.DataEntities
{
    public class GovNotifyPayload
    {

        [JsonProperty("iss")]
        public string Iss { get; set; }

        [JsonProperty("iat")]
        public long Iat { get; set; }

    }
}