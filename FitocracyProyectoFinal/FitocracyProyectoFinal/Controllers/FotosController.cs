using FitocracyProyectoFinal.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitocracyProyectoFinal.Controllers
{
    public class FotosController : Controller
    {
        private MongoDBcontext _dbContext;
        public FotosController()
        {
            _dbContext = new MongoDBcontext();
        }

        public ActionResult downloadPhoto(string id)
        {
            var usu = _dbContext.Usuarios.Find<Usuario>(x => x._id == id).SingleOrDefault();
            return File(usu.Foto, "image/jpg");
        }

        public ActionResult showFotoEntrenamiento(string id)
        {           
            var entrenamiento = _dbContext.Entrenamientos.Find<Entrenamientos>(x => x._id == id).SingleOrDefault();
            return File(entrenamiento.Foto, "image/jpg");
        }

        public ActionResult showEntrenador(string id)
        {          
            var usu = _dbContext.Entrenadores.Find<Entrenadores>(x => x._id == id).SingleOrDefault();
            return File(usu.Foto, "image/jpg");
        }
    }
}