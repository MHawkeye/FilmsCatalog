using FilmsCatalog.Data;
using FilmsCatalog.Models;
using FilmsCatalog.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FilmsCatalog.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly IWebHostEnvironment _hostEnvironment;

        public MovieController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        private User UserEmail()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.Email);

            var users = _db.Users.Where(c => c.Email == claims.Value.ToString());

                return users.First();
        }

        [AllowAnonymous]
        public IActionResult Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(AllFilms));
            }
            else
            {
                bool isEmail = false;

                var movie = _db.Movies.Include(c => c.User).Where(m => m.Id == id);

                if (movie.Any())
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        if (movie.First().User.Email == UserEmail().Email.ToString())
                        {
                            isEmail = true;
                        }
                    }

                    ViewBag.isEmail = isEmail;
                    return View(movie.First());
                }
                else
                {
                    return StatusCode(401);
                }
            }

        }
        [AllowAnonymous]
        public async Task<IActionResult> AllFilms(int page = 1)
        {
            int pageSize = 4;

            IQueryable<Movie> source = _db.Movies.Include(c => c.User);
            var count = await source.CountAsync();
            var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
 
            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = pageViewModel,
                Movies = items
            };
            return View(viewModel);
        }


        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name", "Description", "Year_of_release","Image", "Producer", "User", "Poster")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(movie.Image.FileName);
                    string extension = Path.GetExtension(movie.Image.FileName);

                    movie.Poster = fileName = fileName + DateTime.Now.ToString("yymmssfff")+extension;

                    string path = Path.Combine(wwwRootPath + "/images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await movie.Image.CopyToAsync(fileStream);
                    }

                    movie.User = UserEmail();

                    _db.Movies.Add(movie);
                    await _db.SaveChangesAsync();

                    return RedirectToAction("Index");
            }
            else
            {
                return View(movie);
            }
        }

    
    

        public IActionResult Edit(int id)
        {

            var movie = _db.Movies.Include(c => c.User).Where(m => m.Id == id && m.User.Email ==UserEmail().Email.ToString());

            if (movie.Any())
            {
                return View(movie.First());
            }

            else
            {
                return StatusCode(401);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Id","Name", "Description", "Year_of_release", "Image", "Producer", "User", "Poster")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                Movie movieOld = await _db.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == movie.Id);
                if (movie.Image != null)
                {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        if (movieOld.Poster != null)
                        {
                            var oldFile = Path.Combine(wwwRootPath + "/images/", movieOld.Poster);

                            if (System.IO.File.Exists(oldFile))
                            {
                                System.IO.File.Delete(oldFile);
                            }
                        }
                        string fileName = Path.GetFileNameWithoutExtension(movie.Image.FileName);
                        string extension = Path.GetExtension(movie.Image.FileName);

                        movie.Poster = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                        string path = Path.Combine(wwwRootPath + "/images/", fileName);
                        using var fileStream = new FileStream(path, FileMode.Create);
                        await movie.Image.CopyToAsync(fileStream);
                 }
                else
                {
                    movie.Poster = movieOld.Poster;
                }

                movie.User = movieOld.User;

                _db.Movies.Update(movie);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index");

            }
            return View(movie);
        }

        public async Task<IActionResult> EditAll(int page = 1)
        {
            int pageSize = 1;

            IQueryable<Movie> source = _db.Movies.Include(c => c.User).Where(m=>m.User.Email==UserEmail().Email.ToString());
            var count = await source.CountAsync();
            var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            IndexViewModel viewModel = new IndexViewModel
            {
                PageViewModel = pageViewModel,
                Movies = items
            };
            return View(viewModel);
        }
    }
}
