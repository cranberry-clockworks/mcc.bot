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

namespace Mcc.Bot.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class VacanciesController : ControllerBase
{
    ILogger<VacanciesController> logger;
    private readonly IVacancyStorage vacancyStorage;
    private readonly ITokenStorage permissionStorage;

    public VacanciesController(
        ILogger<VacanciesController> logger,
        IVacancyStorage vacancyStorage,
        ITokenStorage permissionStorage
    )
    {
        this.logger = logger;
        this.vacancyStorage = vacancyStorage;
        this.permissionStorage = permissionStorage;
    }

    /// <summary>
    /// Gets all open vacancies.
    /// </summary>
    /// <returns>Vacancies headers that represent title and the id of the vacancy.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VacancyHeader>>> GetAllVacanciesAsync()
    {
        var list = await vacancyStorage.ListAllVacanciesHeaders();
        return Ok(list);
    }

    /// <summary>
    /// Gets full description of the vacancy by its id.
    /// </summary>
    /// <param name="id">A unique id of the vacancy.</param>
    /// <returns>A full description of the vacancy if found.</returns>
    [HttpGet("{id:guid}", Name = nameof(GetVacancyDescriptionAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Vacancy>> GetVacancyDescriptionAsync(Guid id)
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

    [HttpPost]
    [Authorize(Policy = Policices.CanManageVacanciesPolicy)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Guid>> OpenVacancyAsync(
        [FromForm] string title,
        [FromForm] string description)
    {
        var userId = HttpContext.User.Identity?.GetUserId() ?? null;
        if (userId is null)
            return BadRequest();

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

    [HttpDelete("/{id:guid}")]
    [Authorize(Policy = Policices.CanManageVacanciesPolicy)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseVacancyAsync(Guid id)
    {
        try
        {
            await vacancyStorage.DeleteVacancyByIdAsync(id);

            logger.LogDebug("Closed vacancy. Id: {VacancyId}", id);

            return Ok();
        }
        catch (InvalidOperationException)
        {
            logger.LogWarning("Tried to close not existing vacancy. Id: {VacancyId}", id);
            return NotFound();
        }
    }
}