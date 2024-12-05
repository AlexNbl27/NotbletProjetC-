using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notblet.Models;

namespace NotbletApi.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderModelController : ControllerBase
    {
        private readonly dbaContext _context;

        public OrderModelController(dbaContext context)
        {
            _context = context;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderModel>>> GetOrders()
        {
            // Inclure les relations client et produit
            return await _context.orders
                .Include(o => o.client)
                .Include(o => o.product)
                    .ThenInclude(p => p.category)
                .ToListAsync();
        }

        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderModel>> GetOrderModel(int id)
        {
            var orderModel = await _context.orders
                .Include(o => o.client)
                .Include(o => o.product)
                    .ThenInclude(p => p.category)
                .FirstOrDefaultAsync(o => o.id == id);

            if (orderModel == null)
            {
                return NotFound();
            }

            return orderModel;
        }

        // PUT: api/orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderModel(int id, OrderModel orderModel)
        {
            if (id != orderModel.id)
            {
                return BadRequest("ID mismatch");
            }

            // Vérification si le client existe
            var client = await _context.clients.FindAsync(orderModel.client_id);
            if (client == null)
            {
                return NotFound("Client not found");
            }
            orderModel.client = client;

            // Vérification si le produit existe
            var product = await _context.products.FindAsync(orderModel.product_id);
            if (product == null)
            {
                return NotFound("Product not found");
            }
            orderModel.product = product;

            _context.Entry(orderModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderModelExists(id))
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

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<OrderModel>> PostOrderModel(OrderModel orderModel)
        {
            // Vérification si le client existe
            var client = await _context.clients.FindAsync(orderModel.client_id);
            if (client == null)
            {
                return NotFound("Client not found");
            }
            orderModel.client = client;

            // Vérification si le produit existe
            var product = await _context.products.FindAsync(orderModel.product_id);
            if (product == null)
            {
                return NotFound("Product not found");
            }
            orderModel.product = product;

            _context.orders.Add(orderModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderModel", new { id = orderModel.id }, orderModel);
        }

        // DELETE: api/orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderModel(int id)
        {
            var orderModel = await _context.orders.FindAsync(id);
            if (orderModel == null)
            {
                return NotFound();
            }

            _context.orders.Remove(orderModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderModelExists(int id)
        {
            return _context.orders.Any(e => e.id == id);
        }
    }
}
