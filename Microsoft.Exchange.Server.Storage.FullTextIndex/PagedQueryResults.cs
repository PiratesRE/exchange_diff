using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Ceres.InteractionEngine.Services.Exchange;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.FullTextIndex
{
	public class PagedQueryResults : DisposableBase
	{
		public IEnumerator PagedResults
		{
			get
			{
				return this.pagedResults;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return this.pagedResults != null;
			}
		}

		public void Initialize(IEnumerator<SearchResultItem[]> pagedResults)
		{
			this.pagedResults = (pagedResults ?? new List<SearchResultItem[]>(0).GetEnumerator());
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PagedQueryResults>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.pagedResults != null)
			{
				this.pagedResults.Dispose();
				this.pagedResults = null;
			}
		}

		private IEnumerator<SearchResultItem[]> pagedResults;
	}
}
