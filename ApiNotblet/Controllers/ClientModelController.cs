using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notblet.Models;
using NotbletApi;

namespace NotbletApi.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientModelController : ControllerBase
    {
        private readonly dbaContext _context;

        public ClientModelController(dbaContext context)
        {
            _context = context;
        }

        // GET: api/ClientModel
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientModel>>> Getclients()
        {
            return await _context.clients.ToListAsync();
        }

        // GET: api/ClientModel/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientModel>> GetClientModel(int id)
        {
            var clientModel = await _context.clients.FindAsync(id);

            if (clientModel == null)
            {
                return NotFound();
            }

            return clientModel;
        }

        // PUT: api/ClientModel/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClientModel(int id, ClientModel clientModel)
        {
            if (id != clientModel.id)
            {
                return BadRequest();
            }

            _context.Entry(clientModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientModelExists(id))
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

        // POST: api/ClientModel
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ClientModel>> PostClientModel(ClientModel clientModel)
        {
            _context.clients.Add(clientModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClientModel", new { id = clientModel.id }, clientModel);
        }

        // DELETE: api/ClientModel/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientModel(int id)
        {
            var clientModel = await _context.clients.FindAsync(id);
            if (clientModel == null)
            {
                return NotFound();
            }

            _context.clients.Remove(clientModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientModelExists(int id)
        {
            return _context.clients.Any(e => e.id == id);
        }
    }
}
