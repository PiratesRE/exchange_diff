using System;

namespace Microsoft.Exchange.Connections.Eas.Commands.Autodiscover
{
	[Flags]
	public enum AutodiscoverStatus
	{
		Success = 1,
		ProtocolError = 4098,
		LowOrderByte = 255,
		EveryStepFailed = 260,
		StatusOutOfRange = 511,
		TransientError = 256,
		PermanentError = 4096
	}
}
