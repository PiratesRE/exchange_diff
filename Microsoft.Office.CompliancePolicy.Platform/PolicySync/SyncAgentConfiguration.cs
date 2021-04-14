using System;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[Serializable]
	public class SyncAgentConfiguration
	{
		public SyncAgentConfiguration(int maxOperationalRetryTimes, Workload workload, bool asyncCallSyncSvc, int ioBatchSize, int maxSyncWorkItemsPerJob, int maxPublishWorkItemsPerJob, TimeSpan jobDispatcherWaitIntervalWhenStarve, int parallelJobDispatcherMaxPendingJobNumber, int maxQueueLength, bool reEnqueueNonSuccessWorkItem, TimeSpan? dispatcherTriggerInterval, string certificateSubject, string partnerName, bool enablePolicyApplication, TimeSpan workItemExecuteDelayTime, TimeSpan policySyncSLA, bool enableMonitor, string workItemRetryStrategy = null)
		{
			ArgumentValidator.ThrowIfNegative("maxOperationalRetryTimes", maxOperationalRetryTimes);
			ArgumentValidator.ThrowIfZeroOrNegative("ioBatchSize", ioBatchSize);
			ArgumentValidator.ThrowIfZeroOrNegative("maxSyncWorkItemsPerJob", maxSyncWorkItemsPerJob);
			ArgumentValidator.ThrowIfZeroOrNegative("maxPublishWorkItemsPerJob", maxPublishWorkItemsPerJob);
			ArgumentValidator.ThrowIfNegativeTimeSpan("jobDispatcherWaitIntervalWhenStarve", jobDispatcherWaitIntervalWhenStarve);
			ArgumentValidator.ThrowIfZeroOrNegative("parallelJobDispatcherMaxPendingJobNumber", parallelJobDispatcherMaxPendingJobNumber);
			ArgumentValidator.ThrowIfZeroOrNegative("maxQueueLength", maxQueueLength);
			ArgumentValidator.ThrowIfNegativeTimeSpan("workItemExecuteDelayTime", workItemExecuteDelayTime);
			ArgumentValidator.ThrowIfNegativeTimeSpan("policySyncSLA", policySyncSLA);
			if (dispatcherTriggerInterval != null)
			{
				ArgumentValidator.ThrowIfNegativeTimeSpan("dispatcherTriggerInterval", dispatcherTriggerInterval.Value);
			}
			this.MaxOperationalRetryTimes = maxOperationalRetryTimes;
			this.WorkLoad = workload;
			this.AsyncCallSyncSvc = asyncCallSyncSvc;
			this.IoBatchSize = ioBatchSize;
			this.MaxSyncWorkItemsPerJob = maxSyncWorkItemsPerJob;
			this.MaxPublishWorkItemsPerJob = maxPublishWorkItemsPerJob;
			this.JobDispatcherWaitIntervalWhenStarve = jobDispatcherWaitIntervalWhenStarve;
			this.ParallelJobDispatcherMaxPendingJobNumber = parallelJobDispatcherMaxPendingJobNumber;
			this.MaxQueueLength = maxQueueLength;
			this.ReEnqueueNonSuccessWorkItem = reEnqueueNonSuccessWorkItem;
			this.DispatcherTriggerInterval = dispatcherTriggerInterval;
			this.CertificateSubject = certificateSubject;
			this.PartnerName = partnerName;
			this.EnablePolicyApplication = enablePolicyApplication;
			this.strWorkItemRetryStrategy = workItemRetryStrategy;
			this.WorkItemExecuteDelayTime = workItemExecuteDelayTime;
			this.PolicySyncSLA = policySyncSLA;
			this.EnableMonitor = enableMonitor;
		}

		protected SyncAgentConfiguration()
		{
		}

		public int MaxOperationalRetryTimes { get; protected set; }

		public Workload WorkLoad { get; protected set; }

		public bool AsyncCallSyncSvc { get; protected set; }

		public int IoBatchSize { get; protected set; }

		public int MaxSyncWorkItemsPerJob { get; protected set; }

		public int MaxPublishWorkItemsPerJob { get; protected set; }

		public string CertificateSubject { get; protected set; }

		public string PartnerName { get; protected set; }

		public TimeSpan JobDispatcherWaitIntervalWhenStarve { get; protected set; }

		public int ParallelJobDispatcherMaxPendingJobNumber { get; protected set; }

		public int MaxQueueLength { get; protected set; }

		public bool ReEnqueueNonSuccessWorkItem { get; protected set; }

		public TimeSpan? DispatcherTriggerInterval { get; protected set; }

		public TimeSpan WorkItemExecuteDelayTime { get; protected set; }

		public bool EnablePolicyApplication { get; protected set; }

		public TimeSpan PolicySyncSLA { get; protected set; }

		public bool EnableMonitor { get; protected set; }

		public RetryStrategy RetryStrategy
		{
			get
			{
				if (this.omWorkItemRetryStrategy == null)
				{
					this.omWorkItemRetryStrategy = new RetryStrategy(this.strWorkItemRetryStrategy);
				}
				return this.omWorkItemRetryStrategy;
			}
		}

		protected string WorkItemRetryStrategy
		{
			set
			{
				this.strWorkItemRetryStrategy = value;
			}
		}

		private RetryStrategy omWorkItemRetryStrategy;

		private string strWorkItemRetryStrategy;
	}
}
