using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionLaPiazzolla.Data;
using GestionLaPiazzolla.Models;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Calendar.v3;
using Google.Apis.Auth.OAuth2;
using System.IO;
using Google.Apis.Services;
using Google.Apis.Calendar.v3.Data;
using GestionLaPiazzolla.Reports;
using Microsoft.AspNetCore.Hosting;
using Rotativa.AspNetCore;

namespace GestionLaPiazzolla.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PagosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Pagos
        public async Task<IActionResult> Index(string sortOrder, string cadenaBusqueda)
        {
            ViewData["AlumnoSortParametro"] = String.IsNullOrEmpty(sortOrder) ? "alumno_desc" : "";
            ViewData["MontoSortParametro"] = sortOrder == "Monto" ? "monto_desc" : "Monto";
            ViewData["FechaSortParametro"] = sortOrder == "FechaPago" ? "fecha_desc" : "FechaPago";
            ViewData["ObservacionSortParametro"] = sortOrder == "Observacion" ? "obser_desc" : "Observacion";

            ViewData["FiltroActual"] = cadenaBusqueda;
            var pagos = from p in _context.Pagos.Include(a => a.Alumno)
                        select p;
            if (!String.IsNullOrEmpty(cadenaBusqueda))
            {
                pagos = pagos.Where(p => p.Alumno.Nombre.Contains(cadenaBusqueda)
                                    || p.Alumno.Apellido.Contains(cadenaBusqueda));
            }
            switch (sortOrder)
            {
                case "alumno_desc":
                    pagos = pagos.OrderByDescending(p => p.Alumno.Apellido);
                    break;
                case "Monto":
                    pagos = pagos.OrderBy(p => p.Monto);
                    break;
                case "monto_desc":
                    pagos = pagos.OrderByDescending(p => p.Monto);
                    break;
                case "FechaPago":
                    pagos = pagos.OrderBy(p => p.FechaPago);
                    break;
                case "fecha_desc":
                    pagos = pagos.OrderByDescending(p => p.FechaPago);
                    break;
                case "Observacion":
                    pagos = pagos.OrderBy(p => p.Observacion);
                    break;
                case "obser_desc":
                    pagos = pagos.OrderByDescending(p => p.Observacion);
                    break;
                default:
                    pagos = pagos.OrderBy(p => p.Alumno.Apellido);
                    break;
            }
            return View(await pagos.AsNoTracking().ToListAsync());
        }

        // GET: Pagos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos
                .Include(p => p.Alumno)
                .FirstOrDefaultAsync(m => m.PagoId == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // GET: Pagos/Create
        public IActionResult Create()
        {
            //crearEvento();
            listaDeAlumnos();
            return View();
        }

        // POST: Pagos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PagoId,Monto,FechaPago,Observacion,AlumnoId")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pago);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            listaDeAlumnos(pago);
            return View(pago);
        }

        // GET: Pagos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null)
            {
                return NotFound();
            }
            listaDeAlumnos();
            return View(pago);
        }

        // POST: Pagos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PagoId,Monto,FechaPago,Observacion,AlumnoId")] Pago pago)
        {
            if (id != pago.PagoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PagoExists(pago.PagoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            listaDeAlumnos(pago);
            return View(pago);
        }

        // GET: Pagos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos
                .Include(p => p.Alumno)
                .FirstOrDefaultAsync(m => m.PagoId == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // POST: Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            _context.Pagos.Remove(pago);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ImpresionRecibo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pago = await _context.Pagos
                .Include(p => p.Alumno)                
                .FirstOrDefaultAsync(p => p.PagoId == id);

            if (pago == null)
            {
                return NotFound();
            }
            //return View(pago);
            return new ViewAsPdf("ImpresionRecibo", pago)
            {
                FileName = "Recibo" + pago.PagoId + ".pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }

        public async Task<IActionResult> Recibo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pago = await _context.Pagos
                .Include(p => p.Alumno)
                .FirstOrDefaultAsync(p => p.PagoId == id);

            if (pago == null)
            {
                return NotFound();  
            }
            ReporteRecibo recibo = new ReporteRecibo(_env);

            return File(recibo.imprimirRecibo(pago), "application/pdf");
        }

        private bool PagoExists(int id)
        {
            return _context.Pagos.Any(e => e.PagoId == id);
        }

        private void listaDeAlumnos(object alumnoSeleccionado = null)
        {
            var consultaAlumno = from a in _context.Alumnos
                                 orderby a.Apellido
                                 select new {
                                     AluId=a.AlumnoId,
                                     NombreApellido=a.Nombre + " " + a.Apellido
                                 };
            ViewBag.AlumnoId = new SelectList(consultaAlumno.AsNoTracking(), "AluId", "NombreApellido", alumnoSeleccionado);
        }

        private void crearEvento()
        {

            string acumulador = "";
            string camino = Path.Combine("wwwroot", "js", "lapiazzolla-413c1340968e.json");
            string calendarId = @"ezemense@gmail.com";

            string[] Scopes = { CalendarService.Scope.Calendar };

            ServiceAccountCredential credential;

            using (var stream =
                new FileStream(camino, FileMode.Open, FileAccess.Read))
            {
                var confg = Google.Apis.Json.NewtonsoftJsonSerializer.Instance.Deserialize<JsonCredentialParameters>(stream);
                credential = new ServiceAccountCredential(
                   new ServiceAccountCredential.Initializer(confg.ClientEmail)
                   {
                       Scopes = Scopes
                   }.FromPrivateKey(confg.PrivateKey));
            }

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Calendar API Sample",
            });

            var calendar = service.Calendars.Get(calendarId).Execute();
            //Console.WriteLine("Calendar Name :");
            //Console.WriteLine(calendar.Summary);
            acumulador += "Calendar Name : \n";
            acumulador += calendar.Summary + "\n";
            acumulador += "\n\nclick for more .. \n\n";


            // Define parameters of request.
            EventsResource.ListRequest listRequest = service.Events.List(calendarId);
            listRequest.TimeMin = DateTime.Now;
            listRequest.ShowDeleted = false;
            listRequest.SingleEvents = true;
            listRequest.MaxResults = 10;
            listRequest.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = listRequest.Execute();
            acumulador += "Upcoming events:\n";
            //Console.WriteLine("Upcoming events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime.ToString();
                    if (String.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                    }
                    acumulador += eventItem.Summary + " " + when + "\n";
                    //Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                }
            }
            else
            {
                acumulador += "No upcoming events found.\n\n";
                //Console.WriteLine("No upcoming events found.");
            }
            acumulador += "click for more .. \n\n";
            //Console.WriteLine("click for more .. ");


            var myevent = DB.Find(x => x.Id == "eventid" + 1);

            var InsertRequest = service.Events.Insert(myevent, calendarId);

            try
            {
                InsertRequest.Execute();
            }
            catch (Exception e)
            {
                try
                {
                    acumulador += e.Message;
                    service.Events.Update(myevent, calendarId, myevent.Id).Execute();
                    //Console.WriteLine("Insert/Update new Event ");
                    acumulador += "Insert/Update new Event ";

                }
                catch (Exception t)
                {
                    //Console.WriteLine("can't Insert/Update new Event ");
                    acumulador += "can't Insert/Update new Event \n\n" + t.Message;
                }
            }
        }


        static List<Event> DB =
             new List<Event>() {
                new Event(){
                    Id = "eventid" + 1,
                    Summary = "Google I/O 2015",
                    Location = "800 Howard St., San Francisco, CA 94103",
                    Description = "Una oportunidad de escuchar más sobre los productos para desarrolladores de Google.",
                    Start = new EventDateTime()
                    {
                        DateTime = new DateTime(2019, 01, 13, 15, 30, 0),
                        TimeZone = "America/Los_Angeles",
                    },
                    End = new EventDateTime()
                    {
                        DateTime = new DateTime(2019, 01, 14, 15, 30, 0),
                        TimeZone = "America/Los_Angeles",
                    },
                     Recurrence = new List<string> { "RRULE:FREQ=DAILY;COUNT=2" },
                    Attendees = new List<EventAttendee>
                    {
                        new EventAttendee() { Email = "lpage@example.com"},
                        new EventAttendee() { Email = "sbrin@example.com"}
                    }
                }
            };
    }
}
