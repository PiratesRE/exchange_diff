using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Collections
{
	public sealed class ArrayComparer<T> : IEqualityComparer<T[]>, IComparer<T[]> where T : IComparable<T>, IEquatable<T>
	{
		public static ArrayComparer<T> Comparer
		{
			get
			{
				return ArrayComparer<T>.comparer;
			}
		}

		public bool Equals(T[] x, T[] y)
		{
			if (object.ReferenceEquals(x, y))
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			if (x.Length != y.Length)
			{
				return false;
			}
			for (int i = 0; i < x.Length; i++)
			{
				if ((x[i] != null) ? (!x[i].Equals(y[i])) : (y[i] != null))
				{
					return false;
				}
			}
			return true;
		}

		public int Compare(T[] x, T[] y)
		{
			if (object.ReferenceEquals(x, y))
			{
				return 0;
			}
			if (x == null)
			{
				return -1;
			}
			if (y == null)
			{
				return 1;
			}
			int num = Math.Min(x.Length, y.Length);
			for (int i = 0; i < num; i++)
			{
				int num2 = (x[i] != null) ? x[i].CompareTo(y[i]) : ((y[i] != null) ? -1 : 0);
				if (num2 != 0)
				{
					return num2;
				}
			}
			return x.Length - y.Length;
		}

		public int GetHashCode(T[] obj)
		{
			if (obj == null)
			{
				return 0;
			}
			int num = 0;
			foreach (T t in obj)
			{
				if (t != null)
				{
					num = (num << 1 ^ t.GetHashCode());
				}
				else
				{
					num <<= 1;
				}
			}
			return num;
		}

		private static readonly ArrayComparer<T> comparer = new ArrayComparer<T>();
	}
}
