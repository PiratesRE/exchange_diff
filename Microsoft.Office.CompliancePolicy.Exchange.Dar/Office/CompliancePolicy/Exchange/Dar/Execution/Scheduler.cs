using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Office.CompliancePolicy.Dar;
using Microsoft.Office.CompliancePolicy.Exchange.Dar.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.Execution
{
	internal class Scheduler : SystemWorkloadBase
	{
		public Scheduler(InstanceManager instance)
		{
			this.provider = new ExDarServiceProvider();
			this.taskManager = new DarTaskManager(this.provider);
			this.prioritizedCategories = from DarTaskCategory t in Enum.GetValues(typeof(DarTaskCategory))
			orderby (int)t descending
			select t;
		}

		public override int BlockedTaskCount
		{
			get
			{
				return InstanceManager.Current.GetActiveTaskList(null).Count<DarTask>() - InstanceManager.Current.GetReadyTaskList().Count<DarTask>();
			}
		}

		public override string Id
		{
			get
			{
				return WorkloadType.DarRuntime.ToString();
			}
		}

		public override int TaskCount
		{
			get
			{
				return InstanceManager.Current.GetReadyTaskList().Count<DarTask>();
			}
		}

		public override WorkloadType WorkloadType
		{
			get
			{
				return WorkloadType.DarRuntime;
			}
		}

		public void Start()
		{
			if (SystemWorkloadManager.Status == WorkloadExecutionStatus.NotInitialized)
			{
				SystemWorkloadManager.Initialize(this.provider.ExecutionLog as IWorkloadLogger);
			}
			SystemWorkloadManager.RegisterWorkload(this);
		}

		public void Stop()
		{
			SystemWorkloadManager.UnregisterWorkload(this);
		}

		protected override SystemTaskBase GetTask(ResourceReservationContext context)
		{
			SystemTaskBase result;
			lock (this.instanceLock)
			{
				foreach (DarTaskCategory category in this.prioritizedCategories)
				{
					DarTask darTask = this.taskManager.Dequeue(1, category, context).FirstOrDefault<DarTask>();
					if (darTask != null)
					{
						ResourceKey resourceKey;
						ResourceReservation resourceReservation = this.GetResourceReservation(darTask, context, out resourceKey);
						if (resourceReservation != null)
						{
							darTask.TaskState = DarTaskState.Running;
							return new TaskWrapper(darTask, this.taskManager, this, resourceReservation);
						}
						LogItem.Publish("Scheduler", "ResourcesNotReseved", ("Throttled resource: " + resourceKey != null) ? resourceKey.ToString() : "null", darTask.CorrelationId, ResultSeverityLevel.Warning);
					}
				}
				base.Pause();
				result = null;
			}
			return result;
		}

		private ResourceReservation GetResourceReservation(DarTask task, ResourceReservationContext reservationContext, out ResourceKey throttledResource)
		{
			if (reservationContext == null)
			{
				throw new ArgumentNullException("reservationContext");
			}
			string taskType = task.TaskType;
			ResourceKey[] resources = new ResourceKey[]
			{
				ProcessorResourceKey.Local
			};
			return reservationContext.GetReservation(this, resources, out throttledResource);
		}

		private ExDarServiceProvider provider;

		private DarTaskManager taskManager;

		private IEnumerable<DarTaskCategory> prioritizedCategories;

		private object instanceLock = new object();
	}
}
