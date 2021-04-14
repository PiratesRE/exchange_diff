using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal interface IVersionedItem
	{
		string ID { get; }

		DateTime Version { get; }
	}
}
