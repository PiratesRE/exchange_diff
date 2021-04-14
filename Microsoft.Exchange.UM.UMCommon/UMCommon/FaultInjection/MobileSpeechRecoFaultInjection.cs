using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon.FaultInjection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MobileSpeechRecoFaultInjection
	{
		internal static bool TryCreateException(string exceptionType, ref Exception exception)
		{
			return false;
		}

		internal const uint MobileSpeechRecoRecognitionDelay = 4112919869U;
	}
}
