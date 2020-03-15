using System;
using System.IO;
using Cardofun.Interfaces.ServiceProviders;
using Microsoft.Extensions.FileProviders;

namespace Cardofun.Modules.FileService
{
    public class PhysicalFileService : IPhysicalFileService
    {
        private readonly IFileProvider _fileProvider;
        public PhysicalFileService(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public String ReadAllFile(String path)
        {
            using(var stream = _fileProvider.GetFileInfo(path).CreateReadStream())
                using(var streamReader = new StreamReader(stream))
                    return streamReader.ReadToEnd();
        }
    }
}
