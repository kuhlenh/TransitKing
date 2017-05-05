using System.Threading.Tasks;

namespace BusInfo
{
    public interface ITimeZoneConverter
    {
        Task<string> GetJsonForTimeZoneFromLatLongAsync(string lat, string lon);
    }
}
