using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class Connection : DisposableBase, IExecutionContext, IConnectionProvider
	{
		protected Connection(IDatabaseExecutionContext outerExecutionContext, Database database, string identification)
		{
			this.outerExecutionContext = outerExecutionContext;
			this.database = database;
			this.identification = identification;
			this.owningThread = Thread.CurrentThread;
			Factory.GetDatabaseThreadStats(out this.lastCapturedThreadStats);
			this.connectionStatistics = this.RecordOperation<DatabaseConnectionStatistics>(NullTrackableOperation.Instance);
			if (this.connectionStatistics == null)
			{
				this.connectionStatistics = new DatabaseConnectionStatistics();
			}
			this.connectionStatistics.Count++;
			if (ExTraceGlobals.DirtyObjectsTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				this.dirtyObjects = new List<DataRow>(20);
			}
		}

		public int NumberOfDirtyObjects
		{
			get
			{
				return this.numberOfDirtyObjects;
			}
		}

		public abstract bool TransactionStarted { get; }

		public long TransactionTimeStamp
		{
			get
			{
				return this.transactionTimeStamp;
			}
		}

		public abstract int TransactionId { get; }

		public Database Database
		{
			get
			{
				return this.database;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.valid;
			}
			protected set
			{
				this.valid = value;
			}
		}

		public IDatabaseExecutionContext OuterExecutionContext
		{
			get
			{
				return this.outerExecutionContext;
			}
		}

		public bool NonFatalDuplicateKey { get; set; }

		internal RowStats RowStats
		{
			get
			{
				return this.connectionStatistics.RowStats;
			}
		}

		internal Thread OwningThread
		{
			get
			{
				return this.owningThread;
			}
			set
			{
				this.owningThread = value;
			}
		}

		public static void Initialize()
		{
			Connection.databaseOperationTimeoutDefinition = new ThreadManager.TimeoutDefinition(ConfigurationSchema.DatabaseOperationTimeout.Value, new Action<ThreadManager.ThreadInfo>(Connection.CrashOnTimeout));
		}

		public Connection GetConnection()
		{
			return this;
		}

		private static void CrashOnTimeout(ThreadManager.ThreadInfo threadInfo)
		{
			throw new InvalidOperationException(string.Format("Possible hang detected. Operation: {0}. Client: {2}. MailboxGuid: {3}", threadInfo.MethodName, threadInfo.Client, threadInfo.MailboxGuid));
		}

		protected abstract void OnCommit(byte[] logTransactionInformation);

		public void BeginTransactionIfNeeded()
		{
			this.BeginTransactionIfNeeded(Connection.TransactionOption.NeedTransaction);
		}

		public void OnBeforeTableAccess(Connection.OperationType operationType, Table table, IList<object> partitionValues)
		{
			if (this.outerExecutionContext != null)
			{
				this.outerExecutionContext.OnBeforeTableAccess(operationType, table, partitionValues);
			}
		}

		internal abstract void BeginTransactionIfNeeded(Connection.TransactionOption transactionOption);

		public void Commit()
		{
			this.Commit(null);
		}

		public void Commit(byte[] logTransactionInformation)
		{
			if (ExTraceGlobals.DbInteractionSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					stringBuilder.Append("cn:[");
					stringBuilder.Append(this.GetHashCode());
					stringBuilder.Append("] ");
				}
				stringBuilder.Append("Commit Transaction");
				ExTraceGlobals.DbInteractionSummaryTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			if (this.numberOfDirtyObjects != 0)
			{
				if (this.dirtyObjects != null)
				{
					foreach (DataRow dataRow in this.dirtyObjects)
					{
						StringBuilder stringBuilder2 = new StringBuilder(200);
						stringBuilder2.Append("Connection:Commit(): This data row has not been saved: ");
						dataRow.AppendDirtyTrackingInfoToString(stringBuilder2);
						ExTraceGlobals.DirtyObjectsTracer.TraceDebug(0L, stringBuilder2.ToString());
					}
				}
				Globals.AssertRetail(false, "Not all dirty objects have been saved!");
			}
			this.OnCommit(logTransactionInformation);
			if (!this.SkipDatabaseLogsFlush && ConfigurationSchema.LazyTransactionCommitTimeout.Value == TimeSpan.Zero)
			{
				this.hasCommittedDataRequiringFlush = true;
			}
			this.transactionTimeStamp = Interlocked.Increment(ref Connection.currentTransactionTimeStamp);
		}

		protected abstract void OnAbort(byte[] logTransactionInformation);

		public void Abort()
		{
			this.Abort(null);
		}

		public void Abort(byte[] logTransactionInformation)
		{
			if (ExTraceGlobals.DbInteractionSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					stringBuilder.Append("cn:[");
					stringBuilder.Append(this.GetHashCode());
					stringBuilder.Append("] ");
				}
				stringBuilder.Append("Abort Transaction");
				ExTraceGlobals.DbInteractionSummaryTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			this.OnAbort(logTransactionInformation);
			this.transactionTimeStamp = Interlocked.Increment(ref Connection.currentTransactionTimeStamp);
		}

		public abstract void FlushDatabaseLogs(bool force);

		public TOperationData RecordOperation<TOperationData>(IOperationExecutionTrackable operation) where TOperationData : class, IExecutionTrackingData<TOperationData>, new()
		{
			if (operation == null)
			{
				return default(TOperationData);
			}
			if (this.outerExecutionContext != null)
			{
				return this.outerExecutionContext.RecordOperation<TOperationData>(operation);
			}
			return default(TOperationData);
		}

		public void OnExceptionCatch(Exception exception)
		{
			this.Diagnostics.OnExceptionCatch(exception);
		}

		public void OnDatabaseFailure(bool isCriticalFailure, LID lid)
		{
			if (this.outerExecutionContext != null)
			{
				this.outerExecutionContext.OnDatabaseFailure(isCriticalFailure, lid);
			}
		}

		public IExecutionDiagnostics Diagnostics
		{
			get
			{
				if (this.outerExecutionContext != null)
				{
					return this.outerExecutionContext.Diagnostics;
				}
				return NullExecutionDiagnostics.Instance;
			}
		}

		public bool IsMailboxOperationStarted
		{
			get
			{
				return this.outerExecutionContext != null && this.outerExecutionContext.IsMailboxOperationStarted;
			}
		}

		public bool SkipDatabaseLogsFlush
		{
			get
			{
				return this.outerExecutionContext != null && this.outerExecutionContext.SkipDatabaseLogsFlush;
			}
		}

		internal void CountStatement(Connection.OperationType operationType)
		{
			if (this.database != null && this.database.PerfInstance != null)
			{
				switch (operationType)
				{
				case Connection.OperationType.Query:
					this.database.PerfInstance.NumberOfQueriesPerSec.Increment();
					return;
				case Connection.OperationType.Insert:
					this.database.PerfInstance.NumberOfInsertsPerSec.Increment();
					return;
				case Connection.OperationType.Update:
					this.database.PerfInstance.NumberOfUpdatesPerSec.Increment();
					return;
				case Connection.OperationType.Delete:
					this.database.PerfInstance.NumberOfDeletesPerSec.Increment();
					return;
				case Connection.OperationType.CreateTable:
				case Connection.OperationType.DeleteTable:
				case Connection.OperationType.Other:
					this.database.PerfInstance.NumberOfOthersPerSec.Increment();
					break;
				default:
					return;
				}
			}
		}

		public void ReleaseThread()
		{
			this.owningThread = null;
		}

		public void ForceValid()
		{
			this.IsValid = true;
		}

		internal void AddDirtyObject(DataRow dataRow)
		{
			if (this.dirtyObjects != null)
			{
				this.dirtyObjects.Add(dataRow);
			}
			this.numberOfDirtyObjects++;
		}

		internal void CleanDirtyObject(DataRow dataRow)
		{
			if (this.dirtyObjects != null)
			{
				this.dirtyObjects.Remove(dataRow);
			}
			this.numberOfDirtyObjects--;
		}

		internal void Suspend()
		{
			this.CaptureStatisticsChangeForOperation();
			if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("cn:[");
				stringBuilder.Append(this.GetHashCode());
				stringBuilder.Append("] ");
				stringBuilder.Append("Connection Suspended");
				ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		internal void Resume()
		{
			JET_THREADSTATS jet_THREADSTATS;
			Factory.GetDatabaseThreadStats(out jet_THREADSTATS);
			this.lastCapturedThreadStats = jet_THREADSTATS;
			if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("cn:[");
				stringBuilder.Append(this.GetHashCode());
				stringBuilder.Append("] ");
				stringBuilder.Append("Connection Resumed");
				ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		internal void DumpRowStats()
		{
			this.connectionStatistics.DumpStatistics(this.database);
			this.dumpedTimeInDatabase = this.connectionStatistics.TimeInDatabase;
		}

		internal Connection.DatabaseExecutionTrackingFrame TrackDbOperationExecution(DataAccessOperator operation)
		{
			return new Connection.DatabaseExecutionTrackingFrame(this, operation);
		}

		internal Connection.TimeInDatabaseTrackingFrame TrackTimeInDatabase()
		{
			return new Connection.TimeInDatabaseTrackingFrame(this);
		}

		[Conditional("DEBUG")]
		internal void AssertTrackingTimeInDatabase()
		{
		}

		internal DatabaseOperationStatistics SetCurrentOperationStatisticsObject(DatabaseOperationStatistics newOperationData)
		{
			if (object.ReferenceEquals(this.currentOperationStatistics, newOperationData))
			{
				return newOperationData;
			}
			this.CaptureStatisticsChangeForOperation();
			DatabaseOperationStatistics result = this.currentOperationStatistics;
			this.currentOperationStatistics = newOperationData;
			return result;
		}

		public void IncrementRowStatsCounter(Table table, RowStatsCounterType counterIndex)
		{
			TableClass tableClass = (table != null) ? table.TableClass : TableClass.Temp;
			this.connectionStatistics.RowStats.IncrementCount(tableClass, counterIndex);
			if (this.currentOperationStatistics != null)
			{
				this.currentOperationStatistics.SmallRowStats.IncrementCount(tableClass, counterIndex);
			}
		}

		public void AddRowStatsCounter(Table table, RowStatsCounterType counterIndex, int count)
		{
			TableClass tableClass = (table != null) ? table.TableClass : TableClass.Temp;
			this.connectionStatistics.RowStats.AddCount(tableClass, counterIndex, count);
			if (this.currentOperationStatistics != null)
			{
				this.currentOperationStatistics.SmallRowStats.AddCount(tableClass, counterIndex, count);
			}
		}

		private void AddTimeInDatabase(TimeSpan time)
		{
			this.timeInDatabaseChanged = true;
			if (time.Ticks == 0L)
			{
				time = TimeSpan.FromTicks(1L);
			}
			this.connectionStatistics.TimeInDatabase += time;
			if (this.connectionStatistics.TimeInDatabase - this.dumpedTimeInDatabase > Connection.statisticsDumpInterval)
			{
				this.DumpRowStats();
			}
			if (this.currentOperationStatistics != null)
			{
				this.currentOperationStatistics.TimeInDatabase += time;
			}
		}

		public void IncrementOffPageBlobHits()
		{
			this.connectionStatistics.OffPageBlobHits++;
			if (this.currentOperationStatistics != null)
			{
				this.currentOperationStatistics.OffPageBlobHits++;
			}
		}

		internal void CaptureStatisticsChangeForOperation()
		{
			if (this.timeInDatabaseChanged)
			{
				this.DoCaptureStatisticsChangeForOperation();
				this.timeInDatabaseChanged = false;
			}
		}

		private void DoCaptureStatisticsChangeForOperation()
		{
			JET_THREADSTATS jet_THREADSTATS;
			Factory.GetDatabaseThreadStats(out jet_THREADSTATS);
			JET_THREADSTATS jet_THREADSTATS2 = jet_THREADSTATS - this.lastCapturedThreadStats;
			this.connectionStatistics.ThreadStats += jet_THREADSTATS2;
			if (this.currentOperationStatistics != null)
			{
				this.currentOperationStatistics.ThreadStats += jet_THREADSTATS2;
			}
			this.lastCapturedThreadStats = jet_THREADSTATS;
		}

		internal DatabaseOperationStatistics RecordOperationImpl(DataAccessOperator operation)
		{
			DatabaseOperationStatistics databaseOperationStatistics = this.RecordOperation<DatabaseOperationStatistics>(operation);
			if (databaseOperationStatistics != null)
			{
				databaseOperationStatistics.Count++;
				if (databaseOperationStatistics.Planner == null)
				{
					databaseOperationStatistics.Planner = operation.GetExecutionPlanner();
				}
			}
			return databaseOperationStatistics;
		}

		protected virtual void Close()
		{
			this.DumpRowStats();
		}

		private static readonly TimeSpan statisticsDumpInterval = TimeSpan.FromSeconds(1.0);

		protected static ThreadManager.TimeoutDefinition databaseOperationTimeoutDefinition;

		private Database database;

		private string identification;

		private Thread owningThread;

		private bool valid;

		private int numberOfDirtyObjects;

		private List<DataRow> dirtyObjects;

		private IDatabaseExecutionContext outerExecutionContext;

		private DatabaseConnectionStatistics connectionStatistics;

		private DatabaseOperationStatistics currentOperationStatistics;

		private JET_THREADSTATS lastCapturedThreadStats;

		private bool timeInDatabaseChanged;

		private TimeSpan dumpedTimeInDatabase;

		protected bool hasCommittedDataRequiringFlush;

		protected long transactionTimeStamp;

		protected static long currentTransactionTimeStamp;

		private bool trackingTimeInDatabase;

		public enum OperationType
		{
			Query = 1,
			Insert,
			Update,
			Delete,
			CreateTable,
			DeleteTable,
			Other
		}

		public enum TransactionOption
		{
			NeedTransaction = 1,
			DontNeedTransaction,
			NoTransaction
		}

		internal struct DatabaseExecutionTrackingFrame : IDisposable
		{
			internal DatabaseExecutionTrackingFrame(Connection connection, DataAccessOperator operation)
			{
				this.connection = connection;
				DatabaseOperationStatistics currentOperationStatisticsObject = this.connection.RecordOperationImpl(operation);
				this.savedOperationData = this.connection.SetCurrentOperationStatisticsObject(currentOperationStatisticsObject);
			}

			public void Dispose()
			{
				this.connection.SetCurrentOperationStatisticsObject(this.savedOperationData);
			}

			private readonly Connection connection;

			private DatabaseOperationStatistics savedOperationData;
		}

		internal struct TimeInDatabaseTrackingFrame : IDisposable
		{
			internal TimeInDatabaseTrackingFrame(Connection connection)
			{
				if (connection.trackingTimeInDatabase)
				{
					this.connection = null;
					this.startTimeStamp = default(StopwatchStamp);
					return;
				}
				this.connection = connection;
				this.startTimeStamp = StopwatchStamp.GetStamp();
				connection.trackingTimeInDatabase = true;
			}

			public void Dispose()
			{
				if (this.connection != null)
				{
					this.connection.AddTimeInDatabase(this.startTimeStamp.ElapsedTime);
					this.connection.trackingTimeInDatabase = false;
				}
			}

			private readonly Connection connection;

			private StopwatchStamp startTimeStamp;
		}
	}
}
