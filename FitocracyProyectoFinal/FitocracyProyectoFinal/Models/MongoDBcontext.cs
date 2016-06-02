using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using System.Configuration;

namespace FitocracyProyectoFinal.Models
{
    public class MongoDBcontext
    {
        public const string CONNECTION_STRING_NAME = "mongodb://Tpg17pT9vKIqVbqze4zKltGiQ1fcC/a36ZLWMPnYQLk=@ds017582.mlab.com:17582/fitocracymdb";
        public const string DATABASE_NAME = "fitocracymdb";
        public const string USUARIOS_COLLECTION_NAME = "usuarios";
        public const string ENTRENADORES_COLLECTION_NAME = "entrenadores";
        public const string ENTRENAMIENTOS_COLLECTION_NAME = "entrenamientos";
        public const string LEVELS_COLLECTION_NAME = "levels";
        public const string TRACKS_COLLECTION_NAME = "tracks";
        public const string WORKOUTS_COLLECTION_NAME = "workouts";


        private static readonly IMongoClient _client;
        private static readonly IMongoDatabase _database;

        static MongoDBcontext()
        {
            var credencialesOK = desencriptaConnectionString();
            var nuevaConexion = "mongodb://" + credencialesOK + "@ds017582.mlab.com:17582/fitocracymdb";

            _client = new MongoClient(nuevaConexion);
            _database = _client.GetDatabase(DATABASE_NAME);
        }

        public IMongoClient Client
        {
            get { return _client; }
        }

        public IMongoDatabase Database
        {
            get { return _database; }
        }

        public IMongoCollection<Usuario> Usuarios
        {
            get { return _database.GetCollection<Usuario>(USUARIOS_COLLECTION_NAME); }
        }

        public IMongoCollection<Entrenadores> Entrenadores
        {
            get { return _database.GetCollection<Entrenadores>(ENTRENADORES_COLLECTION_NAME); }
        }

        public IMongoCollection<Entrenamientos> Entrenamientos
        {
            get { return _database.GetCollection<Entrenamientos>(ENTRENAMIENTOS_COLLECTION_NAME); }
        }

        public IMongoCollection<Levels> Levels
        {
            get { return _database.GetCollection<Levels>(LEVELS_COLLECTION_NAME); }
        }

        public IMongoCollection<Tracks> Tracks
        {
            get { return _database.GetCollection<Tracks>(TRACKS_COLLECTION_NAME); }
        }

        public IMongoCollection<Workouts> Workouts
        {
            get { return _database.GetCollection<Workouts>(WORKOUTS_COLLECTION_NAME); }
        }

        private static string desencriptaConnectionString()
        {
            var uriEncriptada = CONNECTION_STRING_NAME;
            EncriptacionClass encryt = new EncriptacionClass();
            var credencialesEncriptadas = uriEncriptada.Split(new[] { '@' })[0].Split(new[] { "mongodb://" }, StringSplitOptions.None)[1];
            string credencialesOK = encryt.Desencrit(credencialesEncriptadas);
            return credencialesOK;
        }

    }
}