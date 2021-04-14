using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ReplicatorEnabledAssociationEnumerator : IEnumerable<MailboxAssociation>, IEnumerable
	{
		public ReplicatorEnabledAssociationEnumerator(IAssociationReplicator replicator, IEnumerable<MailboxAssociation> baseEnumerator, IAssociationStore storeProvider)
		{
			ArgumentValidator.ThrowIfNull("replicator", replicator);
			ArgumentValidator.ThrowIfNull("baseEnumerator", baseEnumerator);
			ArgumentValidator.ThrowIfNull("storeProvider", storeProvider);
			this.replicator = replicator;
			this.baseEnumerator = baseEnumerator;
			this.storeProvider = storeProvider;
		}

		public void TriggerReplication(IAssociationAdaptor masterAdaptor)
		{
			ReplicatorEnabledAssociationEnumerator.Tracer.TraceDebug<int>((long)this.GetHashCode(), "ReplicatorEnabledAssociationEnumerator.TriggerReplication: Found {0} associations out of sync", this.outOfSyncAssociations.Count);
			if (this.outOfSyncAssociations.Count > 0)
			{
				this.replicator.ReplicateAssociation(masterAdaptor, this.outOfSyncAssociations.ToArray());
			}
		}

		public IEnumerator<MailboxAssociation> GetEnumerator()
		{
			ExDateTime minTime = ExDateTime.UtcNow - TimeSpan.FromMinutes(2.0);
			foreach (MailboxAssociation mailboxAssociation in this.baseEnumerator)
			{
				ReplicatorEnabledAssociationEnumerator.Tracer.TraceDebug<MailboxAssociation>((long)this.GetHashCode(), "ReplicatorEnabledAssociationEnumerator.GetEnumerator: Found association: {0}", mailboxAssociation);
				if (this.IsOutOfSyncAssociation(mailboxAssociation) && mailboxAssociation.LastModified < minTime)
				{
					ReplicatorEnabledAssociationEnumerator.Tracer.TraceDebug((long)this.GetHashCode(), "ReplicatorEnabledAssociationEnumerator.GetEnumerator: Association is out of sync");
					this.outOfSyncAssociations.Add(mailboxAssociation);
				}
				yield return mailboxAssociation;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generics interface of GetEnumerator.");
		}

		private bool IsOutOfSyncAssociation(MailboxAssociation association)
		{
			if (association.CurrentVersion > association.SyncedVersion)
			{
				ReplicatorEnabledAssociationEnumerator.Tracer.TraceDebug((long)this.GetHashCode(), "GroupMailboxAccessLayer::IsOutOfSyncAssociation. Association {0}/{1} is out of sync because current version ({2}) is greater than synced version ({3})", new object[]
				{
					association.User,
					association.Group,
					association.CurrentVersion,
					association.SyncedVersion
				});
				return true;
			}
			if (!StringComparer.OrdinalIgnoreCase.Equals(association.SyncedIdentityHash, this.storeProvider.MailboxLocator.IdentityHash))
			{
				ReplicatorEnabledAssociationEnumerator.Tracer.TraceDebug((long)this.GetHashCode(), "GroupMailboxAccessLayer::IsOutOfSyncAssociation. Association {0}/{1} is out of sync because current identity hash of mailbox ({2}) is different than the one synced ({3})", new object[]
				{
					association.User,
					association.Group,
					this.storeProvider.MailboxLocator.IdentityHash,
					association.SyncedIdentityHash
				});
				return true;
			}
			return false;
		}

		private static readonly Trace Tracer = ExTraceGlobals.AssociationReplicationTracer;

		private readonly IAssociationReplicator replicator;

		private readonly IEnumerable<MailboxAssociation> baseEnumerator;

		private readonly IAssociationStore storeProvider;

		private List<MailboxAssociation> outOfSyncAssociations = new List<MailboxAssociation>(10);
	}
}
