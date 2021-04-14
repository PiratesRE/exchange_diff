using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "MailUser", SupportsShouldProcess = true, DefaultParameterSetName = "DisabledUser")]
	public sealed class NewMailUser : NewMailUserBase
	{
		public NewMailUser()
		{
			base.NumberofCalls = ProvisioningCounters.NumberOfNewMailuserCalls;
			base.NumberofSuccessfulCalls = ProvisioningCounters.NumberOfSuccessfulNewMailuserCalls;
			base.AverageTimeTaken = ProvisioningCounters.AverageNewMailuserResponseTime;
			base.AverageBaseTimeTaken = ProvisioningCounters.AverageNewMailuserResponseTimeBase;
			base.AverageTimeTakenWithCache = ProvisioningCounters.AverageNewMailuserResponseTimeWithCache;
			base.AverageBaseTimeTakenWithCache = ProvisioningCounters.AverageNewMailuserResponseTimeBaseWithCache;
			base.AverageTimeTakenWithoutCache = ProvisioningCounters.AverageNewMailuserResponseTimeWithoutCache;
			base.AverageBaseTimeTakenWithoutCache = ProvisioningCounters.AverageNewMailuserResponseTimeBaseWithoutCache;
			base.TotalResponseTime = ProvisioningCounters.TotalNewMailuserResponseTime;
			base.CacheActivePercentage = ProvisioningCounters.NewMailuserCacheActivePercentage;
			base.CacheActiveBasePercentage = ProvisioningCounters.NewMailuserCacheActivePercentageBase;
		}

		protected override void WriteResult(ADObject result)
		{
			TaskLogger.LogEnter(new object[]
			{
				result.Identity
			});
			MailUser result2 = new MailUser((ADUser)result);
			base.WriteResult(result2);
			TaskLogger.LogExit();
		}
	}
}
