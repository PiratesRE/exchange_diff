using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	public class SearchMailboxResult
	{
		public SearchMailboxResult(ADObjectId identity)
		{
			this.identity = identity;
			this.ResultItemsCount = 0;
			this.Success = false;
		}

		public ADObjectId Identity
		{
			get
			{
				return this.identity;
			}
		}

		public ADObjectId TargetMailbox
		{
			get
			{
				return this.targetMailbox;
			}
			set
			{
				this.targetMailbox = value;
			}
		}

		public bool Success
		{
			get
			{
				return this.success;
			}
			set
			{
				this.success = value;
			}
		}

		public string TargetFolder
		{
			get
			{
				return this.targetFolder;
			}
			set
			{
				this.targetFolder = value;
			}
		}

		public int ResultItemsCount
		{
			get
			{
				return this.resultItemsCount;
			}
			set
			{
				this.resultItemsCount = value;
			}
		}

		public ByteQuantifiedSize ResultItemsSize
		{
			get
			{
				return this.resultItemsSize;
			}
			internal set
			{
				this.resultItemsSize = value;
			}
		}

		internal IDictionary<string, KeywordHit> SubQueryResults
		{
			get
			{
				return this.subQueryResults;
			}
		}

		internal Exception LastException
		{
			get
			{
				return this.lastException;
			}
			set
			{
				this.lastException = value;
			}
		}

		private ADObjectId identity;

		private string targetFolder;

		private ADObjectId targetMailbox;

		private int resultItemsCount;

		private bool success;

		private ByteQuantifiedSize resultItemsSize = ByteQuantifiedSize.FromBytes(0UL);

		private Exception lastException;

		private IDictionary<string, KeywordHit> subQueryResults = new Dictionary<string, KeywordHit>();
	}
}
