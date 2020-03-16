using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DualPrep.Data;
using DualPrep.Models;
using DualPrep.Models.ExerciseViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;

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

            int pageSize = 10;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(Exercise exercise)
        {
            var currentUser = await GetCurrentUserAsync();

            ExerciseFavorite exerciseFavorite = new ExerciseFavorite
            {
                Id = currentUser.Id + exercise.Id,
                ExerciseId = exercise.Id,
                ApplicationUserId = currentUser.Id
            };

            if (!ExerciseFavoriteExists(exerciseFavorite.Id))
            {
                _context.Add(exerciseFavorite);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details));
        }

        public async Task<IActionResult> Favorites()
        {
            var currentUser = await GetCurrentUserAsync();
            FavoriteData vm = new FavoriteData();
            var applicationDbContext = _context.ExerciseFavorites
                .Include(o => o.Exercise)
                .Where(a => a.ApplicationUserId == currentUser.Id);
            vm.ExerciseFavorites = applicationDbContext;
            return View(vm);
        }

        [HttpPost, ActionName("Favorites")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFavorites(string id)
        {
            var exerciseFavorite = await _context.ExerciseFavorites.FindAsync(id);

            _context.ExerciseFavorites.Remove(exerciseFavorite);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Favorites));
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
        public async Task<IActionResult> Create([Bind("Id,Name,Summary,Equipment,Directions,Muscle")] Exercise exercise, IFormFile file)
        {
            var currentUser = await GetCurrentUserAsync();

            if (ModelState.IsValid)
            {
                exercise.CreatedByUser = currentUser.Id;
                _context.Add(exercise);
                await _context.SaveChangesAsync();

                UploadFile(file, exercise.Id);

                return RedirectToAction(nameof(Index));
            }

            return View(exercise);
        }

        public async void UploadFile(IFormFile file, int exerciseId)
        {
            if (file != null)
            {
                var fileName = file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/eximages", fileName);

                if (System.IO.File.Exists(path)) 
                { 
                    //Maybe add something here like: 
                    //file already exists, change file name and try again
                }
                else
                {
                    using (var fileStream = new FileStream(path, FileMode.CreateNew))
                    {
                        file.CopyTo(fileStream);
                        var exercise = await _context.Exercises.FindAsync(exerciseId);

                        exercise.ImageUrl = fileName;
                        _context.Update(exercise);
                    }

                }

                //await _context.SaveChangesAsync();
            }
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
                    exercise.CreatedByUser = currentUser.Id;
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

        private bool ExerciseFavoriteExists(string id)
        {
            var currentUser = GetCurrentUserAsync();

            return _context.ExerciseFavorites.Any(e => e.Id == id);
        }

    }
}