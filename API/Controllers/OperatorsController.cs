using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Services.Person;
using Infrastructure.Services;
using Domain.UseCase;
using Domain.ViewModel;
using Domain.UseCase.Builders;
using Infrastructure.Services.Exceptions;
using Domain.Entities.Enums;

namespace api.Controllers
{
    [ApiController]
    public class OperatorsController : ControllerBase
    {
        private readonly PersonService _personService;
        private readonly ILogger<OperatorsController> _logger;

        public OperatorsController(ILogger<OperatorsController> logger)
        {
            _logger = logger;
            _personService = new PersonService(new PersonRepository(), new EntityRepository());
        }

        [HttpGet]
        [Route("/operators")]
        [Route("/operadores")]
        [Authorize(Roles = "User, Operator")]
        public async Task<ICollection<Operator>> Index()
        {
            return await _personService.AllByType<Operator>(PersonRole.Operator);
        }

        [HttpPost]
        [Route("/operators")]
        [Route("/operadores")]
        [Authorize(Roles = "Operator")]
        public async Task<IActionResult> Create([FromBody] OperatorSave op)
        {
            var oper = EntityBuilder.Call<Operator>(op);
            try
            {
                await _personService.Save(oper);
                return StatusCode(201);
            }
            catch(EntityUniq err)
            {
                return StatusCode(401, new {
                    Message = err.Message
                });
            }
        }

        [HttpPut]
        [Route("/operators/{id}")]
        [Route("/operadores/{id}")]
        [Authorize(Roles = "Operator")]
        public async Task<IActionResult> Update(int id, [FromBody] OperatorSave op)
        {
            var oper = EntityBuilder.Call<Operator>(op);
            oper.Id = id;
            try
            {
                await _personService.Save(oper);
                return StatusCode(204);
            }
            catch(EntityUniq err)
            {
                return StatusCode(401, new {
                    Message = err.Message
                });
            }
        }

        [HttpDelete]
        [Route("/operators/{id}")]
        [Route("/operadores/{id}")]
        [Authorize(Roles = "Operator")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _personService.Delete(id);
                return StatusCode(204);
            }
            catch(EntityEmptyId err)
            {
                return StatusCode(401, new {
                    Message = err.Message
                });
            }
            catch(EntityNotFound err)
            {
                return StatusCode(404, new {
                    Message = err.Message
                });
            }
        }
    }
}
