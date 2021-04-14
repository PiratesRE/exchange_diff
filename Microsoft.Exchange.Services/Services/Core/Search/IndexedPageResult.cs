using System;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal class IndexedPageResult : BasePageResult
	{
		public IndexedPageResult(BaseQueryView view, int indexedOffset) : base(view)
		{
			this.indexedOffset = indexedOffset;
		}

		public int IndexedOffset
		{
			get
			{
				return this.indexedOffset;
			}
		}

		private int indexedOffset;
	}
}
