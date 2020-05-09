using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDBTest.Models
{
    public class MovieViewModel
    {
        public MovieModel movieDetails { get; set; }

        public List<User> InvolvedUsers { get; set; }

        public User SignedInUser { get; set; }
    }
}
