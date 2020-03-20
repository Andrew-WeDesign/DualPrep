using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DualPrep.Data;
using DualPrep.Models;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Drawing;

namespace DualPrep.Controllers
{
    public class BlogsController : Controller 
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlogsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Blogs.ToListAsync());
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var currentUser = await GetCurrentUserAsync();

            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            ViewBag.UserId = currentUser.Id;

            return View(blog);
        }

        // GET: Blogs/Create
        public async Task<IActionResult> CreateAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            Blog blog = new Blog();
            return View(blog);
        }

        // POST: Blogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog, IFormFile file)
        {
            var currentUser = await GetCurrentUserAsync();

            if (ModelState.IsValid)
            {
                blog.CreatedByUser = currentUser.Id;
                blog.Date = DateTime.Now;
                _context.Add(blog);
                await _context.SaveChangesAsync();

                UploadFile(file, blog.Id);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        public async void UploadFile(IFormFile file, int blogId)
        {
            if (file != null)
            {
                var fileName = file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\blogimages", Convert.ToString(blogId) + fileName);
                string imgurl = "/blogimages/" + Convert.ToString(blogId) + fileName;
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
                        var blog = await _context.Blogs.FindAsync(blogId);
                        blog.ImageUrl = imgurl;
                        _context.Update(blog);
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

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            var currentUser = await GetCurrentUserAsync();

            if (blog.CreatedByUser != currentUser.Id)
            {
                return NotFound();
            }

            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Blog blog)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();

            if (ModelState.IsValid)
            {
                try
                {
                    blog.CreatedByUser = currentUser.Id;
                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
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
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }


    }

}