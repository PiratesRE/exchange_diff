using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal sealed class MailboxReplicationServicePerMdbPerformanceCountersInstance : PerformanceCounterInstance
	{
		internal MailboxReplicationServicePerMdbPerformanceCountersInstance(string instanceName, MailboxReplicationServicePerMdbPerformanceCountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Mailbox Replication Service Per Mdb")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ActiveMovesTotal = new ExPerformanceCounter(base.CategoryName, "Active Moves: Total Moves", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ActiveMovesTotal, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesTotal);
				this.ActiveMovesInitialSeeding = new ExPerformanceCounter(base.CategoryName, "Active Moves: Moves in Initial Seeding State", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ActiveMovesInitialSeeding, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesInitialSeeding);
				this.ActiveMovesCompletion = new ExPerformanceCounter(base.CategoryName, "Active Moves: Moves in Completion State", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ActiveMovesCompletion, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesCompletion);
				this.ActiveMovesStalledTotal = new ExPerformanceCounter(base.CategoryName, "Active Moves: Stalled Moves Total", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ActiveMovesStalledTotal, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesStalledTotal);
				this.ActiveMovesStalledHA = new ExPerformanceCounter(base.CategoryName, "Active Moves: Stalled Moves (Database Replication)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ActiveMovesStalledHA, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesStalledHA);
				this.ActiveMovesStalledCI = new ExPerformanceCounter(base.CategoryName, "Active Moves: Stalled Moves (Content Indexing)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ActiveMovesStalledCI, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesStalledCI);
				this.ActiveMovesTransientFailures = new ExPerformanceCounter(base.CategoryName, "Active Moves: Transient Failure (Total)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ActiveMovesTransientFailures, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesTransientFailures);
				this.ActiveMovesNetworkFailures = new ExPerformanceCounter(base.CategoryName, "Active Moves: Transient Failure (Network)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ActiveMovesNetworkFailures, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesNetworkFailures);
				this.ActiveMovesMDBOffline = new ExPerformanceCounter(base.CategoryName, "Active Moves: Transient Failure (MDB Offline)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ActiveMovesMDBOffline, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesMDBOffline);
				this.ReadTransferRate = new ExPerformanceCounter(base.CategoryName, "Transfer Rate: Read (KB/sec)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ReadTransferRate, new ExPerformanceCounter[0]);
				list.Add(this.ReadTransferRate);
				this.ReadTransferRateBase = new ExPerformanceCounter(base.CategoryName, "Transfer Rate: Read (KB/sec) (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ReadTransferRateBase, new ExPerformanceCounter[0]);
				list.Add(this.ReadTransferRateBase);
				this.WriteTransferRate = new ExPerformanceCounter(base.CategoryName, "Transfer Rate: Write (KB/sec)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.WriteTransferRate, new ExPerformanceCounter[0]);
				list.Add(this.WriteTransferRate);
				this.WriteTransferRateBase = new ExPerformanceCounter(base.CategoryName, "Transfer Rate: Write (KB/sec) (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.WriteTransferRateBase, new ExPerformanceCounter[0]);
				list.Add(this.WriteTransferRateBase);
				this.MdbQueueQueued = new ExPerformanceCounter(base.CategoryName, "MDB Queue: Queued", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MdbQueueQueued, new ExPerformanceCounter[0]);
				list.Add(this.MdbQueueQueued);
				this.MdbQueueInProgress = new ExPerformanceCounter(base.CategoryName, "MDB Queue: In Progress", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MdbQueueInProgress, new ExPerformanceCounter[0]);
				list.Add(this.MdbQueueInProgress);
				this.MoveRequestsCompleted = new ExPerformanceCounter(base.CategoryName, "Move Requests: Completed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsCompleted, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCompleted);
				this.MoveRequestsCompletedRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Completed/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsCompletedRate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCompletedRate);
				this.MoveRequestsCompletedRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Completed/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsCompletedRateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCompletedRateBase);
				this.MoveRequestsCompletedWithWarnings = new ExPerformanceCounter(base.CategoryName, "Move Requests: Completed with Warnings", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsCompletedWithWarnings, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCompletedWithWarnings);
				this.MoveRequestsCompletedWithWarningsRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Completed with Warnings/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsCompletedWithWarningsRate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCompletedWithWarningsRate);
				this.MoveRequestsCompletedWithWarningsRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Completed with Warnings/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsCompletedWithWarningsRateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCompletedWithWarningsRateBase);
				this.MoveRequestsCanceled = new ExPerformanceCounter(base.CategoryName, "Move Requests: Canceled", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsCanceled, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCanceled);
				this.MoveRequestsCanceledRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Canceled/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsCanceledRate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCanceledRate);
				this.MoveRequestsCanceledRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Canceled/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsCanceledRateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCanceledRateBase);
				this.MoveRequestsTransientTotal = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsTransientTotal, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsTransientTotal);
				this.MoveRequestsTransientTotalRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsTransientTotalRate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsTransientTotalRate);
				this.MoveRequestsTransientTotalRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsTransientTotalRateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsTransientTotalRateBase);
				this.MoveRequestsNetworkFailures = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures (Network)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsNetworkFailures, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsNetworkFailures);
				this.MoveRequestsNetworkFailuresRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures (Network)/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsNetworkFailuresRate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsNetworkFailuresRate);
				this.MoveRequestsNetworkFailuresRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures (Network)/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsNetworkFailuresRateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsNetworkFailuresRateBase);
				this.MoveRequestsProxyBackoff = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures (Proxy Backoff)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsProxyBackoff, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsProxyBackoff);
				this.MoveRequestsProxyBackoffRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures (Proxy Backoff)/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsProxyBackoffRate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsProxyBackoffRate);
				this.MoveRequestsProxyBackoffRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures (Proxy Backoff)/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsProxyBackoffRateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsProxyBackoffRateBase);
				this.MoveRequestsFailTotal = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailTotal, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailTotal);
				this.MoveRequestsFailTotalRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailTotalRate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailTotalRate);
				this.MoveRequestsFailTotalRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailTotalRateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailTotalRateBase);
				this.MoveRequestsFailBadItemLimit = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Bad Item Limit)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailBadItemLimit, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailBadItemLimit);
				this.MoveRequestsFailBadItemLimitRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Bad Item Limit)/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailBadItemLimitRate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailBadItemLimitRate);
				this.MoveRequestsFailBadItemLimitRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Bad Item Limit)/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailBadItemLimitRateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailBadItemLimitRateBase);
				this.MoveRequestsFailNetwork = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Network)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailNetwork, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailNetwork);
				this.MoveRequestsFailNetworkRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Network)/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailNetworkRate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailNetworkRate);
				this.MoveRequestsFailNetworkRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Network)/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailNetworkRateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailNetworkRateBase);
				this.MoveRequestsFailStallCI = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Stall Content Indexing)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailStallCI, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailStallCI);
				this.MoveRequestsFailStallCIRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Stall Content Indexing)/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailStallCIRate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailStallCIRate);
				this.MoveRequestsFailStallCIRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Stall Content Indexing)/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailStallCIRateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailStallCIRateBase);
				this.MoveRequestsFailStallHA = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Stall Database Replication)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailStallHA, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailStallHA);
				this.MoveRequestsFailStallHARate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Stall Database Replication)/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailStallHARate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailStallHARate);
				this.MoveRequestsFailStallHARateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Stall Database Replication)/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailStallHARateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailStallHARateBase);
				this.MoveRequestsFailMAPI = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (MAPI)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailMAPI, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailMAPI);
				this.MoveRequestsFailMAPIRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (MAPI)/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailMAPIRate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailMAPIRate);
				this.MoveRequestsFailMAPIRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (MAPI)/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailMAPIRateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailMAPIRateBase);
				this.MoveRequestsFailOther = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Other)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailOther, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailOther);
				this.MoveRequestsFailOtherRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Other)/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailOtherRate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailOtherRate);
				this.MoveRequestsFailOtherRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Other)/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsFailOtherRateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailOtherRateBase);
				this.MoveRequestsStallsTotal = new ExPerformanceCounter(base.CategoryName, "Move Requests: Stalls", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsStallsTotal, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsTotal);
				this.MoveRequestsStallsTotalRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Move Stalls/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsStallsTotalRate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsTotalRate);
				this.MoveRequestsStallsTotalRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Move Stalls/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsStallsTotalRateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsTotalRateBase);
				this.MoveRequestsStallsHA = new ExPerformanceCounter(base.CategoryName, "Move Requests: Stalls (Database Replication)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsStallsHA, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsHA);
				this.MoveRequestsStallsHARate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Move Stalls (Database Replication)/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsStallsHARate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsHARate);
				this.MoveRequestsStallsHARateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Move Stalls (Database Replication)/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsStallsHARateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsHARateBase);
				this.MoveRequestsStallsCI = new ExPerformanceCounter(base.CategoryName, "Move Requests: Stalls (Content Indexing)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsStallsCI, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsCI);
				this.MoveRequestsStallsCIRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Move Stalls (Content Indexing)/hour", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsStallsCIRate, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsCIRate);
				this.MoveRequestsStallsCIRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Move Stalls (Content Indexing)/hour (base)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.MoveRequestsStallsCIRateBase, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsCIRateBase);
				this.LastScanTime = new ExPerformanceCounter(base.CategoryName, "Last Scan: Timestamp (UTC)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LastScanTime, new ExPerformanceCounter[0]);
				list.Add(this.LastScanTime);
				this.LastScanDuration = new ExPerformanceCounter(base.CategoryName, "Last Scan: Duration (msec)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LastScanDuration, new ExPerformanceCounter[0]);
				list.Add(this.LastScanDuration);
				this.LastScanFailure = new ExPerformanceCounter(base.CategoryName, "Last Scan: Scan Failure", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.LastScanFailure, new ExPerformanceCounter[0]);
				list.Add(this.LastScanFailure);
				this.UtilizationReadHiPri = new ExPerformanceCounter(base.CategoryName, "Utilization: Read jobs (high priority)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.UtilizationReadHiPri, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationReadHiPri);
				this.UtilizationReadCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Utilization: Read jobs (customer expectation)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.UtilizationReadCustomerExpectation, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationReadCustomerExpectation);
				this.UtilizationReadInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Utilization: Read jobs (internal maintenance)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.UtilizationReadInternalMaintenance, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationReadInternalMaintenance);
				this.UtilizationRead = new ExPerformanceCounter(base.CategoryName, "Utilization: Read jobs", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.UtilizationRead, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationRead);
				this.UtilizationWriteHiPri = new ExPerformanceCounter(base.CategoryName, "Utilization: Write jobs (high priority)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.UtilizationWriteHiPri, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationWriteHiPri);
				this.UtilizationWriteCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Utilization: Write jobs (customer expectation)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.UtilizationWriteCustomerExpectation, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationWriteCustomerExpectation);
				this.UtilizationWriteInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Utilization: Write jobs (internal maintenance)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.UtilizationWriteInternalMaintenance, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationWriteInternalMaintenance);
				this.UtilizationWrite = new ExPerformanceCounter(base.CategoryName, "Utilization: Write jobs", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.UtilizationWrite, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationWrite);
				this.ResourceHealthMDBLatencyHiPri = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB latency (high priority)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthMDBLatencyHiPri, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBLatencyHiPri);
				this.ResourceHealthMDBLatencyCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB latency (customer expectation)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthMDBLatencyCustomerExpectation, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBLatencyCustomerExpectation);
				this.ResourceHealthMDBLatencyInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB latency (internal maintenance)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthMDBLatencyInternalMaintenance, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBLatencyInternalMaintenance);
				this.ResourceHealthMDBLatency = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthMDBLatency, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBLatency);
				this.DynamicCapacityMDBLatencyHiPri = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB latency (high priority)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityMDBLatencyHiPri, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBLatencyHiPri);
				this.DynamicCapacityMDBLatencyCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB latency (customer expectation)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityMDBLatencyCustomerExpectation, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBLatencyCustomerExpectation);
				this.DynamicCapacityMDBLatencyInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB latency (internal maintenance)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityMDBLatencyInternalMaintenance, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBLatencyInternalMaintenance);
				this.DynamicCapacityMDBLatency = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityMDBLatency, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBLatency);
				this.ResourceHealthDiskLatencyHiPri = new ExPerformanceCounter(base.CategoryName, "Resource Health: Disk latency (high priority)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthDiskLatencyHiPri, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthDiskLatencyHiPri);
				this.ResourceHealthDiskLatencyCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Resource Health: Disk latency (customer expectation)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthDiskLatencyCustomerExpectation, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthDiskLatencyCustomerExpectation);
				this.ResourceHealthDiskLatencyInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Resource Health: Disk latency (internal maintenance)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthDiskLatencyInternalMaintenance, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthDiskLatencyInternalMaintenance);
				this.ResourceHealthDiskLatency = new ExPerformanceCounter(base.CategoryName, "Resource Health: Disk latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthDiskLatency, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthDiskLatency);
				this.DynamicCapacityDiskLatencyHiPri = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: Disk latency (high priority)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityDiskLatencyHiPri, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityDiskLatencyHiPri);
				this.DynamicCapacityDiskLatencyCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: Disk latency (customer expectation)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityDiskLatencyCustomerExpectation, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityDiskLatencyCustomerExpectation);
				this.DynamicCapacityDiskLatencyInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: Disk latency (internal maintenance)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityDiskLatencyInternalMaintenance, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityDiskLatencyInternalMaintenance);
				this.DynamicCapacityDiskLatency = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: Disk latency", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityDiskLatency, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityDiskLatency);
				this.ResourceHealthMDBReplicationHiPri = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB replication (high priority)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthMDBReplicationHiPri, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBReplicationHiPri);
				this.ResourceHealthMDBReplicationCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB replication (customer expectation)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthMDBReplicationCustomerExpectation, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBReplicationCustomerExpectation);
				this.ResourceHealthMDBReplicationInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB replication (internal maintenance)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthMDBReplicationInternalMaintenance, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBReplicationInternalMaintenance);
				this.ResourceHealthMDBReplication = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB replication", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthMDBReplication, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBReplication);
				this.DynamicCapacityMDBReplicationHiPri = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB replication (high priority)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityMDBReplicationHiPri, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBReplicationHiPri);
				this.DynamicCapacityMDBReplicationCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB replication (customer expectation)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityMDBReplicationCustomerExpectation, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBReplicationCustomerExpectation);
				this.DynamicCapacityMDBReplicationInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB replication (internal maintenance)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityMDBReplicationInternalMaintenance, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBReplicationInternalMaintenance);
				this.DynamicCapacityMDBReplication = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB replication", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityMDBReplication, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBReplication);
				this.ResourceHealthMDBAvailabilityHiPri = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB availability (high priority)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthMDBAvailabilityHiPri, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBAvailabilityHiPri);
				this.ResourceHealthMDBAvailabilityCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB availability (customer expectation)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthMDBAvailabilityCustomerExpectation, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBAvailabilityCustomerExpectation);
				this.ResourceHealthMDBAvailabilityInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB availability (internal maintenance)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthMDBAvailabilityInternalMaintenance, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBAvailabilityInternalMaintenance);
				this.ResourceHealthMDBAvailability = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB availability", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthMDBAvailability, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBAvailability);
				this.DynamicCapacityMDBAvailabilityHiPri = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB availability (high priority)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityMDBAvailabilityHiPri, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBAvailabilityHiPri);
				this.DynamicCapacityMDBAvailabilityCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB availability (customer expectation)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityMDBAvailabilityCustomerExpectation, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBAvailabilityCustomerExpectation);
				this.DynamicCapacityMDBAvailabilityInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB availability (internal maintenance)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityMDBAvailabilityInternalMaintenance, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBAvailabilityInternalMaintenance);
				this.DynamicCapacityMDBAvailability = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB availability", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityMDBAvailability, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBAvailability);
				this.ResourceHealthCIAgeOfLastNotificationHiPri = new ExPerformanceCounter(base.CategoryName, "Resource Health: CI age of last notification (high priority)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthCIAgeOfLastNotificationHiPri, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthCIAgeOfLastNotificationHiPri);
				this.ResourceHealthCIAgeOfLastNotificationCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Resource Health: CI age of last notification (customer expectation)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthCIAgeOfLastNotificationCustomerExpectation, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthCIAgeOfLastNotificationCustomerExpectation);
				this.ResourceHealthCIAgeOfLastNotificationInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Resource Health: CI age of last notification (internal maintenance)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthCIAgeOfLastNotificationInternalMaintenance, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthCIAgeOfLastNotificationInternalMaintenance);
				this.ResourceHealthCIAgeOfLastNotification = new ExPerformanceCounter(base.CategoryName, "Resource Health: CI age of last notification", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.ResourceHealthCIAgeOfLastNotification, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthCIAgeOfLastNotification);
				this.DynamicCapacityCIAgeOfLastNotificationHiPri = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: CI age of last notification (high priority)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityCIAgeOfLastNotificationHiPri, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityCIAgeOfLastNotificationHiPri);
				this.DynamicCapacityCIAgeOfLastNotificationCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: CI age of last notification (customer expectation)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityCIAgeOfLastNotificationCustomerExpectation, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityCIAgeOfLastNotificationCustomerExpectation);
				this.DynamicCapacityCIAgeOfLastNotificationInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: CI age of last notification (internal maintenance)", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityCIAgeOfLastNotificationInternalMaintenance, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityCIAgeOfLastNotificationInternalMaintenance);
				this.DynamicCapacityCIAgeOfLastNotification = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: CI age of last notification", instanceName, true, (autoUpdateTotalInstance == null) ? null : autoUpdateTotalInstance.DynamicCapacityCIAgeOfLastNotification, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityCIAgeOfLastNotification);
				long num = this.ActiveMovesTotal.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal MailboxReplicationServicePerMdbPerformanceCountersInstance(string instanceName) : base(instanceName, "MSExchange Mailbox Replication Service Per Mdb")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.ActiveMovesTotal = new ExPerformanceCounter(base.CategoryName, "Active Moves: Total Moves", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesTotal);
				this.ActiveMovesInitialSeeding = new ExPerformanceCounter(base.CategoryName, "Active Moves: Moves in Initial Seeding State", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesInitialSeeding);
				this.ActiveMovesCompletion = new ExPerformanceCounter(base.CategoryName, "Active Moves: Moves in Completion State", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesCompletion);
				this.ActiveMovesStalledTotal = new ExPerformanceCounter(base.CategoryName, "Active Moves: Stalled Moves Total", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesStalledTotal);
				this.ActiveMovesStalledHA = new ExPerformanceCounter(base.CategoryName, "Active Moves: Stalled Moves (Database Replication)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesStalledHA);
				this.ActiveMovesStalledCI = new ExPerformanceCounter(base.CategoryName, "Active Moves: Stalled Moves (Content Indexing)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesStalledCI);
				this.ActiveMovesTransientFailures = new ExPerformanceCounter(base.CategoryName, "Active Moves: Transient Failure (Total)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesTransientFailures);
				this.ActiveMovesNetworkFailures = new ExPerformanceCounter(base.CategoryName, "Active Moves: Transient Failure (Network)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesNetworkFailures);
				this.ActiveMovesMDBOffline = new ExPerformanceCounter(base.CategoryName, "Active Moves: Transient Failure (MDB Offline)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ActiveMovesMDBOffline);
				this.ReadTransferRate = new ExPerformanceCounter(base.CategoryName, "Transfer Rate: Read (KB/sec)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ReadTransferRate);
				this.ReadTransferRateBase = new ExPerformanceCounter(base.CategoryName, "Transfer Rate: Read (KB/sec) (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ReadTransferRateBase);
				this.WriteTransferRate = new ExPerformanceCounter(base.CategoryName, "Transfer Rate: Write (KB/sec)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.WriteTransferRate);
				this.WriteTransferRateBase = new ExPerformanceCounter(base.CategoryName, "Transfer Rate: Write (KB/sec) (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.WriteTransferRateBase);
				this.MdbQueueQueued = new ExPerformanceCounter(base.CategoryName, "MDB Queue: Queued", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MdbQueueQueued);
				this.MdbQueueInProgress = new ExPerformanceCounter(base.CategoryName, "MDB Queue: In Progress", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MdbQueueInProgress);
				this.MoveRequestsCompleted = new ExPerformanceCounter(base.CategoryName, "Move Requests: Completed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCompleted);
				this.MoveRequestsCompletedRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Completed/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCompletedRate);
				this.MoveRequestsCompletedRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Completed/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCompletedRateBase);
				this.MoveRequestsCompletedWithWarnings = new ExPerformanceCounter(base.CategoryName, "Move Requests: Completed with Warnings", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCompletedWithWarnings);
				this.MoveRequestsCompletedWithWarningsRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Completed with Warnings/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCompletedWithWarningsRate);
				this.MoveRequestsCompletedWithWarningsRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Completed with Warnings/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCompletedWithWarningsRateBase);
				this.MoveRequestsCanceled = new ExPerformanceCounter(base.CategoryName, "Move Requests: Canceled", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCanceled);
				this.MoveRequestsCanceledRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Canceled/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCanceledRate);
				this.MoveRequestsCanceledRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Canceled/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsCanceledRateBase);
				this.MoveRequestsTransientTotal = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsTransientTotal);
				this.MoveRequestsTransientTotalRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsTransientTotalRate);
				this.MoveRequestsTransientTotalRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsTransientTotalRateBase);
				this.MoveRequestsNetworkFailures = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures (Network)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsNetworkFailures);
				this.MoveRequestsNetworkFailuresRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures (Network)/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsNetworkFailuresRate);
				this.MoveRequestsNetworkFailuresRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures (Network)/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsNetworkFailuresRateBase);
				this.MoveRequestsProxyBackoff = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures (Proxy Backoff)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsProxyBackoff);
				this.MoveRequestsProxyBackoffRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures (Proxy Backoff)/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsProxyBackoffRate);
				this.MoveRequestsProxyBackoffRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Transient Failures (Proxy Backoff)/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsProxyBackoffRateBase);
				this.MoveRequestsFailTotal = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailTotal);
				this.MoveRequestsFailTotalRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailTotalRate);
				this.MoveRequestsFailTotalRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailTotalRateBase);
				this.MoveRequestsFailBadItemLimit = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Bad Item Limit)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailBadItemLimit);
				this.MoveRequestsFailBadItemLimitRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Bad Item Limit)/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailBadItemLimitRate);
				this.MoveRequestsFailBadItemLimitRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Bad Item Limit)/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailBadItemLimitRateBase);
				this.MoveRequestsFailNetwork = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Network)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailNetwork);
				this.MoveRequestsFailNetworkRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Network)/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailNetworkRate);
				this.MoveRequestsFailNetworkRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Network)/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailNetworkRateBase);
				this.MoveRequestsFailStallCI = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Stall Content Indexing)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailStallCI);
				this.MoveRequestsFailStallCIRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Stall Content Indexing)/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailStallCIRate);
				this.MoveRequestsFailStallCIRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Stall Content Indexing)/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailStallCIRateBase);
				this.MoveRequestsFailStallHA = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Stall Database Replication)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailStallHA);
				this.MoveRequestsFailStallHARate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Stall Database Replication)/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailStallHARate);
				this.MoveRequestsFailStallHARateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Stall Database Replication)/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailStallHARateBase);
				this.MoveRequestsFailMAPI = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (MAPI)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailMAPI);
				this.MoveRequestsFailMAPIRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (MAPI)/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailMAPIRate);
				this.MoveRequestsFailMAPIRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (MAPI)/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailMAPIRateBase);
				this.MoveRequestsFailOther = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Other)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailOther);
				this.MoveRequestsFailOtherRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Other)/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailOtherRate);
				this.MoveRequestsFailOtherRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Failed (Other)/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsFailOtherRateBase);
				this.MoveRequestsStallsTotal = new ExPerformanceCounter(base.CategoryName, "Move Requests: Stalls", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsTotal);
				this.MoveRequestsStallsTotalRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Move Stalls/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsTotalRate);
				this.MoveRequestsStallsTotalRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Move Stalls/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsTotalRateBase);
				this.MoveRequestsStallsHA = new ExPerformanceCounter(base.CategoryName, "Move Requests: Stalls (Database Replication)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsHA);
				this.MoveRequestsStallsHARate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Move Stalls (Database Replication)/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsHARate);
				this.MoveRequestsStallsHARateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Move Stalls (Database Replication)/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsHARateBase);
				this.MoveRequestsStallsCI = new ExPerformanceCounter(base.CategoryName, "Move Requests: Stalls (Content Indexing)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsCI);
				this.MoveRequestsStallsCIRate = new ExPerformanceCounter(base.CategoryName, "Move Requests: Move Stalls (Content Indexing)/hour", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsCIRate);
				this.MoveRequestsStallsCIRateBase = new ExPerformanceCounter(base.CategoryName, "Move Requests: Move Stalls (Content Indexing)/hour (base)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.MoveRequestsStallsCIRateBase);
				this.LastScanTime = new ExPerformanceCounter(base.CategoryName, "Last Scan: Timestamp (UTC)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LastScanTime);
				this.LastScanDuration = new ExPerformanceCounter(base.CategoryName, "Last Scan: Duration (msec)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LastScanDuration);
				this.LastScanFailure = new ExPerformanceCounter(base.CategoryName, "Last Scan: Scan Failure", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.LastScanFailure);
				this.UtilizationReadHiPri = new ExPerformanceCounter(base.CategoryName, "Utilization: Read jobs (high priority)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationReadHiPri);
				this.UtilizationReadCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Utilization: Read jobs (customer expectation)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationReadCustomerExpectation);
				this.UtilizationReadInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Utilization: Read jobs (internal maintenance)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationReadInternalMaintenance);
				this.UtilizationRead = new ExPerformanceCounter(base.CategoryName, "Utilization: Read jobs", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationRead);
				this.UtilizationWriteHiPri = new ExPerformanceCounter(base.CategoryName, "Utilization: Write jobs (high priority)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationWriteHiPri);
				this.UtilizationWriteCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Utilization: Write jobs (customer expectation)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationWriteCustomerExpectation);
				this.UtilizationWriteInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Utilization: Write jobs (internal maintenance)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationWriteInternalMaintenance);
				this.UtilizationWrite = new ExPerformanceCounter(base.CategoryName, "Utilization: Write jobs", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.UtilizationWrite);
				this.ResourceHealthMDBLatencyHiPri = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB latency (high priority)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBLatencyHiPri);
				this.ResourceHealthMDBLatencyCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB latency (customer expectation)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBLatencyCustomerExpectation);
				this.ResourceHealthMDBLatencyInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB latency (internal maintenance)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBLatencyInternalMaintenance);
				this.ResourceHealthMDBLatency = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBLatency);
				this.DynamicCapacityMDBLatencyHiPri = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB latency (high priority)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBLatencyHiPri);
				this.DynamicCapacityMDBLatencyCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB latency (customer expectation)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBLatencyCustomerExpectation);
				this.DynamicCapacityMDBLatencyInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB latency (internal maintenance)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBLatencyInternalMaintenance);
				this.DynamicCapacityMDBLatency = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBLatency);
				this.ResourceHealthDiskLatencyHiPri = new ExPerformanceCounter(base.CategoryName, "Resource Health: Disk latency (high priority)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthDiskLatencyHiPri);
				this.ResourceHealthDiskLatencyCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Resource Health: Disk latency (customer expectation)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthDiskLatencyCustomerExpectation);
				this.ResourceHealthDiskLatencyInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Resource Health: Disk latency (internal maintenance)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthDiskLatencyInternalMaintenance);
				this.ResourceHealthDiskLatency = new ExPerformanceCounter(base.CategoryName, "Resource Health: Disk latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthDiskLatency);
				this.DynamicCapacityDiskLatencyHiPri = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: Disk latency (high priority)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityDiskLatencyHiPri);
				this.DynamicCapacityDiskLatencyCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: Disk latency (customer expectation)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityDiskLatencyCustomerExpectation);
				this.DynamicCapacityDiskLatencyInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: Disk latency (internal maintenance)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityDiskLatencyInternalMaintenance);
				this.DynamicCapacityDiskLatency = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: Disk latency", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityDiskLatency);
				this.ResourceHealthMDBReplicationHiPri = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB replication (high priority)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBReplicationHiPri);
				this.ResourceHealthMDBReplicationCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB replication (customer expectation)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBReplicationCustomerExpectation);
				this.ResourceHealthMDBReplicationInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB replication (internal maintenance)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBReplicationInternalMaintenance);
				this.ResourceHealthMDBReplication = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB replication", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBReplication);
				this.DynamicCapacityMDBReplicationHiPri = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB replication (high priority)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBReplicationHiPri);
				this.DynamicCapacityMDBReplicationCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB replication (customer expectation)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBReplicationCustomerExpectation);
				this.DynamicCapacityMDBReplicationInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB replication (internal maintenance)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBReplicationInternalMaintenance);
				this.DynamicCapacityMDBReplication = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB replication", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBReplication);
				this.ResourceHealthMDBAvailabilityHiPri = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB availability (high priority)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBAvailabilityHiPri);
				this.ResourceHealthMDBAvailabilityCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB availability (customer expectation)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBAvailabilityCustomerExpectation);
				this.ResourceHealthMDBAvailabilityInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB availability (internal maintenance)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBAvailabilityInternalMaintenance);
				this.ResourceHealthMDBAvailability = new ExPerformanceCounter(base.CategoryName, "Resource Health: MDB availability", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthMDBAvailability);
				this.DynamicCapacityMDBAvailabilityHiPri = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB availability (high priority)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBAvailabilityHiPri);
				this.DynamicCapacityMDBAvailabilityCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB availability (customer expectation)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBAvailabilityCustomerExpectation);
				this.DynamicCapacityMDBAvailabilityInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB availability (internal maintenance)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBAvailabilityInternalMaintenance);
				this.DynamicCapacityMDBAvailability = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: MDB availability", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityMDBAvailability);
				this.ResourceHealthCIAgeOfLastNotificationHiPri = new ExPerformanceCounter(base.CategoryName, "Resource Health: CI age of last notification (high priority)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthCIAgeOfLastNotificationHiPri);
				this.ResourceHealthCIAgeOfLastNotificationCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Resource Health: CI age of last notification (customer expectation)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthCIAgeOfLastNotificationCustomerExpectation);
				this.ResourceHealthCIAgeOfLastNotificationInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Resource Health: CI age of last notification (internal maintenance)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthCIAgeOfLastNotificationInternalMaintenance);
				this.ResourceHealthCIAgeOfLastNotification = new ExPerformanceCounter(base.CategoryName, "Resource Health: CI age of last notification", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.ResourceHealthCIAgeOfLastNotification);
				this.DynamicCapacityCIAgeOfLastNotificationHiPri = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: CI age of last notification (high priority)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityCIAgeOfLastNotificationHiPri);
				this.DynamicCapacityCIAgeOfLastNotificationCustomerExpectation = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: CI age of last notification (customer expectation)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityCIAgeOfLastNotificationCustomerExpectation);
				this.DynamicCapacityCIAgeOfLastNotificationInternalMaintenance = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: CI age of last notification (internal maintenance)", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityCIAgeOfLastNotificationInternalMaintenance);
				this.DynamicCapacityCIAgeOfLastNotification = new ExPerformanceCounter(base.CategoryName, "Dynamic Capacity: CI age of last notification", instanceName, true, null, new ExPerformanceCounter[0]);
				list.Add(this.DynamicCapacityCIAgeOfLastNotification);
				long num = this.ActiveMovesTotal.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
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

		public readonly ExPerformanceCounter ActiveMovesTotal;

		public readonly ExPerformanceCounter ActiveMovesInitialSeeding;

		public readonly ExPerformanceCounter ActiveMovesCompletion;

		public readonly ExPerformanceCounter ActiveMovesStalledTotal;

		public readonly ExPerformanceCounter ActiveMovesStalledHA;

		public readonly ExPerformanceCounter ActiveMovesStalledCI;

		public readonly ExPerformanceCounter ActiveMovesTransientFailures;

		public readonly ExPerformanceCounter ActiveMovesNetworkFailures;

		public readonly ExPerformanceCounter ActiveMovesMDBOffline;

		public readonly ExPerformanceCounter ReadTransferRate;

		public readonly ExPerformanceCounter ReadTransferRateBase;

		public readonly ExPerformanceCounter WriteTransferRate;

		public readonly ExPerformanceCounter WriteTransferRateBase;

		public readonly ExPerformanceCounter MdbQueueQueued;

		public readonly ExPerformanceCounter MdbQueueInProgress;

		public readonly ExPerformanceCounter MoveRequestsCompleted;

		public readonly ExPerformanceCounter MoveRequestsCompletedRate;

		public readonly ExPerformanceCounter MoveRequestsCompletedRateBase;

		public readonly ExPerformanceCounter MoveRequestsCompletedWithWarnings;

		public readonly ExPerformanceCounter MoveRequestsCompletedWithWarningsRate;

		public readonly ExPerformanceCounter MoveRequestsCompletedWithWarningsRateBase;

		public readonly ExPerformanceCounter MoveRequestsCanceled;

		public readonly ExPerformanceCounter MoveRequestsCanceledRate;

		public readonly ExPerformanceCounter MoveRequestsCanceledRateBase;

		public readonly ExPerformanceCounter MoveRequestsTransientTotal;

		public readonly ExPerformanceCounter MoveRequestsTransientTotalRate;

		public readonly ExPerformanceCounter MoveRequestsTransientTotalRateBase;

		public readonly ExPerformanceCounter MoveRequestsNetworkFailures;

		public readonly ExPerformanceCounter MoveRequestsNetworkFailuresRate;

		public readonly ExPerformanceCounter MoveRequestsNetworkFailuresRateBase;

		public readonly ExPerformanceCounter MoveRequestsProxyBackoff;

		public readonly ExPerformanceCounter MoveRequestsProxyBackoffRate;

		public readonly ExPerformanceCounter MoveRequestsProxyBackoffRateBase;

		public readonly ExPerformanceCounter MoveRequestsFailTotal;

		public readonly ExPerformanceCounter MoveRequestsFailTotalRate;

		public readonly ExPerformanceCounter MoveRequestsFailTotalRateBase;

		public readonly ExPerformanceCounter MoveRequestsFailBadItemLimit;

		public readonly ExPerformanceCounter MoveRequestsFailBadItemLimitRate;

		public readonly ExPerformanceCounter MoveRequestsFailBadItemLimitRateBase;

		public readonly ExPerformanceCounter MoveRequestsFailNetwork;

		public readonly ExPerformanceCounter MoveRequestsFailNetworkRate;

		public readonly ExPerformanceCounter MoveRequestsFailNetworkRateBase;

		public readonly ExPerformanceCounter MoveRequestsFailStallCI;

		public readonly ExPerformanceCounter MoveRequestsFailStallCIRate;

		public readonly ExPerformanceCounter MoveRequestsFailStallCIRateBase;

		public readonly ExPerformanceCounter MoveRequestsFailStallHA;

		public readonly ExPerformanceCounter MoveRequestsFailStallHARate;

		public readonly ExPerformanceCounter MoveRequestsFailStallHARateBase;

		public readonly ExPerformanceCounter MoveRequestsFailMAPI;

		public readonly ExPerformanceCounter MoveRequestsFailMAPIRate;

		public readonly ExPerformanceCounter MoveRequestsFailMAPIRateBase;

		public readonly ExPerformanceCounter MoveRequestsFailOther;

		public readonly ExPerformanceCounter MoveRequestsFailOtherRate;

		public readonly ExPerformanceCounter MoveRequestsFailOtherRateBase;

		public readonly ExPerformanceCounter MoveRequestsStallsTotal;

		public readonly ExPerformanceCounter MoveRequestsStallsTotalRate;

		public readonly ExPerformanceCounter MoveRequestsStallsTotalRateBase;

		public readonly ExPerformanceCounter MoveRequestsStallsHA;

		public readonly ExPerformanceCounter MoveRequestsStallsHARate;

		public readonly ExPerformanceCounter MoveRequestsStallsHARateBase;

		public readonly ExPerformanceCounter MoveRequestsStallsCI;

		public readonly ExPerformanceCounter MoveRequestsStallsCIRate;

		public readonly ExPerformanceCounter MoveRequestsStallsCIRateBase;

		public readonly ExPerformanceCounter LastScanTime;

		public readonly ExPerformanceCounter LastScanDuration;

		public readonly ExPerformanceCounter LastScanFailure;

		public readonly ExPerformanceCounter UtilizationReadHiPri;

		public readonly ExPerformanceCounter UtilizationReadCustomerExpectation;

		public readonly ExPerformanceCounter UtilizationReadInternalMaintenance;

		public readonly ExPerformanceCounter UtilizationRead;

		public readonly ExPerformanceCounter UtilizationWriteHiPri;

		public readonly ExPerformanceCounter UtilizationWriteCustomerExpectation;

		public readonly ExPerformanceCounter UtilizationWriteInternalMaintenance;

		public readonly ExPerformanceCounter UtilizationWrite;

		public readonly ExPerformanceCounter ResourceHealthMDBLatencyHiPri;

		public readonly ExPerformanceCounter ResourceHealthMDBLatencyCustomerExpectation;

		public readonly ExPerformanceCounter ResourceHealthMDBLatencyInternalMaintenance;

		public readonly ExPerformanceCounter ResourceHealthMDBLatency;

		public readonly ExPerformanceCounter DynamicCapacityMDBLatencyHiPri;

		public readonly ExPerformanceCounter DynamicCapacityMDBLatencyCustomerExpectation;

		public readonly ExPerformanceCounter DynamicCapacityMDBLatencyInternalMaintenance;

		public readonly ExPerformanceCounter DynamicCapacityMDBLatency;

		public readonly ExPerformanceCounter ResourceHealthDiskLatencyHiPri;

		public readonly ExPerformanceCounter ResourceHealthDiskLatencyCustomerExpectation;

		public readonly ExPerformanceCounter ResourceHealthDiskLatencyInternalMaintenance;

		public readonly ExPerformanceCounter ResourceHealthDiskLatency;

		public readonly ExPerformanceCounter DynamicCapacityDiskLatencyHiPri;

		public readonly ExPerformanceCounter DynamicCapacityDiskLatencyCustomerExpectation;

		public readonly ExPerformanceCounter DynamicCapacityDiskLatencyInternalMaintenance;

		public readonly ExPerformanceCounter DynamicCapacityDiskLatency;

		public readonly ExPerformanceCounter ResourceHealthMDBReplicationHiPri;

		public readonly ExPerformanceCounter ResourceHealthMDBReplicationCustomerExpectation;

		public readonly ExPerformanceCounter ResourceHealthMDBReplicationInternalMaintenance;

		public readonly ExPerformanceCounter ResourceHealthMDBReplication;

		public readonly ExPerformanceCounter DynamicCapacityMDBReplicationHiPri;

		public readonly ExPerformanceCounter DynamicCapacityMDBReplicationCustomerExpectation;

		public readonly ExPerformanceCounter DynamicCapacityMDBReplicationInternalMaintenance;

		public readonly ExPerformanceCounter DynamicCapacityMDBReplication;

		public readonly ExPerformanceCounter ResourceHealthMDBAvailabilityHiPri;

		public readonly ExPerformanceCounter ResourceHealthMDBAvailabilityCustomerExpectation;

		public readonly ExPerformanceCounter ResourceHealthMDBAvailabilityInternalMaintenance;

		public readonly ExPerformanceCounter ResourceHealthMDBAvailability;

		public readonly ExPerformanceCounter DynamicCapacityMDBAvailabilityHiPri;

		public readonly ExPerformanceCounter DynamicCapacityMDBAvailabilityCustomerExpectation;

		public readonly ExPerformanceCounter DynamicCapacityMDBAvailabilityInternalMaintenance;

		public readonly ExPerformanceCounter DynamicCapacityMDBAvailability;

		public readonly ExPerformanceCounter ResourceHealthCIAgeOfLastNotificationHiPri;

		public readonly ExPerformanceCounter ResourceHealthCIAgeOfLastNotificationCustomerExpectation;

		public readonly ExPerformanceCounter ResourceHealthCIAgeOfLastNotificationInternalMaintenance;

		public readonly ExPerformanceCounter ResourceHealthCIAgeOfLastNotification;

		public readonly ExPerformanceCounter DynamicCapacityCIAgeOfLastNotificationHiPri;

		public readonly ExPerformanceCounter DynamicCapacityCIAgeOfLastNotificationCustomerExpectation;

		public readonly ExPerformanceCounter DynamicCapacityCIAgeOfLastNotificationInternalMaintenance;

		public readonly ExPerformanceCounter DynamicCapacityCIAgeOfLastNotification;
	}
}
