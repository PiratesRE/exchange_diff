using System;

namespace Microsoft.Exchange.Transport
{
	internal interface IQueueQuotaMailItem
	{
		string ExoAccountForest { get; }

		Guid ExternalOrganizationId { get; }

		string OriginalFromAddress { get; }

		QueueQuotaTrackingBits QueueQuotaTrackingBits { get; }
	}
}
