using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum MailboxPolicyFlagValues
	{
		None = 0,
		IsDefault = 1,
		IsDefaultArbitrationMailbox = 2
	}
}
