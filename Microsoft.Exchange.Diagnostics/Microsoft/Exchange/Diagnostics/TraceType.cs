using System;

namespace Microsoft.Exchange.Diagnostics
{
	public enum TraceType
	{
		None,
		DebugTrace,
		Debug = 1,
		WarningTrace,
		Warning = 2,
		ErrorTrace,
		Error = 3,
		FatalTrace,
		Fatal = 4,
		InfoTrace,
		Info = 5,
		PerformanceTrace,
		Performance = 6,
		FunctionTrace,
		Function = 7,
		PfdTrace,
		Pfd = 8,
		FaultInjection,
		FaultI = 9
	}
}
