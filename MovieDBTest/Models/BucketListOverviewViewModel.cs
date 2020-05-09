using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDBTest.Models
{
    public class BucketListOverviewViewModel
    {
        public List<BucketListOverView> userBucketLists { get; set; }        
    }

    public class BucketListOverView
    {
        public string BucketListName { get; set; }
        public ObjectId ListId { get; set; }
        public List<User> InvolvedUser { get; set; }

    }
}
