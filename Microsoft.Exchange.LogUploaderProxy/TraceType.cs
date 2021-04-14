using System;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public enum TraceType
	{
		None,
		DebugTrace,
		Debug = 1,
		Warning,
		WarningTrace = 2,
		Error,
		ErrorTrace = 3,
		FatalTrace,
		Fatal = 4,
		Info,
		InfoTrace = 5,
		Performance,
		PerformanceTrace = 6,
		Function,
		FunctionTrace = 7,
		PfdTrace,
		Pfd = 8,
		FaultI,
		FaultInjection = 9
	}
}
