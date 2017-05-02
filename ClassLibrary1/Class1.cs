using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BusInfo
{
    public class MyStopInfo
    {
        private readonly IBusLocator _busLocator;
        public MyStopInfo(IBusLocator busLocator) => this._busLocator = busLocator;

        // Finds the bus route that matches the route short name and finds the closest
        // bus stop that contains the route.
        // Returns a tuple of the user's Route and the nearest Stop in a 1800-meter radius
        public async Task<(Route, Stop)> GetRouteAndStopForLocation(string routeShortName, string lat, string lon, Direction direction = null)
        {
            var routeAndStops = await GetStopsForRoute(routeShortName, lat, lon);
            Stop minDistStop;
            if (direction != null)
                minDistStop = FindClosestStopInDirection(direction, lat, lon, routeAndStops.Item2);
            else
                minDistStop = routeAndStops.Item2.First();

            return (routeAndStops.Item1, minDistStop);
        }

        private async Task<(Route, List<Stop>)> GetStopsForRoute(string routeShortName, string lat, string lon)
        {
            var json = await _busLocator.GetJsonForStopsFromLatLongAsync(lat, lon);
            return FindStopsForRoute(routeShortName, json);
        }

        // Retrieves all bus stops that contain a given route with route short name
        // Returns a tuple of the route and the associated stops in a 1800-meter radius 
        public (Route, List<Stop>) FindStopsForRoute(string routeShortName, string json)
        {
            var jobject = JObject.Parse(json);
            if (jobject["code"].ToString() == "200")
            {
                var routes = jobject["data"]["references"]["routes"].Children();
                var targetRoute = routes.Where(x => x["shortName"].ToString() == routeShortName).FirstOrDefault();
                if (targetRoute != null)
                {
                    var route = targetRoute.ToObject<Route>();
                    var stops = jobject["data"]["list"].Children();
                    List<Stop> stopsForRoute = new List<Stop>();
                    foreach (var s in stops)
                    {
                        var routeIds = s["routeIds"];
                        foreach (var rId in routeIds)
                        {
                            if (rId.ToString() == route.Id)
                            {
                                stopsForRoute.Add(s.ToObject<Stop>());
                            }
                        }
                    }
                    return (route, stopsForRoute);
                }
            }
            return (null, null);
        }

        // Determines the closest stop to the given latitude and longitude
        private Stop FindClosestStopInDirection(Direction direction, string lat, string lon, List<Stop> stopsForRoute)
        {
            var stopWithMinDist = stopsForRoute?.Where(s => s.Direction.Equals(direction))
                                      ?.Select(d => (stop: d, distance: CalculateDistance(lat, lon, d.Lat, d.Lon)))
                                      ?.OrderBy(t => t.distance).First();

            return stopWithMinDist?.stop;
        }

        // Uses distance formula to find distance between two points
        private double CalculateDistance(string lat1, string lon1, double lat2, double lon2)
        {
            return Math.Sqrt(Math.Pow(double.Parse(lat1) - lat2, 2) + Math.Pow(double.Parse(lon1) - lon2, 2));
        }
    }
    public class BusLocator : IBusLocator
    {
        HttpClient http = new HttpClient();
        private const string Key = "TEST";

        public async Task<string> GetJsonForArrivals(string stopId)
        {
            var url = $"http://api.pugetsound.onebusaway.org/api/where/arrivals-and-departures-for-stop/{stopId}.json?key={Key}";
            var json = await http.GetStringAsync(url);
            return json;
        }
        public async Task<string> GetJsonForStopsFromLatLongAsync(string lat, string lon)
        {
            var url = $"http://api.pugetsound.onebusaway.org/api/where/stops-for-location.json?key={Key}&lat={lat}&lon={lon}&radius=1800&maxCount=50";
            var json = await http.GetStringAsync(url);
            return json;
        }
    }
    public class MockBusLocator : IBusLocator
    {
        private static string LoadJson(string file) => File.ReadAllText(file);

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
    public class ArrivalsAndDeparture
    {
        public bool ArrivalEnabled { get; set; }
        public int BlockTripSequence { get; set; }
        public bool DepartureEnabled { get; set; }
        public double DistanceFromStop { get; set; }
        public object Frequency { get; set; }
        public object LastUpdateTime { get; set; }
        public int NumberOfStopsAway { get; set; }
        public bool Predicted { get; set; }
        public object PredictedArrivalInterval { get; set; }
        public object PredictedArrivalTime { get; set; }
        public object PredictedDepartureInterval { get; set; }
        public object PredictedDepartureTime { get; set; }
        public string RouteId { get; set; }
        public string RouteLongName { get; set; }
        public string RouteShortName { get; set; }
        public object ScheduledArrivalInterval { get; set; }
        public object ScheduledArrivalTime { get; set; }
        public object ScheduledDepartureInterval { get; set; }
        public object ScheduledDepartureTime { get; set; }
        public object ServiceDate { get; set; }
        public List<object> SituationIds { get; set; }
        public string Status { get; set; }
        public string StopId { get; set; }
        public int StopSequence { get; set; }
        public int TotalStopsInTrip { get; set; }
        public string TripHeadsign { get; set; }
        public string TripId { get; set; }
        public TripStatus TripStatus { get; set; }
        public string VehicleId { get; set; }
    }
    public class Direction : IEquatable<Direction>
    {
        public string Compass { get; set; }
        public Direction(string v) => Compass = v;
        //add equals override. misspell Length and correct it.show intellisense

        public override bool Equals(object obj) => Equals(obj as Direction);
        public bool Equals(Direction other)
        {
            if (other.Compass.Length == 1)
            {
                return Compass.Contains(other.Compass);
            }
            else if (other.Compass.Length == 2)
            {
                return Compass == other.Compass;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode() => base.GetHashCode();
    }
    public class DirectionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Direction);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader is JTokenReader jtokenreader)
            {
                return new Direction(jtokenreader.CurrentToken.ToString());
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
    public class LastKnownLocation
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
    public class Position
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
    public class Route
    {
        public Route(string agencyId, string color, string description, string id, string longName, string shortName, string textColor, int type, string url)
        {
            AgencyId = agencyId;
            Color = color;
            Description = description;
            Id = id;
            LongName = longName;
            ShortName = shortName;
            TextColor = textColor;
            Type = type;
            Url = url;
        }

        public string AgencyId { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public string TextColor { get; set; }
        public int Type { get; set; }
        public string Url { get; set; }
    }
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
    public class TripStatus
    {
        public string ActiveTripId { get; set; }
        public int BlockTripSequence { get; set; }
        public string ClosestStop { get; set; }
        public int ClosestStopTimeOffset { get; set; }
        public double DistanceAlongTrip { get; set; }
        public object Frequency { get; set; }
        public int LastKnownDistanceAlongTrip { get; set; }
        public LastKnownLocation LastKnownLocation { get; set; }
        public int LastKnownOrientation { get; set; }
        public object LastLocationUpdateTime { get; set; }
        public object LastUpdateTime { get; set; }
        public string NextStop { get; set; }
        public int NextStopTimeOffset { get; set; }
        public double Orientation { get; set; }
        public string Phase { get; set; }
        public Position Position { get; set; }
        public bool Predicted { get; set; }
        public int ScheduleDeviation { get; set; }
        public double ScheduledDistanceAlongTrip { get; set; }
        public object ServiceDate { get; set; }
        public List<object> SituationIds { get; set; }
        public string Status { get; set; }
        public double TotalDistanceAlongTrip { get; set; }
        public string VehicleId { get; set; }
    }
    public interface IBusLocator
    {
        Task<string> GetJsonForStopsFromLatLongAsync(string lat, string lon);
        Task<string> GetJsonForArrivals(string stopId);
    }

}
