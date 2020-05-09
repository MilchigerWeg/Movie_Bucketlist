using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDBTest.Models
{
    [BsonDiscriminator("bucketlist")]
    public class BucketList
    {
        public BucketList()
        {
            MoviesToWatchIds = new List<ObjectId>();
            UsersInListId = new List<ObjectId>();
        }

        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public ObjectId ListId { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("moviesToWatchIds")]
        public List<ObjectId> MoviesToWatchIds { get; set; }
        [BsonElement("usersInListId")]
        public List<ObjectId> UsersInListId { get; set; }
    }
}
