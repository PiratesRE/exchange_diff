using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	internal interface IMailRecipientStorage
	{
		long RecipId { get; }

		long MsgId { get; set; }

		AdminActionStatus AdminActionStatus { get; set; }

		DateTime? DeliveryTime { get; set; }

		DsnFlags DsnCompleted { get; set; }

		DsnFlags DsnNeeded { get; set; }

		DsnRequestedFlags DsnRequested { get; set; }

		Destination DeliveredDestination { get; set; }

		string Email { get; set; }

		string ORcpt { get; set; }

		string PrimaryServerFqdnGuid { get; set; }

		int RetryCount { get; set; }

		Status Status { get; set; }

		RequiredTlsAuthLevel? TlsAuthLevel { get; set; }

		int OutboundIPPool { get; set; }

		IExtendedPropertyCollection ExtendedProperties { get; }

		bool IsDeleted { get; }

		bool IsInSafetyNet { get; }

		bool IsActive { get; }

		bool PendingDatabaseUpdates { get; }

		void MarkToDelete();

		void Materialize(Transaction transaction);

		void Commit(TransactionCommitMode commitMode);

		void ReleaseFromActive();

		void AddToSafetyNet();

		IMailRecipientStorage MoveTo(long targetMailItemId);

		IMailRecipientStorage CopyTo(long targetMailItemId);

		void MinimizeMemory();
	}
}
