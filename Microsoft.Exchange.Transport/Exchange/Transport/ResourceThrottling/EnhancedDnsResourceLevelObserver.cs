using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Categorizer;

namespace Microsoft.Exchange.Transport.ResourceThrottling
{
	internal class EnhancedDnsResourceLevelObserver : IResourceLevelObserver
	{
		public EnhancedDnsResourceLevelObserver(EnhancedDns enhancedDns, IComponentsWrapper componentsWrapper)
		{
			ArgumentValidator.ThrowIfNull("enhancedDns", enhancedDns);
			ArgumentValidator.ThrowIfNull("componentsWrapper", componentsWrapper);
			this.enhancedDns = enhancedDns;
			this.componentsWrapper = componentsWrapper;
		}

		public virtual void HandleResourceChange(IEnumerable<ResourceUse> allResourceUses, IEnumerable<ResourceUse> changedResourceUses, IEnumerable<ResourceUse> rawResourceUses)
		{
			ArgumentValidator.ThrowIfNull("allResourceUses", allResourceUses);
			ArgumentValidator.ThrowIfNull("changedResourceUses", changedResourceUses);
			ArgumentValidator.ThrowIfNull("rawResourceUses", rawResourceUses);
			UseLevel useLevel = ResourceHelper.TryGetCurrentUseLevel(allResourceUses, this.privateBytesResource, UseLevel.Low);
			if (useLevel != UseLevel.Low)
			{
				this.enhancedDns.FlushCache();
				if (this.componentsWrapper.IsBridgeHead)
				{
					Schema.FlushCache();
				}
			}
		}

		public string Name
		{
			get
			{
				return "EnhancedDns";
			}
		}

		public bool Paused
		{
			get
			{
				return false;
			}
		}

		public string SubStatus
		{
			get
			{
				return string.Empty;
			}
		}

		internal const string ResourceObserverName = "EnhancedDns";

		private readonly EnhancedDns enhancedDns;

		private readonly IComponentsWrapper componentsWrapper;

		private readonly ResourceIdentifier privateBytesResource = new ResourceIdentifier("PrivateBytes", "");
	}
}
