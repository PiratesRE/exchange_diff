using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("remove", "ActiveSyncMailboxPolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveActiveSyncMailboxPolicy : RemoveMailboxPolicyBase<ActiveSyncMailboxPolicy>
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
					return Strings.ConfirmationMessageRemoveDefaultActiveSyncMailboxPolicy(this.Identity.ToString());
				}
				return Strings.ConfirmationMessageRemoveActiveSyncMailboxPolicy(this.Identity.ToString());
			}
		}

		protected override bool HandleRemoveWithAssociatedUsers()
		{
			return this.Force || base.ShouldContinue(Strings.WarningRemovePolicyWithAssociatedUsers(base.DataObject.Name));
		}

		protected override void InternalProcessRecord()
		{
			this.WriteWarning(Strings.WarningCmdletIsDeprecated("Remove-ActiveSyncMailboxPolicy", "Remove-MobileMailboxPolicy"));
			base.InternalProcessRecord();
		}

		private SwitchParameter force;
	}
}
