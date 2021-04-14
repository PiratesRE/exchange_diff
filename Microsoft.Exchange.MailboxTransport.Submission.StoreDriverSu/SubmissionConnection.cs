using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging.ConnectionLog;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal sealed class SubmissionConnection
	{
		internal SubmissionConnection(string key, SubmissionConnectionPool pool, string server, string database)
		{
			this.key = key;
			this.pool = pool;
			this.server = server;
			this.database = database;
			this.id = SessionId.GetNextSessionId();
			TraceHelper.SubmissionConnectionTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnection.ctor: Thread {0}, Created: {1}.", Thread.CurrentThread.ManagedThreadId, this.ToString());
		}

		public ulong Id
		{
			get
			{
				return this.id;
			}
		}

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		public bool IsInUse
		{
			get
			{
				return this.referenceCount > 0;
			}
		}

		public int Failures
		{
			get
			{
				return this.failures;
			}
		}

		public bool HasReachedSubmissionLimit
		{
			get
			{
				return this.totalSubmissions >= 20L;
			}
		}

		public bool TimeoutElapsed
		{
			get
			{
				return ExDateTime.UtcNow > this.timeoutDeadline;
			}
		}

		public void SubmissionSuccessful(long size, int recipients)
		{
			Interlocked.Increment(ref this.successfulSubmissions);
			Interlocked.Add(ref this.bytes, size);
			Interlocked.Add(ref this.recipients, (long)recipients);
			TraceHelper.SubmissionConnectionTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnection.SubmissionSuccessful: Thread {0}, Connection: {1}.", Thread.CurrentThread.ManagedThreadId, this.ToString());
			this.ReleaseReference();
		}

		public void SubmissionAborted(string reason)
		{
			ConnectionLog.MapiSubmissionAborted(this.id, this.database, reason);
			TraceHelper.SubmissionConnectionTracer.TracePass<int, string, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnection.SubmissionAborted: Thread {0}, Connection: {1}, Reason: {1}", Thread.CurrentThread.ManagedThreadId, this.ToString(), reason);
			this.ReleaseReference();
		}

		public void SubmissionFailed(string description)
		{
			Interlocked.Increment(ref this.failures);
			ConnectionLog.MapiSubmissionFailed(this.id, this.database, description);
			TraceHelper.SubmissionConnectionTracer.TracePass<int, string, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnection.SubmissionFailed: Thread {0}, Connection: {1}, Description: {2}", Thread.CurrentThread.ManagedThreadId, this.ToString(), description);
			this.ReleaseReference();
		}

		public override string ToString()
		{
			return string.Format("ID={0}, Key={1}, references={2}, totalSubmissions={3}, successfulSubmissions={4}, failures={5}, recipients={6}, bytes={7}", new object[]
			{
				this.id,
				this.key,
				this.referenceCount,
				this.totalSubmissions,
				this.successfulSubmissions,
				this.failures,
				this.recipients,
				this.bytes
			});
		}

		public void StartConnection()
		{
			ConnectionLog.MapiSubmissionConnectionStart(this.id, this.database, this.server);
			TraceHelper.SubmissionConnectionTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnection.StartConnection: Thread {0}, Connection: {1}.", Thread.CurrentThread.ManagedThreadId, this.ToString());
		}

		public void TimeoutExpired()
		{
			this.LogConnectionStopped(true);
			TraceHelper.SubmissionConnectionTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnection.TimeoutExpired: Thread {0}, Connection: {1}.", Thread.CurrentThread.ManagedThreadId, this.ToString());
		}

		public void SubmissionStarted()
		{
			Interlocked.Increment(ref this.referenceCount);
			Interlocked.Increment(ref this.totalSubmissions);
			this.timeoutDeadline = ExDateTime.UtcNow.Add(SubmissionConnection.timeoutInterval);
			TraceHelper.SubmissionConnectionTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnection.SubmissionStarted: Thread {0}, Connection: {1}.", Thread.CurrentThread.ManagedThreadId, this.ToString());
		}

		private void ReleaseReference()
		{
			long num = (long)Interlocked.Decrement(ref this.referenceCount);
			TraceHelper.SubmissionConnectionTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnection.ReleaseReference: Thread {0}, Connection: {1}.", Thread.CurrentThread.ManagedThreadId, this.ToString());
			if (num == 0L && this.pool.CanStopConnection(this))
			{
				TraceHelper.SubmissionConnectionTracer.TracePass<int, string>(TraceHelper.MessageProbeActivityId, (long)this.GetHashCode(), "SubmissionConnection.ReleaseReference: Thread {0}, stopping connection: {1}.", Thread.CurrentThread.ManagedThreadId, this.ToString());
				this.LogConnectionStopped(false);
			}
		}

		private void LogConnectionStopped(bool idle)
		{
			ConnectionLog.MapiSubmissionConnectionStop(this.id, this.database, this.successfulSubmissions, 0L, this.bytes, this.recipients, this.failures, this.HasReachedSubmissionLimit, idle);
		}

		private const int MaxSubmissionsPerConnection = 20;

		private static readonly TimeSpan timeoutInterval = TimeSpan.FromSeconds(5.0);

		private static readonly Trace diag = ExTraceGlobals.SubmissionConnectionTracer;

		private readonly string key;

		private readonly SubmissionConnectionPool pool;

		private readonly string database;

		private readonly string server;

		private readonly ulong id;

		private int referenceCount;

		private long totalSubmissions;

		private long successfulSubmissions;

		private long bytes;

		private long recipients;

		private int failures;

		private ExDateTime timeoutDeadline;
	}
}
