using System.Threading.Tasks;
using Cardofun.Domain.Models;

namespace Cardofun.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        /// <summary>
        /// Allows to create a new user in database
        /// </summary>
        /// <param name="user">New user data</param>
        /// <param name="password">New user password</param>
        /// <returns>Registered User</returns>
         Task<User> RegisterAsync(User user, string password); 
        /// <summary>
        /// Allows user to login with his/her credentials
        /// </summary>
        /// <param name="login">User's login</param>
        /// <param name="password">User's password</param>
        /// <returns>Authenticated user</returns>
        Task<User> LoginAsync(string login, string password);
        /// <summary>
        /// Allows to determine if a user with certain login exists in the database
        /// </summary>
        /// <param name="login">Login too check on</param>
        /// <returns>True - the useralready exists</returns>
        Task<bool> IsExistAsync(string login);
    }
}