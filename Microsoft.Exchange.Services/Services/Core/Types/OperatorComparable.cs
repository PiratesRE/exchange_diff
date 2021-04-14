using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal abstract class OperatorComparable : IComparable<OperatorComparable>
	{
		public static bool operator <(OperatorComparable obj1, OperatorComparable obj2)
		{
			return OperatorComparable.Compare(obj1, obj2) < 0;
		}

		public static bool operator >(OperatorComparable obj1, OperatorComparable obj2)
		{
			return OperatorComparable.Compare(obj1, obj2) > 0;
		}

		public static bool operator ==(OperatorComparable obj1, OperatorComparable obj2)
		{
			return OperatorComparable.Compare(obj1, obj2) == 0;
		}

		public static bool operator !=(OperatorComparable obj1, OperatorComparable obj2)
		{
			return OperatorComparable.Compare(obj1, obj2) != 0;
		}

		public static bool operator <=(OperatorComparable obj1, OperatorComparable obj2)
		{
			return OperatorComparable.Compare(obj1, obj2) <= 0;
		}

		public static bool operator >=(OperatorComparable obj1, OperatorComparable obj2)
		{
			return OperatorComparable.Compare(obj1, obj2) >= 0;
		}

		public static int Compare(OperatorComparable obj1, OperatorComparable obj2)
		{
			if (object.ReferenceEquals(obj1, obj2))
			{
				return 0;
			}
			if (obj1 == null)
			{
				return -1;
			}
			if (obj2 == null)
			{
				return 1;
			}
			return obj1.CompareTo(obj2);
		}

		public override bool Equals(object obj)
		{
			return obj is OperatorComparable && this == (OperatorComparable)obj;
		}

		public abstract int CompareTo(OperatorComparable obj);

		public abstract override int GetHashCode();
	}
}
