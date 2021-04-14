using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public class ResponseTimeStatistics : Statistics<TimeSpan, TimeSpan, TimeSpan>
	{
		[return: Dynamic]
		public static dynamic Calculate(IEnumerable<TimeSpan> samples)
		{
			IEnumerable<TimeSpan> source = samples.DefaultIfEmpty(TimeSpan.Zero);
			ResponseTimeStatistics responseTimeStatistics = new ResponseTimeStatistics();
			responseTimeStatistics.Average = TimeSpan.FromMilliseconds(source.Average((TimeSpan t) => t.TotalMilliseconds));
			responseTimeStatistics.SampleCount = source.Count<TimeSpan>();
			responseTimeStatistics.Maximum = source.Max((TimeSpan t) => t);
			responseTimeStatistics.Minimum = source.Min((TimeSpan t) => t);
			responseTimeStatistics.Sum = TimeSpan.FromMilliseconds(source.Sum((TimeSpan t) => t.TotalMilliseconds));
			return responseTimeStatistics;
		}

		public override string ToString()
		{
			string format = "Avg: {0,-15:hh\\:mm\\:ss\\.fff} Max: {1,-15:hh\\:mm\\:ss\\.fff} Min: {2,-15:hh\\:mm\\:ss\\.fff}";
			return string.Format(format, base.Average, base.Maximum, base.Minimum);
		}
	}
}
