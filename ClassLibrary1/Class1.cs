using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BusInfo
{
    public class MyStopInfo
    {
        private readonly IBusLocator _busLocator;
        private readonly ITimeZoneConverter _timezoneConverter;
        // src: http://stackoverflow.com/questions/5996320/net-timezoneinfo-from-olson-time-zone
        Dictionary<string, string> olsonWindowsTimes = new Dictionary<string, string>()
        {
            { "Africa/Bangui", "W. Central Africa Standard Time" },
            { "Africa/Cairo", "Egypt Standard Time" },
            { "Africa/Casablanca", "Morocco Standard Time" },
            { "Africa/Harare", "South Africa Standard Time" },
            { "Africa/Johannesburg", "South Africa Standard Time" },
            { "Africa/Lagos", "W. Central Africa Standard Time" },
            { "Africa/Monrovia", "Greenwich Standard Time" },
            { "Africa/Nairobi", "E. Africa Standard Time" },
            { "Africa/Windhoek", "Namibia Standard Time" },
            { "America/Anchorage", "Alaskan Standard Time" },
            { "America/Argentina/San_Juan", "Argentina Standard Time" },
            { "America/Asuncion", "Paraguay Standard Time" },
            { "America/Bahia", "Bahia Standard Time" },
            { "America/Bogota", "SA Pacific Standard Time" },
            { "America/Buenos_Aires", "Argentina Standard Time" },
            { "America/Caracas", "Venezuela Standard Time" },
            { "America/Cayenne", "SA Eastern Standard Time" },
            { "America/Chicago", "Central Standard Time" },
            { "America/Chihuahua", "Mountain Standard Time (Mexico)" },
            { "America/Cuiaba", "Central Brazilian Standard Time" },
            { "America/Denver", "Mountain Standard Time" },
            { "America/Fortaleza", "SA Eastern Standard Time" },
            { "America/Godthab", "Greenland Standard Time" },
            { "America/Guatemala", "Central America Standard Time" },
            { "America/Halifax", "Atlantic Standard Time" },
            { "America/Indianapolis", "US Eastern Standard Time" },
            { "America/Indiana/Indianapolis", "US Eastern Standard Time" },
            { "America/La_Paz", "SA Western Standard Time" },
            { "America/Los_Angeles", "Pacific Standard Time" },
            { "America/Mexico_City", "Mexico Standard Time" },
            { "America/Montevideo", "Montevideo Standard Time" },
            { "America/New_York", "Eastern Standard Time" },
            { "America/Noronha", "UTC-02" },
            { "America/Phoenix", "US Mountain Standard Time" },
            { "America/Regina", "Canada Central Standard Time" },
            { "America/Santa_Isabel", "Pacific Standard Time (Mexico)" },
            { "America/Santiago", "Pacific SA Standard Time" },
            { "America/Sao_Paulo", "E. South America Standard Time" },
            { "America/St_Johns", "Newfoundland Standard Time" },
            { "America/Tijuana", "Pacific Standard Time" },
            { "Antarctica/McMurdo", "New Zealand Standard Time" },
            { "Atlantic/South_Georgia", "UTC-02" },
            { "Asia/Almaty", "Central Asia Standard Time" },
            { "Asia/Amman", "Jordan Standard Time" },
            { "Asia/Baghdad", "Arabic Standard Time" },
            { "Asia/Baku", "Azerbaijan Standard Time" },
            { "Asia/Bangkok", "SE Asia Standard Time" },
            { "Asia/Beirut", "Middle East Standard Time" },
            { "Asia/Calcutta", "India Standard Time" },
            { "Asia/Colombo", "Sri Lanka Standard Time" },
            { "Asia/Damascus", "Syria Standard Time" },
            { "Asia/Dhaka", "Bangladesh Standard Time" },
            { "Asia/Dubai", "Arabian Standard Time" },
            { "Asia/Irkutsk", "North Asia East Standard Time" },
            { "Asia/Jerusalem", "Israel Standard Time" },
            { "Asia/Kabul", "Afghanistan Standard Time" },
            { "Asia/Kamchatka", "Kamchatka Standard Time" },
            { "Asia/Karachi", "Pakistan Standard Time" },
            { "Asia/Katmandu", "Nepal Standard Time" },
            { "Asia/Kolkata", "India Standard Time" },
            { "Asia/Krasnoyarsk", "North Asia Standard Time" },
            { "Asia/Kuala_Lumpur", "Singapore Standard Time" },
            { "Asia/Kuwait", "Arab Standard Time" },
            { "Asia/Magadan", "Magadan Standard Time" },
            { "Asia/Muscat", "Arabian Standard Time" },
            { "Asia/Novosibirsk", "N. Central Asia Standard Time" },
            { "Asia/Oral", "West Asia Standard Time" },
            { "Asia/Rangoon", "Myanmar Standard Time" },
            { "Asia/Riyadh", "Arab Standard Time" },
            { "Asia/Seoul", "Korea Standard Time" },
            { "Asia/Shanghai", "China Standard Time" },
            { "Asia/Singapore", "Singapore Standard Time" },
            { "Asia/Taipei", "Taipei Standard Time" },
            { "Asia/Tashkent", "West Asia Standard Time" },
            { "Asia/Tbilisi", "Georgian Standard Time" },
            { "Asia/Tehran", "Iran Standard Time" },
            { "Asia/Tokyo", "Tokyo Standard Time" },
            { "Asia/Ulaanbaatar", "Ulaanbaatar Standard Time" },
            { "Asia/Vladivostok", "Vladivostok Standard Time" },
            { "Asia/Yakutsk", "Yakutsk Standard Time" },
            { "Asia/Yekaterinburg", "Ekaterinburg Standard Time" },
            { "Asia/Yerevan", "Armenian Standard Time" },
            { "Atlantic/Azores", "Azores Standard Time" },
            { "Atlantic/Cape_Verde", "Cape Verde Standard Time" },
            { "Atlantic/Reykjavik", "Greenwich Standard Time" },
            { "Australia/Adelaide", "Cen. Australia Standard Time" },
            { "Australia/Brisbane", "E. Australia Standard Time" },
            { "Australia/Darwin", "AUS Central Standard Time" },
            { "Australia/Hobart", "Tasmania Standard Time" },
            { "Australia/Perth", "W. Australia Standard Time" },
            { "Australia/Sydney", "AUS Eastern Standard Time" },
            { "Etc/GMT", "UTC" },
            { "Etc/GMT+11", "UTC-11" },
            { "Etc/GMT+12", "Dateline Standard Time" },
            { "Etc/GMT+2", "UTC-02" },
            { "Etc/GMT-12", "UTC+12" },
            { "Europe/Amsterdam", "W. Europe Standard Time" },
            { "Europe/Athens", "GTB Standard Time" },
            { "Europe/Belgrade", "Central Europe Standard Time" },
            { "Europe/Berlin", "W. Europe Standard Time" },
            { "Europe/Brussels", "Romance Standard Time" },
            { "Europe/Budapest", "Central Europe Standard Time" },
            { "Europe/Dublin", "GMT Standard Time" },
            { "Europe/Helsinki", "FLE Standard Time" },
            { "Europe/Istanbul", "GTB Standard Time" },
            { "Europe/Kiev", "FLE Standard Time" },
            { "Europe/London", "GMT Standard Time" },
            { "Europe/Minsk", "E. Europe Standard Time" },
            { "Europe/Moscow", "Russian Standard Time" },
            { "Europe/Paris", "Romance Standard Time" },
            { "Europe/Sarajevo", "Central European Standard Time" },
            { "Europe/Warsaw", "Central European Standard Time" },
            { "Indian/Mauritius", "Mauritius Standard Time" },
            { "Pacific/Apia", "Samoa Standard Time" },
            { "Pacific/Auckland", "New Zealand Standard Time" },
            { "Pacific/Fiji", "Fiji Standard Time" },
            { "Pacific/Guadalcanal", "Central Pacific Standard Time" },
            { "Pacific/Guam", "West Pacific Standard Time" },
            { "Pacific/Honolulu", "Hawaiian Standard Time" },
            { "Pacific/Pago_Pago", "UTC-11" },
            { "Pacific/Port_Moresby", "West Pacific Standard Time" },
            { "Pacific/Tongatapu", "Tonga Standard Time" }
        };

        public MyStopInfo(IBusLocator busLocator, ITimeZoneConverter timezoneConverter)
        {
            _busLocator = busLocator;
            _timezoneConverter = timezoneConverter;
        }

        // Finds the closest stop for the given route name and gets arrival data for that stop
        // Returns a list of DateTimes for the timezone of the given lat/lon
        public async Task<List<DateTime>> GetArrivalTimesForRouteName(string routeShortName, string lat, string lon)
        {
            // find the route object for the given name and the closest stop for that route
            var (route, stop) = await GetRouteAndStopForLocation(routeShortName, lat, lon);
            var arrivalData = await GetArrivalsAndDepartures(stop.Id, route.ShortName);
            var convertToDateTime = arrivalData.Select(a => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                                               .AddMilliseconds(Convert.ToDouble(a.PredictedArrivalTime))).Take(3);
            // change from Utc to user's timezone
            TimeZoneInfo timezoneInfo = await GetTimeZoneInfo(lat, lon);
            var adjustedForTimeZone = convertToDateTime.Select(d => TimeZoneInfo.ConvertTimeFromUtc(d, timezoneInfo));

            return adjustedForTimeZone.ToList();
        }

        private async Task<TimeZoneInfo> GetTimeZoneInfo(string lat, string lon)
        {
            var timezoneJson = await _timezoneConverter.GetJsonForTimeZoneFromLatLongAsync(lat, lon);
            var timezoneOlson = JObject.Parse(timezoneJson)["timezoneId"].ToString();
            var timezoneId = olsonWindowsTimes[timezoneOlson];
            var timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            return timezoneInfo;
        }

        public async Task<List<ArrivalsAndDeparture>> GetArrivalsAndDepartures(string stopId, string routeShortName)
        {
            var json = await _busLocator.GetJsonForArrivals(stopId);
            return FindArrivalsForRoute(routeShortName, json);
        }

        // Returns the arrivals and departure data if it contains the route name 
        public List<ArrivalsAndDeparture> FindArrivalsForRoute(string routeShortName, string json)
        {
            var arrivalsAndDeparture = new List<ArrivalsAndDeparture>();
            var jobject = JObject.Parse(json);
            if (jobject["code"].ToString() == "200")
            {
                var results = jobject["data"]["entry"]["arrivalsAndDepartures"].Children().ToList();
                var searchResult = results.Where(x => CleanRouteName(x["routeShortName"].ToString()) == routeShortName);
                if (searchResult.Count() > 0)
                {
                    foreach (var s in searchResult)
                    {
                        var x = s.ToObject<ArrivalsAndDeparture>();
                        arrivalsAndDeparture.Add(x);
                    }
                }
            }
            return arrivalsAndDeparture;
        }

        // Removes the identifier from route name, e.g., ###E for Express routes
        private string CleanRouteName(string routeShortName)
        {
            return Regex.Replace(routeShortName, "[^0-9]", "");
        }

        // Finds the bus route that matches the route short name and finds the closest
        // bus stop that contains the route.
        // Returns a tuple of the user's Route and the nearest Stop in a 1800-meter radius
        public async Task<(Route, Stop)> GetRouteAndStopForLocation(string routeShortName, string lat, string lon, Direction direction = null)
        {
            var routeAndStops = await GetStopsForRoute(routeShortName, lat, lon);
            if (routeAndStops.Item1 == null || routeAndStops.Item2 == null)
            {
                throw new ArgumentException("No stops were found within a mile of your location for your bus route.");
            }

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

    public interface ITimeZoneConverter
    {
        Task<string> GetJsonForTimeZoneFromLatLongAsync(string lat, string lon);
    }

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
    }

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
