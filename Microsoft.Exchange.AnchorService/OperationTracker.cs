using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class OperationTracker : DisposeTrackableBase
	{
		private OperationTracker(string context, ILogger logger)
		{
			ThreadTimes.GetFromCurrentThread(out this.startingKernelTime, out this.startingUserTime);
			this.context = context;
			this.logger = logger;
			this.timeTracker = Stopwatch.StartNew();
			this.logger.Log(MigrationEventType.Instrumentation, "BEGIN: [TID:{0}] [{1} ms] [{2} ms Kernel] [{3} ms User] - {4}", new object[]
			{
				Thread.CurrentThread.ManagedThreadId,
				this.timeTracker.ElapsedMilliseconds,
				this.startingKernelTime.TotalMilliseconds,
				this.startingUserTime.TotalMilliseconds,
				this.context
			});
		}

		public static OperationTracker Create(ILogger logger, string contextFormat, params object[] contextArgs)
		{
			string text = (contextArgs.Length == 0) ? contextFormat : string.Format(contextFormat, contextArgs);
			return new OperationTracker(text, logger);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OperationTracker>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			this.timeTracker.Stop();
			TimeSpan t;
			TimeSpan t2;
			ThreadTimes.GetFromCurrentThread(out t, out t2);
			this.logger.Log(MigrationEventType.Instrumentation, "END: [TID:{0}] [{1} ms] [{2} ms Kernel] [{3} ms User] - {4}", new object[]
			{
				Thread.CurrentThread.ManagedThreadId,
				this.timeTracker.ElapsedMilliseconds,
				(t - this.startingKernelTime).TotalMilliseconds,
				(t2 - this.startingUserTime).TotalMilliseconds,
				this.context
			});
		}

		private readonly string context;

		private readonly ILogger logger;

		private readonly Stopwatch timeTracker;

		private readonly TimeSpan startingUserTime;

		private readonly TimeSpan startingKernelTime;
	}
}
