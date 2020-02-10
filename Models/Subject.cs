using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradesNotification.Models
{
    public class Subject
    {
        [BsonElement("subject")]
        [JsonProperty("subject")]
        public string Name { get; set; }

        [BsonElement("semester")]
        [JsonProperty("semester")]
        public int Semester { get; set; }

        [BsonElement("test1")]
        [JsonProperty("test1")]
        public string Test1 { get; set; }

        [BsonElement("test2")]
        [JsonProperty("test2")]
        public string Test2 { get; set; }

        [BsonElement("test3")]
        [JsonProperty("test3")]
        public string Test3 { get; set; }

        [BsonElement("test4")]
        [JsonProperty("test4")]
        public string Test4 { get; set; }

        [BsonElement("rating")]
        [JsonProperty("rating")]
        public string Rating { get; set; }

        [BsonElement("exam")]
        [JsonProperty("exam")]
        public string Exam { get; set; }

        [BsonElement("grade")]
        [JsonProperty("grade")]
        public string Grade { get; set; }

    }
}
