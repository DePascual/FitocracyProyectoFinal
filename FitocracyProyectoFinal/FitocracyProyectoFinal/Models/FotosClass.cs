using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitocracyProyectoFinal.Models
{
    public class FotosClass: Controller
    {
        private MongoDBcontext _dbContext;
        public FotosClass()
        {
            _dbContext = new MongoDBcontext();
        }

        public ActionResult downloadPhoto(string id)
        {
            var img = _dbContext.Usuarios.Find<Usuario>(x => x._id == id).Project<byte[]>(Builders<Usuario>.Projection.Include(x => x.Foto)).SingleOrDefault();
            return File(img, "image/jpg");
        }

        public ActionResult showFotoEntrenamiento(string id)
        {
            var img = _dbContext.Entrenamientos.Find<Entrenamientos>(x => x._id == id).Project<byte[]>(Builders<Entrenamientos>.Projection.Include(x => x.Foto)).SingleOrDefault();
            return File(img, "image/jpg");
        }

        public ActionResult showEntrenador(string id)
        {
            var img = _dbContext.Entrenadores.Find<Entrenadores>(x => x._id == id).Project<byte[]>(Builders<Entrenadores>.Projection.Include(x => x.Foto)).SingleOrDefault();
            return File(img, "image/jpg");
        }
    }
}