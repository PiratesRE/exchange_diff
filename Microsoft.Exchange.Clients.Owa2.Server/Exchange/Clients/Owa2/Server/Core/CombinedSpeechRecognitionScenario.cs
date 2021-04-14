using System;
using System.Collections.Generic;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class CombinedSpeechRecognitionScenario : SpeechRecognitionScenarioBase
	{
		public CombinedSpeechRecognitionScenario(RequestParameters requestParameters, UserContext userContext) : base(requestParameters, userContext)
		{
			ValidateArgument.NotNull(requestParameters, "requestParameters is null");
			ValidateArgument.NotNull(userContext, "userContext is null");
		}

		protected override void InitializeSpeechRecognitions(RequestParameters requestParameters)
		{
			base.RecognitionHelpers = new Dictionary<MobileSpeechRecoRequestType, SpeechRecognition>();
			RequestParameters requestParameters2 = SpeechRecognitionScenarioBase.CreateRequestParameters(MobileSpeechRecoRequestType.FindInPersonalContacts, requestParameters);
			SpeechRecognition speechRecognition = new LocalSpeechRecognition(requestParameters2, SpeechRecognitionResultsPriority.Wait);
			base.RecognitionHelpers.Add(speechRecognition.RequestType, speechRecognition);
			RequestParameters requestParameters3 = SpeechRecognitionScenarioBase.CreateRequestParameters(MobileSpeechRecoRequestType.FindInGAL, requestParameters);
			speechRecognition = new FindInGALSpeechRecognition(requestParameters3, SpeechRecognitionResultsPriority.Wait);
			base.RecognitionHelpers.Add(speechRecognition.RequestType, speechRecognition);
			RequestParameters requestParameters4 = SpeechRecognitionScenarioBase.CreateRequestParameters(MobileSpeechRecoRequestType.StaticGrammarsCombined, requestParameters);
			speechRecognition = new LocalSpeechRecognition(requestParameters4, SpeechRecognitionResultsPriority.Immediate);
			base.RecognitionHelpers.Add(speechRecognition.RequestType, speechRecognition);
		}

		protected override SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs GetFormattedResultsForHighestConfidenceProcessor(SpeechRecognition recognitionWithHighestConfidence)
		{
			if (recognitionWithHighestConfidence.ResultType == MobileSpeechRecoResultType.None)
			{
				return recognitionWithHighestConfidence.Results;
			}
			switch (recognitionWithHighestConfidence.RequestType)
			{
			case MobileSpeechRecoRequestType.FindInGAL:
			case MobileSpeechRecoRequestType.FindInPersonalContacts:
			{
				SpeechRecognition galRecoHelper = base.RecognitionHelpers[MobileSpeechRecoRequestType.FindInGAL];
				SpeechRecognition personalContactsRecoHelper = base.RecognitionHelpers[MobileSpeechRecoRequestType.FindInPersonalContacts];
				return SpeechRecognitionUtils.GetCombinedPeopleSearchResult(galRecoHelper, personalContactsRecoHelper, recognitionWithHighestConfidence.ResultType);
			}
			case MobileSpeechRecoRequestType.StaticGrammarsCombined:
				return recognitionWithHighestConfidence.Results;
			default:
				throw new ArgumentOutOfRangeException("RequestType", recognitionWithHighestConfidence.RequestType, "Invalid parameter");
			}
		}
	}
}
