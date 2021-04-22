using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Asset_Management.Models.SQL;
using Microsoft.Extensions.Configuration;
using Asset_Management.Models;
using CsvHelper;
using System.IO;
using System.Globalization;
using CsvHelper.Configuration;

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

        public byte[] WriteCsvToMemory(dynamic records)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.Context.RegisterClassMap<ServiceRecordMap>();
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }


        public sealed class ServiceRecordMap : ClassMap<ServiceRecord>
        {
            public ServiceRecordMap()
            {
                AutoMap(CultureInfo.InvariantCulture);
                Map(m => m.Asset).Ignore();
                Map(m => m.Asset.Picture).Ignore();
                Map(m => m.Asset.PictureContentType).Ignore();
                Map(m => m.Asset.Contact.Picture).Ignore();
                Map(m => m.Asset.Contact.PictureContentType).Ignore();
            }
        }


        // GET: ServiceRecords
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

            ViewData["Export"] = export;

            IQueryable<ServiceRecord> serviceRecordIQ = _context.ServiceRecord;

            if (!String.IsNullOrEmpty(searchString))
            {
                serviceRecordIQ = serviceRecordIQ.Where(a => a.AssetId.Equals(searchString) ||
                                a.DeviceName.Contains(searchString) ||
                                a.PartsReplaced.Contains(searchString) ||
                                a.Problem.Equals(searchString) ||
                                a.ServiceRecordId.Equals(searchString)
                            );
            }

            serviceRecordIQ = sortOrder switch
            {
                "name_desc" => serviceRecordIQ.OrderByDescending(s => s.AssetId),
                _ => serviceRecordIQ.OrderBy(s => s.AssetId),
            };

            if (export)
            {
                var result = WriteCsvToMemory(await serviceRecordIQ.AsNoTracking().ToListAsync());
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "export_service_records_" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + ".csv" };
            }

            int pageSize = 10;

            return View(await PaginatedList<ServiceRecord>.CreateAsync(serviceRecordIQ.AsNoTracking(), pageNumber ?? 1, pageSize));
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
