using System.Collections.Generic;
using Newtonsoft.Json;

namespace BusInfo
{
    public class Stop
    {
        public Stop(string code, Direction direction, string id, double lat, int locationType, double lon, string name, List<string> routeIds, string wheelchairBoarding)
        {
            Code = code;
            Direction = direction;
            Id = id;
            Lat = lat;
            LocationType = locationType;
            Lon = lon;
            Name = name;
            RouteIds = routeIds;
            WheelchairBoarding = wheelchairBoarding;
        }

        public string Code { get; set; }

        [JsonConverter(typeof(DirectionConverter))]
        public Direction Direction { get; set; }

        public string Id { get; set; }
        public double Lat { get; set; }
        public int LocationType { get; set; }
        public double Lon { get; set; }
        public string Name { get; set; }
        public List<string> RouteIds { get; set; }
        public string WheelchairBoarding { get; set; }
    }
}
