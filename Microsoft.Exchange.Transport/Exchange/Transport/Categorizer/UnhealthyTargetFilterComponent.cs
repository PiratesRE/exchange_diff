using System;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class UnhealthyTargetFilterComponent : ITransportComponent
	{
		public UnhealthyTargetFilter<IPAddressPortPair> UnhealthyTargetIPAddressFilter
		{
			get
			{
				return this.unhealthyTargetIpAddressFilter;
			}
		}

		public UnhealthyTargetFilter<FqdnPortPair> UnhealthyTargetFqdnFilter
		{
			get
			{
				return this.unhealthyTargetFqdnFilter;
			}
		}

		public UnhealthyTargetFilterConfiguration UnhealthyTargetFilterConfiguration
		{
			get
			{
				return this.unhealthyTargetFilterConfiguration;
			}
		}

		public void Load()
		{
			this.unhealthyTargetFilterConfiguration = new UnhealthyTargetFilterConfiguration();
			this.unhealthyTargetIpAddressFilter = new UnhealthyTargetFilter<IPAddressPortPair>(this.unhealthyTargetFilterConfiguration, null, null);
			this.unhealthyTargetFqdnFilter = new UnhealthyTargetFilter<FqdnPortPair>(this.unhealthyTargetFilterConfiguration, null, null);
			Components.Configuration.LocalServerChanged += this.UpdateConfiguration;
		}

		public void Unload()
		{
			Components.Configuration.LocalServerChanged -= this.UpdateConfiguration;
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public void CleanupExpiredEntries()
		{
			UnhealthyTargetFilter<FqdnPortPair> unhealthyTargetFilter = this.unhealthyTargetFqdnFilter;
			if (unhealthyTargetFilter != null)
			{
				unhealthyTargetFilter.CleanupExpiredEntries();
			}
			UnhealthyTargetFilter<IPAddressPortPair> unhealthyTargetFilter2 = this.unhealthyTargetIpAddressFilter;
			if (unhealthyTargetFilter2 != null)
			{
				unhealthyTargetFilter2.CleanupExpiredEntries();
			}
		}

		private void UpdateConfiguration(TransportServerConfiguration transportServerConfiguration)
		{
			if (this.unhealthyTargetFilterConfiguration != null)
			{
				this.unhealthyTargetFilterConfiguration.ConfigChanged(Components.Configuration.AppConfig.RemoteDelivery.LoadBalancingForServerFailoverEnabled, Components.Configuration.AppConfig.RemoteDelivery.QueueGlitchRetryCount, Components.Configuration.AppConfig.RemoteDelivery.QueueGlitchRetryInterval, transportServerConfiguration.TransportServer.TransientFailureRetryCount, transportServerConfiguration.TransportServer.TransientFailureRetryInterval, transportServerConfiguration.TransportServer.OutboundConnectionFailureRetryInterval);
			}
		}

		public TimeSpan GetServerLatency(IPAddressPortPair target)
		{
			return this.unhealthyTargetIpAddressFilter.GetServerLatency(target);
		}

		public void UpdateServerLatency(SmtpOutTargetHostPicker.SmtpTarget smtpTarget, TimeSpan latency)
		{
			this.unhealthyTargetFqdnFilter.UpdateServerLatency(new FqdnPortPair(smtpTarget.TargetHostName, smtpTarget.Port), latency);
			this.UnhealthyTargetIPAddressFilter.UpdateServerLatency(new IPAddressPortPair(smtpTarget.Address, smtpTarget.Port), latency);
		}

		private UnhealthyTargetFilter<IPAddressPortPair> unhealthyTargetIpAddressFilter;

		private UnhealthyTargetFilter<FqdnPortPair> unhealthyTargetFqdnFilter;

		private UnhealthyTargetFilterConfiguration unhealthyTargetFilterConfiguration;
	}
}
