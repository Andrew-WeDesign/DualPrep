﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DualPrep.Data;
using DualPrep.Models;
using Microsoft.AspNetCore.Identity;

namespace DualPrep.Controllers
{
    public class ExercisesController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExercisesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Exercises
        //public async Task<IActionResult> Index()
        //{
        //    var currentUser = await GetCurrentUserAsync();
        //    return View(await _context.Exercises.ToListAsync());
        //}

        [HttpGet]
        public async Task<IActionResult> Index(string exerciseSearch, string currentFilter, int? pageNumber)
        {
            if (exerciseSearch != null)
            {
                pageNumber = 1;
            }
            else
            {
                exerciseSearch = currentFilter;
            }

            ViewData["GetExerciseSearch"] = exerciseSearch;

            var ExerciseQuery = from x in _context.Exercises select x;

            if (!string.IsNullOrEmpty(exerciseSearch))
            {
                ExerciseQuery = ExerciseQuery.Where(x => x.Name.Contains(exerciseSearch) || x.Summary.Contains(exerciseSearch));
            }

            int pageSize = 3;
            return View(await PaginatedList<Exercise>.CreateAsync(ExerciseQuery.AsNoTracking(), pageNumber ?? 1, pageSize));
        }


        // GET: Exercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exercise == null)
            {
                return NotFound();
            }

            return View(exercise);
        }

        // GET: Exercises/Create
        public async Task<IActionResult> CreateAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            Exercise exercise = new Exercise();
            return View(exercise);
        }

        // POST: Exercises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Summary,Equipment,Directions,Muscle")] Exercise exercise)
        {
            var currentUser = await GetCurrentUserAsync();

            if (ModelState.IsValid)
            {
                exercise.CreatedByUser = currentUser.UserName;
                _context.Add(exercise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exercise);
        }

        // GET: Exercises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercises.FindAsync(id);
            var currentUser = await GetCurrentUserAsync();


            if (exercise.CreatedByUser != currentUser.UserName)
            {
                return NotFound();
            }


            if (exercise == null)
            {
                return NotFound();
            }
            return View(exercise);
        }

        // POST: Exercises/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, /*[Bind(include: "Id,Name,Summary,Equipment,Directions,Muscle")]*/ Exercise exercise)
        {
            if (id != exercise.Id)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();

            if (ModelState.IsValid)
            {
                try
                {
                    exercise.CreatedByUser = currentUser.UserName;
                    _context.Update(exercise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExerciseExists(exercise.Id))
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
            return View(exercise);
        }

        // GET: Meals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exercise == null)
            {
                return NotFound();
            }

            return View(exercise);
        }

        // POST: Meals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExerciseExists(int id)
        {
            return _context.Exercises.Any(e => e.Id == id);
        }

    }
}