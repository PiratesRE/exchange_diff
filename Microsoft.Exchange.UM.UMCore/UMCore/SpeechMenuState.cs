using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal enum SpeechMenuState
	{
		Main,
		Help,
		Mumble1,
		Mumble2,
		Silence1,
		Silence2,
		SpeechError,
		InvalidCommand,
		DtmfFallback,
		Goodbye,
		GoodbyeConfirm
	}
}
