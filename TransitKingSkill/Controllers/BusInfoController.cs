using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AlexaSkillsKit.Speechlet;
using System.Net.Http;
using AlexaSkillsKit.Authentication;
using AlexaSkillsKit.Json;
using Newtonsoft.Json.Linq;

namespace TransitKingSkill.Controllers
{
    [Route("api/[controller]")]
    public class BusInfoController : Controller
    {
        [HttpPost]
        public async Task<HttpResponseMessage> SampleSession()
        {
            var speechlet = new SampleSessionSpeechlet();
            return await speechlet.GetResponseAsync(Request);
        }
    }
}
