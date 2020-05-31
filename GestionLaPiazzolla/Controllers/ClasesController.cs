using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionLaPiazzolla.Data;
using GestionLaPiazzolla.Models;

namespace GestionLaPiazzolla.Controllers
{
    public class ClasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clases
        public async Task<IActionResult> Index(string sortOrder, string cadenaBusqueda, int id)
        {
            ViewData["FechaSortParametro"] = String.IsNullOrEmpty(sortOrder) ? "fecha_desc" : "";
            ViewData[""] = sortOrder == "Nombre" ? "nombre_desc" : "Nombre";
            ViewData[""] = sortOrder == "Observacion" ? "obsv_desc" : "Observacion";
            ViewData["FiltroActual"] = cadenaBusqueda;
            var clases = _context.Clases
                .Include(c => c.Curso)
                .Where(c => c.CursoId == id);
            if (!String.IsNullOrEmpty(cadenaBusqueda))
            {
                clases = clases.Where(c => c.Nombre.Contains(cadenaBusqueda));
            }
            switch (sortOrder)
            {
                case "fecha_desc":
                    clases = clases.OrderByDescending(c => c.FechaYHora);
                    break;
                case "Nombre":
                    clases = clases.OrderBy(c => c.Nombre);
                    break;
                case "nombre_desc":
                    clases = clases.OrderByDescending(c => c.Nombre);
                    break;
                case "Observacion":
                    clases = clases.OrderBy(c => c.Observacion);
                    break;
                case "obsv_desc":
                    clases = clases.OrderByDescending(c => c.Observacion);
                    break;
                default:
                    clases = clases.OrderBy(c => c.FechaYHora);
                    break;
            }
            var curso = _context.Cursos.Where(c => c.CursoId == id).FirstOrDefault();
            ViewBag.Nombre = curso.Nombre;
            ViewBag.IdCurso = curso.CursoId;
            return View(await clases.ToListAsync());
        }

        // GET: Clases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clase = await _context.Clases
                .Include(c => c.Curso)
                .FirstOrDefaultAsync(m => m.ClaseId == id);
            if (clase == null)
            {
                return NotFound();
            }

            return View(clase);
        }

        // GET: Clases/Create
        public IActionResult Create(int? id)
        {
            ViewBag.IdCurso = id;
            return View();
        }

        // POST: Clases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClaseId,Nombre,FechaYHora,Observacion,CursoId")] Clase clase)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clase);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CursoId"] = new SelectList(_context.Cursos, "CursoId", "Descripcion", clase.CursoId);
            return View(clase);
        }

        // GET: Clases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clase = await _context.Clases.FindAsync(id);
            if (clase == null)
            {
                return NotFound();
            }
            ViewData["CursoId"] = new SelectList(_context.Cursos, "CursoId", "Descripcion", clase.CursoId);
            return View(clase);
        }

        // POST: Clases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClaseId,Nombre,FechaYHora,Observacion,CursoId")] Clase clase)
        {
            if (id != clase.ClaseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClaseExists(clase.ClaseId))
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
            ViewData["CursoId"] = new SelectList(_context.Cursos, "CursoId", "Descripcion", clase.CursoId);
            return View(clase);
        }

        // GET: Clases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clase = await _context.Clases
                .Include(c => c.Curso)
                .FirstOrDefaultAsync(m => m.ClaseId == id);
            if (clase == null)
            {
                return NotFound();
            }

            return View(clase);
        }

        // POST: Clases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clase = await _context.Clases.FindAsync(id);
            _context.Clases.Remove(clase);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClaseExists(int id)
        {
            return _context.Clases.Any(e => e.ClaseId == id);
        }
    }
}
