using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class RemoveDistributionGroupBase : RemoveRecipientObjectTask<DistributionGroupIdParameter, ADGroup>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveDistributionGroup(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter BypassSecurityGroupManagerCheck
		{
			get
			{
				return (SwitchParameter)(base.Fields["BypassSecurityGroupManagerCheck"] ?? false);
			}
			set
			{
				base.Fields["BypassSecurityGroupManagerCheck"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.DataObject.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox || base.DataObject.RecipientTypeDetails == RecipientTypeDetails.RemoteGroupMailbox)
			{
				base.WriteError(new RecipientTaskException(Strings.NotAValidDistributionGroup), ExchangeErrorCategory.Client, this.Identity.ToString());
			}
			DistributionGroupMemberTaskBase<DistributionGroupIdParameter>.GetExecutingUserAndCheckGroupOwnership(this, (IDirectorySession)base.DataSession, base.TenantGlobalCatalogSession, base.DataObject, this.BypassSecurityGroupManagerCheck);
		}
	}
}
