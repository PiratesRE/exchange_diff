using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.ResourceThrottling
{
	internal class ResourceLevelObserver : IResourceLevelObserver
	{
		public ResourceLevelObserver(string componentName, IStartableTransportComponent transportComponent, IEnumerable<ResourceIdentifier> observedResources = null)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("componentName", componentName);
			ArgumentValidator.ThrowIfNull("transportComponent", transportComponent);
			this.resourceObserverName = componentName;
			this.transportComponent = transportComponent;
			if (observedResources != null && observedResources.Any<ResourceIdentifier>())
			{
				this.observedResources = observedResources;
			}
		}

		public IStartableTransportComponent TransportComponent
		{
			get
			{
				return this.transportComponent;
			}
		}

		public virtual void HandleResourceChange(IEnumerable<ResourceUse> allResourceUses, IEnumerable<ResourceUse> changedResourceUses, IEnumerable<ResourceUse> rawResourceUses)
		{
			ArgumentValidator.ThrowIfNull("allResourceUses", allResourceUses);
			ArgumentValidator.ThrowIfNull("changedResourceUses", changedResourceUses);
			ArgumentValidator.ThrowIfNull("rawResourceUses", rawResourceUses);
			IEnumerable<ResourceUse> source = from resourceUse in allResourceUses
			where this.observedResources.Any((ResourceIdentifier resource) => resourceUse.Resource.Equals(resource))
			select resourceUse;
			if (source.Any<ResourceUse>())
			{
				if (source.All((ResourceUse resourceUse) => resourceUse.CurrentUseLevel == UseLevel.Low))
				{
					if (this.componentPaused)
					{
						this.transportComponent.Continue();
						this.componentPaused = false;
						return;
					}
				}
				else if (!this.componentPaused)
				{
					this.transportComponent.Pause();
					this.componentPaused = true;
				}
			}
		}

		public string Name
		{
			get
			{
				return this.resourceObserverName;
			}
		}

		public bool Paused
		{
			get
			{
				return this.componentPaused;
			}
		}

		public virtual string SubStatus
		{
			get
			{
				return string.Empty;
			}
		}

		private readonly IStartableTransportComponent transportComponent;

		private readonly string resourceObserverName;

		private bool componentPaused;

		private readonly IEnumerable<ResourceIdentifier> observedResources = new List<ResourceIdentifier>
		{
			new ResourceIdentifier("Aggregate", "")
		};
	}
}
