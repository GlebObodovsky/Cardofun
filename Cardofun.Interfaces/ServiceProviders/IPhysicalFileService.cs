using System;

namespace Cardofun.Interfaces.ServiceProviders
{
    public interface IPhysicalFileService
    {
        /// <summary>
        /// Reads the file by specified path and returns the entire content of it
        /// </summary>
        /// <param name="path">Path to the reading file</param>
        /// <returns></returns>
         String ReadAllFile(String path);
    }
}