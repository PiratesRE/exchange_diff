using System;

namespace Microsoft.Exchange.Security.Authorization
{
	[Flags]
	internal enum ResourceManagerFlags
	{
		None = 0,
		NoAudit = 1,
		InitializeUnderImpersonation = 2
	}
}
