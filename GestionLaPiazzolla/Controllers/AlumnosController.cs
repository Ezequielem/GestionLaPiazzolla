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

namespace GestionLaPiazzolla.Controllers
{
    [Authorize]
    public class AlumnosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlumnosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Alumnos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Alumnos.ToListAsync());
        }

        // GET: Alumnos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos
                .Include(a => a.Direccion.Localidad.Departamento.Provincia)
                .Include(a => a.Sexo)
                .FirstOrDefaultAsync(m => m.AlumnoId == id);
            if (alumno == null)
            {
                return NotFound();
            }

            return View(alumno);
        }

        // GET: Alumnos/Create
        public IActionResult Create()
        {
            var alumno = new Alumno();
            alumno.Direccion = new Direccion();
            listaDeSexos();
            return View();
        }

        // POST: Alumnos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlumnoId,Nombre,Apellido,Dni,FechaDeNacimiento,Email,SexoId,Direccion")] Alumno alumno)
        {
            if (ModelState.IsValid)
            {
                alumno.Direccion.Localidad = null;
                _context.Add(alumno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            listaDeSexos(alumno.SexoId);
            return View(alumno);
        }

        // GET: Alumnos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos.Include(a => a.Direccion.Localidad.Departamento.Provincia)
                .FirstOrDefaultAsync(a => a.AlumnoId == id);
            listaDeSexos();
            listaProvincias();
            listaDepartamentosProvinciaSeleccionada(alumno.Direccion.Localidad.Departamento.Provincia.ProvinciaId);
            listaLocalidadesDepartamentosSeleccionados(alumno.Direccion.Localidad.Departamento.DepartamentoId);
            if (alumno == null)
            {
                return NotFound();
            }            
            return View(alumno);
        }

        // POST: Alumnos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlumnoId,Nombre,Apellido,Dni,FechaDeNacimiento,Email,SexoId,Direccion,DireccionId")] Alumno alumno)
        {
            if (id != alumno.AlumnoId)
            {
                return NotFound();
            }
            if (alumno.Direccion.LocalidadId == 0)
            {
                ModelState.AddModelError("Localidad", "Debe seleccionar una localidad");
            }
            if (alumno.Direccion.Localidad.DepartamentoId == 0)
            {
                ModelState.AddModelError("Departamento", "Debe seleccionar un Departamento");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    alumno.Direccion.Localidad = null;
                    _context.Update(alumno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlumnoExists(alumno.AlumnoId))
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
            listaDeSexos(alumno.SexoId);
            listaProvincias(alumno.Direccion.Localidad.Departamento.ProvinciaId);
            if (alumno.Direccion.Localidad.DepartamentoId != 0)
            {
                listaDepartamentosProvinciaSeleccionada(alumno.Direccion.Localidad.Departamento.Provincia.ProvinciaId, alumno.Direccion.Localidad.DepartamentoId);
                if (alumno.Direccion.LocalidadId != 0)
                {
                    listaLocalidadesDepartamentosSeleccionados(alumno.Direccion.Localidad.Departamento.DepartamentoId, alumno.Direccion.LocalidadId);
                }
            }
            return View(alumno);
        }

        // GET: Alumnos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos
                .Include(a => a.Direccion.Localidad.Departamento.Provincia)
                .FirstOrDefaultAsync(m => m.AlumnoId == id);
            if (alumno == null)
            {
                return NotFound();
            }

            return View(alumno);
        }

        // POST: Alumnos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alumno = await _context.Alumnos.Include(a => a.Direccion).FirstOrDefaultAsync(a => a.AlumnoId == id);            
            _context.Alumnos.Remove(alumno);
            _context.Direcciones.Remove(alumno.Direccion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlumnoExists(int id)
        {
            return _context.Alumnos.Any(e => e.AlumnoId == id);
        }

        public void listaDeSexos(object sexoSeleccionado = null)
        {
            var consultaSexo = from s in _context.Sexo
                               select s;
            ViewBag.SexoId = new SelectList(consultaSexo.AsNoTracking(), "SexoId", "Descripcion", sexoSeleccionado);
        }

        public void listaProvincias(object provinciaSeleccionada = null)
        {
            var consultaProvincias = from p in _context.Provincias
                                     orderby p.Nombre
                                     select p;
            ViewBag.ProvinciaId = new SelectList(consultaProvincias.AsNoTracking(), "ProvinciaId", "Nombre", provinciaSeleccionada);
        }

        public void listaDepartamentosProvinciaSeleccionada(int idProvincia, object departamentoSeleccionado = null)
        {
            var consultaDepartamentos = from d in _context.Departamentos
                                        where d.ProvinciaId == idProvincia
                                        orderby d.Nombre
                                        select d;
            ViewBag.DepartamentoId = new SelectList(consultaDepartamentos.AsNoTracking(), "DepartamentoId", "Nombre", departamentoSeleccionado);
        }

        public void listaLocalidadesDepartamentosSeleccionados(int idDepartamento, object localidadSeleccionada = null)
        {
            var consultaLocalidades = from l in _context.Localidades
                                      where l.DepartamentoId == idDepartamento
                                      orderby l.Nombre
                                      select l;
            ViewBag.LocalidadId = new SelectList(consultaLocalidades, "LocalidadId", "Nombre", localidadSeleccionada);
        }

        public JsonResult obtenerProvincias()
        {
            var consultaProvincias = from p in _context.Provincias
                                     orderby p.Nombre
                                     select new
                                     {
                                         id = p.ProvinciaId,
                                         nombre = p.Nombre
                                     };
            return Json(consultaProvincias);
        }

        public JsonResult obtenerDepartamentos(int ProvId)
        {
            var consultaDepartamentos = from d in _context.Departamentos
                                        where d.ProvinciaId == ProvId
                                        orderby d.Nombre
                                        select new
                                        {
                                            id = d.DepartamentoId,
                                            nombre = d.Nombre
                                        };
            return Json(consultaDepartamentos);
        }

        public JsonResult obtenerLocalidades(int DeptoId)
        {
            var consultaLocalidades = from l in _context.Localidades
                                      where l.DepartamentoId == DeptoId
                                      orderby l.Nombre
                                      select new
                                      {
                                          id = l.LocalidadId,
                                          nombre = l.Nombre
                                      };
            return Json(consultaLocalidades);
        }
    }
}
