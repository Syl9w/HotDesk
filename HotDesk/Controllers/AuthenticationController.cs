using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HotDesk.DataAccess;
using HotDesk.Models;
using HotDesk.Models.Authentication;
using HotDesk.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotDesk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly HotDeskDbContext _context;

        public AuthenticationController(HotDeskDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<AuthenticatedUser> Authenticate(AuthenticationData authenticationData)
        {
            if (string.IsNullOrWhiteSpace(authenticationData.Login) || string.IsNullOrWhiteSpace(authenticationData.Password))
                return Unauthorized();

            var dbUser = _context.Users.FirstOrDefault(x => x.Email == authenticationData.Login);
            if (dbUser == null || !BCrypt.Net.BCrypt.Verify(authenticationData.Password, dbUser.PasswordHash))
                return Unauthorized();

            var user = new AuthenticatedUser
            {
                Id = dbUser.UserId,
                Login = dbUser.Email,
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName
            };

            var key = Encoding.ASCII.GetBytes("MdoXyyQYGY5xICj7pWV8UFMxHBsWDAOoH9nhxQ7X26HXHWfI8s6z0JAVBMyEMw2");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            user.Token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            return new OkObjectResult(user);
        }

        [AllowAnonymous]
        [HttpPost("registration")]
        public ActionResult<User> Register(RegistrationDto registrationData)
        {
            var newUser = new User
            {
                Email = registrationData.Email,
                FirstName = registrationData.FirstName,
                LastName = registrationData.LastName,
                Role = registrationData.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registrationData.Password)
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();

            return newUser;
        }

    }
}
