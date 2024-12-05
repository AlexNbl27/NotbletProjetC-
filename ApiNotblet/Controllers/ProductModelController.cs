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
    [Route("api/products")]
    [ApiController]
    public class ProductModelController : ControllerBase
    {
        private readonly dbaContext _context;

        public ProductModelController(dbaContext context)
        {
            _context = context;
        }

        // GET: api/ProductModel
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> Getproducts()
        {
            return await _context.products
                                 .Include(p => p.category)
                                 .ToListAsync();
        }


        // GET: api/ProductModel/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProductModel(int id)
        {
            var productModel = await _context.products
                                             .Include(p => p.category)
                                             .FirstOrDefaultAsync(p => p.id == id);

            if (productModel == null)
            {
                return NotFound();
            }

            return productModel;
        }

        // PUT: api/ProductModel/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductModel(int id, ProductModel productModel)
        {
            if (id != productModel.id)
            {
                return BadRequest("ID mismatch");
            }

            // Vérifier si la nouvelle catégorie existe
            var category = await _context.categories.FindAsync(productModel.category_id);
            if (category == null)
            {
                return NotFound("Category not found");
            }
            productModel.category = category;


            _context.Entry(productModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductModelExists(id))
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


        // POST: api/ProductModel
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductModel>> PostProductModel(ProductModel productModel)
        {
            // Vérifier si la catégorie existe
            var category = await _context.categories.FindAsync(productModel.category_id);
            if (category == null)
            {
                return NotFound("Category not found");
            }
            productModel.category = category;

            _context.products.Add(productModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductModel", new { id = productModel.id }, productModel);
        }


        // DELETE: api/ProductModel/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductModel(int id)
        {
            var productModel = await _context.products.FindAsync(id);
            if (productModel == null)
            {
                return NotFound();
            }

            _context.products.Remove(productModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductModelExists(int id)
        {
            return _context.products.Any(e => e.id == id);
        }
    }
}
