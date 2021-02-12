using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Authentication;
using Domain.Entities;
using Domain.ViewModel;
using Domain.ViewModel.Jwt;
using Infrastructure.Services;
using Infrastructure.Services.Person;

namespace Domain.UseCase.PersonServices
{
    public class PersonService
    {
        public PersonService(IPersonRepository personRepository, IEntityRepository entityRepository)
        {
            this.personRepository = personRepository;
            this.entityRepository = entityRepository;
        }

        private readonly IPersonRepository personRepository;
        private readonly IEntityRepository entityRepository;

        public async Task Save(IPerson person)
        {
            if (person.Id > 0)
            {
                var size = await personRepository.CountByIdAndDocument<IPerson>(person.Id, person.Document, person.Type);
                if (size > 0) throw new EntityUniq("Documento já cadastrado");
            }
            else
            {
                if(person.Type == 0) person.Type = Convert.ToInt16(person.Role);
                var size = await personRepository.CountByDocument<IPerson>(person.Document, person.Type);
                if (size > 0) throw new EntityUniq("Documento já cadastrado");
            }
            
            await entityRepository.Save(person);
        }

        public async Task Delete(int id)
        {
            await entityRepository.Delete<IPerson>(id);
        }

        public async Task<PersonJwt> Login(PersonLogin personLogin, IToken token)
        {
            IPerson loggedPerson;
            if (personLogin.Document.Length >= 11)
                loggedPerson = await personRepository.FindByDocumentAndPassword<User>(personLogin.Document, personLogin.Password, Convert.ToInt16(PersonRole.User));
            else loggedPerson = await personRepository.FindByDocumentAndPassword<Operator>(personLogin.Document, personLogin.Password, Convert.ToInt16(PersonRole.Operator));

            if (loggedPerson == null) throw new EntityNotFound("Documento e senha inválidos");
            return new PersonJwt()
            {
                Id = loggedPerson.Id,
                Name = loggedPerson.Name,
                Document = loggedPerson.Document,
                Role = loggedPerson.Role.ToString(),
                Token = token.GerarToken(loggedPerson)
            };
        }

        public async Task<OperatorJwt> Login(OperatorLogin userLogin, IToken token)
        {
            IPerson loggedPerson = await personRepository.FindByDocumentAndPassword<Operator>(userLogin.Registration, userLogin.Password, Convert.ToInt16(PersonRole.Operator));
            if (loggedPerson == null) throw new EntityNotFound("Documento e senha inválidos");
            return new OperatorJwt()
            {
                Id = loggedPerson.Id,
                Name = loggedPerson.Name,
                Registration = loggedPerson.Document,
                Role = loggedPerson.Role.ToString(),
                Token = token.GerarToken(loggedPerson)
            };
        }

        public async Task<UserJwt> Login(UserLogin userLogin, IToken token)
        {
            IPerson loggedPerson = await personRepository.FindByDocumentAndPassword<User>(userLogin.CPF, userLogin.Password, Convert.ToInt16(PersonRole.User));
            if (loggedPerson == null) throw new EntityNotFound("Documento e senha inválidos");
            return new UserJwt()
            {
                Id = loggedPerson.Id,
                Name = loggedPerson.Name,
                CPF = loggedPerson.Document,
                Role = loggedPerson.Role.ToString(),
                Token = token.GerarToken(loggedPerson)
            };
        }

        public async Task<ICollection<T>> All<T>(PersonRole role)
        {
           return await personRepository.All<T>(Convert.ToInt16(role));
        }
    }
}