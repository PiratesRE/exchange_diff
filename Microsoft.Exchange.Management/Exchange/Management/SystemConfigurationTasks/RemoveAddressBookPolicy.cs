using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "AddressBookPolicy", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveAddressBookPolicy : RemoveMailboxPolicyBase<AddressBookMailboxPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveAddressBookPolicy(this.Identity.ToString());
			}
		}

		protected override bool HandleRemoveWithAssociatedUsers()
		{
			base.WriteError(new InvalidOperationException(Strings.ErrorRemoveAddressBookPolicyWithAssociatedUsers(base.DataObject.Name)), ErrorCategory.InvalidOperation, base.DataObject.Identity);
			return false;
		}
	}
}
