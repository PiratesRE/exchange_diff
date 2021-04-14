using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum AdminAuditLogFlags
	{
		None = 0,
		AdminAuditLogEnabled = 1,
		TestCmdletLoggingEnabled = 2,
		CaptureDetailsEnabled = 4
	}
}
