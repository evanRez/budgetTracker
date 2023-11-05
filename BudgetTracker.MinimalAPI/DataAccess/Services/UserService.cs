using BudgetTracker.MinimalAPI.DataAccess.Interfaces;
using ClassLib.Models.Users;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BudgetTracker.MinimalAPI.DataAccess.Services
{
    public class UserService : IUserService
    {
        private readonly BudgetTrackerDb _db;

        public UserService(BudgetTrackerDb db)
        {
            _db = db;
        }

        public async Task<UserDTO?> GetUser(string auth0Id)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Id == auth0Id) ?? null;
        }

        public async Task<UserDTO> FindOrCreateUser(ClaimsPrincipal user)
        {
            var userId = user.Claims.SingleOrDefault(x => x.Type == "auth0_user_id")?.Value;
            var userEmail = user.Claims.SingleOrDefault(x => x.Type == "auth0_email")?.Value;
            

            if (userId == null)
            {
                throw new Exception("Could not find a user with the provided Claim Principal Auth0 Id");
            }
            
            var appUser = await GetUser(userId);
            if (appUser == null)
            {
               try
               {
                    appUser = new UserDTO
                    {
                        //Auth0UserId = userId,
                        Id = userId,
                        UpdatedDate = DateTime.UtcNow,
                        Email = userEmail
                    };

                    await _db.Users.AddAsync(appUser);
                    await _db.SaveChangesAsync();
                    
                }
               catch (Exception ex)
               {
                    throw new Exception($"{ex.Message}");
               }
               
            }
            return appUser;
        }
    }
}
