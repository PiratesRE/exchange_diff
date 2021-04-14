using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ArrayComparer<T> : IComparer<T[]>, IEqualityComparer<T[]> where T : IComparable<T>, IEquatable<T>
	{
		public static IComparer<T[]> Comparer
		{
			get
			{
				return ArrayComparer<T>.instance;
			}
		}

		public static IEqualityComparer<T[]> EqualityComparer
		{
			get
			{
				return ArrayComparer<T>.instance;
			}
		}

		public int Compare(T[] a1, T[] a2)
		{
			if (a1 == a2)
			{
				return 0;
			}
			if (a1 == null)
			{
				return -1;
			}
			if (a2 == null)
			{
				return 1;
			}
			int num = 0;
			while (num < a1.Length && num < a2.Length)
			{
				int num2 = a1[num].CompareTo(a2[num]);
				if (num2 != 0)
				{
					return num2;
				}
				num++;
			}
			return a1.Length.CompareTo(a2.Length);
		}

		public bool Equals(T[] a1, T[] a2)
		{
			return this.Compare(a1, a2) == 0;
		}

		public int GetHashCode(T[] a1)
		{
			if (typeof(T) == typeof(byte))
			{
				return (int)MurmurHash.Hash(a1 as byte[]);
			}
			int num = 0;
			for (int i = 0; i < a1.Length; i++)
			{
				int num2 = 8 * (i % 4);
				int num3 = a1[i].GetHashCode() << num2;
				num ^= num3;
			}
			return num;
		}

		private static ArrayComparer<T> instance = new ArrayComparer<T>();
	}
}
