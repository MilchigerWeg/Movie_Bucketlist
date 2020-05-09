using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MovieDBTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDBTest.Provider
{
    public class MongoDbProvider
    {
        static MongoClient client = new MongoClient(Const.MongoDbConst.ConnectionString);
       
        public T GetByObjectId<T>(ObjectId id, string collectionName)
        {
            IMongoDatabase db = client.GetDatabase(Const.MongoDbConst.DbName);

            //Collection aller Filme auswählen
            IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>(collectionName);


            var filter = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", id);

            var result = collection.FindSync(filter).FirstOrDefault();

            if(result == null)
            {
                return default(T);
            }

            var instance = BsonSerializer.Deserialize<T>(result);

            return instance;
        }

        public async Task AddAsync<T>(T newInstance, string collectionName)
        {
            IMongoDatabase db = client.GetDatabase(Const.MongoDbConst.DbName);

            //Collection aller Filme auswählen
            IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>(collectionName);

            await collection.InsertOneAsync(newInstance.ToBsonDocument());
        }

        public async Task ReplaceBucketList(BucketList updatedBucketList)
        {
            IMongoDatabase db = client.GetDatabase(Const.MongoDbConst.DbName);

            var filter = Builders<BucketList>.Filter.Eq(s => s.ListId, updatedBucketList.ListId);
            IMongoCollection<BucketList> collection = db.GetCollection<BucketList>(Const.MongoDbConst.CollectionBucketList);
            await collection.ReplaceOneAsync(filter, updatedBucketList);            
        }

        public BucketList GetBucketList(ObjectId id)
        {
            IMongoDatabase db = client.GetDatabase(Const.MongoDbConst.DbName);

            //Collection aller Filme auswählen
            IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>(Const.MongoDbConst.CollectionBucketList);


            var filter = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", id);
            var bucketList = BsonSerializer.Deserialize<BucketList>(collection.FindSync(filter).FirstOrDefault());

            return bucketList;
        }

        public List<MovieModel> GetBucketListMovies(ObjectId id)
        {
            IMongoDatabase db = client.GetDatabase(Const.MongoDbConst.DbName);

            //Collection aller Filme auswählen
            IMongoCollection<BsonDocument> bucketlistCollection = db.GetCollection<BsonDocument>(Const.MongoDbConst.CollectionBucketList);


            var filter = new FilterDefinitionBuilder<BsonDocument>().Eq("_id", id);
            var bucketList = BsonSerializer.Deserialize<BucketList>(bucketlistCollection.FindSync(filter).FirstOrDefault());

            var movies = new List<MovieModel>();
            IMongoCollection<BsonDocument> movieCollection = db.GetCollection<BsonDocument>(Const.MongoDbConst.CollectionMovies);

            foreach (var movie in bucketList.MoviesToWatchIds)
            {
                movies.Add(GetByObjectId<MovieModel>(movie, Const.MongoDbConst.CollectionMovies));
            }

            return movies;
        }

        public List<User> GetUsersFromBucketList(ObjectId bucketListId)
        {
            var bucketList = GetBucketList(bucketListId);
            var users = new List<User>();

            foreach(var userId in bucketList.UsersInListId)
            {
                var newUser = new User();
                newUser = GetByObjectId<User>(userId, Const.MongoDbConst.CollectionUsers);
                users.Add(newUser);
            }

            return users;
        }

        public void AddSeenMovieToUser(ObjectId movieId, ObjectId userId)
        {
            var user = GetByObjectId<User>(userId, Const.MongoDbConst.CollectionUsers);

            //User hat film schon gesehen
            if (user.MoviesWatchedIds.Contains(movieId))
            {
                return;
            }

            IMongoDatabase db = client.GetDatabase(Const.MongoDbConst.DbName);

            //Collection aller Filme auswählen
            IMongoCollection<User> collection = db.GetCollection<User>(Const.MongoDbConst.CollectionUsers);


            var filter = new FilterDefinitionBuilder<User>().Eq("_id", userId);
            var update = Builders<User>.Update.Push<ObjectId>(u => u.MoviesWatchedIds, movieId);

            collection.FindOneAndUpdateAsync(filter, update);
        }

        public void RemoveSeenMovieFromUser(ObjectId movieId, ObjectId userId)
        {
            var user = GetByObjectId<User>(userId, Const.MongoDbConst.CollectionUsers);

            //User hat film schon gesehen
            if (!user.MoviesWatchedIds.Contains(movieId))
            {
                return;
            }

            IMongoDatabase db = client.GetDatabase(Const.MongoDbConst.DbName);

            //Collection aller Filme auswählen
            IMongoCollection<User> collection = db.GetCollection<User>(Const.MongoDbConst.CollectionUsers);


            //var filter = new FilterDefinitionBuilder<User>().Eq("_id", userId);
            var update = Builders<User>.Update.Pull(u => u.MoviesWatchedIds, movieId);

            collection.FindOneAndUpdateAsync(u => u.Id == userId, update).Wait();
        }

        public User GetUser(string userName)
        {
            IMongoDatabase db = client.GetDatabase(Const.MongoDbConst.DbName);

            //Collection aller Filme auswählen
            IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>(Const.MongoDbConst.CollectionUsers);


            var filter = new FilterDefinitionBuilder<BsonDocument>().Eq("name", userName);
            var userDocument = collection.FindSync(filter).FirstOrDefault();

            if(userDocument == null)
            {
                return null;
            }

            var user = BsonSerializer.Deserialize<User>(userDocument);

            return user;
        }
        
        public MovieModel GetMovieByImdbId(string imdbId)
        {
            IMongoDatabase db = client.GetDatabase(Const.MongoDbConst.DbName);

            //Collection aller Filme auswählen
            IMongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>(Const.MongoDbConst.CollectionMovies);


            var filter = new FilterDefinitionBuilder<BsonDocument>().Eq("imdbID", imdbId);

            var result = collection.FindSync(filter).FirstOrDefault();
            if (result == null)
            {
                return null;
            }

            var movie = BsonSerializer.Deserialize<MovieModel>(result);

            return movie;
        }

        public void AddBucketListToUser(ObjectId userId, ObjectId bucketListId)
        {
            var user = GetByObjectId<User>(userId, Const.MongoDbConst.CollectionUsers);

            //User hat film schon gesehen
            if (user.BucketListsInvolvedIn.Contains(bucketListId))
            {
                return;
            }

            IMongoDatabase db = client.GetDatabase(Const.MongoDbConst.DbName);

            //Collection aller Filme auswählen
            IMongoCollection<User> collection = db.GetCollection<User>(Const.MongoDbConst.CollectionUsers);


            var filter = new FilterDefinitionBuilder<User>().Eq("_id", userId);
            var update = Builders<User>.Update.Push<ObjectId>(u => u.BucketListsInvolvedIn, bucketListId);

            collection.FindOneAndUpdateAsync(filter, update);
        }

        public void RemoveBucketListFromUser(ObjectId userId, ObjectId bucketListId)
        {
            var user = GetByObjectId<User>(userId, Const.MongoDbConst.CollectionUsers);

            //User hat film schon gesehen
            if (!user.BucketListsInvolvedIn.Contains(bucketListId))
            {
                return;
            }

            IMongoDatabase db = client.GetDatabase(Const.MongoDbConst.DbName);

            //Collection aller Filme auswählen
            IMongoCollection<User> collection = db.GetCollection<User>(Const.MongoDbConst.CollectionUsers);


            //var filter = new FilterDefinitionBuilder<User>().Eq("_id", userId);
            var update = Builders<User>.Update.Pull(u => u.BucketListsInvolvedIn, bucketListId);

            collection.FindOneAndUpdateAsync(u => u.Id == userId, update).Wait();
        }

        //One-Time Migration
        [Obsolete]
        public async void InitaliseNewDB(List<Models.MovieModel> movieInfos)
        {
            //Connection zur db aufbauen
            IMongoDatabase db = client.GetDatabase(Const.MongoDbConst.DbName);

            //alle Collections erstellen 
            db.CreateCollection("Users");
            db.CreateCollection("Movies");
            db.CreateCollection("BucketLists");
            //Für alle Colections bearbeitbare Objekte erzeugen
            var userCollection = db.GetCollection<BsonDocument>("Users");
            var movieCollection = db.GetCollection<BsonDocument>("Movies");
            var bucketListCollection = db.GetCollection<BsonDocument>("BucketLists");


            //Liste, in die alle neu erzeugten User kommen
            List<BsonDocument> movieList = new List<BsonDocument>();
            //Liste füllen
            foreach (var movie in movieInfos)
            {
                movieList.Add(new BsonDocument(movie.ToBsonDocument()));
            }

            //neuer User
            var user = new User()
            {
                Name = "test",
                Password = "1234",
                MoviesWatchedIds = new List<ObjectId>(),
                BucketListsInvolvedIn = new List<ObjectId>(),
                Id = new ObjectId()
            };

            //neue BucketList
            var bucketList = new BucketList()
            {
                Name = "testList",
                ListId = new ObjectId(),
                MoviesToWatchIds = new List<ObjectId>(),
                UsersInListId = new List<ObjectId>() { user.Id }
            };

            //Alle Collections schreiben
            await movieCollection.InsertManyAsync(movieList);
            await userCollection.InsertOneAsync(user.ToBsonDocument());
            await bucketListCollection.InsertOneAsync(bucketList.ToBsonDocument());                                    
        }       

    }
}
