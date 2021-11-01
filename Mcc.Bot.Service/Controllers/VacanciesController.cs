using Mcc.Bot.Service.Data;
using Mcc.Bot.Service.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mcc.Bot.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VacanciesController : ControllerBase
    {
        ILogger<VacanciesController> _logger;
        ServiceContext _context;

        public VacanciesController(ILogger<VacanciesController> logger, ServiceContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Gets all open vacancies.
        /// </summary>
        /// <returns>Vacancies headers that represent title and the id of the vacancy.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<VacancyHeader>))]
        public async Task<IActionResult> GetAllVacanciesAsync()
        {
            var vacancies = await _context.Vacancies.Select(
                v => new VacancyHeader()
                {
                    Id = v.Id,
                    Title = v.Title
                }
            ).ToListAsync();

            return Ok(vacancies);
        }

        /// <summary>
        /// Gets full description of the vacancy by its id.
        /// </summary>
        /// <param name="id">A unique id of the vacancy.</param>
        /// <returns>A full description of the vacancy if found.</returns>
        [Route("[controller]/{id:guid}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Vacancy))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVacancyDescriptionAsync(Guid id)
        {
            var result = await _context.Vacancies.FindAsync(id);

            if (result == default)
            {
                _logger.LogDebug("Tried to find the vacncy but not found. Id: {VacancyId}", id);
                return NotFound();
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Opens a new vacancy.
        /// </summary>
        /// <param name="ownerUserId">Sample text goes here...</param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> OpenVacancyAsync(
            [FromForm] ulong ownerUserId, 
            [FromForm] string title, 
            [FromForm] string description)
        {
            var v = new Vacancy
            {
                Id = Guid.NewGuid(),
                OwnerUserId = ownerUserId,
                Title = title,
                Description = description,
                Created = DateTime.UtcNow
            };

            await _context.Vacancies.AddAsync(v);
            await _context.SaveChangesAsync();

            _logger.LogDebug("Create new vacancy. Id: {VacancyId}", v.Id);

            return Ok();
        }

        [Route("[controller]/{id:guid}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CloseVacancyAsync(Guid id)
        {
            
            var delete = await _context.Vacancies.FindAsync(id);

            if (delete == default)
            {
                _logger.LogWarning("Tried to close not existing vacancy. Id: {VacancyId}", id);
                return NotFound();
            }

            _context.Vacancies.Remove(delete);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}