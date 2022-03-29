using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mcc.Bot.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcc.Bot.Service.Data;

/// <summary>
/// A storage that holds open vacancies.
/// </summary>
public interface IVacancyStorage
{
    /// <summary>
    /// Adds new vacancy to the storage.
    /// </summary>
    /// <param name="vacancy">
    /// A new vacancy to store.
    /// </param>
    Task AddVacancyAsync(Vacancy vacancy);

    /// <summary>
    /// Removes the vacancy by given id.
    /// </summary>
    /// <param name="id">
    /// An id of the removed vacancy.
    /// </param>
    Task RemoveVacancyByIdAsync(Guid id);

    /// <summary>
    /// Get all short description of the open vacancies.
    /// </summary>
    /// <returns>List of short description of the vacancies.</returns>
    Task<IList<VacancyShortDescription>> ListAllVacanciesHeadersAsync();

    /// <summary>
    /// Get full description of the vacancy by given id.
    /// </summary>
    /// <param name="id">
    /// An id of the open vacancy.
    /// </param>
    /// <returns>
    /// A full description of the vacancy.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Raised when vacancy not found by the given id.
    /// </exception>
    Task<Vacancy> GetVacancyByIdAsync(Guid id);
}

/// <summary>
/// Implementation of the <see cref="IVacancyStorage"/>.
/// </summary>
internal class VacancyStorage : IVacancyStorage
{
    private readonly ServiceContext context;

    /// <summary>
    /// Creates vacancy storage.
    /// </summary>
    public VacancyStorage(ServiceContext context)
    {
        this.context = context;
    }

    public async Task AddVacancyAsync(Vacancy vacancy)
    {
        await context.Vacancies.AddAsync(vacancy);
        await context.SaveChangesAsync();
    }

    public async Task RemoveVacancyByIdAsync(Guid vacancyId)
    {
        var delete = await context.Vacancies.FindAsync(vacancyId);
        if (delete == default)
        {
            throw new InvalidOperationException(
                $"The vacancy with given id is not found. Id: {vacancyId}"
            );
        }

        context.Vacancies.Remove(delete);
        await context.SaveChangesAsync();
    }

    public async Task<Vacancy> GetVacancyByIdAsync(Guid id)
    {
        var vacancy = await context.Vacancies.FindAsync(id);

        if (vacancy == default)
            throw new InvalidOperationException(
                $"The vacancy with given id is not found. Id: {id}"
            );

        return vacancy;
    }

    public async Task<IList<VacancyShortDescription>> ListAllVacanciesHeadersAsync()
    {
        return await context.Vacancies.Select(
            v => new VacancyShortDescription()
            {
                Id = v.Id,
                Title = v.Title
            }
        ).ToListAsync();
    }
}
