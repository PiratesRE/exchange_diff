using System;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal static class InputStateHelper
	{
		public static bool IsAllowed(InputState state)
		{
			return state != InputState.NotAllowed;
		}

		public static bool IsStarted(InputState state)
		{
			return InputStateHelper.IsAllowed(state) && state != InputState.NotStarted;
		}

		public static bool IsComplete(InputState state)
		{
			return InputStateHelper.IsStarted(state) && state != InputState.StartedNotComplete;
		}

		public static bool IsUnambiguous(InputState state)
		{
			return state == InputState.StartedCompleteNotAmbiguous;
		}
	}
}
