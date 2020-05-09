using System;
using System.Collections.Generic;
using System.Linq;
using MovieDBTest.Models;

namespace MovieDBTest.Const
{
    public class ErrorConst
    {
        public readonly static NonTechnicalErrorViewModel BucketlistNichtExistent = new NonTechnicalErrorViewModel()
        {
            Info = "Die gesuchte Bucketlist existiert nicht",
            Title = "Bucketlist nicht existent"
        };

        public readonly static NonTechnicalErrorViewModel InvalidSession = new NonTechnicalErrorViewModel()
        {
            Info = "Es besteht keine valide Session",
            Title = "Session invalide"
        };

        public readonly static NonTechnicalErrorViewModel UngueltigerUserOderPwd = new NonTechnicalErrorViewModel()
        {
            Info = "Das angegebene Passwort oder der angegebene User sind ungültig",
            Title = "Ungültiges Passwort oder User"
        };

        public readonly static NonTechnicalErrorViewModel UnvollstaendigerUser = new NonTechnicalErrorViewModel()
        {
            Info = "Der angegebene User ist nicht vollständig übermittelt worden",
            Title = "Unvollständiger Nutzer"
        };

        public readonly static NonTechnicalErrorViewModel NutzerExistiertSchon = new NonTechnicalErrorViewModel()
        {
            Info = "Der angegebene Username wird bereits von einer anderen Person verwendet",
            Title = "Nutzername existiert schon"
        };
    }
}
