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
	public class RulePredicatesType
	{
		[XmlArrayItem("String", IsNullable = false)]
		public string[] Categories
		{
			get
			{
				return this.categoriesField;
			}
			set
			{
				this.categoriesField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] ContainsBodyStrings
		{
			get
			{
				return this.containsBodyStringsField;
			}
			set
			{
				this.containsBodyStringsField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] ContainsHeaderStrings
		{
			get
			{
				return this.containsHeaderStringsField;
			}
			set
			{
				this.containsHeaderStringsField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] ContainsRecipientStrings
		{
			get
			{
				return this.containsRecipientStringsField;
			}
			set
			{
				this.containsRecipientStringsField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] ContainsSenderStrings
		{
			get
			{
				return this.containsSenderStringsField;
			}
			set
			{
				this.containsSenderStringsField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] ContainsSubjectOrBodyStrings
		{
			get
			{
				return this.containsSubjectOrBodyStringsField;
			}
			set
			{
				this.containsSubjectOrBodyStringsField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] ContainsSubjectStrings
		{
			get
			{
				return this.containsSubjectStringsField;
			}
			set
			{
				this.containsSubjectStringsField = value;
			}
		}

		public FlaggedForActionType FlaggedForAction
		{
			get
			{
				return this.flaggedForActionField;
			}
			set
			{
				this.flaggedForActionField = value;
			}
		}

		[XmlIgnore]
		public bool FlaggedForActionSpecified
		{
			get
			{
				return this.flaggedForActionFieldSpecified;
			}
			set
			{
				this.flaggedForActionFieldSpecified = value;
			}
		}

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] FromAddresses
		{
			get
			{
				return this.fromAddressesField;
			}
			set
			{
				this.fromAddressesField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] FromConnectedAccounts
		{
			get
			{
				return this.fromConnectedAccountsField;
			}
			set
			{
				this.fromConnectedAccountsField = value;
			}
		}

		public bool HasAttachments
		{
			get
			{
				return this.hasAttachmentsField;
			}
			set
			{
				this.hasAttachmentsField = value;
			}
		}

		[XmlIgnore]
		public bool HasAttachmentsSpecified
		{
			get
			{
				return this.hasAttachmentsFieldSpecified;
			}
			set
			{
				this.hasAttachmentsFieldSpecified = value;
			}
		}

		public ImportanceChoicesType Importance
		{
			get
			{
				return this.importanceField;
			}
			set
			{
				this.importanceField = value;
			}
		}

		[XmlIgnore]
		public bool ImportanceSpecified
		{
			get
			{
				return this.importanceFieldSpecified;
			}
			set
			{
				this.importanceFieldSpecified = value;
			}
		}

		public bool IsApprovalRequest
		{
			get
			{
				return this.isApprovalRequestField;
			}
			set
			{
				this.isApprovalRequestField = value;
			}
		}

		[XmlIgnore]
		public bool IsApprovalRequestSpecified
		{
			get
			{
				return this.isApprovalRequestFieldSpecified;
			}
			set
			{
				this.isApprovalRequestFieldSpecified = value;
			}
		}

		public bool IsAutomaticForward
		{
			get
			{
				return this.isAutomaticForwardField;
			}
			set
			{
				this.isAutomaticForwardField = value;
			}
		}

		[XmlIgnore]
		public bool IsAutomaticForwardSpecified
		{
			get
			{
				return this.isAutomaticForwardFieldSpecified;
			}
			set
			{
				this.isAutomaticForwardFieldSpecified = value;
			}
		}

		public bool IsAutomaticReply
		{
			get
			{
				return this.isAutomaticReplyField;
			}
			set
			{
				this.isAutomaticReplyField = value;
			}
		}

		[XmlIgnore]
		public bool IsAutomaticReplySpecified
		{
			get
			{
				return this.isAutomaticReplyFieldSpecified;
			}
			set
			{
				this.isAutomaticReplyFieldSpecified = value;
			}
		}

		public bool IsEncrypted
		{
			get
			{
				return this.isEncryptedField;
			}
			set
			{
				this.isEncryptedField = value;
			}
		}

		[XmlIgnore]
		public bool IsEncryptedSpecified
		{
			get
			{
				return this.isEncryptedFieldSpecified;
			}
			set
			{
				this.isEncryptedFieldSpecified = value;
			}
		}

		public bool IsMeetingRequest
		{
			get
			{
				return this.isMeetingRequestField;
			}
			set
			{
				this.isMeetingRequestField = value;
			}
		}

		[XmlIgnore]
		public bool IsMeetingRequestSpecified
		{
			get
			{
				return this.isMeetingRequestFieldSpecified;
			}
			set
			{
				this.isMeetingRequestFieldSpecified = value;
			}
		}

		public bool IsMeetingResponse
		{
			get
			{
				return this.isMeetingResponseField;
			}
			set
			{
				this.isMeetingResponseField = value;
			}
		}

		[XmlIgnore]
		public bool IsMeetingResponseSpecified
		{
			get
			{
				return this.isMeetingResponseFieldSpecified;
			}
			set
			{
				this.isMeetingResponseFieldSpecified = value;
			}
		}

		public bool IsNDR
		{
			get
			{
				return this.isNDRField;
			}
			set
			{
				this.isNDRField = value;
			}
		}

		[XmlIgnore]
		public bool IsNDRSpecified
		{
			get
			{
				return this.isNDRFieldSpecified;
			}
			set
			{
				this.isNDRFieldSpecified = value;
			}
		}

		public bool IsPermissionControlled
		{
			get
			{
				return this.isPermissionControlledField;
			}
			set
			{
				this.isPermissionControlledField = value;
			}
		}

		[XmlIgnore]
		public bool IsPermissionControlledSpecified
		{
			get
			{
				return this.isPermissionControlledFieldSpecified;
			}
			set
			{
				this.isPermissionControlledFieldSpecified = value;
			}
		}

		public bool IsReadReceipt
		{
			get
			{
				return this.isReadReceiptField;
			}
			set
			{
				this.isReadReceiptField = value;
			}
		}

		[XmlIgnore]
		public bool IsReadReceiptSpecified
		{
			get
			{
				return this.isReadReceiptFieldSpecified;
			}
			set
			{
				this.isReadReceiptFieldSpecified = value;
			}
		}

		public bool IsSigned
		{
			get
			{
				return this.isSignedField;
			}
			set
			{
				this.isSignedField = value;
			}
		}

		[XmlIgnore]
		public bool IsSignedSpecified
		{
			get
			{
				return this.isSignedFieldSpecified;
			}
			set
			{
				this.isSignedFieldSpecified = value;
			}
		}

		public bool IsVoicemail
		{
			get
			{
				return this.isVoicemailField;
			}
			set
			{
				this.isVoicemailField = value;
			}
		}

		[XmlIgnore]
		public bool IsVoicemailSpecified
		{
			get
			{
				return this.isVoicemailFieldSpecified;
			}
			set
			{
				this.isVoicemailFieldSpecified = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] ItemClasses
		{
			get
			{
				return this.itemClassesField;
			}
			set
			{
				this.itemClassesField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] MessageClassifications
		{
			get
			{
				return this.messageClassificationsField;
			}
			set
			{
				this.messageClassificationsField = value;
			}
		}

		public bool NotSentToMe
		{
			get
			{
				return this.notSentToMeField;
			}
			set
			{
				this.notSentToMeField = value;
			}
		}

		[XmlIgnore]
		public bool NotSentToMeSpecified
		{
			get
			{
				return this.notSentToMeFieldSpecified;
			}
			set
			{
				this.notSentToMeFieldSpecified = value;
			}
		}

		public bool SentCcMe
		{
			get
			{
				return this.sentCcMeField;
			}
			set
			{
				this.sentCcMeField = value;
			}
		}

		[XmlIgnore]
		public bool SentCcMeSpecified
		{
			get
			{
				return this.sentCcMeFieldSpecified;
			}
			set
			{
				this.sentCcMeFieldSpecified = value;
			}
		}

		public bool SentOnlyToMe
		{
			get
			{
				return this.sentOnlyToMeField;
			}
			set
			{
				this.sentOnlyToMeField = value;
			}
		}

		[XmlIgnore]
		public bool SentOnlyToMeSpecified
		{
			get
			{
				return this.sentOnlyToMeFieldSpecified;
			}
			set
			{
				this.sentOnlyToMeFieldSpecified = value;
			}
		}

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] SentToAddresses
		{
			get
			{
				return this.sentToAddressesField;
			}
			set
			{
				this.sentToAddressesField = value;
			}
		}

		public bool SentToMe
		{
			get
			{
				return this.sentToMeField;
			}
			set
			{
				this.sentToMeField = value;
			}
		}

		[XmlIgnore]
		public bool SentToMeSpecified
		{
			get
			{
				return this.sentToMeFieldSpecified;
			}
			set
			{
				this.sentToMeFieldSpecified = value;
			}
		}

		public bool SentToOrCcMe
		{
			get
			{
				return this.sentToOrCcMeField;
			}
			set
			{
				this.sentToOrCcMeField = value;
			}
		}

		[XmlIgnore]
		public bool SentToOrCcMeSpecified
		{
			get
			{
				return this.sentToOrCcMeFieldSpecified;
			}
			set
			{
				this.sentToOrCcMeFieldSpecified = value;
			}
		}

		public SensitivityChoicesType Sensitivity
		{
			get
			{
				return this.sensitivityField;
			}
			set
			{
				this.sensitivityField = value;
			}
		}

		[XmlIgnore]
		public bool SensitivitySpecified
		{
			get
			{
				return this.sensitivityFieldSpecified;
			}
			set
			{
				this.sensitivityFieldSpecified = value;
			}
		}

		public RulePredicateDateRangeType WithinDateRange
		{
			get
			{
				return this.withinDateRangeField;
			}
			set
			{
				this.withinDateRangeField = value;
			}
		}

		public RulePredicateSizeRangeType WithinSizeRange
		{
			get
			{
				return this.withinSizeRangeField;
			}
			set
			{
				this.withinSizeRangeField = value;
			}
		}

		private string[] categoriesField;

		private string[] containsBodyStringsField;

		private string[] containsHeaderStringsField;

		private string[] containsRecipientStringsField;

		private string[] containsSenderStringsField;

		private string[] containsSubjectOrBodyStringsField;

		private string[] containsSubjectStringsField;

		private FlaggedForActionType flaggedForActionField;

		private bool flaggedForActionFieldSpecified;

		private EmailAddressType[] fromAddressesField;

		private string[] fromConnectedAccountsField;

		private bool hasAttachmentsField;

		private bool hasAttachmentsFieldSpecified;

		private ImportanceChoicesType importanceField;

		private bool importanceFieldSpecified;

		private bool isApprovalRequestField;

		private bool isApprovalRequestFieldSpecified;

		private bool isAutomaticForwardField;

		private bool isAutomaticForwardFieldSpecified;

		private bool isAutomaticReplyField;

		private bool isAutomaticReplyFieldSpecified;

		private bool isEncryptedField;

		private bool isEncryptedFieldSpecified;

		private bool isMeetingRequestField;

		private bool isMeetingRequestFieldSpecified;

		private bool isMeetingResponseField;

		private bool isMeetingResponseFieldSpecified;

		private bool isNDRField;

		private bool isNDRFieldSpecified;

		private bool isPermissionControlledField;

		private bool isPermissionControlledFieldSpecified;

		private bool isReadReceiptField;

		private bool isReadReceiptFieldSpecified;

		private bool isSignedField;

		private bool isSignedFieldSpecified;

		private bool isVoicemailField;

		private bool isVoicemailFieldSpecified;

		private string[] itemClassesField;

		private string[] messageClassificationsField;

		private bool notSentToMeField;

		private bool notSentToMeFieldSpecified;

		private bool sentCcMeField;

		private bool sentCcMeFieldSpecified;

		private bool sentOnlyToMeField;

		private bool sentOnlyToMeFieldSpecified;

		private EmailAddressType[] sentToAddressesField;

		private bool sentToMeField;

		private bool sentToMeFieldSpecified;

		private bool sentToOrCcMeField;

		private bool sentToOrCcMeFieldSpecified;

		private SensitivityChoicesType sensitivityField;

		private bool sensitivityFieldSpecified;

		private RulePredicateDateRangeType withinDateRangeField;

		private RulePredicateSizeRangeType withinSizeRangeField;
	}
}
