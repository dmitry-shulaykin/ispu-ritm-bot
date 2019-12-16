using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradesNotification.Models
{
    public class NewMark
    {
        [JsonProperty("test1")]
        public string Test1 { get; set; }

        [JsonProperty("test2")]
        public string Test2 { get; set; }

        [JsonProperty("test3")]
        public string Test3 { get; set; }

        [JsonProperty("test4")]
        public string Test4 { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty("exam")]
        public string Exam { get; set; }

        [JsonProperty("grade")]
        public string Grade { get; set; }
    }
}
