using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class MonitorWorkBroker<TDataAccess> : TypedWorkBroker<MonitorDefinition, MonitorWorkItem, MonitorResult, TDataAccess>, IMonitorWorkBroker, IWorkBrokerBase where TDataAccess : DataAccess, new()
	{
		public MonitorWorkBroker(WorkItemFactory factory) : base(factory)
		{
		}

		public IDataAccessQuery<ProbeResult> GetProbeResults(string sampleMask, DateTime startTime, DateTime endTime)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			IEnumerable<ProbeResult> query = from r in base.GetResultsQuery<ProbeResult>(sampleMask, startTime)
			where r.ExecutionEndTime <= endTime
			select r;
			return tdataAccess.AsDataAccessQuery<ProbeResult>(query);
		}

		public IDataAccessQuery<ProbeResult> GetProbeResult(int probeId, int resultId)
		{
			Activator.CreateInstance<TDataAccess>();
			return base.GetSingleResultQuery<ProbeResult>(probeId, resultId);
		}

		public IDataAccessQuery<MonitorResult> GetSuccessfulMonitorResults(MonitorDefinition definition, DateTime startTime)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			IEnumerable<MonitorResult> query = from r in base.GetResultsQuery<MonitorResult>(definition.ConstructWorkItemResultName(), startTime)
			where r.ResultType == ResultType.Succeeded
			select r;
			return tdataAccess.AsDataAccessQuery<MonitorResult>(query);
		}

		public IDataAccessQuery<MonitorResult> GetSuccessfulMonitorResults(Component component, DateTime startTime)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			IOrderedEnumerable<MonitorResult> query = from r in tdataAccess.GetTable<MonitorResult, string>(MonitorResultIndex.ComponentNameAndExecutionEndTime(component.ToString(), startTime))
			where r.DeploymentId == Settings.DeploymentId && r.ResultType == ResultType.Succeeded
			orderby r.ExecutionStartTime descending
			select r;
			return tdataAccess.AsDataAccessQuery<MonitorResult>(query);
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

		public IDataAccessQuery<MonitorResult> GetLastSuccessfulMonitorResult(MonitorDefinition definition)
		{
			return base.GetLastSuccessfulResultQuery(definition, SqlDateTime.MinValue.Value);
		}

		public IDataAccessQuery<MonitorResult> GetLastMonitorResult(MonitorDefinition definition, TimeSpan searchWindow)
		{
			return base.GetLastResultQuery(definition, searchWindow);
		}

		public IDataAccessQuery<MonitorDefinition> GetMonitorDefinitions(DateTime startTime)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			IEnumerable<MonitorDefinition> query = from d in tdataAccess.GetTable<MonitorDefinition, DateTime>(WorkDefinitionIndex<MonitorDefinition>.StartTime(startTime))
			where d.DeploymentId == Settings.DeploymentId
			select d;
			return tdataAccess.AsDataAccessQuery<MonitorDefinition>(query);
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
