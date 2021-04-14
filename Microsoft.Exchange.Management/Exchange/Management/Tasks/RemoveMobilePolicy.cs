using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("remove", "MobileDeviceMailboxPolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMobilePolicy : RemoveMailboxPolicyBase<MobileMailboxPolicy>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return this.force;
			}
			set
			{
				this.force = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (base.DataObject.IsDefault)
				{
					return Strings.ConfirmationMessageRemoveDefaultMobileMailboxPolicy(this.Identity.ToString());
				}
				return Strings.ConfirmationMessageRemoveMobileMailboxPolicy(this.Identity.ToString());
			}
		}

		protected override bool HandleRemoveWithAssociatedUsers()
		{
			return this.Force || base.ShouldContinue(Strings.WarningRemovePolicyWithAssociatedUsers(base.DataObject.Name));
		}

		private SwitchParameter force;
	}
}
