using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class OutboundCallDetailsEventArgs : EventArgs
	{
		internal OutboundCallDetailsEventArgs(Exception error, UMCallInfoEx exInfo, object state)
		{
			this.Error = error;
			this.CallInfoEx = exInfo;
			this.PlatformState = state;
			this.CallOutcome = ((error == null && exInfo.EndResult == UMOperationResult.Success) ? OutboundCallDetailsEventArgs.OutboundCallOutcome.Success : OutboundCallDetailsEventArgs.OutboundCallOutcome.Failure);
		}

		internal OutboundCallDetailsEventArgs.OutboundCallOutcome CallOutcome { get; private set; }

		internal Exception Error { get; private set; }

		internal UMCallInfoEx CallInfoEx { get; private set; }

		internal object PlatformState { get; private set; }

		internal enum OutboundCallOutcome
		{
			Success,
			Failure
		}
	}
}
