using System;
using System.Collections.Generic;
using System.Net.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mcc.Bot.Service.Models;
using Mcc.Bot.Service.Data;
using System.Linq;

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
            Console.WriteLine("CTRL");
        }

        [HttpGet]
        public IEnumerable<Vacancy> Get()
        {
            return Array.Empty<Vacancy>();
        }

        /// <summary>
        /// Creates a new vacancy.
        /// </summary>
        /// <param name="authorUserId">Sample text goes here...</param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        [HttpPost]
        public void CreateVacancy([FromForm] ulong authorUserId, [FromForm] string title, [FromForm] string description)
        {
            var v = new Vacancy
            {
                Id = Guid.NewGuid(),
                OwnerUserId = authorUserId,
                Title = title,
                Description = description
            };

            _logger.LogWarning("Creating new vacancy: {Vacancy}", v);
        }
    }
}