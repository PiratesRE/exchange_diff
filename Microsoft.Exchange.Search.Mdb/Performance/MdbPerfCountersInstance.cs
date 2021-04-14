using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Search.Performance
{
	internal sealed class MdbPerfCountersInstance : PerformanceCounterInstance
	{
		internal MdbPerfCountersInstance(string instanceName, MdbPerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Search Indexes")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Notifications: Processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.NumberOfNotifications = new ExPerformanceCounter(base.CategoryName, "Notifications: Processed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfNotifications, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.NumberOfNotifications);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Notifications: Updates Processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.NumberOfUpdateNotifications = new ExPerformanceCounter(base.CategoryName, "Notifications: Updates Processed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfUpdateNotifications, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.NumberOfUpdateNotifications);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Notifications: Creates Processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.NumberOfCreateNotifications = new ExPerformanceCounter(base.CategoryName, "Notifications: Creates Processed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfCreateNotifications, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.NumberOfCreateNotifications);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Notifications: Deletes Processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.NumberOfDeleteNotifications = new ExPerformanceCounter(base.CategoryName, "Notifications: Deletes Processed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfDeleteNotifications, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.NumberOfDeleteNotifications);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Notifications: Moves Processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.NumberOfMoveNotifications = new ExPerformanceCounter(base.CategoryName, "Notifications: Moves Processed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfMoveNotifications, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.NumberOfMoveNotifications);
				this.MessagesFromTransport = new ExPerformanceCounter(base.CategoryName, "Source Transport", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesFromTransport, new ExPerformanceCounter[0]);
				list.Add(this.MessagesFromTransport);
				this.MessagesFromEventBasedAssistants = new ExPerformanceCounter(base.CategoryName, "Source Event Based Assistants", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesFromEventBasedAssistants, new ExPerformanceCounter[0]);
				list.Add(this.MessagesFromEventBasedAssistants);
				this.MessagesFromTimeBasedAssistants = new ExPerformanceCounter(base.CategoryName, "Source Time Based Assistants", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesFromTimeBasedAssistants, new ExPerformanceCounter[0]);
				list.Add(this.MessagesFromTimeBasedAssistants);
				this.MessagesFromMigration = new ExPerformanceCounter(base.CategoryName, "Source Migration", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MessagesFromMigration, new ExPerformanceCounter[0]);
				list.Add(this.MessagesFromMigration);
				this.NumberOfItemsInNotificationQueue = new ExPerformanceCounter(base.CategoryName, "Notifications: Queue Length", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfItemsInNotificationQueue, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsInNotificationQueue);
				this.NumberOfBackloggedItemsAddedToRetryTable = new ExPerformanceCounter(base.CategoryName, "Notifications: Delayed Items", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfBackloggedItemsAddedToRetryTable, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfBackloggedItemsAddedToRetryTable);
				this.AgeOfLastNotificationProcessed = new ExPerformanceCounter(base.CategoryName, "Notifications: Age of Last Notification Processed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AgeOfLastNotificationProcessed, new ExPerformanceCounter[0]);
				list.Add(this.AgeOfLastNotificationProcessed);
				this.NumberOfNotificationsNotYetProcessed = new ExPerformanceCounter(base.CategoryName, "Notifications: Awaiting Processing", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfNotificationsNotYetProcessed, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfNotificationsNotYetProcessed);
				this.NumberOfDocumentsSentForProcessingNotifications = new ExPerformanceCounter(base.CategoryName, "Notifications: Items Sent for Processing", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfDocumentsSentForProcessingNotifications, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsSentForProcessingNotifications);
				this.NumberOfDocumentsIndexedNotifications = new ExPerformanceCounter(base.CategoryName, "Notifications: Items Processed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfDocumentsIndexedNotifications, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsIndexedNotifications);
				this.LastSuccessfulPollTimestamp = new ExPerformanceCounter(base.CategoryName, "Notifications: Last Successful Poll Timestamp", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LastSuccessfulPollTimestamp, new ExPerformanceCounter[0]);
				list.Add(this.LastSuccessfulPollTimestamp);
				this.NotificationsStallTime = new ExPerformanceCounter(base.CategoryName, "Notifications: Stall Time", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NotificationsStallTime, new ExPerformanceCounter[0]);
				list.Add(this.NotificationsStallTime);
				this.MailboxesLeftToCrawl = new ExPerformanceCounter(base.CategoryName, "Crawler: Mailboxes Remaining", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MailboxesLeftToCrawl, new ExPerformanceCounter[0]);
				list.Add(this.MailboxesLeftToCrawl);
				this.MailboxesLeftToRecrawl = new ExPerformanceCounter(base.CategoryName, "Crawler: Mailboxes Remaining to Recrawl", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MailboxesLeftToRecrawl, new ExPerformanceCounter[0]);
				list.Add(this.MailboxesLeftToRecrawl);
				this.NumberOfDocumentsSentForProcessingCrawler = new ExPerformanceCounter(base.CategoryName, "Crawler: Items Sent for Processing", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfDocumentsSentForProcessingCrawler, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsSentForProcessingCrawler);
				this.NumberOfDocumentsIndexedCrawler = new ExPerformanceCounter(base.CategoryName, "Crawler: Items Processed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfDocumentsIndexedCrawler, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsIndexedCrawler);
				this.AverageAttemptedCrawlerRate = new ExPerformanceCounter(base.CategoryName, "Crawler: Average Rate of Attempted Items Submission", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageAttemptedCrawlerRate, new ExPerformanceCounter[0]);
				list.Add(this.AverageAttemptedCrawlerRate);
				this.AverageCrawlerRate = new ExPerformanceCounter(base.CategoryName, "Crawler: Average Rate of Items Submission", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.AverageCrawlerRate, new ExPerformanceCounter[0]);
				list.Add(this.AverageCrawlerRate);
				this.FailedItemsCount = new ExPerformanceCounter(base.CategoryName, "Failed Items", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.FailedItemsCount, new ExPerformanceCounter[0]);
				list.Add(this.FailedItemsCount);
				this.RetriableItemsCount = new ExPerformanceCounter(base.CategoryName, "Retry: Retriable Items", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RetriableItemsCount, new ExPerformanceCounter[0]);
				list.Add(this.RetriableItemsCount);
				this.NumberOfDocumentsSentForProcessingRetry = new ExPerformanceCounter(base.CategoryName, "Retry: Items Sent for Processing", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfDocumentsSentForProcessingRetry, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsSentForProcessingRetry);
				this.NumberOfDocumentsIndexedRetry = new ExPerformanceCounter(base.CategoryName, "Retry: Items Processed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfDocumentsIndexedRetry, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsIndexedRetry);
				this.SubmissionDelaysRetry = new ExPerformanceCounter(base.CategoryName, "Retry: Submission Delays", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.SubmissionDelaysRetry, new ExPerformanceCounter[0]);
				list.Add(this.SubmissionDelaysRetry);
				this.DelayTimeRetry = new ExPerformanceCounter(base.CategoryName, "Retry: Submission Delay Time", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DelayTimeRetry, new ExPerformanceCounter[0]);
				list.Add(this.DelayTimeRetry);
				this.MailboxesLeftToDelete = new ExPerformanceCounter(base.CategoryName, "Retry: Deleted Mailboxes Remaining", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MailboxesLeftToDelete, new ExPerformanceCounter[0]);
				list.Add(this.MailboxesLeftToDelete);
				this.NumberOfDocumentsSentForDeletion = new ExPerformanceCounter(base.CategoryName, "Retry: Items Sent for Deletion", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfDocumentsSentForDeletion, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsSentForDeletion);
				this.NumberOfDocumentsDeleted = new ExPerformanceCounter(base.CategoryName, "Retry: Items Deleted", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfDocumentsDeleted, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsDeleted);
				this.TotalDocumentsFirstRetryAttempt = new ExPerformanceCounter(base.CategoryName, "Retry: Documents With One Retry Attempt", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalDocumentsFirstRetryAttempt, new ExPerformanceCounter[0]);
				list.Add(this.TotalDocumentsFirstRetryAttempt);
				this.TotalDocumentsMutlipleRetryAttempts = new ExPerformanceCounter(base.CategoryName, "Retry: Documents With Multiple Retries", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TotalDocumentsMutlipleRetryAttempts, new ExPerformanceCounter[0]);
				list.Add(this.TotalDocumentsMutlipleRetryAttempts);
				this.IndexingStatus = new ExPerformanceCounter(base.CategoryName, "Indexing Status", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.IndexingStatus, new ExPerformanceCounter[0]);
				list.Add(this.IndexingStatus);
				this.FeedingSessions = new ExPerformanceCounter(base.CategoryName, "Feeding Sessions", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.FeedingSessions, new ExPerformanceCounter[0]);
				list.Add(this.FeedingSessions);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Items Processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				this.NumberOfDocumentsProcessed = new ExPerformanceCounter(base.CategoryName, "Items Processed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfDocumentsProcessed, new ExPerformanceCounter[]
				{
					exPerformanceCounter6
				});
				list.Add(this.NumberOfDocumentsProcessed);
				this.NumberOfItems = new ExPerformanceCounter(base.CategoryName, "Number Of Items", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NumberOfItems, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItems);
				this.InstantSearchEnabled = new ExPerformanceCounter(base.CategoryName, "Instant Search Enabled", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.InstantSearchEnabled, new ExPerformanceCounter[0]);
				list.Add(this.InstantSearchEnabled);
				this.CatalogSuspended = new ExPerformanceCounter(base.CategoryName, "Catalog Suspended", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.CatalogSuspended, new ExPerformanceCounter[0]);
				list.Add(this.CatalogSuspended);
				long num = this.NumberOfNotifications.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter7 in list)
					{
						exPerformanceCounter7.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal MdbPerfCountersInstance(string instanceName) : base(instanceName, "MSExchange Search Indexes")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Notifications: Processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.NumberOfNotifications = new ExPerformanceCounter(base.CategoryName, "Notifications: Processed", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.NumberOfNotifications);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Notifications: Updates Processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.NumberOfUpdateNotifications = new ExPerformanceCounter(base.CategoryName, "Notifications: Updates Processed", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.NumberOfUpdateNotifications);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Notifications: Creates Processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.NumberOfCreateNotifications = new ExPerformanceCounter(base.CategoryName, "Notifications: Creates Processed", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.NumberOfCreateNotifications);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Notifications: Deletes Processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.NumberOfDeleteNotifications = new ExPerformanceCounter(base.CategoryName, "Notifications: Deletes Processed", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.NumberOfDeleteNotifications);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Notifications: Moves Processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.NumberOfMoveNotifications = new ExPerformanceCounter(base.CategoryName, "Notifications: Moves Processed", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.NumberOfMoveNotifications);
				this.MessagesFromTransport = new ExPerformanceCounter(base.CategoryName, "Source Transport", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesFromTransport);
				this.MessagesFromEventBasedAssistants = new ExPerformanceCounter(base.CategoryName, "Source Event Based Assistants", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesFromEventBasedAssistants);
				this.MessagesFromTimeBasedAssistants = new ExPerformanceCounter(base.CategoryName, "Source Time Based Assistants", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesFromTimeBasedAssistants);
				this.MessagesFromMigration = new ExPerformanceCounter(base.CategoryName, "Source Migration", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MessagesFromMigration);
				this.NumberOfItemsInNotificationQueue = new ExPerformanceCounter(base.CategoryName, "Notifications: Queue Length", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItemsInNotificationQueue);
				this.NumberOfBackloggedItemsAddedToRetryTable = new ExPerformanceCounter(base.CategoryName, "Notifications: Delayed Items", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfBackloggedItemsAddedToRetryTable);
				this.AgeOfLastNotificationProcessed = new ExPerformanceCounter(base.CategoryName, "Notifications: Age of Last Notification Processed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AgeOfLastNotificationProcessed);
				this.NumberOfNotificationsNotYetProcessed = new ExPerformanceCounter(base.CategoryName, "Notifications: Awaiting Processing", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfNotificationsNotYetProcessed);
				this.NumberOfDocumentsSentForProcessingNotifications = new ExPerformanceCounter(base.CategoryName, "Notifications: Items Sent for Processing", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsSentForProcessingNotifications);
				this.NumberOfDocumentsIndexedNotifications = new ExPerformanceCounter(base.CategoryName, "Notifications: Items Processed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsIndexedNotifications);
				this.LastSuccessfulPollTimestamp = new ExPerformanceCounter(base.CategoryName, "Notifications: Last Successful Poll Timestamp", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LastSuccessfulPollTimestamp);
				this.NotificationsStallTime = new ExPerformanceCounter(base.CategoryName, "Notifications: Stall Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NotificationsStallTime);
				this.MailboxesLeftToCrawl = new ExPerformanceCounter(base.CategoryName, "Crawler: Mailboxes Remaining", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MailboxesLeftToCrawl);
				this.MailboxesLeftToRecrawl = new ExPerformanceCounter(base.CategoryName, "Crawler: Mailboxes Remaining to Recrawl", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MailboxesLeftToRecrawl);
				this.NumberOfDocumentsSentForProcessingCrawler = new ExPerformanceCounter(base.CategoryName, "Crawler: Items Sent for Processing", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsSentForProcessingCrawler);
				this.NumberOfDocumentsIndexedCrawler = new ExPerformanceCounter(base.CategoryName, "Crawler: Items Processed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsIndexedCrawler);
				this.AverageAttemptedCrawlerRate = new ExPerformanceCounter(base.CategoryName, "Crawler: Average Rate of Attempted Items Submission", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageAttemptedCrawlerRate);
				this.AverageCrawlerRate = new ExPerformanceCounter(base.CategoryName, "Crawler: Average Rate of Items Submission", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageCrawlerRate);
				this.FailedItemsCount = new ExPerformanceCounter(base.CategoryName, "Failed Items", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FailedItemsCount);
				this.RetriableItemsCount = new ExPerformanceCounter(base.CategoryName, "Retry: Retriable Items", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RetriableItemsCount);
				this.NumberOfDocumentsSentForProcessingRetry = new ExPerformanceCounter(base.CategoryName, "Retry: Items Sent for Processing", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsSentForProcessingRetry);
				this.NumberOfDocumentsIndexedRetry = new ExPerformanceCounter(base.CategoryName, "Retry: Items Processed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsIndexedRetry);
				this.SubmissionDelaysRetry = new ExPerformanceCounter(base.CategoryName, "Retry: Submission Delays", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.SubmissionDelaysRetry);
				this.DelayTimeRetry = new ExPerformanceCounter(base.CategoryName, "Retry: Submission Delay Time", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DelayTimeRetry);
				this.MailboxesLeftToDelete = new ExPerformanceCounter(base.CategoryName, "Retry: Deleted Mailboxes Remaining", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MailboxesLeftToDelete);
				this.NumberOfDocumentsSentForDeletion = new ExPerformanceCounter(base.CategoryName, "Retry: Items Sent for Deletion", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsSentForDeletion);
				this.NumberOfDocumentsDeleted = new ExPerformanceCounter(base.CategoryName, "Retry: Items Deleted", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfDocumentsDeleted);
				this.TotalDocumentsFirstRetryAttempt = new ExPerformanceCounter(base.CategoryName, "Retry: Documents With One Retry Attempt", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalDocumentsFirstRetryAttempt);
				this.TotalDocumentsMutlipleRetryAttempts = new ExPerformanceCounter(base.CategoryName, "Retry: Documents With Multiple Retries", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalDocumentsMutlipleRetryAttempts);
				this.IndexingStatus = new ExPerformanceCounter(base.CategoryName, "Indexing Status", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.IndexingStatus);
				this.FeedingSessions = new ExPerformanceCounter(base.CategoryName, "Feeding Sessions", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.FeedingSessions);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Items Processed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				this.NumberOfDocumentsProcessed = new ExPerformanceCounter(base.CategoryName, "Items Processed", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter6
				});
				list.Add(this.NumberOfDocumentsProcessed);
				this.NumberOfItems = new ExPerformanceCounter(base.CategoryName, "Number Of Items", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.NumberOfItems);
				this.InstantSearchEnabled = new ExPerformanceCounter(base.CategoryName, "Instant Search Enabled", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.InstantSearchEnabled);
				this.CatalogSuspended = new ExPerformanceCounter(base.CategoryName, "Catalog Suspended", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CatalogSuspended);
				long num = this.NumberOfNotifications.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter7 in list)
					{
						exPerformanceCounter7.Close();
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

		public readonly ExPerformanceCounter NumberOfNotifications;

		public readonly ExPerformanceCounter NumberOfUpdateNotifications;

		public readonly ExPerformanceCounter NumberOfCreateNotifications;

		public readonly ExPerformanceCounter NumberOfDeleteNotifications;

		public readonly ExPerformanceCounter NumberOfMoveNotifications;

		public readonly ExPerformanceCounter MessagesFromTransport;

		public readonly ExPerformanceCounter MessagesFromEventBasedAssistants;

		public readonly ExPerformanceCounter MessagesFromTimeBasedAssistants;

		public readonly ExPerformanceCounter MessagesFromMigration;

		public readonly ExPerformanceCounter NumberOfItemsInNotificationQueue;

		public readonly ExPerformanceCounter NumberOfBackloggedItemsAddedToRetryTable;

		public readonly ExPerformanceCounter AgeOfLastNotificationProcessed;

		public readonly ExPerformanceCounter NumberOfNotificationsNotYetProcessed;

		public readonly ExPerformanceCounter NumberOfDocumentsSentForProcessingNotifications;

		public readonly ExPerformanceCounter NumberOfDocumentsIndexedNotifications;

		public readonly ExPerformanceCounter LastSuccessfulPollTimestamp;

		public readonly ExPerformanceCounter NotificationsStallTime;

		public readonly ExPerformanceCounter MailboxesLeftToCrawl;

		public readonly ExPerformanceCounter MailboxesLeftToRecrawl;

		public readonly ExPerformanceCounter NumberOfDocumentsSentForProcessingCrawler;

		public readonly ExPerformanceCounter NumberOfDocumentsIndexedCrawler;

		public readonly ExPerformanceCounter AverageAttemptedCrawlerRate;

		public readonly ExPerformanceCounter AverageCrawlerRate;

		public readonly ExPerformanceCounter FailedItemsCount;

		public readonly ExPerformanceCounter RetriableItemsCount;

		public readonly ExPerformanceCounter NumberOfDocumentsSentForProcessingRetry;

		public readonly ExPerformanceCounter NumberOfDocumentsIndexedRetry;

		public readonly ExPerformanceCounter SubmissionDelaysRetry;

		public readonly ExPerformanceCounter DelayTimeRetry;

		public readonly ExPerformanceCounter MailboxesLeftToDelete;

		public readonly ExPerformanceCounter NumberOfDocumentsSentForDeletion;

		public readonly ExPerformanceCounter NumberOfDocumentsDeleted;

		public readonly ExPerformanceCounter TotalDocumentsFirstRetryAttempt;

		public readonly ExPerformanceCounter TotalDocumentsMutlipleRetryAttempts;

		public readonly ExPerformanceCounter IndexingStatus;

		public readonly ExPerformanceCounter FeedingSessions;

		public readonly ExPerformanceCounter NumberOfDocumentsProcessed;

		public readonly ExPerformanceCounter NumberOfItems;

		public readonly ExPerformanceCounter InstantSearchEnabled;

		public readonly ExPerformanceCounter CatalogSuspended;
	}
}
