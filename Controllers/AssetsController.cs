﻿using Asset_Management.Models;
using Asset_Management.Models.SQL;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Asset_Management.Controllers
{
    public class AssetsController : Controller
    {
        private readonly AssetContext _context;

        private readonly IConfiguration _configuration;

        public AssetsController(AssetContext context, IConfiguration configuration)
        {
            _context = context;

            _configuration = configuration;
        }

        public byte[] WriteCsvToMemory(dynamic records)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.Context.RegisterClassMap<AssetdMap>();
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }

        public sealed class AssetdMap : ClassMap<Asset>
        {
            public AssetdMap()
            {
                Map(m => m.AssetId).Name("Asset ID");
                Map(m => m.AssetType.AssetType1).Name("Asset Type");
                Map(m => m.Description).Name("Description");
                Map(m => m.Condition.Condition1).Name("Condition");
                Map(m => m.AcquiredDate).Name("Acquired Date");
                Map(m => m.PurchasePrice).Name("Purchase Price");
                Map(m => m.CurrentValue).Name("Current Value");
                Map(m => m.Location.Location1).Name("Location");
                Map(m => m.Brand).Name("Brand");
                Map(m => m.Model).Name("Model");
                Map(m => m.Comments).Name("Comments");
                Map(m => m.Contact.DisplayName).Name("Assigned Contact");
                Map(m => m.RetiredDate).Name("Retired Date");
                Map(m => m.AssetTagNumber).Name("Asset Tag Number");
                Map(m => m.SerialNumber).Name("Serial Number");
                Map(m => m.ServiceTag).Name("Service Tag");
                Map(m => m.WarrantyExpires).Name("Warranty Expires");
                Map(m => m.DeviceId).Name("Device ID");
                Map(m => m.Verified).Name("Is Verified");
                Map(m => m.DateVerified).Name("Verified Date");
                Map(m => m.WorkCenter).Name("WorkCenter");
                Map(m => m.PictureSourceFileName).Name("Picture Source FileName");
            }
        }

        // GET: Assets
        public async Task<IActionResult> Index(string sortOrder,
    string currentFilter,
    string searchString,
    int? pageNumber,
    bool export)
        {
            ViewData["config"] = _configuration["AzureAd:Roles:ReadWrite"];

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            IQueryable<Asset> assetsIQ = _context.Asset.Include(a => a.AssetType).Include(a => a.Condition).Include(a => a.Contact).Include(a => a.Location).Include(a => a.ServiceRecord);

            if (!String.IsNullOrEmpty(searchString))
            {
                assetsIQ = assetsIQ.Where(a => a.AssetId.Equals(searchString) ||
                                a.AssetTagNumber.Equals(searchString) ||
                                a.Brand.Contains(searchString) ||
                                a.Condition.Condition1.Contains(searchString) ||
                                a.Contact.DisplayName.Contains(searchString) ||
                                a.DeviceId.Contains(searchString) ||
                                a.Location.Location1.Contains(searchString) ||
                                a.Model.Contains(searchString) ||
                                a.SerialNumber.Contains(searchString) ||
                                a.ServiceTag.Contains(searchString) ||
                                a.WorkCenter.Contains(searchString) ||
                                a.Contact.DisplayName.Contains(searchString));
            }

            assetsIQ = sortOrder switch
            {
                "name_desc" => assetsIQ.OrderByDescending(s => s.AssetTagNumber),
                "Date" => assetsIQ.OrderBy(s => s.DateVerified),
                "date_desc" => assetsIQ.OrderByDescending(s => s.DateVerified),
                _ => assetsIQ.OrderBy(s => s.AssetTagNumber),
            };

            if (export)
            {
                var result = WriteCsvToMemory(await assetsIQ.AsNoTracking().ToListAsync());
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "export_assets_" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + ".csv" };
            }

            int pageSize = 10;
            
            return View(await PaginatedList<Asset>.CreateAsync(assetsIQ.AsNoTracking(), pageNumber ?? 1, pageSize));
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
            if (User.IsInRole(_configuration["AzureAd:Roles:ReadOnly"]))
            {
                return View("Unauthorized");
            }

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
            if (User.IsInRole(_configuration["AzureAd:Roles:ReadOnly"]))
            {
                return View("Unauthorized");
            }

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
            if (User.IsInRole(_configuration["AzureAd:Roles:ReadOnly"]))
            {
                return View("Unauthorized");
            }

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
            var serviceRecords = _context.ServiceRecord.FirstOrDefault(m => m.AssetId.Equals(id));
            if(serviceRecords != null)
            {
                _context.ServiceRecord.RemoveRange(serviceRecords);
            }

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
