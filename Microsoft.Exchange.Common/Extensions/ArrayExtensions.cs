using System;

namespace Microsoft.Exchange.Extensions
{
	public static class ArrayExtensions
	{
		public static void Swap<T>(this T[] array, int firstIndex, int secondIndex)
		{
			if (firstIndex != secondIndex)
			{
				ArrayExtensions.Swap<T>(ref array[firstIndex], ref array[secondIndex]);
			}
		}

		public static void Shuffle<T>(this T[] array)
		{
			array.Shuffle(0);
		}

		public static void Shuffle<T>(this T[] array, int index)
		{
			array.Shuffle(index, array.Length - index);
		}

		public static void Shuffle<T>(this T[] array, int index, int length)
		{
			array.ValidateRange(index, length);
			if (length > 1)
			{
				Random random = new Random();
				int num = checked(index + 1);
				int i = index + length;
				while (i > num)
				{
					int firstIndex = random.Next(index, i);
					i--;
					array.Swap(firstIndex, i);
				}
			}
		}

		public static bool IsNullOrEmpty<T>(this T[] array)
		{
			return array == null || 0 == array.Length;
		}

		private static void Swap<T>(ref T first, ref T second)
		{
			T t = first;
			first = second;
			second = t;
		}
	}
}
