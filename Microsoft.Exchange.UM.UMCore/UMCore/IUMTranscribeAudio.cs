using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IUMTranscribeAudio
	{
		UMSubscriber TranscriptionUser { get; }

		string AttachmentPath { get; }

		TimeSpan Duration { get; }

		ITranscriptionData TranscriptionData { get; set; }

		bool EnableTopNGrammar { get; }
	}
}
