using System;
using System.Collections.Generic;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public interface IResponderWorkBroker : IWorkBrokerBase
	{
		IDataAccessQuery<MonitorResult> GetMonitorResults(string alertMask, DateTime startTime, DateTime endTime);

		IDataAccessQuery<ResponderResult> GetResponderResults(ResponderDefinition definition, DateTime startTime);

		IDataAccessQuery<ProbeResult> GetProbeResult(int probeId, int resultId);

		IDataAccessQuery<ProbeResult> GetProbeResults(string sampleMask, DateTime startTime, DateTime endTime);

		IDataAccessQuery<ProbeResult> GetProbeResults(string scopeName, DateTime startTime);

		IDataAccessQuery<MonitorResult> GetLastSuccessfulMonitorResult(string alertMask, DateTime startTime, DateTime endTime);

		IDataAccessQuery<MonitorResult> GetLastSuccessfulMonitorResult(int workItemId);

		IDataAccessQuery<ResponderResult> GetLastSuccessfulResponderResult(ResponderDefinition definition);

		IDataAccessQuery<ResponderResult> GetLastSuccessfulResponderResult(ResponderDefinition definition, TimeSpan searchWindow);

		IDataAccessQuery<ResponderResult> GetLastSuccessfulRecoveryAttemptedResponderResult(ResponderDefinition definition, TimeSpan searchWindow);

		IDataAccessQuery<ResponderResult> GetLastSuccessfulRecoveryAttemptedResponderResultByName(ResponderDefinition definition, TimeSpan searchWindow);

		IDataAccessQuery<MonitorResult> GetSuccessfulMonitorResults(DateTime startTime, DateTime endTime);

		IDataAccessQuery<MonitorDefinition> GetMonitorDefinitions(DateTime startTime);

		IDataAccessQuery<MonitorDefinition> GetMonitorDefinition(int workItemId);

		IDataAccessQuery<TEntity> AsDataAccessQuery<TEntity>(IEnumerable<TEntity> query);
	}
}
