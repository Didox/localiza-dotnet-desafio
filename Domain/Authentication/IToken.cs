using System;
using Domain.Entities;
using Domain.Entities.Interfaces;

namespace Domain.Authentication
{
	public interface IToken
	{
		string GerarToken(IPerson person);
	}
}