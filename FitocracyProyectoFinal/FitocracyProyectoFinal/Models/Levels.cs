using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitocracyProyectoFinal.Models
{
    [BsonIgnoreExtraElements(true)]
    public class Levels
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public int Level { get; set; }
        public int Points { get; set; }
    }
}