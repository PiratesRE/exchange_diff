using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsResponse
{
	[XmlType(TypeName = "PropertiesType", Namespace = "HMSETTINGS:")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class PropertiesType
	{
		[XmlIgnore]
		public AccountStatusType AccountStatus
		{
			get
			{
				return this.internalAccountStatus;
			}
			set
			{
				this.internalAccountStatus = value;
				this.internalAccountStatusSpecified = true;
			}
		}

		[XmlIgnore]
		public ParentalControlStatusType ParentalControlStatus
		{
			get
			{
				return this.internalParentalControlStatus;
			}
			set
			{
				this.internalParentalControlStatus = value;
				this.internalParentalControlStatusSpecified = true;
			}
		}

		[XmlIgnore]
		public long MailBoxSize
		{
			get
			{
				return this.internalMailBoxSize;
			}
			set
			{
				this.internalMailBoxSize = value;
				this.internalMailBoxSizeSpecified = true;
			}
		}

		[XmlIgnore]
		public long MaxMailBoxSize
		{
			get
			{
				return this.internalMaxMailBoxSize;
			}
			set
			{
				this.internalMaxMailBoxSize = value;
				this.internalMaxMailBoxSizeSpecified = true;
			}
		}

		[XmlIgnore]
		public int MaxAttachments
		{
			get
			{
				return this.internalMaxAttachments;
			}
			set
			{
				this.internalMaxAttachments = value;
				this.internalMaxAttachmentsSpecified = true;
			}
		}

		[XmlIgnore]
		public long MaxMessageSize
		{
			get
			{
				return this.internalMaxMessageSize;
			}
			set
			{
				this.internalMaxMessageSize = value;
				this.internalMaxMessageSizeSpecified = true;
			}
		}

		[XmlIgnore]
		public long MaxUnencodedMessageSize
		{
			get
			{
				return this.internalMaxUnencodedMessageSize;
			}
			set
			{
				this.internalMaxUnencodedMessageSize = value;
				this.internalMaxUnencodedMessageSizeSpecified = true;
			}
		}

		[XmlIgnore]
		public int MaxFilters
		{
			get
			{
				return this.internalMaxFilters;
			}
			set
			{
				this.internalMaxFilters = value;
				this.internalMaxFiltersSpecified = true;
			}
		}

		[XmlIgnore]
		public int MaxFilterClauseValueLength
		{
			get
			{
				return this.internalMaxFilterClauseValueLength;
			}
			set
			{
				this.internalMaxFilterClauseValueLength = value;
				this.internalMaxFilterClauseValueLengthSpecified = true;
			}
		}

		[XmlIgnore]
		public int MaxRecipients
		{
			get
			{
				return this.internalMaxRecipients;
			}
			set
			{
				this.internalMaxRecipients = value;
				this.internalMaxRecipientsSpecified = true;
			}
		}

		[XmlIgnore]
		public int MaxUserSignatureLength
		{
			get
			{
				return this.internalMaxUserSignatureLength;
			}
			set
			{
				this.internalMaxUserSignatureLength = value;
				this.internalMaxUserSignatureLengthSpecified = true;
			}
		}

		[XmlIgnore]
		public int BlockListAddressMax
		{
			get
			{
				return this.internalBlockListAddressMax;
			}
			set
			{
				this.internalBlockListAddressMax = value;
				this.internalBlockListAddressMaxSpecified = true;
			}
		}

		[XmlIgnore]
		public int BlockListDomainMax
		{
			get
			{
				return this.internalBlockListDomainMax;
			}
			set
			{
				this.internalBlockListDomainMax = value;
				this.internalBlockListDomainMaxSpecified = true;
			}
		}

		[XmlIgnore]
		public int WhiteListAddressMax
		{
			get
			{
				return this.internalWhiteListAddressMax;
			}
			set
			{
				this.internalWhiteListAddressMax = value;
				this.internalWhiteListAddressMaxSpecified = true;
			}
		}

		[XmlIgnore]
		public int WhiteListDomainMax
		{
			get
			{
				return this.internalWhiteListDomainMax;
			}
			set
			{
				this.internalWhiteListDomainMax = value;
				this.internalWhiteListDomainMaxSpecified = true;
			}
		}

		[XmlIgnore]
		public int WhiteToListMax
		{
			get
			{
				return this.internalWhiteToListMax;
			}
			set
			{
				this.internalWhiteToListMax = value;
				this.internalWhiteToListMaxSpecified = true;
			}
		}

		[XmlIgnore]
		public int AlternateFromListMax
		{
			get
			{
				return this.internalAlternateFromListMax;
			}
			set
			{
				this.internalAlternateFromListMax = value;
				this.internalAlternateFromListMaxSpecified = true;
			}
		}

		[XmlIgnore]
		public int MaxDailySendMessages
		{
			get
			{
				return this.internalMaxDailySendMessages;
			}
			set
			{
				this.internalMaxDailySendMessages = value;
				this.internalMaxDailySendMessagesSpecified = true;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "AccountStatus", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public AccountStatusType internalAccountStatus;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalAccountStatusSpecified;

		[XmlElement(ElementName = "ParentalControlStatus", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ParentalControlStatusType internalParentalControlStatus;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalParentalControlStatusSpecified;

		[XmlElement(ElementName = "MailBoxSize", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "long", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public long internalMailBoxSize;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalMailBoxSizeSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "MaxMailBoxSize", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "long", Namespace = "HMSETTINGS:")]
		public long internalMaxMailBoxSize;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalMaxMailBoxSizeSpecified;

		[XmlElement(ElementName = "MaxAttachments", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalMaxAttachments;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalMaxAttachmentsSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "MaxMessageSize", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "long", Namespace = "HMSETTINGS:")]
		public long internalMaxMessageSize;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalMaxMessageSizeSpecified;

		[XmlElement(ElementName = "MaxUnencodedMessageSize", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "long", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public long internalMaxUnencodedMessageSize;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalMaxUnencodedMessageSizeSpecified;

		[XmlElement(ElementName = "MaxFilters", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalMaxFilters;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalMaxFiltersSpecified;

		[XmlElement(ElementName = "MaxFilterClauseValueLength", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalMaxFilterClauseValueLength;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalMaxFilterClauseValueLengthSpecified;

		[XmlElement(ElementName = "MaxRecipients", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalMaxRecipients;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalMaxRecipientsSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "MaxUserSignatureLength", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		public int internalMaxUserSignatureLength;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalMaxUserSignatureLengthSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "BlockListAddressMax", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		public int internalBlockListAddressMax;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalBlockListAddressMaxSpecified;

		[XmlElement(ElementName = "BlockListDomainMax", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalBlockListDomainMax;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalBlockListDomainMaxSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "WhiteListAddressMax", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		public int internalWhiteListAddressMax;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalWhiteListAddressMaxSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "WhiteListDomainMax", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		public int internalWhiteListDomainMax;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalWhiteListDomainMaxSpecified;

		[XmlElement(ElementName = "WhiteToListMax", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalWhiteToListMax;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalWhiteToListMaxSpecified;

		[XmlElement(ElementName = "AlternateFromListMax", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalAlternateFromListMax;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalAlternateFromListMaxSpecified;

		[XmlElement(ElementName = "MaxDailySendMessages", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalMaxDailySendMessages;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalMaxDailySendMessagesSpecified;
	}
}
