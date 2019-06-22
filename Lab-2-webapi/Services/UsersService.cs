using Lab_2_webapi.Models;
using Lab_2_webapi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lab_2_webapi.Services
{
    public interface IUsersService
    {
        UserGetModel Authenticate(string username, string password);
        UserGetModel Register(RegisterPostModel registerInfo);
        IEnumerable<UserGetModel> GetAll();
        //User Create(UserPostModel userPostModel,User user);
        User GetCurentUser(HttpContext httpContext);
        object Upsert(int id, UserPostModel userPostModel, User addedBy);
        object Delete(int id);
      
    }

    public class UsersService : IUsersService
    {
        private TasksDbContext context;
        private readonly AppSettings appSettings;

        public UsersService(TasksDbContext context, IOptions<AppSettings> appSettings)
        {
            this.context = context;
            this.appSettings = appSettings.Value;

        }

        public UserGetModel Authenticate(string username, string password)
        {
            var user = context.Users
                .Include(u => u.UserUserRoles)
                .ThenInclude(uur => uur.UserRole)
                .SingleOrDefault(x => x.Username == username &&
                                 x.Password == ComputeSha256Hash(password));

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var result = new UserGetModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                UserRole = user.UserUserRoles.FirstOrDefault(userUserRole => userUserRole.EndTime == null && (userUserRole.StartTime.CompareTo(DateTime.Today.AddDays(1)) < 0)).UserRole.Name,
                Token = tokenHandler.WriteToken(token)
            };

            // remove password before returning

            return result;
        }

        private string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


        public UserGetModel Register(RegisterPostModel registerInfo)
        {
            User existing = context.Users.FirstOrDefault(u => u.Username == registerInfo.Username);
            if (existing != null)
            {
                return null;
            }

            User toAdd = new User
            {
                Email = registerInfo.Email,
                LastName = registerInfo.LastName,
                FirstName = registerInfo.FirstName,
                Password = ComputeSha256Hash(registerInfo.Password),
                Username = registerInfo.Username,
                UserUserRoles = new List<UserUserRole>(),
                CreatedAt = DateTime.Today
            };

            var regularRole = context.UserRoles.FirstOrDefault(ur => ur.Name == UserRoles.Regular);

            context.Users.Add(toAdd);
            context.UserUserRoles.Add(new UserUserRole
            {
                User = toAdd,
                UserRole = regularRole,
                StartTime = DateTime.Now,
                EndTime = null,
            });

            context.SaveChanges();

            return null;
        }



        public IEnumerable<UserGetModel> GetAll()
        {
            // return users without passwords
            return context.Users
                .Select(user => new UserGetModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    UserRole = user.UserUserRoles.FirstOrDefault(userUserRole => userUserRole.EndTime == null && (userUserRole.StartTime.CompareTo(DateTime.Today.AddDays(1)) < 0)).UserRole.Name,
                    Token = null
                });
        }


        //public User Create(UserPostModel userPostModel,User addedby)
        //{
        //    User toAdd = UserPostModel.ToUser(userPostModel);
        //    toAdd.Password = ComputeSha256Hash(toAdd.Password);
        //    toAdd.CreatedBy = addedby.Username;
        //    toAdd.CreatedByRole = GetCurrentUserRole(addedby).ToString();

        //    context.Users.Add(toAdd);
        //    context.SaveChanges();
        //    return toAdd;
        //}
        public UserRole GetCurrentUserRole(User user)
        {
            return user
                .UserUserRoles

                .FirstOrDefault(uur => (uur.StartTime < DateTime.Today && uur.EndTime > DateTime.Today) || (uur.StartTime < DateTime.Today && uur.EndTime == null))
                .UserRole;
        }
        public User GetCurentUser(HttpContext httpContext)
        {
            string username = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;

            return context
                .Users
                .Include(u => u.UserUserRoles)
                .FirstOrDefault(u => u.Username == username);

        }

        public object Upsert(int id, UserPostModel userPostModel, User requestedBy)
        {
            var existing = context.Users.AsNoTracking().FirstOrDefault(u => u.Id == id);
            if (existing == null)
            {
                User toAdd = new User
                {
                    Email = userPostModel.Email,
                    LastName = userPostModel.LastName,
                    FirstName = userPostModel.FirstName,
                    Password = ComputeSha256Hash(userPostModel.Password),
                    Username = userPostModel.UserName,
                    CreatedBy = requestedBy.Username,
                    CreatedByRole = requestedBy.CreatedByRole,
                    UserUserRoles = new List<UserUserRole>(),
                    CreatedAt = DateTime.Today
                };

                var regularRole = context.UserRoles.AsNoTracking().FirstOrDefault(ur => ur.Name == UserRoles.Regular);

                context.Users.Add(toAdd);
                context.UserUserRoles.Add(new UserUserRole
                {
                    User = toAdd,
                    UserRole = regularRole,
                    StartTime = DateTime.Now,
                    EndTime = null,
                });

                context.SaveChanges();
            }

            User toUpdate = UserPostModel.ToUser(userPostModel);
            toUpdate.Id = existing.Id;
            toUpdate.Password = ComputeSha256Hash(toUpdate.Password);
            toUpdate.UserUserRoles = existing.UserUserRoles;


            context.Users.Update(toUpdate);
            context.SaveChanges();
            return toUpdate;
        }

        public object Delete(int id)
        {
            var existing = context.Users.FirstOrDefault(u => u.Id == id);
            if (existing == null)
            {
                return null;
            }

            context.Users.Remove(existing);
            context.SaveChanges();

            return existing;
        }

    }
    }