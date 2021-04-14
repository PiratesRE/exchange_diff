using System;

namespace Microsoft.Exchange.Security
{
	[Flags]
	internal enum DenyRuleAuthenticationType : short
	{
		Anonymous = 1,
		Basic = 2,
		Digest = 4,
		Forms = 8,
		Windows = 16
	}
}
