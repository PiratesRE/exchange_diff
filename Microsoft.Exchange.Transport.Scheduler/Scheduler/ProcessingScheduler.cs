using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Scheduler.Contracts;
using Microsoft.Exchange.Transport.Scheduler.Processing;

namespace Microsoft.Exchange.Transport.Scheduler
{
	internal class ProcessingScheduler : IProcessingScheduler, IProcessingSchedulerAdmin, IQueueLogProvider
	{
		public ProcessingScheduler(int concurrencyLevel, IMessageProcessor processor, bool running = true) : this(concurrencyLevel, processor, ProcessingScheduler.DefaultNoMetering, ProcessingScheduler.DefaultQueueFactory, ProcessingScheduler.DefaultNoThrottling, ProcessingScheduler.DefaultScopedQueuesManager, ProcessingScheduler.DefaultDiagnostics, running)
		{
		}

		public ProcessingScheduler(int concurrencyLevel, IMessageProcessor processor, ISchedulerMetering metering, IQueueFactory queueFactory, ISchedulerThrottler throttler, IScopedQueuesManager scopedQueuesManager, bool running = true) : this(concurrencyLevel, processor, metering, queueFactory, throttler, scopedQueuesManager, ProcessingScheduler.DefaultDiagnostics, running)
		{
		}

		public ProcessingScheduler(int concurrencyLevel, IMessageProcessor processor, ISchedulerMetering metering, IQueueFactory queueFactory, ISchedulerThrottler throttler, IScopedQueuesManager scopedQueuesManager, ISchedulerDiagnostics schedulerDiagnostics, bool running = true)
		{
			this.eventHandlers = new SchedulingEventHandler[Enum.GetNames(typeof(SchedulingState)).Length];
			for (int i = 0; i < this.eventHandlers.Length; i++)
			{
				this.eventHandlers[i] = delegate(SchedulingState param0, ISchedulableMessage param1)
				{
				};
			}
			this.metering = metering;
			this.throttler = throttler;
			this.schedulerDiagnostics = schedulerDiagnostics;
			this.fifoQueue = new LoggingQueueWrapper(queueFactory.CreateNewQueueInstance());
			this.scopedQueuesManager = scopedQueuesManager;
			this.running = (running ? 1 : 0);
			this.jobDispatcher = new JobDispatcher(concurrencyLevel, processor, metering, schedulerDiagnostics);
			this.jobDispatcher.Start(this);
			this.schedulerDiagnostics.RegisterQueueLogging(this);
		}

		public bool IsRunning
		{
			get
			{
				return this.running == 1;
			}
		}

		public void Submit(ISchedulableMessage message)
		{
			ArgumentValidator.ThrowIfNull("message", message);
			lock (this.queueSyncObject)
			{
				this.fifoQueue.Enqueue(message);
			}
			this.schedulerDiagnostics.Received();
			this.NotifyHandlers(SchedulingState.Received, message);
			if (this.running == 1)
			{
				this.jobDispatcher.WorkAvailable();
			}
		}

		public void SubscribeToEvent(SchedulingState state, SchedulingEventHandler handler)
		{
			ArgumentValidator.ThrowIfNull("handler", handler);
			ArgumentValidator.ThrowIfOutOfRange<int>("state", (int)state, 0, this.eventHandlers.Length);
			lock (this.eventHandlerSyncObject)
			{
				SchedulingEventHandler[] array;
				(array = this.eventHandlers)[(int)state] = (SchedulingEventHandler)Delegate.Combine(array[(int)state], handler);
			}
		}

		public void UnsubscribeFromEvent(SchedulingState state, SchedulingEventHandler handler)
		{
			ArgumentValidator.ThrowIfNull("handler", handler);
			ArgumentValidator.ThrowIfOutOfRange<int>("state", (int)state, 0, this.eventHandlers.Length);
			lock (this.eventHandlerSyncObject)
			{
				SchedulingEventHandler[] array;
				(array = this.eventHandlers)[(int)state] = (SchedulingEventHandler)Delegate.Remove(array[(int)state], handler);
			}
		}

