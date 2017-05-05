using System.IO;
using System.Threading.Tasks;

namespace BusInfo
{
    public class MockBusLocator : IBusLocator
    {
        private static string LoadJson(string file)
        {
            return File.ReadAllText(file);
        }

        public Task<string> GetJsonForArrivals(string stopId)
        {
            var json = LoadJson(@"c:\users\kaseyu\documents\visual studio 2017\Projects\ClassLibrary1\UnitTestProject1\Arrivals.json");
            return Task.FromResult(json);
        }

        public Task<string> GetJsonForStopsFromLatLongAsync(string lat, string lon)
        {
            var json = LoadJson(@"c:\users\kaseyu\documents\visual studio 2017\Projects\ClassLibrary1\UnitTestProject1\StopsForLoc.json");
            return Task.FromResult(json);
        }
    }
}
