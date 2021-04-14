using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Office.Story.V1.ImageAnalysis
{
	[Serializable]
	internal class Histogram<T>
	{
		public Histogram(Func<T, double> valueExtractor, double start, double end, double interval, IEnumerable<T> items) : this(valueExtractor, start, end, interval)
		{
			this.Add(items);
		}

		public Histogram(Func<T, double> valueExtractor, double start, double end, double interval)
		{
			if (interval <= 0.0)
			{
				throw new ArgumentException("Interval must be positive.", "interval");
			}
			if (start >= end)
			{
				throw new ArgumentException("End of the interval must be above Start of the interval.", "end");
			}
			this.valueExtractor = valueExtractor;
			this.Start = start;
			this.End = end;
			this.Interval = interval;
			this.ItemsCount = 0;
			this.OutOfRangeItemsCount = 0;
			this.Bins = new List<Bin<T>>();
			for (double num = this.Start; num < this.End; num += this.Interval)
			{
				this.Bins.Add(new Bin<T>
				{
					Items = new List<T>(),
					RangeStart = num,
					RangeEnd = num + interval
				});
			}
		}

		public double Start { get; private set; }

		public double End { get; private set; }

		public double Interval { get; private set; }

		public int ItemsCount { get; private set; }

		public int OutOfRangeItemsCount { get; private set; }

		public List<Bin<T>> Bins { get; private set; }

		public void Add(T item)
		{
			double num = this.valueExtractor(item);
			if (num >= this.Start && num < this.End)
			{
				int index = (int)((num - this.Start) / this.Interval);
				this.Bins[index].Items.Add(item);
			}
			else
			{
				this.OutOfRangeItemsCount++;
			}
			this.ItemsCount++;
		}

		public void Add(IEnumerable<T> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			foreach (T item in items)
			{
				this.Add(item);
			}
		}

		public double ScanForRange(double startWindow, double endWindow, Func<Bin<T>, double> extractor, Func<double, double, double> aggregator, Func<double, double, bool> exitCriteria)
		{
			if (extractor == null)
			{
				throw new ArgumentNullException("extractor");
			}
			if (aggregator == null)
			{
				throw new ArgumentNullException("aggregator");
			}
			if (exitCriteria == null)
			{
				throw new ArgumentNullException("exitCriteria");
			}
			double arg = 0.0;
			double num;
			if (startWindow < endWindow)
			{
				num = startWindow;
				for (int i = 0; i < this.Bins.Count; i++)
				{
					Bin<T> bin = this.Bins[i];
					if (bin.RangeStart >= startWindow)
					{
						double arg2 = extractor(bin);
						arg = aggregator(arg, arg2);
						if (exitCriteria(arg, arg2))
						{
							break;
						}
						num = Math.Max(num, bin.RangeEnd);
					}
				}
			}
			else
			{
				num = startWindow;
				for (int j = this.Bins.Count - 1; j >= 0; j--)
				{
					Bin<T> bin2 = this.Bins[j];
					if (bin2.RangeEnd >= startWindow)
					{
						double arg3 = extractor(bin2);
						arg = aggregator(arg, arg3);
						if (exitCriteria(arg, arg3))
						{
							break;
						}
						num = Math.Min(num, bin2.RangeStart);
					}
				}
			}
			return num;
		}

		public override string ToString()
		{
			return string.Join("\n", from bin in this.Bins
			select string.Format(CultureInfo.InvariantCulture, "{0}\t{1}\t", new object[]
			{
				bin.RangeStart,
				bin.Items.Count<T>()
			}));
		}

		[NonSerialized]
		private readonly Func<T, double> valueExtractor;
	}
}
