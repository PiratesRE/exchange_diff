using System;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	internal interface IMessageDepotItem
	{
		object MessageObject { get; }

		TransportMessageId Id { get; }

		MessageEnvelope MessageEnvelope { get; }

		bool IsPoison { get; }

		bool IsSuspended { get; }

		DateTime DeferUntil { get; set; }

		DateTime ExpirationTime { get; }

		DateTime ArrivalTime { get; }

		MessageDepotItemStage Stage { get; }

		bool IsDelayDsnGenerated { get; set; }

		object GetProperty(string propertyName);

		void Dehydrate();
	}
}
