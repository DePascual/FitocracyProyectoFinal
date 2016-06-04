using FitocracyProyectoFinal.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitocracyProyectoFinal.Controllers
{
    public class ZonaUsuariosController : Controller
    {
        private MongoDBcontext _dbContext;
        private Usuario _usuario;
        public ZonaUsuariosController()
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
            return PartialView(usuario);
        }
        public ActionResult You()
        {
            //Usuario usuario = (Usuario)Session["infoUsuario"];
            //var collection = _dbContext.GetDatabase().GetCollection<Usuario>("usuarios");
            //var usuCollection = collection.AsQueryable().Where(x => x._id == usuario._id).FirstOrDefault();

            var usu = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).SingleOrDefault();
            return View(usu);
        }
        public ActionResult Track()
        {
            return View(usuario);
        }
        public ActionResult Connect()
        {
            return View();
        }
        public ActionResult Leaders()
        {
            return View(usuario);
        }

        public ActionResult WorkoutDoneAlert()
        {
            return View(usuario);
        }


        public void SignOut(string idUsuario)
        {
            Session["infoUsuario"] = null;
        }



        [HttpPost]
        public ActionResult workoutDone(string _idWorkout)
        {
            var host = Request.Url.Host;
            var port = Request.Url.Port;

            try
            {
                var workCollection = _dbContext.Workouts.Find<Workouts>(x => x._id == _idWorkout).FirstOrDefault();
                var usuCollection = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).SingleOrDefault();

                usuCollection.WorkoutsUser.Add(DateTime.Now.ToString(), workCollection);

                string yearActual = DateTime.Today.Year.ToString();
                string mesActual = DateTime.Today.Month.ToString();

                //Si no existe el año dentro del dictionary es porque es AÑO NUEVO
                if (!usuCollection.EvolutionUser.ContainsKey(yearActual))
                {
                    Dictionary<string, Dictionary<string, int>> nuevoDicAnyo = new Dictionary<string, Dictionary<string, int>>();
                    Dictionary<string, int> nuevoDicMeses = new Dictionary<string, int>();

                    for (int i = 0; i < 12; i++)
                    {
                        nuevoDicMeses.Add((i + 1).ToString(), 0);
                    }

                    nuevoDicAnyo.Add(yearActual, nuevoDicMeses);
                }

                var dicMeses = usuCollection.EvolutionUser.Where(x => x.Key == yearActual).SingleOrDefault().Value;
               
                int puntosMes = 0;

                foreach (var mes in dicMeses)
                {
                    if (mes.Key.Equals(mesActual))
                    {
                        puntosMes = mes.Value;
                        puntosMes += workCollection.Puntos;
                    }
                }

                dicMeses[mesActual] = puntosMes;

                _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x=>x.EvolutionUser[yearActual], dicMeses));

            }
            catch (Exception e)
            {
                string ex = e.ToString();
            }

            return Redirect("http://" + host + ":" + port + "/#/WorkoutDoneAlert");
        }


        [HttpPost]
        public string recuperaWorkouts()
        {
            try
            {
                var workouts = _dbContext.Workouts.Find<Workouts>(new BsonDocument()).ToList();
                return JsonConvert.SerializeObject(workouts);
            }
            catch (Exception)
            {

                return null;
            }
        }

        [HttpPost]
        public string recuperaWorkoutsUsu()
        {
            try
            {
                var usuCollection = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).FirstOrDefault();
                return JsonConvert.SerializeObject(usuCollection.WorkoutsUser);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public string recuperaAllTracks()
        {
            try
            {
                var tracks = _dbContext.Tracks.Find<Tracks>(new BsonDocument()).ToList();
                return JsonConvert.SerializeObject(tracks);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public string recuperaCustomWorkouts()
        {
            Usuario usuario = (Usuario)Session["infoUsuario"];
            try
            {
                var usuCollection = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).FirstOrDefault();
                return JsonConvert.SerializeObject(usuCollection.CustomWorkouts);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPost]
        public string buscadorTracks(string textoBusqueda)
        {
            try
            {
                var tracksEncontrados = _dbContext.Tracks.Find<Tracks>(x => x.Nombre.Contains(textoBusqueda)).ToList();
                return JsonConvert.SerializeObject(tracksEncontrados);
            }
            catch (Exception e)
            {
                string ex = e.ToString();
                return null;
            }
        }


        [HttpPost]
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
                        //var collection = _dbContext.GetDatabase().GetCollection<Usuario>("usuarios");
                        //var usu = collection.AsQueryable().Where(x => x._id == idUsu).FirstOrDefault();
                        //var fotoInicial = usu.Foto;
                        //usu.Foto = array;
                        //collection.Save(usu);

                        _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.Foto, array));
                    }
                    catch (Exception e)
                    {
                        string exc = e.ToString();
                    }

                }

                System.IO.File.Delete(path);
            }
            return Redirect("http://" + host + ":" + port + "/#/You");
        }

        [HttpPost]
        public string UpdateUser(Usuario user)
        {
            try
            {
                if(user.Username != null)
                {
                    _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.Username, user.Username));
                }

                if (user.Birthday != null)
                {
                    _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.Birthday, user.Birthday));
                }

                if (user.Description != null)
                {
                    _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.Description, user.Description));
                }

                var usuChanged = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).FirstOrDefault();
                return JsonConvert.SerializeObject(usuChanged);

            }
            catch (Exception e)
            {
                string exc = e.ToString();
                return null;
            }
        }


        [HttpPost]
        public bool UpdatePassword(string passOld, string passNew)
        {
            EncriptacionClass encriptar = new EncriptacionClass();
            //string passEncriptadaOld = encriptar.Encrit(passOld);
            string passEncriptadaNew = encriptar.Encrit(passNew);

            try
            {
                _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.Password, passEncriptadaNew));
                return true;

            }
            catch (Exception e)
            {
                string exc = e.ToString();
                return false;
            }
        }

        [HttpPost]
        public bool GuardaMyWork(Workouts workout)
        {

            string id = DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "");
            id += "0000000000";
            workout._id = id;

            try
            {

                var usuCollection = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).FirstOrDefault();
                usuCollection.CustomWorkouts.Add(DateTime.Now.ToString(), workout);
                _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.CustomWorkouts, usuCollection.CustomWorkouts));

                //collection.Save(usuCollection);
                //Dictionary<string, Workouts> dic = new Dictionary<string, Workouts>
                //{
                //    { DateTime.Now.ToString(), workout }
                //};
                //_dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Push<Dictionary<string, Workouts>>(e => e.CustomWorkouts, dic));
                //var update = Builders<Usuario>.Update.Push(e => e.CustomWorkouts, dic);
                var usuCollection2 = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).FirstOrDefault();
                return true;

            }
            catch (Exception e)
            {
                string exc = e.ToString();
                return false;
            }
        }

    }
}