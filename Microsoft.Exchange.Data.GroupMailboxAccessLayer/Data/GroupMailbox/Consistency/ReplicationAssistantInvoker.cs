using System;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Assistants;

namespace Microsoft.Exchange.Data.GroupMailbox.Consistency
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ReplicationAssistantInvoker : IReplicationAssistantInvoker
	{
		public ReplicationAssistantInvoker(string targetServerFqdn, IExtensibleLogger logger)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("targetServerFqdn", targetServerFqdn);
			ArgumentValidator.ThrowIfNull("logger", logger);
			this.targetServerFqdn = targetServerFqdn;
			this.logger = logger;
		}

		public bool Invoke(string command, IAssociationAdaptor masterAdaptor, params MailboxAssociation[] associations)
		{
			ArgumentValidator.ThrowIfNull("masterAdaptor", masterAdaptor);
			ADUser masterMailbox = masterAdaptor.MasterLocator.FindAdUser();
			bool isRpcCallSuccessful = false;
			GrayException.MapAndReportGrayExceptions(delegate()
			{
				try
				{
					MailboxLocator[] array = associations.Select(new Func<MailboxAssociation, MailboxLocator>(masterAdaptor.GetSlaveMailboxLocator)).ToArray<MailboxLocator>();
					this.logger.LogEvent(new SchemaBasedLogEvent<MailboxAssociationLogSchema.CommandExecution>
					{
						{
							MailboxAssociationLogSchema.CommandExecution.Command,
							command
						},
						{
							MailboxAssociationLogSchema.CommandExecution.GroupMailbox,
							masterAdaptor.MasterLocator
						},
						{
							MailboxAssociationLogSchema.CommandExecution.UserMailboxes,
							array
						}
					});
					RpcAssociationReplicatorRunNowParameters rpcAssociationReplicatorRunNowParameters = new RpcAssociationReplicatorRunNowParameters
					{
						SlaveMailboxes = array
					};
					ReplicationAssistantInvoker.Tracer.TraceDebug<string, RpcAssociationReplicatorRunNowParameters>((long)this.GetHashCode(), "ReplicationAssistantInvoker::ReplicateAssociations. Calling RpcAssociationReplicator in '{0}' with parameter: '{1}'", this.targetServerFqdn, rpcAssociationReplicatorRunNowParameters);
					using (AssistantsRpcClient assistantsRpcClient = new AssistantsRpcClient(this.targetServerFqdn))
					{
						assistantsRpcClient.StartWithParams("MailboxAssociationReplicationAssistant", masterMailbox.ExchangeGuid, masterMailbox.Database.ObjectGuid, rpcAssociationReplicatorRunNowParameters.ToString());
					}
					isRpcCallSuccessful = true;
				}
				catch (RpcException ex)
				{
					this.LogError(Strings.RpcReplicationCallFailed(ex.ErrorCode));
					masterAdaptor.AssociationStore.SaveMailboxAsOutOfSync();
				}
			});
			return isRpcCallSuccessful;
		}

		private void LogError(string errorDescription)
		{
			ReplicationAssistantInvoker.Tracer.TraceError((long)this.GetHashCode(), errorDescription);
			this.logger.LogEvent(new SchemaBasedLogEvent<MailboxAssociationLogSchema.Error>
			{
				{
					MailboxAssociationLogSchema.Error.Context,
					"ReplicationAssistantInvoker"
				},
				{
					MailboxAssociationLogSchema.Error.Exception,
					errorDescription
				}
			});
		}

		private const string ReplicationAssistantName = "MailboxAssociationReplicationAssistant";

		private static readonly Trace Tracer = ExTraceGlobals.AssociationReplicationTracer;

		private readonly IExtensibleLogger logger;

		private readonly string targetServerFqdn;
	}
}
