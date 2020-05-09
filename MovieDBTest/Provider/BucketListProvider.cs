using MongoDB.Bson;
using MovieDBTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDBTest.Provider
{
    public class BucketListProvider
    {
        public BucketListOverviewViewModel GetBucketListOverviewViewModel(User signedInUser)
        {
            var bucketListModel = new BucketListOverviewViewModel();
            bucketListModel.userBucketLists = new List<BucketListOverView>();

            //Für alle Bucketlists des Users
            foreach (var bucketlist in signedInUser.BucketListsInvolvedIn)
            {
                var newOverview = new BucketListOverView();
                var provider = new MongoDbProvider();

                //Aktuelle Bucketlist auslesen
                var fullList = provider.GetBucketList(bucketlist);
                var users = new List<User>();

                //Alle User der Bucketlist holen
                foreach (var userId in fullList.UsersInListId)
                {
                    users.Add(provider.GetByObjectId<User>(userId, Const.MongoDbConst.CollectionUsers));
                }

                //Model füllen
                newOverview.ListId = bucketlist;
                newOverview.InvolvedUser = users;
                newOverview.BucketListName = fullList.Name;

                bucketListModel.userBucketLists.Add(newOverview);
            }

            return bucketListModel;
        }

        public BucketListViewModel GetBucketListViewModel(User signedInUser, string id)
        {
            var bucketListViewModel = new BucketListViewModel();
            //ObjectId holen
            var bucketListId = new ObjectId(id);

            var dbProvider = new MongoDbProvider();
            var movProvider = new MovieDetailProvider();
            //Bucketlist aus DB holen
            var bucketList = dbProvider.GetBucketList(bucketListId);

            var movieList = new List<MovieViewModel>();
            //Zu allen Movies die genauen Details suchen
            foreach (var movieId in bucketList.MoviesToWatchIds)
            {
                var movieDetails = movProvider.GetMovieViewModel(movieId, bucketListId, signedInUser);

                movieList.Add(movieDetails);
            }

            //Name und Filme in Viewmodel setzen 
            bucketListViewModel.Name = bucketList.Name;
            bucketListViewModel.Movies = movieList; 
            bucketListViewModel.ListId = bucketListId;
            bucketListViewModel.SignedInUser = signedInUser;

            return bucketListViewModel;
        }
    }
}
