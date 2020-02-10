using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;


namespace GradesNotification.Models
{
    [BsonIgnoreExtraElements]
    public class Student
    {
        [BsonElement("chat_id")]
        [JsonProperty("chat_id")]
        public long ChatId { get; set; }

        [BsonId]
        [BsonElement("ritm_login")]
        [JsonProperty("ritm_login")]
        public string RitmLogin { get; set; }

        [BsonElement("ritm_password")]
        [JsonProperty("password")]
        public string Password { get; set; }

        [BsonElement("semesters")]
        [JsonProperty("semesters")]
        public List<Semester> Semesters { get; set; } = new List<Semester>();

    }
}
