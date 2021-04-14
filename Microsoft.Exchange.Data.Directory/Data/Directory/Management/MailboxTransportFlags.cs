using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Flags]
	internal enum MailboxTransportFlags
	{
		None = 0,
		ConnectivityLogEnabled = 1,
		ContentConversionTracingEnabled = 2,
		PipelineTracingEnabled = 4,
		MailboxDeliverySmtpUtf8Enabled = 8
	}
}
