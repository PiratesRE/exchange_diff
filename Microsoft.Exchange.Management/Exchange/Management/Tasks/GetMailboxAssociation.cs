using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "MailboxAssociation", DefaultParameterSetName = "Identity")]
	public sealed class GetMailboxAssociation : GetRecipientObjectTask<MailboxAssociationIdParameter, ADUser>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
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
		public SwitchParameter IncludeNotPromotedProperties
		{
			get
			{
				if (base.Fields.IsChanged("IncludeNotPromotedProperties"))
				{
					return (SwitchParameter)base.Fields["IncludeNotPromotedProperties"];
				}
				return false;
			}
			set
			{
				base.Fields["IncludeNotPromotedProperties"] = value;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is FormatException || exception is StorageTransientException || exception is StoragePermanentException || exception is AssociationNotFoundException || exception is MailboxNotFoundException || exception is ManagementObjectNotFoundException || base.IsKnownException(exception);
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			try
			{
				ADUser aduser = (ADUser)dataObject;
				MailboxAssociationPresentationObject mailboxAssociationPresentationObject = new MailboxAssociationPresentationObject();
				if (!CmdletProxy.TryToProxyOutputObject(mailboxAssociationPresentationObject, base.CurrentTaskContext, aduser, false, this.ConfirmationMessage, null))
				{
					IRecipientSession adSession = (IRecipientSession)base.DataSession;
					MailboxAssociationContext mailboxAssociationContext = new MailboxAssociationContext(adSession, aduser, "Get-MailboxAssociation", this.Identity, this.IncludeNotPromotedProperties);
					mailboxAssociationContext.Execute(new Action<MailboxAssociationFromStore, IAssociationAdaptor, ADUser, IExtensibleLogger>(this.WriteMailboxAssociation));
				}
				else
				{
					base.WriteResult(mailboxAssociationPresentationObject);
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
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void WriteMailboxAssociation(MailboxAssociationFromStore association, IAssociationAdaptor associationAdaptor, ADUser masterMailbox, IExtensibleLogger logger)
		{
			if (association != null)
			{
				ObjectId objectId = new MailboxStoreObjectId(masterMailbox.ObjectId, association.ItemId.ObjectId);
				MailboxLocator slaveMailboxLocator = associationAdaptor.GetSlaveMailboxLocator(association);
				if (base.NeedSuppressingPiiData)
				{
					objectId = SuppressingPiiData.RedactMailboxStoreObjectId(objectId);
				}
				base.WriteResult(new MailboxAssociationPresentationObject
				{
					Identity = objectId,
					ExternalId = slaveMailboxLocator.ExternalId,
					LegacyDn = slaveMailboxLocator.LegacyDn,
					IsMember = association.IsMember,
					JoinedBy = association.JoinedBy,
					GroupSmtpAddress = association.GroupSmtpAddress,
					UserSmtpAddress = association.UserSmtpAddress,
					IsPin = association.IsPin,
					ShouldEscalate = association.ShouldEscalate,
					IsAutoSubscribed = association.IsAutoSubscribed,
					JoinDate = association.JoinDate,
					LastVisitedDate = association.LastVisitedDate,
					PinDate = association.PinDate,
					LastModified = association.LastModified,
					CurrentVersion = association.CurrentVersion,
					SyncedVersion = association.SyncedVersion,
					LastSyncError = association.LastSyncError,
					SyncAttempts = association.SyncAttempts,
					SyncedSchemaVersion = association.SyncedSchemaVersion
				});
				return;
			}
			GetMailboxAssociation.Tracer.TraceDebug((long)this.GetHashCode(), "GetMailboxAssocaition.WriteMailboxAssociation. Skipping null MailboxAssociationFromStore.");
		}

		private const string IncludeNotPromotedPropertiesFieldName = "IncludeNotPromotedProperties";

		private static readonly Trace Tracer = ExTraceGlobals.CmdletsTracer;
	}
}
