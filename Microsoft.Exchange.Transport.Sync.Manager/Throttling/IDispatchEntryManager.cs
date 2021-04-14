using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDispatchEntryManager
	{
		event EventHandler<DispatchEntry> EntryExpiredEvent;

		bool ContainsSubscription(Guid subscriptionGuid);

		int GetNumberOfEntriesForDatabase(Guid databaseGuid);

		bool HasBudget(WorkType workType);

		void Add(DispatchEntry dispatchEntry);

		DispatchEntry RemoveDispatchAttempt(Guid databaseGuid, Guid subscriptionGuid);

		void MarkDispatchSuccess(Guid subscriptionGuid);

		bool TryRemoveDispatchedEntry(Guid subscriptionGuid, out DispatchEntry dispatchEntry);

		void DisabledExpiration();

		XElement GetDiagnosticInfo(SyncDiagnosticMode mode);
	}
}
