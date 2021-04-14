using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal sealed class UnhealthyTargetFilterConfiguration
	{
		public UnhealthyTargetFilterConfiguration(bool enabled, int glitchRetryCount, TimeSpan glitchRetryInterval, int transientFailureRetryCount, TimeSpan transientFailureRetryInterval, TimeSpan outboundConnectionFailureRetryInterval)
		{
			this.enabled = enabled;
			this.glitchRetryCount = glitchRetryCount;
			this.glitchRetryInterval = glitchRetryInterval;
			this.transientFailureRetryCount = transientFailureRetryCount;
			this.transientFailureRetryInterval = transientFailureRetryInterval;
			this.outboundConnectionFailureRetryInterval = outboundConnectionFailureRetryInterval;
		}

		public UnhealthyTargetFilterConfiguration() : this(Components.Configuration.AppConfig.RemoteDelivery.LoadBalancingForServerFailoverEnabled, Components.Configuration.AppConfig.RemoteDelivery.QueueGlitchRetryCount, Components.Configuration.AppConfig.RemoteDelivery.QueueGlitchRetryInterval, Components.Configuration.LocalServer.TransportServer.TransientFailureRetryCount, Components.Configuration.LocalServer.TransportServer.TransientFailureRetryInterval, Components.Configuration.LocalServer.TransportServer.OutboundConnectionFailureRetryInterval)
		{
		}

		internal void AddDiagnosticInfoTo(XElement unhealthyTargetFiltererConfigElement)
		{
			unhealthyTargetFiltererConfigElement.Add(new object[]
			{
				new XElement("Enabled", this.Enabled),
				new XElement("GlitchRetryCount", this.GlitchRetryCount),
				new XElement("GlitchRetryInterval", this.GlitchRetryInterval),
				new XElement("TransientFailureRetryCount", this.TransientFailureRetryCount),
				new XElement("TransientFailureRetryInterval", this.TransientFailureRetryInterval),
				new XElement("OutboundConnectionFailureRetryInterval", this.OutboundConnectionFailureRetryInterval)
			});
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
		}

		public int GlitchRetryCount
		{
			get
			{
				return this.glitchRetryCount;
			}
		}

		public TimeSpan GlitchRetryInterval
		{
			get
			{
				return this.glitchRetryInterval;
			}
		}

		public int TransientFailureRetryCount
		{
			get
			{
				return this.transientFailureRetryCount;
			}
		}

		public TimeSpan TransientFailureRetryInterval
		{
			get
			{
				return this.transientFailureRetryInterval;
			}
		}

		public TimeSpan OutboundConnectionFailureRetryInterval
		{
			get
			{
				return this.outboundConnectionFailureRetryInterval;
			}
		}

		public void ConfigChanged(bool enabled, int glitchRetryCount, TimeSpan glitchRetryInterval, int transientFailureRetryCount, TimeSpan transientFailureRetryInterval, TimeSpan outboundConnectionFailureRetryInterval)
		{
			this.glitchRetryCount = glitchRetryCount;
			this.glitchRetryInterval = glitchRetryInterval;
			this.transientFailureRetryCount = transientFailureRetryCount;
			this.transientFailureRetryInterval = transientFailureRetryInterval;
			this.outboundConnectionFailureRetryInterval = outboundConnectionFailureRetryInterval;
		}

		private bool enabled;

		private int glitchRetryCount;

		private TimeSpan glitchRetryInterval;

		private int transientFailureRetryCount;

		private TimeSpan transientFailureRetryInterval;

		private TimeSpan outboundConnectionFailureRetryInterval;
	}
}
