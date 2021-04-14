using System;
using Microsoft.Exchange.Common;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Servicelets.UnifiedPolicySync
{
	public sealed class ExSyncAgentConfiguration : SyncAgentConfiguration
	{
		public ExSyncAgentConfiguration()
		{
			base.MaxOperationalRetryTimes = AppConfigLoader.GetConfigIntValue("MaxOperationalRetryTimes", 1, 10, 3);
			base.WorkLoad = AppConfigLoader.GetConfigEnumValue<Workload>("Workload", Workload.Exchange);
			base.AsyncCallSyncSvc = AppConfigLoader.GetConfigBoolValue("AsyncCallSyncSvc", true);
			base.IoBatchSize = AppConfigLoader.GetConfigIntValue("IoBatchSize", 1, int.MaxValue, 10);
			base.MaxSyncWorkItemsPerJob = AppConfigLoader.GetConfigIntValue("MaxSyncWorkItemsPerJob", 1, int.MaxValue, 1);
			base.MaxPublishWorkItemsPerJob = AppConfigLoader.GetConfigIntValue("MaxPublishWorkItemsPerJob", 1, int.MaxValue, 1);
			base.JobDispatcherWaitIntervalWhenStarve = AppConfigLoader.GetConfigTimeSpanValue("JobDispatcherWaitIntervalWhenStarve", TimeSpan.FromSeconds(1.0), TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(15.0));
			base.ParallelJobDispatcherMaxPendingJobNumber = AppConfigLoader.GetConfigIntValue("ParallelJobDispatcherMaxPendingJobNumber", 1, 20, 5);
			base.MaxQueueLength = AppConfigLoader.GetConfigIntValue("MaxQueueLength", 1, 5000, 1000);
			base.ReEnqueueNonSuccessWorkItem = AppConfigLoader.GetConfigBoolValue("ReEnqueueNonSuccessWorkItem", true);
			base.DispatcherTriggerInterval = new TimeSpan?(AppConfigLoader.GetConfigTimeSpanValue("DispatcherTriggerInterval", TimeSpan.FromMilliseconds(100.0), TimeSpan.FromMinutes(60.0), TimeSpan.FromSeconds(30.0)));
			base.CertificateSubject = AppConfigLoader.GetConfigStringValue("CertificateSubject", null);
			base.PartnerName = AppConfigLoader.GetConfigStringValue("PartnerName", "ExoUnifiedPolicySyncAgentCallerId");
			base.EnablePolicyApplication = AppConfigLoader.GetConfigBoolValue("EnablePolicyApplication", true);
			this.DelayStartInSeconds = AppConfigLoader.GetConfigIntValue("DelayStartInSeconds", 0, 60, 0);
			this.NotifyRequestTimeout = AppConfigLoader.GetConfigTimeSpanValue("NotifyRequestTimeout", TimeSpan.FromSeconds(5.0), TimeSpan.MaxValue, TimeSpan.FromSeconds(300.0));
			base.WorkItemExecuteDelayTime = AppConfigLoader.GetConfigTimeSpanValue("WorkItemExecuteDelayTime", TimeSpan.FromMilliseconds(100.0), TimeSpan.FromMinutes(60.0), TimeSpan.FromSeconds(30.0));
			base.PolicySyncSLA = AppConfigLoader.GetConfigTimeSpanValue("PolicySyncSLA", TimeSpan.FromMilliseconds(100.0), TimeSpan.FromDays(1.0), TimeSpan.FromMinutes(5.0));
			base.EnableMonitor = AppConfigLoader.GetConfigBoolValue("EnableMonitor", true);
			base.WorkItemRetryStrategy = AppConfigLoader.GetConfigStringValue("WorkItemRetryStrategy", string.Empty);
		}

		public int DelayStartInSeconds { get; private set; }

		public TimeSpan NotifyRequestTimeout { get; private set; }
	}
}
