using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Exchange.Data.Transport.Email;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class DeliverableMailItem
	{
		internal DeliverableMailItem()
		{
		}

		public abstract string OriginalAuthenticator { get; }

		public abstract DateTime DateTimeReceived { get; }

		public abstract string EnvelopeId { get; }

		public abstract RoutingAddress FromAddress { get; }

		public abstract string OriginatingDomain { get; }

		public abstract EmailMessage Message { get; }

		public abstract long MimeStreamLength { get; }

		internal abstract object RecipientCache { get; }

		public abstract ReadOnlyEnvelopeRecipientCollection Recipients { get; }

		public abstract DsnFormatRequested DsnFormatRequested { get; }

		public abstract DeliveryPriority DeliveryPriority { get; }

		public abstract DeliveryMethod InboundDeliveryMethod { get; }

		public abstract bool MustDeliver { get; }

		public abstract Guid TenantId { get; }

		internal abstract Guid SystemProbeId { get; }

		public abstract Stream GetMimeReadStream();

		public abstract bool TryGetListProperty<ItemT>(string name, out ReadOnlyCollection<ItemT> value);

		public abstract bool TryGetProperty<T>(string name, out T value);
	}
}
