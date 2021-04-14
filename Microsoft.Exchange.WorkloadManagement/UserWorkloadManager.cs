using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class UserWorkloadManager : DisposeTrackableBase, IUserWorkloadManager
	{
		public UserWorkloadManager(int maxThreadCount, int maxTasksQueued, int maxDelayCacheTasks, TimeSpan maxDelayCacheTime)
		{
			UserWorkloadManager.ValidateParams(maxThreadCount, maxTasksQueued, maxDelayCacheTasks, maxDelayCacheTime);
			this.maxThreadCount = maxThreadCount;
			this.maxTasksQueued = maxTasksQueued;
			this.maxDelayCacheTime = maxDelayCacheTime;
			this.taskQueue = new Queue<WrappedTask>(maxTasksQueued);
			this.workerThreadCount = 0;
			this.delayCache = new ExactTimeoutCache<int, WrappedTask>(new RemoveItemDelegate<int, WrappedTask>(this.HandleDelayCacheRemove), null, new UnhandledExceptionDelegate(this.HandleDelayCacheThreadException), maxDelayCacheTasks, true, CacheFullBehavior.ExpireExisting);
			this.maxWrappedTaskPoolSize = 2 * maxTasksQueued;
			this.wrappedTaskPool = new Stack<WrappedTask>(this.maxWrappedTaskPoolSize);
			this.allActiveTasks = new ConcurrentDictionary<ITask, WrappedTask>();
			this.PermitSynchronousExecution = UserWorkloadManager.PermitSynchronousTaskExecutionEntry.Value;
		}

		public static TimeSpan MinimumEnforcedDelayTime { get; set; }

		public static UserWorkloadManager Singleton
		{
			get
			{
				if (UserWorkloadManager.singleton == null)
				{
					throw new InvalidOperationException("UserWorkloadManager must be initialized before accessing Singleton.");
				}
				return UserWorkloadManager.singleton;
			}
		}

		public bool PermitSynchronousExecution { get; set; }

		public bool IsQueueFull
		{
			get
			{
				return this.taskQueue.Count >= this.maxTasksQueued;
			}
		}

		public bool Canceled
		{
			get
			{
				return base.IsDisposed;
			}
		}

		public int DelayedTaskCount
		{
			get
			{
				if (this.delayCache != null)
				{
					return this.delayCache.Count;
				}
				return 0;
			}
		}

		public int QueueTaskCount
		{
			get
			{
				return this.taskQueue.Count;
			}
		}

		public int ExecutingTaskCount
		{
			get
			{
				return this.workerThreadCount;
			}
		}

		public int TotalTasks
		{
			get
			{
				return this.DelayedTaskCount + this.QueueTaskCount + this.ExecutingTaskCount;
			}
		}

		public uint TaskSubmissionFailuresPerMinute
		{
			get
			{
				return this.taskSubmissionFailuresPerMinute.GetValue();
			}
		}

		public uint TasksCompletedPerMinute
		{
			get
			{
				return this.tasksCompletedPerMinute.GetValue();
			}
		}

		public uint TaskTimeoutsPerMinute
		{
			get
			{
				return this.taskTimeoutsPerMinute.GetValue();
			}
		}

		public Action<ITask, TimeSpan> OnDelayTask { get; set; }

		public Action<ITask, RemoveReason> OnTaskReleaseFromDelay { get; set; }

		public Func<ITask, DelayInfo, DelayInfo> OnGetDelayForTest { get; set; }

		public static int GetAvailableThreads()
		{
			int result;
			int num;
			ThreadPool.GetAvailableThreads(out result, out num);
			return result;
		}

		public static void Initialize(int maxThreadCount, int maxTasksQueued, int maxDelayCacheTasks, TimeSpan maxDelayCacheTime, int? delayTimeThreshold)
		{
			if (UserWorkloadManager.singleton != null && !UserWorkloadManager.singleton.Canceled)
			{
				throw new InvalidOperationException("UserWorkloadManager may only be initialized once.");
			}
			UserWorkloadManager.ValidateParams(maxThreadCount, maxTasksQueued, maxDelayCacheTasks, maxDelayCacheTime);
			ExTraceGlobals.UserWorkloadManagerTracer.TraceError(0L, "UserWorkloadManager initialized with maxThreadCount: {0}, maxTasksQueued: {1}, maxDelayCacheTasks: {2}, and maxDelayCacheTime: {3}", new object[]
			{
				maxThreadCount,
				maxTasksQueued,
				maxDelayCacheTasks,
				maxDelayCacheTime
			});
			ResourceLoadDelayInfo.Initialize();
			UserWorkloadManagerPerfCounterWrapper.Initialize(maxTasksQueued, maxThreadCount, maxDelayCacheTasks, delayTimeThreshold);
			UserWorkloadManager.MinimumEnforcedDelayTime = ((UserWorkloadManager.E14WLMMinimumEnforcedDelayTimeInSecondsEntry.Value < 0) ? UserWorkloadManager.DefaultMinimumEnforcedDelay : TimeSpan.FromSeconds((double)UserWorkloadManager.E14WLMMinimumEnforcedDelayTimeInSecondsEntry.Value));
			UserWorkloadManager.singleton = new UserWorkloadManager(maxThreadCount, maxTasksQueued, maxDelayCacheTasks, maxDelayCacheTime);
		}

		public void SubmitDelayedTask(ITask task, TimeSpan delayTime)
		{
			UserWorkloadManager.SendWatsonReportOnUnhandledException(delegate
			{
				if (this.Canceled)
				{
					return;
				}
				this.ValidateTask(task);
				if (delayTime > task.MaxExecutionTime)
				{
					delayTime = task.MaxExecutionTime.Add(UserWorkloadManager.ClockCorrectionForMaxDelay);
				}
				this.SubmitDelayedTask(this.GetWrappedTask(task), delayTime, false);
			});
		}

		public bool TrySubmitNewTask(ITask task)
		{
			bool isSubmitted = false;
			UserWorkloadManager.SendWatsonReportOnUnhandledException(delegate
			{
				if (this.Canceled)
				{
					return;
				}
				this.ValidateTask(task);
				WrappedTask wrappedTask = this.GetWrappedTask(task);
				isSubmitted = this.TrySubmitTask(wrappedTask, true, true);
			});
			return isSubmitted;
		}

		public bool TrySubmitNewTaskForTest(ITask task, bool processTask)
		{
			if (this.Canceled)
			{
				return false;
			}
			this.ValidateTask(task);
			WrappedTask wrappedTask = this.GetWrappedTask(task);
			return this.TrySubmitTask(wrappedTask, true, processTask);
		}

		internal static void SendWatsonReportOnUnhandledException(ExWatson.MethodDelegate methodDelegate)
		{
			ExWatson.SendReportOnUnhandledException(methodDelegate, delegate(object exception)
			{
				bool flag = true;
				Exception ex = exception as Exception;
				if (ex != null)
				{
					ExTraceGlobals.UserWorkloadManagerTracer.TraceError<Exception>(0L, "Encountered unhandled exception: {0}", ex);
					flag = !ExWatson.IsWatsonReportAlreadySent(ex);
					if (flag)
					{
						ExWatson.SetWatsonReportAlreadySent(ex);
					}
				}
				ExTraceGlobals.UserWorkloadManagerTracer.TraceError<bool>(0L, "SendWatsonReportOnUnhandledException shouldSendReport: {0}", flag);
				return flag;
			});
		}

		internal static UserWorkloadManagerPerfCounterWrapper GetPerfCounterWrapper(BudgetType budgetType)
		{
			UserWorkloadManagerPerfCounterWrapper userWorkloadManagerPerfCounterWrapper;
			if (!UserWorkloadManager.perfCounterWrappers.TryGetValue(budgetType, out userWorkloadManagerPerfCounterWrapper))
			{
				lock (UserWorkloadManager.perfCounterWrappers)
				{
					if (!UserWorkloadManager.perfCounterWrappers.TryGetValue(budgetType, out userWorkloadManagerPerfCounterWrapper))
					{
						userWorkloadManagerPerfCounterWrapper = new UserWorkloadManagerPerfCounterWrapper(budgetType);
						UserWorkloadManager.perfCounterWrappers.Add(budgetType, userWorkloadManagerPerfCounterWrapper);
					}
				}
			}
			return userWorkloadManagerPerfCounterWrapper;
		}

		internal UserWorkloadManagerResult GetDiagnosticSnapshot(bool dumpCaches)
		{
			UserWorkloadManagerResult userWorkloadManagerResult = new UserWorkloadManagerResult();
			userWorkloadManagerResult.MaxTasksQueued = this.maxTasksQueued;
			userWorkloadManagerResult.MaxThreadCount = this.maxThreadCount;
			userWorkloadManagerResult.MaxDelayCacheTime = this.maxDelayCacheTime.ToString();
			userWorkloadManagerResult.CurrentWorkerThreads = this.workerThreadCount;
			userWorkloadManagerResult.IsQueueFull = this.IsQueueFull;
			userWorkloadManagerResult.Canceled = this.Canceled;
			userWorkloadManagerResult.TotalTaskCount = this.TotalTasks;
			userWorkloadManagerResult.QueuedTaskCount = this.QueueTaskCount;
			userWorkloadManagerResult.DelayedTaskCount = this.DelayedTaskCount;
			long num = this.synchronousExecutions;
			long num2 = this.asyncExecutions;
			userWorkloadManagerResult.SyncToAsyncRatio = string.Format("1:{0}", (num == 0L) ? "?" : ((double)num2 / (double)num).ToString("######0.0###"));
			userWorkloadManagerResult.SynchronousExecutionAllowed = this.PermitSynchronousExecution;
			userWorkloadManagerResult.TaskSubmissionFailuresPerMinute = (int)this.TaskSubmissionFailuresPerMinute;
			userWorkloadManagerResult.TasksCompletedPerMinute = (int)this.TasksCompletedPerMinute;
			userWorkloadManagerResult.TaskTimeoutsPerMinute = (int)this.TaskTimeoutsPerMinute;
			if (dumpCaches)
			{
				List<WLMTaskDetails> list;
				List<WLMTaskDetails> list2;
				List<WLMTaskDetails> list3;
				lock (this.instanceLock)
				{
					list = new List<WLMTaskDetails>(this.taskQueue.Count);
					list2 = new List<WLMTaskDetails>(this.delayCache.Count);
					list3 = new List<WLMTaskDetails>(this.ExecutingTaskCount);
					foreach (KeyValuePair<ITask, WrappedTask> keyValuePair in this.allActiveTasks)
					{
						WrappedTask value = keyValuePair.Value;
						TaskLocation taskLocation = value.TaskLocation;
						WLMTaskDetails taskDetails = value.GetTaskDetails();
						switch (taskLocation)
						{
						case TaskLocation.Queue:
							list.Add(taskDetails);
							break;
						case TaskLocation.DelayCache:
							list2.Add(taskDetails);
							break;
						case TaskLocation.Thread:
							list3.Add(taskDetails);
							break;
						}
					}
				}
				userWorkloadManagerResult.QueuedTasks = list;
				userWorkloadManagerResult.DelayedTasks = list2;
				userWorkloadManagerResult.ExecutingTasks = list3;
			}
			return userWorkloadManagerResult;
		}

		protected override void InternalDispose(bool disposing)
		{
			UserWorkloadManager.SendWatsonReportOnUnhandledException(delegate
			{
				if (disposing)
				{
					lock (this.instanceLock)
					{
						this.disposeLock.EnterWriteLock();
						try
						{
							this.InternalDispose();
						}
						finally
						{
							this.disposeLock.ExitWriteLock();
						}
					}
				}
			});
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UserWorkloadManager>(this);
		}

		private static int GetMaxThreads()
		{
			int result;
			int num;
			ThreadPool.GetMaxThreads(out result, out num);
			return result;
		}

		private static void ValidateParams(int maxThreadCount, int maxTasksQueued, int maxDelayCacheTasks, TimeSpan maxDelayCacheTime)
		{
			if (maxThreadCount < 1)
			{
				throw new ArgumentOutOfRangeException("maxThreadCount", "maxThreadCount must be greater than 0.");
			}
			if (maxThreadCount > UserWorkloadManager.GetMaxThreads())
			{
				throw new ArgumentOutOfRangeException("maxThreadCount", "maxThreadCount may not be greater than " + UserWorkloadManager.GetMaxThreads().ToString());
			}
			if (maxTasksQueued < 1)
			{
				throw new ArgumentOutOfRangeException("maxTasksQueued", "maxTasksQueued must be greater than 0.");
			}
			if (maxDelayCacheTasks < 1)
			{
				throw new ArgumentOutOfRangeException("maxDelayCacheTasks", "maxDelayCacheTasks must be greater than 0.");
			}
			if (maxDelayCacheTime <= TimeSpan.Zero || maxDelayCacheTime > TimeSpan.FromDays(1.0))
			{
				throw new ArgumentOutOfRangeException("maxDelayCacheTime", "maxDelayCacheTime must be positive and less than one day");
			}
		}

		private static void WorkerCallback(object workloadManagerAsObject)
		{
			UserWorkloadManager workloadManager = (UserWorkloadManager)workloadManagerAsObject;
			UserWorkloadManager.SendWatsonReportOnUnhandledException(delegate
			{
				for (;;)
				{
					WrappedTask task = workloadManager.GetTask();
					if (task == null)
					{
						break;
					}
					workloadManager.Execute(task, false);
				}
			});
		}

		private void ValidateTask(ITask task)
		{
			if (task == null)
			{
				throw new ArgumentNullException("task", "Task cannot be null");
			}
			if (task.MaxExecutionTime <= TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("ITask.MaxExecutionTime must be positive");
			}
			if (task.Budget == null)
			{
				throw new ArgumentException("task", "Task cannot have a null budget");
			}
		}

		private bool TrySubmitTask(WrappedTask wrapper, bool isNewTask, bool processTask)
		{
			if (wrapper == null)
			{
				throw new ArgumentNullException("task");
			}
			BudgetType budgetType = wrapper.Task.Budget.Owner.BudgetType;
			ExTraceGlobals.UserWorkloadManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "Task {0} submitted to UserWorkloadManager", wrapper.Task.Description);
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			lock (this.instanceLock)
			{
				if (this.Canceled)
				{
					return false;
				}
				if (isNewTask && this.IsQueueFull)
				{
					ExTraceGlobals.UserWorkloadManagerTracer.TraceError<string>((long)this.GetHashCode(), "Too many jobs queued, task {0} rejected", wrapper.Task.Description);
					flag3 = true;
					flag = false;
				}
				if (!flag3)
				{
					if (isNewTask)
					{
						this.AddTaskToActiveList(wrapper);
					}
					if (this.ExecuteTaskSynchronously(wrapper))
					{
						return true;
					}
					wrapper.StartQueue();
					this.taskQueue.Enqueue(wrapper);
					if (processTask && this.workerThreadCount < this.maxThreadCount)
					{
						this.workerThreadCount++;
						flag2 = true;
					}
				}
			}
			this.UpdatePerfCountersForSubmit(budgetType, isNewTask, flag3);
			if (flag2)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(UserWorkloadManager.WorkerCallback), this);
				this.IncrementWorkerThreadCount(budgetType);
			}
			if (!flag)
			{
				this.taskSubmissionFailuresPerMinute.Add(1U);
			}
			return flag;
		}

		private bool ExecuteTaskSynchronously(WrappedTask wrapper)
		{
			OverBudgetException ex;
			if (this.PermitSynchronousExecution && !wrapper.Task.Budget.TryCheckOverBudget(out ex) && this.GetBudgetBalance(wrapper) >= 0f)
			{
				UserWorkloadManager.SendWatsonReportOnUnhandledException(delegate
				{
					this.Execute(wrapper, true);
				});
				return true;
			}
			return false;
		}

		private float GetBudgetBalance(WrappedTask wrapper)
		{
			StandardBudgetWrapper standardBudgetWrapper = wrapper.Task.Budget as StandardBudgetWrapper;
			if (standardBudgetWrapper == null)
			{
				return 0f;
			}
			return standardBudgetWrapper.GetInnerBudget().CasTokenBucket.GetBalance();
		}

		private void UpdatePerfCountersForSubmit(BudgetType budgetType, bool isNewTask, bool isNewTaskRejected)
		{
			UserWorkloadManagerPerfCounterWrapper perfCounterWrapper = UserWorkloadManager.GetPerfCounterWrapper(budgetType);
			if (perfCounterWrapper != null)
			{
				if (isNewTask)
				{
					perfCounterWrapper.UpdateTotalNewTasksCount();
				}
				if (isNewTaskRejected)
				{
					perfCounterWrapper.UpdateTotalNewTaskRejectionsCount();
					return;
				}
				perfCounterWrapper.UpdateTaskQueueLength((long)this.taskQueue.Count);
			}
		}

		private bool PreExecuteValidation(WrappedTask wrapper, out LocalizedException preExecuteException)
		{
			preExecuteException = null;
			OverBudgetException ex;
			if (wrapper.Task.Budget.TryCheckOverBudget(out ex))
			{
				ExTraceGlobals.UserWorkloadManagerTracer.TraceDebug<BudgetKey, string, OverBudgetException>((long)this.GetHashCode(), "[UserWorkloadManager.PreExecuteValidation] User {0} was over budget in part {1} and therefore the step will be canceled.  Exception: {2}", wrapper.Task.Budget.Owner, ex.PolicyPart, ex);
				preExecuteException = ex;
				return false;
			}
			bool flag = false;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(56604U, ref flag);
			if (flag)
			{
				preExecuteException = new OverBudgetException(wrapper.Task.Budget, "faultInjection", "0", 1000);
				return false;
			}
			ResourceUnhealthyException ex2;
			if (ResourceLoadDelayInfo.TryCheckResourceHealth(wrapper.Task.Budget, wrapper.Task.WorkloadSettings, wrapper.Task.GetResources(), out ex2))
			{
				ExTraceGlobals.UserWorkloadManagerTracer.TraceDebug<BudgetKey, WorkloadSettings, ResourceKey>((long)this.GetHashCode(), "[UserWorkloadManager.PreExecuteValidation] User {0} encountered a resource in critical health for workload settings {1} and therefore the step will be canceled.  Resource: {2}", wrapper.Task.Budget.Owner, wrapper.Task.WorkloadSettings, ex2.ResourceKey);
				preExecuteException = ex2;
				return false;
			}
			return true;
		}

		private void TraceInfiniteDelay(WrappedTask wrapper, DelayInfo delayInfo)
		{
			UserQuotaDelayInfo userQuotaDelayInfo = delayInfo as UserQuotaDelayInfo;
			if (userQuotaDelayInfo != null)
			{
				ExTraceGlobals.UserWorkloadManagerTracer.TraceDebug<BudgetKey, OverBudgetException>((long)this.GetHashCode(), "[UserWorkloadManager.TraceInfiniteDelay] Caller {0} was over budget in a non-latency based policy part.  Exception: {1}", wrapper.Task.Budget.Owner, userQuotaDelayInfo.OverBudgetException);
				return;
			}
			ResourceLoadDelayInfo resourceLoadDelayInfo = delayInfo as ResourceLoadDelayInfo;
			ExTraceGlobals.UserWorkloadManagerTracer.TraceDebug<ResourceKey>((long)this.GetHashCode(), "[UserWorkloadManager.TraceInfiniteDelay] Task accessed critical resource {0} and was stymied.", resourceLoadDelayInfo.ResourceKey);
		}

		private bool ShouldInterruptTask(WrappedTask wrapper, TaskExecuteResult stepResult, out bool canReclaimTaskWrapper)
		{
			if (stepResult == TaskExecuteResult.Undefined)
			{
				throw new ArgumentException("[UserWorkloadManager.ShouldInterruptTask] stepResult cannot be TaskExecuteResult.Undefined", "stepResult");
			}
			canReclaimTaskWrapper = true;
			ResourceKey[] previousStepResources = wrapper.PreviousStepResources;
			DelayInfo delayInfo = ResourceLoadDelayInfo.GetDelay(wrapper.Task.Budget, wrapper.Task.WorkloadSettings, previousStepResources, false);
			bool flag = false;
			if (this.OnGetDelayForTest != null)
			{
				delayInfo = this.OnGetDelayForTest(wrapper.Task, delayInfo);
				flag = true;
			}
			if (delayInfo.Delay == Budget.IndefiniteDelay)
			{
				this.TraceInfiniteDelay(wrapper, delayInfo);
				delayInfo = DelayInfo.NoDelay;
			}
			TimeSpan timeSpan = delayInfo.Delay + (flag ? TimeSpan.Zero : wrapper.UnaccountedForDelay);
			if (timeSpan > TimeSpan.Zero)
			{
				if (stepResult == TaskExecuteResult.StepComplete && timeSpan < UserWorkloadManager.MinimumEnforcedDelayTime)
				{
					ExTraceGlobals.UserWorkloadManagerTracer.TraceDebug<TimeSpan, TimeSpan>((long)this.GetHashCode(), "[UserWorkloadManager.ShouldInterruptTask] Task step suggests delay, but delay {0} is less than the minimum threshold  of {1}.  Deferring delay.", timeSpan, UserWorkloadManager.MinimumEnforcedDelayTime);
					wrapper.UnaccountedForDelay += delayInfo.Delay;
					wrapper.Task.Budget.ResetWorkAccomplished();
					return false;
				}
				TimeSpan totalTime = wrapper.TotalTime;
				if (totalTime + timeSpan > wrapper.Task.MaxExecutionTime)
				{
					ExTraceGlobals.UserWorkloadManagerTracer.TraceDebug<TimeSpan, TimeSpan>((long)this.GetHashCode(), "[UserWorkloadManager.ShouldInterruptTask] Suggested delay {0} would push us pass MaxExecutionTime {1} for the task.  Capping to MaxExecutionTime.", timeSpan, wrapper.Task.MaxExecutionTime);
					timeSpan = wrapper.Task.MaxExecutionTime - totalTime;
				}
				if (timeSpan > TimeSpan.Zero)
				{
					wrapper.UnaccountedForDelay = TimeSpan.Zero;
					wrapper.Task.Budget.ResetWorkAccomplished();
					wrapper.PreviousStepResult = stepResult;
					string instance = ResourceLoadDelayInfo.GetInstance(delayInfo);
					WorkloadManagementLogger.SetThrottlingValues(timeSpan, !(delayInfo is ResourceLoadDelayInfo), instance, wrapper.Task.GetActivityScope());
					if (!(timeSpan <= UserWorkloadManager.SynchronousDelayThreshold))
					{
						this.SubmitDelayedTask(wrapper, timeSpan, true);
						canReclaimTaskWrapper = false;
						return true;
					}
					ThrottlingPerfCounterWrapper.IncrementBudgetsMicroDelayed(wrapper.Task.Budget.Owner);
					Thread.Sleep(timeSpan);
					if (stepResult == TaskExecuteResult.ProcessingComplete)
					{
						this.CompleteTask(wrapper);
						canReclaimTaskWrapper = true;
						return true;
					}
					return false;
				}
			}
			if (stepResult == TaskExecuteResult.ProcessingComplete)
			{
				this.CompleteTask(wrapper);
				return true;
			}
			return false;
		}

		private void SubmitDelayedTask(WrappedTask wrapper, TimeSpan delayTime, bool frameworkSuggested)
		{
			if (delayTime <= TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("delayTime", delayTime, "DelayTime must be greater than zero.");
			}
			if (delayTime > this.maxDelayCacheTime)
			{
				throw new ArgumentOutOfRangeException("delayTime", delayTime, "DelayTime must be less than " + this.maxDelayCacheTime);
			}
			BudgetKey owner = wrapper.Task.Budget.Owner;
			ITask task = null;
			try
			{
				this.disposeLock.EnterReadLock();
				if (base.IsDisposed)
				{
					return;
				}
				task = wrapper.Task;
				if (!frameworkSuggested)
				{
					this.AddTaskToActiveList(wrapper);
				}
				wrapper.StartDelay();
				this.delayCache.TryAddAbsolute(wrapper.GetHashCode(), wrapper, delayTime);
			}
			finally
			{
				try
				{
					this.disposeLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			if (frameworkSuggested)
			{
				UserWorkloadManagerPerfCounterWrapper perfCounterWrapper = UserWorkloadManager.GetPerfCounterWrapper(owner.BudgetType);
				if (perfCounterWrapper != null)
				{
					perfCounterWrapper.UpdateCurrentDelayedTasks((long)this.delayCache.Count);
					perfCounterWrapper.UpdateMaxDelay(owner, (int)delayTime.TotalMilliseconds);
				}
			}
			if (this.OnDelayTask != null && task != null)
			{
				this.OnDelayTask(task, delayTime);
			}
		}

		private void HandleDelayCacheRemove(int key, WrappedTask wrapper, RemoveReason reason)
		{
			UserWorkloadManager.SendWatsonReportOnUnhandledException(delegate
			{
				UserWorkloadManagerPerfCounterWrapper perfCounterWrapper = UserWorkloadManager.GetPerfCounterWrapper(wrapper.Task.Budget.Owner.BudgetType);
				if (perfCounterWrapper != null)
				{
					if (reason != RemoveReason.Cleanup)
					{
						perfCounterWrapper.UpdateCurrentDelayedTasks((long)this.DelayedTaskCount);
					}
					else
					{
						perfCounterWrapper.UpdateCurrentDelayedTasks(0L);
					}
				}
				wrapper.EndDelay();
				switch (reason)
				{
				case RemoveReason.Expired:
				case RemoveReason.PreemptivelyExpired:
				{
					if (this.OnTaskReleaseFromDelay != null)
					{
						this.OnTaskReleaseFromDelay(wrapper.Task, reason);
					}
					if (wrapper.PreviousStepResult == TaskExecuteResult.ProcessingComplete)
					{
						this.CompleteTask(wrapper);
						this.ReturnWrappedTaskToPool(wrapper);
						return;
					}
					this.TrySubmitTask(wrapper, false, true);
					bool canceled = this.Canceled;
					break;
				}
				case RemoveReason.Removed:
					break;
				case RemoveReason.Cleanup:
					this.CancelTask(wrapper);
					return;
				default:
					return;
				}
			});
		}

		private WrappedTask GetTask()
		{
			WrappedTask wrappedTask = null;
			lock (this.instanceLock)
			{
				if (this.taskQueue.Count == 0 || this.Canceled)
				{
					this.workerThreadCount--;
				}
				else
				{
					wrappedTask = this.taskQueue.Dequeue();
					if (wrappedTask != null)
					{
						wrappedTask.EndQueue();
					}
				}
			}
			if (wrappedTask != null)
			{
				this.UpdatePerfCountersForWorkerCallback(wrappedTask);
			}
			return wrappedTask;
		}

		private void UpdatePerfCountersForWorkerCallback(WrappedTask wrapper)
		{
			if (wrapper != null)
			{
				TimeSpan totalTime = wrapper.TotalTime;
				UserWorkloadManagerPerfCounterWrapper perfCounterWrapper = UserWorkloadManager.GetPerfCounterWrapper(wrapper.Task.Budget.Owner.BudgetType);
				if (perfCounterWrapper != null)
				{
					perfCounterWrapper.UpdateAverageTaskWaitTime((long)totalTime.Milliseconds);
					perfCounterWrapper.UpdateTaskQueueLength((long)this.taskQueue.Count);
				}
			}
		}

		private void Execute(WrappedTask wrapper, bool synchronously)
		{
			if (synchronously)
			{
				Interlocked.Increment(ref this.synchronousExecutions);
			}
			else
			{
				Interlocked.Increment(ref this.asyncExecutions);
			}
			string description = wrapper.Task.Description;
			BudgetType budgetType = wrapper.Task.Budget.Owner.BudgetType;
			ExTraceGlobals.UserWorkloadManagerTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "Starting task execution {0}.  Synchronous? {1}", description, synchronously);
			ConnectionPoolManager.BlockImpersonatedCallers();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			UserWorkloadManagerPerfCounterWrapper perfCounterWrapper = UserWorkloadManager.GetPerfCounterWrapper(budgetType);
			WorkloadPolicy workloadPolicy = new WorkloadPolicy(wrapper.Task.WorkloadSettings.WorkloadType);
			WorkloadManagementLogger.SetWorkloadMetadataValues(wrapper.Task.WorkloadSettings.WorkloadType.ToString(), (workloadPolicy != null) ? workloadPolicy.Classification.ToString() : null, wrapper.Task.Budget.ThrottlingPolicy.IsServiceAccount, !wrapper.Task.WorkloadSettings.IsBackgroundLoad, wrapper.Task.GetActivityScope());
			try
			{
				for (;;)
				{
					flag = false;
					flag2 = false;
					if (this.Canceled)
					{
						break;
					}
					wrapper.PreviousStepResources = wrapper.Task.GetResources();
					bool flag4 = false;
					ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(3014012221U, ref flag4);
					TimeSpan totalTime = wrapper.TotalTime;
					if (totalTime > wrapper.Task.MaxExecutionTime || flag4)
					{
						goto IL_14F;
					}
					LocalizedException exception = null;
					if (!this.PreExecuteValidation(wrapper, out exception))
					{
						TaskExecuteResult stepResult = wrapper.CancelStep(exception);
						if (this.ShouldInterruptTask(wrapper, stepResult, out flag3))
						{
							break;
						}
					}
					else
					{
						ExTraceGlobals.UserWorkloadManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "About to execute task {0}", description);
						flag = true;
						TaskExecuteResult taskExecuteResult = wrapper.Execute();
						flag2 = true;
						ExTraceGlobals.UserWorkloadManagerTracer.TraceDebug<string, TaskExecuteResult>((long)this.GetHashCode(), "Finished task execution {0}, ExecuteResult: {1}", description, taskExecuteResult);
						if (this.ShouldInterruptTask(wrapper, taskExecuteResult, out flag3))
						{
							break;
						}
					}
				}
				return;
				IL_14F:
				if (perfCounterWrapper != null)
				{
					perfCounterWrapper.IncrementTimeoutsSeen();
				}
				this.TimeoutTask(wrapper);
			}
			finally
			{
				if (flag3)
				{
					this.ReturnWrappedTaskToPool(wrapper);
					wrapper = null;
				}
				if (flag && !flag2)
				{
					ExTraceGlobals.UserWorkloadManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "Task {0} failed", description);
					if (perfCounterWrapper != null)
					{
						perfCounterWrapper.UpdateTotalTaskExecutionFailuresCount();
					}
				}
				if (!synchronously)
				{
					this.DecrementWorkerThreadCount(budgetType);
				}
				ExTraceGlobals.UserWorkloadManagerTracer.TraceDebug<string>((long)this.GetHashCode(), "Ending task execution {0}", description);
			}
		}

		private void IncrementWorkerThreadCount(BudgetType budgetType)
		{
			int num = 0;
			lock (this.workerThreadCounts)
			{
				if (!this.workerThreadCounts.TryGetValue(budgetType, out num))
				{
					this.workerThreadCounts.Add(budgetType, 1);
				}
				else
				{
					num = (this.workerThreadCounts[budgetType] = num + 1);
				}
			}
		}

		private void DecrementWorkerThreadCount(BudgetType budgetType)
		{
			int num = 0;
			lock (this.workerThreadCounts)
			{
				if (this.workerThreadCounts.TryGetValue(budgetType, out num))
				{
					num = Math.Max(0, num - 1);
					this.workerThreadCounts[budgetType] = num;
				}
			}
		}

		private void HandleDelayCacheThreadException(Exception exception)
		{
			if (!(exception is ThreadAbortException) && !(exception is AppDomainUnloadedException))
			{
				ExWatson.SendReport(exception, ReportOptions.None, null);
			}
		}

		private WrappedTask GetWrappedTask(ITask task)
		{
			WrappedTask result;
			lock (this.wrappedTaskPool)
			{
				if (this.wrappedTaskPool.Count == 0)
				{
					result = new WrappedTask(task);
				}
				else
				{
					WrappedTask wrappedTask = this.wrappedTaskPool.Pop();
					wrappedTask.Initialize(task);
					result = wrappedTask;
				}
			}
			return result;
		}

		private void ReturnWrappedTaskToPool(WrappedTask wrapper)
		{
			wrapper.Initialize(null);
			if (this.wrappedTaskPool.Count >= this.maxWrappedTaskPoolSize)
			{
				ExTraceGlobals.UserWorkloadManagerTracer.TraceDebug((long)this.GetHashCode(), "Wrapped task pool count > maximum wrapped task pool size, so not returning object to pool.");
				return;
			}
			lock (this.wrappedTaskPool)
			{
				if (this.wrappedTaskPool.Count >= this.maxWrappedTaskPoolSize)
				{
					ExTraceGlobals.UserWorkloadManagerTracer.TraceDebug((long)this.GetHashCode(), "Wrapped task pool count > maximum wrapped task pool size, so not returning object to pool.");
				}
				else
				{
					this.wrappedTaskPool.Push(wrapper);
				}
			}
		}

		private void AddTaskToActiveList(WrappedTask wrapper)
		{
			ITask task = wrapper.Task;
			if (!this.allActiveTasks.TryAdd(task, wrapper))
			{
				string message = string.Format("The specified instance of Task '{0}' has already been submitted for execution. Cannot submit it again.", task.Description);
				ExTraceGlobals.UserWorkloadManagerTracer.TraceError((long)this.GetHashCode(), message);
				throw new InvalidOperationException(message);
			}
		}

		private void CompleteTask(WrappedTask wrapper)
		{
			this.RemoveTaskFromActiveList(wrapper.Task);
			this.tasksCompletedPerMinute.Add(1U);
			wrapper.Complete();
		}

		private void TimeoutTask(WrappedTask wrapper)
		{
			this.RemoveTaskFromActiveList(wrapper.Task);
			this.taskTimeoutsPerMinute.Add(1U);
			wrapper.Timeout();
		}

		private void CancelTask(WrappedTask wrapper)
		{
			this.RemoveTaskFromActiveList(wrapper.Task);
			wrapper.Cancel();
		}

		private void RemoveTaskFromActiveList(ITask task)
		{
			WrappedTask wrappedTask;
			this.allActiveTasks.TryRemove(task, out wrappedTask);
		}

		private void InternalDispose()
		{
			if (this.taskQueue != null)
			{
				foreach (WrappedTask wrappedTask in this.taskQueue)
				{
					wrappedTask.Cancel();
				}
				this.taskQueue.Clear();
			}
			if (this.delayCache != null)
			{
				this.delayCache.Dispose();
				this.delayCache = null;
			}
			lock (UserWorkloadManager.perfCounterWrappers)
			{
				foreach (UserWorkloadManagerPerfCounterWrapper userWorkloadManagerPerfCounterWrapper in UserWorkloadManager.perfCounterWrappers.Values)
				{
					userWorkloadManagerPerfCounterWrapper.UpdateCurrentDelayedTasks(0L);
				}
			}
		}

		private const uint LidWLMTimeout = 3014012221U;

		private const uint LidPreExecuteValidationOverBudget = 56604U;

		private static readonly TimeSpan SynchronousDelayThreshold = TimeSpan.FromMilliseconds(10.0);

		private static readonly TimeSpan DefaultMinimumEnforcedDelay = TimeSpan.FromMilliseconds(100.0);

		private static readonly IntAppSettingsEntry E14WLMMinimumEnforcedDelayTimeInSecondsEntry = new IntAppSettingsEntry("E14WLMMinimumEnforcedDelayTimeInSeconds", (int)UserWorkloadManager.DefaultMinimumEnforcedDelay.TotalSeconds, ExTraceGlobals.UserWorkloadManagerTracer);

		private static readonly BoolAppSettingsEntry PermitSynchronousTaskExecutionEntry = new BoolAppSettingsEntry("UserWorkloadManager.PermitSynchronousTaskExecution", false, ExTraceGlobals.UserWorkloadManagerTracer);

		private static readonly TimeSpan ClockCorrectionForMaxDelay = TimeSpan.FromMilliseconds(50.0);

		private static Dictionary<BudgetType, UserWorkloadManagerPerfCounterWrapper> perfCounterWrappers = new Dictionary<BudgetType, UserWorkloadManagerPerfCounterWrapper>();

		private static UserWorkloadManager singleton;

		private readonly int maxTasksQueued;

		private readonly int maxThreadCount;

		private readonly TimeSpan maxDelayCacheTime;

		private readonly int maxWrappedTaskPoolSize;

		private long synchronousExecutions;

		private long asyncExecutions;

		private FixedTimeSum taskSubmissionFailuresPerMinute = new FixedTimeSum(10000, 6);

		private FixedTimeSum tasksCompletedPerMinute = new FixedTimeSum(10000, 6);

		private FixedTimeSum taskTimeoutsPerMinute = new FixedTimeSum(10000, 6);

		private Dictionary<BudgetType, int> workerThreadCounts = new Dictionary<BudgetType, int>();

		private Queue<WrappedTask> taskQueue;

		private int workerThreadCount;

		private ExactTimeoutCache<int, WrappedTask> delayCache;

		private object instanceLock = new object();

		private ReaderWriterLockSlim disposeLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

		private Stack<WrappedTask> wrappedTaskPool;

		private ConcurrentDictionary<ITask, WrappedTask> allActiveTasks;

		private enum DelayResult
		{
			None,
			Delay,
			CancelStepProcessingDone,
			CancelStep,
			Timeout
		}
	}
}
