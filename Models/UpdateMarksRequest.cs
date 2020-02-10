using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradesNotification.Models
{
    public class UpdateMarksRequest
    {
        [JsonProperty("ritm_login")]
        public string RitmLogin { get; set; }
    }
}
