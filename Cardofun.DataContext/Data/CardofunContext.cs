using Microsoft.EntityFrameworkCore;
using Cardofun.Domain.Models;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Cardofun.DataContext.Data
{
    public class CardofunContext: IdentityDbContext<User, Role, Int32, IdentityUserClaim<Int32>,
        UserRole, IdentityUserLogin<Int32>, IdentityRoleClaim<Int32>, IdentityUserToken<Int32>>
    {
        public CardofunContext(DbContextOptions<CardofunContext> options) :base(options) {}

        #region DbSets

        /// <summary>
        /// Represents a set of photos
        /// </summary>
        /// <value></value>
        public DbSet<Photo> Photos { get; set; }
        /// <summary>
        /// Represents a set of user photos
        /// </summary>
        /// <value></value>
        public DbSet<UserPhoto> UserPhotos { get; set; }
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
        /// <summary>
        /// Represents all of friend requests being made by users
        /// </summary>
        /// <value></value>
        public DbSet<FriendRequest> FriendRequests { get; set; }
        /// <summary>
        /// Represents all messages being sent by users
        /// </summary>
        /// <value></value>
        public DbSet<Message> Messages { get; set; }
        #endregion DbSets


        /// <summary>
        /// Configuring DB model
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Changing table names
            modelBuilder.Entity<User>(x => x.ToTable(nameof(Users)));
            modelBuilder.Entity<Role>(x => x.ToTable(nameof(Roles)));
            modelBuilder.Entity<UserRole>(x => x.ToTable(nameof(UserRoles)));
            modelBuilder.Entity<IdentityRoleClaim<Int32>>(x => x.ToTable(nameof(RoleClaims)));
            modelBuilder.Entity<IdentityUserClaim<Int32>>(x => x.ToTable(nameof(UserClaims)));
            modelBuilder.Entity<IdentityUserLogin<Int32>>(x => x.ToTable(nameof(UserLogins)));
            modelBuilder.Entity<IdentityUserToken<Int32>>(x => x.ToTable(nameof(UserTokens)));
            #endregion Changing table names

            #region User
            modelBuilder.Entity<User>()
                .HasKey(x => x.Id);
            
            modelBuilder.Entity<User>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .HasIndex(x => x.UserName)
                .IsUnique();
            
            modelBuilder.Entity<User>()
                .Property(x => x.UserName)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(x => x.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(x => x.EmailConfirmed)
                .IsRequired();
            
            // Created time would be the exact time when the User is created  
            modelBuilder.Entity<User>()
                .Property(x => x.Created)
                .HasDefaultValueSql("getdate()");

            // Last active time would be the exact time when the User is created
            modelBuilder.Entity<User>()
                .Property(x => x.LastActive)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<User>()
                .Property(x => x.PasswordHash)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasOne(x => x.City)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.CityId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            #endregion User

            #region IdentityUser (security)
            modelBuilder.Entity<UserRole>()
                .HasKey(x => new { x.UserId, x.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .IsRequired();

            modelBuilder.Entity<UserRole>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId)
                .IsRequired();
            #endregion IdentityUser (security)

            #region Photos
            modelBuilder.Entity<Photo>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Photo>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Photo>()
                .Property(x => x.Url)
                .IsRequired();

            modelBuilder.Entity<Photo>()
                .HasOne(x => x.UserPhoto)
                .WithOne(x => x.Photo)
                .HasForeignKey<UserPhoto>(x => x.PhotoId);
                            
            modelBuilder.Entity<Photo>()
                .HasOne(x => x.Message)
                .WithOne(x => x.Photo)
                .HasForeignKey<Message>(x => x.PhotoId);

            #endregion Photos

            #region UserPhotos
            modelBuilder.Entity<UserPhoto>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<UserPhoto>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<UserPhoto>()
                .HasOne(x => x.User)
                .WithMany(x => x.Photos)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<UserPhoto>()
                .Property(x => x.DateAdded)
                .IsRequired();
                
            // DateAdded would be the exact date and time when the picture is created  
            modelBuilder.Entity<UserPhoto>()
                .Property(x => x.DateAdded)
                .HasDefaultValueSql("getdate()");

            // There should be only one main photo for each users
            modelBuilder.Entity<UserPhoto>()
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
                .HasKey(x => x.Code);

            modelBuilder.Entity<Language>()
                .Property(x => x.Code)
                .HasMaxLength(2);

            modelBuilder.Entity<Language>()
                .Property(x => x.Name)
                .HasMaxLength(60);
            #endregion Language

            #region LanguageLevel
            modelBuilder.Entity<LanguageLevel>()
                .HasKey(x => new {x.LanguageCode, x.UserId});
            #endregion LanguageLevel

            #region LanguageLearningLevel
            modelBuilder.Entity<LanguageLearningLevel>()
                .HasOne(x => x.Language)
                .WithMany(x => x.LanguageLearningLevels)
                .HasForeignKey(x => x.LanguageCode)
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
                .HasForeignKey(x => x.LanguageCode)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<LanguageSpeakingLevel>()
                .HasOne(x => x.User)
                .WithMany(x => x.LanguagesTheUserSpeaks)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion LanguageSpeakingLevel
        
            #region FriendRequests
            modelBuilder.Entity<FriendRequest>()
                .HasKey(x => new { x.FromUserId, x.ToUserId });

            modelBuilder.Entity<FriendRequest>()
                .HasOne(x => x.ToUser)
                .WithMany(x => x.IncomingFriendRequests)
                .HasForeignKey(x => x.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(x => x.FromUser)
                .WithMany(x => x.OutcomingFriendRequests)
                .HasForeignKey(x => x.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
                .Property(x => x.RequestedAt)
                .HasDefaultValueSql("getdate()");
            #endregion FriendRequests

            #region Messages
            modelBuilder.Entity<Message>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Message>()
                .Property(x => x.Id)  
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Message>()
                .HasOne(x => x.Sender)
                .WithMany(x => x.OutcomingMessages)
                .HasForeignKey(x => x.SenderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(x => x.Recipient)
                .WithMany(x => x.IncomingMessages)
                .HasForeignKey(x => x.RecipientId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .Property(x => x.SentAt)
                .HasDefaultValueSql("getdate()");

            #endregion Messages
        }     
    }
}