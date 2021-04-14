using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SubscriptionProxyPropertyDefinition : PropertyDefinition
	{
		public SubscriptionProxyPropertyDefinition(string name, Type type) : base(name, type)
		{
		}
	}
}
