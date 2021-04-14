using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal abstract class ResourceMeter : IResourceMeter
	{
		protected ResourceMeter(string resourceName, string resourceInstanceName, PressureTransitions pressureTransitions)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("resourceName", resourceName);
			ArgumentValidator.ThrowIfNull("resourceInstanceName", resourceInstanceName);
			this.resourceIdentifier = new ResourceIdentifier(resourceName, resourceInstanceName);
			this.pressureTransitions = pressureTransitions;
			this.resourceUse = new ResourceUse(this.resourceIdentifier, UseLevel.Low, UseLevel.Low);
		}

		public ResourceIdentifier Resource
		{
			get
			{
				return this.resourceIdentifier;
			}
		}

		public long Pressure
		{
			get
			{
				return this.pressure;
			}
		}

		public PressureTransitions PressureTransitions
		{
			get
			{
				return this.pressureTransitions;
			}
		}

		public ResourceUse ResourceUse
		{
			get
			{
				return this.resourceUse;
			}
		}

		public ResourceUse RawResourceUse
		{
			get
			{
				return this.resourceUse;
			}
		}

		public void Refresh()
		{
			this.pressure = this.GetCurrentPressure();
			UseLevel useLevel = this.pressureTransitions.GetUseLevel(this.pressure, this.resourceUse.PreviousUseLevel);
			this.resourceUse = new ResourceUse(this.resourceIdentifier, useLevel, this.resourceUse.CurrentUseLevel);
		}

		protected abstract long GetCurrentPressure();

		private readonly ResourceIdentifier resourceIdentifier;

		private ResourceUse resourceUse;

		private long pressure;

		private PressureTransitions pressureTransitions;
	}
}
