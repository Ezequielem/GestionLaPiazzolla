using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GestionLaPiazzolla.Models;
using System.Net.Mail;
using System.Net;

namespace GestionLaPiazzolla.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Us()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public JsonResult EnviarEmail(string email, string mensaje)
        {
            try
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("contacto@sistemasgem.com");
                correo.To.Add("humberto_menseguet@hotmail.com");
                correo.Subject = email;
                correo.Body = mensaje;
                correo.IsBodyHtml = true;
                correo.Priority = MailPriority.Normal;

                SmtpClient cliente = new SmtpClient();
                cliente.Host = "cmx5.my-hosting-panel.com";
                cliente.Port = 25;
                cliente.EnableSsl = false;
                cliente.UseDefaultCredentials = true;
                cliente.Credentials = new NetworkCredential("contacto@sistemasgem.com", "Humber123LaPiazzolla!");
                cliente.Send(correo);
                return Json("'Success':'true'");
            }
            catch (Exception)
            {
                return Json(String.Format("'Success':'false','Error':'No se ha podido enviar el mensaje'"));

            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
