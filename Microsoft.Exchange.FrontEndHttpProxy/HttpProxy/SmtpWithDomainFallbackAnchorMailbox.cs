using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpProxy
{
	internal class SmtpWithDomainFallbackAnchorMailbox : SmtpAnchorMailbox
	{
		public SmtpWithDomainFallbackAnchorMailbox(string smtp, IRequestContext requestContext) : base(smtp, requestContext)
		{
		}

		protected override ADRawEntry LoadADRawEntry()
		{
			IRecipientSession session = DirectoryHelper.GetRecipientSessionFromSmtpOrLiveId(base.RequestContext.LatencyTracker, base.Smtp, false);
			ADRawEntry adrawEntry = DirectoryHelper.InvokeAccountForest(base.RequestContext.LatencyTracker, () => session.FindByProxyAddress(new SmtpProxyAddress(this.Smtp, true), this.PropertySet));
			if (adrawEntry == null)
			{
				PerfCounters.HttpProxyCountersInstance.RedirectByTenantMailboxCount.Increment();
				adrawEntry = DirectoryHelper.InvokeAccountForest(base.RequestContext.LatencyTracker, () => HttpProxyBackEndHelper.GetDefaultOrganizationMailbox(session, this.ToCookieKey()));
			}
			else
			{
				PerfCounters.HttpProxyCountersInstance.RedirectBySenderMailboxCount.Increment();
			}
			return base.CheckForNullAndThrowIfApplicable<ADRawEntry>(adrawEntry);
		}
	}
}
