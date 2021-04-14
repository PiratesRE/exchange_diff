using System;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class NonIndexableItemPagingInfo
	{
		public NonIndexableItemPagingInfo(int pageSize, string pageReferenceItem)
		{
			this.PageSize = pageSize;
			this.PageItemReference = pageReferenceItem;
		}

		public int PageSize { get; private set; }

		public string PageItemReference { get; private set; }

		public PageDirection PageDirection
		{
			get
			{
				return PageDirection.Next;
			}
		}
	}
}
