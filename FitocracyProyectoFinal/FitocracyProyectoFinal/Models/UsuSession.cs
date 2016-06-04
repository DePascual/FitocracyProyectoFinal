using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitocracyProyectoFinal.Models
{
    public class UsuSession
    {
        public string _id { get; set; }
        public string Username { get; set; }

        public UsuSession(string _id, string Username)
        {
            this._id = _id;
            this.Username = Username;
        }
    }
}