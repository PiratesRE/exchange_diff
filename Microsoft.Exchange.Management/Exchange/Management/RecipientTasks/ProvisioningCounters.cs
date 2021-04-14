using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal static class ProvisioningCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (ProvisioningCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in ProvisioningCounters.AllCounters)
			{
				try
				{
					element.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					element.Add(content);
				}
			}
		}

		public const string CategoryName = "MSExchange Provisioning";

		public static readonly ExPerformanceCounter NumberOfNewMailboxCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of New-Mailbox Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfNewMailuserCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of New-MailUser Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfNewRemoteMailboxCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of New-RemoteMailbox Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfNewSyncMailboxCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of New-SyncMailbox Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfNewSyncMailuserCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of New-SyncMailUser Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfUndoSoftDeletedMailboxCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of Undo-SoftDeletedMailbox Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfUndoSyncSoftDeletedMailboxCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of Undo-SyncSoftDeletedMailbox Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfUndoSyncSoftDeletedMailuserCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of Undo-SyncSoftDeletedMailuser Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSuccessfulNewMailboxCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of Successful New-Mailbox Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSuccessfulNewMailuserCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of Successful New-MailUser Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSuccessfulNewRemoteMailboxCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of Successful New-RemoteMailbox Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSuccessfulNewSyncMailboxCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of Successful New-SyncMailbox Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSuccessfulNewSyncMailuserCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of successful New-SyncMailUser Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSuccessfulUndoSoftDeletedMailboxCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of successful Undo-SoftDeletedMailbox Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSuccessfulUndoSyncSoftDeletedMailboxCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of successful Undo-SyncSoftDeletedMailbox Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfSuccessfulUndoSyncSoftDeletedMailuserCalls = new ExPerformanceCounter("MSExchange Provisioning", "Number of successful Undo-SyncSoftDeletedMailuser Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewMailboxResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Average New-Mailbox Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewMailboxResponseTimeBase = new ExPerformanceCounter("MSExchange Provisioning", "Average New-Mailbox Response TimeBase", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewMailboxResponseTimeWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-Mailbox Response Time with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewMailboxResponseTimeBaseWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-Mailbox Response TimeBase with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewMailboxResponseTimeWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-Mailbox Response Time Without Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewMailboxResponseTimeBaseWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-Mailbox Response TimeBase Without Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewMailuserResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Average New-MailUser Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewMailuserResponseTimeBase = new ExPerformanceCounter("MSExchange Provisioning", "Average New-MailUser Response TimeBase", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewMailuserResponseTimeWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-MailUser Response Time with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewMailuserResponseTimeBaseWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-MailUser Response TimeBase with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewMailuserResponseTimeWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-MailUser Response Time Without Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewMailuserResponseTimeBaseWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-MailUser Response TimeBase Without Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewRemoteMailboxResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Average New-RemoteMailbox Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewRemoteMailboxResponseTimeBase = new ExPerformanceCounter("MSExchange Provisioning", "Average New-RemoteMailbox Response TimeBase", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewRemoteMailboxResponseTimeWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-RemoteMailbox Response Time with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewRemoteMailboxResponseTimeBaseWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-RemoteMailbox Response TimeBase with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewRemoteMailboxResponseTimeWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-RemoteMailbox Response Time Without Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewRemoteMailboxResponseTimeBaseWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-RemoteMailbox Response TimeBase Without Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewSyncMailboxResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Average New-SyncMailbox Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewSyncMailboxResponseTimeBase = new ExPerformanceCounter("MSExchange Provisioning", "Average New-SyncMailbox Response TimeBase", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewSyncMailboxResponseTimeWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-SyncMailbox Response Time with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewSyncMailboxResponseTimeBaseWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-SyncMailbox Response TimeBase with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewSyncMailboxResponseTimeWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-SyncMailbox Response Time Without Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewSyncMailboxResponseTimeBaseWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-SyncMailbox Response TimeBase Without Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewSyncMailuserResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Average New-SyncMailUser Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewSyncMailuserResponseTimeBase = new ExPerformanceCounter("MSExchange Provisioning", "Average New-SyncMailUser Response TimeBase", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewSyncMailuserResponseTimeWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-SyncMailUser Response Time with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewSyncMailuserResponseTimeBaseWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-SyncMailUser Response TimeBase with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewSyncMailuserResponseTimeWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-SyncMailUser Response Time Without Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageNewSyncMailuserResponseTimeBaseWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average New-SyncMailUser Response TimeBase Without Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSoftDeletedMailboxResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SoftDeletedMailbox Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSoftDeletedMailboxResponseTimeBase = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SoftDeletedMailbox Response TimeBase", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSoftDeletedMailboxResponseTimeWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SoftDeletedMailbox Response Time with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSoftDeletedMailboxResponseTimeBaseWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SoftDeletedMailbox Response TimeBase with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSoftDeletedMailboxResponseTimeWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SoftDeletedMailbox Response Time Without Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSoftDeletedMailboxResponseTimeBaseWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SoftDeletedMailbox Response TimeBase Without Active Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSyncSoftDeletedMailboxResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SyncSoftDeletedMailbox Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSyncSoftDeletedMailboxResponseTimeBase = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SyncSoftDeletedMailbox Response TimeBase", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSyncSoftDeletedMailboxResponseTimeWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SyncSoftDeletedMailbox Response Time with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSyncSoftDeletedMailboxResponseTimeBaseWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SyncSoftDeletedMailbox Response TimeBase with Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSyncSoftDeletedMailboxResponseTimeWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SyncSoftDeletedMailbox Response Time Without Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSyncSoftDeletedMailboxResponseTimeBaseWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SyncSoftDeletedMailbox Response TimeBase Without Active Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSyncSoftDeletedMailuserResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SyncSoftDeletedMailuser Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSyncSoftDeletedMailuserResponseTimeBase = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SyncSoftDeletedMailuser Response TimeBase", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSyncSoftDeletedMailuserResponseTimeWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SyncSoftDeletedMailuser Response Time with Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSyncSoftDeletedMailuserResponseTimeBaseWithCache = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SyncSoftDeletedMailuser Response TimeBase with Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSyncSoftDeletedMailuserResponseTimeWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SyncSoftDeletedMailuser Response Time Without Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageUndoSyncSoftDeletedMailuserResponseTimeBaseWithoutCache = new ExPerformanceCounter("MSExchange Provisioning", "Average Undo-SyncSoftDeletedMailuser Response TimeBase Without Active Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalNewMailboxResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Total New-Mailbox Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalNewMailuserResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Total New-MailUser Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalNewRemoteMailboxResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Total New-RemoteMailbox Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalNewSyncMailboxResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Total New-SyncMailbox Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalNewSyncMailuserResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Total New-SyncMailUser Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUndoSoftDeletedMailboxResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Total Undo-SoftDeletedMailbox Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUndoSyncSoftDeletedMailboxResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Total Undo-SyncSoftDeletedMailbox Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalUndoSyncSoftDeletedMailuserResponseTime = new ExPerformanceCounter("MSExchange Provisioning", "Total Undo-SyncSoftDeletedMailuser Response Time", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NewMailboxCacheActivePercentage = new ExPerformanceCounter("MSExchange Provisioning", "Percentage of New-Mailbox Calls with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NewMailboxCacheActivePercentageBase = new ExPerformanceCounter("MSExchange Provisioning", "Percentage Base of New-Mailbox Calls with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NewMailuserCacheActivePercentage = new ExPerformanceCounter("MSExchange Provisioning", "Percentage of New-MailUser Calls with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NewMailuserCacheActivePercentageBase = new ExPerformanceCounter("MSExchange Provisioning", "Percentage Base of New-MailUser Calls with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NewRemoteMailboxCacheActivePercentage = new ExPerformanceCounter("MSExchange Provisioning", "Percentage of New-RemoteMailbox Calls with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NewRemoteMailboxCacheActivePercentageBase = new ExPerformanceCounter("MSExchange Provisioning", "Percentage Base of New-RemoteMailbox Calls with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NewSyncMailboxCacheActivePercentage = new ExPerformanceCounter("MSExchange Provisioning", "Percentage of New-SyncMailbox calls with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NewSyncMailboxCacheActivePercentageBase = new ExPerformanceCounter("MSExchange Provisioning", "Percentage Base of New-SyncMailbox Calls with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NewSyncMailuserCacheActivePercentage = new ExPerformanceCounter("MSExchange Provisioning", "Percentage of New-SyncMailUser Calls with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NewSyncMailuserCacheActivePercentageBase = new ExPerformanceCounter("MSExchange Provisioning", "Percentage Base of New-SyncMailUser Calls with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UndoSoftDeletedMailboxCacheActivePercentage = new ExPerformanceCounter("MSExchange Provisioning", "Percentage of Undo-SoftDeletedMailbox Calls with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UndoSoftDeletedMailboxCacheActivePercentageBase = new ExPerformanceCounter("MSExchange Provisioning", "Percentage Base of Undo-SoftDeletedMailbox Calls with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UndoSyncSoftDeletedMailboxCacheActivePercentage = new ExPerformanceCounter("MSExchange Provisioning", "Percentage of Undo-SyncSoftDeletedMailbox Calls with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UndoSyncSoftDeletedMailboxCacheActivePercentageBase = new ExPerformanceCounter("MSExchange Provisioning", "Percentage Base of Undo-SyncSoftDeletedMailbox Calls with Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UndoSyncSoftDeletedMailuserCacheActivePercentage = new ExPerformanceCounter("MSExchange Provisioning", "Percentage of Undo-SyncSoftDeletedMailuser Calls with Active Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter UndoSyncSoftDeletedMailuserCacheActivePercentageBase = new ExPerformanceCounter("MSExchange Provisioning", "Percentage Base of Undo-SyncSoftDeletedMailuser Calls with Provisioning Cache", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			ProvisioningCounters.NumberOfNewMailboxCalls,
			ProvisioningCounters.NumberOfNewMailuserCalls,
			ProvisioningCounters.NumberOfNewRemoteMailboxCalls,
			ProvisioningCounters.NumberOfNewSyncMailboxCalls,
			ProvisioningCounters.NumberOfNewSyncMailuserCalls,
			ProvisioningCounters.NumberOfUndoSoftDeletedMailboxCalls,
			ProvisioningCounters.NumberOfUndoSyncSoftDeletedMailboxCalls,
			ProvisioningCounters.NumberOfUndoSyncSoftDeletedMailuserCalls,
			ProvisioningCounters.NumberOfSuccessfulNewMailboxCalls,
			ProvisioningCounters.NumberOfSuccessfulNewMailuserCalls,
			ProvisioningCounters.NumberOfSuccessfulNewRemoteMailboxCalls,
			ProvisioningCounters.NumberOfSuccessfulNewSyncMailboxCalls,
			ProvisioningCounters.NumberOfSuccessfulNewSyncMailuserCalls,
			ProvisioningCounters.NumberOfSuccessfulUndoSoftDeletedMailboxCalls,
			ProvisioningCounters.NumberOfSuccessfulUndoSyncSoftDeletedMailboxCalls,
			ProvisioningCounters.NumberOfSuccessfulUndoSyncSoftDeletedMailuserCalls,
			ProvisioningCounters.AverageNewMailboxResponseTime,
			ProvisioningCounters.AverageNewMailboxResponseTimeBase,
			ProvisioningCounters.AverageNewMailboxResponseTimeWithCache,
			ProvisioningCounters.AverageNewMailboxResponseTimeBaseWithCache,
			ProvisioningCounters.AverageNewMailboxResponseTimeWithoutCache,
			ProvisioningCounters.AverageNewMailboxResponseTimeBaseWithoutCache,
			ProvisioningCounters.AverageNewMailuserResponseTime,
			ProvisioningCounters.AverageNewMailuserResponseTimeBase,
			ProvisioningCounters.AverageNewMailuserResponseTimeWithCache,
			ProvisioningCounters.AverageNewMailuserResponseTimeBaseWithCache,
			ProvisioningCounters.AverageNewMailuserResponseTimeWithoutCache,
			ProvisioningCounters.AverageNewMailuserResponseTimeBaseWithoutCache,
			ProvisioningCounters.AverageNewRemoteMailboxResponseTime,
			ProvisioningCounters.AverageNewRemoteMailboxResponseTimeBase,
			ProvisioningCounters.AverageNewRemoteMailboxResponseTimeWithCache,
			ProvisioningCounters.AverageNewRemoteMailboxResponseTimeBaseWithCache,
			ProvisioningCounters.AverageNewRemoteMailboxResponseTimeWithoutCache,
			ProvisioningCounters.AverageNewRemoteMailboxResponseTimeBaseWithoutCache,
			ProvisioningCounters.AverageNewSyncMailboxResponseTime,
			ProvisioningCounters.AverageNewSyncMailboxResponseTimeBase,
			ProvisioningCounters.AverageNewSyncMailboxResponseTimeWithCache,
			ProvisioningCounters.AverageNewSyncMailboxResponseTimeBaseWithCache,
			ProvisioningCounters.AverageNewSyncMailboxResponseTimeWithoutCache,
			ProvisioningCounters.AverageNewSyncMailboxResponseTimeBaseWithoutCache,
			ProvisioningCounters.AverageNewSyncMailuserResponseTime,
			ProvisioningCounters.AverageNewSyncMailuserResponseTimeBase,
			ProvisioningCounters.AverageNewSyncMailuserResponseTimeWithCache,
			ProvisioningCounters.AverageNewSyncMailuserResponseTimeBaseWithCache,
			ProvisioningCounters.AverageNewSyncMailuserResponseTimeWithoutCache,
			ProvisioningCounters.AverageNewSyncMailuserResponseTimeBaseWithoutCache,
			ProvisioningCounters.AverageUndoSoftDeletedMailboxResponseTime,
			ProvisioningCounters.AverageUndoSoftDeletedMailboxResponseTimeBase,
			ProvisioningCounters.AverageUndoSoftDeletedMailboxResponseTimeWithCache,
			ProvisioningCounters.AverageUndoSoftDeletedMailboxResponseTimeBaseWithCache,
			ProvisioningCounters.AverageUndoSoftDeletedMailboxResponseTimeWithoutCache,
			ProvisioningCounters.AverageUndoSoftDeletedMailboxResponseTimeBaseWithoutCache,
			ProvisioningCounters.AverageUndoSyncSoftDeletedMailboxResponseTime,
			ProvisioningCounters.AverageUndoSyncSoftDeletedMailboxResponseTimeBase,
			ProvisioningCounters.AverageUndoSyncSoftDeletedMailboxResponseTimeWithCache,
			ProvisioningCounters.AverageUndoSyncSoftDeletedMailboxResponseTimeBaseWithCache,
			ProvisioningCounters.AverageUndoSyncSoftDeletedMailboxResponseTimeWithoutCache,
			ProvisioningCounters.AverageUndoSyncSoftDeletedMailboxResponseTimeBaseWithoutCache,
			ProvisioningCounters.AverageUndoSyncSoftDeletedMailuserResponseTime,
			ProvisioningCounters.AverageUndoSyncSoftDeletedMailuserResponseTimeBase,
			ProvisioningCounters.AverageUndoSyncSoftDeletedMailuserResponseTimeWithCache,
			ProvisioningCounters.AverageUndoSyncSoftDeletedMailuserResponseTimeBaseWithCache,
			ProvisioningCounters.AverageUndoSyncSoftDeletedMailuserResponseTimeWithoutCache,
			ProvisioningCounters.AverageUndoSyncSoftDeletedMailuserResponseTimeBaseWithoutCache,
			ProvisioningCounters.TotalNewMailboxResponseTime,
			ProvisioningCounters.TotalNewMailuserResponseTime,
			ProvisioningCounters.TotalNewRemoteMailboxResponseTime,
			ProvisioningCounters.TotalNewSyncMailboxResponseTime,
			ProvisioningCounters.TotalNewSyncMailuserResponseTime,
			ProvisioningCounters.TotalUndoSoftDeletedMailboxResponseTime,
			ProvisioningCounters.TotalUndoSyncSoftDeletedMailboxResponseTime,
			ProvisioningCounters.TotalUndoSyncSoftDeletedMailuserResponseTime,
			ProvisioningCounters.NewMailboxCacheActivePercentage,
			ProvisioningCounters.NewMailboxCacheActivePercentageBase,
			ProvisioningCounters.NewMailuserCacheActivePercentage,
			ProvisioningCounters.NewMailuserCacheActivePercentageBase,
			ProvisioningCounters.NewRemoteMailboxCacheActivePercentage,
			ProvisioningCounters.NewRemoteMailboxCacheActivePercentageBase,
			ProvisioningCounters.NewSyncMailboxCacheActivePercentage,
			ProvisioningCounters.NewSyncMailboxCacheActivePercentageBase,
			ProvisioningCounters.NewSyncMailuserCacheActivePercentage,
			ProvisioningCounters.NewSyncMailuserCacheActivePercentageBase,
			ProvisioningCounters.UndoSoftDeletedMailboxCacheActivePercentage,
			ProvisioningCounters.UndoSoftDeletedMailboxCacheActivePercentageBase,
			ProvisioningCounters.UndoSyncSoftDeletedMailboxCacheActivePercentage,
			ProvisioningCounters.UndoSyncSoftDeletedMailboxCacheActivePercentageBase,
			ProvisioningCounters.UndoSyncSoftDeletedMailuserCacheActivePercentage,
			ProvisioningCounters.UndoSyncSoftDeletedMailuserCacheActivePercentageBase
		};
	}
}
