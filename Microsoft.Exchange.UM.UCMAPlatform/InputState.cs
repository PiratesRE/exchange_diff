using System;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	public enum InputState
	{
		NotAllowed,
		NotStarted,
		StartedNotComplete,
		StartedCompleteNotAmbiguous,
		StartedCompleteAmbiguous
	}
}
