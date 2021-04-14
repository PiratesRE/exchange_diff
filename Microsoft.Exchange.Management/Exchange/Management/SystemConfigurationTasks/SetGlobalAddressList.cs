using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "GlobalAddressList", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetGlobalAddressList : SetAddressBookBase<GlobalAddressListIdParameter, GlobalAddressList>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetGlobalAddressList(this.Identity.ToString());
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			this.isDefaultGAL = (bool)((ADObject)dataObject)[AddressBookBaseSchema.IsDefaultGlobalAddressList];
			base.StampChangesOn(dataObject);
			AddressBookBase addressBookBase = (AddressBookBase)dataObject;
			if (addressBookBase.IsModified(ADObjectSchema.Name))
			{
				addressBookBase.DisplayName = addressBookBase.Name;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors && this.isDefaultGAL)
			{
				if (this.DataObject.IsModified(ADObjectSchema.Name) || this.DataObject.IsModified(AddressBookBaseSchema.DisplayName) || this.DataObject.IsModified(AddressBookBaseSchema.RecipientContainer))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorInvalidOperationOnDefaultGAL(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
				base.ValidateBrokenRecipientFilterChange(GlobalAddressList.RecipientFilterForDefaultGal);
				if (this.isDefaultGAL && this.DataObject.ExchangeVersion.IsSameVersion(ExchangeObjectVersion.Exchange2007))
				{
					this.DataObject[AddressBookBaseSchema.RecipientFilterFlags] = ((RecipientFilterableObjectFlags)this.DataObject[AddressBookBaseSchema.RecipientFilterFlags] | RecipientFilterableObjectFlags.IsDefault);
				}
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return GlobalAddressList.FromDataObject((AddressBookBase)dataObject);
		}

		private bool isDefaultGAL;
	}
}
