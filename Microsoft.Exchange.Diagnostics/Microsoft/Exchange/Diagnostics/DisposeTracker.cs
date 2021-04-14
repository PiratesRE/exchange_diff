using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace Microsoft.Exchange.Diagnostics
{
	[CLSCompliant(true)]
	public abstract class DisposeTracker : CriticalFinalizerObject, IDisposable
	{
		public static bool Suppressed
		{
			get
			{
				return DisposeTracker.suppressed;
			}
			set
			{
				DisposeTracker.suppressed = value;
			}
		}

		public static Action<string, string> OnLeakDetected { get; set; }

		public bool HasCollectedStackTrace { get; protected set; }

		internal IList<WatsonExtraDataReportAction> ExtraDataList { get; private set; }

		protected StackTrace StackTrace
		{
			get
			{
				return this.stackTrace;
			}
			set
			{
				this.stackTrace = value;
			}
		}

		protected bool WasProperlyDisposed
		{
			get
			{
				return this.wasProperlyDisposed;
			}
		}

		protected bool StackTraceWasReset
		{
			get
			{
				return this.stackTraceWasReset;
			}
		}

		public static DisposeTracker Get<T>(T obj) where T : IDisposable
		{
			if (DisposeTrackerOptions.Enabled || DisposeTracker.OnLeakDetected != null)
			{
				return new DisposeTrackerObject<T>();
			}
			return DisposeTrackerNullObject.Instance;
		}

		public static void ForceLeakExposure()
		{
			for (int i = 0; i <= GC.MaxGeneration; i++)
			{
				GC.Collect(i, GCCollectionMode.Forced, true);
			}
			for (int j = GC.MaxGeneration; j >= 0; j--)
			{
				GC.Collect(j, GCCollectionMode.Forced, true);
			}
			GC.WaitForPendingFinalizers();
		}

		public static void ResetType<T>(T obj) where T : IDisposable
		{
			DisposeTrackerObject<T>.Reset();
		}

		public void Suppress()
		{
			this.stackTrace = null;
		}

		public void SetReportedStacktraceToCurrentLocation()
		{
			if (this.stackTrace != null)
			{
				this.stackTrace = new StackTrace(true);
				this.stackTraceWasReset = true;
			}
		}

		public void Dispose()
		{
			this.wasProperlyDisposed = true;
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void AddExtraData(string extraData)
		{
			if (this.ExtraDataList == null)
			{
				this.ExtraDataList = new List<WatsonExtraDataReportAction>();
			}
			this.ExtraDataList.Add(new WatsonExtraDataReportAction(extraData));
		}

		public void AddExtraDataWithStackTrace(string header)
		{
			if (this.HasCollectedStackTrace)
			{
				StackTrace stackTrace = new StackTrace(1, DisposeTrackerOptions.UseFullSymbols);
				this.AddExtraData(string.Format(CultureInfo.InvariantCulture, "{0}:{1}{2}", new object[]
				{
					header,
					Environment.NewLine,
					stackTrace
				}));
			}
		}

		protected static bool CanCollectAnotherStackTraceYet()
		{
			return (ulong)(Environment.TickCount - (int)DisposeTracker.lastStacktraceTicks) >= (ulong)((long)DisposeTrackerOptions.ThrottleMilliseconds);
		}

		protected static void RecordStackTraceTimestamp()
		{
			DisposeTracker.lastStacktraceTicks = (uint)Environment.TickCount;
		}

		protected static bool CheckAndUpdateIfCanWatsonThisSecond()
		{
			if (Environment.TickCount - (int)DisposeTracker.lastWatsonTicks >= 1000)
			{
				DisposeTracker.lastWatsonTicks = (uint)Environment.TickCount;
				DisposeTracker.watsonCount = 1L;
				return true;
			}
			return Interlocked.Increment(ref DisposeTracker.watsonCount) < (long)DisposeTrackerOptions.MaximumWatsonsPerSecond;
		}

		protected abstract void Dispose(bool disposing);

		internal const string IsTypeRiskyFieldName = "isTypeRisky";

		private static uint lastStacktraceTicks;

		private static uint lastWatsonTicks;

		private static long watsonCount;

		private static bool suppressed;

		private StackTrace stackTrace;

		private bool stackTraceWasReset;

		private bool wasProperlyDisposed;
	}
}
