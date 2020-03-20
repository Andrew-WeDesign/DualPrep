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
using System.Net;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Drawing;


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

        [HttpGet]
        public async Task<IActionResult> Index(string mealSearch, string currentFilter, int? pageNumber)
        {
            var currentUser = await GetCurrentUserAsync();
            ViewBag.UserId = currentUser.Id;

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

            int pageSize = 10;
            return View(await PaginatedList<Meal>.CreateAsync(MealQuery.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Meals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var currentUser = await GetCurrentUserAsync();

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

            ViewBag.UserId = currentUser.Id;

            return View(meal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(Meal meal)
        {
            var currentUser = await GetCurrentUserAsync();

            MealFavorite mealFavorite = new MealFavorite
            {
                Id = currentUser.Id + meal.Id,
                MealId = meal.Id,
                ApplicationUserId = currentUser.Id
            };
            if (!MealFavoriteExists(mealFavorite.Id))
            {
                _context.Add(mealFavorite);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details));
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

        [HttpPost, ActionName("Favorites")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFavorites(string id)
        {
            var mealFavorite = await _context.MealFavorites.FindAsync(id);

            _context.MealFavorites.Remove(mealFavorite);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Favorites));
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
        public async Task<IActionResult> Create([Bind("Id,Name,Summary,Ingredients,Directions,PrepTime,CookTime,Author")] Meal meal, IFormFile file)
        {
            var currentUser = await GetCurrentUserAsync();

            if (ModelState.IsValid)
            {
                meal.CreatedByUser = currentUser.Id;
                _context.Add(meal);
                await _context.SaveChangesAsync();

                UploadFile(file, meal.Id);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(meal);
        }

        public async void UploadFile(IFormFile file, int mealId)
        {
            if (file != null)
            {
                var fileName = file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\mealimages", Convert.ToString(mealId) + fileName);
                string imgurl = "/mealimages/" + Convert.ToString(mealId) + fileName;
                string checkresult = "";

                if (System.IO.File.Exists(path))
                {
                    //Maybe add something here like: 
                    //file already exists, change file name and try again
                }
                else
                {
                    CheckFile(file, ref checkresult);
                    if (checkresult == "It is image")
                    {
                        using (var fileStream = new FileStream(path, FileMode.CreateNew))
                        {
                            file.CopyTo(fileStream);

                        }
                        var meal = await _context.Meals.FindAsync(mealId);
                        meal.ImageUrl = imgurl;
                        _context.Update(meal);
                    }
                }
            }
        }

        public string CheckFile(IFormFile file, ref string checkresult)
        {
            try
            {
                var isValidImage = Image.FromStream(file.OpenReadStream());
                checkresult = "It is image";
            }
            catch (Exception)
            {
                checkresult = "It is not image";
            }

            return checkresult;
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

            if (meal.CreatedByUser != currentUser.Id)
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

        private bool MealFavoriteExists(string id)
        {
            var currentUser = GetCurrentUserAsync();

            return _context.MealFavorites.Any(e => e.Id == id);
        }
    }
}
