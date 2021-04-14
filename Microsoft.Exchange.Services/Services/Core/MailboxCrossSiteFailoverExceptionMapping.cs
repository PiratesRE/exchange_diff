using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class MailboxCrossSiteFailoverExceptionMapping : MailboxFailoverExceptionMapping
	{
		public MailboxCrossSiteFailoverExceptionMapping() : base(typeof(MailboxCrossSiteFailoverException))
		{
		}

		protected override void DoServiceErrorPostProcessing(LocalizedException exception, ServiceError error)
		{
			MailboxCrossSiteFailoverException ex = base.VerifyExceptionType<MailboxCrossSiteFailoverException>(exception);
			ExTraceGlobals.SessionCacheTracer.TraceDebug<string>(0L, "[MailboxCrossSiteFailoverExceptionMapping::DoServiceErrorPostProcessing] Encountered failover for mailbox database: '{0}'", ex.DatabaseLocationInfo.DatabaseLegacyDN);
			if (CallContext.Current != null)
			{
				CallContext.Current.SessionCache.ReturnStoreSessionsToCache = false;
			}
			base.DoServiceErrorPostProcessing(exception, error);
		}
	}
}
