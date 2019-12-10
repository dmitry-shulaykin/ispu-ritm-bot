using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradesNotification.Models
{
    public class RegisterRequest
    {
        [JsonProperty("ritm_login")]
        public string RitmLogin { get; set; }

        [JsonProperty("ritm_password")]
        public string RitmPassword { get; set; }
    }
}
