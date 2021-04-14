using System;

namespace System.Diagnostics.Tracing
{
	[Flags]
	[__DynamicallyInvokable]
	public enum EventKeywords : long
	{
		[__DynamicallyInvokable]
		None = 0L,
		[__DynamicallyInvokable]
		All = -1L,
		MicrosoftTelemetry = 562949953421312L,
		[__DynamicallyInvokable]
		WdiContext = 562949953421312L,
		[__DynamicallyInvokable]
		WdiDiagnostic = 1125899906842624L,
		[__DynamicallyInvokable]
		Sqm = 2251799813685248L,
		[__DynamicallyInvokable]
		AuditFailure = 4503599627370496L,
		[__DynamicallyInvokable]
		AuditSuccess = 9007199254740992L,
		[__DynamicallyInvokable]
		CorrelationHint = 4503599627370496L,
		[__DynamicallyInvokable]
		EventLogClassic = 36028797018963968L
	}
}
