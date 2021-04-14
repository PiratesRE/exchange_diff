using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.GroupMailbox.Consistency;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RpcAssociationReplicator : IAssociationReplicator
	{
		public RpcAssociationReplicator(IExtensibleLogger logger, string replicationServerFqdn)
		{
			ArgumentValidator.ThrowIfNull("logger", logger);
			this.Logger = logger;
			this.replicationAssistantInvoker = new ReplicationAssistantInvoker((!string.IsNullOrWhiteSpace(replicationServerFqdn)) ? replicationServerFqdn : LocalServerCache.LocalServerFqdn, this.Logger);
		}

		public bool ReplicateAssociation(IAssociationAdaptor masterAdaptor, params MailboxAssociation[] associations)
		{
			ArgumentValidator.ThrowIfNull("associations", associations);
			ArgumentValidator.ThrowIfZeroOrNegative("associations.Length", associations.Length);
			RpcAssociationReplicator.Tracer.TraceDebug((long)this.GetHashCode(), "RpcAssociationReplicator::ReplicateAssociations");
			try
			{
				return this.replicationAssistantInvoker.Invoke("RpcAssociationReplicator", masterAdaptor, associations);
			}
			catch (GrayException ex)
			{
				this.LogError(ex.ToString());
				masterAdaptor.AssociationStore.SaveMailboxAsOutOfSync();
			}
			return false;
		}

		private void LogError(string errorDescription)
		{
			RpcAssociationReplicator.Tracer.TraceError((long)this.GetHashCode(), errorDescription);
			this.Logger.LogEvent(new SchemaBasedLogEvent<MailboxAssociationLogSchema.Error>
			{
				{
					MailboxAssociationLogSchema.Error.Context,
					"RpcAssociationReplicator"
				},
				{
					MailboxAssociationLogSchema.Error.Exception,
					errorDescription
				}
			});
		}

		protected readonly IExtensibleLogger Logger;

		private static readonly Trace Tracer = ExTraceGlobals.AssociationReplicationTracer;

		private readonly ReplicationAssistantInvoker replicationAssistantInvoker;
	}
}
