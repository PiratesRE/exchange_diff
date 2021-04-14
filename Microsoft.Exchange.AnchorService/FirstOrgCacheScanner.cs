using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.AnchorService
{
	internal class FirstOrgCacheScanner : CacheScanner
	{
		internal FirstOrgCacheScanner(AnchorContext context, WaitHandle stopEvent) : base(context, stopEvent)
		{
		}

		internal override string Name
		{
			get
			{
				return "FirstOrgCacheScanner";
			}
		}

		protected override IEnumerable<ADUser> GetLocalMailboxUsers()
		{
			return AnchorADProvider.GetRootOrgProvider(base.Context).GetOrganizationMailboxesByCapability(base.Context.ActiveCapability);
		}
	}
}
