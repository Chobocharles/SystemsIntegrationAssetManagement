using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Asset_Management.Models.SQL;
using Microsoft.AspNetCore.Http;
using System.IO;

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
            var assetContext = _context.Asset.Include(a => a.AssetType).Include(a => a.Condition).Include(a => a.Contact).Include(a => a.Location).Include(a => a.ServiceRecord);
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
                .Include(a => a.ServiceRecord)
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
            ViewData["AssetType"] = new SelectList(_context.AssetType, "AssetTypeId", "AssetType1");
            ViewData["Condition"] = new SelectList(_context.Condition, "ConditionId", "Condition1");
            ViewData["Contact"] = new SelectList(_context.Contact, "ContactId", "DisplayName");
            ViewData["Location"] = new SelectList(_context.Location, "LocationId", "Location1");
            return View();
        }

        // POST: Assets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssetId,AssetTypeId,Description,ConditionId,AcquiredDate,PurchasePrice,CurrentValue,LocationId,Brand,Model,Comments,ContactId,RetiredDate,AssetTagNumber,SerialNumber,ServiceTag,WarrantyExpires,DeviceId,Verified,DateVerified,WorkCenter")] Asset asset, IFormFile img)
        {
            if (ModelState.IsValid)
            {
                if (img != null)
                {
                    asset.Picture = GetByteArrayFromImage(img);
                    asset.PictureSourceFileName = Path.GetFileName(img.FileName);
                    asset.PictureContentType = img.ContentType;
                }

                _context.Add(asset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssetType"] = new SelectList(_context.AssetType, "AssetTypeId", "AssetType1", asset.AssetTypeId);
            ViewData["Condition"] = new SelectList(_context.Condition, "ConditionId", "Condition1", asset.ConditionId);
            ViewData["Contact"] = new SelectList(_context.Contact, "ContactId", "DisplayName", asset.ContactId);
            ViewData["Location"] = new SelectList(_context.Location, "LocationId", "Location1", asset.LocationId);
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
            ViewData["AssetType"] = new SelectList(_context.AssetType, "AssetTypeId", "AssetType1", asset.AssetTypeId);
            ViewData["Condition"] = new SelectList(_context.Condition, "ConditionId", "Condition1", asset.ConditionId);
            ViewData["Contact"] = new SelectList(_context.Contact, "ContactId", "DisplayName", asset.ContactId);
            ViewData["Location"] = new SelectList(_context.Location, "LocationId", "Location1", asset.LocationId);
            return View(asset);
        }

        // POST: Assets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssetId,AssetTypeId,Description,ConditionId,AcquiredDate,PurchasePrice,CurrentValue,LocationId,Brand,Model,Comments,ContactId,RetiredDate,AssetTagNumber,SerialNumber,ServiceTag,WarrantyExpires,DeviceId,Verified,DateVerified,WorkCenter")] Asset asset, IFormFile img)
        {
            if (id != asset.AssetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var assetFound = await _context.Asset.FindAsync(asset.AssetId);

                if (img != null)
                {
                    asset.Picture = GetByteArrayFromImage(img);
                    asset.PictureSourceFileName = Path.GetFileName(img.FileName);
                    asset.PictureContentType = img.ContentType;
                }
                else
                {
                    if (assetFound.Picture != null)
                    {
                        asset.Picture = assetFound.Picture;
                        asset.PictureSourceFileName = assetFound.PictureSourceFileName;
                        asset.PictureContentType = assetFound.PictureContentType;
                    }
                }

                try
                {
                    _context.Entry(assetFound).CurrentValues.SetValues(asset);
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
            ViewData["AssetType"] = new SelectList(_context.AssetType, "AssetTypeId", "AssetType1", asset.AssetTypeId);
            ViewData["Condition"] = new SelectList(_context.Condition, "ConditionId", "Condition1", asset.ConditionId);
            ViewData["Contact"] = new SelectList(_context.Contact, "ContactId", "DisplayName", asset.ContactId);
            ViewData["Location"] = new SelectList(_context.Location, "LocationId", "Location1", asset.LocationId);
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

        private byte[] GetByteArrayFromImage(IFormFile file)
        {
            using var target = new MemoryStream();
            file.CopyTo(target);
            return target.ToArray();
        }
    }
}
