using System;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class LiveIdInstanceQueryProcessor : RbacQuery.RbacQueryProcessor
	{
		internal LiveIdInstanceQueryProcessor(LiveIdInstanceType liveIdInstanceType)
		{
			this.liveIdInstanceType = liveIdInstanceType;
		}

		public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			SmtpAddress smtpAddress;
			if (rbacConfiguration.TryGetExecutingWindowsLiveId(out smtpAddress) && smtpAddress.IsValidAddress)
			{
				DomainCacheValue domainCacheValue = DomainCache.Singleton.Get(new SmtpDomainWithSubdomains(smtpAddress.Domain), rbacConfiguration.ExecutingUserOrganizationId);
				return new bool?(domainCacheValue != null && domainCacheValue.LiveIdInstanceType == this.liveIdInstanceType);
			}
			return null;
		}

		internal const string BusinessLiveIdRoleName = "BusinessLiveId";

		internal const string ConsumerLiveIdRoleName = "ConsumerLiveId";

		private readonly LiveIdInstanceType liveIdInstanceType;
	}
}
