using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusInfo;
using Microsoft.AspNetCore.Mvc;

namespace Skill.Controllers
{
    [Route("api/[controller]")]
    public class BusController : Controller
    {
        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
            var locator = new BusLocator();
            var timezoneConverter = new TimeZoneConverter();
            MyStopInfo busInfo = new MyStopInfo(locator, timezoneConverter);


        }
    }
}
