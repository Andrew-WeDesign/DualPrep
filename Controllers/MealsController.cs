using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DualPrep.Data;
using DualPrep.Models;
using DualPrep.Models.MealViewModels;
using Microsoft.AspNetCore.Identity;
using System.Collections;

namespace DualPrep.Controllers
{
    public class MealsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MealsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Meals
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Meals.ToListAsync());
        //}

        [HttpGet]
        public async Task<IActionResult> Index(string mealSearch, string currentFilter, int? pageNumber)
        {
            if (mealSearch != null)
            {
                pageNumber = 1;
            }
            else
            {
                mealSearch = currentFilter;
            }

            ViewData["GetMealSearch"] = mealSearch;

            var MealQuery = from x in _context.Meals select x;

            if (!string.IsNullOrEmpty(mealSearch))
            {
                MealQuery = MealQuery.Where(x => x.Name.Contains(mealSearch) || x.Summary.Contains(mealSearch) || x.Ingredients.Contains(mealSearch));
            }

            int pageSize = 3;
            return View(await PaginatedList<Meal>.CreateAsync(MealQuery.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Meals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meal = await _context.Meals
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meal == null)
            {
                return NotFound();
            }

            return View(meal);
        }

        public async Task<IActionResult> CreateFavorites(MealFavorite mealFavorite, Meal meal)
        {
            var currentUser = await GetCurrentUserAsync();
            mealFavorite.MealId = meal.Id;
            mealFavorite.ApplicationUserId = currentUser.Id;

            return View(mealFavorite);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFavorites(MealFavorite mealFavorite)
        {
            _context.Add(mealFavorite);
            await _context.SaveChangesAsync();

            return View(mealFavorite);
        }

        public async Task<IActionResult> Favorites()
        {
            var currentUser = await GetCurrentUserAsync();
            FavoriteData vm = new FavoriteData();
            var applicationDbContext = _context.MealFavorites
                .Include(o => o.Meal)
                .Where(a => a.ApplicationUserId == currentUser.Id);
            vm.MealFavorites = applicationDbContext;
            return View(vm);
        }

        // GET: Meals/Create
        public async Task<IActionResult> CreateAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            Meal meal = new Meal();
            return View(meal);
        }

        // POST: Meals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Summary,Ingredients,Directions,PrepTime,CookTime,Author")] Meal meal)
        {
            var currentUser = await GetCurrentUserAsync();

            if (ModelState.IsValid)
            {
                meal.CreatedByUser = currentUser.UserName;
                _context.Add(meal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(meal);
        }

        // GET: Meals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meal = await _context.Meals.FindAsync(id);
            var currentUser = await GetCurrentUserAsync();

            if (meal.CreatedByUser != currentUser.UserName)
            {
                return NotFound();
            }

            if (meal == null)
            {
                return NotFound();
            }
            return View(meal);
        }

        // POST: Meals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, /*[Bind("Id,Name,Summary,Ingredients,Directions,Author")]*/ Meal meal)
        {
            if (id != meal.Id)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();

            if (ModelState.IsValid)
            {
                try
                {
                    meal.CreatedByUser = currentUser.UserName;
                    _context.Update(meal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MealExists(meal.Id))
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
            return View(meal);
        }

        // GET: Meals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meal = await _context.Meals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (meal == null)
            {
                return NotFound();
            }

            return View(meal);
        }

        // POST: Meals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var meal = await _context.Meals.FindAsync(id);
            _context.Meals.Remove(meal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MealExists(int id)
        {
            return _context.Meals.Any(e => e.Id == id);
        }
    }
}
