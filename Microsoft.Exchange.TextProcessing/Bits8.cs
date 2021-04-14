using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal class Bits8 : ABit
	{
		public new static Func<int[], uint, int> GetBitsFunc
		{
			get
			{
				return (int[] array, uint index) => Bits8.CurrentValueInLogicIndexFunc(array[(int)((UIntPtr)(index >> 2))], index);
			}
		}

		public new static Action<int[], uint, int> SetBitsAction
		{
			get
			{
				return delegate(int[] array, uint index, int value)
				{
					int num = (int)(index >> 2);
					array[num] = Bits8.NewValueInPhysicalIndexFunc(array[num], index, value);
				};
			}
		}

		public new static Func<uint, int> GetPhysicalIndexFunc
		{
			get
			{
				return (uint index) => (int)(index >> 2);
			}
		}

		public new static Func<int, uint, int, int> NewValueInPhysicalIndexFunc
		{
			get
			{
				return (int oldValue, uint logicIndex, int value) => (oldValue & (int)Bits8.masks[(int)((UIntPtr)(logicIndex & 3U))]) | value << (int)((3U - (logicIndex & 3U)) * 8U);
			}
		}

		public new static Func<int, uint, int> CurrentValueInLogicIndexFunc
		{
			get
			{
				return (int physicalValue, uint index) => physicalValue >> (int)((3U - (index & 3U)) * 8U) & 255;
			}
		}

		public new const int BitsForCount = 8;

		private static readonly uint[] masks = new uint[]
		{
			16777215U,
			4278255615U,
			4294902015U,
			4294967040U
		};
	}
}
