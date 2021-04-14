using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PriorityReplicator : IAssociationReplicator
	{
		public PriorityReplicator(IAssociationReplicator priorityReplicator, IAssociationReplicator defaultReplicator, IMailboxLocator priorityLocator)
		{
			ArgumentValidator.ThrowIfNull("priorityReplicator", priorityReplicator);
			ArgumentValidator.ThrowIfNull("defaultReplicator", defaultReplicator);
			this.priorityLocator = priorityLocator;
			this.priorityReplicator = priorityReplicator;
			this.defaultReplicator = defaultReplicator;
		}

		public PriorityReplicator(IExtensibleLogger logger, IMailboxAssociationPerformanceTracker performanceTracker, string replicationServerFqdn, IMailboxLocator priorityLocator) : this(new InProcessAssociationReplicator(logger, performanceTracker, OpenAsAdminOrSystemServiceBudgetTypeType.RunAsBackgroundLoad), new RpcAssociationReplicator(logger, replicationServerFqdn), priorityLocator)
		{
		}

		public bool ReplicateAssociation(IAssociationAdaptor masterAdaptor, params MailboxAssociation[] associations)
		{
			ArgumentValidator.ThrowIfNull("associations", associations);
			ArgumentValidator.ThrowIfZeroOrNegative("associations.Length", associations.Length);
			PriorityReplicator.Tracer.TraceDebug((long)this.GetHashCode(), "PriorityReplicator::ReplicateAssociations");
			bool flag = true;
			MailboxAssociation priorityAssociation = this.GetAssociationWithPriority(masterAdaptor, associations);
			if (priorityAssociation != null)
			{
				PriorityReplicator.Tracer.TraceDebug((long)this.GetHashCode(), "PriorityReplicator::ReplicateAssociations. Found priority association");
				flag &= this.priorityReplicator.ReplicateAssociation(masterAdaptor, new MailboxAssociation[]
				{
					priorityAssociation
				});
				MailboxAssociation[] array = (from association in associations
				where !association.Equals(priorityAssociation)
				select association).ToArray<MailboxAssociation>();
				PriorityReplicator.Tracer.TraceDebug<int>((long)this.GetHashCode(), "PriorityReplicator::ReplicateAssociations. Found {0} association to replicate via RPC", array.Length);
				if (array.Length > 0)
				{
					flag &= this.defaultReplicator.ReplicateAssociation(masterAdaptor, array);
				}
			}
			else
			{
				PriorityReplicator.Tracer.TraceDebug<int>((long)this.GetHashCode(), "PriorityReplicator::ReplicateAssociations. Priority association not found. Found {0} association to replicate via RPC", associations.Length);
				flag &= this.defaultReplicator.ReplicateAssociation(masterAdaptor, associations);
			}
			return flag;
		}

		private MailboxAssociation GetAssociationWithPriority(IAssociationAdaptor masterAdaptor, params MailboxAssociation[] associations)
		{
			MailboxAssociation result = null;
			if (this.priorityLocator != null)
			{
				result = associations.FirstOrDefault((MailboxAssociation association) => this.ShouldPrioritize(masterAdaptor.GetSlaveMailboxLocator(association)));
			}
			return result;
		}

		private bool ShouldPrioritize(IMailboxLocator locator)
		{
			PriorityReplicator.Tracer.TraceDebug<IMailboxLocator, IMailboxLocator>((long)this.GetHashCode(), "PriorityReplicator::ShouldPrioritize. Comparing locator {0} with priority locator {1}", locator, this.priorityLocator);
			if (!string.IsNullOrWhiteSpace(this.priorityLocator.ExternalId))
			{
				return string.Equals(this.priorityLocator.ExternalId, locator.ExternalId, StringComparison.InvariantCultureIgnoreCase);
			}
			return string.Equals(this.priorityLocator.LegacyDn, locator.LegacyDn, StringComparison.InvariantCultureIgnoreCase);
		}

		private static readonly Trace Tracer = ExTraceGlobals.AssociationReplicationTracer;

		private readonly IAssociationReplicator priorityReplicator;

		private readonly IAssociationReplicator defaultReplicator;

		private readonly IMailboxLocator priorityLocator;
	}
}
