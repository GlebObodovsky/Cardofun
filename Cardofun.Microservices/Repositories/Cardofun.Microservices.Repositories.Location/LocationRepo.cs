using System.Threading.Tasks;
using Grpc.Core;
using static Cardofun.Infrastructure.ServiceDefinitions.Repositories.LocationRepository.LocationRepo;
using Cardofun.Infrastructure.ServiceDefinitions.Repositories.LocationRepository.Messages;
using Cardofun.DataContext.Data;
using System.Linq;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using Cardofun.Domain.Models;
using GrpcCountry = Cardofun.Infrastructure.ServiceDefinitions.Repositories.LocationRepository.Types.Country;

namespace Cardofun.Microservices.Repositories.Location
{
    public class LocationRepo: LocationRepoBase
    {
        private readonly CardofunContext _context;

        public LocationRepo(CardofunContext context)
        {
            _context = context;
        }

        public async override Task<GetCountriesResponse> GetCountries(GetCountriesRequest request, ServerCallContext context)
        {
            var countries = await GetItemsByPredicates<Country>(
                predicates: country => country.Name.ToUpper().StartsWith(request.SearchBy.ToUpper()));

            var grpcCountries = countries?.Select(c => new GrpcCountry 
                { 
                    IsoCode = c.IsoCode,
                    Name = c.Name,
                    ContinentName = c.ContinentName
                });

            var response = new GetCountriesResponse
            {
                Success = true
            };

            response.Countries.AddRange(grpcCountries);

            return await Task.FromResult<GetCountriesResponse>(response);
        }

        /// <summary>
        /// Sets up includes, predicates and orderings
        /// </summary>
        /// <param name="requestSettings">Set of includes, orderings and other request settings</param>
        /// <param name="predicates">Set of predicates</param>
        /// <typeparam name="TEntity">Db entities for getting back</typeparam>
        /// <returns></returns>
        private IQueryable<TEntity> SetUpRequest<TEntity>(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> requestSettings = null, 
            params Expression<Func<TEntity, bool>> [] predicates) 
            where TEntity : class
        {
            var result = _context.Set<TEntity>().AsQueryable();
                        
            if(requestSettings != null)
                result = requestSettings(result);

            foreach (var predicate in predicates)
                result = result.Where(predicate);

            return result;
        }

        /// <summary>
        /// Gets items with a given type and predicate out of db context
        /// </summary>
        /// <param name="requestSettings">Set of includes, orderings and other request settings</param>
        /// <param name="predicates">Conditions</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
		private async Task<IEnumerable<TEntity>> GetItemsByPredicates<TEntity>(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> requestSettings = null, params Expression<Func<TEntity, bool>> [] predicates) 
            where TEntity : class
                => await SetUpRequest(requestSettings: requestSettings, predicates: predicates).ToArrayAsync();
    }
}