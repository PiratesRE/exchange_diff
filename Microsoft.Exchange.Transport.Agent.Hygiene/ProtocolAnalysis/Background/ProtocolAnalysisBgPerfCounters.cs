using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal static class ProtocolAnalysisBgPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (ProtocolAnalysisBgPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in ProtocolAnalysisBgPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Protocol Analysis Background Agent";

		public static readonly ExPerformanceCounter Socks4Proxy = new ExPerformanceCounter("MSExchange Protocol Analysis Background Agent", "Socks4 Proxy Found", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter Socks5Proxy = new ExPerformanceCounter("MSExchange Protocol Analysis Background Agent", "Socks5 Proxy Found", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter HttpConnectProxy = new ExPerformanceCounter("MSExchange Protocol Analysis Background Agent", "HttpConnect Proxy Found", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter HttpPostProxy = new ExPerformanceCounter("MSExchange Protocol Analysis Background Agent", "HttpPost Proxy Found", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TelnetProxy = new ExPerformanceCounter("MSExchange Protocol Analysis Background Agent", "Telnet Proxy Found", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CiscoProxy = new ExPerformanceCounter("MSExchange Protocol Analysis Background Agent", "Cisco Proxy Found", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter WingateProxy = new ExPerformanceCounter("MSExchange Protocol Analysis Background Agent", "Wingate Proxy Found", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NegativeOpenProxy = new ExPerformanceCounter("MSExchange Protocol Analysis Background Agent", "Negative Open Proxy", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UnknownOpenProxy = new ExPerformanceCounter("MSExchange Protocol Analysis Background Agent", "Unknown Open Proxy", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalOpenProxy = new ExPerformanceCounter("MSExchange Protocol Analysis Background Agent", "Open Proxy Tests", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ReverseDnsSucc = new ExPerformanceCounter("MSExchange Protocol Analysis Background Agent", "Reverse DNS Sucess", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ReverseDnsFail = new ExPerformanceCounter("MSExchange Protocol Analysis Background Agent", "Reverse DNS Failure", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter BlockSender = new ExPerformanceCounter("MSExchange Protocol Analysis Background Agent", "Block Senders", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			ProtocolAnalysisBgPerfCounters.Socks4Proxy,
			ProtocolAnalysisBgPerfCounters.Socks5Proxy,
			ProtocolAnalysisBgPerfCounters.HttpConnectProxy,
			ProtocolAnalysisBgPerfCounters.HttpPostProxy,
			ProtocolAnalysisBgPerfCounters.TelnetProxy,
			ProtocolAnalysisBgPerfCounters.CiscoProxy,
			ProtocolAnalysisBgPerfCounters.WingateProxy,
			ProtocolAnalysisBgPerfCounters.NegativeOpenProxy,
			ProtocolAnalysisBgPerfCounters.UnknownOpenProxy,
			ProtocolAnalysisBgPerfCounters.TotalOpenProxy,
			ProtocolAnalysisBgPerfCounters.ReverseDnsSucc,
			ProtocolAnalysisBgPerfCounters.ReverseDnsFail,
			ProtocolAnalysisBgPerfCounters.BlockSender
		};
	}
}
