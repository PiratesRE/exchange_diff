using System;

namespace Microsoft.Exchange.Rpc.MultiMailboxSearch
{
	[Serializable]
	internal sealed class PageReference : MultiMailboxSearchBase
	{
		internal PageReference(int version) : base(version)
		{
		}

		internal PageReference() : base(MultiMailboxSearchBase.CurrentVersion)
		{
		}

		internal MultiMailboxSearchResult PreviousPageReference
		{
			get
			{
				return this.previousPageReferenceItem;
			}
			set
			{
				this.previousPageReferenceItem = value;
			}
		}

		internal MultiMailboxSearchResult NextPageReference
		{
			get
			{
				return this.nextPageReferenceItem;
			}
			set
			{
				this.nextPageReferenceItem = value;
			}
		}

		private MultiMailboxSearchResult previousPageReferenceItem;

		private MultiMailboxSearchResult nextPageReferenceItem;
	}
}
