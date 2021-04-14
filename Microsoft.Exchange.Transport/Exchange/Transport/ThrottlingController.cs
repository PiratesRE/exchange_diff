using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal sealed class ThrottlingController
	{
		public ThrottlingController(Trace tracer, ResourceManagerConfiguration.ThrottlingControllerConfiguration config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			this.tracer = tracer;
			this.throttleDelayBasedOnPressure = TimeSpan.Zero;
			this.config = config;
			this.maxPressureBasedThrottlingDelayInterval = this.config.MaxThrottlingDelayInterval - this.config.BaseThrottlingDelayInterval;
		}

		public void Increase()
		{
			TimeSpan timeSpan = this.throttleDelayBasedOnPressure;
			if (timeSpan == TimeSpan.Zero)
			{
				timeSpan = this.config.StartThrottlingDelayInterval;
			}
			else
			{
				timeSpan = timeSpan.Add(this.config.StepThrottlingDelayInterval);
			}
			if (timeSpan > this.maxPressureBasedThrottlingDelayInterval)
			{
				timeSpan = this.maxPressureBasedThrottlingDelayInterval;
			}
			this.SetCurrent(timeSpan);
		}

		public void Decrease()
		{
			TimeSpan timeSpan = this.throttleDelayBasedOnPressure;
			timeSpan = timeSpan.Subtract(this.config.StepThrottlingDelayInterval);
			if (timeSpan < TimeSpan.Zero)
			{
				timeSpan = TimeSpan.Zero;
			}
			this.SetCurrent(timeSpan);
		}

		public TimeSpan GetCurrent()
		{
			return this.config.BaseThrottlingDelayInterval.Add(this.throttleDelayBasedOnPressure);
		}

		public void AddDiagnosticInfo(XElement parent, bool showState)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			this.config.AddDiagnosticInfo(parent);
			if (showState)
			{
				parent.Add(new object[]
				{
					new XElement("throttleDelayBasedOnPressure", this.throttleDelayBasedOnPressure),
					new XElement("maxPressureBasedThrottlingDelayInterval", this.maxPressureBasedThrottlingDelayInterval)
				});
			}
		}

		private void SetCurrent(TimeSpan newThrottlingInterval)
		{
			if (this.tracer != null && newThrottlingInterval != this.throttleDelayBasedOnPressure)
			{
				this.tracer.TraceDebug<TimeSpan, TimeSpan>(0L, "throttling interval changed from '{0}' to '{1}' (not including base delay).", this.throttleDelayBasedOnPressure, newThrottlingInterval);
			}
			this.throttleDelayBasedOnPressure = newThrottlingInterval;
		}

		private readonly TimeSpan maxPressureBasedThrottlingDelayInterval;

		private ResourceManagerConfiguration.ThrottlingControllerConfiguration config;

		private TimeSpan throttleDelayBasedOnPressure;

		private Trace tracer;
	}
}
