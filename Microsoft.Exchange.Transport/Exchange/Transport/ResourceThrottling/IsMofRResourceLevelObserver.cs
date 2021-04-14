using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.RecipientAPI;

namespace Microsoft.Exchange.Transport.ResourceThrottling
{
	internal class IsMofRResourceLevelObserver : IResourceLevelObserver
	{
		public IsMofRResourceLevelObserver(IsMemberOfResolverComponent<RoutingAddress> isMemberOfResolver, IComponentsWrapper componentsWrapper)
		{
			ArgumentValidator.ThrowIfNull("isMemberOfResolver", isMemberOfResolver);
			ArgumentValidator.ThrowIfNull("componentsWrapper", componentsWrapper);
			this.isMemberOfResolver = isMemberOfResolver;
			this.componentsWrapper = componentsWrapper;
		}

		public virtual void HandleResourceChange(IEnumerable<ResourceUse> allResourceUses, IEnumerable<ResourceUse> changedResourceUses, IEnumerable<ResourceUse> rawResourceUses)
		{
			ArgumentValidator.ThrowIfNull("allResourceUses", allResourceUses);
			ArgumentValidator.ThrowIfNull("changedResourceUses", changedResourceUses);
			ArgumentValidator.ThrowIfNull("rawResourceUses", rawResourceUses);
			ResourceUse resourceUse = ResourceHelper.TryGetResourceUse(allResourceUses, this.privateBytesResource);
			if (resourceUse == null)
			{
				return;
			}
			if (resourceUse.CurrentUseLevel != UseLevel.Low && resourceUse.CurrentUseLevel > resourceUse.PreviousUseLevel && !this.componentsWrapper.IsShuttingDown && this.componentsWrapper.IsActive)
			{
				lock (this.componentsWrapper.SyncRoot)
				{
					if (this.isMemberOfResolver != null && !this.componentsWrapper.IsShuttingDown && this.componentsWrapper.IsActive)
					{
						this.isMemberOfResolver.ClearCache();
					}
				}
			}
		}

		public string Name
		{
			get
			{
				return "IsMemberOfResolver";
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

		internal const string ResourceObserverName = "IsMemberOfResolver";

		private readonly IsMemberOfResolverComponent<RoutingAddress> isMemberOfResolver;

		private readonly IComponentsWrapper componentsWrapper;

		private readonly ResourceIdentifier privateBytesResource = new ResourceIdentifier("PrivateBytes", "");
	}
}
