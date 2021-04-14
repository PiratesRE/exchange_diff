using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal class JobDispatcher
	{
		public JobDispatcher(int concurrencyLevel, IMessageProcessor processor, ISchedulerMetering metering, ISchedulerDiagnostics schedulerDiagnostics, Func<DateTime> timeProvider)
		{
			ArgumentValidator.ThrowIfInvalidValue<int>("concurrencyLevel", concurrencyLevel, (int count) => count > 0);
			ArgumentValidator.ThrowIfNull("processor", processor);
			ArgumentValidator.ThrowIfNull("metering", metering);
			ArgumentValidator.ThrowIfNull("timeProvider", timeProvider);
			ArgumentValidator.ThrowIfNull("schedulerDiagnostics", schedulerDiagnostics);
			this.processor = processor;
			this.schedulerDiagnostics = schedulerDiagnostics;
			this.concurrencySemaphore = new SemaphoreSlim(concurrencyLevel);
			this.jobManager = new JobManager(metering, timeProvider);
		}

		public JobDispatcher(int concurrencyLevel, IMessageProcessor processor, ISchedulerMetering metering, ISchedulerDiagnostics schedulerDiagnostics) : this(concurrencyLevel, processor, metering, schedulerDiagnostics, () => DateTime.UtcNow)
		{
		}

		public void Start(IProcessingScheduler processingScheduler)
		{
			ArgumentValidator.ThrowIfNull("processingScheduler", processingScheduler);
			this.scheduler = processingScheduler;
			Task.Run(delegate()
			{
				this.RunAsync();
			});
		}

		public void WorkAvailable()
		{
			this.newWorkResetEvent.Set();
		}

		public void EnqueueCommand(ISchedulerCommand command)
		{
			ArgumentValidator.ThrowIfNull("command", command);
			this.commands.Enqueue(command);
			this.WorkAvailable();
		}

		public async void RunAsync()
		{
			while (await this.newWorkResetEvent.WaitAsync())
			{
				for (;;)
				{
					JobDispatcher.<>c__DisplayClass6 CS$<>8__locals1 = new JobDispatcher.<>c__DisplayClass6();
					ISchedulerCommand command;
					while (this.commands.TryDequeue(out command))
					{
						command.Execute();
					}
					await this.concurrencySemaphore.WaitAsync();
					this.concurrencySemaphore.Release();
					JobInfo completedJob;
					while (this.jobsCompletedQueue.TryDequeue(out completedJob))
					{
						await completedJob.ExecutingTask;
						this.schedulerDiagnostics.Processed();
						this.jobManager.End(completedJob);
					}
					if (!this.scheduler.TryGetNext(out CS$<>8__locals1.message))
					{
						break;
					}
					JobInfo jobInfo = new JobInfo(DateTime.UtcNow, CS$<>8__locals1.message.Scopes);
					await this.concurrencySemaphore.WaitAsync();
					this.schedulerDiagnostics.Dispatched();
					this.jobManager.Start(jobInfo);
					Task task = new Task(delegate()
					{
						try
						{
							this.processor.Process(CS$<>8__locals1.message);
						}
						finally
						{
							this.concurrencySemaphore.Release();
							this.jobsCompletedQueue.Enqueue(jobInfo);
							this.WorkAvailable();
						}
					});
					jobInfo.ExecutingTask = task;
					task.Start();
				}
			}
		}

		internal void StartShutdown()
		{
			this.jobManager.StartShutdown();
		}

		internal bool WaitForShutdown(int timeoutMilliseconds = -1)
		{
			return this.jobManager.WaitForShutdown(timeoutMilliseconds);
		}

		private readonly JobManager jobManager;

		private readonly ConcurrentQueue<JobInfo> jobsCompletedQueue = new ConcurrentQueue<JobInfo>();

		private readonly ConcurrentQueue<ISchedulerCommand> commands = new ConcurrentQueue<ISchedulerCommand>();

		private readonly AsyncAutoResetEvent newWorkResetEvent = new AsyncAutoResetEvent();

		private readonly SemaphoreSlim concurrencySemaphore;

		private readonly IMessageProcessor processor;

		private readonly ISchedulerDiagnostics schedulerDiagnostics;

		private IProcessingScheduler scheduler;
	}
}
