using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Herbet_Ioana_Proiect.Data;
using Herbet_Ioana_Proiect.Models;
using Herbet_Ioana_Proiect.Models.DressesViewModels;
using System.Security.Policy;
using Microsoft.AspNetCore.Authorization;

namespace Herbet_Ioana_Proiect.Controllers
{
    [Authorize(Policy = "OnlySales")]
    public class ShopsController : Controller
    {
        private readonly DressesContext _context;

        public ShopsController(DressesContext context)
        {
            _context = context;
        }

        // GET: Shops
        public async Task<IActionResult> Index(int? id, int? dressID)
        {
            var viewModel = new ShopIndexData();
            viewModel.Shops = await _context.Shops
            .Include(i => i.AvailableDresses)
            .ThenInclude(i => i.Dress)
            .ThenInclude(i => i.Orders)
            .ThenInclude(i => i.Customer)
            .Include(i => i.AvailableDresses)
            .ThenInclude(i => i.Dress)
            .ThenInclude(i => i.Designer)
            .AsNoTracking()
            .OrderBy(i => i.ShopName)
            .ToListAsync();
            if (id != null)
            {
                ViewData["ShopID"] = id.Value;
                Shop shop = viewModel.Shops.Where(
                i => i.ID == id.Value).Single();
                viewModel.Dresses = shop.AvailableDresses.Select(s => s.Dress);
            }
            if (dressID != null)
            {
                ViewData["DressID"] = dressID.Value;
                viewModel.Orders = viewModel.Dresses.Where(
                x => x.ID == dressID).Single().Orders;
            }
            return View(viewModel);
        }

        // GET: Shops/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Shops == null)
            {
                return NotFound();
            }

            var shop = await _context.Shops
                .FirstOrDefaultAsync(m => m.ID == id);
            if (shop == null)
            {
                return NotFound();
            }

            return View(shop);
        }

        // GET: Shops/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Shops/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ShopName,Adress")] Shop shop)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shop);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shop);
        }

        // GET: Shops/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shop = await _context.Shops
            .Include(i => i.AvailableDresses).ThenInclude(i => i.Dress)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);
            if (shop == null)
            {
                return NotFound();
            }
            PopulateAvailableDressData(shop);
            return View(shop);

        }
        private void PopulateAvailableDressData(Shop shop)
        {
            var allDresses = _context.Dresses;
            var shopDresses = new HashSet<int>(shop.AvailableDresses.Select(c => c.DressID));
            var viewModel = new List<AvailableDressData>();
            foreach (var dress in allDresses)
            {
                viewModel.Add(new AvailableDressData
                {
                    DressID = dress.ID,
                    Title = dress.Title,
                    IsAvailable = shopDresses.Contains(dress.ID)
                });
            }
            ViewData["Dresses"] = viewModel;
        }


    // POST: Shops/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedDresses)
        {
            if (id == null)
            {
                return NotFound();
            }
            var shopToUpdate = await _context.Shops
            .Include(i => i.AvailableDresses)
            .ThenInclude(i => i.Dress)
            .FirstOrDefaultAsync(m => m.ID == id);
            if (await TryUpdateModelAsync<Shop>(
            shopToUpdate,
            "",
            i => i.ShopName, i => i.Adress))
            {
                UpdateAvailableDresses(selectedDresses, shopToUpdate);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {

                    ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, ");
                }
                return RedirectToAction(nameof(Index));
            }
            UpdateAvailableDresses(selectedDresses, shopToUpdate);
            PopulateAvailableDressData(shopToUpdate);
            return View(shopToUpdate);
        }
        private void UpdateAvailableDresses(string[] selectedDresses, Shop shopToUpdate)
        {
            if (selectedDresses == null)
            {
                shopToUpdate.AvailableDresses = new List<AvailableDress>();
                return;
            }
            var selectedDressesHS = new HashSet<string>(selectedDresses);
            var availableDresses = new HashSet<int>
            (shopToUpdate.AvailableDresses.Select(c => c.Dress.ID));
            foreach (var dress in _context.Dresses)
            {
                if (selectedDressesHS.Contains(dress.ID.ToString()))
                {
                    if (!availableDresses.Contains(dress.ID))
                    {
                        shopToUpdate.AvailableDresses.Add(new AvailableDress
                        {
                            ShopID = shopToUpdate.ID,
                            DressID = dress.ID
                        });
                    }
                }
                else
                {
                    if (availableDresses.Contains(dress.ID))
                    {
                        AvailableDress dressToRemove = shopToUpdate.AvailableDresses.FirstOrDefault(i
                       => i.DressID == dress.ID);
                        _context.Remove(dressToRemove);
                    }
                }
            }
        }

        // GET: Shops/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Shops == null)
            {
                return NotFound();
            }

            var shop = await _context.Shops
                .FirstOrDefaultAsync(m => m.ID == id);
            if (shop == null)
            {
                return NotFound();
            }

            return View(shop);
        }

        // POST: Shops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Shops == null)
            {
                return Problem("Entity set 'DressesContext.Shops'  is null.");
            }
            var shop = await _context.Shops.FindAsync(id);
            if (shop != null)
            {
                _context.Shops.Remove(shop);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShopExists(int id)
        {
          return _context.Shops.Any(e => e.ID == id);
        }
    }
}
