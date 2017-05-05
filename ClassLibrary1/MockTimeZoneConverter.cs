using System.IO;
using System.Threading.Tasks;

namespace BusInfo
{
    public class MockTimeZoneConverter : ITimeZoneConverter
    {
        private static string LoadJson(string file) => File.ReadAllText(file);

        public Task<string> GetJsonForTimeZoneFromLatLongAsync(string lat, string lon)
        {
            var json = LoadJson(@"c:\users\kaseyu\documents\visual studio 2017\Projects\ClassLibrary1\UnitTestProject1\TimeZone.json");
            return Task.FromResult(json);
        }
    }
}
