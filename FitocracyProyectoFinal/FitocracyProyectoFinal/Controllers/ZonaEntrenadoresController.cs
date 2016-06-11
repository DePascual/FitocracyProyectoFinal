using FitocracyProyectoFinal.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitocracyProyectoFinal.ViewModels;

namespace FitocracyProyectoFinal.Controllers
{
    public class ZonaEntrenadoresController : Controller
    {
        #region Variables
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
        #endregion

        #region Vistas
        public ActionResult Home()
        {
            var entrenamientos = _dbContext.Entrenamientos.Find<Entrenamientos>(x => x.Id_Entrenador == entrenador._id).ToList();

            EntrenamientoEntrenadoresVM vM = new EntrenamientoEntrenadoresVM();
            vM.entrenador = entrenador;
            vM.entrenamientosList = entrenamientos;

            return PartialView(vM);
        }

        public ActionResult Entrenamientos()
        {
            var entrenamientos = _dbContext.Entrenamientos.Find<Entrenamientos>(x => x.Id_Entrenador == entrenador._id).ToList();
            return View(entrenamientos);
        }

        public ActionResult EditEntrenamiento()
        {
            return View();
        }
        #endregion


        #region Trasiego de datos
        /// <summary>
        /// Método invocado desde Home.cshtml (Zona entrenadores)
        /// Recupera los datos del entrenamiento seleccionado
        /// </summary>
        /// <param name="idEntrenamiento"></param>
        /// <returns>Redirecciona a EditEntrenamiento</returns>
        public ActionResult BuscaEntrenamiento(string idEntrenamiento)
        {
            var host = Request.Url.Host;
            var port = Request.Url.Port;

            var entrenamiento = _dbContext.Entrenamientos.Find<Entrenamientos>(x => x._id == idEntrenamiento).SingleOrDefault();
            TempData["entrenamiento"] = entrenamiento;
            return Redirect("http://" + host + ":" + port + "/#/EditEntrenamiento");
        }

        /// <summary>
        /// Método invocado desde entrenadoresCtrl.js
        /// Borra de la sesión al entrenador
        /// </summary>
        /// <param name="idEntrenador"></param>
        public void SignOut(string idEntrenador)
        {
            Session["infoEntrenador"] = null;
        }


        /// <summary>
        /// Método invocado desde entrenadorService.js
        /// Cambia el entrenamiento seleccionado
        /// </summary>
        /// <param name="entrenamiento"></param>
        /// <returns>True (si el entrenamiento ha sido cambiada con éxito), o False (en caso contrario)</returns>
        [HttpPost]
        public bool UpdateEntrenamiento(Entrenamientos entrenamiento)
        {
            try
            {
                if (entrenamiento.NombreEntrenamiento != null)
                {
                    _dbContext.Entrenamientos.UpdateOne<Entrenamientos>(x => x._id == entrenamiento._id, Builders<Entrenamientos>.Update.Set(x => x.NombreEntrenamiento, entrenamiento.NombreEntrenamiento));
                }
                if (entrenamiento.Duracion != null)
                {
                    _dbContext.Entrenamientos.UpdateOne<Entrenamientos>(x => x._id == entrenamiento._id, Builders<Entrenamientos>.Update.Set(x => x.Duracion, entrenamiento.Duracion));
                }
                if (entrenamiento.Precio != null)
                {
                    _dbContext.Entrenamientos.UpdateOne<Entrenamientos>(x => x._id == entrenamiento._id, Builders<Entrenamientos>.Update.Set(x => x.Precio, entrenamiento.Precio));
                }
                if (entrenamiento.Familia != null)
                {
                    _dbContext.Entrenamientos.UpdateOne<Entrenamientos>(x => x._id == entrenamiento._id, Builders<Entrenamientos>.Update.Set(x => x.Familia, entrenamiento.Familia));
                }
                if (entrenamiento.Descripcion != null)
                {
                    _dbContext.Entrenamientos.UpdateOne<Entrenamientos>(x => x._id == entrenamiento._id, Builders<Entrenamientos>.Update.Set(x => x.Descripcion, entrenamiento.Descripcion));
                }

                string fecha = DateTime.Today.ToString("dd/MM/yyyy");
                _dbContext.Entrenamientos.UpdateOne<Entrenamientos>(x => x._id == entrenamiento._id, Builders<Entrenamientos>.Update.Set(x => x.LastModification, fecha));

                return true;
            }
            catch (Exception e)
            {
                string ex = e.ToString();
                return false;
            }


        }
        #endregion


        #region Métodos auxiliares
        /// <summary>
        /// Transforma la foto del entrenador en un array de bytes y la guarda en la base de datos
        /// </summary>
        /// <param name="file"></param>
        /// <param name="idUsu"></param>
        /// <returns>Redirecciona a la zona de entrenadores</returns>
        public ActionResult uploadPhoto(HttpPostedFileBase file, string idUsu)
        {
            var host = Request.Url.Host;
            var port = Request.Url.Port;

            var url = Url.RequestContext.RouteData.Values["id"];
            if (file != null)
            {
                string pic = Path.GetFileName(file.FileName);
                string path = Path.Combine(Server.MapPath("~/Content/Imagenes/Profiles"), pic);
                file.SaveAs(path);

                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();

                    try
                    {
                        _dbContext.Entrenadores.UpdateOne<Entrenadores>(x => x._id == entrenador._id, Builders<Entrenadores>.Update.Set(x => x.Foto, array));
                    }
                    catch (Exception e)
                    {
                        string exc = e.ToString();
                    }
                }

                System.IO.File.Delete(path);
            }
            return Redirect("http://" + host + ":" + port + "/#/ZonaEntrenadores");
        }
        #endregion
    }
}