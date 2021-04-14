using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	internal class DnsRemoteConfiguration : DnsConfiguration
	{
		public DnsRemoteConfiguration(XmlElement configNode, TracingContext traceContext) : base(configNode, traceContext, false)
		{
		}

		protected override void InitDnsServerIps(bool useSingleDnsServer)
		{
			if (base.AllZones.Count > 0)
			{
				string text = base.AllZones.First<string>();
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DNSTracer, base.TraceContext, "Using zone={0} to the find DNS server Ips", text, null, "InitDnsServerIps", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsRemoteConfiguration.cs", 40);
				IEnumerable<IPAddress> nsipEndPointsForDomain = DnsUtils.GetNSIpEndPointsForDomain(text);
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DNSTracer, base.TraceContext, "DNS server Ips ={0}", string.Join<IPAddress>(",", nsipEndPointsForDomain), null, "InitDnsServerIps", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsRemoteConfiguration.cs", 45);
				if (useSingleDnsServer)
				{
					base.DnsServerIps = nsipEndPointsForDomain.Take(1).ToList<IPAddress>();
					return;
				}
				base.DnsServerIps = nsipEndPointsForDomain.Distinct<IPAddress>().ToList<IPAddress>();
			}
		}

		protected override void InitSupportedZones()
		{
			throw new DnsMonitorException("Cannot AutoDetect from a remote box, specify a SupportedZones node in WorkContext with AutoDetect='false'", null);
		}

		protected override void InitSupportedTargetServices()
		{
			throw new DnsMonitorException("Cannot AutoDetect from a remote box, specify a SupportedTargetServices node in WorkContext with AutoDetect='false'", null);
		}

		protected override void InitMonitorDomain()
		{
			throw new DnsMonitorException("Cannot AutoDetect from a remote box, specify a MonitorDomain node in WorkContext with AutoDetect='false'", null);
		}

		protected override void InitIpV6Prefix()
		{
			throw new DnsMonitorException("Cannot AutoDetect from a remote box, specify a IPV6 prefix node in WorkContext with AutoDetect='false'", null);
		}
	}
}
