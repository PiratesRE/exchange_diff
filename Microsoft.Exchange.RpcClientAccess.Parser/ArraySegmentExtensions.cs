using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ArraySegmentExtensions
	{
		public static ArraySegment<T> SubSegment<T>(this ArraySegment<T> arraySegment, int offset, int count)
		{
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", string.Format("The request of the SubSegment starts before the ArraySegment. Offset cannot be negative. Offset = {0}", offset));
			}
			if (arraySegment.Count - offset < count)
			{
				throw new ArgumentOutOfRangeException("count", string.Format("The request of the SubSegment goes beyond the size of the original ArraySegment. Offset = {0}. Count = {1}. ArraySegment.Count = {2}.", offset, count, arraySegment.Count));
			}
			return new ArraySegment<T>(arraySegment.Array, arraySegment.Offset + offset, count);
		}

		public static ArraySegment<T> SubSegmentToEnd<T>(this ArraySegment<T> arraySegment, int offset)
		{
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", string.Format("The request of the SubSegment starts before the ArraySegment. Offset cannot be negative. Offset = {0}", offset));
			}
			if (offset > arraySegment.Count)
			{
				throw new ArgumentOutOfRangeException("offset", string.Format("The request of the SubSegment starts beyond the length of the ArraySegment. Offset cannot be larger than the ArraySegment. Offset = {0}. ArraySegment.Count = {1}.", offset, arraySegment.Count));
			}
			return arraySegment.SubSegment(offset, arraySegment.Count - offset);
		}

		public static ArraySegment<T> DeepClone<T>(this ArraySegment<T> arraySegment)
		{
			T[] array = new T[arraySegment.Count];
			Array.Copy(arraySegment.Array, arraySegment.Offset, array, 0, arraySegment.Count);
			return new ArraySegment<T>(array);
		}

		public static void CopyTo<T>(this ArraySegment<T> sourceSegment, ArraySegment<T> destinationSegment)
		{
			Array.Copy(sourceSegment.Array, sourceSegment.Offset, destinationSegment.Array, destinationSegment.Offset, sourceSegment.Count);
		}
	}
}
