using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	internal class EnumFormatter<T>
	{
		public EnumFormatter(string format, Func<T, int> converterToInt32)
		{
			EnumFormatter<T> <>4__this = this;
			this.converterToInt32 = converterToInt32;
			Type typeFromHandle = typeof(T);
			Attribute customAttribute = Attribute.GetCustomAttribute(typeFromHandle, typeof(FlagsAttribute));
			if (customAttribute != null)
			{
				throw new ArgumentException("Flags enums are not supported.", typeFromHandle.ToString());
			}
			T[] source = (T[])Enum.GetValues(typeFromHandle);
			IEnumerable<int> source2 = from v in source
			select this.converterToInt32(v);
			this.minValue = source2.Min<int>();
			int num = source2.Max<int>() - this.minValue + 1;
			if ((long)num > 1000L)
			{
				throw new ArgumentException("Enum's value range is not supported.", typeFromHandle.ToString());
			}
			this.indexLookup = new int[num];
			int[] array = source2.ToArray<int>();
			for (int i = 0; i < array.Length; i++)
			{
				this.indexLookup[array[i] - this.minValue] = i;
			}
			this.names = (from v in Enum.GetNames(typeFromHandle)
			select string.Format(format, v)).ToArray<string>();
		}

		public string Format(T value)
		{
			return this.names[this.GetIndex(value)];
		}

		public EnumFormatter<T> OverrideFormat(T value, string format)
		{
			this.names[this.GetIndex(value)] = format;
			return this;
		}

		[Conditional("DEBUG")]
		private void Validate(string format)
		{
			foreach (T t in (T[])Enum.GetValues(typeof(T)))
			{
			}
		}

		private int GetIndex(T value)
		{
			return this.indexLookup[this.converterToInt32(value) - this.minValue];
		}

		private const uint MaxSupportedRange = 1000U;

		private readonly string[] names;

		private readonly int[] indexLookup;

		private readonly int minValue;

		private readonly Func<T, int> converterToInt32;
	}
}
