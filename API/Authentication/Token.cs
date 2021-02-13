using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;
using System;
using Microsoft.IdentityModel.Tokens;
using Domain.Authentication;
using Domain.Entities.Interfaces;

namespace API.Authentication
{
	public class Token : IToken
	{
		public string GerarToken(IPerson person)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			JToken jAppSettings = JToken.Parse(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "appsettings.json")));
			var key = Encoding.ASCII.GetBytes(jAppSettings["JwtToken"].ToString());
			var expirationTime = Convert.ToInt32(jAppSettings["ExpirationTime"]);
            var tokenDescriptor = new SecurityTokenDescriptor()
			{
				Subject = new ClaimsIdentity(new Claim[]{
						new Claim(ClaimTypes.Name, person.Document),
						new Claim(ClaimTypes.Role, person.Role.ToString()),
					}),
				Expires = DateTime.UtcNow.AddHours(expirationTime),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
