using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Move", "AddressList", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class MoveAddressList : SystemConfigurationObjectActionTask<AddressListIdParameter, AddressBookBase>
	{
		[Parameter(Mandatory = true)]
		public AddressListIdParameter Target
		{
			get
			{
				return (AddressListIdParameter)base.Fields["Target"];
			}
			set
			{
				base.Fields["Target"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageMoveAddressList(this.Identity.ToString(), this.Target.ToString());
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			this.targetContainer = (AddressBookBase)base.GetDataObject<AddressBookBase>(this.Target, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorAddressListNotFound(this.Target.ToString())), new LocalizedString?(Strings.ErrorAddressListNotUnique(this.Target.ToString())));
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			this.DataObject = (AddressBookBase)base.PrepareDataObject();
			if (!this.DataObject.OrganizationId.Equals(this.targetContainer.OrganizationId))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorMoveAddressListAcrossOrganization(this.DataObject.Identity.ToString(), this.targetContainer.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (!base.HasErrors)
			{
				if (this.DataObject.IsTopContainer)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorInvalidOperationOnContainer(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
				else if (this.targetContainer.DistinguishedName.EndsWith(this.DataObject.DistinguishedName, StringComparison.InvariantCultureIgnoreCase))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorMoveAddressListToChildOrSelf(this.DataObject.Identity.ToString(), this.targetContainer.Identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
				else
				{
					this.DataObject.DistinguishedName = this.targetContainer.Id.GetChildId(this.DataObject.Name).DistinguishedName;
				}
			}
			TaskLogger.LogExit();
			return this.DataObject;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			AddressListIdParameter addressListIdParameter = new AddressListIdParameter(this.DataObject.Id);
			AddressBookBase dataObject = (AddressBookBase)base.GetDataObject<AddressBookBase>(addressListIdParameter, base.DataSession, null, new LocalizedString?(Strings.ErrorAddressListNotFound(addressListIdParameter.ToString())), new LocalizedString?(Strings.ErrorAddressListNotUnique(addressListIdParameter.ToString())));
			base.WriteObject(new AddressList(dataObject));
			TaskLogger.LogExit();
		}

		private const string ParameterTarget = "Target";

		private AddressBookBase targetContainer;
	}
}
