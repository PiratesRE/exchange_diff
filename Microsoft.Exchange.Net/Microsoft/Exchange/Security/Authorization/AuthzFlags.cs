using System;

namespace Microsoft.Exchange.Security.Authorization
{
	[Flags]
	public enum AuthzFlags
	{
		Default = 0,
		AuthzSkipTokenGroups = 2,
		AuthzRequireS4ULogon = 4
	}
}
