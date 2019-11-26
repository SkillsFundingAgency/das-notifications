using System;
using JWT;
using JWT.Algorithms;
using Newtonsoft.Json;

namespace SFA.DAS.Notifications.Infrastructure.NotifyEmailService
{
    public class JwtTokenUtility
    {
        public static string CreateToken(string serviceId, string apiKey)
        {
            var payLoad = new GovNotifyPayload
            {
                Iss = serviceId,
                Iat = DateTime.UtcNow.ToUnixTime()
            };

            IJwtEncoder encoder = new JwtEncoder(new HMACSHA256Algorithm(), new NewtonsoftJsonSerializer(), new JwtBase64UrlEncoder());
            var token = encoder.Encode(payLoad, apiKey);

            return token;
        }

        internal class NewtonsoftJsonSerializer : IJsonSerializer
        {
            public string Serialize(object obj)
            {
                return JsonConvert.SerializeObject(obj);
            }

            public T Deserialize<T>(string json)
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}
