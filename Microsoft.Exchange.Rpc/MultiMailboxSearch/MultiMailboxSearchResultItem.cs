using System;

namespace Microsoft.Exchange.Rpc.MultiMailboxSearch
{
	[Serializable]
	internal abstract class MultiMailboxSearchResultItem : MultiMailboxSearchBase
	{
		protected MultiMailboxSearchResultItem(int version) : base(version)
		{
		}

		protected MultiMailboxSearchResultItem() : base(MultiMailboxSearchBase.CurrentVersion)
		{
		}
	}
}
