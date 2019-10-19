using System;

namespace Cardofun.Core.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Converts DateTime to years passed from the given date and time
        /// </summary>
        /// <param name="dateTime">Date and time years passed from</param>
        /// <returns></returns>
        public static Int32 ToAges(this DateTime dateTime)
        {
            var age = DateTime.Today.Year - dateTime.Year;

            if (DateTime.Today.DayOfYear < dateTime.DayOfYear)
                age--;

            return age;
        }
    }
}