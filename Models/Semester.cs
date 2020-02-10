using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradesNotification.Models
{
    public class Semester
    {
        [BsonElement("number")]
        [JsonProperty("number")]
        public int Number { get; set; }

        [BsonElement("subjects")]
        [JsonProperty("subjects")]
        public List<Subject> Subjects {get; set;}
    }
}
