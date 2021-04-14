using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class ExecutionDiagnostics : IExecutionDiagnostics, ILockStatistics
	{
		public ExecutionDiagnostics()
		{
			this.executionStart = StopwatchStamp.GetStamp();
			this.chunkStatistics = ExecutionDiagnostics.ChunkStatisticsContainer.Create();
			this.operationStatistics = ExecutionDiagnostics.OperationStatisticsContainer.Create();
			this.rpcStatistics = ExecutionDiagnostics.RpcStatisticsContainer.Create();
			this.digestCollector = ResourceMonitorDigest.NullCollector;
			this.ropSummaryCollector = RopSummaryCollector.Null;
			this.instanceIdentifier = TimingContext.GetContextIdentifier();
			this.OnBeginChunk();
		}

		public bool? DatabaseRepaired { get; set; }

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
			set
			{
				this.mailboxGuid = value;
			}
		}

		public int MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
			set
			{
				this.mailboxNumber = value;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
			set
			{
				this.databaseGuid = value;
			}
		}

		public ExecutionDiagnostics.OperationSource OpSource
		{
			get
			{
				return this.operationSource;
			}
			internal set
			{
				this.operationSource = value;
			}
		}

		public int OpDetail
		{
			get
			{
				return this.operationDetail;
			}
			internal set
			{
				this.operationDetail = value;
			}
		}

		public ClientType ClientType
		{
			get
			{
				return this.clientType;
			}
		}

		public virtual byte OpNumber
		{
			get
			{
				return 0;
			}
		}

		public Guid ClientActivityId
		{
			get
			{
				return this.clientActivityId;
			}
		}

		public string ClientComponentName
		{
			get
			{
				return this.clientComponentName;
			}
		}

		public string ClientProtocolName
		{
			get
			{
				return this.clientProtocolName;
			}
		}

		public string ClientActionString
		{
			get
			{
				return this.clientActionString;
			}
		}

		public uint ExpandedClientActionStringId
		{
			get
			{
				return this.expandedClientActionStringId;
			}
		}

		public bool SharedLock
		{
			get
			{
				return this.sharedLock;
			}
		}

		public TestCaseId TestCaseId
		{
			get
			{
				return this.testCaseId;
			}
		}

		public ExecutionDiagnostics.IExecutionDiagnosticsStatistics OperationStatistics
		{
			get
			{
				if (this.operationStatistics.Count == 0U)
				{
					if (this.chunkStatistics.Started)
					{
						this.chunkStatistics.Stop(this.executionStart.ElapsedTime);
						this.TraceElapsed((LID)44988U);
					}
					return this.chunkStatistics;
				}
				return this.operationStatistics;
			}
		}

		public ExecutionDiagnostics.IExecutionDiagnosticsStatistics RpcStatistics
		{
			get
			{
				if (this.rpcStatistics.Count == 0U)
				{
					if (this.chunkStatistics.Started)
					{
						this.chunkStatistics.Stop(this.executionStart.ElapsedTime);
						this.TraceElapsed((LID)61372U);
					}
					return this.chunkStatistics;
				}
				return this.rpcStatistics;
			}
		}

		public RowStats RowStatistics
		{
			get
			{
				return this.OperationStatistics.DatabaseCollector.RowStats;
			}
		}

		internal LogTransactionInformationCollector LogTransactionInformationCollector
		{
			get
			{
				if (this.logTransactionInformationCollector == null)
				{
					this.logTransactionInformationCollector = new LogTransactionInformationCollector();
				}
				return this.logTransactionInformationCollector;
			}
		}

		protected ExecutionDiagnostics.IExecutionDiagnosticsStatistics ChunkStatistics
		{
			get
			{
				return this.chunkStatistics;
			}
		}

		protected virtual bool HasClientActivityDataToLog
		{
			get
			{
				return false;
			}
		}

		protected IDigestCollector ActivityCollector
		{
			get
			{
				return this.digestCollector;
			}
		}

		protected IRopSummaryCollector SummaryCollector
		{
			get
			{
				return this.ropSummaryCollector;
			}
			set
			{
				this.ropSummaryCollector = value;
			}
		}

		internal StorePerClientTypePerformanceCountersInstance PerClientPerfInstance
		{
			get
			{
				return this.perClientPerfInstance;
			}
		}

		protected virtual bool HasDataToLog
		{
			get
			{
				return this.IsLongOperation || this.IsResourceIntensive || (this.chunkStatistics.DatabaseTracker.HasDataToLog && ConfigurationSchema.DiagnosticsThresholdDatabaseTime.Value <= this.chunkStatistics.DatabaseCollector.TotalTime) || (this.chunkStatistics.LockTracker.HasDataToLog && ConfigurationSchema.DiagnosticsThresholdLockTime.Value <= this.chunkStatistics.LockTotalTime) || (this.chunkStatistics.DirectoryTracker.HasDataToLog && (ConfigurationSchema.DiagnosticsThresholdDirectoryTime.Value <= this.chunkStatistics.DirectoryTracker.GetTotalTime() || ConfigurationSchema.DiagnosticsThresholdDirectoryCalls.Value <= this.chunkStatistics.DirectoryTracker.GetAggregatedOperationData().Count));
			}
		}

		private bool IsLongOperation
		{
			get
			{
				return ConfigurationSchema.DiagnosticsThresholdInteractionTime.Value <= this.InteractionTotal || ConfigurationSchema.DiagnosticsThresholdChunkElapsedTime.Value <= this.chunkStatistics.ElapsedTime;
			}
		}

		public virtual TimeSpan InteractionTotal
		{
			get
			{
				return this.chunkStatistics.DatabaseCollector.TotalTime + this.chunkStatistics.LockTotalTime + this.chunkStatistics.DirectoryTotalTime;
			}
		}

		protected bool IsResourceIntensive
		{
			get
			{
				return ConfigurationSchema.DiagnosticsThresholdPagesPreread.Value <= this.chunkStatistics.DatabaseCollector.ThreadStats.cPagePreread || ConfigurationSchema.DiagnosticsThresholdPagesRead.Value <= this.chunkStatistics.DatabaseCollector.ThreadStats.cPageRead || ConfigurationSchema.DiagnosticsThresholdPagesDirtied.Value <= this.chunkStatistics.DatabaseCollector.ThreadStats.cPageDirtied;
			}
		}

		public string DiagnosticInformationForWatsonReport
		{
			get
			{
				TraceContentBuilder traceContentBuilder = TraceContentBuilder.Create();
				this.FormatCommonInformation(traceContentBuilder, 0, Guid.Empty);
				ExecutionDiagnostics.FormatLine(traceContentBuilder, 0, "Database Repaired?: " + ((this.DatabaseRepaired != null) ? this.DatabaseRepaired.ToString() : "<unknown>"));
				this.FormatDiagnosticInformation(traceContentBuilder, 0);
				this.FormatOperationInformation(traceContentBuilder, 0);
				this.FormatClientActivityDiagnosticInformation(traceContentBuilder, 0);
				IStoreSimpleQueryTarget<ThreadManager.ThreadDiagnosticInfo> instance = ThreadManager.Instance;
				ExecutionDiagnostics.FormatLine(traceContentBuilder, 0, "Threads:");
				foreach (ThreadManager.ThreadDiagnosticInfo threadDiagnosticInfo in instance.GetRows(null))
				{
					ExecutionDiagnostics.FormatLine(traceContentBuilder, 1, string.Format("Id={0}, Method={1}, Client={2}, Mailbox={3}, Status={4}, StartUtcTime={5}, Duration={6}", new object[]
					{
						threadDiagnosticInfo.NativeId,
						threadDiagnosticInfo.MethodName,
						threadDiagnosticInfo.Client,
						threadDiagnosticInfo.MailboxGuid,
						threadDiagnosticInfo.Status,
						threadDiagnosticInfo.StartUtcTime,
						threadDiagnosticInfo.Duration
					}));
				}
				return traceContentBuilder.ToString();
			}
		}

		public bool InMailboxOperationContext
		{
			get
			{
				return this.inMailboxOperationContext;
			}
		}

		public virtual uint TypeIdentifier
		{
			get
			{
				return 0U;
			}
		}

		int IExecutionDiagnostics.MailboxNumber
		{
			get
			{
				return this.MailboxNumber;
			}
		}

		byte IExecutionDiagnostics.OperationId
		{
			get
			{
				return this.OpNumber;
			}
		}

		byte IExecutionDiagnostics.OperationType
		{
			get
			{
				return (byte)this.OpSource;
			}
		}

		byte IExecutionDiagnostics.ClientType
		{
			get
			{
				return (byte)this.ClientType;
			}
		}

		byte IExecutionDiagnostics.OperationFlags
		{
			get
			{
				return (byte)(this.OpDetail % 1000);
			}
		}

		int IExecutionDiagnostics.CorrelationId
		{
			get
			{
				return (int)this.ExpandedClientActionStringId;
			}
		}

		public static IBinaryLogger GetLogger(LoggerType loggerType)
		{
			return LoggerManager.GetLogger(loggerType);
		}

		public void SetClientActivityInfo(Guid activityId, string componentName, string protocolName, string actionString)
		{
			this.clientActivityId = activityId;
			this.clientComponentName = componentName;
			this.clientProtocolName = protocolName;
			this.clientActionString = actionString;
			string str = string.Format("{0}.{1}", componentName, actionString);
			this.expandedClientActionStringId = ClientActivityStrings.GetStringId(str);
		}

		public void OnExceptionCatch(Exception exception)
		{
			this.OnExceptionCatch(exception, null);
		}

		public void OnExceptionCatch(Exception exception, object diagnosticData)
		{
			ErrorHelper.OnExceptionCatch((byte)this.OpSource, this.OpNumber, (byte)this.ClientType, this.databaseGuid.GetHashCode(), this.MailboxNumber, exception, diagnosticData);
			int value = ConfigurationSchema.MaximumNumberOfExceptions.Value;
			if (value == 0 || (this.exceptionHistory != null && this.exceptionHistory.Count == value))
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "Too many exceptions");
			}
			if (this.exceptionHistory == null)
			{
				this.exceptionHistory = new List<Exception>(value);
			}
			this.exceptionHistory.Add(exception);
		}

		public virtual void OnUnhandledException(Exception exception)
		{
			string arg;
			string text;
			string arg2;
			ErrorHelper.GetExceptionSummary(exception, out arg, out text, out arg2);
			this.TryPrequarantineMailbox(string.Format("{0}: {1}", arg, arg2));
		}

		public byte GetClientType()
		{
			return (byte)this.ClientType;
		}

		public byte GetOperation()
		{
			return this.OpNumber;
		}

		public void OnAfterLockAcquisition(LockManager.LockType lockType, bool locked, bool contested, ILockStatistics owner, TimeSpan waited)
		{
			byte ownerClientType = (contested && owner != null) ? owner.GetClientType() : 0;
			byte ownerOperation = (contested && owner != null) ? owner.GetOperation() : 0;
			LockAcquisitionTracker lockAcquisitionTracker = LockAcquisitionTracker.Create(lockType, locked, contested, ownerClientType, ownerOperation, waited);
			LockAcquisitionTracker.Data data = this.RecordOperation<LockAcquisitionTracker.Data>(lockAcquisitionTracker);
			if (data != null)
			{
				data.Aggregate(lockAcquisitionTracker.Tracked);
			}
		}

		public void SetFastWaitTime(TimeSpan fastWaitTime)
		{
			this.chunkStatistics.FastWaitTime = fastWaitTime;
		}

		public override int GetHashCode()
		{
			return this.ClientType.GetHashCode() ^ this.OpSource.GetHashCode() ^ this.chunkStatistics.LockTracker.GetHashCode() ^ this.chunkStatistics.DirectoryTracker.GetHashCode();
		}

		public void UpdateClientType(ClientType clientType)
		{
			if (this.clientType == clientType)
			{
				return;
			}
			if (this.clientType != ClientType.MaxValue)
			{
				this.DisablePerClientTypePerfCounterUpdate();
			}
			this.clientType = clientType;
			if (this.clientType != ClientType.MaxValue)
			{
				this.EnablePerClientTypePerfCounterUpdate();
			}
		}

		public void UpdateTestCaseId(TestCaseId testCaseId)
		{
			this.testCaseId = testCaseId;
		}

		public virtual TOperationData RecordOperation<TOperationData>(IOperationExecutionTrackable operation) where TOperationData : class, IExecutionTrackingData<TOperationData>, new()
		{
			if (typeof(TOperationData) == typeof(DatabaseOperationStatistics))
			{
				return (TOperationData)((object)this.chunkStatistics.DatabaseTracker.RecordOperation(operation));
			}
			if (typeof(TOperationData) == typeof(DatabaseConnectionStatistics))
			{
				return (TOperationData)((object)this.chunkStatistics.DatabaseCollector);
			}
			if (typeof(TOperationData) == typeof(LockAcquisitionTracker.Data))
			{
				return (TOperationData)((object)this.chunkStatistics.LockTracker.RecordOperation(operation));
			}
			if (typeof(TOperationData) == typeof(ExecutionDiagnostics.DirectoryTrackingData))
			{
				return (TOperationData)((object)this.chunkStatistics.DirectoryTracker.RecordOperation(operation));
			}
			return default(TOperationData);
		}

		public virtual void FormatCommonInformation(TraceContentBuilder cb, int indentLevel, Guid correlationId)
		{
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Correlation ID: " + correlationId.ToString());
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Build Number: " + ExWatson.ApplicationVersion.ToString());
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Database GUID: " + this.DatabaseGuid);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Database Hash: " + this.DatabaseGuid.GetHashCode().ToString());
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Mailbox GUID: " + this.MailboxGuid);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Mailbox Number: " + this.MailboxNumber.ToString());
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Operation source: " + this.OpSource);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Client Type: " + this.clientType);
			if (!string.IsNullOrEmpty(this.clientProtocolName))
			{
				ExecutionDiagnostics.FormatLine(cb, indentLevel, "Client Protocol: " + this.clientProtocolName);
			}
			if (!string.IsNullOrEmpty(this.clientComponentName))
			{
				ExecutionDiagnostics.FormatLine(cb, indentLevel, "Client Component: " + this.clientComponentName);
			}
			if (!string.IsNullOrEmpty(this.clientActionString))
			{
				ExecutionDiagnostics.FormatLine(cb, indentLevel, "Client Action: " + this.clientActionString);
			}
			if (this.clientActivityId != Guid.Empty)
			{
				ExecutionDiagnostics.FormatLine(cb, indentLevel, "Client Activity: " + this.clientActivityId);
			}
			if (this.testCaseId.IsNotNull)
			{
				ExecutionDiagnostics.FormatLine(cb, indentLevel, "Test case id: " + this.testCaseId.ToString());
			}
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Hash Code: " + this.GetHashCode().ToString());
		}

		internal void ResetLogTransactionInformationCollector()
		{
			this.logTransactionInformationCollector = null;
		}

		internal virtual void OnStartMailboxOperation(Guid databaseGuid, int mailboxNumber, Guid mailboxGuid, ExecutionDiagnostics.OperationSource operationSource, IDigestCollector digestCollector, IRopSummaryCollector ropSummaryCollector, bool sharedLock)
		{
			this.mailboxGuid = mailboxGuid;
			this.mailboxNumber = mailboxNumber;
			this.databaseGuid = databaseGuid;
			this.operationSource = operationSource;
			this.digestCollector = ((digestCollector != null) ? digestCollector : ResourceMonitorDigest.NullCollector);
			this.ropSummaryCollector = ((ropSummaryCollector != null) ? ropSummaryCollector : RopSummaryCollector.Null);
			this.inMailboxOperationContext = true;
			this.sharedLock = sharedLock;
			this.OnBeginChunk();
			if (ExTraceGlobals.MailboxLockTracer.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				TraceContentBuilder traceContentBuilder = TraceContentBuilder.Create();
				traceContentBuilder.Append("Lock Mailbox:");
				ExecutionDiagnostics.FormatLine(traceContentBuilder, 0, "Common info:");
				this.FormatCommonInformation(traceContentBuilder, 1, Guid.Empty);
				ExTraceGlobals.MailboxLockTracer.TracePerformance(0L, traceContentBuilder.ToString());
			}
			this.diagnosticDumped = false;
		}

		internal virtual void OnBeforeEndMailboxOperation()
		{
			this.OnEndChunk();
			this.DumpDiagnosticIfNeeded();
		}

		internal virtual void OnAfterEndMailboxOperation()
		{
			ResourceDigestStats activity = new ResourceDigestStats(ref this.chunkStatistics.DatabaseCollector.ThreadStats);
			this.digestCollector.LogActivity(activity);
			if (ExTraceGlobals.MailboxLockTracer.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				TraceContentBuilder traceContentBuilder = TraceContentBuilder.Create();
				traceContentBuilder.Append("Unlock Mailbox: ");
				ExecutionDiagnostics.FormatLine(traceContentBuilder, 0, "Common info:");
				this.FormatCommonInformation(traceContentBuilder, 1, Guid.Empty);
				ExTraceGlobals.MailboxLockTracer.TracePerformance(0L, traceContentBuilder.ToString());
			}
			this.inMailboxOperationContext = false;
			this.OnBeginChunk();
		}

		public void OnBeginChunk()
		{
			if (!this.chunkStatistics.Started)
			{
				this.chunkStatistics.Reset();
				this.chunkStatistics.Start(this.executionStart.ElapsedTime);
				this.TraceReset();
				this.TraceStart((LID)58848U);
			}
		}

		public void OnBeginMailboxTaskQueueChunk()
		{
			this.chunkStatistics.Reset();
			this.chunkStatistics.Start(this.executionStart.ElapsedTime);
			this.TraceReset();
			this.TraceStart((LID)53180U);
		}

		public void OnBeginOperation()
		{
			this.operationStatistics.Reset();
		}

		public void OnBeginRpc()
		{
			this.rpcStatistics.Reset();
		}

		public void OnEndChunk()
		{
			if (this.chunkStatistics.Started)
			{
				this.chunkStatistics.Stop(this.executionStart.ElapsedTime);
				this.TraceElapsed((LID)62432U);
			}
			this.operationStatistics.Aggregate(this.chunkStatistics);
		}

		public void OnEndMailboxTaskQueueChunk()
		{
			this.OnEndChunk();
		}

		public void OnEndOperation(OperationType operationType, uint activityid, byte operationId, uint errorCode, bool isNewActivity)
		{
			this.ropSummaryCollector.Add(new RopTraceKey(operationType, this.MailboxNumber, this.ClientType, activityid, operationId, (uint)this.OpDetail, this.SharedLock), new RopSummaryParameters(this.OperationStatistics.ElapsedTime, errorCode, isNewActivity, this.OperationStatistics.DatabaseCollector.ThreadStats, (uint)this.OperationStatistics.DirectoryCount, (uint)this.OperationStatistics.DatabaseCollector.OffPageBlobHits, this.OperationStatistics.CpuKernelTime, this.OperationStatistics.CpuUserTime, this.OperationStatistics.Count, this.OperationStatistics.MaximumChunkTime, this.OperationStatistics.LockTotalTime, this.OperationStatistics.DirectoryTotalTime, this.OperationStatistics.DatabaseCollector.TotalTime, this.OperationStatistics.FastWaitTime));
			this.rpcStatistics.Aggregate(this.operationStatistics);
		}

		public void OnEndRpc(OperationType operationType, uint activityid, byte operationId, uint errorCode, bool isNewActivity)
		{
			this.ropSummaryCollector.Add(new RopTraceKey(operationType, this.MailboxNumber, this.ClientType, activityid, operationId, (uint)this.OpDetail, this.SharedLock), new RopSummaryParameters(this.RpcStatistics.ElapsedTime, errorCode, isNewActivity, this.RpcStatistics.DatabaseCollector.ThreadStats, (uint)this.RpcStatistics.DirectoryCount, (uint)this.RpcStatistics.DatabaseCollector.OffPageBlobHits, this.RpcStatistics.CpuKernelTime, this.RpcStatistics.CpuUserTime, this.RpcStatistics.Count, this.RpcStatistics.MaximumChunkTime, this.RpcStatistics.LockTotalTime, this.RpcStatistics.DirectoryTotalTime, this.RpcStatistics.DatabaseCollector.TotalTime, this.RpcStatistics.FastWaitTime));
		}

		public void OnTransactionAbort()
		{
			ErrorHelper.AddBreadcrumb(BreadcrumbKind.Abort, (byte)this.OpSource, this.OpNumber, (byte)this.ClientType, this.databaseGuid.GetHashCode(), this.MailboxNumber, 0, null);
		}

		public void TraceReset()
		{
			TimingContext.Reset();
		}

		public void TraceStart(LID lid)
		{
			TimingContext.TraceStart(lid, this.TypeIdentifier, this.instanceIdentifier);
		}

		public void TraceElapsed(LID lid)
		{
			TimingContext.TraceElapsed(lid, this.TypeIdentifier, this.instanceIdentifier);
		}

		internal void TryPrequarantineMailbox(string reason)
		{
			if (this.DatabaseGuid == Guid.Empty || this.MailboxGuid == Guid.Empty)
			{
				return;
			}
			MailboxQuarantineProvider.Instance.PrequarantineMailbox(this.DatabaseGuid, this.MailboxGuid, reason);
			Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_MailboxPrequarantined, new object[]
			{
				this.MailboxGuid.ToString(),
				this.DatabaseGuid.ToString(),
				reason
			});
		}

		internal void SetClientActionStringForTest(string clientAction)
		{
			this.clientActionString = clientAction;
		}

		internal virtual void EnablePerClientTypePerfCounterUpdate()
		{
			this.perClientPerfInstance = PerformanceCounterFactory.GetClientTypeInstance(this.clientType);
			if (PerClientTypeTracing.IsConfigured && PerClientTypeTracing.IsEnabled(this.clientType))
			{
				PerClientTypeTracing.TurnOn();
				this.perClientTracingEnabled = true;
			}
		}

		internal virtual void DisablePerClientTypePerfCounterUpdate()
		{
			if (this.perClientTracingEnabled)
			{
				this.perClientTracingEnabled = false;
				PerClientTypeTracing.TurnOff();
			}
			if (this.perClientPerfInstance != null)
			{
				this.perClientPerfInstance.JetPageReferencedRate.IncrementBy((long)this.OperationStatistics.DatabaseCollector.ThreadStats.cPageReferenced);
				this.perClientPerfInstance.JetPageReadRate.IncrementBy((long)this.OperationStatistics.DatabaseCollector.ThreadStats.cPageRead);
				this.perClientPerfInstance.JetPagePrereadRate.IncrementBy((long)this.OperationStatistics.DatabaseCollector.ThreadStats.cPagePreread);
				this.perClientPerfInstance.JetPageDirtiedRate.IncrementBy((long)this.OperationStatistics.DatabaseCollector.ThreadStats.cPageDirtied);
				this.perClientPerfInstance.JetPageReDirtiedRate.IncrementBy((long)this.OperationStatistics.DatabaseCollector.ThreadStats.cPageRedirtied);
				this.perClientPerfInstance.JetLogRecordRate.IncrementBy((long)this.OperationStatistics.DatabaseCollector.ThreadStats.cLogRecord);
				this.perClientPerfInstance.JetLogRecordBytesRate.IncrementBy((long)((ulong)this.OperationStatistics.DatabaseCollector.ThreadStats.cbLogRecord));
			}
			this.perClientPerfInstance = null;
		}

		internal void ClearExceptionHistory()
		{
			if (this.exceptionHistory != null)
			{
				this.exceptionHistory.Clear();
			}
		}

		protected static void FormatLine(TraceContentBuilder cb, int indentLevel, string line)
		{
			cb.Indent(indentLevel);
			cb.AppendLine(line);
		}

		protected static void FormatThresholdLine(TraceContentBuilder cb, int indentLevel, string label, long value, long threshold, string unit)
		{
			ExecutionDiagnostics.FormatLine(cb, indentLevel, string.Concat(new string[]
			{
				label,
				": ",
				value.ToString("N0", CultureInfo.InvariantCulture),
				" (",
				threshold.ToString("N0", CultureInfo.InvariantCulture),
				") ",
				unit
			}));
		}

		protected LockAcquisitionTracker.Data GetLockAcquisitionData(ExecutionDiagnostics.LockCategory category)
		{
			return this.chunkStatistics.LockTracker.GetAggregatedOperationData<ExecutionDiagnostics.LockCategory>(delegate(ExecutionDiagnostics.ExecutionTracker<LockAcquisitionTracker.Data>.ExecutionEntryKey key, ExecutionDiagnostics.LockCategory contextCategory)
			{
				LockAcquisitionTracker.Key key2 = key.TrackingKey as LockAcquisitionTracker.Key;
				return key2 != null && ExecutionDiagnostics.GetLockCategory(key2.LockType) == contextCategory;
			}, category);
		}

		protected void DumpDiagnosticIfNeeded()
		{
			if (this.chunkStatistics.Started)
			{
				this.chunkStatistics.Stop(this.executionStart.ElapsedTime);
				this.TraceElapsed((LID)36796U);
			}
			Guid correlationId = Guid.NewGuid();
			if (ExTraceGlobals.ExecutionDiagnosticsTracer.IsTraceEnabled(TraceType.DebugTrace) && this.chunkStatistics.DatabaseCollector.TimeInDatabase != TimeSpan.Zero)
			{
				ExTraceGlobals.ExecutionDiagnosticsTracer.TraceDebug(0L, this.GetDetailContent(correlationId).ToString());
			}
			IBinaryLogger logger = ExecutionDiagnostics.GetLogger(LoggerType.LongOperation);
			if (this.DumpDiagnosticIfNeeded(logger, LoggerManager.TraceGuids.LongOperationDetail, correlationId))
			{
				ExecutionDiagnostics.LongOperationSummary summaryContent = this.GetSummaryContent(correlationId);
				using (TraceBuffer traceBuffer = TraceRecord.Create(LoggerManager.TraceGuids.LongOperationSummary, true, false, summaryContent.DatabaseGuid.ToString(), summaryContent.MailboxGuid.ToString(), summaryContent.ClientType, summaryContent.OperationSource, summaryContent.OperationType, summaryContent.OperationName, summaryContent.OperationDetail, summaryContent.ChunkElapsedTime, summaryContent.InteractionTotal, summaryContent.PagesPreread, summaryContent.PagesRead, summaryContent.PagesDirtied, summaryContent.LogBytesWritten, summaryContent.SortOrderCount, summaryContent.NumberOfPlansExecuted, summaryContent.PlansExecutionTime, summaryContent.NumberOfDirectoryOperations, summaryContent.DirectoryOperationsTime, summaryContent.NumberOfLocksAttempted, summaryContent.NumberOfLocksSucceeded, summaryContent.LocksWaitTime, summaryContent.IsLongOperation, summaryContent.IsResourceIntensive, summaryContent.IsContested, summaryContent.TimeInDatabase, summaryContent.ClientProtocol, summaryContent.ClientComponent, summaryContent.ClientAction, summaryContent.CorrelationId.ToString(), summaryContent.BuildNumber, summaryContent.TimeInCpuKernel, summaryContent.TimeInCpuUser, summaryContent.HashCode))
				{
					logger.TryWrite(traceBuffer);
				}
			}
		}

		protected bool DumpClientActivityDiagnosticIfNeeded()
		{
			if (this.ClientActivityId == Guid.Empty)
			{
				return false;
			}
			if (!this.HasClientActivityDataToLog)
			{
				return false;
			}
			IBinaryLogger logger = ExecutionDiagnostics.GetLogger(LoggerType.HeavyClientActivity);
			if (logger == null || !logger.IsLoggingEnabled)
			{
				return false;
			}
			Guid correlationId = Guid.NewGuid();
			TraceContentBuilder traceContentBuilder = TraceContentBuilder.Create();
			traceContentBuilder.AppendLine();
			ExecutionDiagnostics.FormatLine(traceContentBuilder, 0, "Common Info:");
			this.FormatCommonInformation(traceContentBuilder, 1, correlationId);
			this.FormatClientActivityThresholdInformation(traceContentBuilder, 1);
			this.FormatClientActivityDiagnosticInformation(traceContentBuilder, 0);
			ExecutionDiagnostics.TruncateContent(traceContentBuilder);
			List<string> list = traceContentBuilder.ToWideString();
			for (int i = 0; i < list.Count; i++)
			{
				using (TraceBuffer traceBuffer = TraceRecord.Create(LoggerManager.TraceGuids.HeavyClientActivityDetail, true, false, correlationId.ToString(), i, list.Count, list[i]))
				{
					ExecutionDiagnostics.WriteDataToLog(logger, traceBuffer);
				}
			}
			ExecutionDiagnostics.HeavyClientActivitySummary heavyClientActivitySummary = default(ExecutionDiagnostics.HeavyClientActivitySummary);
			this.GetClientActivitySummaryInformation(correlationId, ref heavyClientActivitySummary);
			using (TraceBuffer traceBuffer2 = TraceRecord.Create(LoggerManager.TraceGuids.HeavyClientActivitySummary, true, false, heavyClientActivitySummary.DatabaseGuid.ToString(), heavyClientActivitySummary.MailboxGuid.ToString(), heavyClientActivitySummary.ClientType, heavyClientActivitySummary.OperationSource, heavyClientActivitySummary.Activity, heavyClientActivitySummary.TotalRpcCalls, heavyClientActivitySummary.TotalRops, heavyClientActivitySummary.CorrelationId.ToString()))
			{
				logger.TryWrite(traceBuffer2);
			}
			return true;
		}

		protected bool DumpDiagnosticIfNeeded(IBinaryLogger logger, Guid recordGuid, Guid correlationId)
		{
			if (logger == null || !logger.IsLoggingEnabled)
			{
				return false;
			}
			if (this.diagnosticDumped || !this.HasDataToLog)
			{
				return false;
			}
			TraceContentBuilder detailContent = this.GetDetailContent(correlationId);
			List<string> list = detailContent.ToWideString();
			for (int i = 0; i < list.Count; i++)
			{
				using (TraceBuffer traceBuffer = TraceRecord.Create(recordGuid, true, false, correlationId.ToString(), i, list.Count, list[i]))
				{
					ExecutionDiagnostics.WriteDataToLog(logger, traceBuffer);
					this.diagnosticDumped = true;
				}
			}
			return true;
		}

		protected virtual void FormatClientActivityThresholdInformation(TraceContentBuilder cb, int indentLevel)
		{
		}

		protected virtual void FormatThresholdInformation(TraceContentBuilder cb, int indentLevel)
		{
			long value = (long)this.chunkStatistics.DatabaseCollector.TotalTime.TotalMilliseconds;
			long value2 = (long)this.InteractionTotal.TotalMilliseconds;
			int cPagePreread = this.chunkStatistics.DatabaseCollector.ThreadStats.cPagePreread;
			int cPageRead = this.chunkStatistics.DatabaseCollector.ThreadStats.cPageRead;
			int cPageDirtied = this.chunkStatistics.DatabaseCollector.ThreadStats.cPageDirtied;
			long threshold = (long)ConfigurationSchema.DiagnosticsThresholdDatabaseTime.Value.TotalMilliseconds;
			long threshold2 = (long)ConfigurationSchema.DiagnosticsThresholdInteractionTime.Value.TotalMilliseconds;
			int value3 = ConfigurationSchema.DiagnosticsThresholdPagesPreread.Value;
			int value4 = ConfigurationSchema.DiagnosticsThresholdPagesRead.Value;
			int value5 = ConfigurationSchema.DiagnosticsThresholdPagesDirtied.Value;
			long value6 = (long)this.chunkStatistics.LockTracker.GetTotalTime().TotalMilliseconds;
			long threshold3 = (long)ConfigurationSchema.DiagnosticsThresholdLockTime.Value.TotalMilliseconds;
			long value7 = (long)this.chunkStatistics.DirectoryTracker.GetTotalTime().TotalMilliseconds;
			long threshold4 = (long)ConfigurationSchema.DiagnosticsThresholdDirectoryTime.Value.TotalMilliseconds;
			int count = this.chunkStatistics.DirectoryTracker.GetAggregatedOperationData().Count;
			int value8 = ConfigurationSchema.DiagnosticsThresholdDirectoryCalls.Value;
			long value9 = (long)this.chunkStatistics.ElapsedTime.TotalMilliseconds;
			long threshold5 = (long)ConfigurationSchema.DiagnosticsThresholdChunkElapsedTime.Value.TotalMilliseconds;
			ExecutionDiagnostics.FormatLine(cb, 0, "Diagnostic Thresholds:");
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "AD Operations", (long)count, (long)value8, string.Empty);
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "Total AD Interaction", value7, threshold4, "ms");
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "Total LK Interaction", value6, threshold3, "ms");
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "Total DB Interaction", value, threshold, "ms");
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "Interaction Total", value2, threshold2, "ms");
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "Chunk elapsed time", value9, threshold5, "ms");
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "Pages Preread", (long)cPagePreread, (long)value3, string.Empty);
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "Pages Read", (long)cPageRead, (long)value4, string.Empty);
			ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "Pages Dirtied", (long)cPageDirtied, (long)value5, string.Empty);
		}

		protected virtual void FormatOperationInformation(TraceContentBuilder cb, int indentLevel)
		{
			uint cbLogRecord = (uint)this.chunkStatistics.DatabaseCollector.ThreadStats.cbLogRecord;
			ExecutionDiagnostics.FormatLine(cb, 0, "Additional Operation Information:");
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Lock Contested: " + (this.chunkStatistics.LockTracker.GetAggregatedOperationData().NumberContested > 0));
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Long Operation: " + this.IsLongOperation);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Resource Intensive: " + this.IsResourceIntensive);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Log Bytes Written: " + cbLogRecord.ToString("N0", CultureInfo.InvariantCulture));
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Kernel CPU: " + this.chunkStatistics.CpuKernelTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture) + " ms");
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "User CPU: " + this.chunkStatistics.CpuUserTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture) + " ms");
		}

		protected virtual void FormatDiagnosticInformation(TraceContentBuilder cb, int indentLevel)
		{
			if (this.chunkStatistics.DirectoryTracker.HasDataToLog)
			{
				this.chunkStatistics.DirectoryTracker.FormatData(cb, indentLevel, "Executed AD queries:", this.IsResourceIntensive);
			}
			if (this.chunkStatistics.LockTracker.HasDataToLog)
			{
				this.chunkStatistics.LockTracker.FormatData(cb, indentLevel, "Attempted Lock acquisitions:", this.IsResourceIntensive);
			}
			if (!this.chunkStatistics.DatabaseCollector.RowStats.IsEmpty)
			{
				ExecutionDiagnostics.FormatLine(cb, indentLevel, "Logical IO statistics:");
				cb.Indent(indentLevel + 1);
				this.chunkStatistics.DatabaseCollector.AppendToTraceContentBuilder(cb);
				cb.AppendLine();
			}
			byte[] array;
			TimingContext.ExtractInfo(out array);
			if (array != null && array.Length > 0)
			{
				ExecutionDiagnostics.FormatLine(cb, indentLevel, "Timing trace:");
				ExecutionDiagnostics.FormatTimingInformation(cb, indentLevel, array);
			}
			if (this.chunkStatistics.DatabaseTracker.HasDataToLog)
			{
				this.chunkStatistics.DatabaseTracker.FormatData(cb, indentLevel, "Executed DB plans:", this.IsResourceIntensive);
			}
		}

		protected virtual void FormatClientActivityDiagnosticInformation(TraceContentBuilder cb, int indentLevel)
		{
		}

		protected virtual void GetSummaryInformation(Guid correlationId, ref ExecutionDiagnostics.LongOperationSummary summary)
		{
			summary.CorrelationId = correlationId;
			summary.BuildNumber = ExWatson.ApplicationVersion.ToString();
			summary.MailboxGuid = this.MailboxGuid;
			summary.DatabaseGuid = this.DatabaseGuid;
			summary.ClientType = this.ClientType.ToString();
			summary.OperationSource = this.OpSource.ToString();
			summary.ClientProtocol = this.ClientProtocolName;
			summary.ClientComponent = this.ClientComponentName;
			summary.ClientAction = this.ClientActionString;
			summary.ChunkElapsedTime = (long)this.chunkStatistics.ElapsedTime.TotalMilliseconds;
			summary.InteractionTotal = (long)this.InteractionTotal.TotalMilliseconds;
			summary.TimeInDatabase = (long)this.chunkStatistics.DatabaseCollector.TotalTime.TotalMilliseconds;
			summary.PagesPreread = this.chunkStatistics.DatabaseCollector.ThreadStats.cPagePreread;
			summary.PagesRead = this.chunkStatistics.DatabaseCollector.ThreadStats.cPageRead;
			summary.PagesDirtied = this.chunkStatistics.DatabaseCollector.ThreadStats.cPageDirtied;
			summary.LogBytesWritten = ((this.chunkStatistics.DatabaseCollector.ThreadStats.cbLogRecord >= 0) ? this.chunkStatistics.DatabaseCollector.ThreadStats.cbLogRecord : int.MaxValue);
			summary.IsLongOperation = this.IsLongOperation;
			summary.IsResourceIntensive = this.IsResourceIntensive;
			summary.TimeInCpuKernel = (long)this.chunkStatistics.CpuKernelTime.TotalMilliseconds;
			summary.TimeInCpuUser = (long)this.chunkStatistics.CpuUserTime.TotalMilliseconds;
			summary.HashCode = this.GetHashCode();
			if (this.chunkStatistics.DatabaseTracker.HasDataToLog)
			{
				summary.NumberOfPlansExecuted = this.chunkStatistics.DatabaseTracker.GetTotalCount();
				summary.PlansExecutionTime = (long)this.chunkStatistics.DatabaseTracker.GetTotalTime().TotalMilliseconds;
			}
			if (this.chunkStatistics.LockTracker.HasDataToLog)
			{
				LockAcquisitionTracker.Data aggregatedOperationData = this.chunkStatistics.LockTracker.GetAggregatedOperationData();
				summary.NumberOfLocksAttempted = aggregatedOperationData.Count;
				summary.NumberOfLocksSucceeded = aggregatedOperationData.NumberSucceeded;
				summary.LocksWaitTime = (long)aggregatedOperationData.TotalTime.TotalMilliseconds;
				summary.IsContested = (aggregatedOperationData.NumberContested > 0);
			}
			if (this.chunkStatistics.DirectoryTracker.HasDataToLog)
			{
				ExecutionDiagnostics.DirectoryTrackingData aggregatedOperationData2 = this.chunkStatistics.DirectoryTracker.GetAggregatedOperationData();
				summary.NumberOfDirectoryOperations = aggregatedOperationData2.Count;
				summary.DirectoryOperationsTime = (long)aggregatedOperationData2.ExecutionTime.TotalMilliseconds;
			}
		}

		protected virtual void GetClientActivitySummaryInformation(Guid correlationId, ref ExecutionDiagnostics.HeavyClientActivitySummary summary)
		{
			summary.CorrelationId = correlationId;
			summary.MailboxGuid = this.MailboxGuid;
			summary.DatabaseGuid = this.DatabaseGuid;
			summary.ClientType = this.ClientType.ToString();
			summary.OperationSource = this.OpSource.ToString();
			summary.Activity = this.GetClientActivityString();
		}

		private static void WriteDataToLog(IBinaryLogger logger, TraceBuffer buffer)
		{
			if (!logger.TryWrite(buffer))
			{
				StorePerDatabasePerformanceCountersInstance databaseInstance = PerformanceCounterFactory.GetDatabaseInstance(null);
				if (databaseInstance != null)
				{
					databaseInstance.LostDiagnosticEntries.Increment();
				}
			}
		}

		private static void TruncateContent(TraceContentBuilder cb)
		{
			if (cb.Length > 131052)
			{
				while (cb.Length > 131052)
				{
					cb.Remove();
				}
				cb.Append("<TRUNCATED>");
			}
		}

		private static void FormatTimingInformation(TraceContentBuilder cb, int indentLevel, byte[] buffer)
		{
			if (cb != null)
			{
				DateTime? dateTime = null;
				foreach (TimingContext.LocationAndTimeRecord locationAndTimeRecord in TimingContext.LocationAndTimeRecord.Parse(buffer))
				{
					cb.Indent(indentLevel + 1);
					cb.Append("LID ");
					cb.Append(locationAndTimeRecord.Lid & DiagnosticContext.ContextLidMask);
					cb.Append(", TID ");
					cb.Append(locationAndTimeRecord.Tid);
					cb.Append(", DID ");
					cb.Append(((DiagnosticSource)locationAndTimeRecord.Did).ToString());
					cb.Append(", CID ");
					cb.Append(locationAndTimeRecord.Cid);
					cb.Append(", ");
					if ((locationAndTimeRecord.Lid & DiagnosticContext.ContextSignatureMask) == 813694976U)
					{
						dateTime = new DateTime?(new DateTime((long)locationAndTimeRecord.Info, DateTimeKind.Utc));
						cb.Append(dateTime.Value.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fffffff"));
						cb.Append(" UTC");
					}
					else if (dateTime != null)
					{
						dateTime = new DateTime?(dateTime.Value.AddTicks((long)locationAndTimeRecord.Info));
						cb.Append(dateTime.Value.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fffffff"));
						cb.Append(" UTC, +");
						cb.Append(TimeSpan.FromTicks((long)locationAndTimeRecord.Info).TotalMilliseconds);
						cb.Append(" ms");
					}
					else
					{
						cb.Append(TimeSpan.FromTicks((long)locationAndTimeRecord.Info).TotalMilliseconds);
						cb.Append(" ms");
					}
					cb.AppendLine();
				}
			}
		}

		private static ExecutionDiagnostics.LockCategory GetLockCategory(LockManager.LockType lockType)
		{
			switch (lockType)
			{
			case LockManager.LockType.MailboxExclusive:
				return ExecutionDiagnostics.LockCategory.Mailbox;
			case LockManager.LockType.UserInformationExclusive:
			case LockManager.LockType.UserExclusive:
			case LockManager.LockType.PerUserExclusive:
				break;
			case LockManager.LockType.LogicalIndexCacheExclusive:
				return ExecutionDiagnostics.LockCategory.Component;
			case LockManager.LockType.LogicalIndexExclusive:
				return ExecutionDiagnostics.LockCategory.Component;
			case LockManager.LockType.PerUserCacheExclusive:
				return ExecutionDiagnostics.LockCategory.Component;
			case LockManager.LockType.ChangeNumberAndIdCountersExclusive:
				return ExecutionDiagnostics.LockCategory.Component;
			case LockManager.LockType.MailboxComponentsExclusive:
				return ExecutionDiagnostics.LockCategory.Component;
			default:
				switch (lockType)
				{
				case LockManager.LockType.MailboxShared:
					return ExecutionDiagnostics.LockCategory.Mailbox;
				case LockManager.LockType.LogicalIndexCacheShared:
					return ExecutionDiagnostics.LockCategory.Component;
				case LockManager.LockType.LogicalIndexShared:
					return ExecutionDiagnostics.LockCategory.Component;
				case LockManager.LockType.PerUserCacheShared:
					return ExecutionDiagnostics.LockCategory.Component;
				case LockManager.LockType.ChangeNumberAndIdCountersShared:
					return ExecutionDiagnostics.LockCategory.Component;
				case LockManager.LockType.MailboxComponentsShared:
					return ExecutionDiagnostics.LockCategory.Component;
				}
				break;
			}
			return ExecutionDiagnostics.LockCategory.Other;
		}

		private TraceContentBuilder GetDetailContent(Guid correlationId)
		{
			TraceContentBuilder traceContentBuilder = TraceContentBuilder.Create(ExecutionDiagnostics.maxChunkListSize);
			ExecutionDiagnostics.FormatLine(traceContentBuilder, 0, "Common Info:");
			this.FormatCommonInformation(traceContentBuilder, 1, correlationId);
			this.FormatThresholdInformation(traceContentBuilder, 1);
			this.FormatOperationInformation(traceContentBuilder, 1);
			this.FormatDiagnosticInformation(traceContentBuilder, 0);
			ExecutionDiagnostics.TruncateContent(traceContentBuilder);
			return traceContentBuilder;
		}

		private ExecutionDiagnostics.LongOperationSummary GetSummaryContent(Guid correlationId)
		{
			ExecutionDiagnostics.LongOperationSummary result = default(ExecutionDiagnostics.LongOperationSummary);
			this.GetSummaryInformation(correlationId, ref result);
			return result;
		}

		private string GetClientActivityString()
		{
			return string.Format("{0}{1}{2}{3}{4}", new object[]
			{
				this.clientComponentName ?? string.Empty,
				string.IsNullOrEmpty(this.clientComponentName) ? string.Empty : ".",
				this.clientActionString ?? string.Empty,
				string.IsNullOrEmpty(this.clientActionString) ? string.Empty : ".",
				this.clientActivityId
			});
		}

		private const int MaxContentLength = 131052;

		private static int maxChunkListSize = (int)Math.Ceiling(131052.0 / (double)TraceContentBuilder.MaximumChunkLength) + 1;

		private Guid mailboxGuid;

		private int mailboxNumber;

		private Guid databaseGuid;

		private ExecutionDiagnostics.OperationSource operationSource;

		private int operationDetail;

		private ClientType clientType = ClientType.MaxValue;

		private Guid clientActivityId;

		private string clientComponentName;

		private string clientProtocolName;

		private string clientActionString;

		private uint expandedClientActionStringId;

		private bool sharedLock;

		private IDigestCollector digestCollector;

		private IRopSummaryCollector ropSummaryCollector;

		private StorePerClientTypePerformanceCountersInstance perClientPerfInstance;

		private LogTransactionInformationCollector logTransactionInformationCollector;

		private ExecutionDiagnostics.ChunkStatisticsContainer chunkStatistics;

		private ExecutionDiagnostics.OperationStatisticsContainer operationStatistics;

		private ExecutionDiagnostics.RpcStatisticsContainer rpcStatistics;

		private bool perClientTracingEnabled;

		private bool inMailboxOperationContext;

		private TestCaseId testCaseId = TestCaseId.GetInProcessTestCaseId();

		private List<Exception> exceptionHistory;

		private StopwatchStamp executionStart;

		private bool diagnosticDumped;

		private readonly uint instanceIdentifier;

		internal class ExecutionTracker<TOperationData> where TOperationData : class, IExecutionTrackingData<TOperationData>, new()
		{
			public bool HasDataToLog
			{
				get
				{
					return this.executionEntries != null && this.executionEntries.Count != 0;
				}
			}

			internal ConcurrentDictionary<ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey, TOperationData> ExecutionEntries
			{
				get
				{
					return this.executionEntries;
				}
			}

			public void Reset()
			{
				if (this.executionEntries != null)
				{
					this.executionEntries.Clear();
				}
			}

			public int GetTotalCount()
			{
				int num = 0;
				if (this.executionEntries != null)
				{
					foreach (KeyValuePair<ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey, TOperationData> keyValuePair in this.executionEntries)
					{
						int num2 = num;
						TOperationData value = keyValuePair.Value;
						num = num2 + value.Count;
					}
				}
				return num;
			}

			public TimeSpan GetTotalTime()
			{
				TimeSpan timeSpan = TimeSpan.Zero;
				if (this.executionEntries != null)
				{
					foreach (KeyValuePair<ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey, TOperationData> keyValuePair in this.executionEntries)
					{
						TimeSpan t = timeSpan;
						TOperationData value = keyValuePair.Value;
						timeSpan = t + value.TotalTime;
					}
				}
				return timeSpan;
			}

			public TOperationData GetAggregatedOperationData()
			{
				return this.GetAggregatedOperationData<object>((ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey key, object context) => true, null);
			}

			public void FormatData(TraceContentBuilder cb, int indentLevel, string title, bool includeDetails)
			{
				if (!this.HasDataToLog)
				{
					return;
				}
				ExecutionDiagnostics.FormatLine(cb, indentLevel, title);
				foreach (KeyValuePair<ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey, TOperationData> keyValuePair in this.executionEntries)
				{
					this.FormatEntry(cb, indentLevel, keyValuePair.Key, keyValuePair.Value);
					if (includeDetails)
					{
						TOperationData value = keyValuePair.Value;
						value.AppendDetailsToTraceContentBuilder(cb, indentLevel + 2);
					}
				}
			}

			public TOperationData RecordOperation(IOperationExecutionTrackable operation)
			{
				if (this.executionEntries == null)
				{
					this.executionEntries = new ConcurrentDictionary<ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey, TOperationData>(20, 100);
				}
				ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey key = new ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey(operation.GetTrackingKey());
				TOperationData toperationData;
				if (!this.executionEntries.TryGetValue(key, out toperationData))
				{
					toperationData = Activator.CreateInstance<TOperationData>();
					this.executionEntries.GetOrAdd(key, toperationData);
				}
				return toperationData;
			}

			public override int GetHashCode()
			{
				int num = 0;
				if (this.executionEntries != null)
				{
					foreach (ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey executionEntryKey in this.executionEntries.Keys)
					{
						num ^= executionEntryKey.GetSimpleHashCode();
					}
				}
				return num;
			}

			internal void FormatEntry(TraceContentBuilder cb, int indentLevel, ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey key, TOperationData value)
			{
				cb.Indent(indentLevel + 1);
				value.AppendToTraceContentBuilder(cb);
				cb.Append(", ");
				cb.Append(value.Count);
				cb.Append(", ");
				cb.Append(key.ToString());
				cb.AppendLine();
			}

			internal TOperationData GetAggregatedOperationData<TContext>(Func<ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey, TContext, bool> predicate, TContext context)
			{
				TOperationData result = Activator.CreateInstance<TOperationData>();
				if (this.executionEntries != null)
				{
					foreach (KeyValuePair<ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey, TOperationData> keyValuePair in this.executionEntries)
					{
						if (predicate(keyValuePair.Key, context))
						{
							result.Aggregate(keyValuePair.Value);
						}
					}
				}
				return result;
			}

			private ConcurrentDictionary<ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey, TOperationData> executionEntries;

			internal struct ExecutionEntryKey : IEquatable<ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey>
			{
				internal ExecutionEntryKey(IOperationExecutionTrackingKey trackingKey)
				{
					this.trackingKey = trackingKey;
				}

				internal IOperationExecutionTrackingKey TrackingKey
				{
					get
					{
						return this.trackingKey;
					}
				}

				public override string ToString()
				{
					return this.trackingKey.TrackingKeyToString();
				}

				public override bool Equals(object obj)
				{
					return obj is ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey && this.Equals((ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey)obj);
				}

				public override int GetHashCode()
				{
					return this.trackingKey.GetTrackingKeyHashValue();
				}

				public int GetSimpleHashCode()
				{
					return this.trackingKey.GetSimpleHashValue();
				}

				public bool Equals(ExecutionDiagnostics.ExecutionTracker<TOperationData>.ExecutionEntryKey other)
				{
					return this.trackingKey.IsTrackingKeyEqualTo(other.trackingKey);
				}

				private IOperationExecutionTrackingKey trackingKey;
			}
		}

		public enum OperationSource : byte
		{
			Mapi,
			AdminRpc,
			LogicalIndexCleanup,
			MailboxTask,
			MailboxCleanup,
			MailboxQuarantine,
			MapiTimedEvent,
			MailboxMaintenance,
			OnlineIntegrityCheck,
			LogicalIndexMaintenanceTableTask,
			SearchFolderAgeOut,
			SubobjectsCleanup,
			PerUserCacheFlush,
			SimpleQueryTarget
		}

		public struct HeavyClientActivitySummary
		{
			public Guid DatabaseGuid;

			public Guid MailboxGuid;

			public string ClientType;

			public string OperationSource;

			public string Activity;

			public uint TotalRpcCalls;

			public uint TotalRops;

			public Guid CorrelationId;
		}

		public struct LongOperationSummary
		{
			public Guid DatabaseGuid;

			public Guid MailboxGuid;

			public string ClientType;

			public string OperationSource;

			public string OperationType;

			public string OperationName;

			public string OperationDetail;

			public long ChunkElapsedTime;

			public long InteractionTotal;

			public long TimeInDatabase;

			public int PagesPreread;

			public int PagesRead;

			public int PagesDirtied;

			public int LogBytesWritten;

			public int SortOrderCount;

			public int NumberOfPlansExecuted;

			public long PlansExecutionTime;

			public int NumberOfDirectoryOperations;

			public long DirectoryOperationsTime;

			public int NumberOfLocksAttempted;

			public int NumberOfLocksSucceeded;

			public long LocksWaitTime;

			public bool IsLongOperation;

			public bool IsResourceIntensive;

			public bool IsContested;

			public string ClientProtocol;

			public string ClientComponent;

			public string ClientAction;

			public Guid CorrelationId;

			public string BuildNumber;

			public long TimeInCpuKernel;

			public long TimeInCpuUser;

			public int HashCode;
		}

		public class DirectoryTrackingData : IExecutionTrackingData<ExecutionDiagnostics.DirectoryTrackingData>
		{
			public TimeSpan ExecutionTime { get; set; }

			public int Count { get; set; }

			public TimeSpan TotalTime
			{
				get
				{
					return this.ExecutionTime;
				}
			}

			public void Aggregate(ExecutionDiagnostics.DirectoryTrackingData dataToAggregate)
			{
				this.ExecutionTime += dataToAggregate.ExecutionTime;
				this.Count += dataToAggregate.Count;
			}

			public void Reset()
			{
				this.Count = 0;
				this.ExecutionTime = TimeSpan.Zero;
			}

			public void AppendToTraceContentBuilder(TraceContentBuilder cb)
			{
				cb.Append(((long)this.ExecutionTime.TotalMicroseconds()).ToString("N0", CultureInfo.InvariantCulture));
				cb.Append(" us");
			}

			public void AppendDetailsToTraceContentBuilder(TraceContentBuilder cb, int indentLevel)
			{
			}
		}

		public interface IExecutionDiagnosticsStatistics
		{
			TimeSpan ElapsedTime { get; }

			TimeSpan MaximumChunkTime { get; }

			TimeSpan FastWaitTime { get; }

			TimeSpan CpuKernelTime { get; }

			TimeSpan CpuUserTime { get; }

			uint Count { get; }

			DatabaseConnectionStatistics DatabaseCollector { get; }

			int LockCount { get; }

			TimeSpan LockTotalTime { get; }

			int DirectoryCount { get; }

			TimeSpan DirectoryTotalTime { get; }
		}

		public class ChunkStatisticsContainer : ExecutionDiagnostics.IExecutionDiagnosticsStatistics
		{
			private ChunkStatisticsContainer()
			{
			}

			public TimeSpan ElapsedTime
			{
				get
				{
					return this.elapsedTime;
				}
			}

			public TimeSpan MaximumChunkTime
			{
				get
				{
					return this.elapsedTime;
				}
			}

			public TimeSpan FastWaitTime
			{
				get
				{
					return this.fastWaitTime;
				}
				set
				{
					this.fastWaitTime = value;
				}
			}

			public TimeSpan CpuKernelTime
			{
				get
				{
					return this.cpuKernelTime;
				}
			}

			public TimeSpan CpuUserTime
			{
				get
				{
					return this.cpuUserTime;
				}
			}

			public DatabaseConnectionStatistics DatabaseCollector
			{
				get
				{
					return this.databaseCollector;
				}
			}

			public int LockCount
			{
				get
				{
					return this.lockCollector.Count;
				}
			}

			public TimeSpan LockTotalTime
			{
				get
				{
					return this.lockCollector.TotalTime;
				}
			}

			public int DirectoryCount
			{
				get
				{
					return this.directoryCollector.Count;
				}
			}

			public TimeSpan DirectoryTotalTime
			{
				get
				{
					return this.directoryCollector.TotalTime;
				}
			}

			public uint Count
			{
				get
				{
					return 1U;
				}
			}

			public bool Started
			{
				get
				{
					return this.started;
				}
			}

			internal ExecutionDiagnostics.ExecutionTracker<DatabaseOperationStatistics> DatabaseTracker
			{
				get
				{
					return this.databaseTracker;
				}
			}

			internal ExecutionDiagnostics.ExecutionTracker<LockAcquisitionTracker.Data> LockTracker
			{
				get
				{
					return this.lockTracker;
				}
			}

			internal ExecutionDiagnostics.ExecutionTracker<ExecutionDiagnostics.DirectoryTrackingData> DirectoryTracker
			{
				get
				{
					return this.directoryTracker;
				}
			}

			public static ExecutionDiagnostics.ChunkStatisticsContainer Create()
			{
				return new ExecutionDiagnostics.ChunkStatisticsContainer
				{
					databaseTracker = new ExecutionDiagnostics.ExecutionTracker<DatabaseOperationStatistics>(),
					databaseCollector = new DatabaseConnectionStatistics(),
					lockTracker = new ExecutionDiagnostics.ExecutionTracker<LockAcquisitionTracker.Data>(),
					lockCollector = new LockAcquisitionTracker.Data(),
					directoryTracker = new ExecutionDiagnostics.ExecutionTracker<ExecutionDiagnostics.DirectoryTrackingData>(),
					directoryCollector = new ExecutionDiagnostics.DirectoryTrackingData()
				};
			}

			public void Start(TimeSpan startElapsedTime)
			{
				this.startTime = startElapsedTime;
				this.cpuSuccess = ThreadTimes.GetFromCurrentThread(out this.cpuKernelStart, out this.cpuUserStart);
				this.started = true;
			}

			public void Stop(TimeSpan stopElapsedTime)
			{
				TimeSpan zero = TimeSpan.Zero;
				TimeSpan zero2 = TimeSpan.Zero;
				this.elapsedTime = stopElapsedTime - this.startTime;
				this.started = false;
				if (this.cpuSuccess && ThreadTimes.GetFromCurrentThread(out zero, out zero2))
				{
					this.cpuKernelTime = zero - this.cpuKernelStart;
					this.cpuUserTime = zero2 - this.cpuUserStart;
				}
				this.lockCollector.Aggregate(this.lockTracker.GetAggregatedOperationData());
				this.directoryCollector.Aggregate(this.directoryTracker.GetAggregatedOperationData());
			}

			public void Reset()
			{
				this.startTime = TimeSpan.Zero;
				this.elapsedTime = TimeSpan.Zero;
				this.fastWaitTime = TimeSpan.Zero;
				this.cpuKernelTime = TimeSpan.Zero;
				this.cpuUserTime = TimeSpan.Zero;
				this.cpuSuccess = false;
				this.cpuKernelStart = TimeSpan.Zero;
				this.cpuUserStart = TimeSpan.Zero;
				this.databaseTracker.Reset();
				this.databaseCollector.Reset();
				this.lockTracker.Reset();
				this.lockCollector.Reset();
				this.directoryTracker.Reset();
				this.directoryCollector.Reset();
			}

			private TimeSpan startTime;

			private TimeSpan elapsedTime;

			private TimeSpan fastWaitTime;

			private TimeSpan cpuKernelTime;

			private TimeSpan cpuUserTime;

			private bool cpuSuccess;

			private TimeSpan cpuKernelStart;

			private TimeSpan cpuUserStart;

			private ExecutionDiagnostics.ExecutionTracker<DatabaseOperationStatistics> databaseTracker;

			private DatabaseConnectionStatistics databaseCollector;

			private ExecutionDiagnostics.ExecutionTracker<LockAcquisitionTracker.Data> lockTracker;

			private LockAcquisitionTracker.Data lockCollector;

			private ExecutionDiagnostics.ExecutionTracker<ExecutionDiagnostics.DirectoryTrackingData> directoryTracker;

			private ExecutionDiagnostics.DirectoryTrackingData directoryCollector;

			private bool started;
		}

		public class OperationStatisticsContainer : ExecutionDiagnostics.IExecutionDiagnosticsStatistics
		{
			private OperationStatisticsContainer()
			{
			}

			public static ExecutionDiagnostics.OperationStatisticsContainer Create()
			{
				return new ExecutionDiagnostics.OperationStatisticsContainer
				{
					databaseCollector = new DatabaseConnectionStatistics()
				};
			}

			public void Reset()
			{
				this.elapsedTime = TimeSpan.Zero;
				this.fastWaitTime = TimeSpan.Zero;
				this.cpuKernelTime = TimeSpan.Zero;
				this.cpuUserTime = TimeSpan.Zero;
				this.counter = 0U;
				this.maximumChunkTime = TimeSpan.Zero;
				this.databaseCollector.Reset();
				this.lockCount = 0;
				this.lockTotalTime = TimeSpan.Zero;
				this.directoryCount = 0;
				this.directoryTotalTime = TimeSpan.Zero;
			}

			public void Aggregate(ExecutionDiagnostics.ChunkStatisticsContainer chunk)
			{
				this.elapsedTime += chunk.ElapsedTime;
				this.fastWaitTime += chunk.FastWaitTime;
				this.cpuKernelTime += chunk.CpuKernelTime;
				this.cpuUserTime += chunk.CpuUserTime;
				this.counter += chunk.Count;
				this.maximumChunkTime = TimeSpan.FromTicks(Math.Max(this.maximumChunkTime.Ticks, chunk.MaximumChunkTime.Ticks));
				this.databaseCollector.Aggregate(chunk.DatabaseCollector);
				this.lockCount += chunk.LockCount;
				this.lockTotalTime += chunk.LockTotalTime;
				this.directoryCount += chunk.DirectoryCount;
				this.directoryTotalTime += chunk.DirectoryTotalTime;
			}

			public TimeSpan ElapsedTime
			{
				get
				{
					return this.elapsedTime;
				}
			}

			public TimeSpan MaximumChunkTime
			{
				get
				{
					return this.maximumChunkTime;
				}
			}

			public TimeSpan FastWaitTime
			{
				get
				{
					return this.fastWaitTime;
				}
			}

			public TimeSpan CpuKernelTime
			{
				get
				{
					return this.cpuKernelTime;
				}
			}

			public TimeSpan CpuUserTime
			{
				get
				{
					return this.cpuUserTime;
				}
			}

			public uint Count
			{
				get
				{
					return this.counter;
				}
			}

			public DatabaseConnectionStatistics DatabaseCollector
			{
				get
				{
					return this.databaseCollector;
				}
			}

			public int LockCount
			{
				get
				{
					return this.lockCount;
				}
			}

			public TimeSpan LockTotalTime
			{
				get
				{
					return this.lockTotalTime;
				}
			}

			public int DirectoryCount
			{
				get
				{
					return this.directoryCount;
				}
			}

			public TimeSpan DirectoryTotalTime
			{
				get
				{
					return this.directoryTotalTime;
				}
			}

			private TimeSpan elapsedTime;

			private TimeSpan fastWaitTime;

			private TimeSpan cpuKernelTime;

			private TimeSpan cpuUserTime;

			private uint counter;

			private TimeSpan maximumChunkTime;

			private DatabaseConnectionStatistics databaseCollector;

			private int lockCount;

			private TimeSpan lockTotalTime;

			private int directoryCount;

			private TimeSpan directoryTotalTime;
		}

		public class RpcStatisticsContainer : ExecutionDiagnostics.IExecutionDiagnosticsStatistics
		{
			private RpcStatisticsContainer()
			{
			}

			public static ExecutionDiagnostics.RpcStatisticsContainer Create()
			{
				return new ExecutionDiagnostics.RpcStatisticsContainer
				{
					databaseCollector = new DatabaseConnectionStatistics()
				};
			}

			public void Reset()
			{
				this.elapsedTime = TimeSpan.Zero;
				this.fastWaitTime = TimeSpan.Zero;
				this.cpuKernelTime = TimeSpan.Zero;
				this.cpuUserTime = TimeSpan.Zero;
				this.counter = 0U;
				this.maximumChunkTime = TimeSpan.Zero;
				this.databaseCollector.Reset();
				this.lockCount = 0;
				this.lockTotalTime = TimeSpan.Zero;
				this.directoryCount = 0;
				this.directoryTotalTime = TimeSpan.Zero;
			}

			public void Aggregate(ExecutionDiagnostics.OperationStatisticsContainer operation)
			{
				this.elapsedTime += operation.ElapsedTime;
				this.fastWaitTime += operation.FastWaitTime;
				this.cpuKernelTime += operation.CpuKernelTime;
				this.cpuUserTime += operation.CpuUserTime;
				this.counter += operation.Count;
				this.maximumChunkTime = TimeSpan.FromTicks(Math.Max(this.maximumChunkTime.Ticks, operation.MaximumChunkTime.Ticks));
				this.databaseCollector.Aggregate(operation.DatabaseCollector);
				this.lockCount += operation.LockCount;
				this.lockTotalTime += operation.LockTotalTime;
				this.directoryCount += operation.DirectoryCount;
				this.directoryTotalTime += operation.DirectoryTotalTime;
			}

			public TimeSpan ElapsedTime
			{
				get
				{
					return this.elapsedTime;
				}
			}

			public TimeSpan MaximumChunkTime
			{
				get
				{
					return this.maximumChunkTime;
				}
			}

			public TimeSpan FastWaitTime
			{
				get
				{
					return this.fastWaitTime;
				}
			}

			public TimeSpan CpuKernelTime
			{
				get
				{
					return this.cpuKernelTime;
				}
			}

			public TimeSpan CpuUserTime
			{
				get
				{
					return this.cpuUserTime;
				}
			}

			public uint Count
			{
				get
				{
					return this.counter;
				}
			}

			public DatabaseConnectionStatistics DatabaseCollector
			{
				get
				{
					return this.databaseCollector;
				}
			}

			public int LockCount
			{
				get
				{
					return this.lockCount;
				}
			}

			public TimeSpan LockTotalTime
			{
				get
				{
					return this.lockTotalTime;
				}
			}

			public int DirectoryCount
			{
				get
				{
					return this.directoryCount;
				}
			}

			public TimeSpan DirectoryTotalTime
			{
				get
				{
					return this.directoryTotalTime;
				}
			}

			private TimeSpan elapsedTime;

			private TimeSpan fastWaitTime;

			private TimeSpan cpuKernelTime;

			private TimeSpan cpuUserTime;

			private uint counter;

			private TimeSpan maximumChunkTime;

			private DatabaseConnectionStatistics databaseCollector;

			private int lockCount;

			private TimeSpan lockTotalTime;

			private int directoryCount;

			private TimeSpan directoryTotalTime;
		}

		protected enum LockCategory
		{
			Mailbox,
			Component,
			Other
		}
	}
}
