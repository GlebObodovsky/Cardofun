using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cardofun.DataContext.Data;
using Cardofun.Domain.Models;
using Cardofun.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cardofun.DataContext.Repositories
{
    public class CardofunRepository : ICardofunRepository
    {
        #region Fields
        /// <summary>
        /// Database context
        /// </summary>
        private readonly CardofunContext _context;
        #endregion Fields

        #region Constructor
        public CardofunRepository(CardofunContext context)
        {
            _context = context;
        }
        #endregion Constructor

        #region Functions
        /// <summary>
        /// Gets an item with specified Id out of db context
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T">Type of entity to get</typeparam>
        /// <returns></returns>
        private async Task<T> GetItemAsync<T>(object id)
            => (T)await _context.FindAsync(typeof(T), id);

        /// <summary>
        /// Gets all of items with a given type out of db context
        /// </summary>
        /// <param name="Expression<Func<T"></param>
        /// <param name="includes">Included navigation properties</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
		private async Task<IEnumerable<T>> GetAllItemsAsync<T>(params Expression<Func<T, object>>[] includes)
			where T : class
		{
			IQueryable<T> dbSet = _context.Set<T>();

			foreach (var query in includes)
				dbSet = dbSet.Include(query);
                
            return await dbSet.ToArrayAsync();
        }
        #endregion Functions

        #region ICardofunRepository
        /// <summary>
        /// Adds a new entity to the repository
        /// </summary>
        public void Add<T>(T entity)
            => _context.Add(entity);

        /// <summary>
        /// Deletes an entity from the repository
        /// </summary>
        public void Delete<T>(T entity)
            => _context.Remove(entity);

        /// <summary>
        /// Saves all changes being made previously
        /// </summary>
        /// <returns>true - changes saved</returns>
        public async Task<Boolean> SaveChangesAsync()
            => await _context.SaveChangesAsync() > 0;

        /// <summary>
        /// Gets a user out of the repository
        /// </summary>
        /// <param name="id">id of the user that ought to be returned</param>
        /// <returns></returns>
        public async Task<User> GetUserAsync(int id)
            => await GetItemAsync<User>(id);

        /// <summary>
        /// Gets all of the users out of the repository
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetUsersAsync()
            => await GetAllItemsAsync<User>(
                x => x.City, 
                x => x.City.Country, 
                x => x.Photos, 
                x => x.LanguagesTheUserLearns,
                x => x.LanguagesTheUserSpeaks);
        #endregion ICardofunRepository
    }
}