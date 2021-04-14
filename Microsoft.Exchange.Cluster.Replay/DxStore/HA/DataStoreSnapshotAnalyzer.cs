using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Exchange.DxStore.HA.Events;

namespace Microsoft.Exchange.DxStore.HA
{
	public class DataStoreSnapshotAnalyzer
	{
		public DataStoreSnapshotAnalyzer(DiffReportVerboseMode verboseMode)
		{
			this.Container = new DataStoreMergedContainer(verboseMode);
			this.AnalysisPhase = "None";
			this.TimingInfo = new Dictionary<string, int>();
		}

		public DataStoreMergedContainer Container { get; set; }

		public XElement ClusdbSnapshot { get; set; }

		public XElement DxStoreSnapshot { get; set; }

		public string AnalysisPhase { get; private set; }

		public Dictionary<string, int> TimingInfo { get; private set; }

		public InstanceClientFactory ClientFactory { get; set; }

		public void InitializeIfRequired()
		{
			if (this.ClientFactory == null)
			{
				this.ClientFactory = this.GetDefaultGroupInstanceClientFactory();
			}
		}

		public void AnalyzeDataStores()
		{
			this.InitializeIfRequired();
			this.AnalysisPhase = "Collecting clusdb snapshot";
			this.MeasureDuration(this.AnalysisPhase, delegate
			{
				this.ClusdbSnapshot = this.GetClusdbSnapshot();
			});
			this.AnalysisPhase = "Merging clusdb snapshot with container";
			this.MeasureDuration(this.AnalysisPhase, delegate
			{
				this.Apply(true, this.ClusdbSnapshot, null);
			});
			this.AnalysisPhase = "Collecting dxstore snapshot";
			this.MeasureDuration(this.AnalysisPhase, delegate
			{
				this.DxStoreSnapshot = this.GetDxStoreSnapshot();
			});
			this.AnalysisPhase = "Merging dxstore snapshot with container";
			this.MeasureDuration(this.AnalysisPhase, delegate
			{
				this.Apply(false, this.DxStoreSnapshot, null);
			});
			this.AnalysisPhase = "Creating diff report";
			this.MeasureDuration(this.AnalysisPhase, delegate
			{
				this.Container.Analyze();
			});
		}

		public string GetTimingInfoAsString()
		{
			return string.Join(";", from ti in this.TimingInfo
			select string.Format("{0} = {1}ms", ti.Key, ti.Value));
		}

		public XElement GetClusdbSnapshot()
		{
			string[] filterRootKeys = new string[]
			{
				"Exchange",
				"ExchangeActiveManager",
				"MsExchangeIs"
			};
			ClusdbSnapshotMaker clusdbSnapshotMaker = new ClusdbSnapshotMaker(filterRootKeys, null, false);
			return clusdbSnapshotMaker.GetXElementSnapshot(null);
		}

		public InstanceClientFactory GetDefaultGroupInstanceClientFactory()
		{
			DistributedStoreEventLogger eventLogger = new DistributedStoreEventLogger(false);
			DistributedStoreTopologyProvider distributedStoreTopologyProvider = new DistributedStoreTopologyProvider(eventLogger, null, false);
			InstanceGroupConfig groupConfig = distributedStoreTopologyProvider.GetGroupConfig(null, true);
			if (groupConfig != null)
			{
				return new InstanceClientFactory(groupConfig, null);
			}
			return null;
		}

		public XElement GetDxStoreSnapshot()
		{
			DxStoreInstanceClient localClient = this.ClientFactory.LocalClient;
			InstanceSnapshotInfo instanceSnapshotInfo = localClient.AcquireSnapshot("Public", true, null);
			instanceSnapshotInfo.Decompress();
			return XElement.Parse(instanceSnapshotInfo.Snapshot);
		}

		public bool IsPaxosConfiguredAndLeaderExist(out string statusInfoStr)
		{
			statusInfoStr = "<null>";
			this.InitializeIfRequired();
			InstanceStatusInfo si = null;
			Utils.RunBestEffort(delegate
			{
				si = this.ClientFactory.LocalClient.GetStatus(null);
			});
			if (si != null)
			{
				statusInfoStr = Utils.SerializeObjectToJsonString<InstanceStatusInfo>(si, false, true);
			}
			return si != null && si.IsValidLeaderExist();
		}

