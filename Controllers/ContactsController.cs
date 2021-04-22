using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Asset_Management.Models.SQL;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Http;
using Asset_Management.Models;
using Asset_Management.Models.LDAP;
using Microsoft.Extensions.Configuration;
using CsvHelper.Configuration;
using System.Globalization;
using CsvHelper;

namespace Asset_Management.Controllers
{
    public class ContactsController : Controller
    {
        private readonly AssetContext _context;

        private readonly User _user;

        private readonly IConfiguration _configuration;

        public ContactsController(AssetContext context, User user, IConfiguration configuration)
        {
            _context = context;

            _user = user;

            _configuration = configuration;
        }

        public byte[] WriteCsvToMemory(dynamic records)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.Context.RegisterClassMap<ContactMap>();
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }


        public sealed class ContactMap : ClassMap<Contact>
        {
            public ContactMap()
            {
                AutoMap(CultureInfo.InvariantCulture);
                Map(m => m.Asset).Ignore();
                Map(m => m.Picture).Ignore();
                Map(m => m.PictureContentType).Ignore();
            }
        }

        // GET: Contacts
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

            IQueryable<Contact> contactsIQ = _context.Contact;

            if (!String.IsNullOrEmpty(searchString))
            {
                contactsIQ = contactsIQ.Where(a => a.Address.Contains(searchString) ||
                                a.BusinessPhone.Contains(searchString) ||
                                a.City.Contains(searchString) ||
                                a.Company.Contains(searchString) ||
                                a.Country.Contains(searchString) ||
                                a.DisplayName.Contains(searchString) ||
                                a.EmailAddress.Contains(searchString) ||
                                a.FirstName.Contains(searchString) ||
                                a.JobTitle.Contains(searchString) ||
                                a.LastName.Contains(searchString) ||
                                a.Province.Contains(searchString) ||
                                a.State.Contains(searchString) ||
                                a.ZipCode.Contains(searchString) ||
                                a.ContactId.Equals(searchString)
                            );
            }

            switch (sortOrder)
            {
                case "name_desc":
                    contactsIQ = contactsIQ.OrderByDescending(s => s.LastName);
                    break;
                default:
                    contactsIQ = contactsIQ.OrderBy(s => s.LastName);
                    break;
            }

            if (export)
            {
                var result = WriteCsvToMemory(await contactsIQ.AsNoTracking().ToListAsync());
                var memoryStream = new MemoryStream(result);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "export_contacts_" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + ".csv" };
            }

            int pageSize = 10;

            return View(await PaginatedList<Contact>.CreateAsync(contactsIQ.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        public IActionResult Create()
        {
            if (User.IsInRole(_configuration["AzureAd:Roles:ReadOnly"]))
            {
                return View("Unauthorized");
            }

            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContactId,Company,FirstName,LastName,EmailAddress,JobTitle,BusinessPhone,Extension,HomePhone,MobilePhone,FaxNumber,Address,City,State,Province,ZipCode,Country,WebPage,Notes,Picture")] Contact contact, IFormFile img)
        {
            if (ModelState.IsValid)
            {
                if (img != null)
                {
                    contact.Picture = GetByteArrayFromImage(img);
                    contact.PictureSourceFileName = Path.GetFileName(img.FileName);
                    contact.PictureContentType = img.ContentType;
                }

                contact.DisplayName = String.Format("{0}, {1}", contact.LastName, contact.FirstName);

                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search([Bind("UserID")] ContactBindModel search)
        {
            if (ModelState.IsValid)
            {
                var userFound = _user.GetUserAttributes(search.UserID, _context);
                return Ok(userFound);
            }
            return BadRequest(search);
        }

        // GET: Contacts/Edit/5
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

            var contact = await _context.Contact.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContactId,Company,FirstName,LastName,EmailAddress,JobTitle,BusinessPhone,Extension,HomePhone,MobilePhone,FaxNumber,Address,City,State,Province,ZipCode,Country,WebPage,Notes,Picture")] Contact contact, IFormFile img)
        {
            if (id != contact.ContactId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var contactFound = await _context.Contact.FindAsync(contact.ContactId);

                if (img != null)
                {
                    contact.Picture = GetByteArrayFromImage(img);
                    contact.PictureSourceFileName = Path.GetFileName(img.FileName);
                    contact.PictureContentType = img.ContentType;
                }
                else
                {
                    if (contactFound.Picture != null)
                    {
                        contact.Picture = contactFound.Picture;
                        contact.PictureSourceFileName = contactFound.PictureSourceFileName;
                        contact.PictureContentType = contactFound.PictureContentType;
                    }
                }

                contact.DisplayName = String.Format("{0}, {1}", contact.LastName, contact.FirstName);

                try
                {
                    _context.Entry(contactFound).CurrentValues.SetValues(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.ContactId))
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
            return View(contact);
        }

        // GET: Contacts/Delete/5
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

            var contact = await _context.Contact
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contact.FindAsync(id);
            _context.Contact.Remove(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contact.Any(e => e.ContactId == id);
        }

        private byte[] GetByteArrayFromImage(IFormFile file)
        {
            using var target = new MemoryStream();
            file.CopyTo(target);
            return target.ToArray();
        }
    }
}
