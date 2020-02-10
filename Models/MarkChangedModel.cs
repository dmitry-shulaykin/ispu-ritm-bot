using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradesNotification.Models
{
    public class MarkChangedModel
    {
        [JsonProperty("student")]
        public string Student { get; set; }

        [JsonProperty("semester")]
        public int Semester { get; set; }

        [JsonProperty("subject_name")]
        public string SubjectName { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("prev_value")]
        public string PrevValue { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
