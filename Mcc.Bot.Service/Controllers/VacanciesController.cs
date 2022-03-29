using Mcc.Bot.Service.Security;
using Mcc.Bot.Service.Data;
using Mcc.Bot.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Mcc.Bot.Service.Controllers;


/// <summary>
/// Controller to manage vacancies.
/// </summary>
[ApiController]
[Route("[controller]")]
public class VacanciesController : ControllerBase
{
    private readonly ILogger<VacanciesController> logger;
    private readonly IVacancyStorage vacancyStorage;
    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// Creates instance of the controller.
    /// </summary>
    public VacanciesController(
        ILogger<VacanciesController> logger,
        IVacancyStorage vacancyStorage,
        IHttpContextAccessor httpContextAccessor
    )
    {
        this.logger = logger;
        this.vacancyStorage = vacancyStorage;
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets short descriptions of all opened vacancies.
    /// </summary>
    /// <returns>
    /// A list of vacancies titles with ids to get detailed description.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VacancyShortDescription>>> GetAllVacanciesAsync()
    {
        var list = await vacancyStorage.ListAllVacanciesHeadersAsync();
        return Ok(list);
    }

    /// <summary>
    /// Gets detailed description about specific vacancy.
    /// </summary>
    /// <param name="id">
    /// A unique id of the vacancy.
    /// </param>
    /// <returns>
    /// A full description of the vacancy if found.
    /// </returns>
    [HttpGet("{id:guid}", Name = nameof(GetVacancyDescriptionAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Vacancy>> GetVacancyDescriptionAsync([FromRoute] Guid id)
    {
        try
        {
            var result = await vacancyStorage.GetVacancyByIdAsync(id);
            return Ok(result);
        }
        catch (InvalidOperationException)
        {
            logger.LogDebug("Tried to find the vacancy but not found. Id: {VacancyId}", id);
            return NotFound();
        }
    }

    /// <summary>
    /// Opens a new vacancy.
    /// </summary>
    /// <param name="title">
    /// A title or short description of the vacancy. Should be non empty.
    /// </param>
    /// <param name="description">
    /// A detailed description of the vacancy.
    /// </param>
    /// <returns>
    /// Created vacancy instance.
    /// </returns>
    [HttpPost]
    [Authorize(Policy = Policices.CanManageVacanciesPolicy)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Vacancy>> OpenVacancyAsync(
        [Required][FromForm] string title,
        [Required][FromForm] string description)
    {
        var userId = httpContextAccessor.HttpContext?.User.Identity?.GetUserId();
        if (userId == null)
            return BadRequest();

        if (string.IsNullOrEmpty(title))
            return BadRequest(title);

        var v = new Vacancy
        {
            Id = Guid.NewGuid(),
            OwnerUserId = userId.Value,
            Title = title,
            Description = description,
            Created = DateTime.UtcNow
        };

        await vacancyStorage.AddVacancyAsync(v);

        logger.LogDebug("Create new vacancy. Id: {VacancyId}", v.Id);

        return CreatedAtRoute(nameof(GetVacancyDescriptionAsync), new { id = v.Id }, v);
    }

    /// <summary>
    /// Closes the opened vacancy.
    /// </summary>
    /// <param name="id">
    /// An unique id of the vacancy to close.
    /// </param>
    /// <returns>
    /// The id of the deleted (or not found) vacancy.
    /// </returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policices.CanManageVacanciesPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> CloseVacancyAsync([FromRoute] Guid id)
    {
        try
        {
            await vacancyStorage.RemoveVacancyByIdAsync(id);

            logger.LogDebug("Closed vacancy. Id: {VacancyId}", id);

            return Ok(id);
        }
        catch (InvalidOperationException)
        {
            logger.LogWarning("Tried to close not existing vacancy. Id: {VacancyId}", id);
            return NotFound(id);
        }
    }
}