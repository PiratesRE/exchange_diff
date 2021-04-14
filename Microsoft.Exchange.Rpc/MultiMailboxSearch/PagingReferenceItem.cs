using System;

namespace Microsoft.Exchange.Rpc.MultiMailboxSearch
{
	[Serializable]
	internal sealed class PagingReferenceItem : MultiMailboxSearchBase
	{
		internal PagingReferenceItem(int version, byte[] equalsRestriction, byte[] comparisionFilterRestriction) : base(version)
		{
			this.equalsRestriction = equalsRestriction;
			this.comparisionFilterRestriction = comparisionFilterRestriction;
		}

		internal PagingReferenceItem(byte[] equalsRestriction, byte[] comparisionFilterRestriction) : base(MultiMailboxSearchBase.CurrentVersion)
		{
			this.equalsRestriction = equalsRestriction;
			this.comparisionFilterRestriction = comparisionFilterRestriction;
		}

		internal byte[] EqualsRestriction
		{
			get
			{
				return this.equalsRestriction;
			}
		}

		internal byte[] ComparisionRestriction
		{
			get
			{
				return this.comparisionFilterRestriction;
			}
		}

		private readonly byte[] equalsRestriction;

		private readonly byte[] comparisionFilterRestriction;
	}
}
