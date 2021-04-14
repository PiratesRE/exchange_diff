using System;

namespace Microsoft.Exchange.Rpc.MultiMailboxSearch
{
	[Serializable]
	internal abstract class MultiMailboxResponseBase : MultiMailboxSearchBase
	{
		internal MultiMailboxResponseBase(int version) : base(version)
		{
		}

		internal MultiMailboxResponseBase() : base(MultiMailboxSearchBase.CurrentVersion)
		{
		}

		internal MultiMailboxSearchResultItem[] Results
		{
			get
			{
				return this.results;
			}
			set
			{
				this.results = value;
				if (value != null)
				{
					this.count = (long)value.Length;
				}
			}
		}

		internal long Count
		{
			get
			{
				return this.count;
			}
			set
			{
				this.count = value;
			}
		}

		private MultiMailboxSearchResultItem[] results;

		private long count;
	}
}
