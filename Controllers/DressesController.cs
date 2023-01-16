using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Herbet_Ioana_Proiect.Data;
using Herbet_Ioana_Proiect.Models;
using static System.Reflection.Metadata.BlobBuilder;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Herbet_Ioana_Proiect.Controllers
{
    [Authorize(Roles = "Employee")]
    public class DressesController : Controller
    {
        private readonly DressesContext _context;

        public DressesController(DressesContext context)
        {
            _context = context;
        }

        // GET: Dresses
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string sortOrder,
             string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var dresses = from b in _context.Dresses
                         join a in _context.Designers on b.DesignerID equals a.ID
                         select new Dress
                         {
                             ID = b.ID,
                             Title = b.Title,
                             DesignerID = b.DesignerID,
                             Price = b.Price,
                             Image = b.Image,
                             Designer = a
                         };
            if (!String.IsNullOrEmpty(searchString))
            {
                dresses = dresses.Where(s => s.Title.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    dresses = dresses.OrderByDescending(b => b.Title);
                    break;
                case "Price":
                    dresses = dresses.OrderBy(b => b.Price);
                    break;
                case "price_desc":
                    dresses = dresses.OrderByDescending(b => b.Price);
                    break;
                default:
                    dresses = dresses.OrderBy(b => b.Title);
                    break;
            }
            int pageSize = 2;
            return View(await PaginatedList<Dress>.CreateAsync(dresses.AsNoTracking(), pageNumber ??
           1, pageSize));

        }

        // GET: Dresses/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Dresses == null)
            {
                return NotFound();
            }

            var dress = await _context.Dresses
                .Include(d => d.Designer)
                .Include(s => s.Orders)
                .ThenInclude(e => e.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (dress == null)
            {
                return NotFound();
            }

            return View(dress);
        }

        // GET: Dresses/Create
        public IActionResult Create()
        {
            var designers = _context.Designers.Select(x => new
            {
                x.ID,
                FullName = x.FirstName + " " + x.LastName
            });
            ViewData["DesignerID"] = new SelectList(designers, "ID", "FullName");
            return View();
        }

        // POST: Dresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Image,DesignerID,Price")] Dress dress)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    _context.Add(dress);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex*/)
            {

                ModelState.AddModelError("", "Unable to save changes. " +
                "Try again, and if the problem persists ");
            }

            ViewData["DesignerID"] = new SelectList(_context.Designers, "ID", "ID", dress.DesignerID);

            return View(dress);
        }

        // GET: Dresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Dresses == null)
            {
                return NotFound();
            }

            var dress = await _context.Dresses.FindAsync(id);
            if (dress == null)
            {
                return NotFound();
            }
            var designers = _context.Designers.Select(x => new
            {
                x.ID,
                FullName = x.FirstName + " " + x.LastName
            });
            ViewData["DesignerID"] = new SelectList(designers, "ID", "FullName");
            return View(dress);
        }

        // POST: Dresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dressToUpdate = await _context.Dresses.FirstOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<Dress>(
            dressToUpdate,
            "",
            s => s.DesignerID, s => s.Title, s => s.Image, s => s.Price))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists");
                }
            }
         
        ViewData["DesignerID"] = new SelectList(_context.Designers, "ID", "ID", dressToUpdate.DesignerID);
            return View(dressToUpdate);
        }

        // GET: Dresses/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null || _context.Dresses == null)
            {
                return NotFound();
            }

            var dress = await _context.Dresses
                .Include(d => d.Designer)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (dress == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                "Delete failed. Try again";
            }

            return View(dress);
        }

        // POST: Dresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dress = await _context.Dresses.FindAsync(id);
            if (dress == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Dresses.Remove(dress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {

                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool DressExists(int id)
        {
          return _context.Dresses.Any(e => e.ID == id);
        }
    }
}
