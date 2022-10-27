using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoAPI.Models;

namespace ToDoAPI.Controllers
{
    [EnableCors]
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoesController : ControllerBase
    {
        private readonly ToDoContext _context;

        public ToDoesController(ToDoContext context)
        {
            _context = context;
        }

        // GET: api/ToDoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDo>>> GetToDos()
        {
            if (_context.ToDos == null)
            {
                return NotFound();
            }

            //Step 07 - Modify the GET functionality to include Categories
            var toDos = await _context.ToDos.Include("Category").Select(x => new ToDo()
            {
                //Assign each resource in our data set to a new Resource object for this application.
                Name = x.Name,
                Done = x.Done,
                CategoryId = x.CategoryId,
                Category = x.Category != null ? new Category()
                {
                    CategoryId = x.Category.CategoryId,
                    CatName = x.Category.CatName,
                    CatDesc = x.Category.CatDesc
                } : null
            }).ToListAsync();

            return Ok(toDos);
        }

        // GET: api/ToDoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDo>> GetToDo(int id)
        {

            if (_context.ToDos == null)
            {
                return NotFound();
            }
            //Step 08 - Modify the code below to include Categories
            var toDo = await _context.ToDos.Where(x => x.ToDoId == id).Select(x => new ToDo()
            {
                //Assign each resource in our data set to a new Resource object for this application
                Name = x.Name,
                Done = x.Done,
                CategoryId = x.CategoryId,
                Category = x.Category != null ? new Category()
                {
                    CategoryId = x.Category.CategoryId,
                    CatName = x.Category.CatName,
                    CatDesc = x.Category.CatDesc
                } : null
            }).FirstOrDefaultAsync();

            if (toDo == null)
            {
                return NotFound();
            }

            return Ok(toDo);
        }

        // PUT: api/ToDoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToDo(int id, ToDo toDo)
        {
            if (id != toDo.ToDoId)
            {
                return BadRequest();
            }

            _context.Entry(toDo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoExists(id))
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

        // POST: api/ToDoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ToDo>> PostToDo(ToDo toDo)
        {
            if (_context.ToDos == null)
            {
                return Problem("Entity set 'ResourcesContext.Resources' is null.");
            }
            //Step 09 - Modify the code below to manage how a Resource is posted
            ToDo newToDo = new ToDo()
            {
                Name = toDo.Name,
                Done = toDo.Done,
                Category = toDo.Category
            };

            _context.ToDos.Add(newToDo);
            await _context.SaveChangesAsync();

            return Ok(newToDo);
        }

        // DELETE: api/ToDoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDo(int id)
        {
            var toDo = await _context.ToDos.FindAsync(id);
            if (toDo == null)
            {
                return NotFound();
            }

            _context.ToDos.Remove(toDo);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        private bool ToDoExists(int id)
        {
            return _context.ToDos.Any(e => e.ToDoId == id);
        }
    }
}
