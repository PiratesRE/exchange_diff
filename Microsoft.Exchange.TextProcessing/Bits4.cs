using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal class Bits4 : ABit
	{
		public new static Func<int[], uint, int> GetBitsFunc
		{
			get
			{
				return (int[] array, uint index) => Bits4.CurrentValueInLogicIndexFunc(array[(int)((UIntPtr)(index >> 3))], index);
			}
		}

		public new static Action<int[], uint, int> SetBitsAction
		{
			get
			{
				return delegate(int[] array, uint index, int value)
				{
					int num = (int)(index >> 3);
					array[num] = Bits4.NewValueInPhysicalIndexFunc(array[num], index, value);
				};
			}
		}

		public new static Func<uint, int> GetPhysicalIndexFunc
		{
			get
			{
				return (uint index) => (int)(index >> 3);
			}
		}

		public new static Func<int, uint, int, int> NewValueInPhysicalIndexFunc
		{
			get
			{
				return (int oldValue, uint logicIndex, int value) => (oldValue & (int)Bits4.masks[(int)((UIntPtr)(logicIndex & 7U))]) | value << (int)((7U - (logicIndex & 7U)) * 4U);
			}
		}

		public new static Func<int, uint, int> CurrentValueInLogicIndexFunc
		{
			get
			{
				return (int physicalValue, uint index) => physicalValue >> (int)((7U - (index & 7U)) * 4U) & 15;
			}
		}

		public new const int BitsForCount = 4;

		private static readonly uint[] masks = new uint[]
		{
			268435455U,
			4043309055U,
			4279238655U,
			4293984255U,
			4294905855U,
			4294963455U,
			4294967055U,
			4294967280U
		};
	}
}
