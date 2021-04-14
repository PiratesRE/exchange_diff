using System;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public class Statistics<T, AverageT, SumT>
	{
		public AverageT Average { get; set; }

		public T Maximum { get; set; }

		public T Minimum { get; set; }

		public SumT Sum { get; set; }

		public int SampleCount { get; set; }

		public override string ToString()
		{
			string format = "Avg: {0,-15:F} Max: {1,-15} Min: {2,-15}";
			return string.Format(format, this.Average, this.Maximum, this.Minimum);
		}
	}
}
