using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDBTest.Const
{
    public class MongoDbConst
    {
        public const string DbName = "MovieDB";
        public const string ConnectionString = "mongodb://localhost:27017";

        public const string CollectionBucketList = "BucketLists";
        public const string CollectionUsers = "Users";
        public const string CollectionMovies = "Movies";
    }
}
