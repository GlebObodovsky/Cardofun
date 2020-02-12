using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cardofun.Core.ApiParameters;
using Cardofun.Core.Enumerables;
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
        /// Checks if user with the given login already exists 
        /// </summary>
        /// <param name="login">login by which to make a search</param>
        /// <returns></returns>
        Task<Boolean> CheckIfUserExists(String login);

        /// <summary>
        /// Gets a user out of the repository
        /// </summary>
        /// <param name="id">id of the user that ought to be returned</param>
        /// <returns></returns>
        Task<User> GetUserAsync(Int32 id);

        /// <summary>
        /// Gets a user out of the repository by his/her name
        /// </summary>
        /// <param name="userName">user name of the user that ought to be returned</param>
        /// <returns></returns>
        Task<User> GetUserByNameAsync(String userName);

        /// <summary>
        /// Gets a user with basic details but including the roles out of the repository by his/her user name
        /// </summary>
        /// <param name="userName">user name of the user that ought to be returned</param>
        /// <returns></returns>
        Task<User> GetUserWithRolesByNameAsync(String userName);
         
        /// <summary>
        /// Gets page of users out of the repository
        /// </summary>
        /// <returns></returns>
        Task<PagedList<User>> GetPageOfUsersAsync(UserParams userParams);

        /// <summary>
        /// Gets page of friends with specified status out of the repository
        /// </summary>
        /// <returns></returns>
        Task<PagedList<User>> GetPageOfFriendsAsync(UserFriendParams userFriendParams);

        /// <summary>
        /// Gets an amount of users that are following the user with the given Id
        /// </summary>
        /// <returns></returns>
        Task<Int32> GetCountOfFollowersAsync(Int32 userId);

        /// <summary>
        /// Gets languages by given search pattern
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Language>> GetLanguagesAsync(String languageSearchPattern);

        /// <summary>
        /// Gets countries by given search pattern
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Country>> GetCountriesAsync(String countrySearchPattern);

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
        Task<UserPhoto> GetUserPhotoAsync(Guid id);

        /// <summary>
        /// Gets user's main photo
        /// </summary>
        /// <param name="userId">User of which main photo is needed</param>
        /// <returns></returns>
        Task<UserPhoto> GetMainPhotoForUserAsync(Int32 userId);

        /// <summary>
        /// Gets a request for friendship
        /// </summary>
        /// <param name="fromUserId">Id of a user that sent the request</param>
        /// <param name="toUserId">Id of a user that received the request</param>
        /// <returns></returns>
        Task<FriendRequest> GetFriendRequestAsync(Int32 fromUserId, Int32 toUserId);
 
        /// <summary>
        /// Get's a message by it's Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Message> GetMessageAsync(Guid id);

        /// <summary>
        /// Gets an amount of unread messages for a user by his id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Int32> GetCountOfUnreadMessagesAsync(Int32 userId);

        /// <summary>
        /// Gets a page of dialogues of a user 
        /// </summary>
        /// <returns></returns>
        Task<PagedList<Message>> GetDialoguesForUser(MessagePrams messagePrams);

        /// <summary>
        /// Gets a paginated message thread between two users
        /// </summary>
        /// <returns></returns>
        Task<PagedList<Message>> GetPaginatedMessageThread(MessageThreadPrams messageParams);

        /// <summary>
        /// Get's all the roles available on the server
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Role>> GetRolesAsync();
    }
}