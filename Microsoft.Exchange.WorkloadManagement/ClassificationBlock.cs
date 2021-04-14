using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;

namespace Microsoft.Exchange.WorkloadManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ClassificationBlock
	{
		internal ClassificationBlock(WorkloadClassification classification)
		{
			this.WorkloadClassification = classification;
			this.perfCounter = new ClassificationPerfCounterWrapper(classification);
			ExTraceGlobals.SchedulerTracer.TraceDebug<WorkloadClassification>((long)this.GetHashCode(), "[ClassificationBlock.ctor] Created ClassificationBlock for {0}", classification);
		}

		internal WorkloadClassification WorkloadClassification { get; private set; }

		internal int FairnessFactor
		{
			get
			{
				return this.fairnessFactor;
			}
			set
			{
				lock (this.instanceLock)
				{
					this.fairnessFactor = value;
				}
				this.perfCounter.UpdateFairnessFactor((long)value);
			}
		}

		internal int ActiveThreads
		{
			get
			{
				return this.activeThreadCount;
			}
		}

		internal int WorkloadCount
		{
			get
			{
				return this.workloads.Count;
			}
		}

		internal void Activate()
		{
			int num = Interlocked.Increment(ref this.activeThreadCount);
			this.perfCounter.UpdateActiveThreads((long)num);
		}

		internal void Deactivate()
		{
			int num = Interlocked.Decrement(ref this.activeThreadCount);
			this.perfCounter.UpdateActiveThreads((long)num);
		}

		internal void AddWorkload(SystemWorkloadBase workload)
		{
			if (workload == null)
			{
				throw new ArgumentNullException("workload", "[ClassificationBlock.AddWorkload] workload cannot be null.");
			}
			int num = 0;
			lock (this.instanceLock)
			{
				if (workload.ClassificationBlock != null)
				{
					throw new ArgumentException(string.Format("Workload {0} is already registered to classification block.", workload.Id));
				}
				this.workloads.Add(workload);
				num = this.workloads.Count;
				workload.ClassificationBlock = this;
			}
			this.perfCounter.UpdateWorkloadCount((long)num);
			ExTraceGlobals.SchedulerTracer.TraceDebug<string, WorkloadType>((long)this.GetHashCode(), "[ClassificationBlock.AddWorkload] Added Workload '{0}' for WorkloadType: '{1}'", workload.Id, workload.WorkloadType);
		}

		internal bool RemoveWorkload(SystemWorkloadBase workload)
		{
			bool flag = false;
			int num = 0;
			lock (this.instanceLock)
			{
				flag = this.workloads.Remove(workload);
				if (flag)
				{
					num = this.workloads.Count;
					workload.ClassificationBlock = null;
					if (this.cursorIndex >= this.workloads.Count)
					{
						this.cursorIndex = 0;
					}
				}
			}
			if (flag)
			{
				this.perfCounter.UpdateWorkloadCount((long)num);
			}
			ExTraceGlobals.SchedulerTracer.TraceDebug<string, WorkloadType, bool>((long)this.GetHashCode(), "[Classification.RemoveWorkload] Removed Workload '{0}' for WorkloadType: '{1}', Removed? {2}", workload.Id, workload.WorkloadType, flag);
			return flag;
		}

		internal SystemWorkloadBase[] GetWorkloads()
		{
			SystemWorkloadBase[] result;
			lock (this.instanceLock)
			{
				if (this.workloads == null || this.workloads.Count < 1)
				{
					result = null;
				}
				else
				{
					SystemWorkloadBase[] array = new SystemWorkloadBase[this.workloads.Count];
					this.workloads.CopyTo(array);
					result = array;
				}
			}
			return result;
		}

		internal SystemWorkloadBase GetNextWorkload()
		{
			SystemWorkloadBase systemWorkloadBase = null;
			bool flag = false;
			lock (this.instanceLock)
			{
				if (this.workloads.Count == 0)
				{
					return null;
				}
				int num = this.cursorIndex;
				for (;;)
				{
					systemWorkloadBase = this.workloads[this.cursorIndex];
					if (this.workloads.Count > 1)
					{
						this.cursorIndex = (this.cursorIndex + 1) % this.workloads.Count;
					}
					if (!systemWorkloadBase.Paused && systemWorkloadBase.TaskCount > 0)
					{
						break;
					}
					if (num == this.cursorIndex)
					{
						goto IL_91;
					}
				}
				flag = true;
				IL_91:;
			}
			if (!flag)
			{
				return null;
			}
			return systemWorkloadBase;
		}

		private volatile int cursorIndex;

		private ClassificationPerfCounterWrapper perfCounter;

		private object instanceLock = new object();

		private List<SystemWorkloadBase> workloads = new List<SystemWorkloadBase>();

		private int activeThreadCount;

		private int fairnessFactor;
	}
}
