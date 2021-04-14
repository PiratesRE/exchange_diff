using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class RulePredicatesType
	{
		[XmlArrayItem("String", IsNullable = false)]
		public string[] Categories;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] ContainsBodyStrings;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] ContainsHeaderStrings;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] ContainsRecipientStrings;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] ContainsSenderStrings;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] ContainsSubjectOrBodyStrings;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] ContainsSubjectStrings;

		public FlaggedForActionType FlaggedForAction;

		[XmlIgnore]
		public bool FlaggedForActionSpecified;

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] FromAddresses;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] FromConnectedAccounts;

		public bool HasAttachments;

		[XmlIgnore]
		public bool HasAttachmentsSpecified;

		public ImportanceChoicesType Importance;

		[XmlIgnore]
		public bool ImportanceSpecified;

		public bool IsApprovalRequest;

		[XmlIgnore]
		public bool IsApprovalRequestSpecified;

		public bool IsAutomaticForward;

		[XmlIgnore]
		public bool IsAutomaticForwardSpecified;

		public bool IsAutomaticReply;

		[XmlIgnore]
		public bool IsAutomaticReplySpecified;

		public bool IsEncrypted;

		[XmlIgnore]
		public bool IsEncryptedSpecified;

		public bool IsMeetingRequest;

		[XmlIgnore]
		public bool IsMeetingRequestSpecified;

		public bool IsMeetingResponse;

		[XmlIgnore]
		public bool IsMeetingResponseSpecified;

		public bool IsNDR;

		[XmlIgnore]
		public bool IsNDRSpecified;

		public bool IsPermissionControlled;

		[XmlIgnore]
		public bool IsPermissionControlledSpecified;

		public bool IsReadReceipt;

		[XmlIgnore]
		public bool IsReadReceiptSpecified;

		public bool IsSigned;

		[XmlIgnore]
		public bool IsSignedSpecified;

		public bool IsVoicemail;

		[XmlIgnore]
		public bool IsVoicemailSpecified;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] ItemClasses;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] MessageClassifications;

		public bool NotSentToMe;

		[XmlIgnore]
		public bool NotSentToMeSpecified;

		public bool SentCcMe;

		[XmlIgnore]
		public bool SentCcMeSpecified;

		public bool SentOnlyToMe;

		[XmlIgnore]
		public bool SentOnlyToMeSpecified;

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] SentToAddresses;

		public bool SentToMe;

		[XmlIgnore]
		public bool SentToMeSpecified;

		public bool SentToOrCcMe;

		[XmlIgnore]
		public bool SentToOrCcMeSpecified;

		public SensitivityChoicesType Sensitivity;

		[XmlIgnore]
		public bool SensitivitySpecified;

		public RulePredicateDateRangeType WithinDateRange;

		public RulePredicateSizeRangeType WithinSizeRange;
	}
}
