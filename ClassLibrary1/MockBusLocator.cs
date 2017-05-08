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
            var json = LoadJson(@"C:\Users\kaseyu\Source\Repos\TransitKing\UnitTestProject1\Arrivals.json");
            return Task.FromResult(json);
        }

        public Task<string> GetJsonForStopsFromLatLongAsync(string lat, string lon)
        {
            var json = LoadJson(@"C:\Users\kaseyu\Source\Repos\TransitKing\UnitTestProject1\StopsForLoc.json");
            return Task.FromResult(json);
        }
    }
}
