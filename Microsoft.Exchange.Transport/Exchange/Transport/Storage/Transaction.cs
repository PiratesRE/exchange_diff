using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Windows8;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class Transaction : IDisposeTrackable, IDisposable
	{
		static Transaction()
		{
			Transaction.TransactionTracker = new OperationTracker<Transaction>(() => TimeSpan.FromTicks(Math.Max(TimeSpan.FromSeconds(1.0).Ticks, Transaction.TransactionTracker.PercentileQuery(99.0).Ticks * 2L)), new Action<Transaction, TimeSpan>(Transaction.LogLongRunningTransaction), TimeSpan.FromMilliseconds(100.0), TimeSpan.FromSeconds(10.0));
		}

		private static void LogLongRunningTransaction(Transaction transaction, TimeSpan duration)
		{
			if (ExTraceGlobals.StorageTracer.IsTraceEnabled(TraceType.WarningTrace))
			{
				ExTraceGlobals.StorageTracer.TraceWarning<TimeSpan, StackTrace>((long)transaction.GetHashCode(), "Long running transaction detected: Duration({0}), Stack: {1}", duration, new StackTrace(1, true));
			}
		}

		private Transaction(DataConnection connection)
		{
			this.connection = connection;
			this.connection.AddRef();
			this.disposeTracker = this.GetDisposeTracker();
		}

		internal static Transaction New(DataConnection connection)
		{
			Transaction transaction = new Transaction(connection);
			transaction.EnterTransaction();
			return transaction;
		}

		public event Action OnExitTransaction;

		public DataConnection Connection
		{
			get
			{
				return this.connection;
			}
		}

		public static XElement GetDiagnosticInfo(string argument)
		{
			if (argument.Equals("TransactionsOpen", StringComparison.InvariantCultureIgnoreCase))
			{
				return new XElement("TransactionsOpen", from runningOperation in Transaction.TransactionTracker.GetRunningOperations()
				select new XElement("Transaction", new object[]
				{
					new XElement("Duration", runningOperation.Item2),
					new XElement("StackTrace", runningOperation.Item3)
				}));
			}
			if (argument.StartsWith("TransactionStartTrace", StringComparison.InvariantCultureIgnoreCase))
			{
				Match match = Regex.Match(argument, "TransactionStartTrace\\((\\d+),(\\d+)\\)", RegexOptions.IgnoreCase);
				int val;
				int num;
				if (match.Success && int.TryParse(match.Groups[1].Value, out val) && int.TryParse(match.Groups[2].Value, out num))
				{
					Transaction.TransactionTracker.StartTracing(Math.Min(val, 10000), TimeSpan.FromMilliseconds((double)num));
					return new XElement("TransactionTrace", new XElement("Started", true));
				}
				return new XElement("TransactionTrace", new object[]
				{
					new XElement("Started", false),
					new XElement("Usage", "TransactionTrace([TransactionsToSample], [TransactionDurationThresholdMilliseconds])")
				});
			}
			else
			{
				if (argument.Equals("TransactionTrace", StringComparison.InvariantCultureIgnoreCase))
				{
					ICollection<OperationTracker<Transaction>.StackCounter> tracedStack = Transaction.TransactionTracker.TracedStack;
					XName name = "TransactionTrace";
					object[] array = new object[3];
					array[0] = new XElement("OperationCount", tracedStack.Sum((OperationTracker<Transaction>.StackCounter i) => i.Count));
					array[1] = new XElement("StackCount", tracedStack.Count);
					array[2] = from i in tracedStack
					select new XElement("Stack", new object[]
					{
						new XElement("OperationCount", i.Count),
						new XElement("StackTrace", i.StackTrace)
					});
					return new XElement(name, array);
				}
				if (!argument.StartsWith("TransactionPercentile", StringComparison.InvariantCultureIgnoreCase))
				{
					return null;
				}
				Match match2 = Regex.Match(argument, "TransactionPercentile\\((\\d+)\\)", RegexOptions.IgnoreCase);
				double num2;
				if (match2.Success && double.TryParse(match2.Groups[1].Value, out num2))
				{
					return new XElement("TransactionPercentile", new object[]
					{
						new XElement("Percentile", num2),
						new XElement("Duration", Transaction.TransactionTracker.PercentileQuery(num2))
					});
				}
				return new XElement("TransactionPercentile", new XElement("Usage", "TransactionPercentile([PercentileToQuery])"));
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<Transaction>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Commit()
		{
			this.Commit(TransactionCommitMode.MediumLatencyLazy);
		}

		public void Commit(TransactionCommitMode mode)
		{
			switch (mode)
			{
			case TransactionCommitMode.Lazy:
				this.Commit(CommitTransactionGrbit.LazyFlush, null);
				return;
			case TransactionCommitMode.ShortLatencyLazy:
				this.Commit(CommitTransactionGrbit.LazyFlush, new TimeSpan?(Transaction.ShortLazyTimeout));
				return;
			case TransactionCommitMode.MediumLatencyLazy:
				this.Commit(CommitTransactionGrbit.LazyFlush, new TimeSpan?(Transaction.MediumLazyTimeout));
				return;
			case TransactionCommitMode.Immediate:
				this.Commit(CommitTransactionGrbit.None, null);
				return;
			default:
				return;
			}
		}

		public void AsyncCommit(TimeSpan durableTimeout)
		{
			this.Commit(CommitTransactionGrbit.LazyFlush, new TimeSpan?(durableTimeout));
		}

		public IAsyncResult BeginAsyncCommit(TimeSpan durableTimeout, object asyncState, AsyncCallback callback)
		{
			AsyncResult result = new AsyncResult(callback, asyncState);
			Transaction.PerfCounters.TransactionAsyncCommitCount.Increment();
			Transaction.PerfCounters.TransactionAsyncCommitPendingCount.Increment();
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.connection.Source.RegisterAsyncCommitCallback(this.Commit(CommitTransactionGrbit.LazyFlush, new TimeSpan?(durableTimeout)), delegate
			{
				result.IsCompleted = true;
				Transaction.PerfCounters.TransactionAsyncCommitAveragePendingDuration.IncrementBy(stopwatch.ElapsedTicks);
				Transaction.PerfCounters.TransactionAsyncCommitAveragePendingDurationBase.Increment();
				Transaction.PerfCounters.TransactionAsyncCommitPendingCount.Decrement();
			});
			return result;
		}

		public void EndAsyncCommit(IAsyncResult asyncResult)
		{
			AsyncResult asyncResult2 = (AsyncResult)asyncResult;
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
			asyncResult2.End();
		}

		public void Abort()
		{
			this.ThrowIfTransactionNotRunning();
			try
			{
				Api.JetRollback(this.connection.Session, RollbackTransactionGrbit.None);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
			Transaction.PerfCounters.TransactionAbortCount.Increment();
			this.ExitTransaction();
		}

		public void Checkpoint()
		{
			this.Checkpoint(TransactionCommitMode.Lazy, 100);
		}

		public void Checkpoint(TransactionCommitMode mode, byte maxLoad = 100)
		{
			if (maxLoad == 0 || maxLoad > 100)
			{
				throw new ArgumentException("maxLoad has to be between 1 and 100", "maxLoad");
			}
			long elapsedMilliseconds = this.openStopwatch.ElapsedMilliseconds;
			TimeSpan pause = TimeSpan.FromMilliseconds((double)(elapsedMilliseconds * 100L / (long)((ulong)maxLoad) - elapsedMilliseconds));
			this.Checkpoint(mode, pause);
		}

		public void Checkpoint(TransactionCommitMode mode, TimeSpan pause)
		{
			this.Commit(mode);
			if (pause > TimeSpan.Zero)
			{
				Thread.Sleep(pause);
			}
			this.EnterTransaction();
		}

		public void RestartIfStale(byte maxLoad = 100)
		{
			this.operationsInCurrentTransaction++;
			if (this.openStopwatch.Elapsed > Transaction.StaleTransactionTimeout || this.operationsInCurrentTransaction > 500)
			{
				this.Checkpoint(TransactionCommitMode.Lazy, maxLoad);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			if (this.connection != null)
			{
				if (disposing && this.transactionRunning)
				{
					this.Abort();
				}
				this.connection.Release();
				this.connection = null;
			}
		}

		private JET_COMMIT_ID Commit(CommitTransactionGrbit grbit, TimeSpan? durableTimeout)
		{
			this.ThrowIfTransactionNotRunning();
			JET_COMMIT_ID result = null;
			if (durableTimeout != null || grbit == CommitTransactionGrbit.LazyFlush)
			{
				Transaction.PerfCounters.TransactionSoftCommitCount.Increment();
				Transaction.PerfCounters.TransactionSoftCommitPendingCount.Increment();
			}
			else
			{
				Transaction.PerfCounters.TransactionHardCommitCount.Increment();
				Transaction.PerfCounters.TransactionHardCommitPendingCount.Increment();
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				if (durableTimeout != null)
				{
					Windows8Api.JetCommitTransaction2(this.connection.Session, grbit, durableTimeout.Value, out result);
				}
				else
				{
					Api.JetCommitTransaction(this.connection.Session, grbit);
				}
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
			stopwatch.Stop();
			if (durableTimeout != null || grbit == CommitTransactionGrbit.LazyFlush)
			{
				Transaction.PerfCounters.TransactionSoftCommitAveragePendingDuration.IncrementBy(stopwatch.ElapsedTicks);
				Transaction.PerfCounters.TransactionSoftCommitAveragePendingDurationBase.Increment();
				Transaction.PerfCounters.TransactionSoftCommitPendingCount.Decrement();
			}
			else
			{
				Transaction.PerfCounters.TransactionHardCommitAveragePendingDuration.IncrementBy(stopwatch.ElapsedTicks);
				Transaction.PerfCounters.TransactionHardCommitAveragePendingDurationBase.Increment();
				Transaction.PerfCounters.TransactionHardCommitPendingCount.Decrement();
			}
			this.ExitTransaction();
			return result;
		}

		private void EnterTransaction()
		{
			this.connection.TrackStartTransaction();
			try
			{
				Api.JetBeginTransaction(this.Connection.Session);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this.connection.Source))
				{
					throw;
				}
			}
			this.operationsInCurrentTransaction = 0;
			this.openStopwatch.Restart();
			this.transactionRunning = true;
			Transaction.TransactionTracker.Enter(this);
			Transaction.PerfCounters.TransactionPendingCount.Increment();
			Transaction.PerfCounters.TransactionCount.Increment();
		}

		private void ExitTransaction()
		{
			this.connection.TrackRemoveTransaction();
			this.transactionRunning = false;
			this.openStopwatch.Stop();
			TimeSpan timeSpan = Transaction.TransactionTracker.Exit(this);
			Transaction.PerfCounters.TransactionPendingCount.Decrement();
			Transaction.PerfCounters.TransactionAveragePendingDuration.IncrementBy(timeSpan.Ticks);
			Transaction.PerfCounters.TransactionAveragePendingDurationBase.Increment();
			if (Transaction.TransactionTracker.TotalOperationCount % 10L == 0L)
			{
				Transaction.PerfCounters.TransactionPending99PercentileDuration.RawValue = (long)Transaction.TransactionTracker.PercentileQuery(99.0).TotalSeconds;
			}
			if (this.OnExitTransaction != null)
			{
				this.OnExitTransaction();
			}
		}

		private void ThrowIfTransactionNotRunning()
		{
			if (!this.transactionRunning)
			{
				throw new InvalidOperationException(Strings.NotInTransaction);
			}
		}

		private const int MaxOperationsInTransaction = 500;

		private static readonly OperationTracker<Transaction> TransactionTracker;

		internal static readonly DatabasePerfCountersInstance PerfCounters = DatabasePerfCounters.GetInstance("other");

		public static TimeSpan StaleTransactionTimeout = TransportAppConfig.GetConfigTimeSpan("StaleTransactionTimeout", TimeSpan.FromSeconds(1.0), TimeSpan.FromMinutes(30.0), TimeSpan.FromSeconds(30.0));

		public static TimeSpan ShortLazyTimeout = TransportAppConfig.GetConfigTimeSpan("ShortLazyTransactionTimeout", TimeSpan.FromMilliseconds(10.0), TimeSpan.FromMinutes(30.0), TimeSpan.FromMilliseconds(250.0));

		public static TimeSpan MediumLazyTimeout = TransportAppConfig.GetConfigTimeSpan("MediumLazyTransactionTimeout", TimeSpan.FromMilliseconds(10.0), TimeSpan.FromMinutes(30.0), TimeSpan.FromSeconds(3.0));

		private readonly DisposeTracker disposeTracker;

		private readonly Stopwatch openStopwatch = new Stopwatch();

		private DataConnection connection;

		private bool transactionRunning;

		private int operationsInCurrentTransaction;
	}
}
