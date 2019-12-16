using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradesNotification.Models
{
    public class Student
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("chat_id")]
        public long ChatId { get; set; }

        [JsonProperty("ritm_login")]
        public string RitmLogin { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("semesters")]
        public List<Semester> Semesters { get; set; }

    }
}
