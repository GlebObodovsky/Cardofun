using System;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Cardofun.DataContext.Data;
using Microsoft.EntityFrameworkCore;

namespace Cardofun.DataContext.Seeding
{
    public class Seed
    {
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

            foreach(var fileName in sqlFiles)
            {
                var sqlQuery = File.ReadAllText(fileName);
                if(sqlQuery.StartsWith("EXEC"))
                    context.Database.ExecuteSqlCommand(sqlQuery, new SqlParameter("@FolderPath", Path.GetFullPath(rootPath)));
                else
                   context.Database.ExecuteSqlCommand(sqlQuery);
            }

            context.SaveChanges();

            var txtFiles = Directory.GetFiles(rootPath, "*.txt");
            Parallel.ForEach(txtFiles, t => File.Delete(t));
        }
    }
}