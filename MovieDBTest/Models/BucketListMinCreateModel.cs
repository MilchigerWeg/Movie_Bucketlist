using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDBTest.Models
{
    public class BucketListMinCreateModel
    {
        public BucketListMinCreateModel()
        {
            UserIds = new List<string>();
            MovieImdbIds = new List<string>();
        }

        public string ListId { get; set; }

        public string ListTitle { get; set; }

        public List<string> UserIds { get; set; }

        public List<string> MovieImdbIds { get; set; }
    }
}
