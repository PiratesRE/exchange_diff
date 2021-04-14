using System;
using System.Collections.Generic;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public interface IMonitorWorkBroker : IWorkBrokerBase
	{
		IDataAccessQuery<ProbeResult> GetProbeResults(string sampleMask, DateTime startTime, DateTime endTime);

		IDataAccessQuery<ProbeResult> GetProbeResult(int probeId, int resultId);

		IDataAccessQuery<MonitorResult> GetSuccessfulMonitorResults(MonitorDefinition definition, DateTime startTime);

		IDataAccessQuery<MonitorResult> GetSuccessfulMonitorResults(Component component, DateTime startTime);

		IDataAccessQuery<MonitorResult> GetSuccessfulMonitorResults(DateTime startTime, DateTime endTime);

		IDataAccessQuery<MonitorResult> GetLastSuccessfulMonitorResult(MonitorDefinition definition);

		IDataAccessQuery<MonitorResult> GetLastMonitorResult(MonitorDefinition definition, TimeSpan searchWindow);

		IDataAccessQuery<MonitorDefinition> GetMonitorDefinitions(DateTime startTime);

		IDataAccessQuery<TEntity> AsDataAccessQuery<TEntity>(IEnumerable<TEntity> query);
	}
}
