using ClassLib.Models.Users;
using System.Security.Claims;

namespace BudgetTracker.MinimalAPI.DataAccess.Interfaces
{
    public interface IUserService
    {
        public Task<UserDTO?> GetUser(string auth0Id);
        public Task<UserDTO> FindOrCreateUser (ClaimsPrincipal user);
    }
}
