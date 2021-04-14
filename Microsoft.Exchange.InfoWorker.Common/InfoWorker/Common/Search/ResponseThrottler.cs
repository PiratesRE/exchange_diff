using System;
using System.Threading;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal class ResponseThrottler
	{
		internal ResponseThrottler() : this(null)
		{
		}

		internal ResponseThrottler(WaitHandle abortHandle)
		{
			this.random = new Random();
			this.BackOffDelay = this.random.Next(1, 11);
			this.abortHandle = abortHandle;
		}

		internal int BackOffDelay
		{
			get
			{
				return this.backOffDelay;
			}
			set
			{
				this.backOffDelay = value;
			}
		}

		internal WaitHandle AbortHandle
		{
			get
			{
				return this.abortHandle;
			}
			set
			{
				this.abortHandle = value;
			}
		}

		internal static int MaxRunningCopySearches
		{
			get
			{
				return SearchMailboxExecuter.GetSettingsValue("MaximumCopySearches", 4);
			}
		}

		internal static int MaxRunningSearches
		{
			get
			{
				return SearchMailboxExecuter.GetSettingsValue("MaximumRunningSearches", 8);
			}
		}

		internal static int MaxThreadLimitPerEstimateSearch
		{
			get
			{
				return SearchMailboxExecuter.GetSettingsValue("MaximumThreadLimitPerEstimateSearch", 32);
			}
		}

		internal static int MaxThreadLimitPerServerEstimateSearch
		{
			get
			{
				return SearchMailboxExecuter.GetSettingsValue("MaximumThreadLimitPerServerEstimateSearch", 6);
			}
		}

		internal static int MaxThreadLimitPerCopySearch
		{
			get
			{
				return SearchMailboxExecuter.GetSettingsValue("MaximumThreadLimitPerCopySearch", 1);
			}
		}

		internal static int MaxThreadLimitPerServerCopySearch
		{
			get
			{
				return SearchMailboxExecuter.GetSettingsValue("MaximumThreadLimitPerServerCopySearch", 1);
			}
		}

		internal static int MaxBulkSize
		{
			get
			{
				return SearchMailboxExecuter.GetSettingsValue("MaximumBulkSize", 128);
			}
		}

		internal void BackOffFromStore(MailboxSession storeSession)
		{
			bool flag = false;
			lock (storeSession)
			{
				flag = storeSession.IsInBackoffState;
			}
			if (flag)
			{
				ResponseThrottler.Tracer.TraceDebug<MailboxSession, int>((long)this.GetHashCode(), "The store {0} is busy, backing off {1} milli-seconds", storeSession, this.BackOffDelay);
				this.Backoff();
				return;
			}
			this.ResetBackoffDelay();
		}

		private void Backoff()
		{
			if (this.AbortHandle != null)
			{
				bool flag = this.AbortHandle.WaitOne(this.BackOffDelay, false);
				if (flag)
				{
					throw new BackoffAbortedException();
				}
			}
			else
			{
				Thread.Sleep(this.BackOffDelay);
			}
			if (this.BackOffDelay < 1024)
			{
				this.BackOffDelay <<= 2;
			}
		}

		private void ResetBackoffDelay()
		{
			this.BackOffDelay = this.random.Next(1, 11);
		}

		internal const int MinInitBackOffDelay = 1;

		internal const int MaxInitBackOffDelay = 10;

		internal const int BackOffShift = 2;

		internal const int MaxBackOffDelay = 1024;

		internal const int MaxBackoffRetry = 20;

		internal const int MaxNonCriticalFailsPerMailbox = 3;

		internal const int SearchFolderTimeOutSecs = 180;

		protected static readonly Trace Tracer = ExTraceGlobals.SearchTracer;

		private Random random;

		private int backOffDelay;

		private WaitHandle abortHandle;
	}
}
