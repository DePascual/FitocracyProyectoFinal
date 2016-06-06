using FitocracyProyectoFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitocracyProyectoFinal.Controllers
{
    public class ZonaEntrenadoresController : Controller
    {
        private MongoDBcontext _dbContext;
        private Entrenadores _entrenador;
        public ZonaEntrenadoresController()
        {
            _dbContext = new MongoDBcontext();
        }

        public Entrenadores entrenador
        {
            get
            {
                return _entrenador = (Entrenadores)Session["infoEntrenador"];
            }
            set
            {
                this._entrenador = value;
            }
        }

        public ActionResult Home()
        {
            return PartialView(entrenador);
        }
    }
}