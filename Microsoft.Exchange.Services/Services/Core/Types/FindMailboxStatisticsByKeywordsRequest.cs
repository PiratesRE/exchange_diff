using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "FindMailboxStatisticsByKeywordsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class FindMailboxStatisticsByKeywordsRequest : BaseRequest
	{
		[XmlArray(ElementName = "Mailboxes", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArrayItem(ElementName = "UserMailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(UserMailbox))]
		public UserMailbox[] Mailboxes
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

		[XmlArray(ElementName = "Keywords", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArrayItem(ElementName = "String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
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

		[XmlElement("Language")]
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

		[XmlArrayItem(ElementName = "SmtpAddress", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		[XmlArray(ElementName = "Senders", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
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

		[XmlArray(ElementName = "Recipients", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArrayItem(ElementName = "SmtpAddress", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
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

		[XmlElement("FromDate")]
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

		[XmlElement("ToDate")]
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

		[XmlArrayItem(ElementName = "SearchItemKind", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(SearchItemKind))]
		[XmlArray(ElementName = "MessageTypes", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public SearchItemKind[] MessageTypes
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

		[XmlElement("SearchDumpster")]
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

		[XmlElement("IncludePersonalArchive")]
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

		[XmlElement("IncludeUnsearchableItems")]
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

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new FindMailboxStatisticsByKeywords(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.Mailboxes == null || this.Mailboxes.Length == 0)
			{
				return null;
			}
			string id = this.Mailboxes[0].Id;
			MailboxId mailboxId;
			if (this.Mailboxes[0].IsArchive)
			{
				mailboxId = new MailboxId(new Guid(id), true);
			}
			else
			{
				mailboxId = new MailboxId(id);
			}
			return MailboxIdServerInfo.Create(mailboxId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int currentStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		private UserMailbox[] mailboxesField;

		private string[] keywordsField;

		private string languageField;

		private string[] sendersField;

		private string[] recipientsField;

		private DateTime fromDateField;

		private bool fromDateFieldSpecified;

		private DateTime toDateField;

		private bool toDateFieldSpecified;

		private SearchItemKind[] messageTypesField;

		private bool searchDumpsterField;

		private bool searchDumpsterFieldSpecified;

		private bool includePersonalArchiveField;

		private bool includePersonalArchiveFieldSpecified;

		private bool includeUnsearchableItemsField;

		private bool includeUnsearchableItemsFieldSpecified;
	}
}
