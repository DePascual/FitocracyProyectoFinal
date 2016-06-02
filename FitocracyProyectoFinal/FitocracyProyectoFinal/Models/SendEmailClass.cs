using System;
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
    }
}