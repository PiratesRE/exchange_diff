using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Assistants;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Start", "MailboxAssociationReplication", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
	public sealed class StartMailboxAssociationReplication : RecipientObjectActionTask<MailboxIdParameter, ADUser>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override MailboxIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is FormatException || exception is StorageTransientException || exception is StoragePermanentException || exception is MailboxNotFoundException || exception is RpcException || exception is RpcClientException || base.IsKnownException(exception);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				ADUser dataObject = this.DataObject;
				if (dataObject == null)
				{
					base.WriteError(new ObjectNotFoundException(Strings.MailboxAssociationMailboxNotFound), ExchangeErrorCategory.Client, this.DataObject);
				}
				else
				{
					int num;
					string remoteServerForADUser = TaskHelper.GetRemoteServerForADUser(dataObject, new Task.TaskVerboseLoggingDelegate(base.CurrentTaskContext.CommandShell.WriteVerbose), out num);
					base.WriteVerbose(Strings.MailboxAssociationReplicationRpcRequest(dataObject.Id.ToString(), remoteServerForADUser));
					using (AssistantsRpcClient assistantsRpcClient = new AssistantsRpcClient(remoteServerForADUser))
					{
						assistantsRpcClient.StartWithParams(StartMailboxAssociationReplication.MailboxAssociationReplicationAssistantName, dataObject.ExchangeGuid, dataObject.Database.ObjectGuid, string.Empty);
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.CmdletsTracer;

		private static readonly string MailboxAssociationReplicationAssistantName = "MailboxAssociationReplicationAssistant";
	}
}
