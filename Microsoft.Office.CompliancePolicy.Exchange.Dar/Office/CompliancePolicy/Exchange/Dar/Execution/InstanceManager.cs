using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Office.CompliancePolicy.Dar;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Utility;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.Execution
{
	internal class InstanceManager
	{
		public InstanceManager()
		{
			this.Provider = new ExDarServiceProvider();
			this.Settings = new ExecutionSettings();
			this.Tenants = new ActiveTasks();
			this.TaskAggregates = new TaskAggregates();
			this.Scheduler = new Scheduler(this);
		}

		public static InstanceManager Current
		{
			get
			{
				return InstanceManager.current;
			}
		}

		public ExecutionSettings Settings { get; private set; }

		public ActiveTasks Tenants { get; private set; }

		public TaskAggregates TaskAggregates { get; private set; }

		public Scheduler Scheduler { get; private set; }

		public DarServiceProvider Provider { get; private set; }

		public void Start()
		{
			this.Scheduler.Start();
		}

		public void Stop()
		{
			this.Scheduler.Stop();
		}

		public void NotifyTaskStoreChange(string tenantId, string correlationId)
		{
			if (tenantId == null)
			{
				throw new ArgumentNullException("tenantId");
			}
			if (this.currentSyncTask != null && !this.currentSyncTask.IsCompleted && this.prevSyncTask != null && !this.prevSyncTask.IsCompleted)
			{
				return;
			}
			if (this.currentSyncTask != null && !this.currentSyncTask.IsCompleted)
			{
				this.prevSyncTask = this.currentSyncTask;
			}
			this.currentSyncTask = TenantStore.SyncActiveTasks(tenantId, this.Tenants, correlationId);
		}

		public IEnumerable<DarTask> GetReadyTaskList()
		{
			DateTime now = DateTime.UtcNow;
			return from t in InstanceManager.Current.GetActiveTaskList(null)
			where t.TaskState == DarTaskState.Ready && t.MinTaskScheduleTime < now
			where TaskHelper.IsValid(t)
			select t;
		}

		public IEnumerable<DarTask> GetActiveTaskList(string tenantId = null)
		{
			return this.Tenants.GetByTenantOrAll(tenantId);
		}

		private static InstanceManager current = new InstanceManager();

		private Task currentSyncTask;

		private Task prevSyncTask;
	}
}
