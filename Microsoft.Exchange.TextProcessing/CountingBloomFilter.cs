using System;
using System.Reflection;
using System.Threading;

namespace Microsoft.Exchange.TextProcessing
{
	internal class CountingBloomFilter<TBit> where TBit : ABit
	{
		public CountingBloomFilter() : this(10)
		{
		}

		public CountingBloomFilter(int powerIndexOf2) : this(powerIndexOf2, int.MaxValue, 4)
		{
		}

		public CountingBloomFilter(int powerIndexOf2, int maxValue, int hashNumbers)
		{
			if (hashNumbers < 2)
			{
				throw new ArgumentOutOfRangeException("hashNumbers");
			}
			this.hashNumbers = hashNumbers;
			if (powerIndexOf2 < 0)
			{
				throw new ArgumentOutOfRangeException("powerIndexOf2");
			}
			this.length = 2U << powerIndexOf2;
			FieldInfo field = typeof(TBit).GetField("BitsForCount");
			CountingBloomFilter<TBit>.bitsForCount = (int)field.GetValue(null);
			PropertyInfo property = typeof(TBit).GetProperty("GetBitsFunc");
			this.getBitsFunc = (Func<int[], uint, int>)property.GetValue(null);
			PropertyInfo property2 = typeof(TBit).GetProperty("SetBitsAction");
			this.setBitsAction = (Action<int[], uint, int>)property2.GetValue(null);
			PropertyInfo property3 = typeof(TBit).GetProperty("GetPhysicalIndexFunc");
			this.getPhysicalIndexFunc = (Func<uint, int>)property3.GetValue(null);
			PropertyInfo property4 = typeof(TBit).GetProperty("NewValueInPhysicalIndexFunc");
			this.newValueInPhysicalIndexFunc = (Func<int, uint, int, int>)property4.GetValue(null);
			PropertyInfo property5 = typeof(TBit).GetProperty("CurrentValueInLogicIndexFunc");
			this.currentValueInLogicIndexFunc = (Func<int, uint, int>)property5.GetValue(null);
			if (typeof(TBit) == typeof(Bit1))
			{
				this.markBits = new Func<uint, int, int>(this.MarkBit);
			}
			else
			{
				this.markBits = new Func<uint, int, int>(this.MarkBits);
			}
			CountingBloomFilter<TBit>.maxValue = Math.Min(maxValue, (1 << CountingBloomFilter<TBit>.bitsForCount) - 1);
			this.array = new int[this.GetArrayLength(this.length)];
			for (int i = 0; i < this.array.Length; i++)
			{
				this.array[i] = 0;
			}
		}

		public uint Length
		{
			get
			{
				return this.length;
			}
		}

		public bool Add(byte[] bytes, int count)
		{
			ulong hashcode = FnvHash.Fnv1A64(bytes);
			return this.Add(hashcode, count);
		}

		public bool MaySignificant(byte[] bytes)
		{
			ulong hashcode = FnvHash.Fnv1A64(bytes);
			return this.MaySignificant(hashcode);
		}

		public bool Add(string stringValue, int count)
		{
			ulong hashcode = FnvHash.Fnv1A64(stringValue);
			return this.Add(hashcode, count);
		}

		public bool MaySignificant(string stringValue)
		{
			ulong hashcode = FnvHash.Fnv1A64(stringValue);
			return this.MaySignificant(hashcode);
		}

		public bool Add(ulong hashcode, int count)
		{
			uint num = (uint)(hashcode >> 32) & uint.MaxValue;
			uint num2 = (uint)hashcode & uint.MaxValue;
			int num3 = CountingBloomFilter<TBit>.maxValue;
			for (int i = 0; i < this.hashNumbers; i++)
			{
				num3 = Math.Min(num3, this.markBits((uint)((ulong)num + (ulong)((long)i * (long)((ulong)num2)) & (ulong)(this.length - 1U)), count));
			}
			return num3 == CountingBloomFilter<TBit>.maxValue;
		}

		public bool MaySignificant(ulong hashcode)
		{
			uint num = (uint)(hashcode >> 32) & uint.MaxValue;
			uint num2 = (uint)hashcode & uint.MaxValue;
			for (int i = 0; i < this.hashNumbers; i++)
			{
				if (this.getBitsFunc(this.array, (uint)((ulong)num + (ulong)((long)i * (long)((ulong)num2)) & (ulong)(this.length - 1U))) != CountingBloomFilter<TBit>.maxValue)
				{
					return false;
				}
			}
			return true;
		}

		public bool Add(uint hashcode, int count)
		{
			uint num = hashcode >> 16 & 65535U;
			uint num2 = hashcode & 65535U;
			int num3 = CountingBloomFilter<TBit>.maxValue;
			for (int i = 0; i < this.hashNumbers; i++)
			{
				num3 = Math.Min(num3, this.markBits((uint)((ulong)num + (ulong)((long)i * (long)((ulong)num2)) & (ulong)(this.length - 1U)), count));
			}
			return num3 == CountingBloomFilter<TBit>.maxValue;
		}

		public bool MaySignificant(uint hashcode)
		{
			uint num = hashcode >> 16 & 65535U;
			uint num2 = hashcode & 65535U;
			for (int i = 0; i < this.hashNumbers; i++)
			{
				if (this.getBitsFunc(this.array, (uint)((ulong)num + (ulong)((long)i * (long)((ulong)num2)) & (ulong)(this.length - 1U))) != CountingBloomFilter<TBit>.maxValue)
				{
					return false;
				}
			}
			return true;
		}

		private uint GetArrayLength(uint logicLength)
		{
			if (logicLength <= 0U)
			{
				return 0U;
			}
			return (uint)(((ulong)logicLength * (ulong)((long)CountingBloomFilter<TBit>.bitsForCount) - 1UL) / 32UL) + 1U;
		}

		private int MarkBit(uint index, int value)
		{
			if (value != 0)
			{
				this.setBitsAction(this.array, index, value);
			}
			return this.getBitsFunc(this.array, index);
		}

		private int MarkBits(uint index, int value)
		{
			int num = 0;
			for (;;)
			{
				num++;
				int num2 = this.getPhysicalIndexFunc(index);
				int num3 = this.array[num2];
				int num4 = this.currentValueInLogicIndexFunc(num3, index);
				if (num4 >= CountingBloomFilter<TBit>.maxValue)
				{
					break;
				}
				int num5 = num4 + value;
				if (num5 >= CountingBloomFilter<TBit>.maxValue)
				{
					goto Block_2;
				}
				int value2 = this.newValueInPhysicalIndexFunc(num3, index, num5);
				int num6 = Interlocked.CompareExchange(ref this.array[num2], value2, num3);
				if (num3 == num6 || num >= 1000)
				{
					return num5;
				}
			}
			return CountingBloomFilter<TBit>.maxValue;
			Block_2:
			this.setBitsAction(this.array, index, CountingBloomFilter<TBit>.maxValue);
			return CountingBloomFilter<TBit>.maxValue;
		}

		private const int DefaultHashNumber = 4;

		private const int DefaultIndexOf2 = 10;

		private static int bitsForCount;

		private static int maxValue;

		private readonly Func<int[], uint, int> getBitsFunc;

		private readonly Action<int[], uint, int> setBitsAction;

		private readonly Func<uint, int, int> markBits;

		private readonly Func<uint, int> getPhysicalIndexFunc;

		private readonly Func<int, uint, int, int> newValueInPhysicalIndexFunc;

		private readonly Func<int, uint, int> currentValueInLogicIndexFunc;

		private readonly int[] array;

		private readonly int hashNumbers;

		private readonly uint length;
	}
}
