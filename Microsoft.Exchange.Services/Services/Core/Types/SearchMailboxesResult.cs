using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "SearchMailboxesResultType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "SearchMailboxesResult", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class SearchMailboxesResult
	{
		[XmlArray(ElementName = "SearchQueries", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(Name = "SearchQueries", EmitDefaultValue = false)]
		[XmlArrayItem(ElementName = "MailboxQuery", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(MailboxQuery))]
		public MailboxQuery[] SearchQueries
		{
			get
			{
				return this.searchQueries;
			}
			set
			{
				this.searchQueries = value;
			}
		}

		[IgnoreDataMember]
		[XmlElement("ResultType")]
		public SearchResultType ResultType
		{
			get
			{
				return this.resultType;
			}
			set
			{
				this.resultType = value;
			}
		}

		[DataMember(Name = "ResultType", EmitDefaultValue = false)]
		[XmlIgnore]
		public string ResultTypeString
		{
			get
			{
				return EnumUtilities.ToString<SearchResultType>(this.ResultType);
			}
			set
			{
				this.ResultType = EnumUtilities.Parse<SearchResultType>(value);
			}
		}

		[DataMember(Name = "ItemCount", EmitDefaultValue = true)]
		[XmlElement("ItemCount")]
		public ulong ItemCount
		{
			get
			{
				return this.itemCount;
			}
			set
			{
				this.itemCount = value;
			}
		}

		[DataMember(Name = "Size", EmitDefaultValue = true)]
		[XmlElement("Size")]
		public ulong Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		[DataMember(Name = "PageItemCount", EmitDefaultValue = false)]
		[XmlElement("PageItemCount")]
		public int PageItemCount
		{
			get
			{
				return this.pageItemCount;
			}
			set
			{
				this.pageItemCount = value;
			}
		}

		[DataMember(Name = "PageItemSize", EmitDefaultValue = false)]
		[XmlElement("PageItemSize")]
		public ulong PageItemSize
		{
			get
			{
				return this.pageItemSize;
			}
			set
			{
				this.pageItemSize = value;
			}
		}

		[XmlArrayItem(ElementName = "KeywordStat", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(KeywordStatisticsSearchResult))]
		[DataMember(Name = "KeywordStats", EmitDefaultValue = false)]
		[XmlArray(ElementName = "KeywordStats", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public KeywordStatisticsSearchResult[] KeywordStats
		{
			get
			{
				return this.keywordStats;
			}
			set
			{
				this.keywordStats = value;
			}
		}

		[DataMember(Name = "Items", EmitDefaultValue = false)]
		[XmlArray(ElementName = "Items", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem(ElementName = "SearchPreviewItem", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(SearchPreviewItem))]
		public SearchPreviewItem[] Items
		{
			get
			{
				return this.previewItems;
			}
			set
			{
				this.previewItems = value;
			}
		}

		[XmlArrayItem(ElementName = "FailedMailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(FailedSearchMailbox))]
		[DataMember(Name = "FailedMailboxes", EmitDefaultValue = false)]
		[XmlArray(ElementName = "FailedMailboxes", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public FailedSearchMailbox[] FailedMailboxes
		{
			get
			{
				return this.failedMailboxes;
			}
			set
			{
				this.failedMailboxes = value;
			}
		}

		[XmlArrayItem(ElementName = "Refiner", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(SearchRefinerItem))]
		[DataMember(Name = "Refiners", EmitDefaultValue = false)]
		[XmlArray(ElementName = "Refiners", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public SearchRefinerItem[] Refiners { get; set; }

		[XmlArray(ElementName = "MailboxStats", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(Name = "MailboxStats", EmitDefaultValue = false)]
		[XmlArrayItem(ElementName = "MailboxStat", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(MailboxStatisticsItem))]
		public MailboxStatisticsItem[] MailboxStats { get; set; }

		private MailboxQuery[] searchQueries;

		private SearchResultType resultType;

		private ulong itemCount;

		private ulong size;

		private int pageItemCount;

		private ulong pageItemSize;

		private KeywordStatisticsSearchResult[] keywordStats;

		private SearchPreviewItem[] previewItems;

		private FailedSearchMailbox[] failedMailboxes;
	}
}
