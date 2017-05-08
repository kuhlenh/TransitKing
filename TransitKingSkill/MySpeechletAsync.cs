using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using AlexaSkillsKit.Authentication;
using AlexaSkillsKit.Json;
using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace TransitKingSkill
{
    public abstract class MySpeechletAsync : SpeechletAsync
    {
        /// <summary>
        /// Processes Alexa request AND validates request signature
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public virtual async Task<HttpResponseMessage> GetResponseAsync(HttpRequest httpRequest)
        {
            SpeechletRequestValidationResult validationResult = SpeechletRequestValidationResult.OK;
            DateTime now = DateTime.UtcNow; // reference time for this request

            string chainUrl = null;
            if (String.IsNullOrEmpty(chainUrl = httpRequest.Headers[Sdk.SIGNATURE_CERT_URL_REQUEST_HEADER]))
            {
                validationResult = validationResult | SpeechletRequestValidationResult.NoCertHeader;
            }

            string signature = null;
            if (String.IsNullOrEmpty(signature = httpRequest.Headers[Sdk.SIGNATURE_REQUEST_HEADER]))
            {
                validationResult = validationResult | SpeechletRequestValidationResult.NoSignatureHeader;
            }

            var alexaBytes = await ReadBodyToArray(httpRequest);
            // attempt to verify signature only if we were able to locate certificate and signature headers
            if (validationResult == SpeechletRequestValidationResult.OK)
            {
                if (!(await SpeechletRequestSignatureVerifier.VerifyRequestSignatureAsync(alexaBytes, signature, chainUrl)))
                {
                    validationResult = validationResult | SpeechletRequestValidationResult.InvalidSignature;
                }
            }

            SpeechletRequestEnvelope alexaRequest = null;
            try
            {
                var alexaContent = UTF8Encoding.UTF8.GetString(alexaBytes, 0, alexaBytes.Length);
                alexaRequest = SpeechletRequestEnvelope.FromJson(alexaContent);
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                validationResult = validationResult | SpeechletRequestValidationResult.InvalidJson;
            }
            catch (InvalidCastException)
            {
                validationResult = validationResult | SpeechletRequestValidationResult.InvalidJson;
            }

            // attempt to verify timestamp only if we were able to parse request body
            if (alexaRequest != null)
            {
                if (!SpeechletRequestTimestampVerifier.VerifyRequestTimestamp(alexaRequest, now))
                {
                    validationResult = validationResult | SpeechletRequestValidationResult.InvalidTimestamp;
                }
            }

            if (alexaRequest == null || !OnRequestValidation(validationResult, now, alexaRequest))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = validationResult.ToString()
                };
            }

            string alexaResponse = await DoProcessRequestAsync(alexaRequest);

            HttpResponseMessage httpResponse;
            if (alexaResponse == null)
            {
                httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            else
            {
                httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
                httpResponse.Content = new StringContent(alexaResponse, Encoding.UTF8, "application/json");
                Debug.WriteLine(httpResponse.ToLogString());
            }

            return httpResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestEnvelope"></param>
        /// <returns></returns>
        private async Task<string> DoProcessRequestAsync(SpeechletRequestEnvelope requestEnvelope)
        {
            Session session = requestEnvelope.Session;
            SpeechletResponse response = null;

            // process launch request
            if (requestEnvelope.Request is LaunchRequest)
            {
                var request = requestEnvelope.Request as LaunchRequest;
                if (requestEnvelope.Session.IsNew)
                {
                    await OnSessionStartedAsync(
                        new SessionStartedRequest(request.RequestId, request.Timestamp), session);
                }
                response = await OnLaunchAsync(request, session);
            }

            // process intent request
            else if (requestEnvelope.Request is IntentRequest)
            {
                var request = requestEnvelope.Request as IntentRequest;

                // Do session management prior to calling OnSessionStarted and OnIntentAsync 
                // to allow dev to change session values if behavior is not desired
                DoSessionManagement(request, session);

                if (requestEnvelope.Session.IsNew)
                {
                    await OnSessionStartedAsync(
                        new SessionStartedRequest(request.RequestId, request.Timestamp), session);
                }
                response = await OnIntentAsync(request, session);
            }

            // process session ended request
            else if (requestEnvelope.Request is SessionEndedRequest)
            {
                var request = requestEnvelope.Request as SessionEndedRequest;
                await OnSessionEndedAsync(request, session);
            }

            var responseEnvelope = new SpeechletResponseEnvelope
            {
                Version = requestEnvelope.Version,
                Response = response,
                SessionAttributes = session.Attributes
            };
            return responseEnvelope.ToJson();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DoSessionManagement(IntentRequest request, Session session)
        {
            if (session.IsNew)
            {
                session.Attributes[Session.INTENT_SEQUENCE] = request.Intent.Name;
            }
            else
            {
                // if the session was started as a result of a launch request 
                // a first intent isn't yet set, so set it to the current intent
                if (!session.Attributes.ContainsKey(Session.INTENT_SEQUENCE))
                {
                    session.Attributes[Session.INTENT_SEQUENCE] = request.Intent.Name;
                }
                else
                {
                    session.Attributes[Session.INTENT_SEQUENCE] += Session.SEPARATOR + request.Intent.Name;
                }
            }

            // Auto-session management: copy all slot values from current intent into session
            foreach (var slot in request.Intent.Slots.Values)
            {
                if (!String.IsNullOrEmpty(slot.Value)) session.Attributes[slot.Name] = slot.Value;
            }
        }

        private async static Task<byte[]> ReadBodyToArray(HttpRequest httpRequest)
        {
            using (var memorystream = new MemoryStream())
            {
                await httpRequest.Body.CopyToAsync(memorystream);
                return memorystream.ToArray();
            }
        }


      
    }
}