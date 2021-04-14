using System;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Transport.ResourceThrottling
{
	internal class ShadowRedundancyResourceLevelObserver : ResourceLevelObserver
	{
		public ShadowRedundancyResourceLevelObserver(ShadowRedundancyComponent shadowRedundancyComponent) : base("ShadowRedundancy", shadowRedundancyComponent, null)
		{
		}

		internal const string ResourceObserverName = "ShadowRedundancy";
	}
}
