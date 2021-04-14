using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct LID
	{
		private LID(uint value)
		{
			this.value = value;
		}

		public uint Value
		{
			get
			{
				return this.value;
			}
		}

		public static explicit operator LID(uint value)
		{
			return new LID(value);
		}

		private uint value;
	}
}
