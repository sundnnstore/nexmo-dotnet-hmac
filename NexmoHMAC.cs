using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Lorond.Nexmo
{
    public static class Example
    {
        public static void SignRequestWithHMAC()
        {
            // Depends on Newtonsoft's Json.NET library

            const string apiKey = "API_KEY";
            const string securitySecret = "SECURITY_SECRET";

            var parameters = new SortedDictionary<string, object>
            {
                ["api_key"] = apiKey,
                ["to"] = "<recipient>",
                ["from"] = "<sender>",
                ["text"] = "Hello from Nexmo",
                ["type"] = "text",
                ["timestamp"] = (long)Math.Floor((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds),
            };

            var data = new StringBuilder();
            foreach (var pair in parameters)
                data.AppendFormat("&{0}={1}", pair.Key, pair.Value);

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(securitySecret));
            var signatureBin = hmac.ComputeHash(Encoding.UTF8.GetBytes(data.ToString()));
            var signature = string.Join(string.Empty, signatureBin.Select(x => x.ToString("x2")));

            using (var wc = new WebClient())
            {
                wc.Headers["Content-Type"] = "application/json";

                parameters["sig"] = signature;

                var json = JsonConvert.SerializeObject(parameters, Formatting.Indented);
                var response = wc.UploadString(@"https://rest.nexmo.com/sms/json", json);

                Console.WriteLine(response);
            }
        }
    }
}