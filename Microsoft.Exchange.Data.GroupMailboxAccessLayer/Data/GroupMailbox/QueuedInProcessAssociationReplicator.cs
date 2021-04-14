using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.GroupMailbox.Common;
using Microsoft.Exchange.Data.GroupMailbox.Consistency;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class QueuedInProcessAssociationReplicator : IAssociationReplicator
	{
		public QueuedInProcessAssociationReplicator(GroupMailboxLocator mailbox, IRecipientSession adSession, string replicationServerFqdn, string clientInfoString) : this(mailbox, adSession, replicationServerFqdn, clientInfoString, (IExtensibleLogger logger, IMailboxAssociationPerformanceTracker performanceTracker) => new InProcessAssociationReplicator(logger, performanceTracker, OpenAsAdminOrSystemServiceBudgetTypeType.Unthrottled), (IExtensibleLogger logger, string clientString) => new StoreBuilder(null, new XSOFactory(), logger, clientString), (IExtensibleLogger logger, string targetServerFqdn) => new ReplicationAssistantInvoker(targetServerFqdn, logger), () => MailboxAssociationDiagnosticsFrameFactory.Default)
		{
		}

		public QueuedInProcessAssociationReplicator(GroupMailboxLocator mailbox, IRecipientSession adSession, string replicationServerFqdn, string clientInfoString, Func<IExtensibleLogger, IMailboxAssociationPerformanceTracker, IAssociationReplicator> immediateReplicatorCreator, Func<IExtensibleLogger, string, IStoreBuilder> storeBuilderCreator, Func<IExtensibleLogger, string, IReplicationAssistantInvoker> replicationAssistantInvokerCreator, Func<IDiagnosticsFrameFactory<IExtensibleLogger, IMailboxAssociationPerformanceTracker>> diagnosticsFrameFactoryCreator)
		{
			ArgumentValidator.ThrowIfNull("mailbox", mailbox);
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("clientInfoString", clientInfoString);
			ArgumentValidator.ThrowIfNull("immediateReplicatorCreator", immediateReplicatorCreator);
			ArgumentValidator.ThrowIfNull("storeBuilderCreator", storeBuilderCreator);
			ArgumentValidator.ThrowIfNull("replicationAssistantInvokerCreator", replicationAssistantInvokerCreator);
			ArgumentValidator.ThrowIfNull("diagnosticsFrameFactoryCreator", diagnosticsFrameFactoryCreator);
			this.mailbox = mailbox;
			this.adSession = adSession;
			this.replicationServerFqdn = ((!string.IsNullOrWhiteSpace(replicationServerFqdn)) ? replicationServerFqdn : LocalServerCache.LocalServerFqdn);
			this.clientInfoString = clientInfoString;
			this.immediateReplicatorCreator = immediateReplicatorCreator;
			this.storeBuilderCreator = storeBuilderCreator;
			this.replicationAssistantInvokerCreator = replicationAssistantInvokerCreator;
			this.diagnosticsFrameFactoryCreator = diagnosticsFrameFactoryCreator;
			this.pendingAssociations = new Queue<MailboxAssociation>();
		}

		public bool ReplicateAssociation(IAssociationAdaptor masterAdaptor, params MailboxAssociation[] associations)
		{
			ArgumentValidator.ThrowIfNull("associations", associations);
			ArgumentValidator.ThrowIfZeroOrNegative("associations.Length", associations.Length);
			QueuedInProcessAssociationReplicator.Tracer.TraceDebug<int>((long)this.GetHashCode(), "QueuedInProcessAssociationReplicator::ReplicateAssociation. Enqueuing {0} associations to replicate in the background.", associations.Length);
			masterAdaptor.AssociationStore.SaveMailboxAsOutOfSync();
			foreach (MailboxAssociation item in associations)
			{
				this.pendingAssociations.Enqueue(item);
			}
			return true;
		}

		public void ReplicateQueuedAssociations()
		{
			QueuedInProcessAssociationReplicator.Tracer.TraceDebug((long)this.GetHashCode(), "MailboxTaskProcessor.ReplicateQueuedAssociations: Processing associations.");
			ADUser mailboxAdUser = this.mailbox.FindAdUser();
			IDiagnosticsFrameFactory<IExtensibleLogger, IMailboxAssociationPerformanceTracker> diagnosticsFrameFactory = this.diagnosticsFrameFactoryCreator();
			IMailboxAssociationPerformanceTracker performanceTracker = diagnosticsFrameFactory.CreatePerformanceTracker(null);
			IExtensibleLogger logger = diagnosticsFrameFactory.CreateLogger(mailboxAdUser.ExchangeGuid, mailboxAdUser.OrganizationId);
			IAssociationReplicator replicator = this.immediateReplicatorCreator(logger, performanceTracker);
			using (MailboxAssociationDiagnosticsFrameFactory.Default.CreateDiagnosticsFrame("QueuedInProcessAssociationReplicator", "ReplicateAssociations", logger, performanceTracker))
			{
				bool inProcessReplicationSucceeded = true;
				GroupMailboxAccessLayerHelper.ExecuteOperationWithRetry(logger, "QueuedInProcessAssociationReplicator.ReplicateAssociations", delegate
				{
					IStoreBuilder storeBuilder = this.storeBuilderCreator(logger, this.clientInfoString);
					using (IAssociationStore associationStore = storeBuilder.Create(this.mailbox, performanceTracker))
					{
						QueuedInProcessAssociationReplicator.Tracer.TraceDebug<string>((long)this.GetHashCode(), "QueuedInProcessAssociationReplicator.ReplicateAssociationsImplementation: Created store provider. Mailbox={0}.", mailboxAdUser.ExternalDirectoryObjectId);
						UserAssociationAdaptor masterAdaptor = new UserAssociationAdaptor(associationStore, this.adSession, this.mailbox);
						while (this.pendingAssociations.Count > 0)
						{
							MailboxAssociation association = this.pendingAssociations.Dequeue();
							inProcessReplicationSucceeded &= this.ReplicateSingleAssociation(replicator, masterAdaptor, association);
						}
						if (!inProcessReplicationSucceeded)
						{
							IReplicationAssistantInvoker replicationAssistantInvoker = this.replicationAssistantInvokerCreator(logger, this.replicationServerFqdn);
							replicationAssistantInvoker.Invoke("QueuedInProcessAssociationReplicatorRpcReplication", masterAdaptor, new MailboxAssociation[0]);
						}
					}
				}, new Predicate<Exception>(GrayException.IsGrayException));
			}
			QueuedInProcessAssociationReplicator.Tracer.TraceDebug((long)this.GetHashCode(), "MailboxTaskProcessor.SendNotificationImplementation: Task completed.");
		}

		private bool ReplicateSingleAssociation(IAssociationReplicator replicator, IAssociationAdaptor masterAdaptor, MailboxAssociation association)
		{
			QueuedInProcessAssociationReplicator.Tracer.TraceDebug<IAssociationAdaptor, MailboxAssociation>((long)this.GetHashCode(), "QueuedInProcessAssociationReplicator.ReplicateSingleAssociation: MasterAdaptor={0}, Association={1}.", masterAdaptor, association);
			return replicator.ReplicateAssociation(masterAdaptor, new MailboxAssociation[]
			{
				association
			});
		}

		private static readonly Trace Tracer = ExTraceGlobals.AssociationReplicationTracer;

		private readonly IRecipientSession adSession;

		private readonly string replicationServerFqdn;

		private readonly string clientInfoString;

		private readonly GroupMailboxLocator mailbox;

		private readonly Queue<MailboxAssociation> pendingAssociations;

		private readonly Func<IExtensibleLogger, IMailboxAssociationPerformanceTracker, IAssociationReplicator> immediateReplicatorCreator;

		private readonly Func<IExtensibleLogger, string, IStoreBuilder> storeBuilderCreator;

		private readonly Func<IExtensibleLogger, string, IReplicationAssistantInvoker> replicationAssistantInvokerCreator;

		private readonly Func<IDiagnosticsFrameFactory<IExtensibleLogger, IMailboxAssociationPerformanceTracker>> diagnosticsFrameFactoryCreator;
	}
}
