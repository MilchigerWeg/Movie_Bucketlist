using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace MovieDBTest.Models
{
    public class BucketListCreateModel : BucketListViewModel
    {
        public BucketListCreateModel()
        {
            Movies = new List<MovieViewModel>();
            UsersInBucketList = new List<User>();
        }
        public BucketListCreateModel(User signedInUser) : this()
        {
            SignedInUser = signedInUser;
            UsersInBucketList.Add(SignedInUser);
        }

        public List<User> UsersInBucketList { get; set; }
    }
}
