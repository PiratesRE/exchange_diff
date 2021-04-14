using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("remove", "ManagedFolderMailboxPolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveManagedFolderMailboxPolicy : RemoveMailboxPolicyBase<ManagedFolderMailboxPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveManagedFolderMailboxPolicy(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		protected override bool HandleRemoveWithAssociatedUsers()
		{
			return this.Force || base.ShouldContinue(Strings.WarningRemovePolicyWithAssociatedUsers(base.DataObject.Name));
		}
	}
}
