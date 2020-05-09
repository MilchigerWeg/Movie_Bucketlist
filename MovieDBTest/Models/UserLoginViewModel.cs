using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDBTest.Models
{
    public class UserLoginViewModel
    {
        public User User { get; set; }

        public NonTechnicalErrorViewModel NonTechnicalError { get; set; }
    }
}
