using System;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.Diagnostics
{
	internal sealed class CachePerformanceTracker
	{
		internal static void StartLogging()
		{
			CachePerformanceTracker.log = new StringBuilder();
		}

		internal static void AddPerfData(Operation operation, long totalMilliseconds)
		{
			StringBuilder stringBuilder = CachePerformanceTracker.log;
			if (stringBuilder == null)
			{
				return;
			}
			stringBuilder.AppendFormat("{0}={1}", operation, totalMilliseconds);
		}

		internal static void AddException(Operation operation, Exception ex)
		{
			StringBuilder stringBuilder = CachePerformanceTracker.log;
			if (stringBuilder == null)
			{
				return;
			}
			stringBuilder.AppendFormat("{0}={1}", operation, ex.Message);
		}

		internal static string StopLogging()
		{
			if (CachePerformanceTracker.log == null)
			{
				return string.Empty;
			}
			string result = CachePerformanceTracker.log.ToString();
			CachePerformanceTracker.log = null;
			return result;
		}

		[ThreadStatic]
		private static StringBuilder log;
	}
}
