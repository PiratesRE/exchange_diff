using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Set", "ExchangeUpgradeBucket", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetExchangeUpgradeBucket : SetTopologySystemConfigurationObjectTask<ExchangeUpgradeBucketIdParameter, ExchangeUpgradeBucket>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetExchangeUpgradeBucket(this.DataObject.Name);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (this.DataObject.IsChanged(ExchangeUpgradeBucketSchema.MaxMailboxes) && !this.DataObject.MaxMailboxes.IsUnlimited && this.DataObject.Organizations.Count > 0)
			{
				int mailboxCount = UpgradeBucketTaskHelper.GetMailboxCount(this.DataObject);
				if (this.DataObject.MaxMailboxes.Value < mailboxCount)
				{
					base.WriteError(new RecipientTaskException(Strings.ExchangeUpgradeBucketInvalidCapacityValue), ExchangeErrorCategory.Client, this.DataObject);
				}
			}
			TaskLogger.LogExit();
		}
	}
}
