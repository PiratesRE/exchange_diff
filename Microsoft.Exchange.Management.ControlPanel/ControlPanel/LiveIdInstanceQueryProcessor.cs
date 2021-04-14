using System;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class LiveIdInstanceQueryProcessor : RbacQuery.RbacQueryProcessor
	{
		internal LiveIdInstanceQueryProcessor(Func<DomainCacheValue, bool> predicate)
		{
			this.predicate = predicate;
		}

		public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			SmtpAddress smtpAddress = new SmtpAddress(HttpContextExtensions.CurrentUserLiveID());
			if (smtpAddress.IsValidAddress)
			{
				DomainCacheValue domainCacheValue = DomainCache.Singleton.Get(new SmtpDomainWithSubdomains(smtpAddress.Domain), RbacPrincipal.Current.RbacConfiguration.ExecutingUserOrganizationId);
				return new bool?(domainCacheValue != null && this.predicate(domainCacheValue));
			}
			return new bool?(false);
		}

		internal const string BusinessLiveIdRoleName = "BusinessLiveId";

		internal const string ConsumerLiveIdRoleName = "ConsumerLiveId";

		internal static readonly RbacQuery.RbacQueryProcessor BusinessLiveId = new LiveIdInstanceQueryProcessor((DomainCacheValue x) => x.LiveIdInstanceType == LiveIdInstanceType.Business);

		internal static readonly RbacQuery.RbacQueryProcessor ConsumerLiveId = new LiveIdInstanceQueryProcessor((DomainCacheValue x) => x.LiveIdInstanceType == LiveIdInstanceType.Consumer);

		private Func<DomainCacheValue, bool> predicate;
	}
}
