using FitocracyProyectoFinal.Models;
using FitocracyProyectoFinal.ViewModels;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitocracyProyectoFinal.Controllers
{
    public class PartialsViewsController : Controller
    {
        #region Variables
        private MongoDBcontext _dbContext;
        private Usuario _usuario;
        public PartialsViewsController()
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
        #endregion

        #region Vistas
        public ActionResult ServicesPV()
        {
            return View();
        }

        public ActionResult EntrenamientoSmall()
        {
            return View();
        }

        public ActionResult TablaDatosWorkout(Workouts workout, Tracks[] tracks)
        {
            WorkoutTracksTablaDatosVM datosVM = new WorkoutTracksTablaDatosVM();
            datosVM.workoutVM = workout;
            datosVM.tracksVM = tracks;

            return View(datosVM);
        }

        public ActionResult WorkoutCarruselPV()
        {
            WorkoutTracksTablaDatosVM datosVM = new WorkoutTracksTablaDatosVM();
            if (TempData["workOut"] != null)
            {
                datosVM = (WorkoutTracksTablaDatosVM)TempData["workOut"];
            }
            return View(datosVM);
        }

        public ActionResult workoutsFactory()
        {
            var tracks = _dbContext.Tracks.Find<Tracks>(new BsonDocument()).ToList();
            return View(tracks);
        }

        public ActionResult UserInfo()
        {
            var usuCollection = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).SingleOrDefault();
            return View(usuCollection);
        }

        public ActionResult UserChangePass()
        {
            return View(usuario);
        }

        public ActionResult UserWorkouts()
        {
            return View();
        }
        public ActionResult UserEvolution()
        {
            return View();
        }
        public ActionResult SummaryLevels()
        {
            var levels = _dbContext.Levels.Find<Levels>(new BsonDocument()).ToList();

            return View(levels);
        }
        #endregion

        #region Métodos trasiego de datos
        /// <summary>
        /// Método invocado desde youCtrl
        /// Recupera la información necesaria para pintar el gráfico evolutivo del usuario
        /// </summary>
        /// <returns>Una lista con la evolución del usuario</returns>
        [HttpGet]
        public string evolucionUsu()
        {
            List<Evolution> datos = new List<Evolution>();
            var usuCollection = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).SingleOrDefault();

            string yearActual = DateTime.Today.Year.ToString();

            foreach (var dicAnyos in usuCollection.EvolutionUser)
            {
                if (dicAnyos.Key.Equals(yearActual))
                {
                    foreach (var mes in dicAnyos.Value)
                    {
                        Evolution ev = new Evolution();
                        ev.Mes = mes.Key;
                        ev.Puntos = mes.Value;
                        datos.Add(ev);
                    }
                }
            }

            return JsonConvert.SerializeObject(datos);
        }
        #endregion
    }
}