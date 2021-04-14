using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal struct ReplId : IEquatable<ReplId>, IFormattable
	{
		public ReplId(ushort replIdValue)
		{
			this.replIdValue = replIdValue;
		}

		internal ushort Value
		{
			get
			{
				return this.replIdValue;
			}
		}

		public static ReplId Parse(Reader reader)
		{
			ushort num = reader.ReadUInt16();
			return new ReplId(num);
		}

		internal void Serialize(Writer writer)
		{
			writer.WriteUInt16(this.replIdValue);
		}

		public bool Equals(ReplId other)
		{
			return this.replIdValue == other.replIdValue;
		}

		public override bool Equals(object other)
		{
			return other is ReplId && this.Equals((ReplId)other);
		}

		public override int GetHashCode()
		{
			return this.replIdValue.GetHashCode();
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format != null && (format == "B" || format == "X"))
			{
				return this.replIdValue.ToString("X", formatProvider);
			}
			return string.Format(formatProvider, "ReplId: {0:X}", new object[]
			{
				this.replIdValue
			});
		}

		public override string ToString()
		{
			return this.ToString(null, null);
		}

		public string ToBareString()
		{
			return this.ToString("B", null);
		}

		private readonly ushort replIdValue;
	}
}
