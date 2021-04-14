using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class SearchMailboxesType : BaseRequestType
	{
		[XmlArrayItem("MailboxQuery", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
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

		public PreviewItemResponseShapeType PreviewItemResponseShape
		{
			get
			{
				return this.previewItemResponseShapeField;
			}
			set
			{
				this.previewItemResponseShapeField = value;
			}
		}

		public FieldOrderType SortBy
		{
			get
			{
				return this.sortByField;
			}
			set
			{
				this.sortByField = value;
			}
		}

		public string Language
		{
			get
			{
				return this.languageField;
			}
			set
			{
				this.languageField = value;
			}
		}

		public bool Deduplication
		{
			get
			{
				return this.deduplicationField;
			}
			set
			{
				this.deduplicationField = value;
			}
		}

		[XmlIgnore]
		public bool DeduplicationSpecified
		{
			get
			{
				return this.deduplicationFieldSpecified;
			}
			set
			{
				this.deduplicationFieldSpecified = value;
			}
		}

		public int PageSize
		{
			get
			{
				return this.pageSizeField;
			}
			set
			{
				this.pageSizeField = value;
			}
		}

		[XmlIgnore]
		public bool PageSizeSpecified
		{
			get
			{
				return this.pageSizeFieldSpecified;
			}
			set
			{
				this.pageSizeFieldSpecified = value;
			}
		}

		public string PageItemReference
		{
			get
			{
				return this.pageItemReferenceField;
			}
			set
			{
				this.pageItemReferenceField = value;
			}
		}

		public SearchPageDirectionType PageDirection
		{
			get
			{
				return this.pageDirectionField;
			}
			set
			{
				this.pageDirectionField = value;
			}
		}

		[XmlIgnore]
		public bool PageDirectionSpecified
		{
			get
			{
				return this.pageDirectionFieldSpecified;
			}
			set
			{
				this.pageDirectionFieldSpecified = value;
			}
		}

		private MailboxQueryType[] searchQueriesField;

		private SearchResultType resultTypeField;

		private PreviewItemResponseShapeType previewItemResponseShapeField;

		private FieldOrderType sortByField;

		private string languageField;

		private bool deduplicationField;

		private bool deduplicationFieldSpecified;

		private int pageSizeField;

		private bool pageSizeFieldSpecified;

		private string pageItemReferenceField;

		private SearchPageDirectionType pageDirectionField;

		private bool pageDirectionFieldSpecified;
	}
}
