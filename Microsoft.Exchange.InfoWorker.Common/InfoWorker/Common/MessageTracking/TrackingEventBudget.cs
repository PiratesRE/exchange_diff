using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class TrackingEventBudget : IDisposeTrackable, IDisposable
	{
		public TimeSpan Elapsed
		{
			get
			{
				return this.timer.Elapsed;
			}
		}

		public TrackingEventBudget(TrackingErrorCollection errors, TimeSpan timeBudgetAllowed)
		{
			TraceWrapper.SearchLibraryTracer.TraceDebug<double>(this.GetHashCode(), "Time budget: {0} msec", timeBudgetAllowed.TotalMilliseconds);
			this.budgetUsed = 0U;
			this.errors = errors;
			this.timeBudgetAllowed = timeBudgetAllowed;
			this.timer = Stopwatch.StartNew();
		}

		internal static void AcquireThread()
		{
			int num = Interlocked.Increment(ref TrackingEventBudget.currentSearchingThreadCount);
			if (num <= ServerCache.Instance.NumberOfThreadsAllowed)
			{
				return;
			}
			TraceWrapper.SearchLibraryTracer.TraceError<int, int>(0, "Server too busy, current-threads={0} max-threads={1}", num, ServerCache.Instance.NumberOfThreadsAllowed);
			TrackingError trackingError = new TrackingError(ErrorCode.TotalBudgetExceeded, string.Empty, string.Empty, string.Empty);
			throw new TrackingTransientException(trackingError, null, false);
		}

		internal static void ReleaseThread()
		{
			Interlocked.Decrement(ref TrackingEventBudget.currentSearchingThreadCount);
		}

		internal bool IsUnderBudget()
		{
			TrackingError trackingError;
			return this.IsUnderResourceBudget() && this.IsUnderTimeBudget(out trackingError);
		}

		internal void CheckTimeBudget()
		{
			TrackingError trackingError;
			if (!this.IsUnderTimeBudget(out trackingError))
			{
				throw new TrackingTransientException(trackingError, null, true);
			}
		}

		public void GetTimeBudgetRemainingForWSCall(TrackingAuthorityKind authorityKindToConnect, out TimeSpan clientTimeout, out TimeSpan serverTimeout)
		{
			TrackingAuthorityKindInformation trackingAuthorityKindInformation;
			if (EnumAttributeInfo<TrackingAuthorityKind, TrackingAuthorityKindInformation>.TryGetValue((int)authorityKindToConnect, out trackingAuthorityKindInformation))
			{
				int expectedConnectionLatencyMSec = trackingAuthorityKindInformation.ExpectedConnectionLatencyMSec;
			}
			int num = (int)Math.Min(this.timer.Elapsed.TotalMilliseconds, 2147483647.0);
			if (this.timeBudgetAllowed.TotalMilliseconds <= (double)num)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<double, int>(this.GetHashCode(), "No time budget remaining. Total budget: {0}, Already used up: {1}", this.timeBudgetAllowed.TotalMilliseconds, num);
				clientTimeout = TimeSpan.Zero;
				serverTimeout = TimeSpan.Zero;
				return;
			}
			double val = this.timeBudgetAllowed.TotalMilliseconds - (double)num;
			int num2 = (int)Math.Min(val, 2147483647.0);
			if (num2 <= trackingAuthorityKindInformation.ExpectedConnectionLatencyMSec)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<int, string, int>(this.GetHashCode(), "Remaining time in budget = {0} is less than connection overhead for Connection-Type: {1}. Overhead = {2}", num2, Names<TrackingAuthorityKind>.Map[(int)authorityKindToConnect], trackingAuthorityKindInformation.ExpectedConnectionLatencyMSec);
				clientTimeout = TimeSpan.Zero;
				serverTimeout = TimeSpan.Zero;
				return;
			}
			int num3 = num2 - trackingAuthorityKindInformation.ExpectedConnectionLatencyMSec;
			clientTimeout = TimeSpan.FromMilliseconds((double)num2);
			serverTimeout = TimeSpan.FromMilliseconds((double)num3);
			TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Timeouts calculated based on budget: clientTimeout={0}, serverTimeout={1}, elapsed={2}, total={3}", new object[]
			{
				num2,
				num3,
				num,
				this.timeBudgetAllowed.TotalMilliseconds
			});
		}

		public void IncrementBy(uint incrementValue)
		{
			if ((ulong)(this.budgetUsed + incrementValue) >= (ulong)((long)ServerCache.Instance.MaxTrackingEventBudgetForSingleQuery))
			{
				TrackingError trackingError = this.errors.Add(ErrorCode.BudgetExceeded, string.Empty, string.Empty, string.Empty);
				throw new TrackingFatalException(trackingError, null, true);
			}
			this.budgetUsed += incrementValue;
			Interlocked.Add(ref TrackingEventBudget.totalBudgetUsed, (int)incrementValue);
			if (TrackingEventBudget.totalBudgetUsed > ServerCache.Instance.MaxTrackingEventBudgetForAllQueries)
			{
				TrackingError trackingError2 = this.errors.Add(ErrorCode.TotalBudgetExceeded, string.Empty, string.Empty, string.Empty);
				throw new TrackingTransientException(trackingError2, null, true);
			}
		}

		public void RestoreBudgetTo(uint budgetToRestoreTo)
		{
			if (budgetToRestoreTo > this.budgetUsed)
			{
				throw new InvalidOperationException("Restoring budget to a higher level than what is in use");
			}
			uint num = this.budgetUsed - budgetToRestoreTo;
			this.budgetUsed = budgetToRestoreTo;
			int num2 = 0;
			if (num > 0U)
			{
				num2 = Interlocked.Add(ref TrackingEventBudget.totalBudgetUsed, (int)(-(int)((ulong)num)));
			}
			if (num2 < 0)
			{
				throw new InvalidOperationException(string.Format("Total budget is decremented to {0}. budgetToRestoreTo is {1}, totalBudgetToReduce is {2}", num2, budgetToRestoreTo, num));
			}
		}

		public uint BudgetUsed
		{
			get
			{
				return this.budgetUsed;
			}
		}

		public void TestDelayOperation(string operation)
		{
			if (ServerCache.Instance.IsTimeoutOverrideConfigured)
			{
				lock (TrackingEventBudget.TestOperationCount)
				{
					string text = ServerCache.TryReadRegistryKey<string>(operation, string.Empty);
					if (!string.IsNullOrEmpty(text))
					{
						string[] array = text.Split(new char[]
						{
							':'
						});
						int num = -1;
						HostId hostId = ServerCache.Instance.HostId;
						string y = Names<HostId>.Map[(int)hostId];
						if (array.Length != 2 || !StringComparer.OrdinalIgnoreCase.Equals(array[0], y) || !int.TryParse(array[1], out num))
						{
							TraceWrapper.SearchLibraryTracer.TraceError<string>(this.GetHashCode(), "TEST CODE: Invalid registry key: {0}", text);
						}
						else
						{
							int num2 = 0;
							if (!TrackingEventBudget.TestOperationCount.TryGetValue(operation, out num2))
							{
								num2 = 0;
							}
							num2++;
							TrackingEventBudget.TestOperationCount[operation] = num2;
							if (num2 > num)
							{
								TraceWrapper.SearchLibraryTracer.TraceDebug<string, int>(this.GetHashCode(), "TEST CODE: Delaying {0} operation, count={1}", operation, num2);
								TimeSpan elapsed = this.timer.Elapsed;
								if (elapsed < this.timeBudgetAllowed)
								{
									int num3 = (int)(this.timeBudgetAllowed - elapsed).TotalMilliseconds;
									TraceWrapper.SearchLibraryTracer.TraceDebug<int>(this.GetHashCode(), "TEST CODE: Pause injected, sleeping away remaining budget: {0} milliseconds", num3);
									Thread.Sleep(num3);
									TrackingEventBudget.TestOperationCount.Clear();
								}
								else
								{
									TraceWrapper.SearchLibraryTracer.TraceDebug<double>(this.GetHashCode(), "TEST CODE: Already over budget, sleep skipped. Elapsed time for this request: {0}", elapsed.TotalMilliseconds);
								}
							}
							else
							{
								TraceWrapper.SearchLibraryTracer.TraceDebug<int, string, int>(this.GetHashCode(), "TEST CODE: {0} {1} operations completed (will delay after {2})", num2, operation, num);
							}
						}
					}
				}
			}
		}

		private bool IsUnderTimeBudget(out TrackingError trackingError)
		{
			trackingError = null;
			if (this.timer.Elapsed > this.timeBudgetAllowed)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<long, double>(this.GetHashCode(), "Over time budget, used={0}, allowed={1}", this.timer.ElapsedMilliseconds, this.timeBudgetAllowed.TotalMilliseconds);
				trackingError = this.errors.Add(ErrorCode.TimeBudgetExceeded, string.Empty, string.Empty, string.Empty);
				return false;
			}
			return true;
		}

		private bool IsUnderResourceBudget()
		{
			if ((ulong)this.budgetUsed >= (ulong)((long)ServerCache.Instance.MaxTrackingEventBudgetForSingleQuery))
			{
				TraceWrapper.SearchLibraryTracer.TraceError<uint>(this.GetHashCode(), "Over single request budget, current resource consumption: {0}", this.budgetUsed);
				this.errors.Add(ErrorCode.BudgetExceeded, string.Empty, string.Empty, string.Empty);
				return false;
			}
			if (TrackingEventBudget.totalBudgetUsed >= ServerCache.Instance.MaxTrackingEventBudgetForAllQueries)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<int>(this.GetHashCode(), "Over total budget, current total resource consumption: {0}", TrackingEventBudget.totalBudgetUsed);
				this.errors.Add(ErrorCode.TotalBudgetExceeded, string.Empty, string.Empty, string.Empty);
				return false;
			}
			return true;
		}

		public DisposeTracker GetDisposeTracker()
		{
			this.disposeTracker = DisposeTracker.Get<TrackingEventBudget>(this);
			return this.disposeTracker;
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			if (this.timer.IsRunning)
			{
				this.timer.Stop();
			}
			GC.SuppressFinalize(this);
		}

		public const string SearchRpcsBeforeDelay = "SearchRpcsBeforeDelay";

		public const string GetRpcsBeforeDelay = "GetRpcsBeforeDelay";

		public const string EwsCallsBeforeDelay = "EwsCallsBeforeDelay";

		public const uint DefaultWebServiceFindMessageTrackingReportCost = 10U;

		private static Dictionary<string, int> TestOperationCount = new Dictionary<string, int>(3);

		private static int currentSearchingThreadCount = 0;

		private TimeSpan timeBudgetAllowed;

		private static int totalBudgetUsed;

		private uint budgetUsed;

		private TrackingErrorCollection errors;

		private Stopwatch timer;

		private DisposeTracker disposeTracker;
	}
}
