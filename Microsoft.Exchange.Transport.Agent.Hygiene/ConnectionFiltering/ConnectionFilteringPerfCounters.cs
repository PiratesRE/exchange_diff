using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.ConnectionFiltering
{
	internal static class ConnectionFilteringPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (ConnectionFilteringPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in ConnectionFilteringPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Connection Filtering Agent";

		private static readonly ExPerformanceCounter ConnectionsOnIPAllowListPerSecond = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Connections on IP Allow List/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalConnectionsOnIPAllowList = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Connections on IP Allow List", string.Empty, null, new ExPerformanceCounter[]
		{
			ConnectionFilteringPerfCounters.ConnectionsOnIPAllowListPerSecond
		});

		private static readonly ExPerformanceCounter ConnectionsOnIPBlockListPerSecond = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Connections on IP Block List/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalConnectionsOnIPBlockList = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Connections on IP Block List", string.Empty, null, new ExPerformanceCounter[]
		{
			ConnectionFilteringPerfCounters.ConnectionsOnIPBlockListPerSecond
		});

		private static readonly ExPerformanceCounter ConnectionsOnIPAllowProvidersPerSecond = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Connections on IP Allow List Providers/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalConnectionsOnIPAllowProviders = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Connections on IP Allow List Providers", string.Empty, null, new ExPerformanceCounter[]
		{
			ConnectionFilteringPerfCounters.ConnectionsOnIPAllowProvidersPerSecond
		});

		private static readonly ExPerformanceCounter ConnectionsOnIPBlockProvidersPerSecond = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Connections on IP Block List Providers /sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalConnectionsOnIPBlockProviders = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Connections on IP Block List Providers", string.Empty, null, new ExPerformanceCounter[]
		{
			ConnectionFilteringPerfCounters.ConnectionsOnIPBlockProvidersPerSecond
		});

		private static readonly ExPerformanceCounter MessagesWithOriginatingIPOnIPAllowListPerSecond = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Messages with Originating IP on IP Allow List/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithOriginatingIPOnIPAllowList = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Messages with Originating IP on IP Allow List", string.Empty, null, new ExPerformanceCounter[]
		{
			ConnectionFilteringPerfCounters.MessagesWithOriginatingIPOnIPAllowListPerSecond
		});

		private static readonly ExPerformanceCounter MessagesWithOriginatingIPOnIPBlockListPerSecond = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Messages with Originating IP on IP Block List/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithOriginatingIPOnIPBlockList = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Messages with Originating IP on IP Block List", string.Empty, null, new ExPerformanceCounter[]
		{
			ConnectionFilteringPerfCounters.MessagesWithOriginatingIPOnIPBlockListPerSecond
		});

		private static readonly ExPerformanceCounter MessagesWithOriginatingIPOnIPAllowProvidersPerSecond = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Messages with Originating IP on IP Allow List Providers/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithOriginatingIPOnIPAllowProviders = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Messages with Originating IP on IP Allow List Providers", string.Empty, null, new ExPerformanceCounter[]
		{
			ConnectionFilteringPerfCounters.MessagesWithOriginatingIPOnIPAllowProvidersPerSecond
		});

		private static readonly ExPerformanceCounter MessagesWithOriginatingIPOnIPBlockProvidersPerSecond = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Messages with Originating IP on IP Block List Providers/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalMessagesWithOriginatingIPOnIPBlockProviders = new ExPerformanceCounter("MSExchange Connection Filtering Agent", "Messages with Originating IP on IP Block List Providers", string.Empty, null, new ExPerformanceCounter[]
		{
			ConnectionFilteringPerfCounters.MessagesWithOriginatingIPOnIPBlockProvidersPerSecond
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			ConnectionFilteringPerfCounters.TotalConnectionsOnIPAllowList,
			ConnectionFilteringPerfCounters.TotalConnectionsOnIPBlockList,
			ConnectionFilteringPerfCounters.TotalConnectionsOnIPAllowProviders,
			ConnectionFilteringPerfCounters.TotalConnectionsOnIPBlockProviders,
			ConnectionFilteringPerfCounters.TotalMessagesWithOriginatingIPOnIPAllowList,
			ConnectionFilteringPerfCounters.TotalMessagesWithOriginatingIPOnIPBlockList,
			ConnectionFilteringPerfCounters.TotalMessagesWithOriginatingIPOnIPAllowProviders,
			ConnectionFilteringPerfCounters.TotalMessagesWithOriginatingIPOnIPBlockProviders
		};
	}
}
