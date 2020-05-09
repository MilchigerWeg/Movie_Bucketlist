using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MovieDBTest.Models;

namespace MovieDBTest.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //Wenn schon angemeldet -> kein Userdialog
            if (GetSignedInUser() != null)
            {
                return Redirect("~/Home/BucketListOverview");
            }

            var model = new UserLoginViewModel();
            model.NonTechnicalError = new NonTechnicalErrorViewModel();

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult Index(UserLoginViewModel userViewModel)
        {   
            //Wenn kein model -> eigentlich technischer Fehler
            if (userViewModel == null)
            {
                userViewModel.NonTechnicalError = Const.ErrorConst.UnvollstaendigerUser;
                return View("Index", userViewModel);
            }

            //User suchen
            var dbProvider = new Provider.MongoDbProvider();
            var userFromDb = dbProvider.GetUser(userViewModel.User.Name);

            //User nicht gefunden -> Fehler
            if (userFromDb == null)
            {
                userViewModel.NonTechnicalError = Const.ErrorConst.UngueltigerUserOderPwd;
                return View("Index", userViewModel);
            }

            //Password passt nicht -> gleicher Fehler
            if (Provider.CryptoProvider.ComputeSha256Hash(userViewModel.User.Password)
                != userFromDb.Password)
            {
                userViewModel.NonTechnicalError = Const.ErrorConst.UngueltigerUserOderPwd;
                return View("Index", userViewModel);
            }
            //Usercookie setzen
            SetUserCookie(userViewModel.User.Name);

            //Weiter zu Bucketlists
            return Redirect("~/Home/BucketListOverview");
        }

        /// <summary>
        /// View über alle BucketLists eines Users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult BucketListOverview()
        {
            //Angemeldeter User checken 
            var signedInUser = GetSignedInUser();
            //unangemeldet -> Fehler
            if(signedInUser == null)
            {
                return View("NonTechnicalError", Const.ErrorConst.InvalidSession);
            }
            //angemeldet -> session verlängern
            else
            {
                RefreshCookieSession();
            }

            //wenn nur eine bucketlist angelegt -> weiterleiten
            if(signedInUser.BucketListsInvolvedIn.Count == 1)
            {
                return Redirect("BucketList?id=" + signedInUser.BucketListsInvolvedIn.First());
            }

            var bucketListModel = new Provider.BucketListProvider().GetBucketListOverviewViewModel(signedInUser);

            return View("BucketListOverview", bucketListModel);
        }

        /// <summary>
        /// View für eine einzelne Bucketlist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult BucketList(string id)
        {
            //Angemeldeter User checken 
            var signedInUser = GetSignedInUser();
            //unangemeldet -> Fehler
            if (signedInUser == null)
            {
                return View("NonTechnicalError", Const.ErrorConst.InvalidSession);
            }
            //angemeldet -> session verlängern
            else
            {
                RefreshCookieSession();
            }

            //Gucken, ob user für bucketlist berechtigt ist
            if (signedInUser.BucketListsInvolvedIn.Any(b => b.ToString() == id))
            {
                return View(new Provider.BucketListProvider().GetBucketListViewModel(signedInUser, id));
            }
            //unberechtigt -> Fehler
            else
            {
                return View("NonTechnicalError", Const.ErrorConst.BucketlistNichtExistent);
            }

            
        }

        [HttpPost]
        public IActionResult CreateUser(UserLoginViewModel newUser)
        {
            var dbProvider = new Provider.MongoDbProvider();
            if(dbProvider.GetUser(newUser.User.Name) != null)
            {
                newUser.NonTechnicalError = Const.ErrorConst.NutzerExistiertSchon;
                return View("CreateUser", newUser);
            }

            //Passwort hashen -> minimal security
            newUser.User.Password = Provider.CryptoProvider.ComputeSha256Hash(newUser.User.Password);

            dbProvider.AddAsync<User>(newUser.User, Const.MongoDbConst.CollectionUsers).Wait();
            SetUserCookie(newUser.User.Name);

            return Redirect("Index");
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            //erstmal potenziellen aktuellen Nutzer abmelden
            SignOutFromSession();

            //Model initialisieren 
            var model = new UserLoginViewModel();
            model.NonTechnicalError = new NonTechnicalErrorViewModel();

            return View("CreateUser", model);
        }

        [HttpGet]
        public IActionResult EditBucketList(string listId)
        {

            var signedInUser = GetSignedInUser();
            BucketListCreateModel model = new BucketListCreateModel();

            if (!String.IsNullOrEmpty(listId))
            {
                var viewModel = new Provider.BucketListProvider().GetBucketListViewModel(signedInUser, listId);

                model.ListId = viewModel.ListId;
                model.Movies = viewModel.Movies;
                model.Name = viewModel.Name;            
            }

            model.UsersInBucketList.Add(signedInUser);

            return View("EditBucketList", model);
        }

        public IActionResult MovieDetail(string objectId, string bucketListId)
        {
            //Angemeldeter User checken 
            var signedInUser = GetSignedInUser();
            //unangemeldet -> Fehler
            if (signedInUser == null)
            {
                return View("NonTechnicalError", Const.ErrorConst.InvalidSession);
            }
            //angemeldet -> session verlängern
            else
            {
                RefreshCookieSession();
            }

            var provider = new Provider.MovieDetailProvider();

            //Filmdetails aus DB laden
            var movieViewModel = provider.GetMovieViewModel(new ObjectId(objectId), new ObjectId(bucketListId), signedInUser);

            return View("MovieDetail", movieViewModel);
        }
                

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }        
    }
}
