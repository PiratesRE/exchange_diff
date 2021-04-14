using System;

namespace Microsoft.Exchange.Rpc.IPFilter
{
	internal class IPFilterRange
	{
		public int Identity
		{
			get
			{
				return this.identity;
			}
			set
			{
				this.identity = value;
			}
		}

		public int Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		public DateTime ExpiresOn
		{
			get
			{
				return this.expiresOn;
			}
			set
			{
				this.expiresOn = value;
			}
		}

		public string Comment
		{
			get
			{
				return this.comment;
			}
			set
			{
				this.comment = value;
			}
		}

		public unsafe void GetLowerBound(ulong* high, ulong* low)
		{
			*high = this.highLowerBound;
			*low = this.lowLowerBound;
		}

		public void GetLowerBound(out ulong high, out ulong low)
		{
			high = this.highLowerBound;
			low = this.lowLowerBound;
		}

		public void SetLowerBound(ulong high, ulong low)
		{
			this.highLowerBound = high;
			this.lowLowerBound = low;
		}

		public unsafe void GetUpperBound(ulong* high, ulong* low)
		{
			*high = this.highUpperBound;
			*low = this.lowUpperBound;
		}

		public void GetUpperBound(out ulong high, out ulong low)
		{
			high = this.highUpperBound;
			low = this.lowUpperBound;
		}

		public void SetUpperBound(ulong high, ulong low)
		{
			this.highUpperBound = high;
			this.lowUpperBound = low;
		}

		private int identity;

		private DateTime expiresOn;

		private ulong highLowerBound;

		private ulong lowLowerBound;

		private ulong highUpperBound;

		private ulong lowUpperBound;

		private int flags;

		private string comment;
	}
}
