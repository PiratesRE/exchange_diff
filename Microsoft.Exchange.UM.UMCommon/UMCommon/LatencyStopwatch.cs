using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class LatencyStopwatch
	{
		private void StopSuccess(Stopwatch stopwatch, string operationName)
		{
			stopwatch.Stop();
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "{0} [latency {1}ms]", new object[]
			{
				operationName,
				stopwatch.ElapsedMilliseconds
			});
		}

		private void StopFailure(Stopwatch stopwatch, string operationName, Exception ex)
		{
			stopwatch.Stop();
			CallIdTracer.TraceWarning(ExTraceGlobals.UtilTracer, null, "{0} [latency {1}ms] (exception {2})", new object[]
			{
				operationName,
				stopwatch.ElapsedMilliseconds,
				ex.GetType().Name + " : " + ex.Message
			});
		}

		public T Invoke<T>(string operationName, Func<T> func)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			T result;
			try
			{
				T t = func();
				this.StopSuccess(stopwatch, operationName);
				result = t;
			}
			catch (Exception ex)
			{
				this.StopFailure(stopwatch, operationName, ex);
				throw;
			}
			return result;
		}

		public void Invoke(string operationName, Action func)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				func();
				this.StopSuccess(stopwatch, operationName);
			}
			catch (Exception ex)
			{
				this.StopFailure(stopwatch, operationName, ex);
				throw;
			}
		}
	}
}
