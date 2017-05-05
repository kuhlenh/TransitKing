using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Alexa.NET;
using AlexaSkillsKit;
using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit.Slu;
using AlexaSkillsKit.UI;


namespace TransitKingSkill.Controllers
{
    [Route("api/[controller]")]
    public class HelloWorldController : Controller
    {
        // GET api/helloworld
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/helloworld/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/helloworld
        [HttpPost]
        public void Post([FromBody]string value)
        {
            var speechlet = new SampleSessionSpeechlet();
            return speechlet.GetResponse(Request);
        }

        // PUT api/helloworld/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/helloworld/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }


}
