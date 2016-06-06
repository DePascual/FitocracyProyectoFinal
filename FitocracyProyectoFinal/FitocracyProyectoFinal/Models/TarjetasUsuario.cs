using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitocracyProyectoFinal.Models
{
    [BsonIgnoreExtraElements(true)]
    public class TarjetasUsuario
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }      
        public string CardNumber { get; set; }       
        public string SecurityCode { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
    }
}