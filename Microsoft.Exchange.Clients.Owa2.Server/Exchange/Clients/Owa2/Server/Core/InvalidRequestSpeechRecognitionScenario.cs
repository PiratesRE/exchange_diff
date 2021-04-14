using System;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class InvalidRequestSpeechRecognitionScenario : SpeechRecognitionScenarioBase
	{
		public InvalidRequestSpeechRecognitionScenario(SpeechRecognitionProcessor.SpeechHttpStatus status) : base(null, null)
		{
			ValidateArgument.NotNull(status, "status");
			this.status = status;
		}

		internal override void StartRecoRequestAsync(SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate callback)
		{
			ValidateArgument.NotNull(callback, "callback is null");
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "InvalidRequestSpeechRecognitionScenarios.StartRecoRequestAsync");
			callback(new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs(string.Empty, this.status));
		}

		internal override void SetAudio(byte[] audioBytes)
		{
		}

		protected override void InitializeSpeechRecognitions(RequestParameters requestParameters)
		{
		}

		protected override SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs GetFormattedResultsForHighestConfidenceProcessor(SpeechRecognition recognitionWithHighestConfidence)
		{
			throw new NotSupportedException();
		}

		private SpeechRecognitionProcessor.SpeechHttpStatus status;
	}
}
