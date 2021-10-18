using System;
using Microsoft.EntityFrameworkCore;

namespace Mcc.Bot.Service.Models
{
    public class Vacancy
    {
        public Guid Id { get; init; }
        public ulong OwnerUserId { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
    }
}
