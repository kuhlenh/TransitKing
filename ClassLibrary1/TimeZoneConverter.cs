using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BusInfo
{
    public class TimeZoneConverter : ITimeZoneConverter
    {
        HttpClient http = new HttpClient();
        private const string Key = "demo";

        public async Task<string> GetJsonForTimeZoneFromLatLongAsync(string lat, string lon)
        {
            var url = $"http://api.geonames.org/timezoneJSON?lat={lat}&lng={lon}&username={Key}";
            var json = await http.GetStringAsync(url);
            return json;
        }

        public async Task<string> GetTimeZoneJsonFromLatLonAsync(string lat, string lon)
        {
            var url = $"https://maps.googleapis.com/maps/api/timezone/json?location={lat},{lon}&timestamp={DateTime.UtcNow}&sensor=false";
            var json = await http.GetStringAsync(url);
            return json;
        }
    }
}
