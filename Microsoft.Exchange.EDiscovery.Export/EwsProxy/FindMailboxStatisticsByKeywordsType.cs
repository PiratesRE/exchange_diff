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
	public class FindMailboxStatisticsByKeywordsType : BaseRequestType
	{
		[XmlArrayItem("UserMailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public UserMailboxType[] Mailboxes
		{
			get
			{
				return this.mailboxesField;
			}
			set
			{
				this.mailboxesField = value;
			}
		}

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Keywords
		{
			get
			{
				return this.keywordsField;
			}
			set
			{
				this.keywordsField = value;
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

		[XmlArrayItem("SmtpAddress", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Senders
		{
			get
			{
				return this.sendersField;
			}
			set
			{
				this.sendersField = value;
			}
		}

		[XmlArrayItem("SmtpAddress", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Recipients
		{
			get
			{
				return this.recipientsField;
			}
			set
			{
				this.recipientsField = value;
			}
		}

		public DateTime FromDate
		{
			get
			{
				return this.fromDateField;
			}
			set
			{
				this.fromDateField = value;
			}
		}

		[XmlIgnore]
		public bool FromDateSpecified
		{
			get
			{
				return this.fromDateFieldSpecified;
			}
			set
			{
				this.fromDateFieldSpecified = value;
			}
		}

		public DateTime ToDate
		{
			get
			{
				return this.toDateField;
			}
			set
			{
				this.toDateField = value;
			}
		}

		[XmlIgnore]
		public bool ToDateSpecified
		{
			get
			{
				return this.toDateFieldSpecified;
			}
			set
			{
				this.toDateFieldSpecified = value;
			}
		}

		[XmlArrayItem("SearchItemKind", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public SearchItemKindType[] MessageTypes
		{
			get
			{
				return this.messageTypesField;
			}
			set
			{
				this.messageTypesField = value;
			}
		}

		public bool SearchDumpster
		{
			get
			{
				return this.searchDumpsterField;
			}
			set
			{
				this.searchDumpsterField = value;
			}
		}

		[XmlIgnore]
		public bool SearchDumpsterSpecified
		{
			get
			{
				return this.searchDumpsterFieldSpecified;
			}
			set
			{
				this.searchDumpsterFieldSpecified = value;
			}
		}

		public bool IncludePersonalArchive
		{
			get
			{
				return this.includePersonalArchiveField;
			}
			set
			{
				this.includePersonalArchiveField = value;
			}
		}

		[XmlIgnore]
		public bool IncludePersonalArchiveSpecified
		{
			get
			{
				return this.includePersonalArchiveFieldSpecified;
			}
			set
			{
				this.includePersonalArchiveFieldSpecified = value;
			}
		}

		public bool IncludeUnsearchableItems
		{
			get
			{
				return this.includeUnsearchableItemsField;
			}
			set
			{
				this.includeUnsearchableItemsField = value;
			}
		}

		[XmlIgnore]
		public bool IncludeUnsearchableItemsSpecified
		{
			get
			{
				return this.includeUnsearchableItemsFieldSpecified;
			}
			set
			{
				this.includeUnsearchableItemsFieldSpecified = value;
			}
		}

		private UserMailboxType[] mailboxesField;

		private string[] keywordsField;

		private string languageField;

		private string[] sendersField;

		private string[] recipientsField;

		private DateTime fromDateField;

		private bool fromDateFieldSpecified;

		private DateTime toDateField;

		private bool toDateFieldSpecified;

		private SearchItemKindType[] messageTypesField;

		private bool searchDumpsterField;

		private bool searchDumpsterFieldSpecified;

		private bool includePersonalArchiveField;

		private bool includePersonalArchiveFieldSpecified;

		private bool includeUnsearchableItemsField;

		private bool includeUnsearchableItemsFieldSpecified;
	}
}
