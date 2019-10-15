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
        /// Starts transaction for the repository
        /// </summary>
        void StartTransaction();
        
        /// <summary>
        /// Commits transaction for the repository
        /// </summary>
        void CommitTransaction();

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
        Task<IEnumerable<Language>> GetLanguagesAsync(String languageSearchPattern);

        /// <summary>
        /// Gets cities by given search pattern
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<City>> GetCitiesAsync(String citySearchPattern);

        /// <summary>
        /// Gets photo by given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Photo> GetPhotoAsync(Guid id);

        /// <summary>
        /// Gets user's main photo
        /// </summary>
        /// <param name="userId">User of which main photo is needed</param>
        /// <returns></returns>
        Task<Photo> GetMainPhotoForUserAsync(Int32 userId);
    }
}