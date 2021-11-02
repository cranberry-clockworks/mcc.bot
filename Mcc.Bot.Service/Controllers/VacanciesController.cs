using Mcc.Bot.Service.Data;
using Mcc.Bot.Service.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mcc.Bot.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VacanciesController : ControllerBase
    {
        ILogger<VacanciesController> logger;
        private readonly IVacancyStorage vacancyStorage;

        public VacanciesController(
            ILogger<VacanciesController> logger, 
            IVacancyStorage vacancyStorage
        )
        {
            this.logger = logger;
            this.vacancyStorage = vacancyStorage;
        }

        /// <summary>
        /// Gets all open vacancies.
        /// </summary>
        /// <returns>Vacancies headers that represent title and the id of the vacancy.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<VacancyHeader>))]
        public async Task<IActionResult> GetAllVacanciesAsync()
        {
            var list = await vacancyStorage.ListAllVacanciesHeaders();
            return Ok(list);
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
            try
            {
                var result = await vacancyStorage.GetVacancyByIdAsync(id);
                return Ok(result);
            }
            catch (InvalidOperationException)
            {
                logger.LogDebug("Tried to find the vacncy but not found. Id: {VacancyId}", id);
                return NotFound();
            }
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

            await vacancyStorage.AddVacancyAsync(v);

            logger.LogDebug("Create new vacancy. Id: {VacancyId}", v.Id);

            return Ok();
        }

        [Route("[controller]/{id:guid}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CloseVacancyAsync(Guid id)
        {
            try
            {
                await vacancyStorage.DeleteVacancyByIdAsync(id);
                return Ok();
            } 
            catch (InvalidOperationException)
            {
                logger.LogWarning("Tried to close not existing vacancy. Id: {VacancyId}", id);
                return NotFound();
            }
        }
    }
}