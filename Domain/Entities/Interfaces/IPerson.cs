using System;
using Domain.Entities.Enums;

namespace Domain.Entities.Interfaces
{
    public interface IPerson
    {
        int Id {get; set;}
        string Name {get; set;}
        string Password {get; set;}
        string Document {get; set; }
        int Type {get; set; }
        PersonRole Role {get;}
    }
}
