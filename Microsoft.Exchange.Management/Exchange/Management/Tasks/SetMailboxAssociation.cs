using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Set", "MailboxAssociation", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailboxAssociation : SetRecipientObjectTask<MailboxAssociationIdParameter, MailboxAssociationPresentationObject, ADUser>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override MailboxAssociationIdParameter Identity
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

		[Parameter]
		public SwitchParameter UpdateSlavedData
		{
			get
			{
				if (base.Fields.IsChanged("UpdateSlaveData"))
				{
					return (SwitchParameter)base.Fields["UpdateSlaveData"];
				}
				return false;
			}
			set
			{
				base.Fields["UpdateSlaveData"] = value;
			}
		}

		[Parameter]
		public SwitchParameter ReplicateMasteredData
		{
			get
			{
				if (base.Fields.IsChanged("ReplicateMasteredData"))
				{
					return (SwitchParameter)base.Fields["ReplicateMasteredData"];
				}
				return false;
			}
			set
			{
				base.Fields["ReplicateMasteredData"] = value;
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is FormatException || exception is StorageTransientException || exception is StoragePermanentException || exception is AssociationNotFoundException || exception is MailboxNotFoundException || exception is RpcException || base.IsKnownException(exception);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			CmdletProxy.ThrowExceptionIfProxyIsNeeded(base.CurrentTaskContext, this.DataObject, false, this.ConfirmationMessage, null);
			try
			{
				ADUser dataObject = this.DataObject;
				if (dataObject == null)
				{
					base.WriteError(new ObjectNotFoundException(Strings.MailboxAssociationMailboxNotFound), ExchangeErrorCategory.Client, this.DataObject);
				}
				else
				{
					IRecipientSession adSession = (IRecipientSession)base.DataSession;
					MailboxAssociationContext mailboxAssociationContext = new MailboxAssociationContext(adSession, dataObject, "Set-MailboxAssociation", this.Identity, false);
					mailboxAssociationContext.Execute(new Action<MailboxAssociationFromStore, IAssociationAdaptor, ADUser, IExtensibleLogger>(this.UpdateMailboxAssociation));
				}
			}
			catch (StorageTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, this.Identity.MailboxId);
			}
			catch (StoragePermanentException exception2)
			{
				base.WriteError(exception2, ErrorCategory.ReadError, this.Identity.MailboxId);
			}
			catch (AssociationNotFoundException exception3)
			{
				base.WriteError(exception3, ErrorCategory.ReadError, this.Identity.MailboxId);
			}
			catch (MailboxNotFoundException exception4)
			{
				base.WriteError(exception4, ErrorCategory.ReadError, this.Identity.MailboxId);
			}
			catch (RpcException exception5)
			{
				base.WriteError(exception5, ErrorCategory.ConnectionError, this.Identity.MailboxId);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void UpdateMailboxAssociation(MailboxAssociationFromStore association, IAssociationAdaptor associationAdaptor, ADUser masterMailbox, IExtensibleLogger logger)
		{
			if (association == null)
			{
				SetMailboxAssociation.Tracer.TraceDebug((long)this.GetHashCode(), "SetMailboxAssocaition.UpdateMailboxAssociation. Skipping null MailboxAssociationFromStore.");
				return;
			}
			bool flag = this.Instance.UpdateAssociation(association, associationAdaptor);
			associationAdaptor.SaveAssociation(association, this.ReplicateMasteredData);
			if (this.UpdateSlavedData)
			{
				associationAdaptor.ReplicateAssociation(association);
			}
			if (flag)
			{
				associationAdaptor.SaveSyncState(association);
			}
			if (this.ReplicateMasteredData)
			{
				RpcAssociationReplicator rpcAssociationReplicator = new RpcAssociationReplicator(logger, associationAdaptor.AssociationStore.ServerFullyQualifiedDomainName);
				rpcAssociationReplicator.ReplicateAssociation(associationAdaptor, new MailboxAssociation[]
				{
					association
				});
			}
		}

		private const string UpdateSlavedDataFieldName = "UpdateSlaveData";

		private const string ReplicateMasteredDataFieldName = "ReplicateMasteredData";

		private static readonly Trace Tracer = ExTraceGlobals.CmdletsTracer;
	}
}
