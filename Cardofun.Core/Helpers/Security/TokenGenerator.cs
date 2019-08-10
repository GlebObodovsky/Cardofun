using System;
using System.Collections.Generic;

namespace Cardofun.Core.Helpers.Security
{
    public static class TokenGenerator
    {
        /// <summary>
        /// Returns a random token with a given length
        /// </summary>
        /// <param name="length">Each unit equals to an encoded guid value</param>
        /// <returns></returns>
        public static string Generate(int length = 20)
        {
            var byteArrayList = new List<byte>();

            for (int i = 0; i < length; i++)
                byteArrayList.AddRange(Guid.NewGuid().ToByteArray());

            return Convert.ToBase64String(byteArrayList.ToArray());
        }
    }
}