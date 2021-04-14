using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class SetMailContactBase<TPublicObject> : SetMailEnabledOrgPersonObjectTask<MailContactIdParameter, TPublicObject, ADContact> where TPublicObject : MailContact, new()
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetMailContact(this.Identity.ToString());
			}
		}

		public SetMailContactBase()
		{
		}

		protected override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		protected override bool ShouldUpgradeExchangeVersion(ADObject adObject)
		{
			return base.ShouldUpgradeExchangeVersion(adObject) || adObject.propertyBag.Changed;
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADContact adcontact = (ADContact)base.PrepareDataObject();
			if (adcontact.RecipientDisplayType == null)
			{
				adcontact.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.RemoteMailUser);
			}
			if (adcontact.IsChanged(ADRecipientSchema.ExternalEmailAddress))
			{
				MailContactTaskHelper.ValidateExternalEmailAddress(adcontact, this.ConfigurationSession, new Task.ErrorLoggerDelegate(base.WriteError), base.ProvisioningCache);
			}
			TaskLogger.LogExit();
			return adcontact;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			DistributionGroupTaskHelper.CheckModerationInMixedEnvironment(this.DataObject, new Task.TaskWarningLoggingDelegate(this.WriteWarning), Strings.WarningLegacyExchangeServerForMailContact);
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return MailContact.FromDataObject((ADContact)dataObject);
		}
	}
}
