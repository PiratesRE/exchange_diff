using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Get", "ExchangeUpgradeBucket", DefaultParameterSetName = "Identity")]
	public sealed class GetExchangeUpgradeBucket : GetSystemConfigurationObjectTask<ExchangeUpgradeBucketIdParameter, ExchangeUpgradeBucket>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter EnableMailboxCounting { get; set; }

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			if (this.EnableMailboxCounting)
			{
				ExchangeUpgradeBucket exchangeUpgradeBucket = (ExchangeUpgradeBucket)dataObject;
				exchangeUpgradeBucket.MailboxCount = UpgradeBucketTaskHelper.GetMailboxCount(exchangeUpgradeBucket);
			}
			base.WriteResult(dataObject);
			TaskLogger.LogExit();
		}
	}
}
