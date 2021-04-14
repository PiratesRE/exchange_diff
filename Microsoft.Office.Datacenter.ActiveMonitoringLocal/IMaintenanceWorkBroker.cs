using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public interface IMaintenanceWorkBroker : IWorkBrokerBase
	{
		void RequestRestart(string resultName, int resultId);

		IDataAccessQuery<MaintenanceResult> GetMaintenanceResults(MaintenanceDefinition definition, DateTime startTime);

		Task AddWorkDefinition<TDefinition>(TDefinition definition, TracingContext traceContext) where TDefinition : WorkDefinition;

		Task AddWorkDefinitions<TWorkDefinition>(IEnumerable<TWorkDefinition> workDefinitions, int id, DateTime cleanBeforeTime, CancellationToken cancellationToken, TracingContext traceContext) where TWorkDefinition : WorkDefinition;

		Task<int> DeleteWorkItemResult<TResult>(DateTime startTime, DateTime endTime, int timeOutInSeconds, CancellationToken cancellationToken, TracingContext traceContext) where TResult : WorkItemResult;

		IDataAccessQuery<MaintenanceResult> GetLastSuccessfulMaintenanceResult(MaintenanceDefinition definition, TimeSpan searchWindow);

		IDataAccessQuery<MaintenanceResult> GetLastMaintenanceResult(MaintenanceDefinition definition, TimeSpan searchWindow);

		IDataAccessQuery<TopologyScope> GetTopologyScopes(CancellationToken cancellationToken, TracingContext traceContext);

		Task AsyncInsertTopologyScope(int aggregationLevel, string name, CancellationToken cancellationToken, TracingContext traceContext);

		Task<int> AsyncDisableWorkDefinitions(int createdById, DateTime createdBeforeTimestamp, CancellationToken cancellationToken, TracingContext traceContext);

		IDataAccessQuery<TopologySchema> GetTopologyObjects<TopologySchema, TKey>(IIndexDescriptor<TopologySchema, TKey> indexDescriptor) where TopologySchema : class;

		Task<IEnumerable<TableSchema>> GetTableData<TableSchema, TKey>(IIndexDescriptor<TableSchema, TKey> indexDescriptor) where TableSchema : TableEntity, new();

		IDataAccessQuery<WorkDefinitionOverride> GetWorkDefinitionOverrides(CancellationToken cancellationToken, TracingContext traceContext);

		IDataAccessQuery<ProbeDefinition> GetProbeDefinition(string typeName);

		WorkUnit RequestWorkUnit(CancellationToken cancellationToken, TracingContext traceContext);

		int HeartbeatForWorkUnit(WorkUnit workUnit, int workUnitState, out List<WorkUnitEntry> entries, CancellationToken cancellationToken, TracingContext traceContext);

		Task<int> AddAndRemoveWorkUnitEntries(List<WorkUnitEntry> workUnitEntriesToAdd, List<WorkUnitEntry> workUnitEntriesToRemove, CancellationToken cancellationToken, TracingContext traceContext);

		Task<int> AddWorkUnit(CancellationToken cancellationToken, TracingContext traceContext);

		int GetWorkState(CancellationToken cancellationToken, TracingContext traceContext);

		Task SaveAllStatusEntries(CancellationToken cancellationToken, TracingContext traceContext);
	}
}
