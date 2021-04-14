using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ItemSchema : StoreObjectSchema
	{
		protected ItemSchema()
		{
			base.AddDependencies(new Schema[]
			{
				CoreItemSchema.Instance
			});
		}

		public new static ItemSchema Instance
		{
			get
			{
				if (ItemSchema.instance == null)
				{
					ItemSchema.instance = new ItemSchema();
				}
				return ItemSchema.instance;
			}
		}

		internal virtual void CoreObjectUpdate(CoreItem coreItem, CoreItemOperation operation)
		{
			Item.CoreObjectUpdateInternetMessageId(coreItem);
			Item.CoreObjectUpdatePreview(coreItem);
			Item.CoreObjectUpdateSentRepresentingType(coreItem);
			Item.CoreObjectUpdateAnnotationToken(coreItem);
			this.CoreObjectUpdateAllAttachmentsHidden(coreItem);
			if (coreItem != null && ((IValidatable)coreItem).ValidateAllProperties)
			{
				foreach (PropertyRule propertyRule in this.PropertyRules)
				{
					bool arg = propertyRule.WriteEnforce(coreItem.PropertyBag);
					ExTraceGlobals.StorageTracer.Information<string, bool>((long)this.GetHashCode(), "ItemSchema.CoreObjectUpdate. PropertyRule enfoced. Rule = {0}. Result = {1}", propertyRule.ToString(), arg);
				}
			}
		}

		internal virtual void CoreObjectUpdateComplete(CoreItem coreItem, SaveResult saveResult)
		{
		}

		protected virtual void CoreObjectUpdateAllAttachmentsHidden(CoreItem coreItem)
		{
			Item.CoreObjectUpdateAllAttachmentsHidden(coreItem);
		}

		protected virtual ICollection<PropertyRule> PropertyRules
		{
			get
			{
				return ItemSchema.ItemPropertyRules;
			}
		}

		[Autoload]
		public static readonly StorePropertyDefinition Id = CoreItemSchema.Id;

		public static readonly StorePropertyDefinition DocumentId = InternalSchema.DocumentId;

		public static readonly StorePropertyDefinition ConversationDocumentId = InternalSchema.ConversationDocumentId;

		[Autoload]
		public static readonly StorePropertyDefinition LastModifiedBy = InternalSchema.LastModifierName;

		[Autoload]
		public static readonly StorePropertyDefinition IsReplyRequested = InternalSchema.IsReplyRequested;

		[Autoload]
		public static readonly StorePropertyDefinition IsResponseRequested = InternalSchema.IsResponseRequested;

		[Autoload]
		public static readonly StorePropertyDefinition Categories = InternalSchema.Categories;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition HasAttachment = InternalSchema.HasAttachment;

		[Autoload]
		public static readonly StorePropertyDefinition Importance = InternalSchema.Importance;

		[Autoload]
		public static readonly StorePropertyDefinition Privacy = InternalSchema.Privacy;

		[Autoload]
		public static readonly StorePropertyDefinition NormalizedSubject = CoreItemSchema.NormalizedSubject;

		[Autoload]
		public static readonly StorePropertyDefinition Subject = CoreItemSchema.Subject;

		[Autoload]
		public static readonly StorePropertyDefinition SubjectPrefix = CoreItemSchema.SubjectPrefix;

		[Autoload]
		public static readonly StorePropertyDefinition Sensitivity = InternalSchema.Sensitivity;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition Sender = InternalSchema.Sender;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition From = InternalSchema.From;

		[Autoload]
		public static readonly StorePropertyDefinition Preview = InternalSchema.Preview;

		[Autoload]
		public static readonly StorePropertyDefinition OriginalSensitivity = InternalSchema.OriginalSensitivity;

		[Autoload]
		public static readonly StorePropertyDefinition InReplyTo = InternalSchema.InReplyTo;

		[Autoload]
		public static readonly StorePropertyDefinition ReceivedTime = CoreItemSchema.ReceivedTime;

		public static readonly StorePropertyDefinition XSimSlotNumber = InternalSchema.XSimSlotNumber;

		public static readonly StorePropertyDefinition XSentItem = InternalSchema.XSentItem;

		public static readonly StorePropertyDefinition XSentTime = InternalSchema.XSentTime;

		public static readonly StorePropertyDefinition XMmsMessageId = InternalSchema.XMmsMessageId;

		public static readonly StorePropertyDefinition RenewTime = CoreItemSchema.RenewTime;

		public static readonly StorePropertyDefinition ReceivedOrRenewTime = CoreItemSchema.ReceivedOrRenewTime;

		public static readonly StorePropertyDefinition RichContent = CoreItemSchema.RichContent;

		public static readonly StorePropertyDefinition MailboxGuid = CoreItemSchema.MailboxGuid;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition SentTime = InternalSchema.SentTime;

		[Autoload]
		internal static readonly StorePropertyDefinition BodyContentBase = InternalSchema.BodyContentBase;

		[Autoload]
		internal static readonly StorePropertyDefinition BodyContentLocation = InternalSchema.BodyContentLocation;

		[Autoload]
		public static readonly StorePropertyDefinition Codepage = CoreItemSchema.Codepage;

		[Autoload]
		public static readonly StorePropertyDefinition ConversationIndex = InternalSchema.ConversationIndex;

		[Autoload]
		public static readonly StorePropertyDefinition ConversationTopic = InternalSchema.ConversationTopic;

		[Autoload]
		public static readonly StorePropertyDefinition ConversationTopicHash = InternalSchema.ConversationTopicHash;

		[DetectCodepage]
		public static readonly StorePropertyDefinition SentRepresentingDisplayName = InternalSchema.SentRepresentingDisplayName;

		[Autoload]
		public static readonly StorePropertyDefinition SentRepresentingEmailAddress = InternalSchema.SentRepresentingEmailAddress;

		[Autoload]
		internal static readonly StorePropertyDefinition SentRepresentingEntryId = InternalSchema.SentRepresentingEntryId;

		[Autoload]
		internal static readonly StorePropertyDefinition SentRepresentingSearchKey = InternalSchema.SentRepresentingSearchKey;

		[Autoload]
		public static readonly StorePropertyDefinition SentRepresentingSmtpAddress = InternalSchema.SentRepresentingSmtpAddress;

		[Autoload]
		public static readonly StorePropertyDefinition SentRepresentingType = InternalSchema.SentRepresentingType;

		[Autoload]
		public static readonly StorePropertyDefinition InternetMessageId = InternalSchema.InternetMessageId;

		[Autoload]
		public static readonly StorePropertyDefinition InternetMessageIdHash = InternalSchema.InternetMessageIdHash;

		[Autoload]
		public static readonly StorePropertyDefinition InternetReferences = InternalSchema.InternetReferences;

		[Autoload]
		internal static readonly StorePropertyDefinition InternetCpid = InternalSchema.InternetCpid;

		[Autoload]
		public static readonly StorePropertyDefinition Size = CoreItemSchema.Size;

		[Autoload]
		public static readonly StorePropertyDefinition SendInternetEncoding = InternalSchema.SendInternetEncoding;

		[Autoload]
		public static readonly StorePropertyDefinition QuotaProhibitReceive = InternalSchema.ProhibitReceiveQuota;

		[Autoload]
		public static readonly StorePropertyDefinition QuotaProhibitSend = InternalSchema.ProhibitSendQuota;

		[Autoload]
		public static readonly StorePropertyDefinition SubmittedByAdmin = InternalSchema.SubmittedByAdmin;

		[Autoload]
		public static readonly StorePropertyDefinition SvrGeneratingQuotaMsg = InternalSchema.SvrGeneratingQuotaMsg;

		[Autoload]
		public static readonly StorePropertyDefinition PrimaryMbxOverQuota = InternalSchema.PrimaryMbxOverQuota;

		[Autoload]
		public static readonly StorePropertyDefinition IsPublicFolderQuotaMessage = InternalSchema.IsPublicFolderQuotaMessage;

		[Autoload]
		public static readonly StorePropertyDefinition QuotaType = InternalSchema.QuotaType;

		[Autoload]
		public static readonly StorePropertyDefinition FavLevelMask = CoreItemSchema.FavLevelMask;

		[Autoload]
		public static readonly StorePropertyDefinition StorageQuotaLimit = InternalSchema.StorageQuotaLimit;

		[Autoload]
		public static readonly StorePropertyDefinition ExcessStorageUsed = InternalSchema.ExcessStorageUsed;

		[Autoload]
		public static readonly StorePropertyDefinition SendRichInfo = InternalSchema.SendRichInfo;

		[Autoload]
		public static readonly StorePropertyDefinition Responsibility = InternalSchema.Responsibility;

		[Autoload]
		public static readonly StorePropertyDefinition RecipientType = InternalSchema.RecipientType;

		[Autoload]
		public static readonly StorePropertyDefinition SpamConfidenceLevel = InternalSchema.SpamConfidenceLevel;

		[Autoload]
		public static readonly StorePropertyDefinition ReminderDueBy = InternalSchema.ReminderDueBy;

		internal static readonly StorePropertyDefinition ReminderDueByInternal = InternalSchema.ReminderDueByInternal;

		[Autoload]
		public static readonly StorePropertyDefinition ReminderIsSet = InternalSchema.ReminderIsSet;

		public static readonly StorePropertyDefinition ReminderIsSetInternal = InternalSchema.ReminderIsSetInternal;

		[Autoload]
		public static readonly StorePropertyDefinition ReminderNextTime = InternalSchema.ReminderNextTime;

		[Autoload]
		public static readonly StorePropertyDefinition ReminderMinutesBeforeStart = InternalSchema.ReminderMinutesBeforeStart;

		[Autoload]
		public static readonly StorePropertyDefinition VoiceReminderPhoneNumber = InternalSchema.VoiceReminderPhoneNumber;

		[Autoload]
		public static readonly StorePropertyDefinition IsVoiceReminderEnabled = InternalSchema.IsVoiceReminderEnabled;

		[Autoload]
		public static readonly StorePropertyDefinition LocalDueDate = InternalSchema.LocalDueDate;

		[Autoload]
		public static readonly StorePropertyDefinition LocalStartDate = InternalSchema.LocalStartDate;

		[Autoload]
		public static readonly StorePropertyDefinition UtcStartDate = InternalSchema.UtcStartDate;

		[Autoload]
		public static readonly StorePropertyDefinition UtcDueDate = InternalSchema.UtcDueDate;

		[Autoload]
		public static readonly StorePropertyDefinition TaskStatus = InternalSchema.TaskStatus;

		public static readonly StorePropertyDefinition ReminderMinutesBeforeStartInternal = InternalSchema.ReminderMinutesBeforeStartInternal;

		[Autoload]
		internal static readonly StorePropertyDefinition SentMailEntryId = InternalSchema.SentMailEntryId;

		[Autoload]
		internal static readonly StorePropertyDefinition DeleteAfterSubmit = InternalSchema.DeleteAfterSubmit;

		[Autoload]
		public static readonly StorePropertyDefinition TimeZoneDefinitionStart = InternalSchema.TimeZoneDefinitionStart;

		[OptionalAutoload]
		[LegalTracking]
		internal static readonly StorePropertyDefinition HtmlBody = InternalSchema.HtmlBody;

		[LegalTracking]
		[OptionalAutoload]
		internal static readonly StorePropertyDefinition RtfBody = InternalSchema.RtfBody;

		[Autoload]
		internal static readonly StorePropertyDefinition RtfInSync = InternalSchema.RtfInSync;

		[LegalTracking]
		[OptionalAutoload]
		public static readonly StorePropertyDefinition TextBody = InternalSchema.TextBody;

		public static readonly StorePropertyDefinition BodyTag = InternalSchema.BodyTag;

		[Autoload]
		public static readonly StorePropertyDefinition NativeBodyInfo = CoreItemSchema.NativeBodyInfo;

		[Autoload]
		public static readonly StorePropertyDefinition FlagRequest = InternalSchema.FlagRequest;

		public static readonly StorePropertyDefinition RequestedAction = InternalSchema.RequestedAction;

		[Autoload]
		public static readonly StorePropertyDefinition IconIndex = InternalSchema.IconIndex;

		[Autoload]
		public static readonly StorePropertyDefinition PercentComplete = InternalSchema.PercentComplete;

		[Autoload]
		public static readonly StorePropertyDefinition IsToDoItem = InternalSchema.IsToDoItem;

		[Autoload]
		public static readonly StorePropertyDefinition ConversationId = InternalSchema.ConversationId;

		[Autoload]
		public static readonly StorePropertyDefinition ConversationIdHash = InternalSchema.ConversationIdHash;

		[Autoload]
		public static readonly StorePropertyDefinition ConversationIndexTracking = InternalSchema.ConversationIndexTracking;

		public static readonly StorePropertyDefinition ConversationIndexTrackingEx = InternalSchema.ConversationIndexTrackingEx;

		[Autoload]
		public static readonly StorePropertyDefinition IsFlagSetForRecipient = InternalSchema.IsFlagSetForRecipient;

		[Autoload]
		public static readonly StorePropertyDefinition PropertyExistenceTracker = InternalSchema.PropertyExistenceTracker;

		[LegalTracking]
		public static readonly StorePropertyDefinition AttachmentContent = InternalSchema.AttachmentContent;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition DisplayTo = InternalSchema.DisplayTo;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition DisplayCc = InternalSchema.DisplayCc;

		[LegalTracking]
		public static readonly StorePropertyDefinition DisplayBcc = InternalSchema.DisplayBcc;

		public static readonly StorePropertyDefinition ParentDisplayName = InternalSchema.ParentDisplayName;

		public static readonly StorePropertyDefinition ArticleId = InternalSchema.ArticleId;

		public static readonly StorePropertyDefinition ImapId = InternalSchema.ImapId;

		public static readonly StorePropertyDefinition OriginalSourceServerVersion = InternalSchema.OriginalSourceServerVersion;

		public static readonly StorePropertyDefinition ImapInternalDate = InternalSchema.ImapInternalDate;

		public static readonly StorePropertyDefinition IsUnmodified = InternalSchema.IsUnmodified;

		public static readonly StorePropertyDefinition Not822Renderable = InternalSchema.Not822Renderable;

		public static readonly StorePropertyDefinition ElcAutoCopyTag = InternalSchema.ElcAutoCopyTag;

		public static readonly StorePropertyDefinition ElcMoveDate = InternalSchema.ElcMoveDate;

		public static readonly StorePropertyDefinition EHAMigrationExpiryDate = InternalSchema.EHAMigrationExpirationDate;

		public static readonly StorePropertyDefinition RetentionDate = InternalSchema.RetentionDate;

		public static readonly StorePropertyDefinition ArchiveDate = InternalSchema.ArchiveDate;

		public static readonly StorePropertyDefinition StartDateEtc = InternalSchema.StartDateEtc;

		internal static readonly StorePropertyDefinition RowType = InternalSchema.RowType;

		internal static readonly StorePropertyDefinition SyncCustomState = InternalSchema.SyncCustomState;

		[Autoload]
		public static readonly StorePropertyDefinition ItemColor = InternalSchema.ItemColor;

		public static readonly StorePropertyDefinition FlagStatus = InternalSchema.FlagStatus;

		public static readonly StorePropertyDefinition FlagCompleteTime = InternalSchema.FlagCompleteTime;

		public static readonly StorePropertyDefinition IsClassified = InternalSchema.IsClassified;

		public static readonly StorePropertyDefinition Classification = InternalSchema.Classification;

		public static readonly StorePropertyDefinition ClassificationDescription = InternalSchema.ClassificationDescription;

		public static readonly StorePropertyDefinition ClassificationGuid = InternalSchema.ClassificationGuid;

		public static readonly StorePropertyDefinition ClassificationKeep = InternalSchema.ClassificationKeep;

		public static readonly StorePropertyDefinition QuarantineOriginalSender = InternalSchema.QuarantineOriginalSender;

		public static readonly StorePropertyDefinition JournalingRemoteAccounts = InternalSchema.JournalingRemoteAccounts;

		public static readonly StorePropertyDefinition PurportedSenderDomain = InternalSchema.PurportedSenderDomain;

		public static readonly StorePropertyDefinition BlockStatus = InternalSchema.BlockStatus;

		public static readonly StorePropertyDefinition ReplyTemplateId = InternalSchema.ReplyTemplateId;

		public static readonly StorePropertyDefinition RuleTriggerHistory = InternalSchema.RuleTriggerHistory;

		public static readonly StorePropertyDefinition DelegatedByRule = InternalSchema.DelegatedByRule;

		public static readonly StorePropertyDefinition OriginalMessageEntryId = InternalSchema.OriginalMessageEntryId;

		public static readonly StorePropertyDefinition OriginalMessageSvrEId = InternalSchema.OriginalMessageSvrEId;

		public static readonly StorePropertyDefinition DeferredActionMessageBackPatched = InternalSchema.DeferredActionMessageBackPatched;

		public static readonly StorePropertyDefinition HasDeferredActionMessage = InternalSchema.HasDeferredActionMessage;

		[Autoload]
		public static readonly StorePropertyDefinition MessageStatus = CoreItemSchema.MessageStatus;

		public static readonly StorePropertyDefinition MoveToFolderEntryId = InternalSchema.MoveToFolderEntryId;

		public static readonly StorePropertyDefinition MoveToStoreEntryId = InternalSchema.MoveToStoreEntryId;

		public static readonly StorePropertyDefinition IsAutoForwarded = InternalSchema.IsAutoForwarded;

		public static readonly StorePropertyDefinition FlagSubject = InternalSchema.FlagSubject;

		public static readonly StorePropertyDefinition SearchFullText = InternalSchema.SearchFullText;

		public static readonly StorePropertyDefinition SearchSender = InternalSchema.SearchSender;

		public static readonly StorePropertyDefinition SearchRecipients = InternalSchema.SearchRecipients;

		public static readonly StorePropertyDefinition SearchRecipientsTo = InternalSchema.SearchRecipientsTo;

		public static readonly StorePropertyDefinition SearchRecipientsCc = InternalSchema.SearchRecipientsCc;

		public static readonly StorePropertyDefinition SearchRecipientsBcc = InternalSchema.SearchRecipientsBcc;

		public static readonly StorePropertyDefinition SearchAllIndexedProps = InternalSchema.SearchAllIndexedProps;

		public static readonly StorePropertyDefinition SearchIsPartiallyIndexed = InternalSchema.SearchIsPartiallyIndexed;

		public static readonly StorePropertyDefinition SearchFullTextSubject = InternalSchema.SearchFullTextSubject;

		public static readonly StorePropertyDefinition SearchFullTextBody = InternalSchema.SearchFullTextBody;

		[Autoload]
		public static readonly StorePropertyDefinition CompleteDate = InternalSchema.CompleteDate;

		[Autoload]
		public static readonly StorePropertyDefinition EdgePcl = InternalSchema.ContentFilterPcl;

		[Autoload]
		public static readonly StorePropertyDefinition LinkEnabled = InternalSchema.LinkEnabled;

		[Autoload]
		public static readonly StorePropertyDefinition IsComplete = InternalSchema.IsComplete;

		public static readonly StorePropertyDefinition PopImapPoisonMessageStamp = InternalSchema.PopImapPoisonMessageStamp;

		public static readonly StorePropertyDefinition PopMIMESize = InternalSchema.PopMIMESize;

		public static readonly StorePropertyDefinition PopMIMEOptions = InternalSchema.PopMIMEOptions;

		public static readonly StorePropertyDefinition ImapMIMESize = InternalSchema.ImapMIMESize;

		public static readonly StorePropertyDefinition ImapMIMEOptions = InternalSchema.ImapMIMEOptions;

		public static readonly StorePropertyDefinition ImapAppendStamp = InternalSchema.XMsExchImapAppendStamp;

		public static readonly StorePropertyDefinition ProtocolLog = InternalSchema.ProtocolLog;

		[Autoload]
		public static readonly StorePropertyDefinition CloudId = InternalSchema.CloudId;

		public static readonly StorePropertyDefinition CloudVersion = InternalSchema.CloudVersion;

		public static readonly StorePropertyDefinition InstanceKey = InternalSchema.InstanceKey;

		public static readonly StorePropertyDefinition ResentFrom = InternalSchema.ResentFrom;

		public static readonly StorePropertyDefinition PredictedActions = InternalSchema.PredictedActions;

		public static readonly StorePropertyDefinition InferencePredictedReplyForwardReasons = InternalSchema.InferencePredictedReplyForwardReasons;

		public static readonly StorePropertyDefinition InferencePredictedDeleteReasons = InternalSchema.InferencePredictedDeleteReasons;

		public static readonly StorePropertyDefinition InferencePredictedIgnoreReasons = InternalSchema.InferencePredictedIgnoreReasons;

		[Autoload]
		public static readonly StorePropertyDefinition IsClutter = InternalSchema.IsClutter;

		public static readonly StorePropertyDefinition OriginalDeliveryFolderInfo = InternalSchema.OriginalDeliveryFolderInfo;

		public static readonly StorePropertyDefinition ExtractedMeetings = InternalSchema.ExtractedMeetings;

		public static readonly StorePropertyDefinition ExtractedTasks = InternalSchema.ExtractedTasks;

		public static readonly StorePropertyDefinition ExtractedAddresses = InternalSchema.ExtractedAddresses;

		public static readonly StorePropertyDefinition ExtractedKeywords = InternalSchema.ExtractedKeywords;

		public static readonly StorePropertyDefinition ExtractedPhones = InternalSchema.ExtractedPhones;

		public static readonly StorePropertyDefinition ExtractedEmails = InternalSchema.ExtractedEmails;

		public static readonly StorePropertyDefinition ExtractedUrls = InternalSchema.ExtractedUrls;

		public static readonly StorePropertyDefinition ExtractedContacts = InternalSchema.ExtractedContacts;

		public static readonly StorePropertyDefinition ExtractedMeetingsExists = InternalSchema.ExtractedMeetingsExists;

		public static readonly StorePropertyDefinition ExtractedTasksExists = InternalSchema.ExtractedTasksExists;

		public static readonly StorePropertyDefinition ExtractedAddressesExists = InternalSchema.ExtractedAddressesExists;

		public static readonly StorePropertyDefinition ExtractedKeywordsExists = InternalSchema.ExtractedKeywordsExists;

		public static readonly StorePropertyDefinition ExtractedUrlsExists = InternalSchema.ExtractedUrlsExists;

		public static readonly StorePropertyDefinition ExtractedPhonesExists = InternalSchema.ExtractedPhonesExists;

		public static readonly StorePropertyDefinition ExtractedEmailsExists = InternalSchema.ExtractedEmailsExists;

		public static readonly StorePropertyDefinition ExtractedContactsExists = InternalSchema.ExtractedContactsExists;

		public static readonly StorePropertyDefinition XmlExtractedMeetings = InternalSchema.XmlExtractedMeetings;

		public static readonly StorePropertyDefinition XmlExtractedTasks = InternalSchema.XmlExtractedTasks;

		public static readonly StorePropertyDefinition XmlExtractedAddresses = InternalSchema.XmlExtractedAddresses;

		public static readonly StorePropertyDefinition XmlExtractedKeywords = InternalSchema.XmlExtractedKeywords;

		public static readonly StorePropertyDefinition XmlExtractedPhones = InternalSchema.XmlExtractedPhones;

		public static readonly StorePropertyDefinition XmlExtractedEmails = InternalSchema.XmlExtractedEmails;

		public static readonly StorePropertyDefinition XmlExtractedUrls = InternalSchema.XmlExtractedUrls;

		public static readonly StorePropertyDefinition XmlExtractedContacts = InternalSchema.XmlExtractedContacts;

		public static readonly StorePropertyDefinition AnnotationToken = InternalSchema.AnnotationToken;

		public static readonly StorePropertyDefinition DetectedLanguage = InternalSchema.DetectedLanguage;

		public static readonly StorePropertyDefinition IsPartiallyIndexed = InternalSchema.IsPartiallyIndexed;

		public static readonly StorePropertyDefinition LastIndexingAttemptTime = InternalSchema.LastIndexingAttemptTime;

		public static readonly StorePropertyDefinition IndexingErrorCode = InternalSchema.IndexingErrorCode;

		[Autoload]
		public static readonly StorePropertyDefinition Fid = InternalSchema.Fid;

		[Autoload]
		public static readonly StorePropertyDefinition DlpSenderOverride = InternalSchema.DlpSenderOverride;

		[Autoload]
		public static readonly StorePropertyDefinition DlpFalsePositive = InternalSchema.DlpFalsePositive;

		[Autoload]
		public static readonly StorePropertyDefinition DlpDetectedClassifications = InternalSchema.DlpDetectedClassifications;

		[Autoload]
		public static readonly StorePropertyDefinition DlpDetectedClassificationObjects = InternalSchema.DlpDetectedClassificationObjects;

		[Autoload]
		public static readonly StorePropertyDefinition HasDlpDetectedClassifications = InternalSchema.HasDlpDetectedClassifications;

		[Autoload]
		public static readonly StorePropertyDefinition RecoveryOptions = InternalSchema.RecoveryOptions;

		[Autoload]
		public static readonly StorePropertyDefinition ConversationCreatorSID = InternalSchema.ConversationCreatorSID;

		public static readonly StorePropertyDefinition ConversationFamilyId = InternalSchema.ConversationFamilyId;

		public static readonly StorePropertyDefinition ConversationFamilyIndex = InternalSchema.ConversationFamilyIndex;

		[Autoload]
		public static readonly StorePropertyDefinition ExchangeApplicationFlags = InternalSchema.ExchangeApplicationFlags;

		[Autoload]
		public static readonly StorePropertyDefinition SupportsSideConversation = InternalSchema.SupportsSideConversation;

		[Autoload]
		public static readonly StorePropertyDefinition InferenceProcessingNeeded = InternalSchema.InferenceProcessingNeeded;

		[Autoload]
		public static readonly StorePropertyDefinition InferenceProcessingActions = InternalSchema.InferenceProcessingActions;

		public static readonly StorePropertyDefinition InferenceClassificationResult = InternalSchema.InferenceClassificationResult;

		public static readonly StorePropertyDefinition WorkingSetId = InternalSchema.WorkingSetId;

		public static readonly StorePropertyDefinition WorkingSetSource = InternalSchema.WorkingSetSource;

		public static readonly StorePropertyDefinition WorkingSetSourcePartition = InternalSchema.WorkingSetSourcePartition;

		public static readonly StorePropertyDefinition WorkingSetFlags = InternalSchema.WorkingSetFlags;

		public static readonly StorePropertyDefinition ConversationLoadRequiredByInference = InternalSchema.ConversationLoadRequiredByInference;

		public static readonly StorePropertyDefinition InferenceConversationClutterActionApplied = InternalSchema.InferenceConversationClutterActionApplied;

		public static readonly StorePropertyDefinition InferenceNeverClutterOverrideApplied = InternalSchema.InferenceNeverClutterOverrideApplied;

		public static readonly StorePropertyDefinition ItemMovedByRule = InternalSchema.ItemMovedByRule;

		public static readonly StorePropertyDefinition ItemMovedByConversationAction = InternalSchema.ItemMovedByConversationAction;

		public static readonly StorePropertyDefinition IsStopProcessingRuleApplicable = InternalSchema.IsStopProcessingRuleApplicable;

		private static ItemSchema instance = null;

		private static readonly PropertyRule[] ItemPropertyRules = new PropertyRule[]
		{
			PropertyRuleLibrary.TruncateSubject
		};
	}
}
