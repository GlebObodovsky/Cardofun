using Microsoft.EntityFrameworkCore;
using Cardofun.Domain.Models;
using System;

namespace Cardofun.DataContext.Data
{
    public class CardofunContext: DbContext
    {
        public CardofunContext(DbContextOptions<CardofunContext> options) :base(options) {}


        #region DbSets

        /// <summary>
        /// Represents a set of stored Users and their base information
        /// </summary>
        /// <value></value>
        public DbSet<User> Users { get; set; }
        /// <summary>
        /// Represents a set of user photos
        /// </summary>
        /// <value></value>
        public DbSet<Photo> Photos { get; set; }
        /// <summary>
        /// Represents a set of stored Continents
        /// </summary>
        /// <value></value>
        public DbSet<Continent> Continents { get; set; }
        /// <summary>
        /// Represents a set of stored Countries
        /// </summary>
        /// <value></value>
        public DbSet<Country> Countries { get; set; }
        /// <summary>
        /// Represents a set of stored Cities
        /// </summary>
        /// <value></value>
        public DbSet<City> Cities { get; set; }
        /// <summary>
        /// Represents a set of stored Languages
        /// </summary>
        /// <value></value>
        public DbSet<Language> Languages { get; set; }
        /// <summary>
        /// Represents a set of stored LanguageSpeakingLevels
        /// </summary>
        /// <value></value>
        public DbSet<LanguageSpeakingLevel> LanguageSpeakingLevels { get; set; }
        /// <summary>
        /// Represents a set of stored LanguageSpeakingLevels
        /// </summary>
        /// <value></value>
        public DbSet<LanguageLearningLevel> LanguageLearningLevels { get; set; }

        #endregion DbSets


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
                .Property(x => x.Created)
                .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<User>()
                .Property(x => x.PasswordHash)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(x => x.PasswordSalt)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasOne(x => x.City)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            #endregion User

            #region Photos
            modelBuilder.Entity<Photo>()
                .HasIndex(x => x.Id);
            
            modelBuilder.Entity<Photo>()
                .HasOne(x => x.User)
                .WithMany(x => x.Photos)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Photo>()
                .Property(x => x.Url)
                .IsRequired();
                
            modelBuilder.Entity<Photo>()
                .Property(x => x.DateAdded)
                .IsRequired();
                
            modelBuilder.Entity<Photo>()
                .Property(x => x.DateAdded)
                .HasDefaultValue(DateTime.Now);

            modelBuilder.Entity<Photo>()
                .HasIndex(e => new { e.UserId, e.IsMain })
                .IsUnique()
                .HasFilter("[IsMain] = 1");
            #endregion Photos

            #region Continent
            modelBuilder.Entity<Continent>()
                .HasKey(x => x.Name);

            modelBuilder.Entity<Continent>()
                .Property(x => x.Name)
                .HasMaxLength(30);
            #endregion Continent

            #region Country
            modelBuilder.Entity<Country>()
                .HasKey(x => x.IsoCode);

            modelBuilder.Entity<Country>()
                .Property(x => x.IsoCode)
                .HasMaxLength(2);

            modelBuilder.Entity<Country>()
                .Property(x => x.Name)
                .HasMaxLength(100);
            
            modelBuilder.Entity<Country>()
                .HasOne(x => x.Continent)
                .WithMany(x => x.Countries)
                .HasForeignKey(x => x.ContinentName)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion Country

            #region City
            modelBuilder.Entity<City>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<City>()
                .HasIndex(x => new {x.CountryIsoCode, x.Name})
                .IsUnique();

            modelBuilder.Entity<City>()
                .Property(x => x.Name)
                .HasMaxLength(150);
            
            modelBuilder.Entity<City>()
                .HasOne(x => x.Country)
                .WithMany(x => x.Cities)
                .HasForeignKey(x => x.CountryIsoCode)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            #endregion City

            #region Language
            modelBuilder.Entity<Language>()
                .HasKey(x => x.Name);

            modelBuilder.Entity<Language>()
                .Property(x => x.Name)
                .HasMaxLength(60);
            
            modelBuilder.Entity<Language>()
                .HasOne(x => x.CountryOfOrigin)
                .WithMany(x => x.Languages)
                .HasForeignKey(x => x.CountryOfOriginCode)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            #endregion Language

            #region LanguageLevel
            modelBuilder.Entity<LanguageLevel>()
                .HasKey(x => new {x.LanguageName, x.UserId});
            #endregion LanguageLevel

            #region LanguageLearningLevel
            modelBuilder.Entity<LanguageLearningLevel>()
                .HasOne(x => x.Language)
                .WithMany(x => x.LanguageLearningLevels)
                .HasForeignKey(x => x.LanguageName)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LanguageLearningLevel>()
                .HasOne(x => x.User)
                .WithMany(x => x.LanguagesTheUserLearns)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion LanguageLearningLevel

            #region LanguageSpeakingLevel
            modelBuilder.Entity<LanguageSpeakingLevel>()
                .HasOne(x => x.Language)
                .WithMany(x => x.LanguageSpeakingLevels)
                .HasForeignKey(x => x.LanguageName)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<LanguageSpeakingLevel>()
                .HasOne(x => x.User)
                .WithMany(x => x.LanguagesTheUserSpeaks)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion LanguageSpeakingLevel
        }     
    }
}
