using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class UnlimitedBytes : Unlimited<long>
	{
		public UnlimitedBytes()
		{
		}

		public UnlimitedBytes(long value) : base(value)
		{
		}

		public long KB
		{
			get
			{
				return base.Value >> 10;
			}
		}

		public long MB
		{
			get
			{
				return base.Value >> 20;
			}
		}

		public long GB
		{
			get
			{
				return base.Value >> 30;
			}
		}

		public static UnlimitedBytes FromKB(long kiloBytes)
		{
			return new UnlimitedBytes(kiloBytes << 10);
		}

		public static UnlimitedBytes FromMB(long megaBytes)
		{
			return new UnlimitedBytes(megaBytes << 20);
		}

		public static UnlimitedBytes FromGB(long gigaBytes)
		{
			return new UnlimitedBytes(gigaBytes << 30);
		}

		public static UnlimitedBytes operator +(UnlimitedBytes left, long right)
		{
			if (!left.IsUnlimited)
			{
				return new UnlimitedBytes(left.Value + right);
			}
			return left;
		}

		public static UnlimitedBytes operator *(UnlimitedBytes left, long right)
		{
			if (!left.IsUnlimited)
			{
				return new UnlimitedBytes(left.Value * right);
			}
			return left;
		}

		public static UnlimitedBytes operator /(UnlimitedBytes left, long right)
		{
			if (!left.IsUnlimited)
			{
				return new UnlimitedBytes(left.Value / right);
			}
			return left;
		}

		public new static UnlimitedBytes UnlimitedValue = new UnlimitedBytes();
	}
}