		public void Apply(bool isClusdb, XElement element, string fullKeyName = null)
		{
			if (string.IsNullOrEmpty(fullKeyName))
			{
				fullKeyName = element.Attribute("Name").Value;
			}
			DataStoreMergedContainer.KeyEntry keyEntry = this.Container.AddOrUpdateKey(fullKeyName, isClusdb);
			if (element.HasElements)
			{
				List<XElement> list = new List<XElement>();
				foreach (XElement xelement in element.Elements())
				{
					string localName = xelement.Name.LocalName;
					if (Utils.IsEqual(localName, "Key", StringComparison.OrdinalIgnoreCase))
					{
						list.Add(xelement);
					}
					else if (Utils.IsEqual(localName, "Value", StringComparison.OrdinalIgnoreCase))
					{
						string value = xelement.Attribute("Name").Value;
						string value2 = xelement.Attribute("Kind").Value;
						string value3 = xelement.Value;
						keyEntry.AddOrUpdateProperty(value, value3, value2, isClusdb);
					}
				}
				if (list.Count > 0)
				{
					foreach (XElement xelement2 in list)
					{
						string value4 = xelement2.Attribute("Name").Value;
						string fullKeyName2 = Path.Combine(fullKeyName, value4);
						this.Apply(isClusdb, xelement2, fullKeyName2);
					}
				}
			}
		}

		public void LogDiffDetailsToEventLog()
		{
			this.Container.Analyze();
			DataStoreDiffReport report = this.Container.Report;
			DxStoreHACrimsonEvents.DataStoreDiffStats.Log<bool, int, int, int, int, int, int, int, int, int, int, int, int, int, int>(report.IsEverythingMatches, report.TotalKeysCount, report.TotalPropertiesCount, report.TotalClusdbKeysCount, report.TotalClusdbPropertiesCount, report.TotalDxStoreKeysCount, report.TotalDxStorePropertiesCount, report.CountKeysOnlyInClusdb, report.CountKeysOnlyInDxStore, report.CountKeysInClusdbAndDxStoreAndPropertiesMatch, report.CountKeysInClusdbAndDxStoreButPropertiesDifferent, report.CountPropertiesOnlyInClusdb, report.CountPropertiesOnlyInDxStore, report.CountPropertiesSameInClusdbAndDxStore, report.CountPropertiesDifferentInClusdbAndDxStore);
			string verboseReport = report.VerboseReport;
			int length = verboseReport.Length;
			int num = RegistryParameters.DistributedStoreDiffVerboseReportMaxCharsPerLine;
			if (num == 0)
			{
				num = 500;
			}
			int num2 = (int)Math.Ceiling((double)length / (double)num);
			int num3 = 1;
			int i = 0;
			while (i < length)
			{
				string text = verboseReport.Substring(i, Math.Min(num, length - i));
				DxStoreHACrimsonEvents.DataStoreDiffVerboseReport.Log<int, int, string>(num3, num2, text);
				i += num;
				num3++;
			}
		}

		public void CopyClusdbSnapshotToDxStore()
		{
			InstanceSnapshotInfo instanceSnapshotInfo = new InstanceSnapshotInfo();
			instanceSnapshotInfo.FullKeyName = "Public";
			instanceSnapshotInfo.Snapshot = this.ClusdbSnapshot.ToString();
			instanceSnapshotInfo.Compress();
			using (InstanceClientFactory defaultGroupInstanceClientFactory = this.GetDefaultGroupInstanceClientFactory())
			{
				DxStoreInstanceClient localClient = defaultGroupInstanceClientFactory.LocalClient;
				localClient.ApplySnapshot(instanceSnapshotInfo, false, null);
			}
		}

		private void MeasureDuration(string tag, Action action)
		{
			Stopwatch stopwatch = new Stopwatch();
			try
			{
				stopwatch.Start();
				action();
			}
			finally
			{
				TimeSpan elapsed = stopwatch.Elapsed;
				this.TimingInfo[tag] = (int)elapsed.TotalMilliseconds;
			}
		}
	}
}
