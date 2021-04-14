using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	internal enum SmtpPermission
	{
		None = 0,
		Submit = 1,
		Relay = 2,
		SendAs = 4,
		Default = 1
	}
}
