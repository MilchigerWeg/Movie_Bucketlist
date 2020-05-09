using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDBTest.Models
{
    [BsonDiscriminator("user")]
    public class User
    {
        public User()
        {
            MoviesWatchedIds = new List<ObjectId>();
            BucketListsInvolvedIn = new List<ObjectId>();
        }

        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public ObjectId Id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("moviesWatchedIds")]
        public List<ObjectId> MoviesWatchedIds { get; set; }
        [BsonElement("bucketlistsInvolvedId")]
        public List<ObjectId> BucketListsInvolvedIn { get; set; }

        public override string ToString()
        {
            var user = new { Name = this.Name, Id = this.Id.ToString() };

            return Newtonsoft.Json.JsonConvert.SerializeObject(user);
        }
    }
}
