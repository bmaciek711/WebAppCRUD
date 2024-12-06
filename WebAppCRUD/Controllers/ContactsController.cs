using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppCRUD.Data;
using WebAppCRUD.Models;

namespace WebAppCRUD.Controllers
{
    public class ContactsController(AppDbContext context) : Controller
    {
        public async Task<IActionResult> Index(string? searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var contacts = await context.Contacts.ToListAsync();
            contacts = SearchContacts(searchString, contacts);
            return View(contacts);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contact contact)
        {
            if (!ModelState.IsValid) return View(contact);
            context.Add(contact);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CreateBusiness()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBusiness(BusinessContact businessContact)
        {
            if (!ModelState.IsValid) return View(businessContact);
            context.Add(businessContact);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var contact = await context.Contacts.FindAsync(id);
            return contact switch
            {
                null => NotFound(),
                BusinessContact businessContact => View("EditBusiness", businessContact),
                _ => View(contact)
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contact contact)
        {
            if (id != contact.Id) 
                return NotFound();

            if (!ModelState.IsValid) 
                return View(contact);

            try
            {
                await UpdateContact(contact);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(contact.Id)) 
                    return NotFound();

                throw;
            }
        }

        private async Task UpdateContact(Contact contact)
        {
            var entity = contact is BusinessContact businessContact ? (Contact)businessContact : contact;
            context.Update(entity);
            await context.SaveChangesAsync();
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contact = await context.Contacts.FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null) return NotFound();

            return View(contact);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await context.Contacts.FindAsync(id);
            if (contact != null) context.Contacts.Remove(contact);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return context.Contacts.Any(e => e.Id == id);
        }

        private static List<Contact> SearchContacts(string? searchString, List<Contact> contacts)
        {
            if (string.IsNullOrEmpty(searchString)) 
                return contacts;

            return contacts.Where(c => 
                c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                c.PhoneNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                (c is BusinessContact bc && 
                 (bc.CompanyName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                  bc.Position.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
            ).ToList();
        }


        public IActionResult Privacy()
        {
            return View();
        }
    }
}
