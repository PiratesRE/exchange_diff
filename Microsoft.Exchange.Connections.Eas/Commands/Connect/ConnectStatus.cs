using System;

namespace Microsoft.Exchange.Connections.Eas.Commands.Connect
{
	[Flags]
	public enum ConnectStatus
	{
		IsPermanent = 1,
		RequiresSyncKeyReset = 2,
		Success = 4,
		AutodiscoverFailed = 8
	}
}
