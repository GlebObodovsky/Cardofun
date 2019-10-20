using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Cardofun.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Cardofun.API.Helpers
{
    /// <summary>
    /// Updates "LastActive" property of a User that executed an action
    /// </summary>
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            var nameIdentifier = resultContext.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier);

            if(nameIdentifier == null)
                return;

            var userId = Int32.Parse(nameIdentifier.Value);
        
            var cardofunRepo = resultContext.HttpContext.RequestServices.GetService<ICardofunRepository>();

            var user = await cardofunRepo.GetUserAsync(userId);

            if(user == null)
                return;

            user.LastActive = DateTime.Now;

            await cardofunRepo.SaveChangesAsync();
        }
    }
}