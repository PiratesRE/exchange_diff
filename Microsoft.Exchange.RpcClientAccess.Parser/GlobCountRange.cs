using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	public struct GlobCountRange : IEquatable<GlobCountRange>
	{
		public GlobCountRange(ulong lowBound, ulong highBound)
		{
			GlobCountSet.VerifyGlobCountArgument(lowBound, "lowBound");
			GlobCountSet.VerifyGlobCountArgument(highBound, "highBound");
			if (lowBound > highBound)
			{
				throw new ArgumentException("Lowbound should be less or equal to highbound");
			}
			this.lowBound = lowBound;
			this.highBound = highBound;
		}

		public ulong LowBound
		{
			get
			{
				return this.lowBound;
			}
		}

		public ulong HighBound
		{
			get
			{
				return this.highBound;
			}
		}

		public override bool Equals(object other)
		{
			return other is GlobCountRange && this.Equals((GlobCountRange)other);
		}

		public override int GetHashCode()
		{
			return this.lowBound.GetHashCode() ^ this.highBound.GetHashCode();
		}

		public bool Equals(GlobCountRange other)
		{
			return this.highBound == other.highBound && this.lowBound == other.lowBound;
		}

		public GlobCountRange Clone()
		{
			return new GlobCountRange(this.lowBound, this.highBound);
		}

		public override string ToString()
		{
			return string.Format("[0x{0:X}, 0x{1:X}]", this.lowBound, this.highBound);
		}

		internal bool Contains(ulong element)
		{
			return element >= this.lowBound && element <= this.highBound;
		}

		private readonly ulong lowBound;

		private readonly ulong highBound;
	}
}
