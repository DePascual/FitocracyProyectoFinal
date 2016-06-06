﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace FitocracyProyectoFinal.Models
{
    public class SendEmailClass
    {
        public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static void EmailChangePass(Usuario usu, string newPass)
        {
            MailMessage nuevoCorreo = new MailMessage();
            nuevoCorreo.To.Add(new MailAddress(usu.Email));
            nuevoCorreo.From = new MailAddress("mail.pruebas.daw@gmail.com");
            nuevoCorreo.Subject = "FITOCRACY: Your new password";
            nuevoCorreo.IsBodyHtml = true;

            string body = "Hey " + usu.Username + "!!<br /> ";
            body += "You have forgotten your password for access...Don't worry!!<br /><br /> ";
            body += "Your new password is <span style='color:#1da6da'>" + newPass + "</span><br />";
            body += "Log in to your account whit the new password. To change the password, go to your custom site, and change it!! (If you want do it) <br/>";
            body += "Thanks for your confidence!!<br/>";
            body += "<hr />";
            body += "FITOCRACY Team!!";
            nuevoCorreo.Body = body;

            SmtpClient servidor = new SmtpClient();
            servidor.Host = "smtp.gmail.com";
            servidor.Port = 587;
            servidor.EnableSsl = true;
            servidor.DeliveryMethod = SmtpDeliveryMethod.Network;
            servidor.Credentials = new NetworkCredential("mail.pruebas.daw@gmail.com", "avellanedadaw");
            servidor.Timeout = 2000;

            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            try
            {
                servidor.Send(nuevoCorreo);
            }
            catch (Exception ex)
            {
                string e = ex.ToString();
            }

            nuevoCorreo.Dispose();

        }


        public static void EmailConnect(Usuario usu, string message)
        {

            MailMessage nuevoCorreo = new MailMessage();
            nuevoCorreo.To.Add(new MailAddress("mail.pruebas.daw@gmail.com"));
            nuevoCorreo.From = new MailAddress("mail.pruebas.daw@gmail.com");
            nuevoCorreo.Subject = usu.Username + " needs our help!!! IMPORTANT!!";
            nuevoCorreo.IsBodyHtml = true;

            string body = "Email sendding for " + usu.Username + "!!<br /><hr/> ";
            body += "<span style='color:#1da6da'> " + message + "</span>";
            body += "<hr/>";
            body += "Email user: " + usu.Email + "<br/>";
            body += "User code: " + usu._id + "<br/>";
            body += "<hr />";
            body += "FITOCRACY Team!!";
            nuevoCorreo.Body = body;

            SmtpClient servidor = new SmtpClient();
            servidor.Host = "smtp.gmail.com";
            servidor.Port = 587;
            servidor.EnableSsl = true;
            servidor.DeliveryMethod = SmtpDeliveryMethod.Network;
            servidor.Credentials = new NetworkCredential("mail.pruebas.daw@gmail.com", "avellanedadaw");
            servidor.Timeout = 2000;

            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            try
            {
                servidor.Send(nuevoCorreo);
            }
            catch (Exception ex)
            {
                string e = ex.ToString();
            }

            nuevoCorreo.Dispose();

        }

        public static void EmailEntrenamientoComprado(Entrenamientos entrenamiento, Usuario usuario)
        {

            MailMessage nuevoCorreo = new MailMessage();
            nuevoCorreo.To.Add(new MailAddress(usuario.Email));
            nuevoCorreo.From = new MailAddress("mail.pruebas.daw@gmail.com");
            nuevoCorreo.Subject = "FITOCRACY: Your new training";
            nuevoCorreo.IsBodyHtml = true;

            string body = "Hey " + usuario.Username + "!!<br /> ";
            body += "Your new training <span style='color:#1da6da'>" + entrenamiento.NombreEntrenamiento + "</span> has been deal correctly !! Enjoy it!!!<br /> ";
            body += "Be happy and strong!!";
            body += "<hr />";
            body += "FITOCRACY Team!!";
            nuevoCorreo.Body = body;
            //nuevoCorreo.Attachments.Add(new Attachment(Server.MapPath("~/facturas/" + miCliente.email + "_" + today + ".pdf")));
            nuevoCorreo.Attachments.Add(new Attachment(System.Web.HttpContext.Current.Server.MapPath("~/Content/Pdfs/pdfEntrenamiento.pdf")));

            SmtpClient servidor = new SmtpClient();
            servidor.Host = "smtp.gmail.com";
            servidor.Port = 587;
            servidor.EnableSsl = true;
            servidor.DeliveryMethod = SmtpDeliveryMethod.Network;
            servidor.Credentials = new NetworkCredential("mail.pruebas.daw@gmail.com", "avellanedadaw");
            servidor.Timeout = 2000;

            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            try
            {
                servidor.Send(nuevoCorreo);
            }
            catch (Exception ex)
            {
                string e = ex.ToString();
            }

            nuevoCorreo.Dispose();

        }
    }
}