using System;
using System.Collections.Generic;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class SingleSpeechRecognitionScenario : SpeechRecognitionScenarioBase
	{
		public SingleSpeechRecognitionScenario(RequestParameters requestParameters, UserContext userContext) : base(requestParameters, userContext)
		{
			ValidateArgument.NotNull(requestParameters, "requestParameters is null");
			ValidateArgument.NotNull(userContext, "userContext is null");
		}

		protected override void InitializeSpeechRecognitions(RequestParameters requestParameters)
		{
			base.RecognitionHelpers = new Dictionary<MobileSpeechRecoRequestType, SpeechRecognition>();
			SpeechRecognition speechRecognition = new LocalSpeechRecognition(requestParameters, SpeechRecognitionResultsPriority.Immediate);
			base.RecognitionHelpers.Add(speechRecognition.RequestType, speechRecognition);
		}

		protected override SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs GetFormattedResultsForHighestConfidenceProcessor(SpeechRecognition recognitionWithHighestConfidence)
		{
			return recognitionWithHighestConfidence.Results;
		}
	}
}
