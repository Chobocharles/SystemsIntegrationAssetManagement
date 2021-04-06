using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Asset_Management.Models.SQL;

namespace Asset_Management.Controllers
{
    public class AssetsController : Controller
    {
        private readonly AssetContext _context;

        public AssetsController(AssetContext context)
        {
            _context = context;
        }

        // GET: Assets
        public async Task<IActionResult> Index()
        {
            var assetContext = _context.Asset.Include(a => a.AssetType).Include(a => a.Condition).Include(a => a.Contact).Include(a => a.Location);
            return View(await assetContext.ToListAsync());
        }

        // GET: Assets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Asset
                .Include(a => a.AssetType)
                .Include(a => a.Condition)
                .Include(a => a.Contact)
                .Include(a => a.Location)
                .FirstOrDefaultAsync(m => m.AssetId == id);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }

        // GET: Assets/Create
        public IActionResult Create()
        {
            ViewData["AssetTypeId"] = new SelectList(_context.AssetType, "AssetTypeId", "AssetTypeId");
            ViewData["ConditionId"] = new SelectList(_context.Condition, "ConditionId", "ConditionId");
            ViewData["ContactId"] = new SelectList(_context.Contact, "ContactId", "ContactId");
            ViewData["LocationId"] = new SelectList(_context.Location, "LocationId", "LocationId");
            return View();
        }

        // POST: Assets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssetId,AssetTypeId,Description,ConditionId,AcquiredDate,PurchasePrice,CurrentValue,LocationId,Brand,Model,Comments,ContactId,RetiredDate,AssetTagNumber,SerialNumber,ServiceTag,WarrantyExpires,DeviceId,Verified,DateVerified,WorkCenter")] Asset asset)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssetTypeId"] = new SelectList(_context.AssetType, "AssetTypeId", "AssetTypeId", asset.AssetTypeId);
            ViewData["ConditionId"] = new SelectList(_context.Condition, "ConditionId", "ConditionId", asset.ConditionId);
            ViewData["ContactId"] = new SelectList(_context.Contact, "ContactId", "ContactId", asset.ContactId);
            ViewData["LocationId"] = new SelectList(_context.Location, "LocationId", "LocationId", asset.LocationId);
            return View(asset);
        }

        // GET: Assets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Asset.FindAsync(id);
            if (asset == null)
            {
                return NotFound();
            }
            ViewData["AssetTypeId"] = new SelectList(_context.AssetType, "AssetTypeId", "AssetTypeId", asset.AssetTypeId);
            ViewData["ConditionId"] = new SelectList(_context.Condition, "ConditionId", "ConditionId", asset.ConditionId);
            ViewData["ContactId"] = new SelectList(_context.Contact, "ContactId", "ContactId", asset.ContactId);
            ViewData["LocationId"] = new SelectList(_context.Location, "LocationId", "LocationId", asset.LocationId);
            return View(asset);
        }

        // POST: Assets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssetId,AssetTypeId,Description,ConditionId,AcquiredDate,PurchasePrice,CurrentValue,LocationId,Brand,Model,Comments,ContactId,RetiredDate,AssetTagNumber,SerialNumber,ServiceTag,WarrantyExpires,DeviceId,Verified,DateVerified,WorkCenter")] Asset asset)
        {
            if (id != asset.AssetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssetExists(asset.AssetId))
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
            ViewData["AssetTypeId"] = new SelectList(_context.AssetType, "AssetTypeId", "AssetTypeId", asset.AssetTypeId);
            ViewData["ConditionId"] = new SelectList(_context.Condition, "ConditionId", "ConditionId", asset.ConditionId);
            ViewData["ContactId"] = new SelectList(_context.Contact, "ContactId", "ContactId", asset.ContactId);
            ViewData["LocationId"] = new SelectList(_context.Location, "LocationId", "LocationId", asset.LocationId);
            return View(asset);
        }

        // GET: Assets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Asset
                .Include(a => a.AssetType)
                .Include(a => a.Condition)
                .Include(a => a.Contact)
                .Include(a => a.Location)
                .FirstOrDefaultAsync(m => m.AssetId == id);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }

        // POST: Assets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asset = await _context.Asset.FindAsync(id);
            _context.Asset.Remove(asset);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssetExists(int id)
        {
            return _context.Asset.Any(e => e.AssetId == id);
        }
    }
}
