using System;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	[Flags]
	internal enum ProtocolLoggingTag
	{
		None = 0,
		ConnectDisconnect = 1,
		Rops = 2,
		OperationSpecific = 4,
		ApplicationData = 8,
		Failures = 16,
		Logon = 32,
		Throttling = 64,
		Warnings = 128
	}
}
