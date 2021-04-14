using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal class SearchProgressEvent : EventArgs
	{
		internal SearchProgressEvent(LocalizedString activity, LocalizedString statusDescription, int percentCompleted) : this(activity, statusDescription, percentCompleted, 0L, ByteQuantifiedSize.Zero)
		{
		}

		internal SearchProgressEvent(LocalizedString activity, LocalizedString statusDescription, int percentCompleted, long resultItems, ByteQuantifiedSize resultSize)
		{
			this.activity = activity;
			this.statusDescription = statusDescription;
			this.percentCompleted = percentCompleted;
			this.resultItems = resultItems;
			this.resultSize = resultSize;
		}

		internal LocalizedString Activity
		{
			get
			{
				return this.activity;
			}
		}

		internal LocalizedString StatusDescription
		{
			get
			{
				return this.statusDescription;
			}
		}

		internal int PercentCompleted
		{
			get
			{
				return this.percentCompleted;
			}
		}

		internal long ResultItems
		{
			get
			{
				return this.resultItems;
			}
		}

		internal ByteQuantifiedSize ResultSize
		{
			get
			{
				return this.resultSize;
			}
		}

		private LocalizedString activity;

		private LocalizedString statusDescription;

		private int percentCompleted;

		private long resultItems;

		private ByteQuantifiedSize resultSize;
	}
}
