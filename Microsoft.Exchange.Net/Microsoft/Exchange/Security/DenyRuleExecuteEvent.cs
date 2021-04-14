using System;

namespace Microsoft.Exchange.Security
{
	[Flags]
	internal enum DenyRuleExecuteEvent : short
	{
		PreAuthentication = 1,
		PostAuthentication = 2,
		Always = 3
	}
}
