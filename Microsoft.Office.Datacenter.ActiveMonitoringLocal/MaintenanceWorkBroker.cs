using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class MaintenanceWorkBroker<TDataAccess> : TypedWorkBroker<MaintenanceDefinition, MaintenanceWorkItem, MaintenanceResult, TDataAccess>, IMaintenanceWorkBroker, IWorkBrokerBase where TDataAccess : DataAccess, new()
	{
		public MaintenanceWorkBroker(WorkItemFactory factory) : base(factory)
		{
		}

		public void RequestRestart(string resultName, int resultId)
		{
			base.RequestRestart(RestartRequest.CreateMaintenanceRestartRequest(resultName, resultId));
		}

		public IDataAccessQuery<MaintenanceResult> GetMaintenanceResults(MaintenanceDefinition definition, DateTime startTime)
		{
			return base.GetResultsQuery(definition, startTime);
		}

		public Task AddWorkDefinition<TDefinition>(TDefinition definition, TracingContext traceContext) where TDefinition : WorkDefinition
		{
			this.InitializeCreatedByWorkItem<TDefinition>(definition, traceContext);
			this.ValidateAndSyncAttributes<TDefinition>(definition);
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			return tdataAccess.AsyncInsert<TDefinition>(definition, default(CancellationToken), traceContext);
		}

		public Task AddWorkDefinitions<TWorkDefinition>(IEnumerable<TWorkDefinition> workDefinitions, int id, DateTime cleanBeforeTime, CancellationToken cancellationToken, TracingContext traceContext) where TWorkDefinition : WorkDefinition
		{
			foreach (TWorkDefinition tworkDefinition in workDefinitions)
			{
				WorkDefinition definition = tworkDefinition;
				this.InitializeCreatedByWorkItem<WorkDefinition>(definition, traceContext);
				this.ValidateAndSyncAttributes<WorkDefinition>(definition);
			}
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			return tdataAccess.AsyncInsertManyDefinitionsAndCleanup<TWorkDefinition>(workDefinitions, id, cleanBeforeTime, cancellationToken, traceContext);
		}

		public Task<int> DeleteWorkItemResult<TResult>(DateTime startTime, DateTime endTime, int timeOutInSeconds, CancellationToken cancellationToken, TracingContext traceContext) where TResult : WorkItemResult
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			return tdataAccess.AsyncDeleteWorkItemResult<TResult>(startTime, endTime, timeOutInSeconds, cancellationToken, traceContext);
		}

		public IDataAccessQuery<MaintenanceResult> GetLastSuccessfulMaintenanceResult(MaintenanceDefinition definition, TimeSpan searchWindow)
		{
			return base.GetLastSuccessfulResultQuery(definition, searchWindow);
		}

		public IDataAccessQuery<MaintenanceResult> GetLastMaintenanceResult(MaintenanceDefinition definition, TimeSpan searchWindow)
		{
			IEnumerable<MaintenanceResult> source = from r in base.GetResultsQuery(definition, DateTime.UtcNow - searchWindow)
			select r;
			return this.AsDataAccessQuery<MaintenanceResult>(source.Take(1));
		}

		public IDataAccessQuery<TopologyScope> GetTopologyScopes(CancellationToken cancellationToken, TracingContext traceContext)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			IEnumerable<TopologyScope> query = from s in tdataAccess.GetTable<TopologyScope, TopologyScope>(new TopologyScopeIndex())
			select s;
			return this.AsDataAccessQuery<TopologyScope>(query);
		}

		public IDataAccessQuery<ProbeDefinition> GetProbeDefinition(string typeName)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			IEnumerable<ProbeDefinition> query = from s in tdataAccess.GetTable<ProbeDefinition, string>(ProbeDefinitionIndex.TypeName(typeName))
			select s;
			return this.AsDataAccessQuery<ProbeDefinition>(query);
		}

		public Task AsyncInsertTopologyScope(int aggregationLevel, string name, CancellationToken cancellationToken, TracingContext traceContext)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			return tdataAccess.AsyncInsert<TopologyScope>(new TopologyScope
			{
				AggregationLevel = aggregationLevel,
				Name = name
			}, cancellationToken, traceContext);
		}

		public Task<int> AsyncDisableWorkDefinitions(int createdById, DateTime createdBeforeTimestamp, CancellationToken cancellationToken, TracingContext traceContext)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			return tdataAccess.AsyncDisableWorkDefinitions(createdById, createdBeforeTimestamp, cancellationToken, traceContext);
		}

		public IDataAccessQuery<TopologySchema> GetTopologyObjects<TopologySchema, TKey>(IIndexDescriptor<TopologySchema, TKey> indexDescriptor) where TopologySchema : class
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			return tdataAccess.GetTopologyDataAccessProvider().GetTable<TopologySchema, TKey>(indexDescriptor);
		}

		public Task<IEnumerable<TableSchema>> GetTableData<TableSchema, TKey>(IIndexDescriptor<TableSchema, TKey> indexDescriptor) where TableSchema : TableEntity, new()
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			return tdataAccess.GetTopologyDataAccessProvider().GetTableData<TableSchema, TKey>(indexDescriptor, default(CancellationToken), new TracingContext());
		}

		public IDataAccessQuery<WorkDefinitionOverride> GetWorkDefinitionOverrides(CancellationToken cancellationToken, TracingContext traceContext)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			IEnumerable<WorkDefinitionOverride> query = from o in tdataAccess.GetTable<WorkDefinitionOverride, WorkDefinitionOverride>(new WorkDefinitionOverrideIndex())
			select o;
			return this.AsDataAccessQuery<WorkDefinitionOverride>(query);
		}

		public WorkUnit RequestWorkUnit(CancellationToken cancellationToken, TracingContext traceContext)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			return tdataAccess.RequestWorkUnit(cancellationToken, traceContext);
		}

		public int HeartbeatForWorkUnit(WorkUnit workUnit, int workUnitState, out List<WorkUnitEntry> entries, CancellationToken cancellationToken, TracingContext traceContext)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			return tdataAccess.HeartbeatForWorkUnit(workUnit, workUnitState, out entries, cancellationToken, traceContext);
		}

		public Task<int> AddAndRemoveWorkUnitEntries(List<WorkUnitEntry> workUnitEntriesToAdd, List<WorkUnitEntry> workUnitEntriesToRemove, CancellationToken cancellationToken, TracingContext traceContext)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			return tdataAccess.AddAndRemoveWorkUnitEntries(workUnitEntriesToAdd, workUnitEntriesToRemove, cancellationToken, traceContext);
		}

		public Task<int> AddWorkUnit(CancellationToken cancellationToken, TracingContext traceContext)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			return tdataAccess.AddWorkUnit(cancellationToken, traceContext);
		}

		public int GetWorkState(CancellationToken cancellationToken, TracingContext traceContext)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			return tdataAccess.GetWorkState(cancellationToken, traceContext);
		}

		public Task SaveAllStatusEntries(CancellationToken cancellationToken, TracingContext traceContext)
		{
			TDataAccess tdataAccess = Activator.CreateInstance<TDataAccess>();
			Task<List<StatusEntryCollection>> allStatusEntries = tdataAccess.GetAllStatusEntries(cancellationToken, traceContext);
			List<StatusEntryCollection> result = allStatusEntries.Result;
			foreach (StatusEntryCollection statusEntryCollection in result)
			{
				foreach (StatusEntry entry in statusEntryCollection.ItemsToRemove)
				{
					tdataAccess.SaveStatusEntry(entry, cancellationToken, traceContext).Wait();
				}
				foreach (StatusEntry entry2 in statusEntryCollection)
				{
					tdataAccess.SaveStatusEntry(entry2, cancellationToken, traceContext).Wait();
				}
			}
			TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>();
			taskCompletionSource.SetResult(0);
			return taskCompletionSource.Task;
		}

		private void ValidateAndSyncAttributes<TDefinition>(TDefinition definition) where TDefinition : WorkDefinition
		{
			List<string> list = new List<string>();
			if (!definition.Validate(list))
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("The work definition fails validation or misses mandatory properties. ");
				list.ForEach(delegate(string e)
				{
					sb.AppendLine(e);
				});
				throw new ArgumentException(sb.ToString());
			}
			definition.SyncExtensionAttributes(false);
			if (definition.StartTime == DateTime.MinValue)
			{
				definition.StartTime = DateTime.UtcNow.AddSeconds((double)this.random.Next(definition.RecurrenceIntervalSeconds));
			}
		}

		private void InitializeCreatedByWorkItem<TDefinition>(TDefinition definition, TracingContext traceContext) where TDefinition : WorkDefinition
		{
			if (definition != null && definition.CreatedById == 0 && traceContext != null && traceContext.WorkItem != null)
			{
				definition.CreatedById = traceContext.WorkItem.Id;
			}
		}

		bool IWorkBrokerBase.IsLocal()
		{
			return base.IsLocal();
		}

		TimeSpan IWorkBrokerBase.get_DefaultResultWindow()
		{
			return base.DefaultResultWindow;
		}

		private Random random = new Random();
	}
}
