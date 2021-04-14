using System;

namespace Microsoft.Exchange.MessagingPolicies.AddressRewrite
{
	internal abstract class IBlock
	{
		internal abstract int Written { get; }

		internal abstract int Free { get; }
	}
}
