using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class CatScheduler
	{
		internal CatScheduler(StageInfo[] stages, SubmitMessageQueue submitMessageQueue, IProcessingQuotaComponent processingQuotaComponent)
		{
			this.stages = new ReadOnlyCollection<StageInfo>(stages);
			this.submitMessageQueue = submitMessageQueue;
			ExPerformanceCounter perfCounter = (Components.QueueManager.PerfCountersTotal != null) ? Components.QueueManager.PerfCountersTotal.CategorizerJobAvailability : null;
			this.monitor = new JobHealthMonitor(Math.Min(CatScheduler.maxExecutingJobs, CatScheduler.maxJobThreads), Components.TransportAppConfig.Resolver.JobHealthTimeThreshold, perfCounter);
			this.jobs = new CatSchedulerJobList(this, this.monitor);
			this.jobThreadEntry = new WaitCallback(this.JobThreadEntry);
			this.throttlingEnabled = (Components.IsBridgehead && (Components.TransportAppConfig.ThrottlingConfig.CategorizerTenantThrottlingEnabled || Components.TransportAppConfig.ThrottlingConfig.CategorizerSenderThrottlingEnabled));
			if (this.throttlingEnabled)
			{
				int maxExecutingThreadsLimit = Math.Min(CatScheduler.maxExecutingJobs, CatScheduler.maxJobThreads) + Math.Abs(CatScheduler.maxExecutingJobs - CatScheduler.maxJobThreads) * Components.TransportAppConfig.ThrottlingConfig.CategorizerThrottlingAsyncThreadPercentage / 100;
				this.conditionManager = new SingleQueueWaitConditionManager(this.submitMessageQueue, NextHopSolutionKey.Submission, maxExecutingThreadsLimit, Components.TransportAppConfig.ThrottlingConfig.GetConfig(true), new CostFactory(), processingQuotaComponent, null, ExTraceGlobals.QueuingTracer);
				this.submitMessageQueue.SetConditionManager(this.conditionManager);
			}
		}

		public static ExEventLog EventLogger
		{
			get
			{
				return CatScheduler.eventLogger;
			}
		}

		public static int MaxExecutingJobs
		{
			get
			{
				return CatScheduler.maxExecutingJobs;
			}
		}

		public IList<StageInfo> Stages
		{
			get
			{
				return this.stages;
			}
		}

		public bool Retired
		{
			get
			{
				return this.retired;
			}
		}

		public JobList JobList
		{
			get
			{
				return this.jobs;
			}
		}

		public void Retire()
		{
			ExTraceGlobals.SchedulerTracer.TraceDebug(0L, "Retire Categorizer Scheduler");
			this.retired = true;
			this.jobs.Retire();
		}

		public void Stop()
		{
			if (!this.retired)
			{
				this.Retire();
			}
			ExTraceGlobals.SchedulerTracer.TraceDebug(0L, "Stop Categorizer Scheduler");
			this.jobs.Stop();
		}

		public void CheckAndScheduleJobThread()
		{
			if (this.jobs.PendingJobCount > 0 || (!this.submitMessageQueue.Suspended && (this.submitMessageQueue.ActiveCount > 0 || (this.throttlingEnabled && this.submitMessageQueue.LockedCount > 0 && this.conditionManager.MapStateChanged))))
			{
				if (this.retired)
				{
					ExTraceGlobals.SchedulerTracer.TraceDebug(0L, "Cat Scheduler is retired - don't start any more threads");
					return;
				}
				ExTraceGlobals.SchedulerTracer.TraceDebug(0L, "More work to do - try and schedule a thread");
				this.ScheduleJobThread();
			}
		}

		public void MonitorJobs()
		{
			this.monitor.UpdateJobUsagePerfCounter(null);
		}

		public void JobThreadEntry(object ignored)
		{
			ExTraceGlobals.SchedulerTracer.TraceDebug(0L, "Start JobThreadEntry to run a job");
			for (int i = 0; i < CatScheduler.maxJobsPerThread; i++)
			{
				if (this.retired)
				{
					ExTraceGlobals.SchedulerTracer.TraceDebug(0L, "Categorizer is retired - stop running more jobs");
					break;
				}
				Job nextJobToRun = this.jobs.GetNextJobToRun();
				if (nextJobToRun == null)
				{
					ExTraceGlobals.SchedulerTracer.TraceDebug(0L, "no more work to do or unable to run more jobs currently");
					break;
				}
				nextJobToRun.ExecutePendingTasks();
			}
			Interlocked.Decrement(ref this.executingJobThreads);
			this.CheckAndScheduleJobThread();
		}

		public void RunningJobCompleted(Job job, TransportMailItem mailItem)
		{
			ExTraceGlobals.SchedulerTracer.TraceDebug<Job>(0L, "Job({0}) completed", job);
			this.jobs.RemoveExecutingJob(job);
			this.MessageCompleted(mailItem, job.GetThrottlingContext(), job.GetQueuedRecipientsByAgeToken());
		}

		public void RunningJobRetired(Job job, TransportMailItem mailItem)
		{
			ExTraceGlobals.SchedulerTracer.TraceDebug<Job>(0L, "Job({0}) retired", job);
			bool flag = this.jobs.RemoveExecutingJob(job);
			if (flag)
			{
				this.MessageCompleted(mailItem, job.GetThrottlingContext(), job.GetQueuedRecipientsByAgeToken());
			}
		}

		public void MoveRunningJobToPending(Job job)
		{
			this.jobs.MoveRunningJobToPending(job);
		}

		public Job CreateNewJob()
		{
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2621844797U);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3674615101U);
			TransportMailItem transportMailItem = (TransportMailItem)this.submitMessageQueue.Dequeue();
			if (transportMailItem == null)
			{
				return null;
			}
			ThrottlingContext context2;
			Job job = CategorizerJobsUtil.SetupNewJob(transportMailItem, this.stages, (QueuedRecipientsByAgeToken ageToken, ThrottlingContext context, IList<StageInfo> catStages) => ReusableJob.NewJob(this, context, ageToken), out context2);
			if (job == null)
			{
				this.NotifyConditionManagerMessageCompleted(context2);
			}
			return job;
		}

		internal CategorizerItem GetScheduledCategorizerItemById(long mailItemId)
		{
			Job[] array = this.jobs.ToArray();
			foreach (Job job in array)
			{
				CategorizerItem categorizerItemById = job.GetCategorizerItemById(mailItemId);
				if (categorizerItemById != null)
				{
					return categorizerItemById;
				}
			}
			return null;
		}

		internal void VisitCategorizerItems(Func<CategorizerItem, bool> visitor)
		{
			Job[] array = this.jobs.ToArray();
			foreach (Job job in array)
			{
				job.VisitCategorizerItems(visitor);
			}
		}

		internal int GetMailItemCount()
		{
			Job[] array = this.jobs.ToArray();
			int num = 0;
			foreach (Job job in array)
			{
				num += job.GetMailItemCount();
			}
			return num;
		}

		internal void TimedUpdate()
		{
			if (!this.throttlingEnabled)
			{
				return;
			}
			this.conditionManager.TimedUpdate();
		}

		internal XElement GetDiagnosticInfo(bool verbose, bool conditionalQueuing)
		{
			XElement xelement = new XElement("Categorizer");
			xelement.Add(new XElement("ExecutingJobs", this.jobs.ExecutingJobCount));
			xelement.Add(new XElement("PendingJobs", this.jobs.PendingJobCount));
			if (verbose)
			{
				Job[] array = this.JobList.ToArray();
				XElement xelement2 = new XElement("jobs");
				foreach (Job job in array)
				{
					XElement xelement3 = new XElement("job");
					xelement3.Add(new XElement("Id", job.Id));
					xelement3.Add(new XElement("MailItemCount", job.GetMailItemCount()));
					xelement2.Add(xelement3);
				}
				xelement.Add(xelement2);
			}
			if (conditionalQueuing && this.conditionManager != null)
			{
				xelement.Add(this.conditionManager.GetDiagnosticInfo(verbose));
			}
			xelement.Add(this.submitMessageQueue.GetDiagnosticInfo(verbose, conditionalQueuing));
			return xelement;
		}

		private void ScheduleJobThread()
		{
			if (this.jobs.ExecutingJobCount >= CatScheduler.MaxExecutingJobs)
			{
				ExTraceGlobals.SchedulerTracer.TraceDebug(0L, "ScheduleJobThread: max # executing jobs limit reached");
				return;
			}
			int num = Interlocked.Increment(ref this.executingJobThreads);
			if (num > CatScheduler.maxJobThreads)
			{
				Interlocked.Decrement(ref this.executingJobThreads);
				ExTraceGlobals.SchedulerTracer.TraceDebug(0L, "ScheduleJobThread: max executing thread count reached");
				return;
			}
			if (this.retired)
			{
				Interlocked.Decrement(ref this.executingJobThreads);
				ExTraceGlobals.SchedulerTracer.TraceDebug(0L, "ScheduleJobThread: Categorizer is retiring");
				return;
			}
			ExTraceGlobals.SchedulerTracer.TraceDebug(0L, "ScheduleJobThread: queue JobThreadEntry to run a job");
			ThreadPool.QueueUserWorkItem(this.jobThreadEntry);
		}

		private void MessageCompleted(TransportMailItem mailItem, ThrottlingContext context, QueuedRecipientsByAgeToken token)
		{
			Components.QueueManager.GetQueuedRecipientsByAge().TrackExitingCategorizer(token);
			this.NotifyConditionManagerMessageCompleted(context);
		}

		private void NotifyConditionManagerMessageCompleted(ThrottlingContext context)
		{
			if (!this.throttlingEnabled)
			{
				return;
			}
			if (context == null)
			{
				return;
			}
			context.AddBreadcrumb(CategorizerBreadcrumb.NotifyFinished);
			this.conditionManager.MessageCompleted(context.CreationTime, context.Cost.Condition);
		}

		private const int MaxJobsPerThread = 1;

		private static readonly int maxExecutingJobs = Components.TransportAppConfig.Resolver.MaxExecutingJobs;

		private static readonly int maxJobThreads = Components.TransportAppConfig.Resolver.MaxJobThreads;

		private static readonly int maxJobsPerThread = Components.TransportAppConfig.Resolver.MaxJobsPerThread;

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.SchedulerTracer.Category, TransportEventLog.GetEventSource());

		private readonly IList<StageInfo> stages;

		private readonly JobHealthMonitor monitor;

		private readonly bool throttlingEnabled;

		private CatSchedulerJobList jobs;

		private int executingJobThreads;

		private SubmitMessageQueue submitMessageQueue;

		private WaitCallback jobThreadEntry;

		private volatile bool retired;

		private SingleQueueWaitConditionManager conditionManager;
	}
}
