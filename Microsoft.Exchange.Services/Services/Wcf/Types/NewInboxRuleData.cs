using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class NewInboxRuleData : OptionsPropertyChangeTracker
	{
		[DataMember]
		public string[] ApplyCategory
		{
			get
			{
				return this.applyCategory;
			}
			set
			{
				this.applyCategory = value;
				base.TrackPropertyChanged("ApplyCategory");
			}
		}

		[DataMember]
		public string[] BodyContainsWords
		{
			get
			{
				return this.bodyContainsWords;
			}
			set
			{
				this.bodyContainsWords = value;
				base.TrackPropertyChanged("BodyContainsWords");
			}
		}

		[DataMember]
		public Identity CopyToFolder
		{
			get
			{
				return this.copyToFolder;
			}
			set
			{
				this.copyToFolder = value;
				base.TrackPropertyChanged("CopyToFolder");
			}
		}

		[DataMember]
		public bool DeleteMessage
		{
			get
			{
				return this.deleteMessage;
			}
			set
			{
				this.deleteMessage = value;
				base.TrackPropertyChanged("DeleteMessage");
			}
		}

		[DataMember]
		public string[] ExceptIfBodyContainsWords
		{
			get
			{
				return this.exceptIfBodyContainsWords;
			}
			set
			{
				this.exceptIfBodyContainsWords = value;
				base.TrackPropertyChanged("ExceptIfBodyContainsWords");
			}
		}

		[DataMember]
		public string ExceptIfFlaggedForAction
		{
			get
			{
				return this.exceptIfFlaggedForAction;
			}
			set
			{
				this.exceptIfFlaggedForAction = value;
				base.TrackPropertyChanged("ExceptIfFlaggedForAction");
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfFrom
		{
			get
			{
				return this.exceptIfFrom;
			}
			set
			{
				this.exceptIfFrom = value;
				base.TrackPropertyChanged("ExceptIfFrom");
			}
		}

		[DataMember]
		public string[] ExceptIfFromAddressContainsWords
		{
			get
			{
				return this.exceptIfFromAddressContainsWords;
			}
			set
			{
				this.exceptIfFromAddressContainsWords = value;
				base.TrackPropertyChanged("ExceptIfFromAddressContainsWords");
			}
		}

		[DataMember]
		public Identity[] ExceptIfFromSubscription
		{
			get
			{
				return this.exceptIfFromSubscription;
			}
			set
			{
				this.exceptIfFromSubscription = value;
				base.TrackPropertyChanged("ExceptIfFromSubscription");
			}
		}

		[DataMember]
		public bool ExceptIfHasAttachment
		{
			get
			{
				return this.exceptIfHasAttachment;
			}
			set
			{
				this.exceptIfHasAttachment = value;
				base.TrackPropertyChanged("ExceptIfHasAttachment");
			}
		}

		[DataMember]
		public Identity[] ExceptIfHasClassification
		{
			get
			{
				return this.exceptIfHasClassification;
			}
			set
			{
				this.exceptIfHasClassification = value;
				base.TrackPropertyChanged("ExceptIfHasClassification");
			}
		}

		[DataMember]
		public string[] ExceptIfHeaderContainsWords
		{
			get
			{
				return this.exceptIfHeaderContainsWords;
			}
			set
			{
				this.exceptIfHeaderContainsWords = value;
				base.TrackPropertyChanged("ExceptIfHeaderContainsWords");
			}
		}

		[IgnoreDataMember]
		public NullableInboxRuleMessageType ExceptIfMessageTypeMatches
		{
			get
			{
				return this.exceptIfMessageTypeMatches;
			}
			set
			{
				this.exceptIfMessageTypeMatches = value;
				base.TrackPropertyChanged("ExceptIfMessageTypeMatches");
			}
		}

		[DataMember(Name = "ExceptIfMessageTypeMatches", IsRequired = false, EmitDefaultValue = false)]
		public string ExceptIfMessageTypeMatchesString
		{
			get
			{
				return EnumUtilities.ToString<NullableInboxRuleMessageType>(this.ExceptIfMessageTypeMatches);
			}
			set
			{
				this.ExceptIfMessageTypeMatches = EnumUtilities.Parse<NullableInboxRuleMessageType>(value);
			}
		}

		[DataMember]
		public bool ExceptIfMyNameInCcBox
		{
			get
			{
				return this.exceptIfMyNameInCcBox;
			}
			set
			{
				this.exceptIfMyNameInCcBox = value;
				base.TrackPropertyChanged("ExceptIfMyNameInCcBox");
			}
		}

		[DataMember]
		public bool ExceptIfMyNameInToBox
		{
			get
			{
				return this.exceptIfMyNameInToBox;
			}
			set
			{
				this.exceptIfMyNameInToBox = value;
				base.TrackPropertyChanged("ExceptIfMyNameInToBox");
			}
		}

		[DataMember]
		public bool ExceptIfMyNameInToOrCcBox
		{
			get
			{
				return this.exceptIfMyNameInToOrCcBox;
			}
			set
			{
				this.exceptIfMyNameInToOrCcBox = value;
				base.TrackPropertyChanged("ExceptIfMyNameInToOrCcBox");
			}
		}

		[DataMember]
		public bool ExceptIfMyNameNotInToBox
		{
			get
			{
				return this.exceptIfMyNameNotInToBox;
			}
			set
			{
				this.exceptIfMyNameNotInToBox = value;
				base.TrackPropertyChanged("ExceptIfMyNameNotInToBox");
			}
		}

		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string ExceptIfReceivedAfterDate
		{
			get
			{
				return this.exceptIfReceivedAfterDate;
			}
			set
			{
				this.exceptIfReceivedAfterDate = value;
				base.TrackPropertyChanged("ExceptIfReceivedAfterDate");
			}
		}

		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string ExceptIfReceivedBeforeDate
		{
			get
			{
				return this.exceptIfReceivedBeforeDate;
			}
			set
			{
				this.exceptIfReceivedBeforeDate = value;
				base.TrackPropertyChanged("ExceptIfReceivedBeforeDate");
			}
		}

		[DataMember]
		public string[] ExceptIfRecipientAddressContainsWords
		{
			get
			{
				return this.exceptIfRecipientAddressContainsWords;
			}
			set
			{
				this.exceptIfRecipientAddressContainsWords = value;
				base.TrackPropertyChanged("ExceptIfRecipientAddressContainsWords");
			}
		}

		[DataMember]
		public bool ExceptIfSentOnlyToMe
		{
			get
			{
				return this.exceptIfSentOnlyToMe;
			}
			set
			{
				this.exceptIfSentOnlyToMe = value;
				base.TrackPropertyChanged("ExceptIfSentOnlyToMe");
			}
		}

		[DataMember]
		public PeopleIdentity[] ExceptIfSentTo
		{
			get
			{
				return this.exceptIfSentTo;
			}
			set
			{
				this.exceptIfSentTo = value;
				base.TrackPropertyChanged("ExceptIfSentTo");
			}
		}

		[DataMember]
		public string[] ExceptIfSubjectContainsWords
		{
			get
			{
				return this.exceptIfSubjectContainsWords;
			}
			set
			{
				this.exceptIfSubjectContainsWords = value;
				base.TrackPropertyChanged("ExceptIfSubjectContainsWords");
			}
		}

		[DataMember]
		public string[] ExceptIfSubjectOrBodyContainsWords
		{
			get
			{
				return this.exceptIfSubjectOrBodyContainsWords;
			}
			set
			{
				this.exceptIfSubjectOrBodyContainsWords = value;
				base.TrackPropertyChanged("ExceptIfSubjectOrBodyContainsWords");
			}
		}

		[IgnoreDataMember]
		public NullableImportance ExceptIfWithImportance
		{
			get
			{
				return this.exceptIfWithImportance;
			}
			set
			{
				this.exceptIfWithImportance = value;
				base.TrackPropertyChanged("ExceptIfWithImportance");
			}
		}

		[DataMember(Name = "ExceptIfWithImportance", IsRequired = false, EmitDefaultValue = false)]
		public string ExceptIfWithImportanceString
		{
			get
			{
				return EnumUtilities.ToString<NullableImportance>(this.ExceptIfWithImportance);
			}
			set
			{
				this.ExceptIfWithImportance = EnumUtilities.Parse<NullableImportance>(value);
			}
		}

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public ulong? ExceptIfWithinSizeRangeMaximum
		{
			get
			{
				return this.exceptIfWithinSizeRangeMaximum;
			}
			set
			{
				this.exceptIfWithinSizeRangeMaximum = value;
				base.TrackPropertyChanged("ExceptIfWithinSizeRangeMaximum");
			}
		}

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public ulong? ExceptIfWithinSizeRangeMinimum
		{
			get
			{
				return this.exceptIfWithinSizeRangeMinimum;
			}
			set
			{
				this.exceptIfWithinSizeRangeMinimum = value;
				base.TrackPropertyChanged("ExceptIfWithinSizeRangeMinimum");
			}
		}

		[IgnoreDataMember]
		public NullableSensitivity ExceptIfWithSensitivity
		{
			get
			{
				return this.exceptIfWithSensitivity;
			}
			set
			{
				this.exceptIfWithSensitivity = value;
				base.TrackPropertyChanged("ExceptIfWithSensitivity");
			}
		}

		[DataMember(Name = "ExceptIfWithSensitivity", IsRequired = false, EmitDefaultValue = false)]
		public string ExceptIfWithSensitivityString
		{
			get
			{
				return EnumUtilities.ToString<NullableSensitivity>(this.ExceptIfWithSensitivity);
			}
			set
			{
				this.ExceptIfWithSensitivity = EnumUtilities.Parse<NullableSensitivity>(value);
			}
		}

		[DataMember]
		public string FlaggedForAction
		{
			get
			{
				return this.flaggedForAction;
			}
			set
			{
				this.flaggedForAction = value;
				base.TrackPropertyChanged("FlaggedForAction");
			}
		}

		[DataMember]
		public PeopleIdentity[] ForwardAsAttachmentTo
		{
			get
			{
				return this.forwardAsAttachmentTo;
			}
			set
			{
				this.forwardAsAttachmentTo = value;
				base.TrackPropertyChanged("ForwardAsAttachmentTo");
			}
		}

		[DataMember]
		public PeopleIdentity[] ForwardTo
		{
			get
			{
				return this.forwardTo;
			}
			set
			{
				this.forwardTo = value;
				base.TrackPropertyChanged("ForwardTo");
			}
		}

		[DataMember]
		public PeopleIdentity[] From
		{
			get
			{
				return this.from;
			}
			set
			{
				this.from = value;
				base.TrackPropertyChanged("From");
			}
		}

		[DataMember]
		public string[] FromAddressContainsWords
		{
			get
			{
				return this.fromAddressContainsWords;
			}
			set
			{
				this.fromAddressContainsWords = value;
				base.TrackPropertyChanged("FromAddressContainsWords");
			}
		}

		[DataMember]
		public Identity[] FromSubscription
		{
			get
			{
				return this.fromSubscription;
			}
			set
			{
				this.fromSubscription = value;
				base.TrackPropertyChanged("FromSubscription");
			}
		}

		[DataMember]
		public bool HasAttachment
		{
			get
			{
				return this.hasAttachment;
			}
			set
			{
				this.hasAttachment = value;
				base.TrackPropertyChanged("HasAttachment");
			}
		}

		[DataMember]
		public Identity[] HasClassification
		{
			get
			{
				return this.hasClassification;
			}
			set
			{
				this.hasClassification = value;
				base.TrackPropertyChanged("HasClassification");
			}
		}

		[DataMember]
		public string[] HeaderContainsWords
		{
			get
			{
				return this.headerContainsWords;
			}
			set
			{
				this.headerContainsWords = value;
				base.TrackPropertyChanged("HeaderContainsWords");
			}
		}

		[DataMember]
		public bool MarkAsRead
		{
			get
			{
				return this.markAsRead;
			}
			set
			{
				this.markAsRead = value;
				base.TrackPropertyChanged("MarkAsRead");
			}
		}

		[IgnoreDataMember]
		public NullableImportance MarkImportance
		{
			get
			{
				return this.markImportance;
			}
			set
			{
				this.markImportance = value;
				base.TrackPropertyChanged("MarkImportance");
			}
		}

		[DataMember(Name = "MarkImportance", IsRequired = false, EmitDefaultValue = false)]
		public string MarkImportanceString
		{
			get
			{
				return EnumUtilities.ToString<NullableImportance>(this.MarkImportance);
			}
			set
			{
				this.MarkImportance = EnumUtilities.Parse<NullableImportance>(value);
			}
		}

		[IgnoreDataMember]
		public NullableInboxRuleMessageType MessageTypeMatches
		{
			get
			{
				return this.messageTypeMatches;
			}
			set
			{
				this.messageTypeMatches = value;
				base.TrackPropertyChanged("MessageTypeMatches");
			}
		}

		[DataMember(Name = "MessageTypeMatches", IsRequired = false, EmitDefaultValue = false)]
		public string MessageTypeMatchesString
		{
			get
			{
				return EnumUtilities.ToString<NullableInboxRuleMessageType>(this.MessageTypeMatches);
			}
			set
			{
				this.MessageTypeMatches = EnumUtilities.Parse<NullableInboxRuleMessageType>(value);
			}
		}

		[DataMember]
		public bool MyNameInCcBox
		{
			get
			{
				return this.myNameInCcBox;
			}
			set
			{
				this.myNameInCcBox = value;
				base.TrackPropertyChanged("MyNameInCcBox");
			}
		}

		[DataMember]
		public bool MyNameInToBox
		{
			get
			{
				return this.myNameInToBox;
			}
			set
			{
				this.myNameInToBox = value;
				base.TrackPropertyChanged("MyNameInToBox");
			}
		}

		[DataMember]
		public bool MyNameInToOrCcBox
		{
			get
			{
				return this.myNameInToOrCcBox;
			}
			set
			{
				this.myNameInToOrCcBox = value;
				base.TrackPropertyChanged("MyNameInToOrCcBox");
			}
		}

		[DataMember]
		public bool MyNameNotInToBox
		{
			get
			{
				return this.myNameNotInToBox;
			}
			set
			{
				this.myNameNotInToBox = value;
				base.TrackPropertyChanged("MyNameNotInToBox");
			}
		}

		[DataMember]
		public Identity MoveToFolder
		{
			get
			{
				return this.moveToFolder;
			}
			set
			{
				this.moveToFolder = value;
				base.TrackPropertyChanged("MoveToFolder");
			}
		}

		[DataMember]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
				base.TrackPropertyChanged("Name");
			}
		}

		[DataMember]
		public int Priority
		{
			get
			{
				return this.priority;
			}
			set
			{
				this.priority = value;
				base.TrackPropertyChanged("Priority");
			}
		}

		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string ReceivedAfterDate
		{
			get
			{
				return this.receivedAfterDate;
			}
			set
			{
				this.receivedAfterDate = value;
				base.TrackPropertyChanged("ReceivedAfterDate");
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false)]
		public string ReceivedBeforeDate
		{
			get
			{
				return this.receivedBeforeDate;
			}
			set
			{
				this.receivedBeforeDate = value;
				base.TrackPropertyChanged("ReceivedBeforeDate");
			}
		}

		[DataMember]
		public string[] RecipientAddressContainsWords
		{
			get
			{
				return this.recipientAddressContainsWords;
			}
			set
			{
				this.recipientAddressContainsWords = value;
				base.TrackPropertyChanged("RecipientAddressContainsWords");
			}
		}

		[DataMember]
		public PeopleIdentity[] RedirectTo
		{
			get
			{
				return this.redirectTo;
			}
			set
			{
				this.redirectTo = value;
				base.TrackPropertyChanged("RedirectTo");
			}
		}

		[DataMember]
		public string[] SendTextMessageNotificationTo
		{
			get
			{
				return this.sendTextMessageNotificationTo;
			}
			set
			{
				this.sendTextMessageNotificationTo = value;
				base.TrackPropertyChanged("SendTextMessageNotificationTo");
			}
		}

		[DataMember]
		public bool SentOnlyToMe
		{
			get
			{
				return this.sentOnlyToMe;
			}
			set
			{
				this.sentOnlyToMe = value;
				base.TrackPropertyChanged("SentOnlyToMe");
			}
		}

		[DataMember]
		public PeopleIdentity[] SentTo
		{
			get
			{
				return this.sentTo;
			}
			set
			{
				this.sentTo = value;
				base.TrackPropertyChanged("SentTo");
			}
		}

		[DataMember]
		public bool StopProcessingRules
		{
			get
			{
				return this.stopProcessingRules;
			}
			set
			{
				this.stopProcessingRules = value;
				base.TrackPropertyChanged("StopProcessingRules");
			}
		}

		[DataMember]
		public string[] SubjectContainsWords
		{
			get
			{
				return this.subjectContainsWords;
			}
			set
			{
				this.subjectContainsWords = value;
				base.TrackPropertyChanged("SubjectContainsWords");
			}
		}

		[DataMember]
		public string[] SubjectOrBodyContainsWords
		{
			get
			{
				return this.subjectOrBodyContainsWords;
			}
			set
			{
				this.subjectOrBodyContainsWords = value;
				base.TrackPropertyChanged("SubjectOrBodyContainsWords");
			}
		}

		[IgnoreDataMember]
		public NullableImportance WithImportance
		{
			get
			{
				return this.withImportance;
			}
			set
			{
				this.withImportance = value;
				base.TrackPropertyChanged("WithImportance");
			}
		}

		[DataMember(Name = "WithImportance", IsRequired = false, EmitDefaultValue = false)]
		public string WithImportanceString
		{
			get
			{
				return EnumUtilities.ToString<NullableImportance>(this.WithImportance);
			}
			set
			{
				this.WithImportance = EnumUtilities.Parse<NullableImportance>(value);
			}
		}

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public ulong? WithinSizeRangeMaximum
		{
			get
			{
				return this.withinSizeRangeMaximum;
			}
			set
			{
				this.withinSizeRangeMaximum = value;
				base.TrackPropertyChanged("WithinSizeRangeMaximum");
			}
		}

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public ulong? WithinSizeRangeMinimum
		{
			get
			{
				return this.withinSizeRangeMinimum;
			}
			set
			{
				this.withinSizeRangeMinimum = value;
				base.TrackPropertyChanged("WithinSizeRangeMinimum");
			}
		}

		[IgnoreDataMember]
		public NullableSensitivity WithSensitivity
		{
			get
			{
				return this.withSensitivity;
			}
			set
			{
				this.withSensitivity = value;
				base.TrackPropertyChanged("WithSensitivity");
			}
		}

		[DataMember(Name = "WithSensitivity", IsRequired = false, EmitDefaultValue = false)]
		public string WithSensitivityString
		{
			get
			{
				return EnumUtilities.ToString<NullableSensitivity>(this.WithSensitivity);
			}
			set
			{
				this.WithSensitivity = EnumUtilities.Parse<NullableSensitivity>(value);
			}
		}

		private string[] applyCategory;

		private string[] bodyContainsWords;

		private Identity copyToFolder;

		private bool deleteMessage;

		private string[] exceptIfBodyContainsWords;

		private string exceptIfFlaggedForAction;

		private PeopleIdentity[] exceptIfFrom;

		private string[] exceptIfFromAddressContainsWords;

		private Identity[] exceptIfFromSubscription;

		private bool exceptIfHasAttachment;

		private Identity[] exceptIfHasClassification;

		private string[] exceptIfHeaderContainsWords;

		private NullableInboxRuleMessageType exceptIfMessageTypeMatches;

		private bool exceptIfMyNameInCcBox;

		private bool exceptIfMyNameInToBox;

		private bool exceptIfMyNameInToOrCcBox;

		private bool exceptIfMyNameNotInToBox;

		private string exceptIfReceivedAfterDate;

		private string exceptIfReceivedBeforeDate;

		private string[] exceptIfRecipientAddressContainsWords;

		private bool exceptIfSentOnlyToMe;

		private PeopleIdentity[] exceptIfSentTo;

		private string[] exceptIfSubjectContainsWords;

		private string[] exceptIfSubjectOrBodyContainsWords;

		private NullableImportance exceptIfWithImportance;

		private NullableSensitivity exceptIfWithSensitivity;

		private ulong? exceptIfWithinSizeRangeMaximum;

		private ulong? exceptIfWithinSizeRangeMinimum;

		private string flaggedForAction;

		private PeopleIdentity[] forwardAsAttachmentTo;

		private PeopleIdentity[] forwardTo;

		private PeopleIdentity[] from;

		private string[] fromAddressContainsWords;

		private Identity[] fromSubscription;

		private bool hasAttachment;

		private Identity[] hasClassification;

		private string[] headerContainsWords;

		private bool markAsRead;

		private NullableImportance markImportance;

		private NullableInboxRuleMessageType messageTypeMatches;

		private Identity moveToFolder;

		private bool myNameInCcBox;

		private bool myNameInToBox;

		private bool myNameInToOrCcBox;

		private bool myNameNotInToBox;

		private string name;

		private int priority;

		private string receivedAfterDate;

		private string receivedBeforeDate;

		private string[] recipientAddressContainsWords;

		private PeopleIdentity[] redirectTo;

		private string[] sendTextMessageNotificationTo;

		private bool sentOnlyToMe;

		private PeopleIdentity[] sentTo;

		private bool stopProcessingRules;

		private string[] subjectContainsWords;

		private string[] subjectOrBodyContainsWords;

		private NullableImportance withImportance;

		private NullableSensitivity withSensitivity;

		private ulong? withinSizeRangeMaximum;

		private ulong? withinSizeRangeMinimum;
	}
}
