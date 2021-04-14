using System;

namespace Microsoft.Exchange.Rpc.MultiMailboxSearch
{
	[Serializable]
	internal sealed class MultiMailboxSearchRefinersResult : MultiMailboxSearchResultItem
	{
		internal MultiMailboxSearchRefinersResult(string name, long value) : base(MultiMailboxSearchBase.CurrentVersion)
		{
			this.entryName = name;
			this.entryCount = value;
		}

		internal MultiMailboxSearchRefinersResult(int version, string name, long value) : base(version)
		{
			this.entryName = name;
			this.entryCount = value;
		}

		internal long Value
		{
			get
			{
				return this.entryCount;
			}
		}

		internal string Name
		{
			get
			{
				return this.entryName;
			}
		}

		private readonly long entryCount;

		private readonly string entryName;
	}
}
