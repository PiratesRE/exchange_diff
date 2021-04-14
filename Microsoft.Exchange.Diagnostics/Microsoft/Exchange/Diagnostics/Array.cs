using System;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Array<T>
	{
		public static T[] New(int size)
		{
			if (size != 0)
			{
				return new T[size];
			}
			return Array<T>.Empty;
		}

		public static readonly T[] Empty = new T[0];

		public static readonly ArraySegment<T> EmptySegment = new ArraySegment<T>(Array<T>.Empty);
	}
}
