using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Remove", "UMMailboxPolicy", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveUMPolicy : RemoveMailboxPolicyBase<UMMailboxPolicy>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveUMMailboxPolicy(this.Identity.ToString());
			}
		}

		protected override bool HandleRemoveWithAssociatedUsers()
		{
			base.WriteError(new CannotDeleteAssociatedMailboxPolicyException(this.Identity.ToString()), ErrorCategory.WriteError, base.DataObject);
			return false;
		}
	}
}
