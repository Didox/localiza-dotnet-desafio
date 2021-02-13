using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Authentication;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.Interfaces;
using Domain.UseCase.Builders;
using Domain.ViewModel;
using Domain.ViewModel.Jwt;
using Infrastructure.Services;
using Infrastructure.Services.Exceptions;
using Infrastructure.Services.Person;

namespace Domain.UseCase
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
            if (person.Id == 0)
            {
                if (person.Type == 0) person.Type = Convert.ToInt16(person.Role);
                var size = await personRepository.CountByDocument<IPerson>(person.Document, person.Type);
                if (size > 0) throw new EntityUniq("Documento já cadastrado"); 
                await entityRepository.Save(person);
            }
            else
            {
                var size = await personRepository.CountByIdAndDocument<IPerson>(person.Id, person.Document, person.Type);
                if (size > 0) throw new EntityUniq("Documento já cadastrado");
                await entityRepository.Update(person);
            }
        }

        public async Task SaveUser(UserSave completeUser, int id = 0)
        {
            if (id == 0)
            {
                var address = EntityBuilder.Call<Address>(completeUser);
                var userBuilder = EntityBuilder.Call<User>(completeUser);

                var size = await personRepository.CountByDocument<User>(userBuilder.CPF, Convert.ToInt16(userBuilder.Role));
                if (size > 0) throw new EntityUniq("CPF já cadastrado");

                await entityRepository.Save(address);
                userBuilder.AddressId = address.Id;
                await entityRepository.Save(userBuilder);
            }
            else
            {
                var user = await entityRepository.FindById<User>(id);
                if (user == null || user.Id == 0) throw new EntityNotFound("Usuário não encontrado");

                var address = EntityBuilder.Call<Address>(completeUser);
                var userBuilder = EntityBuilder.Call<User>(completeUser);
                userBuilder.Id = user.Id;
                userBuilder.AddressId = user.AddressId;
                address.Id = Convert.ToInt32(user.AddressId);

                var size = await personRepository.CountByIdAndDocument<User>(userBuilder.Id, userBuilder.CPF, Convert.ToInt16(userBuilder.Role));
                if (size > 0) throw new EntityUniq("CPF já cadastrado");

                await entityRepository.Update(address);
                await entityRepository.Update(user);
            }
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

        public async Task<ICollection<T>> AllByType<T>(PersonRole role)
        {
           return await personRepository.AllByType<T>(Convert.ToInt16(role));
        }

        public async Task<ICollection<User>> AllUsers()
        {
            //Exemplo usando parâmetros
            //var parameters = new List<dynamic>();
            //parameters.Add(new
            //{
            //    Key = "Nome",
            //    Value = "Danilo"
            //});
            ////var completeUsers = await personRepository.All<CompleteUser>("sp_completeUsers", parameters);

            var completeUsers = await personRepository.All<CompleteUser>("sp_completeUsers");
            var users = new List<User>();
            foreach(var completeUser in completeUsers)
            {
                var user = EntityBuilder.Call<User>(completeUser);
                user.Address = EntityBuilder.Call<Address>(completeUser);
                users.Add(user);
            }
            return users;
        }
    }
}
