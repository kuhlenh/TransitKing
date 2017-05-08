﻿using System.IO;
using System.Threading.Tasks;

namespace BusInfo
{
    public class MockTimeZoneConverter : ITimeZoneConverter
    {
        private static string LoadJson(string file) => File.ReadAllText(file);

        public Task<string> GetTimeZoneJsonFromLatLonAsync(string lat, string lon)
        {
            var json = LoadJson(@"C:\Users\kaseyu\Source\Repos\TransitKing\UnitTestProject1\Location.json");
            return Task.FromResult(json);
        }
    }
}
