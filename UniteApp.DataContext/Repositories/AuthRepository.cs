using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UniteApp.DataContext.Data;
using UniteApp.Domain.Models;
using UniteApp.Interfaces.Repositories;
using System.Security.Cryptography;
using System;
using System.Text;

namespace UniteApp.DataContext.Repositories
{
    /// <summary>
    /// Managing Users and their credentials
    /// </summary>
    public class AuthRepository : IAuthRepository
    {
        #region Fields
        /// <summary>
        /// Database context
        /// </summary>
        private readonly UniteContext _context;
        #endregion Fields

        #region Constructor
        public AuthRepository(UniteContext context)
        {
            _context = context;
        }
        #endregion Constructor

        #region IAuthRepository
        /// <summary>
        /// Allows to create a new user in database
        /// </summary>
        /// <param name="user">New user data</param>
        /// <param name="password">New user password</param>
        /// <returns>Registered User</returns>
        public async Task<User> RegisterAsync(User user, string password)
        {
            if(await IsExistAsync(user.Login))
                return null;

            byte [] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }
        /// <summary>
        /// Allows user to login with his/her credentials
        /// </summary>
        /// <param name="login">User's login</param>
        /// <param name="password">User's password</param>
        /// <returns>Authenticated user</returns>        
        public async Task<User> LoginAsync(string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login.ToUpper() == login.ToUpper());

            if(user == null)
                return null;

            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            else
                return user;
        }
        /// <summary>
        /// Allows to determine if a user with certain login exists in the database
        /// </summary>
        /// <param name="login">Login too check on</param>
        /// <returns>True - the useralready exists</returns>
        public async Task<bool> IsExistAsync(string login)
        {
            return await _context.Users.AnyAsync(u => u.Login.ToUpper() == login.ToUpper());
        }
        #endregion IAuthRepository
        
        #region Functions
        /// <summary>
        /// Creating password hash and password salt out of given password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        /// <summary>
        /// Verifies if a given password is correct
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        /// <returns></returns>
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                
                if(computedHash.Length != passwordHash.Length)
                    return false;

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordHash[i])
                        return false;
                }

                return true;
            }
        }
        #endregion Functions
    }
}