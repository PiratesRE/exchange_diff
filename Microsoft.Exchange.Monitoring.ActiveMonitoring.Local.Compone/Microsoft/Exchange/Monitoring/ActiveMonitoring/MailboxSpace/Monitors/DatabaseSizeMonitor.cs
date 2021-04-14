using System;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.MailboxSpace.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.MailboxSpace.Monitors
{
	public class DatabaseSizeMonitor : MonitorWorkItem
	{
		private double MaximumDatabaseSize
		{
			get
			{
				return base.Result.StateAttribute8;
			}
			set
			{
				base.Result.StateAttribute8 = value;
			}
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			base.Result.IsAlert = false;
			ProbeResult lastProbeResult = WorkItemResultHelper.GetLastProbeResult(base.Definition.SampleMask, base.Broker, base.MonitoringWindowStartTime, base.Result.ExecutionStartTime, cancellationToken);
			if (lastProbeResult == null || string.IsNullOrEmpty(lastProbeResult.StateAttribute1))
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "Unable to find last database space probe result or it's a passive copy", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\DatabaseSizeMonitor.cs", 191);
				return;
			}
			string stateAttribute = lastProbeResult.StateAttribute1;
			double stateAttribute2 = lastProbeResult.StateAttribute6;
			double stateAttribute3 = lastProbeResult.StateAttribute8;
			bool flag = bool.Parse(lastProbeResult.StateAttribute2);
			bool flag2 = bool.Parse(lastProbeResult.StateAttribute4);
			base.Result.StateAttribute1 = stateAttribute;
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "Performing database size calculation for database {0}", stateAttribute, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\DatabaseSizeMonitor.cs", 65);
			int num;
			if (!int.TryParse(base.Definition.Attributes["NumberOfDatabasesPerDisk"], out num))
			{
				num = 4;
			}
			double searchSizeFactor;
			if (!double.TryParse(base.Definition.Attributes["SearchSizeFactorThreshold"], out searchSizeFactor))
			{
				searchSizeFactor = 0.2;
			}
			ByteQuantifiedSize databaseBufferThreshold;
			if (!ByteQuantifiedSize.TryParse(base.Definition.Attributes["DatabaseBufferThreshold"], out databaseBufferThreshold))
			{
				databaseBufferThreshold = MailboxSpaceDiscovery.DatabaseBufferThreshold;
			}
			ByteQuantifiedSize databaseLogsThreshold;
			if (!ByteQuantifiedSize.TryParse(base.Definition.Attributes["DatabaseLogsThreshold"], out databaseLogsThreshold))
			{
				databaseLogsThreshold = MailboxSpaceDiscovery.DatabaseLogsThreshold;
			}
			if (!string.IsNullOrEmpty(lastProbeResult.Exception))
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "Probe for database {0} is in failed state so turning the monitor red", stateAttribute, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\DatabaseSizeMonitor.cs", 98);
				base.Result.Exception = lastProbeResult.Exception;
				base.Result.Error = lastProbeResult.Error;
				base.Result.StateAttribute2 = Strings.UnableToGetDatabaseSize(stateAttribute);
				base.Result.LastFailedProbeId = lastProbeResult.WorkItemId;
				base.Result.LastFailedProbeResultId = lastProbeResult.ResultId;
				base.Result.IsAlert = true;
				return;
			}
			if (stateAttribute3 == 0.0)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "Unable to get disk capacity for database {0} so turning the monitor red", stateAttribute, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\DatabaseSizeMonitor.cs", 117);
				base.Result.StateAttribute2 = string.Format("DiskCapacityForDatabase{0}NotPopulatedByDatabaseSpaceProbe", stateAttribute);
				base.Result.LastFailedProbeId = lastProbeResult.WorkItemId;
				base.Result.LastFailedProbeResultId = lastProbeResult.ResultId;
				base.Result.IsAlert = true;
				return;
			}
			if (num < 1)
			{
				base.Result.IsAlert = true;
				throw new ArgumentOutOfRangeException("NumberOfDatabasesPerDisk", num, "Value cannot be less than 1");
			}
			this.MaximumDatabaseSize = DatabaseSizeMonitor.GetMaximumDatabaseSize(stateAttribute3, databaseBufferThreshold.ToBytes(), databaseLogsThreshold.ToBytes(), searchSizeFactor);
			if (stateAttribute2 > this.MaximumDatabaseSize / (double)num)
			{
				if (!flag)
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.MailboxSpaceTracer, base.TraceContext, "Database size for database {0} is above threshold, setting monitor into alert state", stateAttribute, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\MailboxSpace\\DatabaseSizeMonitor.cs", 154);
					base.Result.StateAttribute5 = DatabaseProvisioningResponder.ProvisioningState.DisableProvisioningBySpace.ToString();
					base.Result.LastFailedProbeId = lastProbeResult.WorkItemId;
					base.Result.LastFailedProbeResultId = lastProbeResult.ResultId;
					base.Result.IsAlert = true;
				}
			}
			else if (!flag && flag2)
			{
				base.Result.StateAttribute5 = DatabaseProvisioningResponder.ProvisioningState.DisableIsExcludedFromProvisioningBySpaceMonitoring.ToString();
				base.Result.LastFailedProbeId = lastProbeResult.WorkItemId;
				base.Result.LastFailedProbeResultId = lastProbeResult.ResultId;
				base.Result.IsAlert = true;
			}
			base.Result.StateAttribute2 = searchSizeFactor.ToString();
			base.Result.StateAttribute3 = flag.ToString();
			base.Result.StateAttribute4 = num.ToString();
			base.Result.StateAttribute6 = stateAttribute2;
			base.Result.StateAttribute7 = stateAttribute3;
			base.Result.StateAttribute9 = databaseBufferThreshold.ToBytes();
			base.Result.StateAttribute10 = databaseLogsThreshold.ToBytes();
		}

		internal static double GetMaximumDatabaseSize(double diskCapacity, ulong databaseBufferThresholdBytes, ulong databaseLogsThresholdBytes, double searchSizeFactor)
		{
			return (diskCapacity - (databaseBufferThresholdBytes + databaseLogsThresholdBytes)) / (1.0 + searchSizeFactor);
		}
	}
}
