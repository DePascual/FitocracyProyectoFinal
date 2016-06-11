using FitocracyProyectoFinal.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FitocracyProyectoFinal.Controllers
{
    public class ZonaUsuariosController : Controller
    {
        #region Variables
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
        #endregion

        #region Vistas
        public ActionResult Home()
        {
            return PartialView(usuario);
        }
        public ActionResult You()
        {
            var usu = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).SingleOrDefault();
            int puntos = pointsToNextLevel(usu);
            ViewData["pointsToNextLevel"] = puntos;
            ViewData["nextLevel"] = usu.Level + 1;
            return View(usu);
        }
       
        public ActionResult Track()
        {
            return View(usuario);
        }
        public ActionResult Connect()
        {
            return View(usuario);
        }
        public ActionResult Leaders()
        {
           var listLeaders = _dbContext.Usuarios.Find<Usuario>(new BsonDocument()).Sort(Builders<Usuario>.Sort.Descending(x=>x.Points)).ToList();
            return View(listLeaders);
        }

        public ActionResult WorkoutDoneAlert()
        {
            var usu = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).SingleOrDefault();
            return View(usu);
        }
        #endregion

        #region Métodos de trasiego de datos

        /// <summary>
        /// Método invocado desde userCtrl.js
        /// Borra de la sesión al ususario
        /// </summary>
        /// <param name="idUsuario"></param>
        public void SignOut(string idUsuario)
        {
            Session["infoUsuario"] = null;
        }


        /// <summary>
        /// Método invocado desde WorkoutCarruselPV.cshtml
        /// Guarda en la Base de datos el entrenamiento que el usuario acaba de realizar
        /// Suma los puntos obtenidos y comprueba que el nivel es el correcto
        /// </summary>
        /// <param name="_idWorkout"></param>
        /// <returns>Redirige a WorkoutAlert.cshtml</returns>
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
                _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.WorkoutsUser, usuCollection.WorkoutsUser));


                usuCollection.Points += workCollection.Puntos;
                _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.Points, usuCollection.Points));

                int levelActual = compruebaNivelActual(usuCollection.Points);
                if (levelActual != usuCollection.Level)
                {
                    usuCollection.Level = levelActual;
                    _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.Level, usuCollection.Level));
                }


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

                _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.EvolutionUser[yearActual], dicMeses));
               
            }
            catch (Exception e)
            {
                string ex = e.ToString();
            }

            return Redirect("http://" + host + ":" + port + "/#/WorkoutDoneAlert");
        }

        
        /// <summary>
        /// Método invocado desde trackService.js
        /// Recupera todos los Workouts
        /// </summary>
        /// <returns>Lista de Workouts</returns>
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

        /// <summary>
        /// Método invocado desde trackService.js
        /// Recupera todos los Workouts del usuario
        /// </summary>
        /// <returns>Lista de Workouts</returns>
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

        /// <summary>
        /// Método invocado desde trackService.js
        /// Recupera todos los tracks 
        /// </summary>
        /// <returns>Lista de Tracks</returns>
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

        /// <summary>
        /// Método invocado desde trackService.js
        /// Recupera todos los Workouts diseñados por el usuario
        /// </summary>
        /// <returns>Lista de Workouts</returns>
        [HttpPost]
        public string recuperaCustomWorkouts()
        {
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

        /// <summary>
        /// Método invocado desde trackService.js
        /// Busca el track 
        /// </summary>
        /// <param name="textoBusqueda"></param>
        /// <returns>Listado de tracks coincidentes con la búsqueda</returns>
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

        /// <summary>
        /// Transforma la foto del usuario en un array de bytes y la guarda en la base de datos
        /// </summary>
        /// <param name="file"></param>
        /// <param name="idUsu"></param>
        /// <returns>Redirige a You.cshtml</returns>
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

        /// <summary>
        /// Método invocado desde youService.js
        /// GUarda los cambios que el usuario ha decidido cambiar
        /// </summary>
        /// <param name="user"></param>
        /// <returns>El nuevo usuario</returns>
        [HttpPost]
        public string UpdateUser(Usuario user)
        {
            try
            {
                if (user.Username != null)
                {
                    _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.Username, user.Username));
                }

                if (user.Birthday != null)
                {
                    int edad = calculaEdad(user.Birthday);
                    _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.Birthday, user.Birthday));
                    _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.Age, edad));
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


        /// <summary>
        /// Método invocado desde youService.js
        /// Cambia la contraseña del usuario
        /// </summary>
        /// <param name="passOld"></param>
        /// <param name="passNew"></param>
        /// <returns>True (si la contraseña ha sido cambiada con éxito), o False (en caso contrario)</returns>
        [HttpPost]
        public bool UpdatePassword(string passOld, string passNew)
        {
            EncriptacionClass encriptar = new EncriptacionClass();
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

        /// <summary>
        /// Método invocado desde trackService.js
        /// Guarda el entrenamiento diseñado por el usuario
        /// </summary>
        /// <param name="workout"></param>
        /// <returns>True (si el entrenamiento ha sido cambiado con éxito), o False (en caso contrario)</returns>
        [HttpPost]
        public bool GuardaMyWork(Workouts workout)
        {

            string id = DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "");

            for (int i = id.Length; i < 24; i++)
            {
                id += "0";
            }

            workout._id = id;

            try
            {
                var usuCollection = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).FirstOrDefault();
                usuCollection.CustomWorkouts.Add(DateTime.Now.ToString(), workout);
                _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.CustomWorkouts, usuCollection.CustomWorkouts));
                var usuCollection2 = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).FirstOrDefault();
                return true;

            }
            catch (Exception e)
            {
                string exc = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// Método invocado desde trackService.js
        /// Borra un entrenamiento diseñado por el usuario
        /// </summary>
        /// <param name="idMyWork"></param>
        /// <returns>True (si el entrenamiento ha sido eliminado con éxito), o False (en caso contrario)</returns>
        [HttpPost]
        public bool BorraMyWork(string idMyWork)
        {
            try
            {
                var usuCollection = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).FirstOrDefault();
                usuCollection.CustomWorkouts.Remove(idMyWork);
                _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.CustomWorkouts, usuCollection.CustomWorkouts));

                return true;
            }
            catch (Exception e)
            {
                string exc = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// Método invocado desde youService.js
        /// Manda un mensaje de contacto a Fitocracy
        /// </summary>
        /// <param name="areaMessage"></param>
        /// <returns>True (si el mensaje ha sido enviado con éxito), o False (en caso contrario)</returns>
        [HttpPost]
        public bool Message(string areaMessage)
        {
            try
            {
                SendEmailClass.EmailConnect(usuario, areaMessage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }                  
        }
        #endregion

        #region Métodos auxiliares
        /// <summary>
        /// Calcula la edad del usuario mediante la fecha de cumpleaños
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns>La edad del usuario</returns>
        private int calculaEdad(string birthday)
        {
            var fecha = birthday.Split(new char[] { ' ' })[0].Replace("{", "").Replace("}", "");
            DateTime nac = DateTime.ParseExact(birthday, "dd/MM/yyyy", new CultureInfo("es-ES"));
            int edad = DateTime.Today.AddTicks(-nac.Ticks).Year - 1;
            return edad;
        }

        /// <summary>
        /// Comprueba que los puntos del usuario son de un determinado nivel
        /// </summary>
        /// <param name="points"></param>
        /// <returns>El nivel actual</returns>
        public int compruebaNivelActual(int points)
        {
            int levelAct = 0;
            var levelList = _dbContext.Levels.Find<Levels>(new BsonDocument()).ToList();

            for (int i = 0; i < levelList.Count(); i++)
            {
                if (points > levelList[i].Points && points < levelList[i + 1].Points)
                {
                    levelAct = levelList[i].Level;
                }

                if (points == levelList[i].Points)
                {
                    levelAct = levelList[i].Level;
                }
            }

            return levelAct;
        }
        
        /// <summary>
        /// Calcula los puntos necesarios para que el usuario adquiera el nuevo nivel
        /// </summary>
        /// <param name="usu"></param>
        /// <returns>Cantidad de puntos</returns>
        public int pointsToNextLevel(Usuario usu)
        {
            int puntos = 0;

            var levelList = _dbContext.Levels.Find<Levels>(new BsonDocument()).ToList();
            for (int i = 0; i < levelList.Count(); i++)
            {
                if (levelList[i].Level == usu.Level)
                {
                    puntos = levelList[i + 1].Points - usu.Points;
                }
            }
            return puntos;
        }
        #endregion

    }
}