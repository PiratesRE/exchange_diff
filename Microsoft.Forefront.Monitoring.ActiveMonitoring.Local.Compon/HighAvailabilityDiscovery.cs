using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics.Components.Entities;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class HighAvailabilityDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!DiscoveryUtils.IsForefrontForOfficeDatacenter())
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.CommonTracer, HighAvailabilityDiscovery.traceContext, "[HighAvailabilityDiscovery.DoWork]: This is not a FFO datacenter machine.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\HighAvailability\\HighAvailabilityDiscovery.cs", 44);
				base.Result.StateAttribute1 = "HighAvailabilityDiscovery: This is not a FFO datacenter machine.";
				return;
			}
			HighAvailabilityDiscovery.HighAvailabilityConfig highAvailabilityConfig = HighAvailabilityDiscovery.ReadConfiguration(base.Definition.ExtensionAttributes);
			this.SetupDiskSpaceMonitor(highAvailabilityConfig.DiskSpaceMonitorConfig);
		}

		private static string GetPercentageFreeSpaceCounterName(DriveInfo driveInfo)
		{
			return string.Format("LogicalDisk\\% Free Space\\{0}", driveInfo.Name.Substring(0, 2));
		}

		private static HighAvailabilityDiscovery.HighAvailabilityConfig ReadConfiguration(string extensionAttributes)
		{
			IEnumerable<HighAvailabilityDiscovery.DiskSpaceMonitorConfig> diskSpaceMonitorConfig = XDocument.Parse(extensionAttributes).Descendants("DiskSpaceMonitor").Select(delegate(XElement dsp)
			{
				HighAvailabilityDiscovery.DiskSpaceMonitorConfig diskSpaceMonitorConfig2 = new HighAvailabilityDiscovery.DiskSpaceMonitorConfig();
				diskSpaceMonitorConfig2.Name = (string)dsp.Attribute("Name");
				diskSpaceMonitorConfig2.Enabled = (((bool?)dsp.Attribute("Enabled")) ?? true);
				HighAvailabilityDiscovery.DiskSpaceMonitorConfig diskSpaceMonitorConfig3 = diskSpaceMonitorConfig2;
				double? num = (double?)dsp.Attribute("PercentFreeSpaceThreshold");
				diskSpaceMonitorConfig3.PercentFreeSpaceThreshold = ((num != null) ? num.GetValueOrDefault() : 10.0);
				return diskSpaceMonitorConfig2;
			});
			return new HighAvailabilityDiscovery.HighAvailabilityConfig
			{
				DiskSpaceMonitorConfig = diskSpaceMonitorConfig
			};
		}

		private static HighAvailabilityDiscovery.DiskSpaceMonitorConfig GetDiskSpaceConfig(IEnumerable<HighAvailabilityDiscovery.DiskSpaceMonitorConfig> diskSpaceMonitorConfig, DriveInfo drive)
		{
			HighAvailabilityDiscovery.DiskSpaceMonitorConfig diskSpaceMonitorConfig2 = diskSpaceMonitorConfig.FirstOrDefault((HighAvailabilityDiscovery.DiskSpaceMonitorConfig dsc) => string.Equals(dsc.Name, drive.Name, StringComparison.OrdinalIgnoreCase));
			if (diskSpaceMonitorConfig2 == null)
			{
				diskSpaceMonitorConfig2 = new HighAvailabilityDiscovery.DiskSpaceMonitorConfig
				{
					Name = drive.Name,
					Enabled = true,
					PercentFreeSpaceThreshold = 10.0
				};
			}
			return diskSpaceMonitorConfig2;
		}

		private void SetupDiskSpaceMonitor(IEnumerable<HighAvailabilityDiscovery.DiskSpaceMonitorConfig> diskSpaceMonitorConfig)
		{
			IEnumerable<DriveInfo> enumerable = from dtm in DriveInfo.GetDrives()
			where dtm.DriveType != DriveType.NoRootDirectory && dtm.DriveType != DriveType.Removable
			select dtm;
			foreach (DriveInfo driveInfo in enumerable)
			{
				HighAvailabilityDiscovery.DiskSpaceMonitorConfig diskSpaceConfig = HighAvailabilityDiscovery.GetDiskSpaceConfig(diskSpaceMonitorConfig, driveInfo);
				if (diskSpaceConfig.Enabled)
				{
					string name = string.Format("DiskSpaceMonitor{0}", driveInfo.Name.Substring(0, 1));
					string sampleMask = PerformanceCounterNotificationItem.GenerateResultName(HighAvailabilityDiscovery.GetPercentageFreeSpaceCounterName(driveInfo));
					string name2 = ExchangeComponent.FfoDeployment.Name;
					Component ffoDeployment = ExchangeComponent.FfoDeployment;
					double percentFreeSpaceThreshold = diskSpaceConfig.PercentFreeSpaceThreshold;
					bool enabled = true;
					MonitorDefinition monitorDefinition = OverallConsecutiveSampleValueBelowThresholdMonitor.CreateDefinition(name, sampleMask, name2, ffoDeployment, percentFreeSpaceThreshold, 3, enabled);
					monitorDefinition.TargetResource = driveInfo.ToString();
					base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, HighAvailabilityDiscovery.traceContext);
					double num = diskSpaceConfig.PercentFreeSpaceThreshold / 100.0;
					string text = string.Format("Drive {0} has less than {1:P2} free disk space (~ < {2:F2}GB).", driveInfo.Name, num, (double)driveInfo.TotalSize * num / Math.Pow(1024.0, 3.0));
					ResponderDefinition definition = EscalateResponder.CreateDefinition(string.Format("DiskSpaceResponder{0}", driveInfo.Name.Substring(0, 1)), ExchangeComponent.FfoDeployment.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.None, "OPs SE", text, text, true, NotificationServiceClass.Urgent, (int)TimeSpan.FromHours(24.0).TotalSeconds, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
					base.Broker.AddWorkDefinition<ResponderDefinition>(definition, HighAvailabilityDiscovery.traceContext);
				}
			}
		}

		private const double DefaultPercentFreeSpaceThreshold = 10.0;

		private static TracingContext traceContext = new TracingContext();

		private class HighAvailabilityConfig
		{
			public HighAvailabilityConfig()
			{
				this.DiskSpaceMonitorConfig = new List<HighAvailabilityDiscovery.DiskSpaceMonitorConfig>();
			}

			public IEnumerable<HighAvailabilityDiscovery.DiskSpaceMonitorConfig> DiskSpaceMonitorConfig { get; set; }
		}

		private class DiskSpaceMonitorConfig
		{
			public string Name { get; set; }

			public bool Enabled { get; set; }

			public double PercentFreeSpaceThreshold { get; set; }

			public override string ToString()
			{
				return string.Format("Name = {0}; Enabled = {1}; PercentFreeSpaceThreshold = {2:P2};", this.Name, this.Enabled, this.PercentFreeSpaceThreshold / 100.0);
			}
		}
	}
}
