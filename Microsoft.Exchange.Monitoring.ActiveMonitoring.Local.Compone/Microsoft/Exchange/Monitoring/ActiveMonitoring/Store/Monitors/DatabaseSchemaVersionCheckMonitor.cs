using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.MailboxSpace.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Store.Monitors
{
	public class DatabaseSchemaVersionCheckMonitor : MonitorWorkItem
	{
		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			base.Result.IsAlert = false;
			string targetResource = base.Definition.TargetResource;
			base.Result.StateAttribute1 = targetResource;
			ProbeResult lastProbeResult = WorkItemResultHelper.GetLastProbeResult(base.Definition.SampleMask, base.Broker, base.MonitoringWindowStartTime, base.Result.ExecutionStartTime, cancellationToken);
			if (lastProbeResult != null && !string.IsNullOrEmpty(lastProbeResult.StateAttribute1))
			{
				bool isExcludedFromProvisioning;
				bool isExcludedFromProvisioningBySchemaMonitoring;
				if (!string.IsNullOrEmpty(lastProbeResult.Exception) || !bool.TryParse(lastProbeResult.StateAttribute2, out isExcludedFromProvisioning) || !bool.TryParse(lastProbeResult.StateAttribute3, out isExcludedFromProvisioningBySchemaMonitoring))
				{
					WTFDiagnostics.TraceError<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Probe for database {0} is in failed state or we are unable to get provisioning state from the probe so turning the monitor red", targetResource, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseSchemaVersionCheckMonitor.cs", 56);
					base.Result.Exception = lastProbeResult.Exception;
					base.Result.Error = lastProbeResult.Error;
					base.Result.StateAttribute2 = "FailingDueToProbeFailuresOrIncompleteData";
					base.Result.LastFailedProbeId = lastProbeResult.WorkItemId;
					base.Result.LastFailedProbeResultId = lastProbeResult.ResultId;
					base.Result.IsAlert = true;
					return;
				}
				int num = (int)lastProbeResult.StateAttribute6;
				int num2;
				if (!int.TryParse(base.Definition.Attributes["MinimumRequiredDatabaseSchemaVersion"], out num2))
				{
					num2 = 126;
				}
				DatabaseProvisioningResponder.ProvisioningState provisioningState;
				base.Result.IsAlert = this.PopulateProvisioningChangesNeeded(targetResource, num, num2, isExcludedFromProvisioning, isExcludedFromProvisioningBySchemaMonitoring, out provisioningState);
				if (base.Result.IsAlert)
				{
					base.Result.StateAttribute5 = provisioningState.ToString();
					base.Result.LastFailedProbeId = lastProbeResult.WorkItemId;
					base.Result.LastFailedProbeResultId = lastProbeResult.ResultId;
				}
				base.Result.StateAttribute3 = isExcludedFromProvisioning.ToString();
				base.Result.StateAttribute4 = isExcludedFromProvisioningBySchemaMonitoring.ToString();
				base.Result.StateAttribute6 = (double)num;
				base.Result.StateAttribute7 = (double)num2;
			}
		}

		internal bool PopulateProvisioningChangesNeeded(string databaseName, int currentSchemaVersion, int minimumRequiredSchemaVersion, bool isExcludedFromProvisioning, bool isExcludedFromProvisioningBySchemaMonitoring, out DatabaseProvisioningResponder.ProvisioningState provisioningState)
		{
			provisioningState = DatabaseProvisioningResponder.ProvisioningState.None;
			if (currentSchemaVersion == -1)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Database schema version for database {0} is not populated by probe, failing the monitor", databaseName, null, "PopulateProvisioningChangesNeeded", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseSchemaVersionCheckMonitor.cs", 132);
				return true;
			}
			if (currentSchemaVersion < minimumRequiredSchemaVersion)
			{
				if (!isExcludedFromProvisioning)
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Database schema version for database {0} is below threshold, setting monitor into alert state", databaseName, null, "PopulateProvisioningChangesNeeded", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseSchemaVersionCheckMonitor.cs", 149);
					provisioningState = DatabaseProvisioningResponder.ProvisioningState.DisableProvisioningBySchema;
					return true;
				}
			}
			else
			{
				if (isExcludedFromProvisioning && isExcludedFromProvisioningBySchemaMonitoring)
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Database schema version for database {0} is above threshold but provisioning is disabled, setting monitor into alert state", databaseName, null, "PopulateProvisioningChangesNeeded", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseSchemaVersionCheckMonitor.cs", 167);
					provisioningState = DatabaseProvisioningResponder.ProvisioningState.EnableProvisioningBySchema;
					return true;
				}
				if (!isExcludedFromProvisioning && isExcludedFromProvisioningBySchemaMonitoring)
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Database schema version for database {0} is above threshold and provisioning enabled but IsExcludedFromProvisioningBySchemaMonitoring is disabled, setting monitor into alert state", databaseName, null, "PopulateProvisioningChangesNeeded", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseSchemaVersionCheckMonitor.cs", 184);
					provisioningState = DatabaseProvisioningResponder.ProvisioningState.DisableIsExcludedFromProvisioningBySchemaMonitoring;
					return true;
				}
			}
			return false;
		}
	}
}
