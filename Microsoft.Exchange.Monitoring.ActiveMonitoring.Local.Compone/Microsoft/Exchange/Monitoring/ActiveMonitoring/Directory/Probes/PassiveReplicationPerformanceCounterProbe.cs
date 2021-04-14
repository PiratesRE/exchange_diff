using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class PassiveReplicationPerformanceCounterProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.PassiveReplicationPerfCounterProbe, delegate
			{
				string arg = new ServerIdParameter().ToString();
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting PassiveReplicationPerfCounterProbe on DC: {0}", arg, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\PassiveReplicationPerformanceCounterProbe.cs", 78);
				string empty = string.Empty;
				string defaultNC = DirectoryGeneralUtils.GetDefaultNC(Environment.MachineName);
				string partition = string.Format("CN=Configuration,{0}", defaultNC);
				if (!PerformanceCounterCategory.Exists(this.PerformanceCounterCategoryName))
				{
					CounterCreationData counterCreationData = new CounterCreationData();
					counterCreationData.CounterName = this.CounterLatencyInSecondsDomainNC;
					counterCreationData.CounterType = PerformanceCounterType.NumberOfItems32;
					CounterCreationData counterCreationData2 = new CounterCreationData();
					counterCreationData2.CounterName = this.CounterLatencyInSecondsConfigNC;
					counterCreationData2.CounterType = PerformanceCounterType.NumberOfItems32;
					CounterCreationDataCollection counterCreationDataCollection = new CounterCreationDataCollection();
					counterCreationDataCollection.Add(counterCreationData);
					counterCreationDataCollection.Add(counterCreationData2);
					PerformanceCounterCategory.Create(this.PerformanceCounterCategoryName, this.PerformanceCounterCategoryHelp, PerformanceCounterCategoryType.MultiInstance, counterCreationDataCollection);
				}
				this.PublishReplicationPerfCounter(defaultNC, this.CounterLatencyInSecondsDomainNC);
				this.PublishReplicationPerfCounter(partition, this.CounterLatencyInSecondsConfigNC);
				PerformanceCounter.CloseSharedResources();
			});
		}

		private void PublishReplicationPerfCounter(string partition, string counterName)
		{
			string replicationXml = DirectoryUtils.GetReplicationXml(Environment.MachineName, partition);
			if (string.IsNullOrEmpty(replicationXml))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Replication information for Partition {0}:\n\nSourceDCName, LastSuccessfulSyncTime\n", partition);
			using (XmlReader xmlReader = XmlReader.Create(new StringReader(replicationXml)))
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(xmlReader);
				XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("REPL");
				if (elementsByTagName.Item(0).ChildNodes.Count == 0)
				{
					return;
				}
				XmlNodeList childNodes = elementsByTagName.Item(0).ChildNodes;
				string text = string.Empty;
				DateTime utcNow = DateTime.UtcNow;
				foreach (object obj in childNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string text2 = xmlNode["pszSourceDsaDN"].OuterXml;
					text2 = text2.Split(new char[]
					{
						','
					})[1].Substring(3);
					text = xmlNode["ftimeLastSyncSuccess"].InnerText;
					DateTime value = Convert.ToDateTime(text);
					stringBuilder.AppendFormat("{0},{1}\n", text2, text);
					long rawValue = (long)utcNow.Subtract(value).TotalSeconds;
					if (value.Year > 2000)
					{
						using (PerformanceCounter performanceCounter = new PerformanceCounter(this.PerformanceCounterCategoryName, counterName, text2, false))
						{
							performanceCounter.RawValue = rawValue;
						}
					}
				}
			}
			if (string.IsNullOrEmpty(base.Result.StateAttribute1))
			{
				base.Result.StateAttribute1 = string.Format("{0}\n", stringBuilder.ToString());
				return;
			}
			ProbeResult result = base.Result;
			result.StateAttribute1 += stringBuilder.ToString();
		}

		public const int StaleDataLimitInDays = 32;

		public readonly string PerformanceCounterCategoryName = "MSExchange Domain Controller Inbound Replication Latency";

		public readonly string PerformanceCounterCategoryHelp = "Counter that determines the last successful sync time with the inbound neighbor Domain Controller.";

		public readonly string CounterLatencyInSecondsDomainNC = "Latency In Seconds Domain NC";

		public readonly string CounterLatencyInSecondsConfigNC = "Latency In Seconds Config NC";
	}
}
