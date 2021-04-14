using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.ExchangeSystem
{
	public class ExDateRange : IComparable, IComparable<ExDateRange>
	{
		public ExDateRange(ExDateTime start, ExDateTime end)
		{
			this.start = start;
			this.end = end;
		}

		public ExDateTime Start
		{
			get
			{
				return this.start;
			}
		}

		public ExDateTime End
		{
			get
			{
				return this.end;
			}
		}

		public static ExDateRange Intersection(ExDateRange a, ExDateRange b)
		{
			if (a == null && b != null)
			{
				return new ExDateRange(b.Start, b.End);
			}
			if (a != null && b == null)
			{
				return new ExDateRange(a.Start, a.End);
			}
			ExDateTime t = (ExDateTime.Compare(a.Start, b.Start) > 0) ? a.Start : b.Start;
			ExDateTime t2 = (ExDateTime.Compare(a.End, b.End) < 0) ? a.End : b.End;
			if (!(t <= t2))
			{
				return null;
			}
			return new ExDateRange(t, t2);
		}

		public static bool AreEqual(ExDateRange a, ExDateRange b)
		{
			if (a != null)
			{
				return a.Equals(b);
			}
			return b == null;
		}

		public static bool AreOverlapping(ExDateRange a, ExDateRange b)
		{
			if (a == null)
			{
				throw new ArgumentNullException("a");
			}
			if (b == null)
			{
				throw new ArgumentNullException("b");
			}
			return ExDateRange.Intersection(a, b) != null;
		}

		public bool Equals(ExDateRange otherRange)
		{
			return otherRange != null && ExDateTime.Compare(this.Start, otherRange.Start) == 0 && ExDateTime.Compare(this.End, otherRange.End) == 0;
		}

		public bool ContainsDate(ExDateTime dateToTest, bool startInclusive, bool endInclusive)
		{
			bool flag;
			if (startInclusive)
			{
				flag = (ExDateTime.Compare(this.Start, dateToTest) <= 0);
			}
			else
			{
				flag = (ExDateTime.Compare(this.Start, dateToTest) < 0);
			}
			if (endInclusive)
			{
				return flag && ExDateTime.Compare(this.End, dateToTest) >= 0;
			}
			return flag && ExDateTime.Compare(this.End, dateToTest) > 0;
		}

		public override string ToString()
		{
			return this.start.ToString() + "-" + this.end.ToString();
		}

		public int CompareTo(object obj)
		{
			if (obj is ExDateRange)
			{
				return this.CompareTo((ExDateRange)obj);
			}
			throw new ArgumentException("Invalid comparison of ExDateRange value to a different type");
		}

		public int CompareTo(ExDateRange other)
		{
			if (this.Equals(other))
			{
				return 0;
			}
			if (this.Start.CompareTo(other.start) == 0)
			{
				return this.End.CompareTo(other.End);
			}
			return this.Start.CompareTo(other.Start);
		}

		public static List<ExDateRange> SubtractRanges(ExDateRange sourceRange, List<ExDateRange> rangesToRemove)
		{
			List<ExDateRange> list = new List<ExDateRange>();
			list.Add(sourceRange);
			rangesToRemove.Sort();
			foreach (ExDateRange rangeToRemove in rangesToRemove)
			{
				list = ExDateRange.Subtract(list, rangeToRemove);
			}
			return list;
		}

		public static List<ExDateRange> Subtract(ExDateRange sourceRange, ExDateRange rangeToRemove)
		{
			List<ExDateRange> list = new List<ExDateRange>();
			ExDateRange exDateRange = ExDateRange.Intersection(sourceRange, rangeToRemove);
			if (exDateRange == null || exDateRange.Start.Equals(exDateRange.End))
			{
				list.Add(sourceRange);
			}
			else if (!sourceRange.Equals(exDateRange))
			{
				if (sourceRange.Start < exDateRange.Start)
				{
					list.Add(new ExDateRange(sourceRange.Start, exDateRange.Start));
				}
				if (exDateRange.End < sourceRange.End)
				{
					list.Add(new ExDateRange(exDateRange.End, sourceRange.End));
				}
			}
			return list;
		}

		public static List<ExDateRange> Subtract(List<ExDateRange> sourceRanges, ExDateRange rangeToRemove)
		{
			List<ExDateRange> list = new List<ExDateRange>();
			foreach (ExDateRange exDateRange in sourceRanges)
			{
				if (ExDateRange.Intersection(exDateRange, rangeToRemove) == null)
				{
					list.Add(exDateRange);
				}
				else
				{
					list.AddRange(ExDateRange.Subtract(exDateRange, rangeToRemove));
				}
			}
			return list;
		}

		private readonly ExDateTime start;

		private readonly ExDateTime end;
	}
}
