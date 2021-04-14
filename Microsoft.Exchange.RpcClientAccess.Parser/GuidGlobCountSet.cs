using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal struct GuidGlobCountSet : IEquatable<GuidGlobCountSet>
	{
		public GuidGlobCountSet(Guid guid, GlobCountSet globCountSet)
		{
			this.guid = guid;
			this.globCountSet = globCountSet;
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		public GlobCountSet GlobCountSet
		{
			get
			{
				return this.globCountSet;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.globCountSet == null || this.globCountSet.IsEmpty;
			}
		}

		public int CountRanges
		{
			get
			{
				if (this.globCountSet != null)
				{
					return this.globCountSet.CountRanges;
				}
				return 0;
			}
		}

		public ulong CountIds
		{
			get
			{
				if (this.globCountSet != null)
				{
					return this.globCountSet.CountIds;
				}
				return 0UL;
			}
		}

		public GuidGlobCountSet Clone()
		{
			return new GuidGlobCountSet(this.guid, this.globCountSet.Clone());
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", this.guid, this.globCountSet);
		}

		public bool Equals(GuidGlobCountSet other)
		{
			return this.guid.Equals(other.guid) && this.globCountSet.Equals(other.globCountSet);
		}

		public override bool Equals(object obj)
		{
			return obj is GuidGlobCountSet && this.Equals((GuidGlobCountSet)obj);
		}

		public override int GetHashCode()
		{
			return this.guid.GetHashCode() ^ this.globCountSet.GetHashCode();
		}

		private readonly Guid guid;

		private readonly GlobCountSet globCountSet;
	}
}
