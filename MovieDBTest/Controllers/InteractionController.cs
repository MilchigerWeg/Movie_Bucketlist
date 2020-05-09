using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieDBTest.Models;

namespace MovieDBTest.Controllers
{
    public class InteractionController : ControllerBase
    {
        [HttpGet]
        public void SetMovieWatchedStatus(string movieId, bool isSetToWatched)
        {
            var signedInUser = GetSignedInUser();

            if(signedInUser == null)
            {
                return;
            }

            var provider = new Provider.MongoDbProvider();
            var movieObjId = new MongoDB.Bson.ObjectId(movieId);
            
            if (isSetToWatched)
            {
                provider.AddSeenMovieToUser(movieObjId, signedInUser.Id);
            }
            else
            {
                provider.RemoveSeenMovieFromUser(movieObjId, signedInUser.Id);
            }
        }

        [HttpGet]
        public PartialViewResult GetNewBucketListMovie(string title)
        {
            var movieModel = new Provider.MovieDetailProvider().GetMovieModel(title);

            if(movieModel!= null && movieModel.Title != null)
            {
                return PartialView("~/Views/Interaction/MovieModelPartial.cshtml", movieModel);
            }

            return null;
        }

        [HttpGet]
        public String GetExistingUser(string userName)
        {
            var provider = new Provider.MongoDbProvider();

            return provider.GetUser(userName)?.ToString() ?? String.Empty;
        }

        [HttpPost]
        public ActionResult SaveBucketList([FromBody]BucketListMinCreateModel saveModel)
        {
            //Leeres Model -> Bad Request
            if(saveModel == null)
            {
                return StatusCode(400);
            }

            var dbProvider = new Provider.MongoDbProvider();
            var movProvider = new Provider.MovieDetailProvider();
            BucketList bucketListToSave;
            List<User> existingUsers = new List<User>();
            bool isNewList = saveModel.ListId.All(s => s == '0');

            //ListId ist nur 0 -> neue id -> neue Liste
            if (isNewList){
                bucketListToSave = new BucketList();
            }
            else
            {
                //Aktuelle Bucketlist lesen -> Beweis, dass sie existiert
                bucketListToSave = dbProvider.GetBucketList(new MongoDB.Bson.ObjectId(saveModel.ListId));
            }

            ///(neuen) Titel setzen 
            bucketListToSave.Name = saveModel.ListTitle;

            //gucken, welche Nutzer existieren
            foreach (var userId in saveModel.UserIds)
            {
                var tmpUser = dbProvider.GetByObjectId<User>(new MongoDB.Bson.ObjectId(userId), Const.MongoDbConst.CollectionUsers);
                if(tmpUser != null)
                {
                    existingUsers.Add(tmpUser);
                }
            }

            //Liste leer machen
            bucketListToSave.UsersInListId.Clear();
            //Alle Ids existierender User hinzufügen
            bucketListToSave.UsersInListId.AddRange(existingUsers.Select(m => m.Id));

            //Usern Liste zuweisen

            bucketListToSave.MoviesToWatchIds.Clear();
            foreach(var movieId in saveModel.MovieImdbIds)
            {
                //suche Film in DB
                var tmpMovie = dbProvider.GetMovieByImdbId(movieId);
                //muss angelegt werden
                if(tmpMovie == null)
                {
                    //Film aus API lesen
                    var movie = movProvider.GetMovieModelByImdbId(movieId);
                    //Film in eingener MongoDb ablegen
                    dbProvider.AddAsync<MovieModel>(movie, Const.MongoDbConst.CollectionMovies).Wait();
                    //Film wieder aus Db lesen, um Id zu bekommen (geht oft schief, wenn man ID selbst setzt)
                    tmpMovie = dbProvider.GetMovieByImdbId(movieId);
                }

                //Id wieder an Bucketlist anhängen
                bucketListToSave.MoviesToWatchIds.Add(tmpMovie.Id);
            }

            

            if (isNewList)
            {
                //BucketList schreiben
                dbProvider.AddAsync<BucketList>(bucketListToSave, Const.MongoDbConst.CollectionBucketList).Wait();
            }
            else
            {
                dbProvider.ReplaceBucketList(bucketListToSave).Wait();
            }

            //USer brauchen noch Bucketlist id
            //Bucketlistid zurückgeben -> dann wird neue liste nicht immer als neue liste nach jedem save gewertet
            
            return StatusCode(202);
        }
    }
}