		public bool TryGetNext(out ISchedulableMessage message)
		{
			if (this.running == 0)
			{
				message = null;
				return false;
			}
			this.UpdateComponents();
			if (this.scopedQueuesManager.TryGetNext(out message))
			{
				this.NotifyHandlers(SchedulingState.Unscoped, message);
				this.NotifyHandlers(SchedulingState.Dispatched, message);
				return true;
			}
			ISchedulableMessage schedulableMessage;
			while (this.fifoQueue.TryDequeue(out schedulableMessage))
			{
				IEnumerable<IMessageScope> scopes = this.throttler.GetThrottlingScopes(schedulableMessage.Scopes).ToList<IMessageScope>();
				IMessageScope throttledScope;
				if (this.scopedQueuesManager.IsAlreadyScoped(scopes, out throttledScope))
				{
					this.scopedQueuesManager.Add(schedulableMessage, throttledScope);
					this.schedulerDiagnostics.Throttled();
					this.NotifyHandlers(SchedulingState.Scoped, schedulableMessage);
				}
				else
				{
					if (!this.throttler.ShouldThrottle(scopes, out throttledScope))
					{
						message = schedulableMessage;
						this.NotifyHandlers(SchedulingState.Dispatched, schedulableMessage);
						return true;
					}
					this.scopedQueuesManager.Add(schedulableMessage, throttledScope);
					this.schedulerDiagnostics.Throttled();
					this.NotifyHandlers(SchedulingState.Scoped, schedulableMessage);
				}
			}
			message = null;
			return false;
		}

		public void Pause()
		{
			Interlocked.CompareExchange(ref this.running, 0, 1);
		}

		public void Resume()
		{
			if (Interlocked.CompareExchange(ref this.running, 1, 0) == 0)
			{
				this.jobDispatcher.WorkAvailable();
			}
		}

		public bool Shutdown(int timeoutMilliseconds = -1)
		{
			ShutdownCommand shutdownCommand = new ShutdownCommand(this);
			this.jobDispatcher.EnqueueCommand(shutdownCommand);
			return shutdownCommand.WaitForDone(timeoutMilliseconds);
		}

		public IEnumerable<QueueLogInfo> FlushLogs(DateTime checkpoint, ISchedulerMetering metering)
		{
			QueueLogInfo queueLogInfo = new QueueLogInfo("Unscoped", checkpoint)
			{
				UsageData = metering.GetTotalUsage()
			};
			this.fifoQueue.Flush(checkpoint, queueLogInfo);
			return new QueueLogInfo[]
			{
				queueLogInfo
			};
		}

		public SchedulerDiagnosticsInfo GetDiagnosticsInfo()
		{
			return this.schedulerDiagnostics.GetDiagnosticsInfo();
		}

		internal void StartShutdown()
		{
			this.Pause();
			this.jobDispatcher.StartShutdown();
		}

		internal bool WaitForShutdown(int timeoutMilliseconds = -1)
		{
			return this.jobDispatcher.WaitForShutdown(timeoutMilliseconds);
		}

		private void UpdateComponents()
		{
			this.RefreshIfNeeded(this.metering);
			this.RefreshIfNeeded(this.scopedQueuesManager);
			this.RefreshIfNeeded(this.schedulerDiagnostics);
		}

		private void RefreshIfNeeded(object component)
		{
			RefreshableComponent refreshableComponent = component as RefreshableComponent;
			if (refreshableComponent != null)
			{
				refreshableComponent.RefreshIfNecessary();
			}
		}

		private void NotifyHandlers(SchedulingState state, ISchedulableMessage message)
		{
			SchedulingEventHandler schedulingEventHandler = this.eventHandlers[(int)state];
			schedulingEventHandler(state, message);
		}

		public static readonly ISchedulerMetering DefaultNoMetering = new NoOpMetering();

		public static readonly ISchedulerThrottler DefaultNoThrottling = new NoOpThrottler();

		public static readonly IQueueFactory DefaultQueueFactory = new ConcurrentQueueFactory();

		public static readonly ISchedulerDiagnostics DefaultDiagnostics = new SchedulerDiagnostics();

		public static readonly IScopedQueuesManager DefaultScopedQueuesManager = new ScopedQueuesManager(TimeSpan.FromMinutes(2.0), TimeSpan.FromMinutes(15.0), ProcessingScheduler.DefaultQueueFactory, ProcessingScheduler.DefaultNoThrottling, ProcessingScheduler.DefaultDiagnostics, () => DateTime.UtcNow);

		private readonly LoggingQueueWrapper fifoQueue;

		private readonly JobDispatcher jobDispatcher;

		private readonly IScopedQueuesManager scopedQueuesManager;

		private readonly ISchedulerMetering metering;

		private readonly ISchedulerThrottler throttler;

		private readonly ISchedulerDiagnostics schedulerDiagnostics;

		private readonly SchedulingEventHandler[] eventHandlers;

		private readonly object eventHandlerSyncObject = new object();

		private readonly object queueSyncObject = new object();

		private int running;
	}
}
