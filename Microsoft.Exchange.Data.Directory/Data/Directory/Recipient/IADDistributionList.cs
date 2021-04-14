using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal interface IADDistributionList
	{
		ADObjectId ManagedBy { get; set; }

		MultiValuedProperty<ADObjectId> Members { get; }

		DeliveryReportsReceiver SendDeliveryReportsTo { get; set; }

		bool SendOofMessageToOriginatorEnabled { get; set; }

		ADPagedReader<ADRawEntry> Expand(int pageSize, params PropertyDefinition[] propertiesToReturn);

		ADPagedReader<TEntry> Expand<TEntry>(int pageSize, params PropertyDefinition[] propertiesToReturn) where TEntry : MiniRecipient, new();

		ADPagedReader<ADRecipient> Expand(int pageSize);
	}
}
