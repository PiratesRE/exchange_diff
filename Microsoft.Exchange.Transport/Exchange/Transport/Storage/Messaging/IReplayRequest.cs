using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Transport.Storage.Messaging
{
	internal interface IReplayRequest : IDisposable
	{
		long TotalReplayedMessages { get; }

		long ContinuationToken { get; }

		DateTime DateCreated { get; }

		Destination Destination { get; }

		string DiagnosticInformation { get; set; }

		DateTime EndTime { get; }

		long RequestId { get; }

		long PrimaryRequestId { get; set; }

		DateTime StartTime { get; }

		ResubmitRequestState State { get; set; }

		int OutstandingMailItemCount { get; }

		DateTime LastResubmittedMessageOrginalDeliveryTime { get; }

		Guid CorrelationId { get; }

		bool IsTestRequest { get; }

		IEnumerable<TransportMailItem> GetMessagesForRedelivery(int count);

		void AddMailItemReference();

		void ReleaseMailItemReference();

		void Commit();

		void Materialize(Transaction transaction);

		void Delete();
	}
}
