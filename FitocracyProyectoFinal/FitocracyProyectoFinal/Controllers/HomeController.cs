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
        private MongoDBcontext _dbContext;
        public HomeController()
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


        //View que trabaja a modo de Layout de la sección
        public ActionResult Index()
        {
            return View(usuario);
        }

        #region Partials Views => Carga con Angular
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


        #region Metodos POST 
        //Metodos
        [HttpPost]
        public bool Logeo(Usuario usuario)
        {
            var existeUsu = _dbContext.Usuarios.Find(x => x.Username == usuario.Username && x.Password == usuario.Password).Any() ? true : false;
            return existeUsu;
        }

        public string loginRecupUsuario(Usuario usuario)
        {
            EncriptacionClass encriptar = new EncriptacionClass();
            string passEncriptada = encriptar.Encrit(usuario.Password);

            //Usuario usu = new Usuario();
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


        [HttpPost]
        public string Registro(Usuario usuario)
        {
            EncriptacionClass encriptar = new EncriptacionClass();
            string passEncriptada = encriptar.Encrit(usuario.Password);

            usuario.Password = passEncriptada;
            usuario.Foto = ImgToDb(new FileInfo(Server.MapPath("~//Content//Imagenes//Profiles//nophoto.png")));
            usuario.WorkoutsUser = new Dictionary<string, Workouts>();
            usuario.CustomWorkouts = new Dictionary<string, Workouts>();

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
                //var collection = _dbContext.GetDatabase().GetCollection<Usuario>("usuarios");
                //var existe = collection.AsQueryable().Where(x => x.Email == usuario.Email).Any();

                var existe = _dbContext.Usuarios.Find<Usuario>(x => x.Email == usuario.Email).Any();

                if (!existe)
                {
                    _dbContext.Usuarios.InsertOne(usuario);
                    //collection.Insert(usuario);
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

        [HttpPost]
        public bool ForgotPassword(string Email)
        {
            //var collection = _dbContext.GetDatabase().GetCollection<Usuario>("usuarios");
            //var usu = collection.AsQueryable().Where(x => x.Email == Email).Select(x => x).FirstOrDefault();

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
        #endregion

        #region Metodos auxiliares
        //Metodos Auxiliares
        private byte[] ImgToDb(FileInfo info)
        {
            byte[] content = new byte[info.Length];
            FileStream imagestream = info.OpenRead();
            imagestream.Read(content, 0, content.Length);
            imagestream.Close();
            return content;
        }


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


        public bool UpdatePassword(string passNew, Usuario usuario)
        {
            EncriptacionClass encriptar = new EncriptacionClass();
            string passEncriptadaNew = encriptar.Encrit(passNew);

            try
            {
                var usuCollection = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).SingleOrDefault();


                //var collection = _dbContext.GetDatabase().GetCollection<Usuario>("usuarios");
                //var usuCollection = collection.AsQueryable().Where(x => x._id == usuario._id).FirstOrDefault();
                usuCollection.Password = passEncriptadaNew;
                _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.Password, passEncriptadaNew));

                //collection.Save(usuCollection);
                return true;

            }
            catch (Exception e)
            {
                string exc = e.ToString();
                return false;
            }
        }
        #endregion
    }
}