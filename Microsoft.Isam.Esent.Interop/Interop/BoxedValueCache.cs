using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal static class BoxedValueCache<T> where T : struct, IEquatable<T>
	{
		public static object GetBoxedValue(T? value)
		{
			if (value == null)
			{
				return null;
			}
			T value2 = value.Value;
			int num = (value2.GetHashCode() & int.MaxValue) % 257;
			IEquatable<T> equatable = BoxedValueCache<T>.BoxedValues[num];
			if (equatable == null || !equatable.Equals(value2))
			{
				equatable = value2;
				BoxedValueCache<T>.BoxedValues[num] = equatable;
			}
			return equatable;
		}

		private const int NumCachedBoxedValues = 257;

		private static readonly IEquatable<T>[] BoxedValues = new IEquatable<T>[257];
	}
}
