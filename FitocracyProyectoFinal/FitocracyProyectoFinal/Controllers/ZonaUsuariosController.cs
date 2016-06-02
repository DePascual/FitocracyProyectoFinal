using FitocracyProyectoFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitocracyProyectoFinal.Controllers
{
    public class ZonaUsuariosController : Controller
    {
        private MongoDBcontext _dbContext;
        public ZonaUsuariosController()
        {
            _dbContext = new MongoDBcontext();
        }

        private Usuario _usuario;
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
            return View(usuario);
        }
    }
}