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

        Entrenamientos entrenamientoRecuperado = new Entrenamientos();

        public ActionResult Home()
        {
            var entrenamientos = _dbContext.Entrenamientos.Find<Entrenamientos>(x => x.Id_Entrenador == entrenador._id).ToList();

            EntrenamientoEntrenadoresVM vM = new EntrenamientoEntrenadoresVM();
            vM.entrenador = entrenador;
            vM.entrenamientosList = entrenamientos;
          
            return PartialView(vM);
        }

        public ActionResult EditEntrenamiento()
        {
            return View();
        }



        public ActionResult BuscaEntrenamiento(string idEntrenamiento)
        {
            var host = Request.Url.Host;
            var port = Request.Url.Port;

            var entrenamiento = _dbContext.Entrenamientos.Find<Entrenamientos>(x => x._id == idEntrenamiento).SingleOrDefault();
            TempData["entrenamiento"] = entrenamiento;
            //return PartialView();

            //entrenamientoRecuperado = entrenamiento;

            return Redirect("http://" + host + ":" + port + "/#/EditEntrenamiento");
        }



        public void SignOut(string idEntrenador)
        {
            Session["infoEntrenador"] = null;
        }





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
    }
}