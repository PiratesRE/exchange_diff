using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Script.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.SharePointSignalStore
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SharePointSignalRestDataProvider
	{
		public SharePointSignalRestDataProvider()
		{
			this.analyticsSignalSources = new List<IAnalyticsSignalSource>();
			this.anyDataProvided = false;
			this.report = new List<SharePointSignalRestDataProvider.AnalyticsSignalProviderReportItem>();
		}

		public void ProvideDataFor(ISharePointSender<string> sender)
		{
			IEnumerable<AnalyticsSignal> enumerable = new List<AnalyticsSignal>();
			foreach (IAnalyticsSignalSource analyticsSignalSource in this.analyticsSignalSources)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				IEnumerable<AnalyticsSignal> signals = analyticsSignalSource.GetSignals();
				stopwatch.Stop();
				if (signals != null)
				{
					enumerable = enumerable.Concat(signals);
					this.report.Add(new SharePointSignalRestDataProvider.AnalyticsSignalProviderReportItem(analyticsSignalSource.GetSourceName(), signals.Count<AnalyticsSignal>(), stopwatch.Elapsed));
					if (signals.Any<AnalyticsSignal>())
					{
						this.anyDataProvided = true;
					}
				}
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["signals"] = enumerable;
			string data = new JavaScriptSerializer().Serialize(dictionary);
			sender.SetData(data);
		}

		public bool AnyDataProvided()
		{
			return this.anyDataProvided;
		}

		public void AddAnalyticsSignalSource(IAnalyticsSignalSource analyticsSignalSource)
		{
			this.analyticsSignalSources.Add(analyticsSignalSource);
		}

		public void PrintProviderReport(ILogger logger)
		{
			foreach (SharePointSignalRestDataProvider.AnalyticsSignalProviderReportItem analyticsSignalProviderReportItem in this.report)
			{
				logger.LogInfo(analyticsSignalProviderReportItem.ToString(), new object[0]);
			}
		}

		internal static Dictionary<string, object> CreateSignalProperties(Dictionary<string, string> properties)
		{
			List<object> list = new List<object>();
			if (properties != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in properties)
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
					dictionary2["type"] = "SP.KeyValue";
					dictionary["__metadata"] = dictionary2;
					dictionary["ValueType"] = "Edm.String";
					dictionary["Key"] = keyValuePair.Key;
					dictionary["Value"] = keyValuePair.Value;
					list.Add(dictionary);
				}
			}
			Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
			dictionary3["results"] = list;
			return dictionary3;
		}

		private List<IAnalyticsSignalSource> analyticsSignalSources;

		private bool anyDataProvided;

		private List<SharePointSignalRestDataProvider.AnalyticsSignalProviderReportItem> report;

		private class AnalyticsSignalProviderReportItem
		{
			public AnalyticsSignalProviderReportItem(string analyticsSignalSourceName, int numberOfsignals, TimeSpan timeUsed)
			{
				this.AnalyticsSignalSourceName = analyticsSignalSourceName;
				this.NumberOfSignals = numberOfsignals;
				this.TimeUsed = timeUsed;
			}

			public string AnalyticsSignalSourceName { get; private set; }

			public int NumberOfSignals { get; private set; }

			public TimeSpan TimeUsed { get; set; }

			public override string ToString()
			{
				return string.Format("Providing {0} {1} signals (used {2} seconds).", this.NumberOfSignals, this.AnalyticsSignalSourceName, this.TimeUsed.TotalSeconds);
			}
		}
	}
}
