using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal abstract class ABit
	{
		public static Func<int[], uint, int> GetBitsFunc
		{
			get
			{
				return null;
			}
		}

		public static Action<int[], uint, int> SetBitsAction
		{
			get
			{
				return null;
			}
		}

		public static Func<uint, int> GetPhysicalIndexFunc
		{
			get
			{
				return null;
			}
		}

		public static Func<int, uint, int, int> NewValueInPhysicalIndexFunc
		{
			get
			{
				return null;
			}
		}

		public static Func<int, uint, int> CurrentValueInLogicIndexFunc
		{
			get
			{
				return null;
			}
		}

		public const int BitsForCount = 0;
	}
}
