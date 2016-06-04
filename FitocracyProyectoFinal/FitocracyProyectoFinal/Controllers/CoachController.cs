using FitocracyProyectoFinal.Models;
using FitocracyProyectoFinal.ViewModels;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitocracyProyectoFinal.Controllers
{
    public class CoachController : Controller
    {
        private MongoDBcontext _dbContext;
        private Usuario _usuario;
        public CoachController()
        {
            _dbContext = new MongoDBcontext();
        }
        public Usuario usuario
        {
            get
            {
                return _usuario = (Usuario)Session["infoUsuario"];
            }
            set
            {
                this._usuario = value;
            }
        }

        public ActionResult Home()
        {
            List<Entrenamientos> entrenamientosList = _dbContext.Entrenamientos.Find<Entrenamientos>(new BsonDocument()).ToList();
            List<Entrenadores> entrenadoresList = _dbContext.Entrenadores.Find<Entrenadores>(new BsonDocument()).ToList();

            EntrenamientoEntrenadoresVM vM = new EntrenamientoEntrenadoresVM(entrenadoresList, entrenamientosList);

            return View(vM);
        }


        public ActionResult irAdetalleEntrenamiento(string idEntrenador, string idEntrenamiento)
        {
            var host = Request.Url.Host;
            var port = Request.Url.Port;

            var entrenador = _dbContext.Entrenadores.Find<Entrenadores>(x => x._id == idEntrenador).SingleOrDefault();
            var entrenamiento = _dbContext.Entrenamientos.Find<Entrenamientos>(x => x._id == idEntrenamiento).SingleOrDefault();
            EntrenamientoEntrenadoresVM vM = new EntrenamientoEntrenadoresVM(entrenador, entrenamiento);
            TempData["vm"] = vM;

            return Redirect("http://" + host + ":" + port + "/#/detalleEntrenamiento");
        }

        public ActionResult detalleEntrenamiento()
        {
            return View();
        }
    }
}