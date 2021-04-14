using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Monitoring
{
	internal static class IgnoreTransientErrors
	{
		public static bool IgnoreTransientError(string errorKey, uint transientErrorWindowMinutes, ErrorType errorType, out bool isTransitioningState)
		{
			bool flag = false;
			long num = (long)((ulong)(transientErrorWindowMinutes * 60U));
			IgnoreTransientErrors.CheckFailureInfo checkFailureInfo;
			if (IgnoreTransientErrors.Failures.TryGetValue(errorKey, out checkFailureInfo))
			{
				checkFailureInfo.NumSuccessiveFailures += 1U;
				if (checkFailureInfo.LatestPass >= checkFailureInfo.StartOfFailures)
				{
					checkFailureInfo.StartOfFailures = DateTime.UtcNow;
				}
				if (num == 0L)
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug((long)IgnoreTransientErrors.Failures.GetHashCode(), "IgnoreTransientError(): Error with key '{0}' is _NOT_ being ignored. It has now occurred {1} successive times, which exceeds the maximum {2} secs. Last pass time is: {3}. Start of Failures is: {4}. FailedDuration is: {5} secs. ", new object[]
					{
						errorKey,
						checkFailureInfo.NumSuccessiveFailures,
						num,
						checkFailureInfo.LatestPass,
						checkFailureInfo.StartOfFailures,
						checkFailureInfo.FailedDurationSeconds
					});
				}
				else if (checkFailureInfo.FailedDurationSeconds <= num)
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug((long)IgnoreTransientErrors.Failures.GetHashCode(), "IgnoreTransientError(): Error with key '{0}' is being ignored. It has now occurred {1} successive times. Last pass time is: {2}. Start of Failures is: {3}. FailedDuration is: {4} secs. Threshold is: {5} secs.", new object[]
					{
						errorKey,
						checkFailureInfo.NumSuccessiveFailures,
						checkFailureInfo.LatestPass,
						checkFailureInfo.StartOfFailures,
						checkFailureInfo.FailedDurationSeconds,
						num
					});
					flag = true;
				}
				else
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug((long)IgnoreTransientErrors.Failures.GetHashCode(), "IgnoreTransientError(): Error with key '{0}' is _NOT_ being ignored. It has now occurred {1} successive times, which exceeds the maximum {2} secs. Last pass time is: {3}. Start of Failures is: {4}. FailedDuration is: {5} secs.", new object[]
					{
						errorKey,
						checkFailureInfo.NumSuccessiveFailures,
						num,
						checkFailureInfo.LatestPass,
						checkFailureInfo.StartOfFailures,
						checkFailureInfo.FailedDurationSeconds
					});
				}
				if (checkFailureInfo.ErrorType != errorType || checkFailureInfo.LastIgnoreTransientErrorValue != flag)
				{
					isTransitioningState = true;
					ExTraceGlobals.HealthChecksTracer.TraceDebug((long)IgnoreTransientErrors.Failures.GetHashCode(), "IgnoreTransientError(): Setting isTransitioningState={0} for errorKey '{1}'. LastIgnoreTransientErrorValue is: {2}. Current IgnoreTransientErrorValue is: {3}. Last ErrorType is: {4}. Current ErrorType is: {5}.", new object[]
					{
						isTransitioningState,
						errorKey,
						checkFailureInfo.LastIgnoreTransientErrorValue,
						flag,
						checkFailureInfo.ErrorType,
						errorType
					});
				}
				else
				{
					isTransitioningState = false;
				}
				checkFailureInfo.ErrorType = errorType;
			}
			else
			{
				isTransitioningState = true;
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string, bool>((long)IgnoreTransientErrors.Failures.GetHashCode(), "IgnoreTransientError(): First time recording error with key '{0}'. Returning isTransitioningState={1}.", errorKey, isTransitioningState);
				checkFailureInfo = new IgnoreTransientErrors.CheckFailureInfo(DateTime.UtcNow, errorType);
				IgnoreTransientErrors.Failures[errorKey] = checkFailureInfo;
				if (num == 0L)
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug((long)IgnoreTransientErrors.Failures.GetHashCode(), "IgnoreTransientError(): Error with key '{0}' is _NOT_ being ignored. It has now occurred {1} successive times, which exceeds the maximum {2} secs. Last pass time is: {3}. Start of Failures is: {4}. FailedDuration is: {5} secs. ", new object[]
					{
						errorKey,
						checkFailureInfo.NumSuccessiveFailures,
						num,
						checkFailureInfo.LatestPass,
						checkFailureInfo.StartOfFailures,
						checkFailureInfo.FailedDurationSeconds
					});
				}
				else if (checkFailureInfo.FailedDurationSeconds <= num)
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug((long)IgnoreTransientErrors.Failures.GetHashCode(), "IgnoreTransientError(): Error with key '{0}' is being ignored. It has now occurred {1} successive times. Last pass time is: {2}. Start of Failures is: {3}. FailedDuration is: {4} secs. Threshold is: {5} secs.", new object[]
					{
						errorKey,
						checkFailureInfo.NumSuccessiveFailures,
						checkFailureInfo.LatestPass,
						checkFailureInfo.StartOfFailures,
						checkFailureInfo.FailedDurationSeconds,
						num
					});
					flag = true;
				}
				else
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug((long)IgnoreTransientErrors.Failures.GetHashCode(), "IgnoreTransientError(): Error with key '{0}' is _NOT_ being ignored. It has now occurred {1} successive times, which exceeds the maximum {2} secs. Last pass time is: {3}. Start of Failures is: {4}. FailedDuration is: {5} secs. ", new object[]
					{
						errorKey,
						checkFailureInfo.NumSuccessiveFailures,
						num,
						checkFailureInfo.LatestPass,
						checkFailureInfo.StartOfFailures,
						checkFailureInfo.FailedDurationSeconds
					});
				}
			}
			checkFailureInfo.LastIgnoreTransientErrorValue = flag;
			return flag;
		}

		public static bool ResetError(string errorKey)
		{
			bool flag = false;
			IgnoreTransientErrors.CheckFailureInfo checkFailureInfo;
			if (IgnoreTransientErrors.Failures.TryGetValue(errorKey, out checkFailureInfo))
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)IgnoreTransientErrors.Failures.GetHashCode(), "ResetError(): Resetting error with key '{0}'.", errorKey);
				if (checkFailureInfo.ErrorType != ErrorType.Success)
				{
					flag = true;
				}
				checkFailureInfo.NumSuccessiveFailures = 0U;
				checkFailureInfo.LatestPass = DateTime.UtcNow;
				checkFailureInfo.ErrorType = ErrorType.Success;
			}
			else
			{
				flag = true;
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string, bool>((long)IgnoreTransientErrors.Failures.GetHashCode(), "ResetError(): First time recording error with key '{0}'. Returning isTransitioningState={1}.", errorKey, flag);
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)IgnoreTransientErrors.Failures.GetHashCode(), "ResetError(): Error with key '{0}' has not been logged before. This means it hasn't failed or issued a warning before.", errorKey);
				checkFailureInfo = new IgnoreTransientErrors.CheckFailureInfo(DateTime.UtcNow);
				IgnoreTransientErrors.Failures[errorKey] = checkFailureInfo;
			}
			return flag;
		}

		public static bool HasPassed(string errorKey)
		{
			IgnoreTransientErrors.CheckFailureInfo checkFailureInfo;
			if (IgnoreTransientErrors.Failures.TryGetValue(errorKey, out checkFailureInfo))
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string, uint>((long)IgnoreTransientErrors.Failures.GetHashCode(), "HasPassed(): Error with key '{0}' has been logged before. NumSuccessiveFailures = {1}.", errorKey, checkFailureInfo.NumSuccessiveFailures);
				return checkFailureInfo.NumSuccessiveFailures == 0U;
			}
			ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)IgnoreTransientErrors.Failures.GetHashCode(), "HasPassed(): Error with key '{0}' has not been logged before. This means it hasn't failed or issued a warning before.", errorKey);
			return true;
		}

		internal static bool ContainsKey(string errorKey)
		{
			IgnoreTransientErrors.CheckFailureInfo checkFailureInfo;
			return IgnoreTransientErrors.Failures.TryGetValue(errorKey, out checkFailureInfo);
		}

		public const uint DefaultSuccessiveErrorsThreshold = 3U;

		private static Dictionary<string, IgnoreTransientErrors.CheckFailureInfo> Failures = new Dictionary<string, IgnoreTransientErrors.CheckFailureInfo>(StringComparer.OrdinalIgnoreCase);

		internal class CheckFailureInfo
		{
			public CheckFailureInfo(DateTime latestPass)
			{
				this.m_numSuccessiveFailures = 0U;
				this.m_latestPass = latestPass;
				this.m_lastIgnoreTransientErrorValue = false;
				this.m_errorType = ErrorType.Success;
			}

			public CheckFailureInfo(DateTime startOfFailures, ErrorType errorType)
			{
				this.m_numSuccessiveFailures = 1U;
				this.m_startOfFailures = startOfFailures;
				this.m_lastIgnoreTransientErrorValue = false;
				this.m_errorType = errorType;
			}

			public uint NumSuccessiveFailures
			{
				get
				{
					return this.m_numSuccessiveFailures;
				}
				set
				{
					this.m_numSuccessiveFailures = value;
				}
			}

			public DateTime LatestPass
			{
				get
				{
					return this.m_latestPass;
				}
				set
				{
					this.m_latestPass = value;
				}
			}

			public DateTime StartOfFailures
			{
				get
				{
					return this.m_startOfFailures;
				}
				set
				{
					this.m_startOfFailures = value;
				}
			}

			public bool LastIgnoreTransientErrorValue
			{
				get
				{
					return this.m_lastIgnoreTransientErrorValue;
				}
				set
				{
					this.m_lastIgnoreTransientErrorValue = value;
				}
			}

			public ErrorType ErrorType
			{
				get
				{
					return this.m_errorType;
				}
				set
				{
					this.m_errorType = value;
				}
			}

			public long FailedDurationSeconds
			{
				get
				{
					if (!(this.m_startOfFailures > DateTime.MinValue))
					{
						return 0L;
					}
					if (this.m_latestPass > this.m_startOfFailures)
					{
						return 0L;
					}
					return Convert.ToInt64(Math.Ceiling(DateTime.UtcNow.Subtract(this.m_startOfFailures).TotalSeconds));
				}
			}

			private uint m_numSuccessiveFailures;

			private DateTime m_latestPass;

			private DateTime m_startOfFailures;

			private bool m_lastIgnoreTransientErrorValue;

			private ErrorType m_errorType;
		}
	}
}
