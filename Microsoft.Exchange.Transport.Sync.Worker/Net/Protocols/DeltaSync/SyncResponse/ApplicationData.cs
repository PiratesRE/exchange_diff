using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Net.Protocols.DeltaSync.Email;
using Microsoft.Exchange.Net.Protocols.DeltaSync.HMFolder;
using Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SyncResponse
{
	[XmlType(TypeName = "ApplicationData", Namespace = "AirSync:")]
	[Serializable]
	public class ApplicationData
	{
		[XmlIgnore]
		public byte Read
		{
			get
			{
				return this.internalRead;
			}
			set
			{
				this.internalRead = value;
				this.internalReadSpecified = true;
			}
		}

		[XmlIgnore]
		public byte ReplyToOrForwardState
		{
			get
			{
				return this.internalReplyToOrForwardState;
			}
			set
			{
				this.internalReplyToOrForwardState = value;
				this.internalReplyToOrForwardStateSpecified = true;
			}
		}

		[XmlIgnore]
		public byte Importance
		{
			get
			{
				return this.internalImportance;
			}
			set
			{
				this.internalImportance = value;
				this.internalImportanceSpecified = true;
			}
		}

		[XmlIgnore]
		public Categories Categories
		{
			get
			{
				if (this.internalCategories == null)
				{
					this.internalCategories = new Categories();
				}
				return this.internalCategories;
			}
			set
			{
				this.internalCategories = value;
			}
		}

		[XmlIgnore]
		public Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType ConfirmedJunk
		{
			get
			{
				return this.internalConfirmedJunk;
			}
			set
			{
				this.internalConfirmedJunk = value;
				this.internalConfirmedJunkSpecified = true;
			}
		}

		[XmlIgnore]
		public Flag Flag
		{
			get
			{
				if (this.internalFlag == null)
				{
					this.internalFlag = new Flag();
				}
				return this.internalFlag;
			}
			set
			{
				this.internalFlag = value;
			}
		}

		[XmlIgnore]
		public FolderId FolderId
		{
			get
			{
				if (this.internalFolderId == null)
				{
					this.internalFolderId = new FolderId();
				}
				return this.internalFolderId;
			}
			set
			{
				this.internalFolderId = value;
			}
		}

		[XmlIgnore]
		public DisplayName DisplayName
		{
			get
			{
				if (this.internalDisplayName == null)
				{
					this.internalDisplayName = new DisplayName();
				}
				return this.internalDisplayName;
			}
			set
			{
				this.internalDisplayName = value;
			}
		}

		[XmlIgnore]
		public int Version
		{
			get
			{
				return this.internalVersion;
			}
			set
			{
				this.internalVersion = value;
				this.internalVersionSpecified = true;
			}
		}

		[XmlIgnore]
		public ParentId ParentId
		{
			get
			{
				if (this.internalParentId == null)
				{
					this.internalParentId = new ParentId();
				}
				return this.internalParentId;
			}
			set
			{
				this.internalParentId = value;
			}
		}

		[XmlIgnore]
		public ConversationTopic ConversationTopic
		{
			get
			{
				if (this.internalConversationTopic == null)
				{
					this.internalConversationTopic = new ConversationTopic();
				}
				return this.internalConversationTopic;
			}
			set
			{
				this.internalConversationTopic = value;
			}
		}

		[XmlIgnore]
		public ConversationIndex ConversationIndex
		{
			get
			{
				if (this.internalConversationIndex == null)
				{
					this.internalConversationIndex = new ConversationIndex();
				}
				return this.internalConversationIndex;
			}
			set
			{
				this.internalConversationIndex = value;
			}
		}

		[XmlIgnore]
		public From From
		{
			get
			{
				if (this.internalFrom == null)
				{
					this.internalFrom = new From();
				}
				return this.internalFrom;
			}
			set
			{
				this.internalFrom = value;
			}
		}

		[XmlIgnore]
		public Subject Subject
		{
			get
			{
				if (this.internalSubject == null)
				{
					this.internalSubject = new Subject();
				}
				return this.internalSubject;
			}
			set
			{
				this.internalSubject = value;
			}
		}

		[XmlIgnore]
		public string DateReceived
		{
			get
			{
				return this.internalDateReceived;
			}
			set
			{
				this.internalDateReceived = value;
			}
		}

		[XmlIgnore]
		public Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType HasAttachments
		{
			get
			{
				return this.internalHasAttachments;
			}
			set
			{
				this.internalHasAttachments = value;
				this.internalHasAttachmentsSpecified = true;
			}
		}

		[XmlIgnore]
		public Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType IsFromSomeoneAddressBook
		{
			get
			{
				return this.internalIsFromSomeoneAddressBook;
			}
			set
			{
				this.internalIsFromSomeoneAddressBook = value;
				this.internalIsFromSomeoneAddressBookSpecified = true;
			}
		}

		[XmlIgnore]
		public Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType IsToAllowList
		{
			get
			{
				return this.internalIsToAllowList;
			}
			set
			{
				this.internalIsToAllowList = value;
				this.internalIsToAllowListSpecified = true;
			}
		}

		[XmlIgnore]
		public string LegacyId
		{
			get
			{
				return this.internalLegacyId;
			}
			set
			{
				this.internalLegacyId = value;
			}
		}

		[XmlIgnore]
		public string MessageClass
		{
			get
			{
				return this.internalMessageClass;
			}
			set
			{
				this.internalMessageClass = value;
			}
		}

		[XmlIgnore]
		public string Message
		{
			get
			{
				return this.internalMessage;
			}
			set
			{
				this.internalMessage = value;
			}
		}

		[XmlIgnore]
		public byte Sensitivity
		{
			get
			{
				return this.internalSensitivity;
			}
			set
			{
				this.internalSensitivity = value;
				this.internalSensitivitySpecified = true;
			}
		}

		[XmlIgnore]
		public int Size
		{
			get
			{
				return this.internalSize;
			}
			set
			{
				this.internalSize = value;
				this.internalSizeSpecified = true;
			}
		}

		[XmlIgnore]
		public Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType TrustedSource
		{
			get
			{
				return this.internalTrustedSource;
			}
			set
			{
				this.internalTrustedSource = value;
				this.internalTrustedSourceSpecified = true;
			}
		}

		[XmlIgnore]
		public int Version2
		{
			get
			{
				return this.internalVersion2;
			}
			set
			{
				this.internalVersion2 = value;
				this.internalVersion2Specified = true;
			}
		}

		[XmlIgnore]
		public Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType IsBondedSender
		{
			get
			{
				return this.internalIsBondedSender;
			}
			set
			{
				this.internalIsBondedSender = value;
				this.internalIsBondedSenderSpecified = true;
			}
		}

		[XmlIgnore]
		public byte TypeData
		{
			get
			{
				return this.internalTypeData;
			}
			set
			{
				this.internalTypeData = value;
				this.internalTypeDataSpecified = true;
			}
		}

		[XmlElement(ElementName = "Read", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "unsignedByte", Namespace = "EMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public byte internalRead;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalReadSpecified;

		[XmlElement(ElementName = "ReplyToOrForwardState", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "unsignedByte", Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public byte internalReplyToOrForwardState;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalReplyToOrForwardStateSpecified;

		[XmlElement(ElementName = "Importance", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "unsignedByte", Namespace = "EMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public byte internalImportance;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalImportanceSpecified;

		[XmlElement(Type = typeof(Categories), ElementName = "Categories", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Categories internalCategories;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "ConfirmedJunk", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMMAIL:")]
		public Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType internalConfirmedJunk;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalConfirmedJunkSpecified;

		[XmlElement(Type = typeof(Flag), ElementName = "Flag", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Flag internalFlag;

		[XmlElement(Type = typeof(FolderId), ElementName = "FolderId", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public FolderId internalFolderId;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(DisplayName), ElementName = "DisplayName", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMFOLDER:")]
		public DisplayName internalDisplayName;

		[XmlElement(ElementName = "Version", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMFOLDER:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public int internalVersion;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalVersionSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(ParentId), ElementName = "ParentId", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMFOLDER:")]
		public ParentId internalParentId;

		[XmlElement(Type = typeof(ConversationTopic), ElementName = "ConversationTopic", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ConversationTopic internalConversationTopic;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(ConversationIndex), ElementName = "ConversationIndex", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMMAIL:")]
		public ConversationIndex internalConversationIndex;

		[XmlElement(Type = typeof(From), ElementName = "From", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "EMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public From internalFrom;

		[XmlElement(Type = typeof(Subject), ElementName = "Subject", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "EMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Subject internalSubject;

		[XmlElement(ElementName = "DateReceived", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "EMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalDateReceived;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "HasAttachments", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMMAIL:")]
		public Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType internalHasAttachments;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalHasAttachmentsSpecified;

		[XmlElement(ElementName = "IsFromSomeoneAddressBook", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType internalIsFromSomeoneAddressBook;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalIsFromSomeoneAddressBookSpecified;

		[XmlElement(ElementName = "IsToAllowList", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType internalIsToAllowList;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalIsToAllowListSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "LegacyId", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMMAIL:")]
		public string internalLegacyId;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "MessageClass", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "EMAIL:")]
		public string internalMessageClass;

		[XmlElement(ElementName = "Message", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string internalMessage;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "Sensitivity", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "unsignedByte", Namespace = "HMMAIL:")]
		public byte internalSensitivity;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalSensitivitySpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "Size", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMMAIL:")]
		public int internalSize;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalSizeSpecified;

		[XmlElement(ElementName = "TrustedSource", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType internalTrustedSource;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalTrustedSourceSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "Version", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int", Namespace = "HMMAIL:")]
		public int internalVersion2;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalVersion2Specified;

		[XmlElement(ElementName = "IsBondedSender", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMMAIL:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType internalIsBondedSender;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalIsBondedSenderSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "TypeData", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "unsignedByte", Namespace = "HMMAIL:")]
		public byte internalTypeData;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalTypeDataSpecified;
	}
}
