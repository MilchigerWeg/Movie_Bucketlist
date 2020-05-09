using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace MovieDBTest.Provider
{
    public class MovieDetailProvider
    {
        private string baseUrl = "http://www.omdbapi.com/";

        public Models.MovieModel GetMovieModel(string title)
        {
            //Parameter aus API-Key, Titel und Erscheinungsjahr
            //Fragezeichen am Anfang muss sein
            string urlParams = "?apikey=cf7659c9&plot=full&type=movie&t=" + title ;
                        
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(baseUrl);

            HttpResponseMessage httpResponse = client.GetAsync(urlParams).Result;

            //Client disposen (GC undso)
            client.Dispose();

            if (httpResponse.IsSuccessStatusCode)
            {
                var jsonString = httpResponse.Content.ReadAsStringAsync().Result;

                return Newtonsoft.Json.JsonConvert.DeserializeObject<Models.MovieModel>(jsonString);
            }            
            
            return null;
        }

        public Models.MovieModel GetMovieModelByImdbId(string imdbId)
        {
            //Parameter aus API-Key, Titel und Erscheinungsjahr
            //Fragezeichen am Anfang muss sein
            string urlParams = "?apikey=cf7659c9&plot=full&type=movie&i=" + imdbId;

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(baseUrl);

            HttpResponseMessage httpResponse = client.GetAsync(urlParams).Result;

            //Client disposen (GC undso)
            client.Dispose();

            if (httpResponse.IsSuccessStatusCode)
            {
                var jsonString = httpResponse.Content.ReadAsStringAsync().Result;

                return Newtonsoft.Json.JsonConvert.DeserializeObject<Models.MovieModel>(jsonString);
            }

            return null;
        }

        public Models.MovieViewModel GetMovieViewModel(ObjectId movieId, ObjectId bucketListId, Models.User signedInUser)
        {
            var provider = new Provider.MongoDbProvider();
            var movieViewModel = new Models.MovieViewModel();
            movieViewModel.movieDetails = provider.GetByObjectId<Models.MovieModel>(movieId, Const.MongoDbConst.CollectionMovies);
            movieViewModel.SignedInUser = signedInUser;

            //wenn Movie aus bucketlist kommt
            if (bucketListId != null)
            {
                movieViewModel.InvolvedUsers = provider.GetUsersFromBucketList(bucketListId);
            }

            return movieViewModel;
        }
    }
}
