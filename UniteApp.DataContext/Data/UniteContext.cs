using Microsoft.EntityFrameworkCore;
using UniteApp.Domain.Models;

namespace UniteApp.DataContext.Data
{
    public class UniteContext: DbContext
    {
        public UniteContext(DbContextOptions<UniteContext> options) :base(options) {}
    }
}
