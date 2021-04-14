using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class EmailPeopleSpeechRecognitionResultHandler : FindPeopleSpeechRecognitionResultHandler
	{
		public EmailPeopleSpeechRecognitionResultHandler(RequestParameters parameters, UserContext userContext, HttpContext httpContext) : base(parameters, userContext, httpContext)
		{
		}

		public override void ProcessAndFormatSpeechRecognitionResults(string result, out string jsonResponse, out SpeechRecognitionProcessor.SpeechHttpStatus httpStatus)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string>((long)this.GetHashCode(), "Entering EmailPeopleSearchRecognitionResultHandler.ProcessAndFormatSpeechRecognitionResults with results '{0}'", result);
			jsonResponse = null;
			httpStatus = SpeechRecognitionProcessor.SpeechHttpStatus.Success;
			List<Persona> uniquePersonaList = base.GetUniquePersonaList(result);
			List<Persona> list = new List<Persona>();
			foreach (Persona persona in uniquePersonaList)
			{
				if (persona.EmailAddresses != null && persona.EmailAddresses.Length > 0)
				{
					list.Add(persona);
				}
			}
			if (list.Count == 0)
			{
				httpStatus = SpeechRecognitionProcessor.SpeechHttpStatus.NoContactWithEmailAddress;
				return;
			}
			jsonResponse = SpeechRecognitionResultHandler.JsonSerialize(list.ToArray());
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string>((long)this.GetHashCode(), "Persona array json:{0}", jsonResponse);
		}
	}
}
