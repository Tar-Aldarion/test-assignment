using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ContactContext _context;

        public ContactController(ContactContext context)
        {
            _context = context;
        }

        // ---

        // GET: api/Contact
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContactsList()
        {
            // Return all contacts

            return await _context.ContactInfo.ToListAsync();
        }


        // GET: api/Contact/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(int id)
        {
            // Search contacts by CID

            var contact = await _context.ContactInfo.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }


        // GET: api/Contact/Info?FirstName=Mia
        // GET: api/Contact/Info?FirstName=Mia&LastName=Paltrow
        // ...
        [HttpGet("Info")]
        public async Task<ActionResult<Contact>> GetContact(/*int? CID, */string FirstName = "", string LastName = "", string Address = "", string Phone = "")
        {
            // Search based on any property combination, except CID

            var contact = await _context.ContactInfo.SingleOrDefaultAsync(x => 
                (string.IsNullOrEmpty(FirstName) ? true : (x.FirstName == FirstName)) &&
                (string.IsNullOrEmpty(LastName) ? true : (x.LastName == LastName)) &&
                (string.IsNullOrEmpty(Address) ? true : (x.Address == Address)) &&
                (string.IsNullOrEmpty(Phone) ? true : (x.Phone == Phone))
            );

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }


        // GET: api/Contact/Search?Query=...
        // ...
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContact(string Query = "")
        {
            // Search based on any property combination, except CID

            if (!String.IsNullOrEmpty(Query))
            {
                var contacts = from c in _context.ContactInfo select c;         // LINQ query to be run against DB

                contacts = contacts.Where(x =>
                    (x.FirstName.Contains(Query)) ||
                    (x.LastName.Contains(Query)) ||
                    (x.Address.Contains(Query)) ||
                    (x.Phone.Contains(Query))
                );

                if (contacts.Count() == 0)
                {
                    return NotFound();
                }
                else 
                {
                    return (await contacts.ToListAsync());
                }
            }
            else 
            {
                return NotFound();
            }
        }


        // PUT: api/Contact/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(int id, Contact contact)
        {
            // Update an existing contact

            if (id != contact.CID)
            {
                return BadRequest();
            }

            _context.Entry(contact).State = EntityState.Modified;           // Contact entry is marked for update

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Contact
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Contact>> PostContact(Contact contact)
        {
            string reformattedPhone = ReformatPhoneEntry(contact.Phone);

            // Create a new Contact if phone entry does not exist yet

            if (PhoneExists(reformattedPhone))
            {
                return BadRequest("Contact was not created becase the phone already exists!");
            }
            else 
            {
                contact.Phone = reformattedPhone;

                _context.ContactInfo.Add(contact);
                await _context.SaveChangesAsync();

                //return CreatedAtAction("GetContact", new { id = contact.CID }, contact);
                return Created("GetContact", contact);
            }
        }


        // DELETE: api/Contact/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Contact>> DeleteContact(int id)
        {
            var contact = await _context.ContactInfo.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.ContactInfo.Remove(contact);
            await _context.SaveChangesAsync();

            return contact;
        }

        // ---

        private bool ContactExists(int id)
        {
            return _context.ContactInfo.Any(e => e.CID == id);
        }

        private bool PhoneExists(string phone)
        {
            return _context.ContactInfo.Any(e => e.Phone == phone);
        }

        /// <summary>
        /// Reformat phone entry string by removing all non-numeric characters from it
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        private string ReformatPhoneEntry(string phone)
        {
            return Regex.Replace(phone, "[^.0-9]", "");
        }
    }
}
