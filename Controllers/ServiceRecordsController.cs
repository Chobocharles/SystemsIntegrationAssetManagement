using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Asset_Management.Models.SQL;
using Microsoft.Extensions.Configuration;

namespace Asset_Management.Controllers
{
    public class ServiceRecordsController : Controller
    {
        private readonly AssetContext _context;

        private readonly IConfiguration _configuration;

        public ServiceRecordsController(AssetContext context, IConfiguration configuration)
        {
            _context = context;

            _configuration = configuration;
        }

        // GET: ServiceRecords
        public async Task<IActionResult> Index()
        {
            ViewData["config"] = _configuration["AzureAd:Roles:ReadWrite"];

            return View(await _context.ServiceRecord.ToListAsync());
        }

        // GET: ServiceRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRecord = await _context.ServiceRecord
                .FirstOrDefaultAsync(m => m.ServiceRecordId == id);
            if (serviceRecord == null)
            {
                return NotFound();
            }

            return View(serviceRecord);
        }

        // GET: ServiceRecords/Create
        public IActionResult Create()
        {
            if (User.IsInRole(_configuration["AzureAd:Roles:ReadOnly"]))
            {
                return View("Unauthorized");
            }

            ViewData["Asset"] = new SelectList(_context.Asset, "AssetId", "AssetTagNumber");
            return View();
        }

        // POST: ServiceRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceRecordId,AssetId,Problem,PartsReplaced,DescriptionOfWork,DeviceName,ServiceDate")] ServiceRecord serviceRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviceRecord);
        }

        // GET: ServiceRecords/Edit/5
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

            var serviceRecord = await _context.ServiceRecord.FindAsync(id);
            if (serviceRecord == null)
            {
                return NotFound();
            }
            return View(serviceRecord);
        }

        // POST: ServiceRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceRecordId,AssetId,Problem,PartsReplaced,DescriptionOfWork,DeviceName,ServiceDate")] ServiceRecord serviceRecord)
        {
            if (id != serviceRecord.ServiceRecordId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRecordExists(serviceRecord.ServiceRecordId))
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
            return View(serviceRecord);
        }

        // GET: ServiceRecords/Delete/5
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

            var serviceRecord = await _context.ServiceRecord
                .FirstOrDefaultAsync(m => m.ServiceRecordId == id);
            if (serviceRecord == null)
            {
                return NotFound();
            }

            return View(serviceRecord);
        }

        // POST: ServiceRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRecord = await _context.ServiceRecord.FindAsync(id);
            _context.ServiceRecord.Remove(serviceRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceRecordExists(int id)
        {
            return _context.ServiceRecord.Any(e => e.ServiceRecordId == id);
        }
    }
}
