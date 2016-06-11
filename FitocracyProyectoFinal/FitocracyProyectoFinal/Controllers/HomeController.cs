using FitocracyProyectoFinal.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.IO;

namespace FitocracyProyectoFinal.Controllers
{
    public class HomeController : Controller
    {
        #region Variables
        private MongoDBcontext _dbContext;
        private Usuario _usuario;

        public HomeController()
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
        /// <summary>
        /// View que trabaja a modo de Layout de la aplicación
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(usuario);
        }

        //PartialsViews
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Registro()
        {
            return View();
        }

        public ActionResult ChangePass()
        {
            return View();
        }
        #endregion


        #region Metodos trasiego de datos       
        /// <summary>
        /// Método invocado desde loginService.js
        /// Conecta con la Base de datos y comprueba el usuario que quiere logearse
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns>El usuario recuperado</returns>
        public string loginRecupUsuario(Usuario usuario)
        {
            EncriptacionClass encriptar = new EncriptacionClass();
            string passEncriptada = encriptar.Encrit(usuario.Password);
            try
            {
                var usu = _dbContext.Usuarios.Find<Usuario>(x => x.Username == usuario.Username && x.Password == passEncriptada).SingleOrDefault();
                Session["infoUsuario"] = usu;
                return JsonConvert.SerializeObject(usu);
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Método invocado desde loginService.js
        /// Conecta con la Base de datos y comprueba el entrenador que quiere logearse
        /// </summary>
        /// <param name="entrenador"></param>
        /// <returns>El entrenador logueado</returns>
        public string loginRecupEntrenador(Entrenadores entrenador)
        {
            EncriptacionClass encriptar = new EncriptacionClass();
            string passEncriptada = encriptar.Encrit(entrenador.CoachPass);
            try
            {
                var entrenadorColl = _dbContext.Entrenadores.Find<Entrenadores>(x => x.CoachName == entrenador.CoachName && x.CoachPass == passEncriptada).SingleOrDefault();
                Session["infoEntrenador"] = entrenadorColl;
                return JsonConvert.SerializeObject(entrenadorColl);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Método invocado desde registroService.js
        /// Crea el objeto usuario necesario para introducirlo en la Base de Datos
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns>El usuario registrado</returns>
        [HttpPost]
        public string Registro(Usuario usuario)
        {
            EncriptacionClass encriptar = new EncriptacionClass();
            string passEncriptada = encriptar.Encrit(usuario.Password);

            usuario.Password = passEncriptada;
            usuario.Foto = ImgToDb(new FileInfo(Server.MapPath("~//Content//Imagenes//Profiles//nophoto.png")));
            usuario.WorkoutsUser = new Dictionary<string, Workouts>();
            usuario.CustomWorkouts = new Dictionary<string, Workouts>();
            usuario.EntrenamientosCompradosUser = new Dictionary<string, Entrenamientos>();

            int yearActual = DateTime.Today.Year;
            int mesActual = DateTime.Today.Month;

            Dictionary<string, Dictionary<string, int>> nuevoDicAnyo = new Dictionary<string, Dictionary<string, int>>();
            Dictionary<string, int> nuevoDicMeses = new Dictionary<string, int>();

            for (int i = 0; i < 12; i++)
            {
                nuevoDicMeses.Add((i + 1).ToString(), 0);
            }

            nuevoDicAnyo.Add(yearActual.ToString(), nuevoDicMeses);

            usuario.EvolutionUser = nuevoDicAnyo;

            try
            {
                var existe = _dbContext.Usuarios.Find<Usuario>(x => x.Email == usuario.Email).Any();

                if (!existe)
                {
                    _dbContext.Usuarios.InsertOne(usuario);
                    Session["infoUsuario"] = usuario;
                    return JsonConvert.SerializeObject(usuario);
                }
                else
                {
                    return null;
                }


            }
            catch (Exception e)
            {
                String ex = e.ToString();
                Console.Write("Error en la inserción del nuevo usuario");
                return null;
            }
        }

        /// <summary>
        /// Método invocado desde loginService.js
        /// Manda por email la nueva contraseña al usuario
        /// </summary>
        /// <param name="Email"></param>
        /// <returns>True (si el email ha sido enviado con éxito), o False (en caso contrario)</returns>
        [HttpPost]
        public bool ForgotPassword(string Email)
        {
            var usu = _dbContext.Usuarios.Find<Usuario>(x => x.Email == Email).SingleOrDefault();
            try
            {
                string newPass = generaNuevaPassword();
                bool ok = UpdatePassword(newPass, usu);
                SendEmailClass.EmailChangePass(usu, newPass);
                return true;
            }
            catch (Exception e)
            {
                string ex = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// Método invocado desde youService.js
        /// Cambia la contraseña del usuario
        /// </summary>
        /// <param name="passNew"></param>
        /// <param name="usuario"></param>
        /// <returns>True (si la contraseña ha sido cambiada con éxito), o False (en caso contrario)</returns>
        [HttpPost]
        public bool UpdatePassword(string passNew, Usuario usuario)
        {
            EncriptacionClass encriptar = new EncriptacionClass();
            string passEncriptadaNew = encriptar.Encrit(passNew);

            try
            {
                var usuCollection = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).SingleOrDefault();
                usuCollection.Password = passEncriptadaNew;
                _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.Password, passEncriptadaNew));
                return true;
            }
            catch (Exception e)
            {
                string exc = e.ToString();
                return false;
            }
        }
        #endregion

        #region Metodos auxiliares
        /// <summary>
        /// Transforma un fichero en un array de bytes
        /// </summary>
        /// <param name="info"></param>
        /// <returns>Un array de bytes</returns>
        private byte[] ImgToDb(FileInfo info)
        {
            byte[] content = new byte[info.Length];
            FileStream imagestream = info.OpenRead();
            imagestream.Read(content, 0, content.Length);
            imagestream.Close();
            return content;
        }

        /// <summary>
        /// Genera una nueva contraseña con caracteres aleatorios
        /// </summary>
        /// <returns>La nueva contraseña</returns>
        public string generaNuevaPassword()
        {
            string newPass = "";
            string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int n = caracteres.Length;

            Random r = new Random();

            for (int i = 0; i < 12; i++)
            {
                newPass += caracteres[r.Next(n)];
            }
            return newPass;
        }
        #endregion
    }
}