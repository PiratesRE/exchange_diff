using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsResponse
{
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "HMSETTINGS:")]
	[Serializable]
	public class PropertiesType
	{
		public AccountStatusType AccountStatus
		{
			get
			{
				return this.accountStatusField;
			}
			set
			{
				this.accountStatusField = value;
			}
		}

		public ParentalControlStatusType ParentalControlStatus
		{
			get
			{
				return this.parentalControlStatusField;
			}
			set
			{
				this.parentalControlStatusField = value;
			}
		}

		public long MailBoxSize
		{
			get
			{
				return this.mailBoxSizeField;
			}
			set
			{
				this.mailBoxSizeField = value;
			}
		}

		public long MaxMailBoxSize
		{
			get
			{
				return this.maxMailBoxSizeField;
			}
			set
			{
				this.maxMailBoxSizeField = value;
			}
		}

		public int MaxAttachments
		{
			get
			{
				return this.maxAttachmentsField;
			}
			set
			{
				this.maxAttachmentsField = value;
			}
		}

		public long MaxMessageSize
		{
			get
			{
				return this.maxMessageSizeField;
			}
			set
			{
				this.maxMessageSizeField = value;
			}
		}

		public long MaxUnencodedMessageSize
		{
			get
			{
				return this.maxUnencodedMessageSizeField;
			}
			set
			{
				this.maxUnencodedMessageSizeField = value;
			}
		}

		public int MaxFilters
		{
			get
			{
				return this.maxFiltersField;
			}
			set
			{
				this.maxFiltersField = value;
			}
		}

		public int MaxFilterClauseValueLength
		{
			get
			{
				return this.maxFilterClauseValueLengthField;
			}
			set
			{
				this.maxFilterClauseValueLengthField = value;
			}
		}

		public int MaxRecipients
		{
			get
			{
				return this.maxRecipientsField;
			}
			set
			{
				this.maxRecipientsField = value;
			}
		}

		public int MaxUserSignatureLength
		{
			get
			{
				return this.maxUserSignatureLengthField;
			}
			set
			{
				this.maxUserSignatureLengthField = value;
			}
		}

		public int BlockListAddressMax
		{
			get
			{
				return this.blockListAddressMaxField;
			}
			set
			{
				this.blockListAddressMaxField = value;
			}
		}

		public int BlockListDomainMax
		{
			get
			{
				return this.blockListDomainMaxField;
			}
			set
			{
				this.blockListDomainMaxField = value;
			}
		}

		public int WhiteListAddressMax
		{
			get
			{
				return this.whiteListAddressMaxField;
			}
			set
			{
				this.whiteListAddressMaxField = value;
			}
		}

		public int WhiteListDomainMax
		{
			get
			{
				return this.whiteListDomainMaxField;
			}
			set
			{
				this.whiteListDomainMaxField = value;
			}
		}

		public int WhiteToListMax
		{
			get
			{
				return this.whiteToListMaxField;
			}
			set
			{
				this.whiteToListMaxField = value;
			}
		}

		public int AlternateFromListMax
		{
			get
			{
				return this.alternateFromListMaxField;
			}
			set
			{
				this.alternateFromListMaxField = value;
			}
		}

		public int MaxDailySendMessages
		{
			get
			{
				return this.maxDailySendMessagesField;
			}
			set
			{
				this.maxDailySendMessagesField = value;
			}
		}

		private AccountStatusType accountStatusField;

		private ParentalControlStatusType parentalControlStatusField;

		private long mailBoxSizeField;

		private long maxMailBoxSizeField;

		private int maxAttachmentsField;

		private long maxMessageSizeField;

		private long maxUnencodedMessageSizeField;

		private int maxFiltersField;

		private int maxFilterClauseValueLengthField;

		private int maxRecipientsField;

		private int maxUserSignatureLengthField;

		private int blockListAddressMaxField;

		private int blockListDomainMaxField;

		private int whiteListAddressMaxField;

		private int whiteListDomainMaxField;

		private int whiteToListMaxField;

		private int alternateFromListMaxField;

		private int maxDailySendMessagesField;
	}
}
