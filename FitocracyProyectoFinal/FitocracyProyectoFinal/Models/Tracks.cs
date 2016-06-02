using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitocracyProyectoFinal.Models
{
    [BsonIgnoreExtraElements(true)]
    public class Tracks
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Nombre { get; set; }
        public string Link { get; set; }
        public int Series { get; set; }
        public int Repeticiones { get; set; }
    }
}