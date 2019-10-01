using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cardofun.DataContext.Data;
using Cardofun.DataContext.Helpers;
using Cardofun.Domain.Models;
using Cardofun.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

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
        /// <typeparam name="TEntity">Type of entity to get</typeparam>
        /// <typeparam name="TKey">Primary key of entity to get</typeparam>
        /// <returns></returns>
        private async Task<TEntity> GetItemAsync<TEntity, TKey>(TKey id, params Expression<Func<TEntity, object>>[] includes) 
            where TEntity: class
        {
            // Getting the primary key info
            var keyProperty = _context.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties[0];

            // Getting the requested entity joining along all the needed properties (tables)
            return (TEntity)await _context
                .Set<TEntity>()
                .IncludeAll(includes)
                .FirstOrDefaultAsync(e => EF.Property<TKey>(e, keyProperty.Name).Equals(id));
        }

        /// <summary>
        /// Gets all of items with a given type out of db context
        /// </summary>
        /// <param name="Expression<Func<T"></param>
        /// <param name="includes">Included navigation properties</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
		private async Task<IEnumerable<TEntity>> GetAllItemsAsync<TEntity>(params Expression<Func<TEntity, object>>[] includes) where TEntity : class
            => await _context.Set<TEntity>().IncludeAll(includes).ToArrayAsync();
        #endregion Functions

        #region ICardofunRepository
        /// <summary>
        /// Adds a new entity to the repository
        /// </summary>
        public void Add<TEntity>(TEntity entity)
            => _context.Add(entity);

        /// <summary>
        /// Deletes an entity from the repository
        /// </summary>
        public void Delete<TEntity>(TEntity entity)
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
            => await GetItemAsync<User, Int32>(id, 
            x => x.City,
            x => x.City.Country,
            x => x.Photos,
            x => x.LanguagesTheUserLearns,
            x => x.LanguagesTheUserLearns.Select(l => l.Language),
            x => x.LanguagesTheUserSpeaks,
            x => x.LanguagesTheUserSpeaks.Select(l => l.Language));
         
        /// <summary>
        /// Gets all of the users out of the repository
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users
                .Include(x => x.City) 
                    .ThenInclude(x => x.Country) 
                .Include(x => x.Photos) 
                .Include(x => x.LanguagesTheUserLearns) 
                    .ThenInclude(x => x.Language) 
                .Include(x => x.LanguagesTheUserSpeaks) 
                    .ThenInclude(x => x.Language)
                .ToListAsync();
        }
        #endregion ICardofunRepository
    }
}