using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cardofun.Domain.Models;

namespace Cardofun.Interfaces.Repositories
{
    /// <summary>
    /// A repository that manages data of Cardofun context
    /// </summary>
    public interface ICardofunRepository
    {
        /// <summary>
        /// Adds a new entity to the repository
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        void Add<T>(T entity);
         
        /// <summary>
        /// Deletes an entity from the repository
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        void Delete<T>(T entity);

        /// <summary>
        /// Saves all changes being made previously
        /// </summary>
        /// <returns>true - changes saved</returns>
        Task<Boolean> SaveChangesAsync();

        /// <summary>
        /// Gets a user out of the repository
        /// </summary>
        /// <param name="id">id of the user that ought to be returned</param>
        /// <returns></returns>
        Task<User> GetUserAsync(Int32 id);
         
        /// <summary>
        /// Gets all of the users out of the repository
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<User>> GetUsersAsync();

        /// <summary>
        /// Gets languages by given search pattern
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Language>> GetLanguages(string languageSearchPattern);

        /// <summary>
        /// Gets cities by given search pattern
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<City>> GetCities(string citySearchPattern);

        /// <summary>
        /// Gets photo by given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Photo> GetPhoto(Guid id);
    }
}