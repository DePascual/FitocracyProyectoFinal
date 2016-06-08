using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitocracyProyectoFinal.Models
{
    [BsonIgnoreExtraElements(true)]
    public class Entrenamientos
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Familia { get; set; }
        public string NombreEntrenamiento { get; set; }
        public string Id_Entrenador { get; set; }
        public string Precio { get; set; }
        public string Duracion { get; set; }
        public string Descripcion { get; set; }
        public byte[] Foto { get; set; }
        public string Who { get; set; }
        public string Goals { get; set; }
        public string Requirements { get; set; }
        public string WhatYouGet { get; set; }
        public string LastModification { get; set; }
    }
}