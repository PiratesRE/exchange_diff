using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class PerfCounterHelper
	{
		public static string GetInterestingCountersHtml(string interestedProtocol)
		{
			string[] headers = new string[]
			{
				"Counter",
				"Protocol",
				"Value"
			};
			List<string[]> list = new List<string[]>(4);
			foreach (string text in PerfCounterHelper.HttpProxyInterestingCounters.Keys)
			{
				InstanceDataCollectionCollection counterCollection = PerfCounterHelper.GetCounterCollection(text);
				PerfCounterHelper.PerfCounterMetadata[] array = PerfCounterHelper.HttpProxyInterestingCounters[text];
				InstanceDataCollection instanceCollection;
				for (int i = 0; i < array.Length; i++)
				{
					PerfCounterHelper.PerfCounterMetadata counterMetadata = array[i];
					if (counterCollection.Contains(counterMetadata.CounterName))
					{
						instanceCollection = counterCollection[counterMetadata.CounterName];
						list.AddRange(from string key in instanceCollection.Keys
						where (key.StartsWith(interestedProtocol, StringComparison.CurrentCultureIgnoreCase) || instanceCollection[key].RawValue >= (long)counterMetadata.InterestThreshold) && PerfCounterHelper.PZeroProtocols.Contains(key, StringComparer.CurrentCultureIgnoreCase)
						select new string[]
						{
							counterMetadata.CounterName,
							key,
							instanceCollection[key].RawValue.ToString() + counterMetadata.Units
						});
					}
				}
			}
			return HtmlHelper.CreateTable(headers, list);
		}

		public static void GetPerSiteCountersHtml(string counterName, string protocol, string destinationSite, string units, out string sameDestSiteHtml, out string sameLocalSiteHtml)
		{
			IEnumerable<InstanceData> rowData;
			IEnumerable<InstanceData> rowData2;
			PerfCounterHelper.GetPerSiteCounters(counterName, protocol, destinationSite, out rowData, out rowData2);
			string[] headers = new string[]
			{
				"Counter",
				"Protocol",
				"Site",
				"Value"
			};
			Func<InstanceData, string[]> getRowCellsDelegate = delegate(InstanceData counter)
			{
				string[] array = counter.InstanceName.Split(new char[]
				{
					';'
				});
				string text = (array.Length == 2) ? array[0] : string.Empty;
				string text2 = (array.Length == 2) ? array[1].ToUpper() : string.Empty;
				return new string[]
				{
					counterName,
					text,
					text2,
					counter.RawValue.ToString() + units
				};
			};
			sameDestSiteHtml = HtmlHelper.CreateTable<InstanceData>(headers, rowData, getRowCellsDelegate);
			sameLocalSiteHtml = HtmlHelper.CreateTable<InstanceData>(headers, rowData2, getRowCellsDelegate);
		}

		public static void GetPerSiteCounters(string counterName, string protocol, string destinationSite, out IEnumerable<InstanceData> sameDestSite, out IEnumerable<InstanceData> sameLocalSite)
		{
			PerfCounterHelper.ThrowIfNullOrEmpty(counterName, "counterName");
			PerfCounterHelper.ThrowIfNullOrEmpty(protocol, "protocol");
			PerfCounterHelper.ThrowIfNullOrEmpty(destinationSite, "destinationSite");
			InstanceDataCollectionCollection counterCollection = PerfCounterHelper.GetCounterCollection(PerfCounterHelper.HttpProxyPerSiteCategoryName);
			if (!counterCollection.Contains(counterName))
			{
				throw new ArgumentException(string.Format("Counter {0} not found in category {1}", counterName, PerfCounterHelper.HttpProxyPerSiteCategoryName));
			}
			InstanceDataCollection instanceCollection = counterCollection[counterName];
			sameDestSite = from string key in instanceCollection.Keys
			where key.EndsWith(';' + destinationSite.ToLower())
			orderby key
			select instanceCollection[key];
			sameLocalSite = (from string key in instanceCollection.Keys
			where key.StartsWith(protocol.ToLower() + ';')
			orderby instanceCollection[key].RawValue descending
			select instanceCollection[key]).Take(20);
		}

		private static InstanceDataCollectionCollection GetCounterCollection(string categoryName)
		{
			PerformanceCounterCategory performanceCounterCategory = new PerformanceCounterCategory(categoryName);
			return performanceCounterCategory.ReadCategory();
		}

		private static void ThrowIfNullOrEmpty(string obj, string argumentName)
		{
			if (string.IsNullOrEmpty(obj))
			{
				throw new ArgumentNullException(argumentName);
			}
		}

		public static readonly string UnitPercent = "%";

		public static readonly string UnitMs = "ms";

		public static readonly string HttpProxyCategoryName = "MSExchange HttpProxy";

		public static readonly string HttpProxyPerSiteCategoryName = "MSExchange HttpProxy Per Site";

		private static readonly string[] PZeroProtocols = new string[]
		{
			"AutoDiscover",
			"EAS",
			"EWS",
			"OWA",
			"RPCHTTP"
		};

		private static readonly Dictionary<string, PerfCounterHelper.PerfCounterMetadata[]> HttpProxyInterestingCounters = new Dictionary<string, PerfCounterHelper.PerfCounterMetadata[]>
		{
			{
				PerfCounterHelper.HttpProxyCategoryName,
				new PerfCounterHelper.PerfCounterMetadata[]
				{
					new PerfCounterHelper.PerfCounterMetadata
					{
						CounterName = "Average Authentication Latency",
						Units = PerfCounterHelper.UnitMs,
						InterestThreshold = 150
					},
					new PerfCounterHelper.PerfCounterMetadata
					{
						CounterName = "Average Tenant Lookup Latency",
						Units = PerfCounterHelper.UnitMs,
						InterestThreshold = 80
					},
					new PerfCounterHelper.PerfCounterMetadata
					{
						CounterName = "MailboxServerLocator Average Latency (Moving Average)",
						Units = PerfCounterHelper.UnitMs,
						InterestThreshold = 200
					},
					new PerfCounterHelper.PerfCounterMetadata
					{
						CounterName = "MailboxServerLocator Failure Rate",
						Units = PerfCounterHelper.UnitPercent,
						InterestThreshold = 1
					}
				}
			}
		};

		private class PerfCounterMetadata
		{
			public string CounterName;

			public string Units;

			public int InterestThreshold;
		}
	}
}
