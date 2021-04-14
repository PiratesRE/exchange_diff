using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Remove", "ExchangeUpgradeBucket", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveExchangeUpgradeBucket : RemoveSystemConfigurationObjectTask<ExchangeUpgradeBucketIdParameter, ExchangeUpgradeBucket>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveExchangeUpgradeBucket(base.DataObject.Name);
			}
		}
	}
}
