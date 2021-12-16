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
    ValueTask AddVacancyAsync(Vacancy vacancy);

    /// <summary>
    /// Removes the vacancy by given id.
    /// </summary>
    /// <param name="id">
    /// An id of the removed vacancy.
    /// </param>
    ValueTask RemoveVacancyByIdAsync(Guid id);

    /// <summary>
    /// Get all short description of the open vacancies.
    /// </summary>
    /// <returns>List of short description of the vacancies.</returns>
    ValueTask<IList<VacancyShortDescription>> ListAllVacanciesHeaders();

    /// <summary>
    /// Get full description of the vacancy by given id.
    /// </summary>
    /// <param name="id">
    /// An id of the open vacancy.
    /// </param>
    /// <returns>A full description of the vacancy.</returns>
    ValueTask<Vacancy> GetVacancyByIdAsync(Guid id);
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

    /// <inheritdoc />
    public async ValueTask AddVacancyAsync(Vacancy vacancy)
    {
        await context.Vacancies.AddAsync(vacancy);
        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async ValueTask RemoveVacancyByIdAsync(Guid vacancyId)
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

    /// <inheritdoc />
    public async ValueTask<Vacancy> GetVacancyByIdAsync(Guid id)
    {
        var vacancy = await context.Vacancies.FindAsync(id);

        if (vacancy == default)
            throw new InvalidOperationException(
                $"The vacancy with given id is not found. Id: {id}"
            );

        return vacancy;
    }

    /// <inheritdoc />
    public async ValueTask<IList<VacancyShortDescription>> ListAllVacanciesHeaders()
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
