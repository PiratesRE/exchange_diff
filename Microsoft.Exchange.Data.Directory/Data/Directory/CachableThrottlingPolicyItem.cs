using System;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class CachableThrottlingPolicyItem : CachableItem
	{
		public CachableThrottlingPolicyItem(IThrottlingPolicy policy)
		{
			this.ThrottlingPolicy = policy;
		}

		public IThrottlingPolicy ThrottlingPolicy { get; private set; }

		public override long ItemSize
		{
			get
			{
				return 1L;
			}
		}
	}
}
