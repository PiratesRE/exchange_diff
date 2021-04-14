using System;
using System.Net;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal interface ITransportMailItemFacade
	{
		long RecordId { get; }

		IMailRecipientCollectionFacade Recipients { get; }

		RoutingAddress From { set; }

		RoutingAddress OriginalFrom { get; }

		EmailMessage Message { get; }

		DeliveryPriority Priority { get; set; }

		string PrioritizationReason { get; set; }

		MimeDocument MimeDocument { set; }

		bool IsJournalReport();

		void PrepareJournalReport();

		string ReceiveConnectorName { get; set; }

		bool PipelineTracingEnabled { get; }

		void CommitLazy();

		IAsyncResult BeginCommitForReceive(AsyncCallback callback, object context);

		bool EndCommitForReceive(IAsyncResult ar, out Exception exception);

		object ADRecipientCacheAsObject { get; }

		object OrganizationIdAsObject { get; }

		Guid ExternalOrganizationId { get; }

		ITransportSettingsFacade TransportSettings { get; }

		bool IsOriginating { get; }

		MailDirectionality Directionality { get; }

		bool IsProbe { get; }

		bool FallbackToRawOverride { get; set; }

		IPAddress SourceIPAddress { get; set; }
	}
}
