using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class TimeBasedAssistantControllerWrapper : SystemWorkloadBase, IDisposable
	{
		public TimeBasedAssistantControllerWrapper(TimeBasedAssistantController controller)
		{
			this.controller = controller;
		}

		public TimeBasedAssistantController Controller
		{
			get
			{
				return this.controller;
			}
		}

		public override WorkloadType WorkloadType
		{
			get
			{
				return this.Controller.TimeBasedAssistantType.WorkloadType;
			}
		}

		public override string Id
		{
			get
			{
				return this.Controller.TimeBasedAssistantType.Identifier.ToString();
			}
		}

		public override int TaskCount
		{
			get
			{
				return this.GetTaskCount();
			}
		}

		public override int BlockedTaskCount
		{
			get
			{
				return 0;
			}
		}

		public void Dispose()
		{
			this.controller.Dispose();
		}

		protected override SystemTaskBase GetTask(ResourceReservationContext context)
		{
			List<Guid> list = null;
			Guid guid = this.lastProcessedDriverGuid;
			lock (this.instanceLock)
			{
				TimeBasedAssistantTask timeBasedAssistantTask = null;
				List<TimeBasedAssistantTask> list2 = new List<TimeBasedAssistantTask>();
				foreach (SystemTaskBase systemTaskBase in this.tasksWaitingExecution)
				{
					TimeBasedAssistantTask timeBasedAssistantTask2 = (TimeBasedAssistantTask)systemTaskBase;
					IEnumerable<ResourceKey> resourceDependencies = timeBasedAssistantTask2.ResourceDependencies;
					if (resourceDependencies != null)
					{
						ResourceReservation reservation = context.GetReservation(this, resourceDependencies);
						if (reservation != null)
						{
							timeBasedAssistantTask2.ResourceReservation = reservation;
							timeBasedAssistantTask = timeBasedAssistantTask2;
							break;
						}
					}
					else
					{
						list2.Add(timeBasedAssistantTask2);
					}
				}
				foreach (TimeBasedAssistantTask value in list2)
				{
					this.tasksWaitingExecution.Remove(value);
				}
				if (timeBasedAssistantTask != null)
				{
					this.tasksWaitingExecution.Remove(timeBasedAssistantTask);
					return timeBasedAssistantTask;
				}
			}
			TimeBasedDatabaseDriver nextDriver;
			ResourceReservation reservation2;
			for (;;)
			{
				nextDriver = this.Controller.GetNextDriver(guid);
				if (nextDriver == null)
				{
					break;
				}
				guid = nextDriver.DatabaseInfo.Guid;
				if (list != null && list.Contains(guid))
				{
					goto Block_4;
				}
				if (!nextDriver.HasTask())
				{
					ExTraceGlobals.TimeBasedAssistantControllerTracer.TraceDebug<Guid, LocalizedString>((long)this.GetHashCode(), "Skipping database '{0}' for assistant '{1}'. There is no task to execute.", guid, this.Controller.TimeBasedAssistantType.Name);
				}
				else
				{
					IEnumerable<ResourceKey> resourceDependencies2 = nextDriver.ResourceDependencies;
					if (resourceDependencies2 != null)
					{
						reservation2 = context.GetReservation(this, resourceDependencies2);
						if (reservation2 != null)
						{
							goto IL_1E1;
						}
						ExTraceGlobals.TimeBasedAssistantControllerTracer.TraceDebug<Guid, LocalizedString>((long)this.GetHashCode(), "Skipping database '{0}' for assistant '{1}'. Dependent resources are not currently available for this assistant.", guid, this.Controller.TimeBasedAssistantType.Name);
					}
					else
					{
						ExTraceGlobals.TimeBasedAssistantControllerTracer.TraceDebug<Guid, LocalizedString>((long)this.GetHashCode(), "The driver for database {0} assistant {1} did not return any resource dependencies. This is possible only when the driver is not started. Skipping tasks from this driver.", guid, this.Controller.TimeBasedAssistantType.Name);
					}
				}
				if (list == null)
				{
					list = new List<Guid>();
				}
				list.Add(guid);
			}
			ExTraceGlobals.TimeBasedAssistantControllerTracer.TraceDebug<LocalizedString>((long)this.GetHashCode(), "There are no drivers available for the assistant controller '{0}' at this time. No task available for execution.", this.Controller.TimeBasedAssistantType.Name);
			return null;
			Block_4:
			ExTraceGlobals.TimeBasedAssistantControllerTracer.TraceDebug<LocalizedString>((long)this.GetHashCode(), "Could not find any tasks to execute for the assistant controller '{0}'. No task available for execution.", this.Controller.TimeBasedAssistantType.Name);
			return null;
			IL_1E1:
			ExTraceGlobals.TimeBasedAssistantControllerTracer.TraceDebug<Guid, LocalizedString>((long)this.GetHashCode(), "A task is available for execution on database {0} for assistant {1}. Submitting the task to RUBS for execution", guid, this.Controller.TimeBasedAssistantType.Name);
			this.lastProcessedDriverGuid = guid;
			return new TimeBasedAssistantTask(this, nextDriver, reservation2);
		}

		protected override void YieldTask(SystemTaskBase task)
		{
			lock (this.instanceLock)
			{
				this.tasksWaitingExecution.AddLast(task);
			}
		}

		private int GetTaskCount()
		{
			return this.Controller.GetTaskCount() + this.tasksWaitingExecution.Count;
		}

		private Guid lastProcessedDriverGuid = Guid.Empty;

		private TimeBasedAssistantController controller;

		private LinkedList<SystemTaskBase> tasksWaitingExecution = new LinkedList<SystemTaskBase>();

		private object instanceLock = new object();
	}
}
