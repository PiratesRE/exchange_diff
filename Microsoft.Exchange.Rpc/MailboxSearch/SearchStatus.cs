using System;

namespace Microsoft.Exchange.Rpc.MailboxSearch
{
	internal class SearchStatus
	{
		public SearchStatus(string searchId, string owner, int status, int percentCompleted, long estimateResultItems, ulong estimateResultSize, long resultItems, ulong resultSize, string resultLink)
		{
			this.m_searchId = searchId;
			this.m_owner = owner;
			this.m_status = status;
			this.m_percentCompleted = percentCompleted;
			this.m_estimateResultItems = estimateResultItems;
			this.m_estimateResultSize = estimateResultSize;
			this.m_resultItems = resultItems;
			this.m_resultSize = resultSize;
			this.m_resultLink = resultLink;
		}

		public string SearchId
		{
			get
			{
				return this.m_searchId;
			}
			set
			{
				this.m_searchId = value;
			}
		}

		public string Owner
		{
			get
			{
				return this.m_owner;
			}
			set
			{
				this.m_owner = value;
			}
		}

		public int Status
		{
			get
			{
				return this.m_status;
			}
			set
			{
				this.m_status = value;
			}
		}

		public int PercentCompleted
		{
			get
			{
				return this.m_percentCompleted;
			}
			set
			{
				this.m_percentCompleted = value;
			}
		}

		public long EstimateResultItems
		{
			get
			{
				return this.m_estimateResultItems;
			}
			set
			{
				this.m_estimateResultItems = value;
			}
		}

		public ulong EstimateResultSize
		{
			get
			{
				return this.m_estimateResultSize;
			}
			set
			{
				this.m_estimateResultSize = value;
			}
		}

		public long ResultItems
		{
			get
			{
				return this.m_resultItems;
			}
			set
			{
				this.m_resultItems = value;
			}
		}

		public ulong ResultSize
		{
			get
			{
				return this.m_resultSize;
			}
			set
			{
				this.m_resultSize = value;
			}
		}

		public string ResultLink
		{
			get
			{
				return this.m_resultLink;
			}
			set
			{
				this.m_resultLink = value;
			}
		}

		private string m_searchId;

		private string m_owner;

		private int m_status;

		private int m_percentCompleted;

		private long m_estimateResultItems;

		private ulong m_estimateResultSize;

		private long m_resultItems;

		private ulong m_resultSize;

		private string m_resultLink;
	}
}
