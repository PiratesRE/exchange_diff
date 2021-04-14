using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal sealed class SubmissionThreadLimiter : DisposeTrackableBase
	{
		public static int ConcurrentSubmissions
		{
			get
			{
				return SubmissionThreadLimiter.concurrentSubmissions;
			}
		}

		public static SubmissionDatabaseThreadMap DatabaseThreadMap
		{
			get
			{
				return SubmissionThreadLimiter.databaseThreadMap;
			}
		}

		public void BeginSubmission(ulong id, string server, string database)
		{
			int num = Interlocked.Increment(ref SubmissionThreadLimiter.concurrentSubmissions);
			if (num > Components.Configuration.LocalServer.MaxConcurrentMailboxSubmissions)
			{
				StoreDriverSubmission.LogEvent(MSExchangeStoreDriverSubmissionEventLogConstants.Tuple_TooManySubmissionThreads, null, new object[]
				{
					Components.Configuration.LocalServer.MaxConcurrentMailboxSubmissions
				});
				string message = string.Format("Total thread count exceeded limit of {0} threads.", Components.Configuration.LocalServer.MaxConcurrentMailboxSubmissions);
				throw new ThreadLimitExceededException(message);
			}
			if (SubmissionThreadLimiter.databaseThreadMap.ThreadLimit > 0)
			{
				SubmissionThreadLimiter.databaseThreadMap.CheckAndIncrement(database, id, database);
				this.databaseThreadMapEntry = database;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SubmissionThreadLimiter>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			Interlocked.Decrement(ref SubmissionThreadLimiter.concurrentSubmissions);
			if (!disposing)
			{
				return;
			}
			if (this.databaseThreadMapEntry != null)
			{
				SubmissionThreadLimiter.databaseThreadMap.Decrement(this.databaseThreadMapEntry);
			}
		}

		private static readonly Trace diag = ExTraceGlobals.SubmissionConnectionPoolTracer;

		private static readonly SubmissionDatabaseThreadMap databaseThreadMap = new SubmissionDatabaseThreadMap(SubmissionThreadLimiter.diag);

		private static int concurrentSubmissions;

		private string databaseThreadMapEntry;
	}
}
