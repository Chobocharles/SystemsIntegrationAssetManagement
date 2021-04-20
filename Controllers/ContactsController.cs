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

        // GET: Contacts
        public async Task<IActionResult> Index()
        {
            ViewData["config"] = _configuration["AzureAd:Roles:ReadWrite"];

            return View(await _context.Contact.ToListAsync());
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
