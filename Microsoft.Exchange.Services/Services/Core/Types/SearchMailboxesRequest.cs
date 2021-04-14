using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core.Search;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "SearchMailboxesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Name = "SearchMailboxesRequest", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class SearchMailboxesRequest : BaseRequest
	{
		[IgnoreDataMember]
		[XmlArrayItem(ElementName = "MailboxQuery", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(MailboxQuery))]
		[XmlArray(ElementName = "SearchQueries", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
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

		[DataMember(Name = "SearchId", IsRequired = true)]
		[XmlIgnore]
		public string SearchId
		{
			get
			{
				return this.searchId;
			}
			set
			{
				this.searchId = value;
			}
		}

		[DataMember(Name = "MailboxId", IsRequired = false)]
		[XmlIgnore]
		public string MailboxId
		{
			get
			{
				return this.mailboxId;
			}
			set
			{
				this.mailboxId = value;
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

		[XmlIgnore]
		[DataMember(Name = "ResultType", IsRequired = true)]
		public string ResultTypeString
		{
			get
			{
				return EnumUtilities.ToString<SearchResultType>(this.resultType);
			}
			set
			{
				this.resultType = EnumUtilities.Parse<SearchResultType>(value);
			}
		}

		[DataMember(Name = "PreviewItemResponseShape", IsRequired = false)]
		[XmlElement("PreviewItemResponseShape")]
		public PreviewItemResponseShape PreviewItemResponseShape
		{
			get
			{
				return this.previewItemResponseShape;
			}
			set
			{
				this.previewItemResponseShape = value;
			}
		}

		[XmlElement("SortBy")]
		[DataMember(Name = "SortBy", IsRequired = false)]
		public SortResults SortBy
		{
			get
			{
				return this.sortBy;
			}
			set
			{
				this.sortBy = value;
			}
		}

		[DataMember(Name = "Language", IsRequired = false)]
		[XmlElement("Language")]
		public string Language
		{
			get
			{
				return this.language;
			}
			set
			{
				this.language = value;
			}
		}

		[XmlElement("Deduplication")]
		[DataMember(Name = "Deduplication", IsRequired = false)]
		public bool Deduplication
		{
			get
			{
				return this.deduplication;
			}
			set
			{
				this.deduplication = value;
			}
		}

		[XmlElement("PageSize")]
		[DataMember(Name = "PageSize", IsRequired = false)]
		public int PageSize
		{
			get
			{
				return this.pageSize;
			}
			set
			{
				this.pageSize = value;
			}
		}

		[XmlElement("PageItemReference")]
		[DataMember(Name = "PageItemReference", IsRequired = false)]
		public string PageItemReference
		{
			get
			{
				return this.pageItemReference;
			}
			set
			{
				this.pageItemReference = value;
			}
		}

		[IgnoreDataMember]
		[XmlElement("PageDirection")]
		public SearchPageDirectionType PageDirection
		{
			get
			{
				return this.pageDirection;
			}
			set
			{
				this.pageDirection = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "PageDirection", IsRequired = true)]
		public string PageDirectionString
		{
			get
			{
				return EnumUtilities.ToString<SearchPageDirectionType>(this.pageDirection);
			}
			set
			{
				this.pageDirection = EnumUtilities.Parse<SearchPageDirectionType>(value);
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new SearchMailboxes(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int currentStep)
		{
			return null;
		}

		private MailboxQuery[] searchQueries;

		private string searchId;

		private string mailboxId;

		private SearchResultType resultType;

		private PreviewItemResponseShape previewItemResponseShape;

		private SortResults sortBy;

		private string language;

		private bool deduplication;

		private int pageSize;

		private string pageItemReference;

		private SearchPageDirectionType pageDirection;
	}
}
