using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cardofun.Core.ApiParameters;
using Cardofun.Core.Enumerables;
using Cardofun.Core.Helpers;
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
        /// <param name="id">Key</param>
        /// <param name="include">Set of includes</param>
        /// <typeparam name="TEntity">Type of entity to get</typeparam>
        /// <typeparam name="TKey">Type of the passing key (id) of entity to get</typeparam>
        /// <returns></returns>
        private async Task<TEntity> GetItemAsync<TEntity, TKey>(TKey id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null) 
            where TEntity : class
        {
            // Getting the primary key info
            var keyProperty = _context.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties[0];

            var result = _context.Set<TEntity>().AsQueryable();

            if(include != null)
                result = include(result);

            // Getting the requested entity joining along all the needed properties (tables)
            return await result.FirstOrDefaultAsync(e => EF.Property<TKey>(e, keyProperty.Name).Equals(id));
        }

        /// <summary>
        /// Gets an item with a given type and predicate out of db context
        /// </summary>
        /// <param name="includes">Included navigation properties</param>
        /// <param name="predicates">Conditions</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
		private async Task<TEntity> GetItemByPredicatesAsync<TEntity>(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, params Expression<Func<TEntity, bool>> [] predicates) 
            where TEntity : class
        {
            var result = _context.Set<TEntity>().AsQueryable();
                        
            if(include != null)
                result = include(result);

            foreach (var predicate in predicates)
                result = result.Where(predicate);

            return await result.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all of items with a given type out of db context
        /// </summary>
        /// <param name="includes">Included navigation properties</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
		private async Task<IEnumerable<TEntity>> GetAllItemsAsync<TEntity>(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null) 
            where TEntity : class
        {
            var result = _context.Set<TEntity>().AsQueryable();

            if(include != null)
                result = include(result);

            return await result.ToArrayAsync();
        }

        /// <summary>
        /// Gets all of items with a given type out of db context
        /// </summary>
        /// <param name="includes">Included navigation properties</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
		private async Task<PagedList<TEntity>> GetPageOfItemsAsync<TEntity>(PaginationParams paginationParams, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,  params Expression<Func<TEntity, bool>> [] predicates) 
            where TEntity : class
        {
            var result = _context.Set<TEntity>().AsQueryable();

            if(include != null)
                result = include(result);

            foreach (var predicate in predicates)
                result = result.Where(predicate);

            return await result.ToPagedListAsync(paginationParams.PageNumber, paginationParams.PageSize);
        }

        /// <summary>
        /// Gets items with a given type and predicate out of db context
        /// </summary>
        /// <param name="includes">Included navigation properties</param>
        /// <param name="predicates">Conditions</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
		private async Task<IEnumerable<TEntity>> GetItemsByPredicates<TEntity>(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, params Expression<Func<TEntity, bool>> [] predicates) 
            where TEntity : class
        {
            var result = _context.Set<TEntity>().AsQueryable();
            
            if(include != null)
                result = include(result);

            foreach (var predicate in predicates)
                result = result.Where(predicate);

            return await result.ToArrayAsync();
        }
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
        /// Starts transaction for the repository
        /// </summary>
        public void StartTransaction()
            => _context.Database.BeginTransaction();

        /// <summary>
        /// Commits transaction for the repository
        /// </summary>
        public void CommitTransaction()
            => _context.Database.CommitTransaction();

        #region Users
        /// <summary>
        /// Checks if user with the given login already exists 
        /// </summary>
        /// <param name="login">login by which to make a search</param>
        /// <returns></returns>
        public async Task<Boolean> CheckIfUserExists(String login)
            => await _context.Users.AnyAsync(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Gets a user out of the repository
        /// </summary>
        /// <param name="id">id of the user that ought to be returned</param>
        /// <returns></returns>
        public async Task<User> GetUserAsync(int id)
            => await GetItemAsync<User, Int32>(id, 
                user => user
                    .Include(u => u.City)
                        .ThenInclude(c => c.Country)
                    .Include(u => u.Photos)
                    .Include(u => u.LanguagesTheUserLearns)
                        .ThenInclude(u => u.Language)
                    .Include(u => u.LanguagesTheUserSpeaks)
                        .ThenInclude(u => u.Language)); 
         
        /// <summary>
        /// Gets page of users out of the repository
        /// </summary>
        /// <returns></returns>
        public async Task<PagedList<User>> GetPageOfUsersAsync(UserParams userParams)
            => await GetPageOfItemsAsync<User>(
                // Pagination parameters
                userParams,
                // Includes
                user => user
                    .Include(x => x.City) 
                        .ThenInclude(x => x.Country) 
                    .Include(x => x.Photos) 
                    .Include(x => x.LanguagesTheUserLearns) 
                        .ThenInclude(x => x.Language) 
                    .Include(x => x.LanguagesTheUserSpeaks) 
                        .ThenInclude(x => x.Language),
                // Filterings
                user => user.Id != userParams.UserId,
                user => !userParams.Sex.HasValue || user.Sex == userParams.Sex,
                user => !userParams.AgeMin.HasValue || user.BirthDate.ToAges() >= userParams.AgeMin,
                user => !userParams.AgeMax.HasValue || user.BirthDate.ToAges() <= userParams.AgeMax,
                user => !userParams.CityId.HasValue || user.CityId == userParams.CityId,
                user => userParams.CityId.HasValue || String.IsNullOrEmpty(userParams.CountryIsoCode) || user.City.CountryIsoCode.Equals(userParams.CountryIsoCode),
                user => String.IsNullOrEmpty(userParams.LanguageLearningCode) || user.LanguagesTheUserLearns.Any(l => l.LanguageCode.Equals(userParams.LanguageLearningCode)),
                user => String.IsNullOrEmpty(userParams.LanguageSpeakingCode) || user.LanguagesTheUserSpeaks.Any(l => l.LanguageCode.Equals(userParams.LanguageSpeakingCode)));
        #endregion Users

        #region Languages
        /// <summary>
        /// Gets languages by given search pattern
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Language>> GetLanguagesAsync(String languageSearchPattern)
            => await GetItemsByPredicates<Language>(predicates: language => language.Name.ToUpper().Contains(languageSearchPattern.ToUpper()));
        #endregion Languages

        #region Countries
        /// <summary>
        /// Gets countries by given search pattern
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Country>> GetCountriesAsync(String countrySearchPattern)
            => await GetItemsByPredicates<Country>(
                predicates: country => country.Name.ToUpper().StartsWith(countrySearchPattern.ToUpper()));
        #endregion Countries

        #region Cities
        /// <summary>
        /// Gets cities by given search pattern
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<City>> GetCitiesAsync(String citySearchPattern)
            => await GetItemsByPredicates<City>(
                include: city => city.Include(c => c.Country),
                predicates: city => city.Name.ToUpper().StartsWith(citySearchPattern.ToUpper()));
        #endregion Cities
    
        #region Photos
        /// <summary>
        /// Gets photo by given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Photo> GetPhotoAsync(Guid id)
            => await GetItemAsync<Photo, Guid>(id);

        /// <summary>
        /// Gets user's main photo
        /// </summary>
        /// <param name="userId">User of which main photo is needed</param>
        /// <returns></returns>
        public async Task<Photo> GetMainPhotoForUserAsync(Int32 userId)
            => await GetItemByPredicatesAsync<Photo>(null,
                photo => photo.UserId == userId,
                photo => photo.IsMain);
        #endregion Photos
    }
    #endregion ICardofunRepository
}