using System;

namespace Microsoft.Exchange.Search.Core.Diagnostics
{
	[Flags]
	internal enum DiagnosticsLoggingTag
	{
		None = 0,
		Informational = 1,
		Warnings = 2,
		Failures = 4
	}
}
