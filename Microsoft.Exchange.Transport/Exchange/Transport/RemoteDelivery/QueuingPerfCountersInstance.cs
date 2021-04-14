using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal sealed class QueuingPerfCountersInstance : PerformanceCounterInstance
	{
		internal QueuingPerfCountersInstance(string instanceName, QueuingPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport Queues")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ExternalActiveRemoteDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "External Active Remote Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExternalActiveRemoteDeliveryQueueLength);
				this.InternalActiveRemoteDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Internal Active Remote Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InternalActiveRemoteDeliveryQueueLength);
				this.ExternalRetryRemoteDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "External Retry Remote Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExternalRetryRemoteDeliveryQueueLength);
				this.InternalRetryRemoteDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Internal Retry Remote Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InternalRetryRemoteDeliveryQueueLength);
				this.ActiveMailboxDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Active Mailbox Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMailboxDeliveryQueueLength);
				this.RetryMailboxDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Retry Mailbox Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RetryMailboxDeliveryQueueLength);
				this.ActiveNonSmtpDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Active Non-Smtp Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveNonSmtpDeliveryQueueLength);
				this.RetryNonSmtpDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Retry Non-Smtp Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RetryNonSmtpDeliveryQueueLength);
				this.InternalAggregateDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Internal Aggregate Delivery Queue Length (All Internal Queues)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InternalAggregateDeliveryQueueLength);
				this.ExternalAggregateDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "External Aggregate Delivery Queue Length (All External Queues)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExternalAggregateDeliveryQueueLength);
				this.InternalLargestDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Internal Largest Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InternalLargestDeliveryQueueLength);
				this.InternalLargestUnlockedDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Internal Largest Unlocked Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InternalLargestUnlockedDeliveryQueueLength);
				this.ExternalLargestDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "External Largest Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExternalLargestDeliveryQueueLength);
				this.ExternalLargestUnlockedDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "External Largest Unlocked Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExternalLargestUnlockedDeliveryQueueLength);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Items Queued for Delivery Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.ItemsQueuedForDeliveryTotal = new ExPerformanceCounter(base.CategoryName, "Items Queued For Delivery Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.ItemsQueuedForDeliveryTotal);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Items Completed Delivery Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.ItemsCompletedDeliveryTotal = new ExPerformanceCounter(base.CategoryName, "Items Completed Delivery Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.ItemsCompletedDeliveryTotal);
				this.ItemsQueuedForDeliveryExpiredTotal = new ExPerformanceCounter(base.CategoryName, "Items Queued For Delivery Expired Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ItemsQueuedForDeliveryExpiredTotal);
				this.LocksExpiredInDeliveryTotal = new ExPerformanceCounter(base.CategoryName, "Locks Expired In Delivery Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LocksExpiredInDeliveryTotal);
				this.ItemsDeletedByAdminTotal = new ExPerformanceCounter(base.CategoryName, "Items Deleted By Admin Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ItemsDeletedByAdminTotal);
				this.ItemsResubmittedTotal = new ExPerformanceCounter(base.CategoryName, "Items Resubmitted Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ItemsResubmittedTotal);
				this.MessagesQueuedForDelivery = new ExPerformanceCounter(base.CategoryName, "Messages Queued For Delivery", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesQueuedForDelivery);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Messages Queued for Delivery Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.MessagesQueuedForDeliveryTotal = new ExPerformanceCounter(base.CategoryName, "Messages Queued For Delivery Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.MessagesQueuedForDeliveryTotal);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Messages Completed Delivery Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.MessagesCompletedDeliveryTotal = new ExPerformanceCounter(base.CategoryName, "Messages Completed Delivery Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.MessagesCompletedDeliveryTotal);
				this.UnreachableQueueLength = new ExPerformanceCounter(base.CategoryName, "Unreachable Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UnreachableQueueLength);
				this.PoisonQueueLength = new ExPerformanceCounter(base.CategoryName, "Poison Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.PoisonQueueLength);
				this.SubmissionQueueLength = new ExPerformanceCounter(base.CategoryName, "Submission Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SubmissionQueueLength);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Messages Submitted Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.MessagesSubmittedTotal = new ExPerformanceCounter(base.CategoryName, "Messages Submitted Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.MessagesSubmittedTotal);
				this.MessagesSubmittedRecently = new ExPerformanceCounter(base.CategoryName, "Messages Submitted Recently", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesSubmittedRecently);
				this.SubmissionQueueItemsExpiredTotal = new ExPerformanceCounter(base.CategoryName, "Submission Queue Items Expired Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SubmissionQueueItemsExpiredTotal);
				this.SubmissionQueueLocksExpiredTotal = new ExPerformanceCounter(base.CategoryName, "Submission Queue Locks Expired Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SubmissionQueueLocksExpiredTotal);
				this.AggregateShadowQueueLength = new ExPerformanceCounter(base.CategoryName, "Aggregate Shadow Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AggregateShadowQueueLength);
				this.ShadowQueueAutoDiscardsTotal = new ExPerformanceCounter(base.CategoryName, "Shadow Queue Auto Discards Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowQueueAutoDiscardsTotal);
				this.MessagesCompletingCategorization = new ExPerformanceCounter(base.CategoryName, "Messages Completing Categorization", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesCompletingCategorization);
				this.MessagesDeferredDuringCategorization = new ExPerformanceCounter(base.CategoryName, "Messages Deferred during Categorization", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDeferredDuringCategorization);
				this.MessagesResubmittedDuringCategorization = new ExPerformanceCounter(base.CategoryName, "Messages Resubmitted during Categorization", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesResubmittedDuringCategorization);
				this.CategorizerJobAvailability = new ExPerformanceCounter(base.CategoryName, "Categorizer Job Availability", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CategorizerJobAvailability);
				long num = this.ExternalActiveRemoteDeliveryQueueLength.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter6 in list)
					{
						exPerformanceCounter6.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal QueuingPerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport Queues")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ExternalActiveRemoteDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "External Active Remote Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExternalActiveRemoteDeliveryQueueLength);
				this.InternalActiveRemoteDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Internal Active Remote Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InternalActiveRemoteDeliveryQueueLength);
				this.ExternalRetryRemoteDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "External Retry Remote Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExternalRetryRemoteDeliveryQueueLength);
				this.InternalRetryRemoteDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Internal Retry Remote Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InternalRetryRemoteDeliveryQueueLength);
				this.ActiveMailboxDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Active Mailbox Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMailboxDeliveryQueueLength);
				this.RetryMailboxDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Retry Mailbox Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RetryMailboxDeliveryQueueLength);
				this.ActiveNonSmtpDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Active Non-Smtp Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveNonSmtpDeliveryQueueLength);
				this.RetryNonSmtpDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Retry Non-Smtp Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RetryNonSmtpDeliveryQueueLength);
				this.InternalAggregateDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Internal Aggregate Delivery Queue Length (All Internal Queues)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InternalAggregateDeliveryQueueLength);
				this.ExternalAggregateDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "External Aggregate Delivery Queue Length (All External Queues)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExternalAggregateDeliveryQueueLength);
				this.InternalLargestDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Internal Largest Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InternalLargestDeliveryQueueLength);
				this.InternalLargestUnlockedDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "Internal Largest Unlocked Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InternalLargestUnlockedDeliveryQueueLength);
				this.ExternalLargestDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "External Largest Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExternalLargestDeliveryQueueLength);
				this.ExternalLargestUnlockedDeliveryQueueLength = new ExPerformanceCounter(base.CategoryName, "External Largest Unlocked Delivery Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ExternalLargestUnlockedDeliveryQueueLength);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Items Queued for Delivery Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.ItemsQueuedForDeliveryTotal = new ExPerformanceCounter(base.CategoryName, "Items Queued For Delivery Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.ItemsQueuedForDeliveryTotal);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Items Completed Delivery Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.ItemsCompletedDeliveryTotal = new ExPerformanceCounter(base.CategoryName, "Items Completed Delivery Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.ItemsCompletedDeliveryTotal);
				this.ItemsQueuedForDeliveryExpiredTotal = new ExPerformanceCounter(base.CategoryName, "Items Queued For Delivery Expired Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ItemsQueuedForDeliveryExpiredTotal);
				this.LocksExpiredInDeliveryTotal = new ExPerformanceCounter(base.CategoryName, "Locks Expired In Delivery Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LocksExpiredInDeliveryTotal);
				this.ItemsDeletedByAdminTotal = new ExPerformanceCounter(base.CategoryName, "Items Deleted By Admin Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ItemsDeletedByAdminTotal);
				this.ItemsResubmittedTotal = new ExPerformanceCounter(base.CategoryName, "Items Resubmitted Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ItemsResubmittedTotal);
				this.MessagesQueuedForDelivery = new ExPerformanceCounter(base.CategoryName, "Messages Queued For Delivery", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesQueuedForDelivery);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Messages Queued for Delivery Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.MessagesQueuedForDeliveryTotal = new ExPerformanceCounter(base.CategoryName, "Messages Queued For Delivery Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.MessagesQueuedForDeliveryTotal);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Messages Completed Delivery Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.MessagesCompletedDeliveryTotal = new ExPerformanceCounter(base.CategoryName, "Messages Completed Delivery Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.MessagesCompletedDeliveryTotal);
				this.UnreachableQueueLength = new ExPerformanceCounter(base.CategoryName, "Unreachable Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UnreachableQueueLength);
				this.PoisonQueueLength = new ExPerformanceCounter(base.CategoryName, "Poison Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.PoisonQueueLength);
				this.SubmissionQueueLength = new ExPerformanceCounter(base.CategoryName, "Submission Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SubmissionQueueLength);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Messages Submitted Per Second", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.MessagesSubmittedTotal = new ExPerformanceCounter(base.CategoryName, "Messages Submitted Total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.MessagesSubmittedTotal);
				this.MessagesSubmittedRecently = new ExPerformanceCounter(base.CategoryName, "Messages Submitted Recently", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesSubmittedRecently);
				this.SubmissionQueueItemsExpiredTotal = new ExPerformanceCounter(base.CategoryName, "Submission Queue Items Expired Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SubmissionQueueItemsExpiredTotal);
				this.SubmissionQueueLocksExpiredTotal = new ExPerformanceCounter(base.CategoryName, "Submission Queue Locks Expired Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SubmissionQueueLocksExpiredTotal);
				this.AggregateShadowQueueLength = new ExPerformanceCounter(base.CategoryName, "Aggregate Shadow Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AggregateShadowQueueLength);
				this.ShadowQueueAutoDiscardsTotal = new ExPerformanceCounter(base.CategoryName, "Shadow Queue Auto Discards Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ShadowQueueAutoDiscardsTotal);
				this.MessagesCompletingCategorization = new ExPerformanceCounter(base.CategoryName, "Messages Completing Categorization", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesCompletingCategorization);
				this.MessagesDeferredDuringCategorization = new ExPerformanceCounter(base.CategoryName, "Messages Deferred during Categorization", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesDeferredDuringCategorization);
				this.MessagesResubmittedDuringCategorization = new ExPerformanceCounter(base.CategoryName, "Messages Resubmitted during Categorization", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesResubmittedDuringCategorization);
				this.CategorizerJobAvailability = new ExPerformanceCounter(base.CategoryName, "Categorizer Job Availability", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CategorizerJobAvailability);
				long num = this.ExternalActiveRemoteDeliveryQueueLength.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter6 in list)
					{
						exPerformanceCounter6.Close();
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

		public readonly ExPerformanceCounter ExternalActiveRemoteDeliveryQueueLength;

		public readonly ExPerformanceCounter InternalActiveRemoteDeliveryQueueLength;

		public readonly ExPerformanceCounter ExternalRetryRemoteDeliveryQueueLength;

		public readonly ExPerformanceCounter InternalRetryRemoteDeliveryQueueLength;

		public readonly ExPerformanceCounter ActiveMailboxDeliveryQueueLength;

		public readonly ExPerformanceCounter RetryMailboxDeliveryQueueLength;

		public readonly ExPerformanceCounter ActiveNonSmtpDeliveryQueueLength;

		public readonly ExPerformanceCounter RetryNonSmtpDeliveryQueueLength;

		public readonly ExPerformanceCounter InternalAggregateDeliveryQueueLength;

		public readonly ExPerformanceCounter ExternalAggregateDeliveryQueueLength;

		public readonly ExPerformanceCounter InternalLargestDeliveryQueueLength;

		public readonly ExPerformanceCounter InternalLargestUnlockedDeliveryQueueLength;

		public readonly ExPerformanceCounter ExternalLargestDeliveryQueueLength;

		public readonly ExPerformanceCounter ExternalLargestUnlockedDeliveryQueueLength;

		public readonly ExPerformanceCounter ItemsQueuedForDeliveryTotal;

		public readonly ExPerformanceCounter ItemsCompletedDeliveryTotal;

		public readonly ExPerformanceCounter ItemsQueuedForDeliveryExpiredTotal;

		public readonly ExPerformanceCounter LocksExpiredInDeliveryTotal;

		public readonly ExPerformanceCounter ItemsDeletedByAdminTotal;

		public readonly ExPerformanceCounter ItemsResubmittedTotal;

		public readonly ExPerformanceCounter MessagesQueuedForDelivery;

		public readonly ExPerformanceCounter MessagesQueuedForDeliveryTotal;

		public readonly ExPerformanceCounter MessagesCompletedDeliveryTotal;

		public readonly ExPerformanceCounter UnreachableQueueLength;

		public readonly ExPerformanceCounter PoisonQueueLength;

		public readonly ExPerformanceCounter SubmissionQueueLength;

		public readonly ExPerformanceCounter MessagesSubmittedTotal;

		public readonly ExPerformanceCounter MessagesSubmittedRecently;

		public readonly ExPerformanceCounter SubmissionQueueItemsExpiredTotal;

		public readonly ExPerformanceCounter SubmissionQueueLocksExpiredTotal;

		public readonly ExPerformanceCounter AggregateShadowQueueLength;

		public readonly ExPerformanceCounter ShadowQueueAutoDiscardsTotal;

		public readonly ExPerformanceCounter MessagesCompletingCategorization;

		public readonly ExPerformanceCounter MessagesDeferredDuringCategorization;

		public readonly ExPerformanceCounter MessagesResubmittedDuringCategorization;

		public readonly ExPerformanceCounter CategorizerJobAvailability;
	}
}
