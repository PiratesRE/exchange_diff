using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMailboxAssociationPerformanceTracker : IMailboxPerformanceTracker, IPerformanceTracker
	{
		void IncrementAssociationsRead();

		void IncrementAssociationsCreated();

		void IncrementAssociationsUpdated();

		void IncrementAssociationsDeleted();

		void IncrementFailedAssociationsSearch();

		void IncrementNonUniqueAssociationsFound();

		void IncrementAutoSubscribedMembers();

		void IncrementMissingLegacyDns();

		void IncrementAssociationReplicationAttempts();

		void IncrementFailedAssociationReplications();

		void SetNewSessionRequired(bool isRequired);

		void SetNewSessionWrongServer(bool isWrongServer);

		void SetNewSessionLatency(long milliseconds);

		void SetAADQueryLatency(long milliseconds);
	}
}
