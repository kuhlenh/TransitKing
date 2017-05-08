using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit;
using AlexaSkillsKit.UI;
using AlexaSkillsKit.Slu;
using Microsoft.Extensions.Logging;
using NLog;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.IO;
using BusInfo;


namespace TransitKingSkill
{
    public class BusSpeechlet : MySpeechletAsync
    {
        private const string BusKey = "Bus";
        private const string BusForIntent = "BusForIntent";
        MyStopInfo _busInfo = new MyStopInfo(new BusLocator(), new TimeZoneConverter());
        (string lat, string lon) conventionCenter = ("47.611959", "-122.332893");

        public override async Task<SpeechletResponse> OnIntentAsync(IntentRequest intentRequest, Session session)
        {
            // Get intent from the request object.
            var intent = intentRequest.Intent;
            var intentName = intent?.Name;

            // Note: If the session is started with an intent, no welcome message will be rendered;
            // rather, the intent specific response will be returned.
            if (BusForIntent.Equals(intentName))
            {
                return await GetBusResponse(intent, session);
            }

            throw new SpeechletException("Invalid Intent");
        }

        private async Task<SpeechletResponse> GetBusResponse(Intent intent, Session session)
        {
            // Retrieve date from the intent slot
            var busSlot = intent.Slots[BusKey];

            string output;
            if (busSlot != null)
            {
                string busRoute = busSlot.ToString();
                var times = await _busInfo.GetArrivalTimesForRouteName(busRoute, conventionCenter.lat, conventionCenter.lon);

                if (times != null)
                {
                    output = $"The next {busSlot} bus arrives at ";
                    foreach (var time in times)
                    {
                        output += time;
                    }
                }
                else
                {
                    output = $"Sorry, no bus info is available for {busSlot}";
                }
            }
            else
            {
                output = $"Sorry, no bus info is available for {busSlot}";
            }

            // Here we are setting shouldEndSession to false to not end the session and
            // prompt the user for input
            return BuildSpeechletResponse(intent.Name, output, false);
        }

        public override Task<SpeechletResponse> OnLaunchAsync(LaunchRequest launchRequest, Session session)
        {
            return Task.FromResult(GetWelcomeResponse());
        }

        private SpeechletResponse GetWelcomeResponse()
        {
            var output = "Welcome to the Transit King app. Please request a bus number near you.";
            return BuildSpeechletResponse("Welcome", output, false);
        }

        private SpeechletResponse BuildSpeechletResponse(string title, string output, bool shouldEndSession)
        {
            // Create the Simple card content
            var card = new SimpleCard
            {
                Title = $"SessionSpeechlet - {title}",
                Content = $"SessionSpeechlet - {output}"
            };

            // Create the plain text output
            var speech = new PlainTextOutputSpeech { Text = output };

            // Create the speechlet response
            var response = new SpeechletResponse
            {
                ShouldEndSession = shouldEndSession,
                OutputSpeech = speech,
                Card = card
            };
            return response;
        }

        public override Task OnSessionEndedAsync(SessionEndedRequest sessionEndedRequest, Session session)
        {
            throw new NotImplementedException();
        }

        public override Task OnSessionStartedAsync(SessionStartedRequest sessionStartedRequest, Session session)
        {
            throw new NotImplementedException();
        }
    }
}
