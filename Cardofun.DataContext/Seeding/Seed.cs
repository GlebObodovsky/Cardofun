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
using Microsoft.AspNetCore.Identity;
using Cardofun.Core.NameConstants;

namespace Cardofun.DataContext.Seeding
{
    public static class Seed
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

        public async static Task SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if(userManager.Users.Any())
                return;

            await roleManager.CreateAsync(new Role { Name = RoleConstants.Admin });
            await roleManager.CreateAsync(new Role { Name = RoleConstants.Moderator });
            await roleManager.CreateAsync(new Role { Name = RoleConstants.Member });

            var userData = File.ReadAllText("../Cardofun.DataContext/Seeding/Resources/Users.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);

            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "password");
                await userManager.AddToRoleAsync(user, RoleConstants.Member);
            }

            // creating Admin user
            var admin = new User 
            { 
                UserName = RoleConstants.Admin,
                Email = RoleConstants.Admin,
                CityId = 1
            };

            var result = await userManager.CreateAsync(admin, "password");
            if (!result.Succeeded)
                throw new Exception("The admin user hasn't been created");

            await userManager.AddToRolesAsync(admin, new[] { RoleConstants.Admin, RoleConstants.Moderator });
        }
    }
}