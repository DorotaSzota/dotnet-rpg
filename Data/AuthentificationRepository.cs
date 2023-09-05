using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_rpg.Data
{
    public class AuthentificationRepository : IAuthentificationRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthentificationRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Username.ToLower().Equals(username.ToLower()));

            if(user is null)
            {
                response.Success = false;
                response.Message = "User not found";
            }
            else if(!VerifyPasswordHash(password,user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message= "Wrong pswd";
            }
            else
            {
                response.Data = CreateToken(user);
            }
            return response;

        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
        var response = new ServiceResponse<int>();
        if(await UserExists(user.Username))
        {
            response.Success = false;
            response.Message  = "User already exists.";
            return response;
        }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            response.Data = user.Id;
            return response;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(u=>u.Username.ToLower() == username.ToLower()))
            {return true;}
            return false;

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                passwordSalt = hmac.Key;
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash); // do i have any idea what i am doing??? O_o

            }

        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)   
            };
            /*
            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
            if(appSettingsToken is null)
                throw new Exception("AppSettings Token is null");
                */

           SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

            SigningCredentials credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}