using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Remove", "MailboxAssociation", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMailboxAssociation : RemoveRecipientObjectTask<MailboxAssociationIdParameter, ADUser>
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveMailboxAssociation(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			if (this.Identity.AssociationIdType == null)
			{
				base.WriteError(new MailboxAssociationInvalidOperationException(Strings.NoMailboxAssociationIdentityProvided), ExchangeErrorCategory.Client, this.Identity);
			}
			base.InternalValidate();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is FormatException || exception is StorageTransientException || exception is StoragePermanentException || exception is AssociationNotFoundException || exception is MailboxNotFoundException || base.IsKnownException(exception);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			CmdletProxy.ThrowExceptionIfProxyIsNeeded(base.CurrentTaskContext, base.DataObject, false, this.ConfirmationMessage, null);
			try
			{
				ADUser dataObject = base.DataObject;
				if (dataObject == null)
				{
					base.WriteError(new ObjectNotFoundException(Strings.MailboxAssociationMailboxNotFound), ExchangeErrorCategory.Client, base.DataObject);
				}
				else
				{
					IRecipientSession adSession = (IRecipientSession)base.DataSession;
					MailboxAssociationContext mailboxAssociationContext = new MailboxAssociationContext(adSession, dataObject, "Remove-MailboxAssociation", this.Identity, false);
					mailboxAssociationContext.Execute(new Action<MailboxAssociationFromStore, IAssociationAdaptor, ADUser, IExtensibleLogger>(this.DeleteMailboxAssociation));
				}
			}
			catch (StorageTransientException exception)
			{
				base.WriteError(exception, ExchangeErrorCategory.ServerTransient, this.Identity.MailboxId);
			}
			catch (StoragePermanentException exception2)
			{
				base.WriteError(exception2, ExchangeErrorCategory.ServerOperation, this.Identity.MailboxId);
			}
			catch (AssociationNotFoundException exception3)
			{
				base.WriteError(exception3, ExchangeErrorCategory.ServerOperation, this.Identity.MailboxId);
			}
			catch (MailboxNotFoundException exception4)
			{
				base.WriteError(exception4, ExchangeErrorCategory.ServerOperation, this.Identity.MailboxId);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void DeleteMailboxAssociation(MailboxAssociationFromStore association, IAssociationAdaptor associationAdaptor, ADUser masterMailbox, IExtensibleLogger logger)
		{
			if (association != null)
			{
				associationAdaptor.DeleteAssociation(association);
				return;
			}
			base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.Identity.AssociationIdValue, typeof(MailboxAssociationPresentationObject).ToString(), this.Identity.MailboxId.ToString())), ExchangeErrorCategory.Client, this.Identity);
		}

		private static readonly Trace Tracer = ExTraceGlobals.CmdletsTracer;
	}
}
