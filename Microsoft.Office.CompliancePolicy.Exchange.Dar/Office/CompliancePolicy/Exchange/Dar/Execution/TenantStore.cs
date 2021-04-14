using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Office.CompliancePolicy.Dar;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Diagnostics;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Utility;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.Execution
{
	public class TenantStore
	{
		private TenantStore(OrganizationId orgId)
		{
			if (orgId == null)
			{
				throw new ArgumentNullException("orgId");
			}
			this.orgName = ((orgId.OrganizationalUnit != null) ? orgId.OrganizationalUnit.Name : string.Empty);
			this.storeProvider = new StoreObjectProvider(orgId);
			this.task.Start();
		}

		public static IEnumerable<T> Find<T>(string tenantId, SearchFilter filter, bool minimalObject, string correlationId) where T : IConfigurable, new()
		{
			TenantStore tenantStore = TenantStore.Get(tenantId);
			return tenantStore.FindInStore<T>(filter, minimalObject, correlationId).Result;
		}

		public static void SaveTaskAggregate(DarTaskAggregate taskAggregate, string correlationId)
		{
			TenantStore tenantStore = TenantStore.Get(taskAggregate.ScopeId);
			tenantStore.SaveToStore<TaskAggregateStoreObject>(TaskAggregateStoreObject.FromDarTaskAggregate(taskAggregate, tenantStore.storeProvider), correlationId).Wait();
		}

		public static void SaveTask(DarTask task, string correlationId)
		{
			TenantStore tenantStore = TenantStore.Get(task.TenantId);
			tenantStore.SaveToStore<TaskStoreObject>(TaskStoreObject.FromDarTask(task, tenantStore.storeProvider), correlationId).Wait();
		}

		public static void DeleteTasks(string tenantId, SearchFilter searchFilter, string correlationId)
		{
			TenantStore tenantStore = TenantStore.Get(tenantId);
			tenantStore.DeleteFromStore<TaskStoreObject>(searchFilter, correlationId).Wait();
		}

		public static void DeleteTaskAggregates(string tenantId, SearchFilter searchFilter, string correlationId)
		{
			TenantStore tenantStore = TenantStore.Get(tenantId);
			tenantStore.DeleteFromStore<TaskAggregateStoreObject>(searchFilter, correlationId).Wait();
		}

		public static Task SyncActiveTasks(string tenantId, ActiveTasks activeTasks, string correlationId)
		{
			return TenantStore.Get(tenantId).SyncActiveTasks(activeTasks, tenantId, correlationId);
		}

		public static TenantStore Get(string tenantId)
		{
			OrganizationId organizationId;
			if (!OrganizationId.TryCreateFromBytes(Convert.FromBase64String(tenantId), Encoding.UTF8, out organizationId))
			{
				throw new ArgumentException("Cannot get organizationId from: " + tenantId);
			}
			return TenantStore.accessedTenants.GetOrAdd(tenantId, (string key) => new TenantStore(organizationId));
		}

		public Task SyncActiveTasks(ActiveTasks activeTasks, string tenantId, string correlationId)
		{
			return this.Enqueue<Dictionary<string, DarTask>, Dictionary<string, DarTask>>(delegate(Dictionary<string, DarTask> activeTasksList)
			{
				Dictionary<string, DarTask> dictionary = this.SyncListWithStore(activeTasksList);
				activeTasks.Update(tenantId, dictionary);
				return dictionary;
			}, activeTasks.GetByTenant(tenantId), "Sync", correlationId);
		}

		public Task<IEnumerable<T>> FindInStore<T>(SearchFilter filter, bool minimalObject, string correlationId) where T : IConfigurable, new()
		{
			return this.Enqueue<SearchFilter, IEnumerable<T>>((SearchFilter obj) => this.storeProvider.FindPaged<T>(filter, null, true, null, InstanceManager.Current.Settings.PageSize, minimalObject ? TenantStore.minimalProperties : null), filter, "Read", correlationId);
		}

		public Task SaveToStore<T>(T setObject, string correlationId) where T : IConfigurable
		{
			return this.Enqueue<T, bool>(delegate(T obj)
			{
				this.storeProvider.Save(obj);
				return true;
			}, setObject, "Save", correlationId);
		}

		public Task DeleteFromStore<T>(T deleteObject, string correlationId) where T : IConfigurable
		{
			return this.Enqueue<T, bool>(delegate(T obj)
			{
				this.storeProvider.Delete(obj);
				return true;
			}, deleteObject, "Delete", correlationId);
		}

		public Task DeleteFromStore<T>(SearchFilter searchFilter, string correlationId) where T : IConfigurable, new()
		{
			return this.FindInStore<T>(searchFilter, true, correlationId).ContinueWith(delegate(Task<IEnumerable<T>> t)
			{
				foreach (T deleteObject in t.Result)
				{
					this.DeleteFromStore<T>(deleteObject, correlationId).Wait();
				}
			});
		}

		private Task<TOut> Enqueue<TIn, TOut>(Func<TIn, TOut> action, TIn parameter, string tag, string correlationId)
		{
			string text = Helper.DumpObject(parameter);
			LogItem.Publish("TenantStore", "Enqueue" + tag, ((!string.IsNullOrEmpty(text)) ? ("Input: " + text) : string.Empty) + ((this.queueLength > 0) ? (". Requests to go: " + this.queueLength.ToString()) : string.Empty), this.orgName, correlationId, ResultSeverityLevel.Informational);
			Interlocked.Increment(ref this.queueLength);
			Task<TOut> result;
			lock (this.storeProvider)
			{
				Task<TOut> task = this.task.ContinueWith<TOut>((Task t) => this.PeformStoreAction<TIn, TOut>(action, parameter, tag, correlationId));
				this.task = task;
				result = task;
			}
			return result;
		}

		private TOut PeformStoreAction<TIn, TOut>(Func<TIn, TOut> action, TIn parameter, string tag, string correlationId)
		{
			Interlocked.Decrement(ref this.queueLength);
			TOut result = default(TOut);
			Stopwatch stopwatch = Stopwatch.StartNew();
			ExceptionHandler.Handle(delegate
			{
				result = action(parameter);
			}, new ExceptionGroupHandler(ExceptionGroupHandlers.Web), new ExceptionHandlingOptions(TimeSpan.FromMinutes(1.0))
			{
				ClientId = "TenantStore",
				Operation = tag,
				CorrelationId = correlationId
			});
			stopwatch.Stop();
			string text = Helper.DumpObject(result);
			LogItem.Publish("TenantStore", "Finished" + tag, ((!string.IsNullOrEmpty(text)) ? ("Output: " + text + ". ") : string.Empty) + "Elapsed:" + stopwatch.Elapsed.ToString(), correlationId);
			return result;
		}

		private Dictionary<string, DarTask> SyncListWithStore(Dictionary<string, DarTask> list)
		{
			List<SearchFilter> list2 = new List<SearchFilter>();
			Dictionary<string, DarTask> dictionary = new Dictionary<string, DarTask>();
			foreach (TaskStoreObject taskStoreObject in this.storeProvider.FindPaged<TaskStoreObject>(TaskHelper.GetActiveTaskFilter(), null, true, null, InstanceManager.Current.Settings.PageSize, TenantStore.minimalProperties))
			{
				string id = taskStoreObject.Id;
				if (list.ContainsKey(id))
				{
					dictionary[id] = list[id];
					DarTask darTask;
					if (list.TryGetValue(id, out darTask))
					{
						TaskStoreObject taskStoreObject2 = (darTask.WorkloadContext != null) ? TaskStoreObject.FromDarTask(darTask, this.storeProvider) : null;
						if (taskStoreObject2 != null && taskStoreObject2.LastModifiedTime >= taskStoreObject.LastModifiedTime)
						{
							continue;
						}
					}
				}
				list2.Add(new SearchFilter.IsEqualTo(EwsStoreObjectSchema.AlternativeId.StorePropertyDefinition, taskStoreObject.Id));
			}
			if (list2.Count > 0)
			{
				foreach (TaskStoreObject taskStoreObject3 in this.storeProvider.FindPaged<TaskStoreObject>(new SearchFilter.SearchFilterCollection(1, list2.ToArray()), null, true, null, InstanceManager.Current.Settings.PageSize, new ProviderPropertyDefinition[0]))
				{
					dictionary[taskStoreObject3.Id] = taskStoreObject3.ToDarTask(InstanceManager.Current.Provider);
				}
			}
			return dictionary;
		}

		private static ProviderPropertyDefinition[] minimalProperties = new ProviderPropertyDefinition[]
		{
			EwsStoreObjectSchema.Identity,
			TaskStoreObjectSchema.Id,
			TaskStoreObjectSchema.LastModifiedTime,
			EwsStoreObjectSchema.ItemClass,
			TaskStoreObjectSchema.TaskType
		};

		private static ConcurrentDictionary<string, TenantStore> accessedTenants = new ConcurrentDictionary<string, TenantStore>();

		private readonly StoreObjectProvider storeProvider;

		private readonly string orgName;

		private Task task = new Task(delegate()
		{
		});

		private int queueLength;
	}
}
