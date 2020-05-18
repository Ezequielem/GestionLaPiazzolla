﻿using System;
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
    public class ProfesoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfesoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Profesores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Profesores.ToListAsync());
        }

        // GET: Profesores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profesor = await _context.Profesores
                .Include(p => p.Direccion.Localidad.Departamento.Provincia)
                .Include(p => p.Sexo)
                .FirstOrDefaultAsync(m => m.ProfesorId == id);
            if (profesor == null)
            {
                return NotFound();
            }

            return View(profesor);
        }

        // GET: Profesores/Create
        public IActionResult Create()
        {
            var profesor = new Profesor();
            profesor.Direccion = new Direccion();
            listaDeSexos();
            return View();
        }

        // POST: Profesores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProfesorId,Nombre,Apellido,Dni,FechaDeNacimiento,Email,SexoId,Direccion")] Profesor profesor)
        {
            if (ModelState.IsValid)
            {
                profesor.Direccion.Localidad = null;
                _context.Add(profesor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            listaDeSexos(profesor.SexoId);
            return View(profesor);
        }

        // GET: Profesores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profesor = await _context.Profesores.Include(m => m.Direccion.Localidad.Departamento.Provincia).
                FirstOrDefaultAsync(m => m.ProfesorId == id);           
            listaDeSexos();
            listaProvincias();
            listaDepartamentosProvinciaSeleccionada(profesor.Direccion.Localidad.Departamento.Provincia.ProvinciaId);
            listaLocalidadesDepartamentosSeleccionados(profesor.Direccion.Localidad.Departamento.DepartamentoId);
            if (profesor == null)
            {
                return NotFound();
            }
            return View(profesor);
        }

        // POST: Profesores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProfesorId,Nombre,Apellido,Dni,FechaDeNacimiento,Email,SexoId,Direccion,DireccionId")] Profesor profesor)
        {
            if (id != profesor.ProfesorId)
            {
                return NotFound();
            }
            if (profesor.Direccion.LocalidadId == 0)
            {
                ModelState.AddModelError("Localidad", "Debe seleccionar una localidad");
            }
            if (profesor.Direccion.Localidad.DepartamentoId == 0)
            {
                ModelState.AddModelError("Departamento", "Debe seleccionar un Departamento");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    profesor.Direccion.Localidad = null;
                    _context.Update(profesor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfesorExists(profesor.ProfesorId))
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
            listaDeSexos(profesor.SexoId);
            listaProvincias(profesor.Direccion.Localidad.Departamento.ProvinciaId);
            if (profesor.Direccion.Localidad.DepartamentoId != 0)
            {
                listaDepartamentosProvinciaSeleccionada(profesor.Direccion.Localidad.Departamento.Provincia.ProvinciaId, profesor.Direccion.Localidad.DepartamentoId);
                if (profesor.Direccion.LocalidadId != 0)
                {
                    listaLocalidadesDepartamentosSeleccionados(profesor.Direccion.Localidad.Departamento.DepartamentoId, profesor.Direccion.LocalidadId);
                }
            }
            return View(profesor);
        }      

        // GET: Profesores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profesor = await _context.Profesores
                .Include(m => m.Direccion.Localidad.Departamento.Provincia)
                .FirstOrDefaultAsync(m => m.ProfesorId == id);
            if (profesor == null)
            {
                return NotFound();
            }

            return View(profesor);
        }

        // POST: Profesores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profesor = await _context.Profesores.Include(m => m.Direccion).FirstOrDefaultAsync(m => m.ProfesorId == id);            
            _context.Profesores.Remove(profesor);
            _context.Direcciones.Remove(profesor.Direccion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfesorExists(int id)
        {
            return _context.Profesores.Any(e => e.ProfesorId == id);
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
