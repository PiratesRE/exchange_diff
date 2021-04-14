using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Clutter
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReverseComparer<T> : IComparer<T> where T : struct, IComparable<T>
	{
		private ReverseComparer()
		{
		}

		public static ReverseComparer<T> Instance
		{
			get
			{
				return ReverseComparer<T>.defaultInstance;
			}
		}

		public int Compare(T object1, T object2)
		{
			return -((IComparable)((object)object1)).CompareTo(object2);
		}

		private static readonly ReverseComparer<T> defaultInstance = new ReverseComparer<T>();
	}
}
