using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class IntRange : IComparable<IntRange>, IComparable
	{
		public IntRange(int singleInteger)
		{
			this.UpperBound = singleInteger;
			this.LowerBound = singleInteger;
		}

		public IntRange(int lowerBound, int upperBound)
		{
			if (lowerBound > upperBound)
			{
				throw new ArgumentException("Lower bound cannot be higher than upper.", "lowerBound");
			}
			this.LowerBound = lowerBound;
			this.UpperBound = upperBound;
		}

		public static bool operator ==(IntRange value1, IntRange value2)
		{
			return value1 == value2 || (value1 != null && value2 != null && value1.Equals(value2));
		}

		public static bool operator !=(IntRange value1, IntRange value2)
		{
			return !(value1 == value2);
		}

		public bool Contains(int value)
		{
			return value >= this.LowerBound && value <= this.UpperBound;
		}

		public static IntRange Parse(string expression)
		{
			IntRange result = null;
			if (!IntRange.TryParse(expression, out result))
			{
				throw new ArgumentException(DataStrings.InvalidIntRangeArgument(expression), "expression");
			}
			return result;
		}

		public static bool TryParse(string expression, out IntRange range)
		{
			range = null;
			string[] array = (expression[0] == '-') ? expression.Substring(1).Split(new char[]
			{
				'-'
			}, 2) : expression.Split(new char[]
			{
				'-'
			}, 2);
			if (expression[0] == '-')
			{
				array[0] = "-" + array[0];
			}
			int num = 0;
			int num2 = 0;
			if (array.Length == 1)
			{
				if (int.TryParse(expression, out num))
				{
					range = new IntRange(num, num);
				}
			}
			else if (int.TryParse(array[0], out num) && int.TryParse(array[1], out num2) && num2 >= num)
			{
				range = new IntRange(num, num2);
			}
			return null != range;
		}

		public override string ToString()
		{
			if (this.LowerBound == this.UpperBound)
			{
				return this.LowerBound.ToString();
			}
			return this.LowerBound.ToString() + "-" + this.UpperBound.ToString();
		}

		public int LowerBound { get; private set; }

		public int UpperBound { get; private set; }

		public override bool Equals(object obj)
		{
			IntRange intRange = obj as IntRange;
			return intRange != null && intRange.LowerBound == this.LowerBound && intRange.UpperBound == this.UpperBound;
		}

		public override int GetHashCode()
		{
			return this.LowerBound ^ this.UpperBound;
		}

		public int CompareTo(IntRange x)
		{
			if (x == null)
			{
				return 1;
			}
			int num = this.LowerBound.CompareTo(x.LowerBound);
			if (num != 0)
			{
				return num;
			}
			return this.UpperBound.CompareTo(x.UpperBound);
		}

		int IComparable.CompareTo(object obj)
		{
			IntRange x = obj as IntRange;
			return this.CompareTo(x);
		}

		public const int NoUpperBound = 2147483647;
	}
}
