using System;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Flags]
	public enum AllowedServices
	{
		None = 0,
		IMAP = 1,
		POP = 2,
		UM = 4,
		IIS = 8,
		SMTP = 16,
		Federation = 32,
		UMCallRouter = 64
	}
}
