using FitocracyProyectoFinal.Models;
using FitocracyProyectoFinal.ViewModels;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FitocracyProyectoFinal.Controllers
{
    public class CoachController : Controller
    {
        #region Variables
        /// <summary>
        /// Declaración de variables
        /// </summary>
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
        #endregion

        #region Vistas
        /// <summary>
        /// Carga de View Model con los datos necesarios para la vista
        /// </summary>
        /// <returns>Vista Home.cshtml de la zona Coach</returns>
        public ActionResult Home()
        {
            List<Entrenamientos> entrenamientosList = _dbContext.Entrenamientos.Find<Entrenamientos>(new BsonDocument()).ToList();
            List<Entrenadores> entrenadoresList = _dbContext.Entrenadores.Find<Entrenadores>(new BsonDocument()).ToList();

            EntrenamientoEntrenadoresVM vM = new EntrenamientoEntrenadoresVM(entrenadoresList, entrenamientosList);

            return View(vM);
        }

        /// <summary>
        /// Método invocado desde EntrenamientoSmall.cshtml
        /// Muestra una descripción más detallada del entrenamiento seleccionado
        /// </summary>
        /// <param name="idEntrenador"></param>
        /// <param name="idEntrenamiento"></param>
        /// <returns>Redirección a detalleEntrenamiento.cshtml</returns>
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

        /// <summary>
        /// Método que utiliza AngularJs para enrutar y pintar detalleEntrenamiento.cshtml
        /// </summary>
        /// <returns></returns>
        public ActionResult detalleEntrenamiento()
        {
            return View();
        }
        #endregion

        #region Métodos trasiego de datos
        /// <summary>
        /// Método invocado desde trackService.js, petición Ajax
        /// Recupera el entrenamiento indicado
        /// </summary>
        /// <param name="idEntrenamiento"></param>
        /// <returns>Devuelve un entrenamiento</returns>
        [HttpPost]
        public string recuperaEntrenamiento(string idEntrenamiento)
        {
            var entrenamiento = _dbContext.Entrenamientos.Find<Entrenamientos>(x => x._id == idEntrenamiento).SingleOrDefault();
            TempData["entrenamientoCompra"] = entrenamiento;
            return JsonConvert.SerializeObject(entrenamiento);
        }


        /// <summary>
        /// Método invocado desde trackService.js, petición Ajax
        /// Realiza el volcado de datos cuando se realiza una compra
        /// </summary>
        /// <param name="datosTarjeta"></param>
        /// <returns>True (si el volcado ha sido satisfactorio), o False (en caso contrario)</returns>
        [HttpPost]
        public bool compraEntrenamiento(TarjetasUsuario datosTarjeta)
        {
            try
            {
                EncriptacionClass encriptacion = new EncriptacionClass();
                var numeroEncriptado = encriptacion.Encrit(datosTarjeta.CardNumber);
                var seguridadEncriptada = encriptacion.Encrit(datosTarjeta.SecurityCode);

                var existeTarjeta = _dbContext.Tarjetas.Find<TarjetasUsuario>(x => x.CardNumber == numeroEncriptado
                                                                        && x.SecurityCode == seguridadEncriptada
                                                                        && x.Year == datosTarjeta.Year
                                                                        && x.Month == datosTarjeta.Month)
                                                                        .Any() ? true : false;

                if (!existeTarjeta)
                {
                    datosTarjeta.CardNumber = numeroEncriptado;
                    datosTarjeta.SecurityCode = seguridadEncriptada;
                    _dbContext.Tarjetas.InsertOne(datosTarjeta);
                }

                if (Convert.ToInt32(datosTarjeta.Month) < DateTime.Now.Month || Convert.ToInt32(datosTarjeta.Year) < DateTime.Now.Year)
                {
                    return false;
                }
                else
                {
                    var usuCollection = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).SingleOrDefault();
                    var entrenamiento = (Entrenamientos)TempData["entrenamientoCompra"];
                    usuCollection.EntrenamientosCompradosUser.Add(DateTime.Now.ToString(), entrenamiento);
                    _dbContext.Usuarios.UpdateOne<Usuario>(x => x._id == usuario._id, Builders<Usuario>.Update.Set(x => x.EntrenamientosCompradosUser, usuCollection.EntrenamientosCompradosUser));
                }

                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Método invocado desde trackCtrl.js
        /// Envía un email con el entrenamiento comprado
        /// </summary>
        [HttpGet]
        public void mandarEntrenamiento()
        {
            var usu = _dbContext.Usuarios.Find<Usuario>(x => x._id == usuario._id).SingleOrDefault();
            var entrenamiento = usu.EntrenamientosCompradosUser.Values.Last();

            try
            {
                SendEmailClass.EmailEntrenamientoComprado(entrenamiento, usuario);
            }
            catch (Exception e)
            {
                string ex = e.ToString();
            }
        }
        #endregion
    }
}