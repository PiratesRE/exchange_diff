using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal class Bits16 : ABit
	{
		public new static Func<int[], uint, int> GetBitsFunc
		{
			get
			{
				return (int[] array, uint index) => Bits16.CurrentValueInLogicIndexFunc(array[(int)((UIntPtr)(index >> 1))], index);
			}
		}

		public new static Action<int[], uint, int> SetBitsAction
		{
			get
			{
				return delegate(int[] array, uint index, int value)
				{
					int num = (int)(index >> 1);
					array[num] = Bits16.NewValueInPhysicalIndexFunc(array[num], index, value);
				};
			}
		}

		public new static Func<uint, int> GetPhysicalIndexFunc
		{
			get
			{
				return (uint index) => (int)(index >> 1);
			}
		}

		public new static Func<int, uint, int, int> NewValueInPhysicalIndexFunc
		{
			get
			{
				return (int oldValue, uint logicIndex, int value) => (oldValue & (int)Bits16.masks[(int)((UIntPtr)(logicIndex & 1U))]) | value << (int)((1U - (logicIndex & 1U)) * 16U);
			}
		}

		public new static Func<int, uint, int> CurrentValueInLogicIndexFunc
		{
			get
			{
				return (int physicalValue, uint index) => physicalValue >> (int)((1U - (index & 1U)) * 16U) & 65535;
			}
		}

		public new const int BitsForCount = 16;

		private static readonly uint[] masks = new uint[]
		{
			65535U,
			4294901760U
		};
	}
}
