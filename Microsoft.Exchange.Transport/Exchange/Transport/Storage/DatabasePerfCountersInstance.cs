using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Storage
{
	internal sealed class DatabasePerfCountersInstance : PerformanceCounterInstance
	{
		internal DatabasePerfCountersInstance(string instanceName, DatabasePerfCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeTransport Database")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TransactionPendingCount = new ExPerformanceCounter(base.CategoryName, "Transaction Pending Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionPendingCount, new ExPerformanceCounter[0]);
				list.Add(this.TransactionPendingCount);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Transactions/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.TransactionCount = new ExPerformanceCounter(base.CategoryName, "Transaction Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionCount, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.TransactionCount);
				this.TransactionPending99PercentileDuration = new ExPerformanceCounter(base.CategoryName, "Transaction Pending 99 Percentile Duration", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionPending99PercentileDuration, new ExPerformanceCounter[0]);
				list.Add(this.TransactionPending99PercentileDuration);
				this.TransactionAveragePendingDuration = new ExPerformanceCounter(base.CategoryName, "Transaction Pending Average Duration", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionAveragePendingDuration, new ExPerformanceCounter[0]);
				list.Add(this.TransactionAveragePendingDuration);
				this.TransactionAveragePendingDurationBase = new ExPerformanceCounter(base.CategoryName, "Transaction Pending Average Duration Base", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionAveragePendingDurationBase, new ExPerformanceCounter[0]);
				list.Add(this.TransactionAveragePendingDurationBase);
				this.TransactionSoftCommitPendingCount = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Soft Pending", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionSoftCommitPendingCount, new ExPerformanceCounter[0]);
				list.Add(this.TransactionSoftCommitPendingCount);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Soft/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TransactionSoftCommitCount = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Soft Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionSoftCommitCount, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TransactionSoftCommitCount);
				this.TransactionSoftCommitAveragePendingDuration = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Soft Average Pending Duration", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionSoftCommitAveragePendingDuration, new ExPerformanceCounter[0]);
				list.Add(this.TransactionSoftCommitAveragePendingDuration);
				this.TransactionSoftCommitAveragePendingDurationBase = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Soft Average Pending Duration Base", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionSoftCommitAveragePendingDurationBase, new ExPerformanceCounter[0]);
				list.Add(this.TransactionSoftCommitAveragePendingDurationBase);
				this.TransactionHardCommitPendingCount = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Hard Pending", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionHardCommitPendingCount, new ExPerformanceCounter[0]);
				list.Add(this.TransactionHardCommitPendingCount);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Hard/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.TransactionHardCommitCount = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Hard Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionHardCommitCount, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.TransactionHardCommitCount);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Transaction Aborts/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.TransactionAbortCount = new ExPerformanceCounter(base.CategoryName, "Transaction Abort Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionAbortCount, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.TransactionAbortCount);
				this.TransactionHardCommitAveragePendingDuration = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Hard Pending Average Duration", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionHardCommitAveragePendingDuration, new ExPerformanceCounter[0]);
				list.Add(this.TransactionHardCommitAveragePendingDuration);
				this.TransactionHardCommitAveragePendingDurationBase = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Hard Pending Average Duration Base", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionHardCommitAveragePendingDurationBase, new ExPerformanceCounter[0]);
				list.Add(this.TransactionHardCommitAveragePendingDurationBase);
				this.TransactionAsyncCommitPendingCount = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Async Pending", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionAsyncCommitPendingCount, new ExPerformanceCounter[0]);
				list.Add(this.TransactionAsyncCommitPendingCount);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Async/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.TransactionAsyncCommitCount = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Async Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionAsyncCommitCount, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.TransactionAsyncCommitCount);
				this.TransactionAsyncCommitAveragePendingDuration = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Async Average Pending Duration", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionAsyncCommitAveragePendingDuration, new ExPerformanceCounter[0]);
				list.Add(this.TransactionAsyncCommitAveragePendingDuration);
				this.TransactionAsyncCommitAveragePendingDurationBase = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Async Pending Average Duration Base", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionAsyncCommitAveragePendingDurationBase, new ExPerformanceCounter[0]);
				list.Add(this.TransactionAsyncCommitAveragePendingDurationBase);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Transaction Durable Callback Count/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				this.TransactionDurableCallbackCount = new ExPerformanceCounter(base.CategoryName, "Transaction Durable Callback Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransactionDurableCallbackCount, new ExPerformanceCounter[]
				{
					exPerformanceCounter6
				});
				list.Add(this.TransactionDurableCallbackCount);
				this.MailItemCount = new ExPerformanceCounter(base.CategoryName, "MailItem Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MailItemCount, new ExPerformanceCounter[0]);
				list.Add(this.MailItemCount);
				this.MailRecipientCount = new ExPerformanceCounter(base.CategoryName, "MailRecipient Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MailRecipientCount, new ExPerformanceCounter[0]);
				list.Add(this.MailRecipientCount);
				this.MailRecipientActiveCount = new ExPerformanceCounter(base.CategoryName, "MailRecipient Active Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MailRecipientActiveCount, new ExPerformanceCounter[0]);
				list.Add(this.MailRecipientActiveCount);
				this.MailRecipientSafetyNetCount = new ExPerformanceCounter(base.CategoryName, "MailRecipient SafetyNet Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MailRecipientSafetyNetCount, new ExPerformanceCounter[0]);
				list.Add(this.MailRecipientSafetyNetCount);
				this.MailRecipientSafetyNetMdbCount = new ExPerformanceCounter(base.CategoryName, "MailRecipient SafetyNet Mdb Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MailRecipientSafetyNetMdbCount, new ExPerformanceCounter[0]);
				list.Add(this.MailRecipientSafetyNetMdbCount);
				this.MailRecipientShadowSafetyNetCount = new ExPerformanceCounter(base.CategoryName, "MailRecipient Shadow SafetyNet Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MailRecipientShadowSafetyNetCount, new ExPerformanceCounter[0]);
				list.Add(this.MailRecipientShadowSafetyNetCount);
				this.TransportQueueDatabaseFileSize = new ExPerformanceCounter(base.CategoryName, "Transport Queue Database File Size (MB)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransportQueueDatabaseFileSize, new ExPerformanceCounter[0]);
				list.Add(this.TransportQueueDatabaseFileSize);
				this.TransportQueueDatabaseInternalFreeSpace = new ExPerformanceCounter(base.CategoryName, "Transport Queue Database Internal Free Space (MB)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.TransportQueueDatabaseInternalFreeSpace, new ExPerformanceCounter[0]);
				list.Add(this.TransportQueueDatabaseInternalFreeSpace);
				this.GenerationCount = new ExPerformanceCounter(base.CategoryName, "Generation Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.GenerationCount, new ExPerformanceCounter[0]);
				list.Add(this.GenerationCount);
				this.GenerationExpiredCount = new ExPerformanceCounter(base.CategoryName, "Generation Expired Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.GenerationExpiredCount, new ExPerformanceCounter[0]);
				list.Add(this.GenerationExpiredCount);
				this.GenerationLastCleanupLatency = new ExPerformanceCounter(base.CategoryName, "Generation Last Cleanup Latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.GenerationLastCleanupLatency, new ExPerformanceCounter[0]);
				list.Add(this.GenerationLastCleanupLatency);
				this.BootloaderOutstandingItems = new ExPerformanceCounter(base.CategoryName, "Bootloader Outstanding Items", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.BootloaderOutstandingItems, new ExPerformanceCounter[0]);
				list.Add(this.BootloaderOutstandingItems);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Bootloaded Items/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				this.BootloadedItemCount = new ExPerformanceCounter(base.CategoryName, "Bootloaded Item Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.BootloadedItemCount, new ExPerformanceCounter[]
				{
					exPerformanceCounter7
				});
				list.Add(this.BootloadedItemCount);
				this.BootloadedItemAverageLatency = new ExPerformanceCounter(base.CategoryName, "Bootloaded Item Average Latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.BootloadedItemAverageLatency, new ExPerformanceCounter[0]);
				list.Add(this.BootloadedItemAverageLatency);
				this.BootloadedItemAverageLatencyBase = new ExPerformanceCounter(base.CategoryName, "Bootloaded Item Average Latency Base", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.BootloadedItemAverageLatencyBase, new ExPerformanceCounter[0]);
				list.Add(this.BootloadedItemAverageLatencyBase);
				this.BootloadedRecentPoisonMessageCount = new ExPerformanceCounter(base.CategoryName, "Bootloaded Recent (within 24 hours) Poison Messages", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.BootloadedRecentPoisonMessageCount, new ExPerformanceCounter[0]);
				list.Add(this.BootloadedRecentPoisonMessageCount);
				ExPerformanceCounter exPerformanceCounter8 = new ExPerformanceCounter(base.CategoryName, "Replayed Items/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter8);
				this.ReplayedItemCount = new ExPerformanceCounter(base.CategoryName, "Replayed Item Count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ReplayedItemCount, new ExPerformanceCounter[]
				{
					exPerformanceCounter8
				});
				list.Add(this.ReplayedItemCount);
				this.ReplayedItemAverageLatency = new ExPerformanceCounter(base.CategoryName, "Replayed Item Average Latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ReplayedItemAverageLatency, new ExPerformanceCounter[0]);
				list.Add(this.ReplayedItemAverageLatency);
				this.ReplayedItemAverageLatencyBase = new ExPerformanceCounter(base.CategoryName, "Average Replayed Item Average Latency Base", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ReplayedItemAverageLatencyBase, new ExPerformanceCounter[0]);
				list.Add(this.ReplayedItemAverageLatencyBase);
				this.ReplayBookmarkAverageLatency = new ExPerformanceCounter(base.CategoryName, "Replay Bookmark Average Latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ReplayBookmarkAverageLatency, new ExPerformanceCounter[0]);
				list.Add(this.ReplayBookmarkAverageLatency);
				this.ReplayBookmarkAverageLatencyBase = new ExPerformanceCounter(base.CategoryName, "Replay Bookmark Average Latency Base", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ReplayBookmarkAverageLatencyBase, new ExPerformanceCounter[0]);
				list.Add(this.ReplayBookmarkAverageLatencyBase);
				ExPerformanceCounter exPerformanceCounter9 = new ExPerformanceCounter(base.CategoryName, "DataRow seeks/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter9);
				this.Seeks = new ExPerformanceCounter(base.CategoryName, "DataRow seeks total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.Seeks, new ExPerformanceCounter[]
				{
					exPerformanceCounter9
				});
				list.Add(this.Seeks);
				ExPerformanceCounter exPerformanceCounter10 = new ExPerformanceCounter(base.CategoryName, "DataRow seeks prefix/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter10);
				this.PrefixSeeks = new ExPerformanceCounter(base.CategoryName, "DataRow seeks prefix total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.PrefixSeeks, new ExPerformanceCounter[]
				{
					exPerformanceCounter10
				});
				list.Add(this.PrefixSeeks);
				ExPerformanceCounter exPerformanceCounter11 = new ExPerformanceCounter(base.CategoryName, "DataRow loads/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter11);
				this.LoadFromCurrent = new ExPerformanceCounter(base.CategoryName, "DataRow loads total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LoadFromCurrent, new ExPerformanceCounter[]
				{
					exPerformanceCounter11
				});
				list.Add(this.LoadFromCurrent);
				ExPerformanceCounter exPerformanceCounter12 = new ExPerformanceCounter(base.CategoryName, "DataRow updates/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter12);
				this.Update = new ExPerformanceCounter(base.CategoryName, "DataRow updates total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.Update, new ExPerformanceCounter[]
				{
					exPerformanceCounter12
				});
				list.Add(this.Update);
				ExPerformanceCounter exPerformanceCounter13 = new ExPerformanceCounter(base.CategoryName, "DataRow new inserts/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter13);
				this.New = new ExPerformanceCounter(base.CategoryName, "DataRow new inserts total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.New, new ExPerformanceCounter[]
				{
					exPerformanceCounter13
				});
				list.Add(this.New);
				ExPerformanceCounter exPerformanceCounter14 = new ExPerformanceCounter(base.CategoryName, "DataRow clones/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter14);
				this.Clone = new ExPerformanceCounter(base.CategoryName, "DataRow clones total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.Clone, new ExPerformanceCounter[]
				{
					exPerformanceCounter14
				});
				list.Add(this.Clone);
				ExPerformanceCounter exPerformanceCounter15 = new ExPerformanceCounter(base.CategoryName, "DataRow moves/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter15);
				this.Move = new ExPerformanceCounter(base.CategoryName, "DataRow moves total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.Move, new ExPerformanceCounter[]
				{
					exPerformanceCounter15
				});
				list.Add(this.Move);
				ExPerformanceCounter exPerformanceCounter16 = new ExPerformanceCounter(base.CategoryName, "DataRow deletes/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter16);
				this.Delete = new ExPerformanceCounter(base.CategoryName, "DataRow deletes total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.Delete, new ExPerformanceCounter[]
				{
					exPerformanceCounter16
				});
				list.Add(this.Delete);
				ExPerformanceCounter exPerformanceCounter17 = new ExPerformanceCounter(base.CategoryName, "DataRow minimize memory/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter17);
				this.MinimizeMemory = new ExPerformanceCounter(base.CategoryName, "DataRow minimize memory total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MinimizeMemory, new ExPerformanceCounter[]
				{
					exPerformanceCounter17
				});
				list.Add(this.MinimizeMemory);
				ExPerformanceCounter exPerformanceCounter18 = new ExPerformanceCounter(base.CategoryName, "Stream read/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter18);
				this.StreamReads = new ExPerformanceCounter(base.CategoryName, "Stream read total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.StreamReads, new ExPerformanceCounter[]
				{
					exPerformanceCounter18
				});
				list.Add(this.StreamReads);
				ExPerformanceCounter exPerformanceCounter19 = new ExPerformanceCounter(base.CategoryName, "Stream bytes read/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter19);
				this.StreamBytesRead = new ExPerformanceCounter(base.CategoryName, "Stream bytes read total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.StreamBytesRead, new ExPerformanceCounter[]
				{
					exPerformanceCounter19
				});
				list.Add(this.StreamBytesRead);
				ExPerformanceCounter exPerformanceCounter20 = new ExPerformanceCounter(base.CategoryName, "Stream writes/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter20);
				this.StreamWrites = new ExPerformanceCounter(base.CategoryName, "Stream writes total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.StreamWrites, new ExPerformanceCounter[]
				{
					exPerformanceCounter20
				});
				list.Add(this.StreamWrites);
				ExPerformanceCounter exPerformanceCounter21 = new ExPerformanceCounter(base.CategoryName, "Stream bytes written/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter21);
				this.StreamBytesWritten = new ExPerformanceCounter(base.CategoryName, "Stream bytes written total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.StreamBytesWritten, new ExPerformanceCounter[]
				{
					exPerformanceCounter21
				});
				list.Add(this.StreamBytesWritten);
				ExPerformanceCounter exPerformanceCounter22 = new ExPerformanceCounter(base.CategoryName, "Stream set length/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter22);
				this.StreamSetLen = new ExPerformanceCounter(base.CategoryName, "Stream set length count", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.StreamSetLen, new ExPerformanceCounter[]
				{
					exPerformanceCounter22
				});
				list.Add(this.StreamSetLen);
				ExPerformanceCounter exPerformanceCounter23 = new ExPerformanceCounter(base.CategoryName, "Lazy bytes load requested/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter23);
				this.LazyBytesLoadRequested = new ExPerformanceCounter(base.CategoryName, "Lazy bytes load requested total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LazyBytesLoadRequested, new ExPerformanceCounter[]
				{
					exPerformanceCounter23
				});
				list.Add(this.LazyBytesLoadRequested);
				ExPerformanceCounter exPerformanceCounter24 = new ExPerformanceCounter(base.CategoryName, "Lazy bytes load performed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter24);
				this.LazyBytesLoadPerformed = new ExPerformanceCounter(base.CategoryName, "Lazy bytes load performed total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LazyBytesLoadPerformed, new ExPerformanceCounter[]
				{
					exPerformanceCounter24
				});
				list.Add(this.LazyBytesLoadPerformed);
				ExPerformanceCounter exPerformanceCounter25 = new ExPerformanceCounter(base.CategoryName, "Column cache load/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter25);
				this.ColumnsCacheLoads = new ExPerformanceCounter(base.CategoryName, "Column cache load total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ColumnsCacheLoads, new ExPerformanceCounter[]
				{
					exPerformanceCounter25
				});
				list.Add(this.ColumnsCacheLoads);
				ExPerformanceCounter exPerformanceCounter26 = new ExPerformanceCounter(base.CategoryName, "Column cache loaded columns/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter26);
				this.ColumnsCacheColumnLoads = new ExPerformanceCounter(base.CategoryName, "Column cache loaded columns total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ColumnsCacheColumnLoads, new ExPerformanceCounter[]
				{
					exPerformanceCounter26
				});
				list.Add(this.ColumnsCacheColumnLoads);
				ExPerformanceCounter exPerformanceCounter27 = new ExPerformanceCounter(base.CategoryName, "Column cache loaded bytes/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter27);
				this.ColumnsCacheByteLoads = new ExPerformanceCounter(base.CategoryName, "Column cache loaded bytes total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ColumnsCacheByteLoads, new ExPerformanceCounter[]
				{
					exPerformanceCounter27
				});
				list.Add(this.ColumnsCacheByteLoads);
				ExPerformanceCounter exPerformanceCounter28 = new ExPerformanceCounter(base.CategoryName, "Column cache save/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter28);
				this.ColumnsCacheSaves = new ExPerformanceCounter(base.CategoryName, "Column cache save total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ColumnsCacheSaves, new ExPerformanceCounter[]
				{
					exPerformanceCounter28
				});
				list.Add(this.ColumnsCacheSaves);
				ExPerformanceCounter exPerformanceCounter29 = new ExPerformanceCounter(base.CategoryName, "Column cache saved columns/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter29);
				this.ColumnsCacheColumnSaves = new ExPerformanceCounter(base.CategoryName, "Column cache saved columns total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ColumnsCacheColumnSaves, new ExPerformanceCounter[]
				{
					exPerformanceCounter29
				});
				list.Add(this.ColumnsCacheColumnSaves);
				ExPerformanceCounter exPerformanceCounter30 = new ExPerformanceCounter(base.CategoryName, "Column cache saved bytes/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter30);
				this.ColumnsCacheByteSaves = new ExPerformanceCounter(base.CategoryName, "Column cache saved bytes total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ColumnsCacheByteSaves, new ExPerformanceCounter[]
				{
					exPerformanceCounter30
				});
				list.Add(this.ColumnsCacheByteSaves);
				ExPerformanceCounter exPerformanceCounter31 = new ExPerformanceCounter(base.CategoryName, "Extended property writes/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter31);
				this.ExtendedPropertyWrites = new ExPerformanceCounter(base.CategoryName, "Extended property writes total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ExtendedPropertyWrites, new ExPerformanceCounter[]
				{
					exPerformanceCounter31
				});
				list.Add(this.ExtendedPropertyWrites);
				ExPerformanceCounter exPerformanceCounter32 = new ExPerformanceCounter(base.CategoryName, "Extended property bytes written/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter32);
				this.ExtendedPropertyBytesWritten = new ExPerformanceCounter(base.CategoryName, "Extended property bytes written total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ExtendedPropertyBytesWritten, new ExPerformanceCounter[]
				{
					exPerformanceCounter32
				});
				list.Add(this.ExtendedPropertyBytesWritten);
				ExPerformanceCounter exPerformanceCounter33 = new ExPerformanceCounter(base.CategoryName, "Extended property reads/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter33);
				this.ExtendedPropertyReads = new ExPerformanceCounter(base.CategoryName, "Extended property reads total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ExtendedPropertyReads, new ExPerformanceCounter[]
				{
					exPerformanceCounter33
				});
				list.Add(this.ExtendedPropertyReads);
				ExPerformanceCounter exPerformanceCounter34 = new ExPerformanceCounter(base.CategoryName, "Extended property bytes read/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter34);
				this.ExtendedPropertyBytesRead = new ExPerformanceCounter(base.CategoryName, "Extended property bytes read total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ExtendedPropertyBytesRead, new ExPerformanceCounter[]
				{
					exPerformanceCounter34
				});
				list.Add(this.ExtendedPropertyBytesRead);
				ExPerformanceCounter exPerformanceCounter35 = new ExPerformanceCounter(base.CategoryName, "MailItem new/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter35);
				this.NewMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem new total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NewMailItem, new ExPerformanceCounter[]
				{
					exPerformanceCounter35
				});
				list.Add(this.NewMailItem);
				ExPerformanceCounter exPerformanceCounter36 = new ExPerformanceCounter(base.CategoryName, "MailItem clone create/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter36);
				this.NewCloneMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem clone create total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.NewCloneMailItem, new ExPerformanceCounter[]
				{
					exPerformanceCounter36
				});
				list.Add(this.NewCloneMailItem);
				ExPerformanceCounter exPerformanceCounter37 = new ExPerformanceCounter(base.CategoryName, "MailItem dehydrate/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter37);
				this.DehydrateMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem dehydrate total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DehydrateMailItem, new ExPerformanceCounter[]
				{
					exPerformanceCounter37
				});
				list.Add(this.DehydrateMailItem);
				ExPerformanceCounter exPerformanceCounter38 = new ExPerformanceCounter(base.CategoryName, "MailItem load/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter38);
				this.LoadMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem load total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LoadMailItem, new ExPerformanceCounter[]
				{
					exPerformanceCounter38
				});
				list.Add(this.LoadMailItem);
				ExPerformanceCounter exPerformanceCounter39 = new ExPerformanceCounter(base.CategoryName, "MailItem commit immediate/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter39);
				this.CommitImmediateMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem commit immediate total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.CommitImmediateMailItem, new ExPerformanceCounter[]
				{
					exPerformanceCounter39
				});
				list.Add(this.CommitImmediateMailItem);
				ExPerformanceCounter exPerformanceCounter40 = new ExPerformanceCounter(base.CategoryName, "MailItem materialize/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter40);
				this.MaterializeMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem materialize", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MaterializeMailItem, new ExPerformanceCounter[]
				{
					exPerformanceCounter40
				});
				list.Add(this.MaterializeMailItem);
				ExPerformanceCounter exPerformanceCounter41 = new ExPerformanceCounter(base.CategoryName, "MailItem begin commit/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter41);
				this.BeginCommitMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem begin commit total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.BeginCommitMailItem, new ExPerformanceCounter[]
				{
					exPerformanceCounter41
				});
				list.Add(this.BeginCommitMailItem);
				ExPerformanceCounter exPerformanceCounter42 = new ExPerformanceCounter(base.CategoryName, "MailItem commit lazy/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter42);
				this.CommitLazyMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem commit lazy total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.CommitLazyMailItem, new ExPerformanceCounter[]
				{
					exPerformanceCounter42
				});
				list.Add(this.CommitLazyMailItem);
				ExPerformanceCounter exPerformanceCounter43 = new ExPerformanceCounter(base.CategoryName, "MailItem delete lazy/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter43);
				this.DeleteLazyMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem delete lazy total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DeleteLazyMailItem, new ExPerformanceCounter[]
				{
					exPerformanceCounter43
				});
				list.Add(this.DeleteLazyMailItem);
				ExPerformanceCounter exPerformanceCounter44 = new ExPerformanceCounter(base.CategoryName, "MailItem writeMimeTo/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter44);
				this.WriteMimeTo = new ExPerformanceCounter(base.CategoryName, "MailItem writeMimeTo total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.WriteMimeTo, new ExPerformanceCounter[]
				{
					exPerformanceCounter44
				});
				list.Add(this.WriteMimeTo);
				this.CurrentConnections = new ExPerformanceCounter(base.CategoryName, "Database connections current", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.CurrentConnections, new ExPerformanceCounter[0]);
				list.Add(this.CurrentConnections);
				this.RejectedConnections = new ExPerformanceCounter(base.CategoryName, "Database connections rejected total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.RejectedConnections, new ExPerformanceCounter[0]);
				list.Add(this.RejectedConnections);
				this.CursorsOpened = new ExPerformanceCounter(base.CategoryName, "Cursors opened total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.CursorsOpened, new ExPerformanceCounter[0]);
				list.Add(this.CursorsOpened);
				this.CursorsClosed = new ExPerformanceCounter(base.CategoryName, "Cursors closed total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.CursorsClosed, new ExPerformanceCounter[0]);
				list.Add(this.CursorsClosed);
				long num = this.Seeks.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter45 in list)
					{
						exPerformanceCounter45.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal DatabasePerfCountersInstance(string instanceName) : base(instanceName, "MSExchangeTransport Database")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TransactionPendingCount = new ExPerformanceCounter(base.CategoryName, "Transaction Pending Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransactionPendingCount);
				ExPerformanceCounter exPerformanceCounter = new ExPerformanceCounter(base.CategoryName, "Transactions/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter);
				this.TransactionCount = new ExPerformanceCounter(base.CategoryName, "Transaction Count", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter
				});
				list.Add(this.TransactionCount);
				this.TransactionPending99PercentileDuration = new ExPerformanceCounter(base.CategoryName, "Transaction Pending 99 Percentile Duration", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransactionPending99PercentileDuration);
				this.TransactionAveragePendingDuration = new ExPerformanceCounter(base.CategoryName, "Transaction Pending Average Duration", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransactionAveragePendingDuration);
				this.TransactionAveragePendingDurationBase = new ExPerformanceCounter(base.CategoryName, "Transaction Pending Average Duration Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransactionAveragePendingDurationBase);
				this.TransactionSoftCommitPendingCount = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Soft Pending", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransactionSoftCommitPendingCount);
				ExPerformanceCounter exPerformanceCounter2 = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Soft/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter2);
				this.TransactionSoftCommitCount = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Soft Count", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter2
				});
				list.Add(this.TransactionSoftCommitCount);
				this.TransactionSoftCommitAveragePendingDuration = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Soft Average Pending Duration", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransactionSoftCommitAveragePendingDuration);
				this.TransactionSoftCommitAveragePendingDurationBase = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Soft Average Pending Duration Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransactionSoftCommitAveragePendingDurationBase);
				this.TransactionHardCommitPendingCount = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Hard Pending", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransactionHardCommitPendingCount);
				ExPerformanceCounter exPerformanceCounter3 = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Hard/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter3);
				this.TransactionHardCommitCount = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Hard Count", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter3
				});
				list.Add(this.TransactionHardCommitCount);
				ExPerformanceCounter exPerformanceCounter4 = new ExPerformanceCounter(base.CategoryName, "Transaction Aborts/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter4);
				this.TransactionAbortCount = new ExPerformanceCounter(base.CategoryName, "Transaction Abort Count", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter4
				});
				list.Add(this.TransactionAbortCount);
				this.TransactionHardCommitAveragePendingDuration = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Hard Pending Average Duration", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransactionHardCommitAveragePendingDuration);
				this.TransactionHardCommitAveragePendingDurationBase = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Hard Pending Average Duration Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransactionHardCommitAveragePendingDurationBase);
				this.TransactionAsyncCommitPendingCount = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Async Pending", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransactionAsyncCommitPendingCount);
				ExPerformanceCounter exPerformanceCounter5 = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Async/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter5);
				this.TransactionAsyncCommitCount = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Async Count", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter5
				});
				list.Add(this.TransactionAsyncCommitCount);
				this.TransactionAsyncCommitAveragePendingDuration = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Async Average Pending Duration", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransactionAsyncCommitAveragePendingDuration);
				this.TransactionAsyncCommitAveragePendingDurationBase = new ExPerformanceCounter(base.CategoryName, "Transaction Commit Async Pending Average Duration Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransactionAsyncCommitAveragePendingDurationBase);
				ExPerformanceCounter exPerformanceCounter6 = new ExPerformanceCounter(base.CategoryName, "Transaction Durable Callback Count/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter6);
				this.TransactionDurableCallbackCount = new ExPerformanceCounter(base.CategoryName, "Transaction Durable Callback Count", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter6
				});
				list.Add(this.TransactionDurableCallbackCount);
				this.MailItemCount = new ExPerformanceCounter(base.CategoryName, "MailItem Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MailItemCount);
				this.MailRecipientCount = new ExPerformanceCounter(base.CategoryName, "MailRecipient Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MailRecipientCount);
				this.MailRecipientActiveCount = new ExPerformanceCounter(base.CategoryName, "MailRecipient Active Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MailRecipientActiveCount);
				this.MailRecipientSafetyNetCount = new ExPerformanceCounter(base.CategoryName, "MailRecipient SafetyNet Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MailRecipientSafetyNetCount);
				this.MailRecipientSafetyNetMdbCount = new ExPerformanceCounter(base.CategoryName, "MailRecipient SafetyNet Mdb Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MailRecipientSafetyNetMdbCount);
				this.MailRecipientShadowSafetyNetCount = new ExPerformanceCounter(base.CategoryName, "MailRecipient Shadow SafetyNet Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MailRecipientShadowSafetyNetCount);
				this.TransportQueueDatabaseFileSize = new ExPerformanceCounter(base.CategoryName, "Transport Queue Database File Size (MB)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransportQueueDatabaseFileSize);
				this.TransportQueueDatabaseInternalFreeSpace = new ExPerformanceCounter(base.CategoryName, "Transport Queue Database Internal Free Space (MB)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.TransportQueueDatabaseInternalFreeSpace);
				this.GenerationCount = new ExPerformanceCounter(base.CategoryName, "Generation Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GenerationCount);
				this.GenerationExpiredCount = new ExPerformanceCounter(base.CategoryName, "Generation Expired Count", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GenerationExpiredCount);
				this.GenerationLastCleanupLatency = new ExPerformanceCounter(base.CategoryName, "Generation Last Cleanup Latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.GenerationLastCleanupLatency);
				this.BootloaderOutstandingItems = new ExPerformanceCounter(base.CategoryName, "Bootloader Outstanding Items", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.BootloaderOutstandingItems);
				ExPerformanceCounter exPerformanceCounter7 = new ExPerformanceCounter(base.CategoryName, "Bootloaded Items/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter7);
				this.BootloadedItemCount = new ExPerformanceCounter(base.CategoryName, "Bootloaded Item Count", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter7
				});
				list.Add(this.BootloadedItemCount);
				this.BootloadedItemAverageLatency = new ExPerformanceCounter(base.CategoryName, "Bootloaded Item Average Latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.BootloadedItemAverageLatency);
				this.BootloadedItemAverageLatencyBase = new ExPerformanceCounter(base.CategoryName, "Bootloaded Item Average Latency Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.BootloadedItemAverageLatencyBase);
				this.BootloadedRecentPoisonMessageCount = new ExPerformanceCounter(base.CategoryName, "Bootloaded Recent (within 24 hours) Poison Messages", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.BootloadedRecentPoisonMessageCount);
				ExPerformanceCounter exPerformanceCounter8 = new ExPerformanceCounter(base.CategoryName, "Replayed Items/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter8);
				this.ReplayedItemCount = new ExPerformanceCounter(base.CategoryName, "Replayed Item Count", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter8
				});
				list.Add(this.ReplayedItemCount);
				this.ReplayedItemAverageLatency = new ExPerformanceCounter(base.CategoryName, "Replayed Item Average Latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ReplayedItemAverageLatency);
				this.ReplayedItemAverageLatencyBase = new ExPerformanceCounter(base.CategoryName, "Average Replayed Item Average Latency Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ReplayedItemAverageLatencyBase);
				this.ReplayBookmarkAverageLatency = new ExPerformanceCounter(base.CategoryName, "Replay Bookmark Average Latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ReplayBookmarkAverageLatency);
				this.ReplayBookmarkAverageLatencyBase = new ExPerformanceCounter(base.CategoryName, "Replay Bookmark Average Latency Base", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ReplayBookmarkAverageLatencyBase);
				ExPerformanceCounter exPerformanceCounter9 = new ExPerformanceCounter(base.CategoryName, "DataRow seeks/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter9);
				this.Seeks = new ExPerformanceCounter(base.CategoryName, "DataRow seeks total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter9
				});
				list.Add(this.Seeks);
				ExPerformanceCounter exPerformanceCounter10 = new ExPerformanceCounter(base.CategoryName, "DataRow seeks prefix/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter10);
				this.PrefixSeeks = new ExPerformanceCounter(base.CategoryName, "DataRow seeks prefix total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter10
				});
				list.Add(this.PrefixSeeks);
				ExPerformanceCounter exPerformanceCounter11 = new ExPerformanceCounter(base.CategoryName, "DataRow loads/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter11);
				this.LoadFromCurrent = new ExPerformanceCounter(base.CategoryName, "DataRow loads total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter11
				});
				list.Add(this.LoadFromCurrent);
				ExPerformanceCounter exPerformanceCounter12 = new ExPerformanceCounter(base.CategoryName, "DataRow updates/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter12);
				this.Update = new ExPerformanceCounter(base.CategoryName, "DataRow updates total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter12
				});
				list.Add(this.Update);
				ExPerformanceCounter exPerformanceCounter13 = new ExPerformanceCounter(base.CategoryName, "DataRow new inserts/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter13);
				this.New = new ExPerformanceCounter(base.CategoryName, "DataRow new inserts total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter13
				});
				list.Add(this.New);
				ExPerformanceCounter exPerformanceCounter14 = new ExPerformanceCounter(base.CategoryName, "DataRow clones/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter14);
				this.Clone = new ExPerformanceCounter(base.CategoryName, "DataRow clones total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter14
				});
				list.Add(this.Clone);
				ExPerformanceCounter exPerformanceCounter15 = new ExPerformanceCounter(base.CategoryName, "DataRow moves/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter15);
				this.Move = new ExPerformanceCounter(base.CategoryName, "DataRow moves total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter15
				});
				list.Add(this.Move);
				ExPerformanceCounter exPerformanceCounter16 = new ExPerformanceCounter(base.CategoryName, "DataRow deletes/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter16);
				this.Delete = new ExPerformanceCounter(base.CategoryName, "DataRow deletes total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter16
				});
				list.Add(this.Delete);
				ExPerformanceCounter exPerformanceCounter17 = new ExPerformanceCounter(base.CategoryName, "DataRow minimize memory/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter17);
				this.MinimizeMemory = new ExPerformanceCounter(base.CategoryName, "DataRow minimize memory total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter17
				});
				list.Add(this.MinimizeMemory);
				ExPerformanceCounter exPerformanceCounter18 = new ExPerformanceCounter(base.CategoryName, "Stream read/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter18);
				this.StreamReads = new ExPerformanceCounter(base.CategoryName, "Stream read total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter18
				});
				list.Add(this.StreamReads);
				ExPerformanceCounter exPerformanceCounter19 = new ExPerformanceCounter(base.CategoryName, "Stream bytes read/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter19);
				this.StreamBytesRead = new ExPerformanceCounter(base.CategoryName, "Stream bytes read total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter19
				});
				list.Add(this.StreamBytesRead);
				ExPerformanceCounter exPerformanceCounter20 = new ExPerformanceCounter(base.CategoryName, "Stream writes/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter20);
				this.StreamWrites = new ExPerformanceCounter(base.CategoryName, "Stream writes total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter20
				});
				list.Add(this.StreamWrites);
				ExPerformanceCounter exPerformanceCounter21 = new ExPerformanceCounter(base.CategoryName, "Stream bytes written/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter21);
				this.StreamBytesWritten = new ExPerformanceCounter(base.CategoryName, "Stream bytes written total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter21
				});
				list.Add(this.StreamBytesWritten);
				ExPerformanceCounter exPerformanceCounter22 = new ExPerformanceCounter(base.CategoryName, "Stream set length/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter22);
				this.StreamSetLen = new ExPerformanceCounter(base.CategoryName, "Stream set length count", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter22
				});
				list.Add(this.StreamSetLen);
				ExPerformanceCounter exPerformanceCounter23 = new ExPerformanceCounter(base.CategoryName, "Lazy bytes load requested/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter23);
				this.LazyBytesLoadRequested = new ExPerformanceCounter(base.CategoryName, "Lazy bytes load requested total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter23
				});
				list.Add(this.LazyBytesLoadRequested);
				ExPerformanceCounter exPerformanceCounter24 = new ExPerformanceCounter(base.CategoryName, "Lazy bytes load performed/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter24);
				this.LazyBytesLoadPerformed = new ExPerformanceCounter(base.CategoryName, "Lazy bytes load performed total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter24
				});
				list.Add(this.LazyBytesLoadPerformed);
				ExPerformanceCounter exPerformanceCounter25 = new ExPerformanceCounter(base.CategoryName, "Column cache load/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter25);
				this.ColumnsCacheLoads = new ExPerformanceCounter(base.CategoryName, "Column cache load total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter25
				});
				list.Add(this.ColumnsCacheLoads);
				ExPerformanceCounter exPerformanceCounter26 = new ExPerformanceCounter(base.CategoryName, "Column cache loaded columns/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter26);
				this.ColumnsCacheColumnLoads = new ExPerformanceCounter(base.CategoryName, "Column cache loaded columns total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter26
				});
				list.Add(this.ColumnsCacheColumnLoads);
				ExPerformanceCounter exPerformanceCounter27 = new ExPerformanceCounter(base.CategoryName, "Column cache loaded bytes/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter27);
				this.ColumnsCacheByteLoads = new ExPerformanceCounter(base.CategoryName, "Column cache loaded bytes total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter27
				});
				list.Add(this.ColumnsCacheByteLoads);
				ExPerformanceCounter exPerformanceCounter28 = new ExPerformanceCounter(base.CategoryName, "Column cache save/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter28);
				this.ColumnsCacheSaves = new ExPerformanceCounter(base.CategoryName, "Column cache save total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter28
				});
				list.Add(this.ColumnsCacheSaves);
				ExPerformanceCounter exPerformanceCounter29 = new ExPerformanceCounter(base.CategoryName, "Column cache saved columns/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter29);
				this.ColumnsCacheColumnSaves = new ExPerformanceCounter(base.CategoryName, "Column cache saved columns total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter29
				});
				list.Add(this.ColumnsCacheColumnSaves);
				ExPerformanceCounter exPerformanceCounter30 = new ExPerformanceCounter(base.CategoryName, "Column cache saved bytes/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter30);
				this.ColumnsCacheByteSaves = new ExPerformanceCounter(base.CategoryName, "Column cache saved bytes total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter30
				});
				list.Add(this.ColumnsCacheByteSaves);
				ExPerformanceCounter exPerformanceCounter31 = new ExPerformanceCounter(base.CategoryName, "Extended property writes/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter31);
				this.ExtendedPropertyWrites = new ExPerformanceCounter(base.CategoryName, "Extended property writes total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter31
				});
				list.Add(this.ExtendedPropertyWrites);
				ExPerformanceCounter exPerformanceCounter32 = new ExPerformanceCounter(base.CategoryName, "Extended property bytes written/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter32);
				this.ExtendedPropertyBytesWritten = new ExPerformanceCounter(base.CategoryName, "Extended property bytes written total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter32
				});
				list.Add(this.ExtendedPropertyBytesWritten);
				ExPerformanceCounter exPerformanceCounter33 = new ExPerformanceCounter(base.CategoryName, "Extended property reads/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter33);
				this.ExtendedPropertyReads = new ExPerformanceCounter(base.CategoryName, "Extended property reads total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter33
				});
				list.Add(this.ExtendedPropertyReads);
				ExPerformanceCounter exPerformanceCounter34 = new ExPerformanceCounter(base.CategoryName, "Extended property bytes read/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter34);
				this.ExtendedPropertyBytesRead = new ExPerformanceCounter(base.CategoryName, "Extended property bytes read total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter34
				});
				list.Add(this.ExtendedPropertyBytesRead);
				ExPerformanceCounter exPerformanceCounter35 = new ExPerformanceCounter(base.CategoryName, "MailItem new/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter35);
				this.NewMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem new total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter35
				});
				list.Add(this.NewMailItem);
				ExPerformanceCounter exPerformanceCounter36 = new ExPerformanceCounter(base.CategoryName, "MailItem clone create/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter36);
				this.NewCloneMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem clone create total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter36
				});
				list.Add(this.NewCloneMailItem);
				ExPerformanceCounter exPerformanceCounter37 = new ExPerformanceCounter(base.CategoryName, "MailItem dehydrate/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter37);
				this.DehydrateMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem dehydrate total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter37
				});
				list.Add(this.DehydrateMailItem);
				ExPerformanceCounter exPerformanceCounter38 = new ExPerformanceCounter(base.CategoryName, "MailItem load/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter38);
				this.LoadMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem load total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter38
				});
				list.Add(this.LoadMailItem);
				ExPerformanceCounter exPerformanceCounter39 = new ExPerformanceCounter(base.CategoryName, "MailItem commit immediate/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter39);
				this.CommitImmediateMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem commit immediate total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter39
				});
				list.Add(this.CommitImmediateMailItem);
				ExPerformanceCounter exPerformanceCounter40 = new ExPerformanceCounter(base.CategoryName, "MailItem materialize/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter40);
				this.MaterializeMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem materialize", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter40
				});
				list.Add(this.MaterializeMailItem);
				ExPerformanceCounter exPerformanceCounter41 = new ExPerformanceCounter(base.CategoryName, "MailItem begin commit/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter41);
				this.BeginCommitMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem begin commit total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter41
				});
				list.Add(this.BeginCommitMailItem);
				ExPerformanceCounter exPerformanceCounter42 = new ExPerformanceCounter(base.CategoryName, "MailItem commit lazy/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter42);
				this.CommitLazyMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem commit lazy total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter42
				});
				list.Add(this.CommitLazyMailItem);
				ExPerformanceCounter exPerformanceCounter43 = new ExPerformanceCounter(base.CategoryName, "MailItem delete lazy/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter43);
				this.DeleteLazyMailItem = new ExPerformanceCounter(base.CategoryName, "MailItem delete lazy total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter43
				});
				list.Add(this.DeleteLazyMailItem);
				ExPerformanceCounter exPerformanceCounter44 = new ExPerformanceCounter(base.CategoryName, "MailItem writeMimeTo/sec", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(exPerformanceCounter44);
				this.WriteMimeTo = new ExPerformanceCounter(base.CategoryName, "MailItem writeMimeTo total", instanceName, true, null, new ExPerformanceCounter[]
				{
					exPerformanceCounter44
				});
				list.Add(this.WriteMimeTo);
				this.CurrentConnections = new ExPerformanceCounter(base.CategoryName, "Database connections current", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CurrentConnections);
				this.RejectedConnections = new ExPerformanceCounter(base.CategoryName, "Database connections rejected total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.RejectedConnections);
				this.CursorsOpened = new ExPerformanceCounter(base.CategoryName, "Cursors opened total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CursorsOpened);
				this.CursorsClosed = new ExPerformanceCounter(base.CategoryName, "Cursors closed total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.CursorsClosed);
				long num = this.Seeks.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter45 in list)
					{
						exPerformanceCounter45.Close();
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

		public readonly ExPerformanceCounter Seeks;

		public readonly ExPerformanceCounter PrefixSeeks;

		public readonly ExPerformanceCounter LoadFromCurrent;

		public readonly ExPerformanceCounter Update;

		public readonly ExPerformanceCounter New;

		public readonly ExPerformanceCounter Clone;

		public readonly ExPerformanceCounter Move;

		public readonly ExPerformanceCounter Delete;

		public readonly ExPerformanceCounter MinimizeMemory;

		public readonly ExPerformanceCounter StreamReads;

		public readonly ExPerformanceCounter StreamBytesRead;

		public readonly ExPerformanceCounter StreamWrites;

		public readonly ExPerformanceCounter StreamBytesWritten;

		public readonly ExPerformanceCounter StreamSetLen;

		public readonly ExPerformanceCounter LazyBytesLoadRequested;

		public readonly ExPerformanceCounter LazyBytesLoadPerformed;

		public readonly ExPerformanceCounter ColumnsCacheLoads;

		public readonly ExPerformanceCounter ColumnsCacheColumnLoads;

		public readonly ExPerformanceCounter ColumnsCacheByteLoads;

		public readonly ExPerformanceCounter ColumnsCacheSaves;

		public readonly ExPerformanceCounter ColumnsCacheColumnSaves;

		public readonly ExPerformanceCounter ColumnsCacheByteSaves;

		public readonly ExPerformanceCounter ExtendedPropertyWrites;

		public readonly ExPerformanceCounter ExtendedPropertyBytesWritten;

		public readonly ExPerformanceCounter ExtendedPropertyReads;

		public readonly ExPerformanceCounter ExtendedPropertyBytesRead;

		public readonly ExPerformanceCounter TransactionPendingCount;

		public readonly ExPerformanceCounter TransactionCount;

		public readonly ExPerformanceCounter TransactionPending99PercentileDuration;

		public readonly ExPerformanceCounter TransactionAveragePendingDuration;

		public readonly ExPerformanceCounter TransactionAveragePendingDurationBase;

		public readonly ExPerformanceCounter TransactionSoftCommitPendingCount;

		public readonly ExPerformanceCounter TransactionSoftCommitCount;

		public readonly ExPerformanceCounter TransactionSoftCommitAveragePendingDuration;

		public readonly ExPerformanceCounter TransactionSoftCommitAveragePendingDurationBase;

		public readonly ExPerformanceCounter TransactionHardCommitPendingCount;

		public readonly ExPerformanceCounter TransactionHardCommitCount;

		public readonly ExPerformanceCounter TransactionAbortCount;

		public readonly ExPerformanceCounter TransactionHardCommitAveragePendingDuration;

		public readonly ExPerformanceCounter TransactionHardCommitAveragePendingDurationBase;

		public readonly ExPerformanceCounter TransactionAsyncCommitPendingCount;

		public readonly ExPerformanceCounter TransactionAsyncCommitCount;

		public readonly ExPerformanceCounter TransactionAsyncCommitAveragePendingDuration;

		public readonly ExPerformanceCounter TransactionAsyncCommitAveragePendingDurationBase;

		public readonly ExPerformanceCounter TransactionDurableCallbackCount;

		public readonly ExPerformanceCounter NewMailItem;

		public readonly ExPerformanceCounter NewCloneMailItem;

		public readonly ExPerformanceCounter DehydrateMailItem;

		public readonly ExPerformanceCounter LoadMailItem;

		public readonly ExPerformanceCounter CommitImmediateMailItem;

		public readonly ExPerformanceCounter MaterializeMailItem;

		public readonly ExPerformanceCounter BeginCommitMailItem;

		public readonly ExPerformanceCounter CommitLazyMailItem;

		public readonly ExPerformanceCounter DeleteLazyMailItem;

		public readonly ExPerformanceCounter WriteMimeTo;

		public readonly ExPerformanceCounter MailItemCount;

		public readonly ExPerformanceCounter MailRecipientCount;

		public readonly ExPerformanceCounter MailRecipientActiveCount;

		public readonly ExPerformanceCounter MailRecipientSafetyNetCount;

		public readonly ExPerformanceCounter MailRecipientSafetyNetMdbCount;

		public readonly ExPerformanceCounter MailRecipientShadowSafetyNetCount;

		public readonly ExPerformanceCounter TransportQueueDatabaseFileSize;

		public readonly ExPerformanceCounter TransportQueueDatabaseInternalFreeSpace;

		public readonly ExPerformanceCounter GenerationCount;

		public readonly ExPerformanceCounter GenerationExpiredCount;

		public readonly ExPerformanceCounter GenerationLastCleanupLatency;

		public readonly ExPerformanceCounter BootloaderOutstandingItems;

		public readonly ExPerformanceCounter BootloadedItemCount;

		public readonly ExPerformanceCounter BootloadedItemAverageLatency;

		public readonly ExPerformanceCounter BootloadedItemAverageLatencyBase;

		public readonly ExPerformanceCounter BootloadedRecentPoisonMessageCount;

		public readonly ExPerformanceCounter ReplayedItemCount;

		public readonly ExPerformanceCounter ReplayedItemAverageLatency;

		public readonly ExPerformanceCounter ReplayedItemAverageLatencyBase;

		public readonly ExPerformanceCounter ReplayBookmarkAverageLatency;

		public readonly ExPerformanceCounter ReplayBookmarkAverageLatencyBase;

		public readonly ExPerformanceCounter CurrentConnections;

		public readonly ExPerformanceCounter RejectedConnections;

		public readonly ExPerformanceCounter CursorsOpened;

		public readonly ExPerformanceCounter CursorsClosed;
	}
}
