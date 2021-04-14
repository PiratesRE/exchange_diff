using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public class ThroughputStatistics : Statistics<double, double, double>
	{
		[return: Dynamic]
		public static dynamic Calculate(IEnumerable<double> samples)
		{
			IEnumerable<double> source = samples.DefaultIfEmpty<double>();
			ThroughputStatistics throughputStatistics = new ThroughputStatistics();
			throughputStatistics.Average = source.Average((double t) => t);
			throughputStatistics.Maximum = source.Max((double t) => t);
			throughputStatistics.Minimum = source.Min((double t) => t);
			throughputStatistics.Sum = source.Sum((double t) => t);
			throughputStatistics.SampleCount = source.Count<double>();
			return throughputStatistics;
		}

		public override string ToString()
		{
			string format = "Avg: {0,-15:F} Max: {1,-15:F} Min: {2,-15:F}";
			return string.Format(format, base.Average, base.Maximum, base.Minimum);
		}
	}
}
