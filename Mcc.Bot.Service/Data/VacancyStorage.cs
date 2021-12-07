using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mcc.Bot.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcc.Bot.Service.Data;

public interface IVacancyStorage
{
    Task AddVacancyAsync(Vacancy v);
    Task DeleteVacancyByIdAsync(Guid vacancyId);
    Task<IList<VacancyHeader>> ListAllVacanciesHeaders();
    Task<Vacancy> GetVacancyByIdAsync(Guid vacancyId);
}

public class VacancyStorage : IVacancyStorage
{
    private readonly ServiceContext context;

    public VacancyStorage(ServiceContext context)
    {
        this.context = context;
    }

    public async Task AddVacancyAsync(Vacancy vacancy)
    {
        await context.Vacancies.AddAsync(vacancy);
        await context.SaveChangesAsync();
    }

    public async Task DeleteVacancyByIdAsync(Guid vacancyId)
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

    public async Task<IList<VacancyHeader>> ListAllVacanciesHeaders()
    {
        return await context.Vacancies.Select(
            v => new VacancyHeader()
            {
                Id = v.Id,
                Title = v.Title
            }
        ).ToListAsync();
    }
}
