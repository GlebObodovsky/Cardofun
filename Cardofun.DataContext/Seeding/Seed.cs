using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Cardofun.DataContext.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Cardofun.Domain.Models;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace Cardofun.DataContext.Seeding
{
    public class Seed
    {
        /// <summary>
        /// Applying all the needed functions and stored procedures
        /// </summary>
        /// <param name="context"></param>
        public static void PropagateSql(CardofunContext context)
        {
            var location = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Seed)).Location);

            var sqlFiles = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(Seed)).Location), "sql"), "*.sql")
                .OrderBy(fileName => fileName);

            foreach(var fileName in sqlFiles)
            {
                var sqlQuery = File.ReadAllText(fileName);
                   context.Database.ExecuteSqlRaw(sqlQuery);
            }

            context.SaveChanges();
        }

        public static void SeedCitiesAndLanguages(CardofunContext context)
        {
            if(context.Continents.Any() || context.Countries.Any() || context.Cities.Any() || context.Languages.Any())
                return;

            var rootPath = "../Cardofun.DataContext/Seeding/Resources";

            var sqlFiles = Directory.GetFiles(rootPath, "*.sql")
                .OrderBy(fileName => fileName);

            context.Database.SetCommandTimeout(TimeSpan.FromMinutes(20));
            
            var zipArchives = Directory.GetFiles(rootPath, "*.zip");

            Parallel.ForEach(zipArchives, z => ZipFile.ExtractToDirectory(z, rootPath));

            foreach (var fileName in sqlFiles)
            {
                var sqlQuery = File.ReadAllText(fileName);
                if (sqlQuery.StartsWith("EXEC"))
                    context.Database.ExecuteSqlCommand(sqlQuery, new SqlParameter("@FolderPath", Path.GetFullPath(rootPath)));
                else
                    context.Database.ExecuteSqlCommand(sqlQuery);
            }

            context.SaveChanges();

            var txtFiles = Directory.GetFiles(rootPath, "*.txt");
            Parallel.ForEach(txtFiles, t => File.Delete(t));
        }

        public static void SeedUsers(CardofunContext context)
        {
            if(context.Users.Any())
                return;

            var userData = File.ReadAllText("../Cardofun.DataContext/Seeding/Resources/Users.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            
            // removed after migrating to IdentityUser as it is not needed anymore
            // Parallel.ForEach(users, user => 
            //     {
            //         byte[] passwordHash, passwordSalt; 
            //         CreatePasswordHash("password", out passwordHash, out passwordSalt);
            //         user.PasswordHash = passwordHash;
            //         user.PasswordSalt = passwordSalt;
            //     });

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        // removed after migrating to IdentityUser as it is not needed anymore
        /// <summary>
        /// Creating password hash and password salt out of given password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        // private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        // {
        //     using(var hmac = new HMACSHA512())
        //     {
        //         passwordSalt = hmac.Key;
        //         passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        //     }
        // }
    }
}