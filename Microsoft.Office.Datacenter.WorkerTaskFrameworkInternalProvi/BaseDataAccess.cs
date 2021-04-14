using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public abstract class BaseDataAccess
	{
		internal abstract IDataAccessQuery<TEntity> GetTable<TEntity, TKey>(IIndexDescriptor<TEntity, TKey> indexDescriptor) where TEntity : class;

		internal abstract Task<int> AsyncGetWorkItemPackages<TWorkItem>(int deploymentId, Action<string> processResult, CancellationToken cancellationToken, TracingContext traceContext);

		internal abstract Task AsyncInsert<TEntity>(TEntity entity, CancellationToken cancellationToken, TracingContext traceContext) where TEntity : class, IWorkData;

		internal abstract Task AsyncInsertMany<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken, TracingContext traceContext) where TEntity : class, IWorkData;

		internal abstract Task AsyncInsertManyDefinitionsAndCleanup<TEntity>(IEnumerable<TEntity> entities, int id, DateTime cleanBeforeTime, CancellationToken cancellationToken, TracingContext traceContext) where TEntity : WorkDefinition;

		internal abstract Task<int> AsyncExecuteReader<TEntity>(IDataAccessQuery<TEntity> query, Action<TEntity> processResult, CancellationToken cancellationToken, TracingContext traceContext);

		internal abstract Task<TEntity> AsyncExecuteScalar<TEntity>(IDataAccessQuery<TEntity> query, CancellationToken cancellationToken, TracingContext traceContext);

		internal abstract Task<IEnumerable<TopologySchema>> GetTableData<TopologySchema, TKey>(IIndexDescriptor<TopologySchema, TKey> indexDescriptor, CancellationToken cancellationToken, TracingContext traceContext) where TopologySchema : TableEntity, new();

		internal abstract WorkUnit RequestWorkUnit(CancellationToken cancellationToken, TracingContext traceContext);

		internal abstract int HeartbeatForWorkUnit(WorkUnit workUnit, int workUnitState, out List<WorkUnitEntry> entries, CancellationToken cancellationToken, TracingContext traceContext);

		internal abstract Task<int> AddAndRemoveWorkUnitEntries(List<WorkUnitEntry> workUnitEntriesToAdd, List<WorkUnitEntry> workUnitEntriesToRemove, CancellationToken cancellationToken, TracingContext traceContext);

		internal abstract Task<int> AddWorkUnit(CancellationToken cancellationToken, TracingContext traceContext);

		internal abstract int GetWorkState(CancellationToken cancellationToken, TracingContext traceContext);

		internal abstract Task<bool> RequestRecovery(string metricName, string recoveryType, CancellationToken cancellationToken, TracingContext traceContext);

		internal abstract Task<StatusEntryCollection> GetStatusEntries(string key, CancellationToken cancellationToken, TracingContext traceContext);

		internal abstract Task SaveStatusEntry(StatusEntry entry, CancellationToken cancellationToken, TracingContext traceContext);

		internal abstract IDataAccessQuery<TEntity> AsDataAccessQuery<TEntity>(IEnumerable<TEntity> query);
	}
}
