using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public class SizeAndCountStatistics : Statistics<int, double, long>
	{
		[return: Dynamic]
		public static dynamic Calculate(IEnumerable<int> samples)
		{
			IEnumerable<int> source = samples.DefaultIfEmpty<int>();
			SizeAndCountStatistics sizeAndCountStatistics = new SizeAndCountStatistics();
			sizeAndCountStatistics.Average = source.Average((int t) => t);
			sizeAndCountStatistics.Maximum = source.Max((int t) => t);
			sizeAndCountStatistics.Minimum = source.Min((int t) => t);
			sizeAndCountStatistics.Sum = (long)source.Sum((int t) => t);
			sizeAndCountStatistics.SampleCount = source.Count<int>();
			return sizeAndCountStatistics;
		}
	}
}
