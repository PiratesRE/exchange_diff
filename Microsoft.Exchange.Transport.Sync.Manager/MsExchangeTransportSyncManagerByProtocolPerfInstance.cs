using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MsExchangeTransportSyncManagerByProtocolPerfInstance : PerformanceCounterInstance
	{
		internal MsExchangeTransportSyncManagerByProtocolPerfInstance(string instanceName, MsExchangeTransportSyncManagerByProtocolPerfInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Transport Sync Manager By Protocol")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageDispatchTime = new ExPerformanceCounter(base.CategoryName, "Average Time to Complete Dispatch (sec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageDispatchTime);
				this.AverageDispatchTimeBase = new ExPerformanceCounter(base.CategoryName, "Average Time to Complete Dispatch Base (sec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageDispatchTimeBase);
				this.LastDispatchTime = new ExPerformanceCounter(base.CategoryName, "Time to Complete Last Dispatch (msec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LastDispatchTime);
				this.LastSubscriptionProcessingTime = new ExPerformanceCounter(base.CategoryName, "Last Subscription Dispatch to Completion Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LastSubscriptionProcessingTime);
				this.ProcessingTimeToSyncSubscription95Percent = new ExPerformanceCounter(base.CategoryName, "95 Percentile Processing Time to Sync a Subscription (secs)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingTimeToSyncSubscription95Percent);
				this.SubscriptionsCompletingSync = new ExPerformanceCounter(base.CategoryName, "Subscriptions Completing Sync", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SubscriptionsCompletingSync);
				this.SubscriptionsQueued = new ExPerformanceCounter(base.CategoryName, "Dispatch Queue - Total Subscriptions", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SubscriptionsQueued);
				this.SyncNowSubscriptionsQueued = new ExPerformanceCounter(base.CategoryName, "Dispatch Queue - Total Sync Now Subscriptions", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SyncNowSubscriptionsQueued);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Subscription Dispatch - Total Attempts per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.SubscriptionsDispatched = new ExPerformanceCounter(base.CategoryName, "Subscription Dispatch - Total Attempts", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.SubscriptionsDispatched);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Subscription Dispatch - Total Successful Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.SuccessfulSubmissions = new ExPerformanceCounter(base.CategoryName, "Subscription Dispatch - Total Successful", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.SuccessfulSubmissions);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Subscription Dispatch - Total Duplicate Attempts Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.DuplicateSubmissions = new ExPerformanceCounter(base.CategoryName, "Subscription Dispatch - Total Duplicate Attempts", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.DuplicateSubmissions);
				this.TemporarySubmissionFailures = new ExPerformanceCounter(base.CategoryName, "Subscription Dispatch - Total Transient Failures", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TemporarySubmissionFailures);
				long num = this.AverageDispatchTime.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter4 in list)
					{
						exPerformanceCounter4.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal MsExchangeTransportSyncManagerByProtocolPerfInstance(string instanceName) : base(instanceName, "MSExchange Transport Sync Manager By Protocol")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.AverageDispatchTime = new ExPerformanceCounter(base.CategoryName, "Average Time to Complete Dispatch (sec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageDispatchTime);
				this.AverageDispatchTimeBase = new ExPerformanceCounter(base.CategoryName, "Average Time to Complete Dispatch Base (sec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageDispatchTimeBase);
				this.LastDispatchTime = new ExPerformanceCounter(base.CategoryName, "Time to Complete Last Dispatch (msec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LastDispatchTime);
				this.LastSubscriptionProcessingTime = new ExPerformanceCounter(base.CategoryName, "Last Subscription Dispatch to Completion Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.LastSubscriptionProcessingTime);
				this.ProcessingTimeToSyncSubscription95Percent = new ExPerformanceCounter(base.CategoryName, "95 Percentile Processing Time to Sync a Subscription (secs)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.ProcessingTimeToSyncSubscription95Percent);
				this.SubscriptionsCompletingSync = new ExPerformanceCounter(base.CategoryName, "Subscriptions Completing Sync", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SubscriptionsCompletingSync);
				this.SubscriptionsQueued = new ExPerformanceCounter(base.CategoryName, "Dispatch Queue - Total Subscriptions", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SubscriptionsQueued);
				this.SyncNowSubscriptionsQueued = new ExPerformanceCounter(base.CategoryName, "Dispatch Queue - Total Sync Now Subscriptions", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SyncNowSubscriptionsQueued);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Subscription Dispatch - Total Attempts per second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.SubscriptionsDispatched = new ExPerformanceCounter(base.CategoryName, "Subscription Dispatch - Total Attempts", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.SubscriptionsDispatched);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Subscription Dispatch - Total Successful Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.SuccessfulSubmissions = new ExPerformanceCounter(base.CategoryName, "Subscription Dispatch - Total Successful", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.SuccessfulSubmissions);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Subscription Dispatch - Total Duplicate Attempts Per Second", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.DuplicateSubmissions = new ExPerformanceCounter(base.CategoryName, "Subscription Dispatch - Total Duplicate Attempts", instanceName, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.DuplicateSubmissions);
				this.TemporarySubmissionFailures = new ExPerformanceCounter(base.CategoryName, "Subscription Dispatch - Total Transient Failures", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TemporarySubmissionFailures);
				long num = this.AverageDispatchTime.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter4 in list)
					{
						exPerformanceCounter4.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter AverageDispatchTime;

		public readonly ExPerformanceCounter AverageDispatchTimeBase;

		public readonly ExPerformanceCounter LastDispatchTime;

		public readonly ExPerformanceCounter LastSubscriptionProcessingTime;

		public readonly ExPerformanceCounter ProcessingTimeToSyncSubscription95Percent;

		public readonly ExPerformanceCounter SubscriptionsCompletingSync;

		public readonly ExPerformanceCounter SubscriptionsQueued;

		public readonly ExPerformanceCounter SyncNowSubscriptionsQueued;

		public readonly ExPerformanceCounter SubscriptionsDispatched;

		public readonly ExPerformanceCounter SuccessfulSubmissions;

		public readonly ExPerformanceCounter DuplicateSubmissions;

		public readonly ExPerformanceCounter TemporarySubmissionFailures;
	}
}
