using Microsoft.EntityFrameworkCore;
using UniteApp.Domain.Models;

namespace UniteApp.DataContext.Data
{
    public class UniteContext: DbContext
    {
        public UniteContext(DbContextOptions<UniteContext> options) :base(options) {}
        /// <summary>
        /// Represents a set of stored Users and their base information
        /// </summary>
        /// <value></value>
        public DbSet<User> Users { get; set; }
        /// <summary>
        /// Configuring DB model
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region User
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Login)
                .IsUnique();
            
            modelBuilder.Entity<User>()
                .Property(x => x.Login)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(x => x.PasswordHash)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(x => x.PasswordSalt)
                .IsRequired();
            #endregion User
        }     
    }
}
