using System;
using System.Management.Automation;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Cmdlet("Set", "MailPublicFolder", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetMailPublicFolder : SetMailEnabledRecipientObjectTask<MailPublicFolderIdParameter, MailPublicFolder, ADPublicFolder>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetMailPublicFolder(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] Contacts
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[MailPublicFolderSchema.Contacts];
			}
			set
			{
				base.Fields[MailPublicFolderSchema.Contacts] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter ForwardingAddress
		{
			get
			{
				return (RecipientIdParameter)base.Fields[MailPublicFolderSchema.ForwardingAddress];
			}
			set
			{
				base.Fields[MailPublicFolderSchema.ForwardingAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProxyAddress ExternalEmailAddress
		{
			get
			{
				return (ProxyAddress)base.Fields[MailPublicFolderSchema.ExternalEmailAddress];
			}
			set
			{
				base.Fields[MailPublicFolderSchema.ExternalEmailAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string EntryId
		{
			get
			{
				return (string)base.Fields[MailPublicFolderSchema.EntryId];
			}
			set
			{
				base.Fields[MailPublicFolderSchema.EntryId] = value;
			}
		}

		private ADObjectId GetRecipientIdentityAndValidateTypeForContacts(RecipientIdParameter recipientIdParameter, Task.ErrorLoggerDelegate errorHandler)
		{
			ADRecipient adrecipient = (ADRecipient)base.GetDataObject<ADRecipient>(recipientIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(recipientIdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(recipientIdParameter.ToString())), ExchangeErrorCategory.Client);
			if (RecipientType.UserMailbox != adrecipient.RecipientType && RecipientType.MailUser != adrecipient.RecipientType && RecipientType.MailContact != adrecipient.RecipientType)
			{
				errorHandler(new RecipientTaskException(Strings.ErrorIndividualRecipientNeeded(recipientIdParameter.ToString())), ExchangeErrorCategory.Client, recipientIdParameter);
				return null;
			}
			return (ADObjectId)adrecipient.Identity;
		}

		private ADObjectId GetRecipientIdentityAndValidateTypeForFwd(RecipientIdParameter recipientIdParameter, Task.ErrorLoggerDelegate errorHandler)
		{
			ADRecipient adrecipient = (ADRecipient)base.GetDataObject<ADRecipient>(recipientIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(recipientIdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(recipientIdParameter.ToString())), ExchangeErrorCategory.Client);
			if (adrecipient.RecipientType == RecipientType.Invalid || RecipientType.User == adrecipient.RecipientType || RecipientType.Contact == adrecipient.RecipientType || RecipientType.Group == adrecipient.RecipientType || RecipientType.PublicDatabase == adrecipient.RecipientType || RecipientType.SystemAttendantMailbox == adrecipient.RecipientType || RecipientType.SystemMailbox == adrecipient.RecipientType || RecipientType.MicrosoftExchange == adrecipient.RecipientType)
			{
				errorHandler(new RecipientTaskException(Strings.ErrorInvalidRecipientType(recipientIdParameter.ToString(), adrecipient.RecipientType.ToString())), ExchangeErrorCategory.Client, recipientIdParameter);
				return null;
			}
			return (ADObjectId)adrecipient.Identity;
		}

		private OrganizationId ResolveCurrentOrganization()
		{
			if (MapiTaskHelper.IsDatacenter)
			{
				OrganizationIdParameter organization = MapiTaskHelper.ResolveTargetOrganizationIdParameter(null, this.Identity, base.CurrentOrganizationId, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
				return MapiTaskHelper.ResolveTargetOrganization(base.DomainController, organization, ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), base.CurrentOrganizationId, base.ExecutingUserOrganizationId);
			}
			return base.CurrentOrganizationId ?? base.ExecutingUserOrganizationId;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.CurrentOrganizationId = this.ResolveCurrentOrganization();
			base.InternalBeginProcessing();
			MailPublicFolder mailPublicFolder = (MailPublicFolder)this.GetDynamicParameters();
			if (base.Fields.IsModified(MailPublicFolderSchema.Contacts))
			{
				mailPublicFolder.Contacts.Clear();
				if (this.Contacts != null)
				{
					foreach (RecipientIdParameter recipientIdParameter in this.Contacts)
					{
						ADObjectId recipientIdentityAndValidateTypeForContacts = this.GetRecipientIdentityAndValidateTypeForContacts(recipientIdParameter, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
						mailPublicFolder.Contacts.Add(recipientIdentityAndValidateTypeForContacts);
					}
				}
			}
			if (base.Fields.IsModified(MailPublicFolderSchema.ForwardingAddress))
			{
				mailPublicFolder.ForwardingAddress = null;
				if (this.ForwardingAddress != null)
				{
					ADObjectId recipientIdentityAndValidateTypeForFwd = this.GetRecipientIdentityAndValidateTypeForFwd(this.ForwardingAddress, new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
					mailPublicFolder.ForwardingAddress = recipientIdentityAndValidateTypeForFwd;
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.DataObject.IsModified(MailPublicFolderSchema.Contacts))
			{
				foreach (ADObjectId adObjectId in this.DataObject.Contacts.Added)
				{
					this.GetRecipientIdentityAndValidateTypeForContacts(new RecipientIdParameter(adObjectId), new Task.ErrorLoggerDelegate(base.WriteError));
				}
			}
			if (this.DataObject.IsModified(MailPublicFolderSchema.ForwardingAddress) && this.DataObject.ForwardingAddress != null)
			{
				this.GetRecipientIdentityAndValidateTypeForFwd(new RecipientIdParameter(this.DataObject.ForwardingAddress), new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(MailPublicFolderSchema.ExternalEmailAddress))
			{
				this.DataObject.ExternalEmailAddress = this.ExternalEmailAddress;
				MailUserTaskHelper.ValidateExternalEmailAddress(this.DataObject, this.ConfigurationSession, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
			}
			ADObjectId adobjectId = null;
			if (base.Fields.IsModified(MailPublicFolderSchema.EntryId) && this.IsValidToUpdateEntryId(this.EntryId, out adobjectId))
			{
				this.DataObject.EntryId = this.EntryId;
				if (adobjectId != null)
				{
					this.DataObject.ContentMailbox = adobjectId;
				}
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is StoragePermanentException || base.IsKnownException(exception);
		}

		protected override bool ShouldSupportPreResolveOrgIdBasedOnIdentity()
		{
			return false;
		}

		private bool IsValidToUpdateEntryId(string entryId, out ADObjectId contentMailbox)
		{
			contentMailbox = null;
			if (string.IsNullOrEmpty(entryId))
			{
				return false;
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(entryId, this.DataObject.EntryId))
			{
				return false;
			}
			StoreObjectId storeObjectId = null;
			try
			{
				storeObjectId = StoreObjectId.FromHexEntryId(entryId);
			}
			catch (FormatException innerException)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorSetMailPublicFolderEntryIdWrongFormat(entryId), innerException), ExchangeErrorCategory.Client, this.DataObject.Identity);
			}
			using (PublicFolderDataProvider publicFolderDataProvider = new PublicFolderDataProvider(this.ConfigurationSession, "Set-MailPublicFolder", Guid.Empty))
			{
				Guid exchangeGuid = Guid.Empty;
				try
				{
					PublicFolder publicFolder = (PublicFolder)publicFolderDataProvider.Read<PublicFolder>(new PublicFolderId(storeObjectId));
					exchangeGuid = publicFolder.ContentMailboxGuid;
				}
				catch (ObjectNotFoundException innerException2)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorSetMailPublicFolderEntryIdNotExist(entryId), innerException2), ExchangeErrorCategory.Client, this.DataObject.Identity);
				}
				ADRecipient adrecipient = base.TenantGlobalCatalogSession.FindByExchangeGuid(exchangeGuid);
				if (adrecipient != null)
				{
					contentMailbox = adrecipient.Id;
				}
				if (string.IsNullOrEmpty(this.DataObject.EntryId))
				{
					return true;
				}
				StoreObjectId storeObjectId2 = StoreObjectId.FromHexEntryId(this.DataObject.EntryId);
				try
				{
					PublicFolder publicFolder2 = (PublicFolder)publicFolderDataProvider.Read<PublicFolder>(new PublicFolderId(storeObjectId2));
				}
				catch (ObjectNotFoundException)
				{
					return true;
				}
				if (ArrayComparer<byte>.Comparer.Equals(storeObjectId2.LongTermFolderId, storeObjectId.LongTermFolderId))
				{
					return true;
				}
			}
			return false;
		}
	}
}
