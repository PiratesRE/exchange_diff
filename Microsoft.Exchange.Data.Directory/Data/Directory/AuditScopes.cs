using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Flags]
	public enum AuditScopes
	{
		Admin = 1,
		Delegate = 2,
		Owner = 4
	}
}
