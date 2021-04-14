using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public enum SpeechLoggerProcessType
	{
		BeginRequest,
		RequestCompleted,
		BeginReadAudio,
		EndReadAudio,
		HandleRecoResults
	}
}
