using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.FrontendProxyRoutingAgent
{
	internal static class FrontendProxyAgentPerformanceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (FrontendProxyAgentPerformanceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in FrontendProxyAgentPerformanceCounters.AllCounters)
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

		public const string CategoryName = "MSExchangeFrontendTransport Proxy Routing Agent";

		public static readonly ExPerformanceCounter MessagesSuccessfullyRouted = new ExPerformanceCounter("MSExchangeFrontendTransport Proxy Routing Agent", "MessagesSuccessfullyRouted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MessagesFailedToRoute = new ExPerformanceCounter("MSExchangeFrontendTransport Proxy Routing Agent", "MessagesFailedToRoute", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			FrontendProxyAgentPerformanceCounters.MessagesSuccessfullyRouted,
			FrontendProxyAgentPerformanceCounters.MessagesFailedToRoute
		};
	}
}
