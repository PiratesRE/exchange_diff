using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class OutboundProxyBySourceTracker
	{
		public OutboundProxyBySourceTracker(string resourceForestMatchingDomains)
		{
			ArgumentValidator.ThrowIfNull("resourceForestMatchingDomains", resourceForestMatchingDomains);
			if (!string.IsNullOrEmpty(resourceForestMatchingDomains))
			{
				OutboundProxyBySourceTracker.O365Domains = resourceForestMatchingDomains.Split(new char[]
				{
					'|'
				});
			}
			this.smtpProxyTracker = new ConnectionsTracker(new ConnectionsTracker.GetExPerfCounterDelegate(OutboundProxyBySourceTracker.GetConnectionsCurrentCounter), new ConnectionsTracker.GetExPerfCounterDelegate(OutboundProxyBySourceTracker.GetConnectionsTotalCounter));
		}

		public static ExPerformanceCounter GetConnectionsCurrentCounter(string ehloDomain)
		{
			string office365DomainFromEhlo = OutboundProxyBySourceTracker.GetOffice365DomainFromEhlo(ehloDomain);
			return OutboundProxyBySourcePerfCounters.GetInstance(office365DomainFromEhlo).ConnectionsCurrent;
		}

		public static ExPerformanceCounter GetConnectionsTotalCounter(string ehloDomain)
		{
			string office365DomainFromEhlo = OutboundProxyBySourceTracker.GetOffice365DomainFromEhlo(ehloDomain);
			return OutboundProxyBySourcePerfCounters.GetInstance(office365DomainFromEhlo).ConnectionsTotal;
		}

		public void IncrementProxyCount(string forest)
		{
			ArgumentValidator.ThrowIfNull("forest", forest);
			this.smtpProxyTracker.IncrementProxyCount(forest);
		}

		public void DecrementProxyCount(string forest)
		{
			ArgumentValidator.ThrowIfNull("forest", forest);
			this.smtpProxyTracker.DecrementProxyCount(forest);
		}

		public bool TryGetDiagnosticInfo(DiagnosableParameters parameters, out XElement diagnosticInfo)
		{
			diagnosticInfo = null;
			return false;
		}

		internal static string GetOffice365DomainFromEhlo(string ehloDomain)
		{
			string result = "Forest-UNKNOWN";
			if (!string.IsNullOrEmpty(ehloDomain) && OutboundProxyBySourceTracker.O365Domains != null)
			{
				int num = ehloDomain.IndexOf('.');
				if (num != -1 && num < ehloDomain.Length - 1)
				{
					string text = ehloDomain.Substring(num + 1);
					foreach (string value in OutboundProxyBySourceTracker.O365Domains)
					{
						if (!string.IsNullOrEmpty(value) && text.EndsWith(value, StringComparison.OrdinalIgnoreCase))
						{
							result = text;
							break;
						}
					}
				}
			}
			return result;
		}

		private const string UnknownResourceForest = "Forest-UNKNOWN";

		private static string[] O365Domains;

		private ConnectionsTracker smtpProxyTracker;
	}
}
