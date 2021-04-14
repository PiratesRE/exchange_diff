using System;

namespace Microsoft.Exchange.Rpc.MultiMailboxSearch
{
	[Serializable]
	internal sealed class PagingInfo : MultiMailboxSearchBase
	{
		internal PagingInfo(int version, int pageSize, PagingDirection direction, long referenceDocumentId, PagingReferenceItem pagingReferenceItem) : base(version)
		{
			this.pageSize = pageSize;
			this.direction = direction;
			this.referenceDocumentId = referenceDocumentId;
			this.pagingReferenceItem = pagingReferenceItem;
		}

		internal PagingInfo(int pageSize, PagingDirection direction, long referenceDocumentId, PagingReferenceItem pagingReferenceItem) : base(MultiMailboxSearchBase.CurrentVersion)
		{
			this.pageSize = pageSize;
			this.direction = direction;
			this.referenceDocumentId = referenceDocumentId;
			this.pagingReferenceItem = pagingReferenceItem;
		}

		internal int PageSize
		{
			get
			{
				return this.pageSize;
			}
		}

		internal PagingDirection Direction
		{
			get
			{
				return this.direction;
			}
		}

		internal long ReferenceDocumentId
		{
			get
			{
				return this.referenceDocumentId;
			}
		}

		internal PagingReferenceItem ReferenceItem
		{
			get
			{
				return this.pagingReferenceItem;
			}
		}

		private readonly int pageSize;

		private readonly PagingDirection direction;

		private readonly long referenceDocumentId;

		private readonly PagingReferenceItem pagingReferenceItem;
	}
}
