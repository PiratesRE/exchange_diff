using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class InboxRuleParameters : SetObjectProperties
	{
		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		public override string SuppressConfirmParameterName
		{
			get
			{
				return "AlwaysDeleteOutlookRulesBlob";
			}
		}

		[DataMember]
		public string Name
		{
			get
			{
				return (string)base["Name"];
			}
			set
			{
				base["Name"] = value;
			}
		}

		[DataMember]
		public PeopleIdentity[] From
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["From"]);
			}
			set
			{
				base["From"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] SentTo
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["SentTo"]);
			}
			set
			{
				base["SentTo"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public Identity FromSubscription
		{
			get
			{
				string value = ((string[])base["FromSubscription"]).ToCommaSeperatedString();
				return Identity.FromIdParameter(value);
			}
			set
			{
				base["FromSubscription"] = value.ToTaskIdStringArray();
			}
		}

		[DataMember]
		public string[] SubjectContainsWords
		{
			get
			{
				return (string[])base["SubjectContainsWords"];
			}
			set
			{
				base["SubjectContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] SubjectOrBodyContainsWords
		{
			get
			{
				return (string[])base["SubjectOrBodyContainsWords"];
			}
			set
			{
				base["SubjectOrBodyContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] FromAddressContainsWords
		{
			get
			{
				return (string[])base["FromAddressContainsWords"];
			}
			set
			{
				base["FromAddressContainsWords"] = value;
			}
		}

		[DataMember]
		public bool? MyNameInToOrCcBox
		{
			get
			{
				return (bool?)base["MyNameInToOrCcBox"];
			}
			set
			{
				base["MyNameInToOrCcBox"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? SentOnlyToMe
		{
			get
			{
				return (bool?)base["SentOnlyToMe"];
			}
			set
			{
				base["SentOnlyToMe"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? MyNameInToBox
		{
			get
			{
				return (bool?)base["MyNameInToBox"];
			}
			set
			{
				base["MyNameInToBox"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? MyNameInCcBox
		{
			get
			{
				return (bool?)base["MyNameInCcBox"];
			}
			set
			{
				base["MyNameInCcBox"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? MyNameNotInToBox
		{
			get
			{
				return (bool?)base["MyNameNotInToBox"];
			}
			set
			{
				base["MyNameNotInToBox"] = (value ?? false);
			}
		}

		[DataMember]
		public string[] BodyContainsWords
		{
			get
			{
				return (string[])base["BodyContainsWords"];
			}
			set
			{
				base["BodyContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] RecipientAddressContainsWords
		{
			get
			{
				return (string[])base["RecipientAddressContainsWords"];
			}
			set
			{
				base["RecipientAddressContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] HeaderContainsWords
		{
			get
			{
				return (string[])base["HeaderContainsWords"];
			}
			set
			{
				base["HeaderContainsWords"] = value;
			}
		}

		[DataMember]
		public string WithImportance
		{
			get
			{
				return (string)base["WithImportance"];
			}
			set
			{
				base["WithImportance"] = value;
			}
		}

		[DataMember]
		public string WithSensitivity
		{
			get
			{
				return (string)base["WithSensitivity"];
			}
			set
			{
				base["WithSensitivity"] = value;
			}
		}

		[DataMember]
		public bool? HasAttachment
		{
			get
			{
				return (bool?)base["HasAttachment"];
			}
			set
			{
				base["HasAttachment"] = (value ?? false);
			}
		}

		[DataMember]
		public string MessageTypeMatches
		{
			get
			{
				return (string)base["MessageTypeMatches"];
			}
			set
			{
				base["MessageTypeMatches"] = value;
			}
		}

		[DataMember]
		public Identity HasClassification
		{
			get
			{
				string value = ((string[])base["HasClassification"]).ToCommaSeperatedString();
				return Identity.FromIdParameter(value);
			}
			set
			{
				base["HasClassification"] = value.ToTaskIdStringArray();
			}
		}

		[DataMember]
		public string FlaggedForAction
		{
			get
			{
				return (string)base["FlaggedForAction"];
			}
			set
			{
				base["FlaggedForAction"] = value;
			}
		}

		[DataMember]
		public NumberRange WithinSizeRange
		{
			get
			{
				return new NumberRange
				{
					AtLeast = ((ByteQuantifiedSize?)base["WithinSizeRangeMinimum"]).ToKB(),
					AtMost = ((ByteQuantifiedSize?)base["WithinSizeRangeMaximum"]).ToKB()
				};
			}
			set
			{
				base["WithinSizeRangeMinimum"] = ((value == null) ? null : value.AtLeast.ToByteSize());
				base["WithinSizeRangeMaximum"] = ((value == null) ? null : value.AtMost.ToByteSize());
			}
		}

		[DataMember]
		public DateRange WithinDateRange
		{
			get
			{
				return new DateRange
				{
					BeforeDate = ((ExDateTime?)base["ReceivedBeforeDate"]).ToIdentity(),
					AfterDate = ((ExDateTime?)base["ReceivedAfterDate"]).ToIdentity()
				};
			}
			set
			{
				base["ReceivedBeforeDate"] = ((value == null || value.BeforeDate == null) ? null : value.BeforeDate.RawIdentity.ToEcpExDateTime("yyyy/MM/dd HH:mm:ss"));
				base["ReceivedAfterDate"] = ((value == null || value.AfterDate == null) ? null : value.AfterDate.RawIdentity.ToEcpExDateTime("yyyy/MM/dd HH:mm:ss"));
			}
		}

		[DataMember]
		public Identity MoveToFolder
		{
			get
			{
				return Identity.FromIdParameter(base["MoveToFolder"]);
			}
			set
			{
				base["MoveToFolder"] = value.ToMailboxFolderIdParameter();
			}
		}

		[DataMember]
		public Identity CopyToFolder
		{
			get
			{
				return Identity.FromIdParameter(base["CopyToFolder"]);
			}
			set
			{
				base["CopyToFolder"] = value.ToMailboxFolderIdParameter();
			}
		}

		[DataMember]
		public bool? DeleteMessage
		{
			get
			{
				return (bool?)base["DeleteMessage"];
			}
			set
			{
				base["DeleteMessage"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? StopProcessingRules
		{
			get
			{
				return (bool?)base["StopProcessingRules"];
			}
			set
			{
				base["StopProcessingRules"] = (value ?? false);
			}
		}

		[DataMember]
		public Identity ApplyCategory
		{
			get
			{
				string value = ((string[])base["ApplyCategory"]).ToCommaSeperatedString();
				return Identity.FromIdParameter(value);
			}
			set
			{
				base["ApplyCategory"] = value.ToTaskIdStringArray();
			}
		}

		[DataMember]
		public bool? MarkAsRead
		{
			get
			{
				return (bool?)base["MarkAsRead"];
			}
			set
			{
				base["MarkAsRead"] = (value ?? false);
			}
		}

		[DataMember]
		public PeopleIdentity[] ForwardTo
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ForwardTo"]);
			}
			set
			{
				base["ForwardTo"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public string SendTextMessageNotificationTo
		{
			get
			{
				return (string)base["SendTextMessageNotificationTo"];
			}
			set
			{
				base["SendTextMessageNotificationTo"] = value;
			}
		}

		[DataMember]
		public PeopleIdentity[] RedirectTo
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["RedirectTo"]);
			}
			set
			{
				base["RedirectTo"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] ForwardAsAttachmentTo
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ForwardAsAttachmentTo"]);
			}
			set
			{
				base["ForwardAsAttachmentTo"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public string MarkImportance
		{
			get
			{
				return (string)base["MarkImportance"];
			}
			set
			{
				base["MarkImportance"] = value;
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfFrom
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ExceptIfFrom"]);
			}
			set
			{
				base["ExceptIfFrom"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfSentTo
		{
			get
			{
				return PeopleIdentity.FromIdParameters(base["ExceptIfSentTo"]);
			}
			set
			{
				base["ExceptIfSentTo"] = value.ToIdParameters();
			}
		}

		[DataMember]
		public Identity ExceptIfFromSubscription
		{
			get
			{
				string value = ((string[])base["ExceptIfFromSubscription"]).ToCommaSeperatedString();
				return Identity.FromIdParameter(value);
			}
			set
			{
				base["ExceptIfFromSubscription"] = value.ToTaskIdStringArray();
			}
		}

		[DataMember]
		public string[] ExceptIfSubjectContainsWords
		{
			get
			{
				return (string[])base["ExceptIfSubjectContainsWords"];
			}
			set
			{
				base["ExceptIfSubjectContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfSubjectOrBodyContainsWords
		{
			get
			{
				return (string[])base["ExceptIfSubjectOrBodyContainsWords"];
			}
			set
			{
				base["ExceptIfSubjectOrBodyContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfFromAddressContainsWords
		{
			get
			{
				return (string[])base["ExceptIfFromAddressContainsWords"];
			}
			set
			{
				base["ExceptIfFromAddressContainsWords"] = value;
			}
		}

		[DataMember]
		public bool? ExceptIfMyNameInToOrCcBox
		{
			get
			{
				return (bool?)base["ExceptIfMyNameInToOrCcBox"];
			}
			set
			{
				base["ExceptIfMyNameInToOrCcBox"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? ExceptIfSentOnlyToMe
		{
			get
			{
				return (bool?)base["ExceptIfSentOnlyToMe"];
			}
			set
			{
				base["ExceptIfSentOnlyToMe"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? ExceptIfMyNameInToBox
		{
			get
			{
				return (bool?)base["ExceptIfMyNameInToBox"];
			}
			set
			{
				base["ExceptIfMyNameInToBox"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? ExceptIfMyNameInCcBox
		{
			get
			{
				return (bool?)base["ExceptIfMyNameInCcBox"];
			}
			set
			{
				base["ExceptIfMyNameInCcBox"] = (value ?? false);
			}
		}

		[DataMember]
		public bool? ExceptIfMyNameNotInToBox
		{
			get
			{
				return (bool?)base["ExceptIfMyNameNotInToBox"];
			}
			set
			{
				base["ExceptIfMyNameNotInToBox"] = (value ?? false);
			}
		}

		[DataMember]
		public string[] ExceptIfBodyContainsWords
		{
			get
			{
				return (string[])base["ExceptIfBodyContainsWords"];
			}
			set
			{
				base["ExceptIfBodyContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfRecipientAddressContainsWords
		{
			get
			{
				return (string[])base["ExceptIfRecipientAddressContainsWords"];
			}
			set
			{
				base["ExceptIfRecipientAddressContainsWords"] = value;
			}
		}

		[DataMember]
		public string[] ExceptIfHeaderContainsWords
		{
			get
			{
				return (string[])base["ExceptIfHeaderContainsWords"];
			}
			set
			{
				base["ExceptIfHeaderContainsWords"] = value;
			}
		}

		[DataMember]
		public string ExceptIfWithImportance
		{
			get
			{
				return (string)base["ExceptIfWithImportance"];
			}
			set
			{
				base["ExceptIfWithImportance"] = value;
			}
		}

		[DataMember]
		public string ExceptIfWithSensitivity
		{
			get
			{
				return (string)base["ExceptIfWithSensitivity"];
			}
			set
			{
				base["ExceptIfWithSensitivity"] = value;
			}
		}

		[DataMember]
		public bool? ExceptIfHasAttachment
		{
			get
			{
				return (bool?)base["ExceptIfHasAttachment"];
			}
			set
			{
				base["ExceptIfHasAttachment"] = (value ?? false);
			}
		}

		[DataMember]
		public string ExceptIfMessageTypeMatches
		{
			get
			{
				return (string)base["ExceptIfMessageTypeMatches"];
			}
			set
			{
				base["ExceptIfMessageTypeMatches"] = value;
			}
		}

		[DataMember]
		public Identity ExceptIfHasClassification
		{
			get
			{
				string value = ((string[])base["ExceptIfHasClassification"]).ToCommaSeperatedString();
				return Identity.FromIdParameter(value);
			}
			set
			{
				base["ExceptIfHasClassification"] = value.ToTaskIdStringArray();
			}
		}

		[DataMember]
		public string ExceptIfFlaggedForAction
		{
			get
			{
				return (string)base["ExceptIfFlaggedForAction"];
			}
			set
			{
				base["ExceptIfFlaggedForAction"] = value;
			}
		}

		[DataMember]
		public NumberRange ExceptIfWithinSizeRange
		{
			get
			{
				return new NumberRange
				{
					AtLeast = ((ByteQuantifiedSize?)base["ExceptIfWithinSizeRangeMinimum"]).ToKB(),
					AtMost = ((ByteQuantifiedSize?)base["ExceptIfWithinSizeRangeMaximum"]).ToKB()
				};
			}
			set
			{
				base["ExceptIfWithinSizeRangeMinimum"] = ((value == null) ? null : value.AtLeast.ToByteSize());
				base["ExceptIfWithinSizeRangeMaximum"] = ((value == null) ? null : value.AtMost.ToByteSize());
			}
		}

		[DataMember]
		public DateRange ExceptIfWithinDateRange
		{
			get
			{
				return new DateRange
				{
					BeforeDate = ((ExDateTime?)base["ExceptIfReceivedBeforeDate"]).ToIdentity(),
					AfterDate = ((ExDateTime?)base["ExceptIfReceivedAfterDate"]).ToIdentity()
				};
			}
			set
			{
				base["ExceptIfReceivedBeforeDate"] = ((value == null || value.BeforeDate == null) ? null : value.BeforeDate.RawIdentity.ToEcpExDateTime("yyyy/MM/dd HH:mm:ss"));
				base["ExceptIfReceivedAfterDate"] = ((value == null || value.AfterDate == null) ? null : value.AfterDate.RawIdentity.ToEcpExDateTime("yyyy/MM/dd HH:mm:ss"));
			}
		}
	}
}
