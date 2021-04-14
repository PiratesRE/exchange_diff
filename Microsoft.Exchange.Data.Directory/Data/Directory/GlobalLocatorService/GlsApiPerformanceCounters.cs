using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal static class GlsApiPerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (GlsApiPerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in GlsApiPerformanceCounters.AllCounters)
			{
				try
				{
					element.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					element.Add(content);
				}
			}
		}

		public const string CategoryName = "MSExchange Global Locator APIs";

		public static readonly ExPerformanceCounter FindDomainAverageOverallLatency = new ExPerformanceCounter("MSExchange Global Locator APIs", "FindDomain Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FindDomainAverageOverallLatencyBase = new ExPerformanceCounter("MSExchange Global Locator APIs", "Base for FindDomain Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FindTenantAverageOverallLatency = new ExPerformanceCounter("MSExchange Global Locator APIs", "FindTenant Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FindTenantAverageOverallLatencyBase = new ExPerformanceCounter("MSExchange Global Locator APIs", "Base for FindTenant Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FindUserAverageOverallLatency = new ExPerformanceCounter("MSExchange Global Locator APIs", "FindUser Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter FindUserAverageOverallLatencyBase = new ExPerformanceCounter("MSExchange Global Locator APIs", "Base for FindUser Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SaveDomainAverageOverallLatency = new ExPerformanceCounter("MSExchange Global Locator APIs", "SaveDomain Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SaveDomainAverageOverallLatencyBase = new ExPerformanceCounter("MSExchange Global Locator APIs", "Base for SaveDomain Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SaveTenantAverageOverallLatency = new ExPerformanceCounter("MSExchange Global Locator APIs", "SaveTenant Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SaveTenantAverageOverallLatencyBase = new ExPerformanceCounter("MSExchange Global Locator APIs", "Base for SaveTenant Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SaveUserAverageOverallLatency = new ExPerformanceCounter("MSExchange Global Locator APIs", "SaveUser Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SaveUserAverageOverallLatencyBase = new ExPerformanceCounter("MSExchange Global Locator APIs", "Base for SaveUser Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeleteDomainAverageOverallLatency = new ExPerformanceCounter("MSExchange Global Locator APIs", "DeleteDomain Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeleteDomainAverageOverallLatencyBase = new ExPerformanceCounter("MSExchange Global Locator APIs", "Base for DeleteDomain Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeleteTenantAverageOverallLatency = new ExPerformanceCounter("MSExchange Global Locator APIs", "DeleteTenant Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeleteTenantAverageOverallLatencyBase = new ExPerformanceCounter("MSExchange Global Locator APIs", "Base for DeleteTenant Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeleteUserAverageOverallLatency = new ExPerformanceCounter("MSExchange Global Locator APIs", "DeleteUser Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DeleteUserAverageOverallLatencyBase = new ExPerformanceCounter("MSExchange Global Locator APIs", "Base for DeleteUser Average Overall Latency", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			GlsApiPerformanceCounters.FindDomainAverageOverallLatency,
			GlsApiPerformanceCounters.FindDomainAverageOverallLatencyBase,
			GlsApiPerformanceCounters.FindTenantAverageOverallLatency,
			GlsApiPerformanceCounters.FindTenantAverageOverallLatencyBase,
			GlsApiPerformanceCounters.FindUserAverageOverallLatency,
			GlsApiPerformanceCounters.FindUserAverageOverallLatencyBase,
			GlsApiPerformanceCounters.SaveDomainAverageOverallLatency,
			GlsApiPerformanceCounters.SaveDomainAverageOverallLatencyBase,
			GlsApiPerformanceCounters.SaveTenantAverageOverallLatency,
			GlsApiPerformanceCounters.SaveTenantAverageOverallLatencyBase,
			GlsApiPerformanceCounters.SaveUserAverageOverallLatency,
			GlsApiPerformanceCounters.SaveUserAverageOverallLatencyBase,
			GlsApiPerformanceCounters.DeleteDomainAverageOverallLatency,
			GlsApiPerformanceCounters.DeleteDomainAverageOverallLatencyBase,
			GlsApiPerformanceCounters.DeleteTenantAverageOverallLatency,
			GlsApiPerformanceCounters.DeleteTenantAverageOverallLatencyBase,
			GlsApiPerformanceCounters.DeleteUserAverageOverallLatency,
			GlsApiPerformanceCounters.DeleteUserAverageOverallLatencyBase
		};
	}
}
