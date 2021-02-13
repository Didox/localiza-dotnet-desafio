using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.ViewModel;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigureController : ControllerBase
    {
        private readonly ILogger<ConfigureController> _logger;

        public ConfigureController(ILogger<ConfigureController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("/start")]
        [Route("/comecar")]
        [AllowAnonymous]
        public string InicialConfig()
        {
            
            Task.Run(async () =>
            {
                var sql = new SqlDriver();
                //await sql.CreateTable<Person>();
                //await sql.CreateTable<Address>();
                //await sql.CreateTable<Model>();
                //await sql.CreateTable<Brand>();
                //await sql.CreateTable<Category>();
                //await sql.CreateTable<Vehicle>();
                //await sql.CreateTable<Checklist>();
                //await sql.CreateTable<Schedule>();
            });


            return "Sistema configurado";
        }
    }
}
