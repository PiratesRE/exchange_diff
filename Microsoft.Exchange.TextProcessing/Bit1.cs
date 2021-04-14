using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal class Bit1 : ABit
	{
		public new static Func<int[], uint, int> GetBitsFunc
		{
			get
			{
				return (int[] array, uint index) => array[(int)((UIntPtr)(index >> 5))] >> (int)(31U - (index & 31U)) & 1;
			}
		}

		public new static Action<int[], uint, int> SetBitsAction
		{
			get
			{
				return delegate(int[] array, uint index, int value)
				{
					array[(int)((UIntPtr)(index >> 5))] |= 1 << (int)(31U - (index & 31U));
				};
			}
		}

		public new static Func<uint, int> GetPhysicalIndexFunc
		{
			get
			{
				return null;
			}
		}

		public new static Func<int, uint, int, int> NewValueInPhysicalIndexFunc
		{
			get
			{
				return null;
			}
		}

		public new static Func<int, uint, int> CurrentValueInLogicIndexFunc
		{
			get
			{
				return null;
			}
		}

		public new const int BitsForCount = 1;
	}
}
