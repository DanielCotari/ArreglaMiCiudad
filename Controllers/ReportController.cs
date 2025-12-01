using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArreglaMiCiudad.Data;
using ArreglaMiCiudad.Models;

namespace ArreglaMiCiudad.Controllers
{
    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var reports = _context.Reports
                .Include(r => r.Category)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return View(reports);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                })
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int categoryId, string description, string latitud, string longitud)
        {
            if (categoryId == 0 || string.IsNullOrWhiteSpace(description))
            {
                ViewBag.Categories = _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Name
                    })
                    .ToList();

                ModelState.AddModelError(string.Empty, "Completa la categoría y la descripción.");
                return View();
            }

            var report = new Report
            {
                UserId = 1,
                CategoryId = categoryId,
                Description = description,
                Latitud = latitud,
                Longitud = longitud,
                StatusId = 1,
                StatusText = "Pendiente",
                CreatedAt = DateTime.Now
            };

            _context.Reports.Add(report);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var report = _context.Reports
                .Include(r => r.Category)
                .FirstOrDefault(r => r.ReportId == id);

            if (report == null)
                return NotFound();

            return View(report);
        }
    }
}
