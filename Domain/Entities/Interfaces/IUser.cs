using System;

namespace Domain.Entities.Interfaces
{
    public interface IUser
    {
        string CPF { get; }
        DateTime Birthday { get; set;}
        int? AddressId {get; set;}
    }
}
