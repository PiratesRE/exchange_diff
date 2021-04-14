using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class ResponderWorkBroker<TDataAccess> : TypedWorkBroker<ResponderDefinition, ResponderWorkItem, ResponderResult, TDataAccess>, IResponderWorkBroker, IWorkBrokerBase where TDataAccess : DataAccess, new()
	{
		public ResponderWorkBroker(WorkItemFactory factory) : base(factory)
		{
		}

		public IDataAccessQuery<ResponderResult> GetResponderResults(ResponderDefinition definition, DateTime startTime)
		{
			return base.GetResultsQuery(definition, startTime);
		}

		public IDataAccessQuery<MonitorResult> GetMonitorResults(string alertMask, DateTime startTime, DateTime endTime)
		{
			IEnumerable<MonitorResult> query = from r in base.GetResultsQuery<MonitorResult>(alertMask, startTime)
			where r.ExecutionEndTime <= endTime
			select r;
			return this.AsDataAccessQuery<MonitorResult>(query);
		}

		public IDataAccessQuery<ProbeResult> GetProbeResult(int probeId, int resultId)
		{
			Activator.CreateInstance<TDataAccess>();
			return base.GetSingleResultQuery<ProbeResult>(probeId, resultId);
		}

		public IDataAccessQuery<ProbeResult> GetProbeResults(string sampleMask, DateTime startTime, DateTime endTime)
		{
			IEnumerable<ProbeResult> query = from r in base.GetResultsQuery<ProbeResult>(sampleMask, startTime)
			where r.ExecutionEndTime <= endTime
			select r;
			return this.AsDataAccessQuery<ProbeResult>(query);
		}

		public IDataAccessQuery<ProbeResult> GetProbeResults(string scopeName, DateTime startTime)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			IOrderedEnumerable<ProbeResult> query = from r in tdataAccess.GetTable<ProbeResult, string>(ProbeResultIndex.ScopeNameAndExecutionEndTime(scopeName, startTime))
			where r.DeploymentId == Settings.DeploymentId
			orderby r.ExecutionStartTime descending
			select r;
			return tdataAccess.AsDataAccessQuery<ProbeResult>(query);
		}

		public IDataAccessQuery<MonitorResult> GetLastSuccessfulMonitorResult(string alertMask, DateTime startTime, DateTime endTime)
		{
			IEnumerable<MonitorResult> query = from r in base.GetLastSuccessfulResultQuery<MonitorResult>(alertMask, startTime)
			where r.ExecutionEndTime <= endTime
			select r;
			return this.AsDataAccessQuery<MonitorResult>(query);
		}

		public IDataAccessQuery<MonitorResult> GetLastSuccessfulMonitorResult(int workItemId)
		{
			return base.GetLastSuccessfulResultQuery<MonitorResult>(workItemId, SqlDateTime.MinValue.Value);
		}

		public IDataAccessQuery<ResponderResult> GetLastSuccessfulResponderResult(ResponderDefinition definition)
		{
			return base.GetLastSuccessfulResultQuery(definition, SqlDateTime.MinValue.Value);
		}

		public IDataAccessQuery<ResponderResult> GetLastSuccessfulResponderResult(ResponderDefinition definition, TimeSpan searchWindow)
		{
			return base.GetLastSuccessfulResultQuery(definition, searchWindow);
		}

		public IDataAccessQuery<ResponderResult> GetLastSuccessfulRecoveryAttemptedResponderResult(ResponderDefinition definition, TimeSpan searchWindow)
		{
			IEnumerable<ResponderResult> source = from r in base.GetResultsQuery(definition, DateTime.UtcNow - searchWindow)
			where r.ResultType == ResultType.Succeeded && r.IsRecoveryAttempted
			select r;
			return this.AsDataAccessQuery<ResponderResult>(source.Take(1));
		}

		public IDataAccessQuery<ResponderResult> GetLastSuccessfulRecoveryAttemptedResponderResultByName(ResponderDefinition definition, TimeSpan searchWindow)
		{
			IEnumerable<ResponderResult> source = from r in base.GetResultsQuery<ResponderResult>(definition.ConstructWorkItemResultName(), DateTime.UtcNow - searchWindow)
			where r.ResultType == ResultType.Succeeded && r.IsRecoveryAttempted
			select r;
			return this.AsDataAccessQuery<ResponderResult>(source.Take(1));
		}

		public IDataAccessQuery<MonitorResult> GetSuccessfulMonitorResults(DateTime startTime, DateTime endTime)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			IOrderedEnumerable<MonitorResult> query = from r in tdataAccess.GetTable<MonitorResult, DateTime>(MonitorResultIndex.ExecutionEndTime(startTime))
			where r.ResultType == ResultType.Succeeded && r.DeploymentId == Settings.DeploymentId && r.ExecutionEndTime <= endTime
			orderby r.ExecutionStartTime descending
			select r;
			return this.AsDataAccessQuery<MonitorResult>(query);
		}

		public IDataAccessQuery<MonitorDefinition> GetMonitorDefinitions(DateTime startTime)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			IEnumerable<MonitorDefinition> query = from d in tdataAccess.GetTable<MonitorDefinition, DateTime>(WorkDefinitionIndex<MonitorDefinition>.StartTime(startTime))
			where d.DeploymentId == Settings.DeploymentId
			select d;
			return this.AsDataAccessQuery<MonitorDefinition>(query);
		}

		public IDataAccessQuery<MonitorDefinition> GetMonitorDefinition(int workItemId)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			IEnumerable<MonitorDefinition> source = from d in tdataAccess.GetTable<MonitorDefinition, int>(WorkDefinitionIndex<MonitorDefinition>.Id(workItemId))
			where d.DeploymentId == Settings.DeploymentId
			select d;
			return this.AsDataAccessQuery<MonitorDefinition>(source.Take(1));
		}

		bool IWorkBrokerBase.IsLocal()
		{
			return base.IsLocal();
		}

		TimeSpan IWorkBrokerBase.get_DefaultResultWindow()
		{
			return base.DefaultResultWindow;
		}
	}
}
