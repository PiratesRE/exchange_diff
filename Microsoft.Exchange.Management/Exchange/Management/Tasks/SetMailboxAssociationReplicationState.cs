using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Set", "MailboxAssociationReplicationState", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailboxAssociationReplicationState : SetRecipientObjectTask<MailboxIdParameter, MailboxAssociationReplicationStatePresentationObject, ADUser>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
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

		protected override IConfigurable PrepareDataObject()
		{
			IConfigurable configurable = base.PrepareDataObject();
			CmdletProxy.ThrowExceptionIfProxyIsNeeded(base.CurrentTaskContext, (ADUser)configurable, false, this.ConfirmationMessage, null);
			return configurable;
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				ExchangePrincipal mailboxOwner = ExchangePrincipal.FromADUser(this.DataObject, null);
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, "Client=Management;Action=Set-MailboxAssociationReplicationState"))
				{
					if (this.Instance.NextReplicationTime != null)
					{
						LocalAssociationStore.SaveMailboxSyncStatus(mailboxSession, this.Instance.NextReplicationTime, null);
					}
				}
			}
			catch (StorageTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, this.Identity);
			}
			catch (StoragePermanentException exception2)
			{
				base.WriteError(exception2, ErrorCategory.ReadError, this.Identity);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.CmdletsTracer;
	}
}
