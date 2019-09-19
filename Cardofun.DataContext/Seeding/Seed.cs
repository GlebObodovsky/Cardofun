using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Cardofun.DataContext.Data;
using Microsoft.EntityFrameworkCore;

namespace Cardofun.DataContext.Seeding
{
    public class Seed
    {
        public static void SeedCities(CardofunContext context)
        {
            if(context.Cities.Any())
                return;

            var sqlFiles = Directory.GetFiles("../Cardofun.DataContext/Seeding/Resources", "*.sql")
                .OrderBy(fileName => fileName);

            context.Database.SetCommandTimeout(TimeSpan.FromMinutes(20));

            foreach(var fileName in sqlFiles)
            {
                var sqlQuery = File.ReadAllText(fileName);
                if(sqlQuery.StartsWith("EXEC"))
                    context.Database.ExecuteSqlCommand(sqlQuery, new SqlParameter("@FolderPath", Path.GetFullPath("../Cardofun.DataContext/Seeding/Resources")));
                else
                   context.Database.ExecuteSqlCommand(sqlQuery);
            }

            context.SaveChanges();
        }
    }
}