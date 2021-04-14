using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal sealed class ADServerMetrics
	{
		public ADServerMetrics(ADServer dc)
		{
			this.ServerId = dc.Id;
			this.DnsHostName = dc.DnsHostName;
		}

		public ADServerMetrics(string dnsHostName)
		{
			this.DnsHostName = dnsHostName;
		}

		public ADObjectId ServerId { get; private set; }

		public string DnsHostName { get; private set; }

		public bool IsSuitable { get; internal set; }

		public LocalizedString ErrorMessage { get; internal set; }

		public bool IsSynchronized { get; internal set; }

		public ExDateTime UpdateTime { get; private set; }

		public long HighestUsn { get; private set; }

		public double InjectionRate { get; private set; }

		public long IncomingDebt { get; set; }

		public long OutgoingDebt { get; set; }

		public int IncomingHealth { get; set; }

		public int OutgoingHealth { get; set; }

		public ICollection<ADReplicationLinkMetrics> DomainReplicationMetrics { get; private set; }

		public ICollection<ADReplicationLinkMetrics> ConfigReplicationMetrics { get; private set; }

		internal void Initialize(ExDateTime updateTime, long highestUsn, ICollection<ADReplicationLinkMetrics> configReplicationMetrics, ICollection<ADReplicationLinkMetrics> domainReplicationMetrics)
		{
			if (configReplicationMetrics == null)
			{
				throw new ArgumentNullException("configReplicationMetrics");
			}
			if (domainReplicationMetrics == null)
			{
				throw new ArgumentNullException("domainReplicationMetrics");
			}
			if (highestUsn <= 0L)
			{
				throw new ArgumentException(null, "highestUsn");
			}
			this.UpdateTime = updateTime;
			this.HighestUsn = highestUsn;
			this.ConfigReplicationMetrics = configReplicationMetrics;
			this.DomainReplicationMetrics = domainReplicationMetrics;
		}

		internal void SetInjectionRate(ADServerMetrics previousMetrics)
		{
			if (!this.IsSuitable)
			{
				throw new InvalidOperationException();
			}
			if (previousMetrics != null && previousMetrics.IsSuitable)
			{
				this.InjectionRate = (double)(this.HighestUsn - previousMetrics.HighestUsn) / (this.UpdateTime - previousMetrics.UpdateTime).TotalSeconds;
			}
			else
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "[ADServerMetrics::SetInjectionRate] Could not calculate actual injection rate for {0} because there no previous data for this DC.", this.DnsHostName);
				this.InjectionRate = (double)this.HighestUsn;
			}
			ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<string, double>((long)this.GetHashCode(), "[ADServerMetrics::SetInjectionRate] {0}: InjectionRate={1}.", this.DnsHostName, this.InjectionRate);
		}
	}
}
