using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDBTest.Models
{
    public class BucketListViewModel
    {
        public String Name { get; set; }

        public ObjectId ListId { get; set; }

        public List<MovieViewModel> Movies { get; set; }

        public User SignedInUser { get; set; }

    }
}
