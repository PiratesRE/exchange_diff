using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Office.Story.V1.ImageAnalysis
{
	[Serializable]
	internal class CountStatistics
	{
		public CountStatistics()
		{
			this.Maximum = double.NegativeInfinity;
			this.Minimum = double.PositiveInfinity;
			this.Average = 0.0;
			this.Count = 0.0;
		}

		public CountStatistics(IEnumerable<double> sequence) : this()
		{
			if (sequence == null)
			{
				throw new ArgumentNullException("sequence");
			}
			this.Add(sequence);
		}

		public double Average { get; set; }

		public double Minimum { get; set; }

		public double Maximum { get; set; }

		public double Count { get; set; }

		public void Add(IEnumerable<double> sequence)
		{
			if (sequence == null)
			{
				throw new ArgumentNullException("sequence");
			}
			foreach (double num in sequence)
			{
				double value = num;
				this.Add(value);
			}
		}

		public void Add(double value)
		{
			this.Average = this.Average * this.Count + value;
			this.Count += 1.0;
			this.Average /= this.Count;
			if (this.Minimum > value)
			{
				this.Minimum = value;
			}
			if (this.Maximum < value)
			{
				this.Maximum = value;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Minimum: {0:G} Average: {1:G} Maximum: {2:G} Count: {3:G}", new object[]
			{
				this.Minimum,
				this.Average,
				this.Maximum,
				this.Count
			});
		}
	}
}
