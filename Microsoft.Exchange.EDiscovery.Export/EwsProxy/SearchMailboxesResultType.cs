using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class SearchMailboxesResultType
	{
		[XmlArrayItem("MailboxQuery", IsNullable = false)]
		public MailboxQueryType[] SearchQueries
		{
			get
			{
				return this.searchQueriesField;
			}
			set
			{
				this.searchQueriesField = value;
			}
		}

		public SearchResultType ResultType
		{
			get
			{
				return this.resultTypeField;
			}
			set
			{
				this.resultTypeField = value;
			}
		}

		public long ItemCount
		{
			get
			{
				return this.itemCountField;
			}
			set
			{
				this.itemCountField = value;
			}
		}

		public long Size
		{
			get
			{
				return this.sizeField;
			}
			set
			{
				this.sizeField = value;
			}
		}

		public int PageItemCount
		{
			get
			{
				return this.pageItemCountField;
			}
			set
			{
				this.pageItemCountField = value;
			}
		}

		public long PageItemSize
		{
			get
			{
				return this.pageItemSizeField;
			}
			set
			{
				this.pageItemSizeField = value;
			}
		}

		[XmlArrayItem("KeywordStat", IsNullable = false)]
		public KeywordStatisticsSearchResultType[] KeywordStats
		{
			get
			{
				return this.keywordStatsField;
			}
			set
			{
				this.keywordStatsField = value;
			}
		}

		[XmlArrayItem("SearchPreviewItem", IsNullable = false)]
		public SearchPreviewItemType[] Items
		{
			get
			{
				return this.itemsField;
			}
			set
			{
				this.itemsField = value;
			}
		}

		[XmlArrayItem("FailedMailbox", IsNullable = false)]
		public FailedSearchMailboxType[] FailedMailboxes
		{
			get
			{
				return this.failedMailboxesField;
			}
			set
			{
				this.failedMailboxesField = value;
			}
		}

		[XmlArrayItem("Refiner", IsNullable = false)]
		public SearchRefinerItemType[] Refiners
		{
			get
			{
				return this.refinersField;
			}
			set
			{
				this.refinersField = value;
			}
		}

		[XmlArrayItem("MailboxStat", IsNullable = false)]
		public MailboxStatisticsItemType[] MailboxStats
		{
			get
			{
				return this.mailboxStatsField;
			}
			set
			{
				this.mailboxStatsField = value;
			}
		}

		private MailboxQueryType[] searchQueriesField;

		private SearchResultType resultTypeField;

		private long itemCountField;

		private long sizeField;

		private int pageItemCountField;

		private long pageItemSizeField;

		private KeywordStatisticsSearchResultType[] keywordStatsField;

		private SearchPreviewItemType[] itemsField;

		private FailedSearchMailboxType[] failedMailboxesField;

		private SearchRefinerItemType[] refinersField;

		private MailboxStatisticsItemType[] mailboxStatsField;
	}
}
