using System;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal sealed class MServeDomainQueryList : QueryListBase<MServeQueryResult>
	{
		protected override MServeQueryResult CreateResult(UserResultMapping userResultMapping)
		{
			return new MServeQueryResult(userResultMapping);
		}

		public override void Execute()
		{
			ExTraceGlobals.FrameworkTracer.TraceDebug<MServeDomainQueryList, int>((long)this.GetHashCode(), "{0} Execute() called for {1} addresses.", this, this.resultDictionary.Values.Count);
			foreach (MServeQueryResult mserveQueryResult in this.resultDictionary.Values)
			{
				mserveQueryResult.RedirectServer = MserveDomainCache.Singleton.Get(mserveQueryResult.UserResultMapping.SmtpAddress.Domain);
			}
		}
	}
}
