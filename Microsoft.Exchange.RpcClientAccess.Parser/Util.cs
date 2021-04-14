using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Util
	{
		public static IEqualityComparer<TValue> CreateSelectorEqualityComparer<TValue, TKey>(Func<TValue, TKey> keySelector)
		{
			return Util.CreateSelectorEqualityComparer<TValue, TKey>(keySelector, EqualityComparer<TKey>.Default);
		}

		public static IEqualityComparer<TValue> CreateSelectorEqualityComparer<TValue, TKey>(Func<TValue, TKey> keySelector, IEqualityComparer<TKey> keyEqualityComparer)
		{
			return new Util.SelectorEqualityComparer<TValue, TKey>(keySelector, keyEqualityComparer);
		}

		public static void DisposeIfPresent(IDisposable disposable)
		{
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
		{
			ICollection<T> collection = enumerable as ICollection<T>;
			if (collection == null)
			{
				using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
				{
					return !enumerator.MoveNext();
				}
			}
			return collection.Count == 0;
		}

		public static T[] MergeArrays<T>(params ICollection<T>[] collections)
		{
			int num = 0;
			foreach (ICollection<T> collection in collections)
			{
				if (collection != null)
				{
					num += collection.Count;
				}
			}
			T[] array = new T[num];
			int num2 = 0;
			foreach (ICollection<T> collection2 in collections)
			{
				if (collection2 != null)
				{
					collection2.CopyTo(array, num2);
					num2 += collection2.Count;
				}
			}
			return array;
		}

		public static void AppendToString<T>(StringBuilder stringBuilder, IEnumerable<T> collection)
		{
			if (collection != null)
			{
				bool flag = true;
				foreach (T t in collection)
				{
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append((t != null) ? t.ToString() : "null");
					flag = false;
				}
			}
		}

		public static void AppendToString(StringBuilder stringBuilder, IEnumerable collection)
		{
			if (collection != null)
			{
				bool flag = true;
				foreach (object obj in collection)
				{
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append((obj != null) ? obj.ToString() : "null");
					flag = false;
				}
			}
		}

		public static void AppendToString(StringBuilder stringBuilder, byte[] buffer)
		{
			if (buffer != null)
			{
				Util.AppendToString(stringBuilder, buffer, 0, buffer.Length);
			}
		}

		public static void AppendToString(StringBuilder stringBuilder, byte[] buffer, int offset, int length)
		{
			for (int i = 0; i < length; i++)
			{
				if (i != 0 && i % 4 == 0)
				{
					stringBuilder.Append(" ");
				}
				byte b = buffer[offset + i];
				stringBuilder.Append("0123456789ABCDEF"[(b & 240) >> 4]);
				stringBuilder.Append("0123456789ABCDEF"[(int)(b & 15)]);
			}
		}

		public static void AppendToString(StringBuilder stringBuilder, object value)
		{
			if (value == null)
			{
				stringBuilder.Append("null");
				return;
			}
			if (value is string)
			{
				stringBuilder.Append((string)value);
				return;
			}
			if (value is byte[])
			{
				Util.AppendToString(stringBuilder, (byte[])value);
				return;
			}
			if (value is IEnumerable)
			{
				Util.AppendToString(stringBuilder, (IEnumerable)value);
				return;
			}
			stringBuilder.Append(value);
		}

		public static void ThrowOnNullArgument(object argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
		}

		public static void ThrowOnNullOrEmptyArgument(string argument, string argumentName)
		{
			if (string.IsNullOrEmpty(argument))
			{
				throw new ArgumentNullException(argumentName, "Cannot be null or empty.");
			}
		}

		public static void ThrowOnIntPtrZero(IntPtr intPtr, string intPtrName)
		{
			if (object.Equals(intPtr, IntPtr.Zero))
			{
				throw new ArgumentException("Cannot be IntPtr.Zero", intPtrName);
			}
		}

		private const string HexDigits = "0123456789ABCDEF";

		private sealed class SelectorEqualityComparer<TValue, TKey> : IEqualityComparer<TValue>
		{
			public SelectorEqualityComparer(Func<TValue, TKey> keySelector, IEqualityComparer<TKey> keyEqualityComparer)
			{
				this.keySelector = keySelector;
				this.keyEqualityComparer = keyEqualityComparer;
			}

			public bool Equals(TValue x, TValue y)
			{
				return this.keyEqualityComparer.Equals(this.keySelector(x), this.keySelector(y));
			}

			public int GetHashCode(TValue obj)
			{
				return this.keyEqualityComparer.GetHashCode(this.keySelector(obj));
			}

			private readonly Func<TValue, TKey> keySelector;

			private readonly IEqualityComparer<TKey> keyEqualityComparer;
		}
	}
}
