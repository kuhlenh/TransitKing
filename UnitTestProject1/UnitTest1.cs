using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusInfo;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        MyStopInfo busInfo = new MyStopInfo(new MockBusLocator());
        (string lat, string lon) conventionCenter = ("47.611959", "-122.332893");
        (string lat, string lon) microsoftCampus = ("47.639905", "-122.125485");
        string busRoute = "545";
        Direction direction = new Direction("N");

        [TestMethod]
        public async Task TestGetRouteAndStopForLocation()
        {
            var actual = await busInfo.GetRouteAndStopForLocation(busRoute, conventionCenter.lat, conventionCenter.lon, direction);
            var expectedRoute = new Route("40", "", "Redmond Seattle", "40_100236", "Redmond - Seattle", "545", "", 3, "http://www.soundtransit.org/Schedules/ST-Express-Bus/545");
            var expectedStop = new Stop("700", new Direction("NW"), "1_700", 47.610951, 0, -122.33725, "4th Ave & Pike St", new List<string>(), "UNKNOWN");

            Assert.AreEqual(actual.Item2.Id, expectedStop.Id);
            Assert.AreEqual(actual.Item1.Id, expectedRoute.Id);  
        }

        [TestMethod]
        public async Task TestGetRouteAndStopNoDirection()
        {
            var actual = await busInfo.GetRouteAndStopForLocation(busRoute, conventionCenter.lat, conventionCenter.lon);
            var expectedRoute = new Route("40", "", "Redmond Seattle", "40_100236", "Redmond - Seattle", "545", "", 3, "http://www.soundtransit.org/Schedules/ST-Express-Bus/545");
            var expectedStop = new Stop("700", new Direction("NW"), "1_700", 47.610951, 0, -122.33725, "4th Ave & Pike St", new List<string>(), "UNKNOWN");

            Assert.AreEqual(actual.Item2.Id, expectedStop.Id);
            Assert.AreEqual(actual.Item1.Id, expectedRoute.Id);
        }

        [TestMethod]
        public async Task TestGetRouteAndStopMissing()
        {
            var actual = await busInfo.GetRouteAndStopForLocation("541", conventionCenter.lat, conventionCenter.lon);
            Assert.AreEqual(actual.Item2, null);
            Assert.AreEqual(actual.Item1, null);
        }
    }
}
