using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MessageItemSchema : ItemSchema
	{
		public new static MessageItemSchema Instance
		{
			get
			{
				if (MessageItemSchema.instance == null)
				{
					MessageItemSchema.instance = new MessageItemSchema();
				}
				return MessageItemSchema.instance;
			}
		}

		internal override void CoreObjectUpdate(CoreItem coreItem, CoreItemOperation operation)
		{
			base.CoreObjectUpdate(coreItem, operation);
			MessageItem.CoreObjectUpdateConversationTopic(coreItem);
			MessageItem.CoreObjectUpdateConversationIndex(coreItem);
			MessageItem.CoreObjectUpdateConversationIndexFixup(coreItem, operation);
			MessageItem.CoreObjectUpdateIconIndex(coreItem);
			MessageItem.CoreObjectUpdateMimeSkeleton(coreItem);
			if (operation == CoreItemOperation.Send && coreItem != null && coreItem.Session != null && coreItem.Session.ActivitySession != null)
			{
				StoreObjectId internalStoreObjectId = ((ICoreObject)coreItem).InternalStoreObjectId;
				string name = base.GetType().Name;
				coreItem.Session.ActivitySession.CaptureMessageSent(internalStoreObjectId, name);
			}
		}

		protected override void AddConstraints(List<StoreObjectConstraint> constraints)
		{
			constraints.Add(new ExtendedRuleConditionConstraint());
			base.AddConstraints(constraints);
		}

		public static readonly StorePropertyDefinition MessageBccMe = InternalSchema.MessageBccMe;

		[Autoload]
		public static readonly StorePropertyDefinition DRMLicense = InternalSchema.DRMLicense;

		[Autoload]
		public static readonly StorePropertyDefinition DRMServerLicenseCompressed = InternalSchema.DRMServerLicenseCompressed;

		[Autoload]
		internal static readonly StorePropertyDefinition DRMServerLicense = InternalSchema.DRMServerLicense;

		[Autoload]
		public static readonly StorePropertyDefinition DRMRights = InternalSchema.DRMRights;

		[Autoload]
		public static readonly StorePropertyDefinition DRMExpiryTime = InternalSchema.DRMExpiryTime;

		[Autoload]
		public static readonly StorePropertyDefinition DRMPropsSignature = InternalSchema.DRMPropsSignature;

		internal static readonly StorePropertyDefinition DrmPublishLicense = InternalSchema.DrmPublishLicense;

		public static readonly StorePropertyDefinition DRMPrelicenseFailure = InternalSchema.DRMPrelicenseFailure;

		public static readonly StorePropertyDefinition IsSigned = InternalSchema.IsSigned;

		public static readonly StorePropertyDefinition IsReadReceipt = InternalSchema.IsReadReceipt;

		public static readonly StorePropertyDefinition DeliverAsRead = InternalSchema.DeliverAsRead;

		[Autoload]
		public static readonly StorePropertyDefinition DavSubmitData = InternalSchema.DavSubmitData;

		[Autoload]
		public static readonly StorePropertyDefinition IsReadReceiptPending = InternalSchema.IsReadReceiptPending;

		[Autoload]
		public static readonly StorePropertyDefinition IsNotReadReceiptPending = InternalSchema.IsNotReadReceiptPending;

		[Autoload]
		public static readonly StorePropertyDefinition Flags = CoreItemSchema.Flags;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedUrl = InternalSchema.LinkedUrl;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedId = InternalSchema.LinkedId;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedSiteUrl = InternalSchema.LinkedSiteUrl;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedObjectVersion = InternalSchema.LinkedObjectVersion;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedDocumentCheckOutTo = InternalSchema.LinkedDocumentCheckOutTo;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedDocumentSize = InternalSchema.LinkedDocumentSize;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedPendingState = InternalSchema.LinkedPendingState;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedLastFullSyncTime = InternalSchema.LinkedLastFullSyncTime;

		[Autoload]
		public static readonly StorePropertyDefinition LinkedItemUpdateHistory = InternalSchema.LinkedItemUpdateHistory;

		[Autoload]
		public static readonly StorePropertyDefinition UnifiedPolicyNotificationId = InternalSchema.UnifiedPolicyNotificationId;

		[Autoload]
		public static readonly StorePropertyDefinition UnifiedPolicyNotificationData = InternalSchema.UnifiedPolicyNotificationData;

		[Autoload]
		public static readonly StorePropertyDefinition IsReadReceiptRequested = InternalSchema.IsReadReceiptRequested;

		[Autoload]
		public static readonly StorePropertyDefinition DeferUnreadFlag = InternalSchema.ItemTemporaryFlag;

		[DetectCodepage]
		public static readonly StorePropertyDefinition ReadReceiptDisplayName = InternalSchema.ReadReceiptDisplayName;

		public static readonly StorePropertyDefinition ReadReceiptEmailAddress = InternalSchema.ReadReceiptEmailAddress;

		[Autoload]
		public static readonly StorePropertyDefinition ReadReceiptAddrType = InternalSchema.ReadReceiptAddrType;

		[Autoload]
		public static readonly StorePropertyDefinition LastVerbExecuted = InternalSchema.LastVerbExecuted;

		[Autoload]
		public static readonly StorePropertyDefinition LastVerbExecutionTime = InternalSchema.LastVerbExecutionTime;

		[Autoload]
		public static readonly StorePropertyDefinition ReplyForwardStatus = InternalSchema.ReplyForwardStatus;

		[Autoload]
		public static readonly StorePropertyDefinition ReplyToBlob = InternalSchema.ReplyToBlob;

		public static readonly StorePropertyDefinition ReplyToBlobExists = InternalSchema.ReplyToBlobExists;

		[DetectCodepage]
		public static readonly StorePropertyDefinition ReplyToNames = InternalSchema.ReplyToNames;

		public static readonly StorePropertyDefinition ReplyToNamesExists = InternalSchema.ReplyToNamesExists;

		[Autoload]
		public static readonly StorePropertyDefinition LikersBlob = InternalSchema.LikersBlob;

		[DetectCodepage]
		public static readonly StorePropertyDefinition LikeCount = InternalSchema.LikeCount;

		public static readonly StorePropertyDefinition PeopleCentricConversationId = InternalSchema.PeopleCentricConversationId;

		[Autoload]
		public static readonly StorePropertyDefinition SenderAddressType = InternalSchema.SenderAddressType;

		[DetectCodepage]
		public static readonly StorePropertyDefinition SenderDisplayName = InternalSchema.SenderDisplayName;

		[Autoload]
		public static readonly StorePropertyDefinition SenderEmailAddress = InternalSchema.SenderEmailAddress;

		[Autoload]
		internal static readonly StorePropertyDefinition SenderEntryId = InternalSchema.SenderEntryId;

		[Autoload]
		public static readonly StorePropertyDefinition DRMProtectionNeeded = InternalSchema.XMsExchangeOrganizationRightsProtectMessage;

		[Autoload]
		public static readonly StorePropertyDefinition IsResend = CoreItemSchema.IsResend;

		[Autoload]
		public static readonly StorePropertyDefinition NeedSpecialRecipientProcessing = CoreItemSchema.NeedSpecialRecipientProcessing;

		public static readonly StorePropertyDefinition SwappedToDoStore = InternalSchema.SwappedToDoStore;

		[Autoload]
		public static readonly StorePropertyDefinition AllAttachmentsHidden = InternalSchema.AllAttachmentsHidden;

		public static readonly StorePropertyDefinition MessageAudioNotes = InternalSchema.MessageAudioNotes;

		public static readonly StorePropertyDefinition SenderTelephoneNumber = InternalSchema.SenderTelephoneNumber;

		public static readonly StorePropertyDefinition PstnCallbackTelephoneNumber = InternalSchema.PstnCallbackTelephoneNumber;

		public static readonly StorePropertyDefinition UcSubject = InternalSchema.UcSubject;

		public static readonly StorePropertyDefinition VoiceMessageDuration = InternalSchema.VoiceMessageDuration;

		public static readonly StorePropertyDefinition VoiceMessageSenderName = InternalSchema.VoiceMessageSenderName;

		public static readonly StorePropertyDefinition FaxNumberOfPages = InternalSchema.FaxNumberOfPages;

		public static readonly StorePropertyDefinition VoiceMessageAttachmentOrder = InternalSchema.VoiceMessageAttachmentOrder;

		public static readonly StorePropertyDefinition QuickCaptureReminders = InternalSchema.ModernReminders;

		public static readonly StorePropertyDefinition ModernReminders = InternalSchema.ModernReminders;

		public static readonly StorePropertyDefinition ModernRemindersState = InternalSchema.ModernRemindersState;

		public static readonly StorePropertyDefinition CallId = InternalSchema.CallId;

		public static readonly StorePropertyDefinition XMsExchangeUMPartnerAssignedID = InternalSchema.XMsExchangeUMPartnerAssignedID;

		public static readonly StorePropertyDefinition XMsExchangeUMPartnerContent = InternalSchema.XMsExchangeUMPartnerContent;

		public static readonly StorePropertyDefinition XMsExchangeUMPartnerContext = InternalSchema.XMsExchangeUMPartnerContext;

		public static readonly StorePropertyDefinition XMsExchangeUMPartnerStatus = InternalSchema.XMsExchangeUMPartnerStatus;

		public static readonly StorePropertyDefinition XMsExchangeUMDialPlanLanguage = InternalSchema.XMsExchangeUMDialPlanLanguage;

		public static readonly StorePropertyDefinition XMsExchangeUMCallerInformedOfAnalysis = InternalSchema.XMsExchangeUMCallerInformedOfAnalysis;

		public static readonly StorePropertyDefinition ReceivedSPF = InternalSchema.ReceivedSPF;

		public static readonly StorePropertyDefinition AsrData = InternalSchema.AsrData;

		public static readonly StorePropertyDefinition XCDRDataCallStartTime = InternalSchema.XCDRDataCallStartTime;

		public static readonly StorePropertyDefinition XCDRDataCallType = InternalSchema.XCDRDataCallType;

		public static readonly StorePropertyDefinition XCDRDataCallIdentity = InternalSchema.XCDRDataCallIdentity;

		public static readonly StorePropertyDefinition XCDRDataParentCallIdentity = InternalSchema.XCDRDataParentCallIdentity;

		public static readonly StorePropertyDefinition XCDRDataUMServerName = InternalSchema.XCDRDataUMServerName;

		public static readonly StorePropertyDefinition XCDRDataDialPlanGuid = InternalSchema.XCDRDataDialPlanGuid;

		public static readonly StorePropertyDefinition XCDRDataDialPlanName = InternalSchema.XCDRDataDialPlanName;

		public static readonly StorePropertyDefinition XCDRDataCallDuration = InternalSchema.XCDRDataCallDuration;

		public static readonly StorePropertyDefinition XCDRDataGatewayGuid = InternalSchema.XCDRDataGatewayGuid;

		public static readonly StorePropertyDefinition XCDRDataIPGatewayAddress = InternalSchema.XCDRDataIPGatewayAddress;

		public static readonly StorePropertyDefinition XCDRDataIPGatewayName = InternalSchema.XCDRDataIPGatewayName;

		public static readonly StorePropertyDefinition XCDRDataCalledPhoneNumber = InternalSchema.XCDRDataCalledPhoneNumber;

		public static readonly StorePropertyDefinition XCDRDataCallerPhoneNumber = InternalSchema.XCDRDataCallerPhoneNumber;

		public static readonly StorePropertyDefinition XCDRDataOfferResult = InternalSchema.XCDRDataOfferResult;

		public static readonly StorePropertyDefinition XCDRDataDropCallReason = InternalSchema.XCDRDataDropCallReason;

		public static readonly StorePropertyDefinition XCDRDataReasonForCall = InternalSchema.XCDRDataReasonForCall;

		public static readonly StorePropertyDefinition XCDRDataTransferredNumber = InternalSchema.XCDRDataTransferredNumber;

		public static readonly StorePropertyDefinition XCDRDataDialedString = InternalSchema.XCDRDataDialedString;

		public static readonly StorePropertyDefinition XCDRDataCallerMailboxAlias = InternalSchema.XCDRDataCallerMailboxAlias;

		public static readonly StorePropertyDefinition XCDRDataCalleeMailboxAlias = InternalSchema.XCDRDataCalleeMailboxAlias;

		public static readonly StorePropertyDefinition XCDRDataAutoAttendantName = InternalSchema.XCDRDataAutoAttendantName;

		public static readonly StorePropertyDefinition XCDRDataAudioCodec = InternalSchema.XCDRDataAudioCodec;

		public static readonly StorePropertyDefinition XCDRDataBurstDensity = InternalSchema.XCDRDataBurstDensity;

		public static readonly StorePropertyDefinition XCDRDataBurstDuration = InternalSchema.XCDRDataBurstDuration;

		public static readonly StorePropertyDefinition XCDRDataJitter = InternalSchema.XCDRDataJitter;

		public static readonly StorePropertyDefinition XCDRDataNMOS = InternalSchema.XCDRDataNMOS;

		public static readonly StorePropertyDefinition XCDRDataNMOSDegradation = InternalSchema.XCDRDataNMOSDegradation;

		public static readonly StorePropertyDefinition XCDRDataNMOSDegradationJitter = InternalSchema.XCDRDataNMOSDegradationJitter;

		public static readonly StorePropertyDefinition XCDRDataNMOSDegradationPacketLoss = InternalSchema.XCDRDataNMOSDegradationPacketLoss;

		public static readonly StorePropertyDefinition XCDRDataPacketLoss = InternalSchema.XCDRDataPacketLoss;

		public static readonly StorePropertyDefinition XCDRDataRoundTrip = InternalSchema.XCDRDataRoundTrip;

		public static readonly StorePropertyDefinition ExpiryTime = InternalSchema.ExpiryTime;

		[Autoload]
		public static readonly StorePropertyDefinition IsDeliveryReceiptRequested = InternalSchema.IsDeliveryReceiptRequested;

		[Autoload]
		public static readonly StorePropertyDefinition IsNonDeliveryReceiptRequested = InternalSchema.IsNonDeliveryReceiptRequested;

		[Autoload]
		public static readonly StorePropertyDefinition HasBeenSubmitted = InternalSchema.HasBeenSubmitted;

		[Autoload]
		public static readonly StorePropertyDefinition IsAssociated = InternalSchema.IsAssociated;

		[Autoload]
		public static readonly StorePropertyDefinition IsDraft = InternalSchema.IsDraft;

		[Autoload]
		public static readonly StorePropertyDefinition IsRead = InternalSchema.IsRead;

		[Autoload]
		public static readonly StorePropertyDefinition WasEverRead = InternalSchema.WasEverRead;

		public static readonly StorePropertyDefinition MapiHasAttachment = CoreItemSchema.MapiHasAttachment;

		internal static readonly StorePropertyDefinition MapiPriority = InternalSchema.MapiPriority;

		[Autoload]
		internal static readonly StorePropertyDefinition MapiReplyToBlob = InternalSchema.MapiReplyToBlob;

		internal static readonly StorePropertyDefinition MapiReplyToNames = InternalSchema.MapiReplyToNames;

		internal static readonly StorePropertyDefinition MapiLikersBlob = InternalSchema.MapiLikersBlob;

		internal static readonly StorePropertyDefinition MapiLikeCount = InternalSchema.MapiLikeCount;

		public static readonly StorePropertyDefinition MessageDeliveryNotificationSent = InternalSchema.MessageDeliveryNotificationSent;

		public static readonly StorePropertyDefinition MessageAnswered = InternalSchema.MessageAnswered;

		public static readonly StorePropertyDefinition MimeConversionFailed = InternalSchema.MimeConversionFailed;

		public static readonly StorePropertyDefinition MessageDelMarked = InternalSchema.MessageDelMarked;

		public static readonly StorePropertyDefinition MessageDraft = InternalSchema.MessageDraft;

		public static readonly StorePropertyDefinition MessageHidden = InternalSchema.MessageHidden;

		public static readonly StorePropertyDefinition MessageHighlighted = InternalSchema.MessageHighlighted;

		public static readonly StorePropertyDefinition MessageInConflict = CoreItemSchema.MessageInConflict;

		internal static readonly StorePropertyDefinition MessageRecipients = InternalSchema.MessageRecipients;

		public static readonly StorePropertyDefinition MessageRemoteDelete = InternalSchema.MessageRemoteDelete;

		public static readonly StorePropertyDefinition MessageRemoteDownload = InternalSchema.MessageRemoteDownload;

		public static readonly StorePropertyDefinition MessageTagged = InternalSchema.MessageTagged;

		public static readonly StorePropertyDefinition MID = InternalSchema.MID;

		public static readonly StorePropertyDefinition LTID = InternalSchema.LTID;

		public static readonly StorePropertyDefinition OriginalAuthorName = InternalSchema.OriginalAuthorName;

		public static readonly StorePropertyDefinition ReceivedRepresenting = InternalSchema.ReceivedRepresenting;

		public static readonly StorePropertyDefinition ReceivedRepresentingEntryId = InternalSchema.ReceivedRepresentingEntryId;

		public static readonly StorePropertyDefinition ReceivedRepresentingAddressType = InternalSchema.ReceivedRepresentingAddressType;

		[DetectCodepage]
		public static readonly StorePropertyDefinition ReceivedRepresentingDisplayName = InternalSchema.ReceivedRepresentingDisplayName;

		public static readonly StorePropertyDefinition ReceivedRepresentingEmailAddress = InternalSchema.ReceivedRepresentingEmailAddress;

		public static readonly StorePropertyDefinition ReceivedRepresentingSmtpAddress = InternalSchema.ReceivedRepresentingSmtpAddress;

		public static readonly StorePropertyDefinition ReceivedRepresentingSearchKey = InternalSchema.ReceivedRepresentingSearchKey;

		public static readonly StorePropertyDefinition ElcAutoCopyLabel = InternalSchema.ElcAutoCopyLabel;

		public static readonly StorePropertyDefinition SharingProviderGuid = InternalSchema.SharingProviderGuid;

		public static readonly StorePropertyDefinition SharingProviderName = InternalSchema.SharingProviderName;

		public static readonly StorePropertyDefinition SharingProviderUrl = InternalSchema.SharingProviderUrl;

		public static readonly StorePropertyDefinition SharingRemotePath = InternalSchema.SharingRemotePath;

		public static readonly StorePropertyDefinition SharingRemoteName = InternalSchema.SharingRemoteName;

		public static readonly StorePropertyDefinition SharingLocalName = InternalSchema.SharingLocalName;

		public static readonly StorePropertyDefinition SharingLocalUid = InternalSchema.SharingLocalUid;

		public static readonly StorePropertyDefinition SharingLocalType = InternalSchema.SharingLocalType;

		public static readonly StorePropertyDefinition SharingCapabilities = InternalSchema.SharingCapabilities;

		public static readonly StorePropertyDefinition SharingFlavor = InternalSchema.SharingFlavor;

		public static readonly StorePropertyDefinition SharingInstanceGuid = InternalSchema.SharingInstanceGuid;

		public static readonly StorePropertyDefinition SharingRemoteType = InternalSchema.SharingRemoteType;

		public static readonly StorePropertyDefinition SharingLastSync = InternalSchema.SharingLastSync;

		public static readonly StorePropertyDefinition SharingRssHash = InternalSchema.SharingRssHash;

		public static readonly StorePropertyDefinition SharingRemoteLastMod = InternalSchema.SharingRemoteLastMod;

		public static readonly StorePropertyDefinition SharingConfigUrl = InternalSchema.SharingConfigUrl;

		public static readonly StorePropertyDefinition SharingDetail = InternalSchema.SharingDetail;

		public static readonly StorePropertyDefinition SharingTimeToLive = InternalSchema.SharingTimeToLive;

		public static readonly StorePropertyDefinition SharingBindingEid = InternalSchema.SharingBindingEid;

		public static readonly StorePropertyDefinition SharingIndexEid = InternalSchema.SharingIndexEid;

		public static readonly StorePropertyDefinition SharingRemoteComment = InternalSchema.SharingRemoteComment;

		public static readonly StorePropertyDefinition SharingLocalStoreUid = InternalSchema.SharingLocalStoreUid;

		public static readonly StorePropertyDefinition SharingRemoteByteSize = InternalSchema.SharingRemoteByteSize;

		public static readonly StorePropertyDefinition SharingRemoteCrc = InternalSchema.SharingRemoteCrc;

		public static readonly StorePropertyDefinition SharingLastAutoSync = InternalSchema.SharingLastAutoSync;

		public static readonly StorePropertyDefinition SharingSavedSession = InternalSchema.SharingSavedSession;

		public static readonly StorePropertyDefinition SharingSubscriptionVersion = InternalSchema.SharingSubscriptionVersion;

		public static readonly StorePropertyDefinition SharingDetailedStatus = InternalSchema.SharingDetailedStatus;

		public static readonly StorePropertyDefinition SharingDiagnostics = InternalSchema.SharingDiagnostics;

		public static readonly StorePropertyDefinition SharingSendAsState = InternalSchema.SharingSendAsState;

		public static readonly StorePropertyDefinition SharingSendAsValidatedEmail = InternalSchema.SharingSendAsValidatedEmail;

		public static readonly StorePropertyDefinition SharingSendAsSubmissionUrl = InternalSchema.SharingSendAsSubmissionUrl;

		public static readonly StorePropertyDefinition SharingEwsUri = InternalSchema.SharingEwsUri;

		public static readonly StorePropertyDefinition SharingRemoteExchangeVersion = InternalSchema.SharingRemoteExchangeVersion;

		public static readonly StorePropertyDefinition SharingRemoteUserDomain = InternalSchema.SharingRemoteUserDomain;

		public static readonly StorePropertyDefinition RssServerLockStartTime = InternalSchema.RssServerLockStartTime;

		public static readonly StorePropertyDefinition RssServerLockTimeout = InternalSchema.RssServerLockTimeout;

		public static readonly StorePropertyDefinition RssServerLockClientName = InternalSchema.RssServerLockClientName;

		public static readonly StorePropertyDefinition ReceivedBy = InternalSchema.ReceivedBy;

		public static readonly StorePropertyDefinition ReceivedByAddrType = InternalSchema.ReceivedByAddrType;

		public static readonly StorePropertyDefinition ReceivedByEmailAddress = InternalSchema.ReceivedByEmailAddress;

		public static readonly StorePropertyDefinition ReceivedByEntryId = InternalSchema.ReceivedByEntryId;

		[DetectCodepage]
		public static readonly StorePropertyDefinition ReceivedByName = InternalSchema.ReceivedByName;

		public static readonly StorePropertyDefinition ReceivedBySearchKey = InternalSchema.ReceivedBySearchKey;

		[Autoload]
		public static readonly StorePropertyDefinition ReplyTime = InternalSchema.ReplyTime;

		internal static readonly StorePropertyDefinition TnefCorrelationKey = InternalSchema.TnefCorrelationKey;

		public static readonly StorePropertyDefinition TransportMessageHeaders = InternalSchema.TransportMessageHeaders;

		public static readonly StorePropertyDefinition OofReplyType = InternalSchema.OofReplyType;

		[Autoload]
		public static readonly StorePropertyDefinition AutoResponseSuppress = InternalSchema.AutoResponseSuppress;

		public static readonly StorePropertyDefinition MessageLocaleId = InternalSchema.MessageLocaleId;

		internal static readonly StorePropertyDefinition AssociatedSearchFolderLastUsedTime = InternalSchema.AssociatedSearchFolderLastUsedTime;

		public static readonly StorePropertyDefinition ToDoSubOrdinal = InternalSchema.ToDoSubOrdinal;

		public static readonly StorePropertyDefinition ToDoOrdinalDate = InternalSchema.ToDoOrdinalDate;

		public static readonly StorePropertyDefinition XMsExchOrganizationAuthDomain = InternalSchema.XMsExchOrganizationAuthDomain;

		public static readonly StorePropertyDefinition XMsExchOrganizationAuthAs = InternalSchema.XMsExchOrganizationAuthAs;

		public static readonly StorePropertyDefinition XMsExchOrganizationAuthMechanism = InternalSchema.XMsExchOrganizationAuthMechanism;

		public static readonly StorePropertyDefinition XMsExchOrganizationAuthSource = InternalSchema.XMsExchOrganizationAuthSource;

		public static readonly StorePropertyDefinition XMsExchOrganizationOriginalClientIPAddress = CoreItemSchema.XMsExchOrganizationOriginalClientIPAddress;

		public static readonly StorePropertyDefinition XMsExchOrganizationOriginalServerIPAddress = CoreItemSchema.XMsExchOrganizationOriginalServerIPAddress;

		public static readonly StorePropertyDefinition SenderIdStatus = InternalSchema.SenderIdStatus;

		public static readonly StorePropertyDefinition ApprovalAllowedDecisionMakers = InternalSchema.ApprovalAllowedDecisionMakers;

		public static readonly StorePropertyDefinition ApprovalRequestor = InternalSchema.ApprovalRequestor;

		public static readonly StorePropertyDefinition ApprovalDecisionMaker = InternalSchema.ApprovalDecisionMaker;

		public static readonly StorePropertyDefinition ApprovalDecision = InternalSchema.ApprovalDecision;

		public static readonly StorePropertyDefinition ApprovalDecisionTime = InternalSchema.ApprovalDecisionTime;

		public static readonly StorePropertyDefinition ApprovalRequestMessageId = InternalSchema.ApprovalRequestMessageId;

		public static readonly StorePropertyDefinition ApprovalStatus = InternalSchema.ApprovalStatus;

		public static readonly StorePropertyDefinition ApprovalDecisionMakersNdred = InternalSchema.ApprovalDecisionMakersNdred;

		public static readonly StorePropertyDefinition ApprovalApplicationId = InternalSchema.ApprovalApplicationId;

		public static readonly StorePropertyDefinition ApprovalApplicationData = InternalSchema.ApprovalApplicationData;

		public static readonly StorePropertyDefinition SecureSubmitFlags = InternalSchema.SecureSubmitFlags;

		[Autoload]
		public static readonly StorePropertyDefinition ClientSubmittedSecurely = CoreItemSchema.ClientSubmittedSecurely;

		[Autoload]
		public static readonly StorePropertyDefinition ServerSubmittedSecurely = InternalSchema.ServerSubmittedSecurely;

		[Autoload]
		public static readonly StorePropertyDefinition AcceptLanguage = InternalSchema.AcceptLanguage;

		public static readonly StorePropertyDefinition DlExpansionProhibited = InternalSchema.DlExpansionProhibited;

		public static readonly StorePropertyDefinition RecipientReassignmentProhibited = InternalSchema.RecipientReassignmentProhibited;

		public static readonly StorePropertyDefinition DeferredDeliveryTime = InternalSchema.DeferredDeliveryTime;

		public static readonly StorePropertyDefinition DeferredSendTime = InternalSchema.DeferredSendTime;

		public static readonly StorePropertyDefinition HasWrittenTracking = InternalSchema.HasWrittenTracking;

		public static readonly StorePropertyDefinition MessageSubmissionId = InternalSchema.MessageSubmissionId;

		public static readonly StorePropertyDefinition ProviderSubmitTime = InternalSchema.ProviderSubmitTime;

		public static readonly StorePropertyDefinition SenderSearchKey = InternalSchema.SenderSearchKey;

		public static readonly StorePropertyDefinition SenderSmtpAddress = InternalSchema.SenderSmtpAddress;

		public static readonly StorePropertyDefinition MapiRecurrenceType = InternalSchema.MapiRecurrenceType;

		[Autoload]
		internal static readonly StorePropertyDefinition ReportTag = InternalSchema.ReportTag;

		[Autoload]
		public static readonly StorePropertyDefinition VotingBlob = InternalSchema.OutlookUserPropsVerbStream;

		[Autoload]
		public static readonly StorePropertyDefinition VotingResponse = InternalSchema.VotingResponse;

		[Autoload]
		internal static readonly StorePropertyDefinition IsVotingResponse = InternalSchema.IsVotingResponse;

		internal static readonly StorePropertyDefinition LocalDirectory = InternalSchema.LocalDirectory;

		[Autoload]
		public static readonly StorePropertyDefinition MimeSkeleton = InternalSchema.MimeSkeleton;

		public static readonly StorePropertyDefinition BodyContentId = InternalSchema.BodyContentId;

		[Autoload]
		public static readonly StorePropertyDefinition MessageToMe = InternalSchema.MessageToMe;

		[Autoload]
		public static readonly StorePropertyDefinition MessageCcMe = InternalSchema.MessageCcMe;

		public static readonly StorePropertyDefinition FolderId = InternalSchema.FolderId;

		public static readonly StorePropertyDefinition XLoop = InternalSchema.XLoop;

		public static readonly StorePropertyDefinition DoNotDeliver = InternalSchema.DoNotDeliver;

		public static readonly StorePropertyDefinition DropMessageInHub = InternalSchema.DropMessageInHub;

		public static readonly StorePropertyDefinition SystemProbeDrop = InternalSchema.SystemProbeDrop;

		public static readonly StorePropertyDefinition XLAMNotificationId = InternalSchema.XLAMNotificationId;

		public static readonly StorePropertyDefinition MapiSubmitLamNotificationId = InternalSchema.MapiSubmitLamNotificationId;

		public static readonly StorePropertyDefinition MapiSubmitSystemProbeActivityId = InternalSchema.MapiSubmitSystemProbeActivityId;

		public static readonly StorePropertyDefinition XMSExchangeOutlookProtectionRuleVersion = InternalSchema.XMSExchangeOutlookProtectionRuleVersion;

		public static readonly StorePropertyDefinition XMSExchangeOutlookProtectionRuleConfigTimestamp = InternalSchema.XMSExchangeOutlookProtectionRuleConfigTimestamp;

		public static readonly StorePropertyDefinition XMSExchangeOutlookProtectionRuleOverridden = InternalSchema.XMSExchangeOutlookProtectionRuleOverridden;

		[Autoload]
		public static readonly StorePropertyDefinition RequireProtectedPlayOnPhone = InternalSchema.XRequireProtectedPlayOnPhone;

		[Autoload]
		public static readonly StorePropertyDefinition AppointmentState = InternalSchema.AppointmentState;

		public static readonly StorePropertyDefinition TextMessageDeliveryStatus = InternalSchema.TextMessageDeliveryStatus;

		public static readonly StorePropertyDefinition MessageAnnotation = InternalSchema.MessageAnnotation;

		public static readonly StorePropertyDefinition OscContactSources = InternalSchema.OscContactSources;

		public static readonly StorePropertyDefinition OscSyncEnabledOnServer = InternalSchema.OscSyncEnabledOnServer;

		public static readonly StorePropertyDefinition OutlookContactLinkDateTime = InternalSchema.OutlookContactLinkDateTime;

		public static readonly StorePropertyDefinition OutlookContactLinkVersion = InternalSchema.OutlookContactLinkVersion;

		public static readonly StorePropertyDefinition ExtractionResult = InternalSchema.ExtractionResult;

		public static readonly StorePropertyDefinition TriageFeatureVector = InternalSchema.TriageFeatureVector;

		public static readonly StorePropertyDefinition InferenceClassificationTrackingEx = InternalSchema.InferenceClassificationTrackingEx;

		public static readonly StorePropertyDefinition InferenceActionTruth = InternalSchema.InferenceActionTruth;

		public static readonly StorePropertyDefinition InferenceUniqueActionLabelData = InternalSchema.InferenceUniqueActionLabelData;

		public static readonly StorePropertyDefinition LatestMessageWordCount = InternalSchema.LatestMessageWordCount;

		[Autoload]
		public static readonly StorePropertyDefinition IsFromFavoriteSender = InternalSchema.IsFromFavoriteSender;

		[Autoload]
		public static readonly StorePropertyDefinition IsFromPerson = InternalSchema.IsFromPerson;

		[Autoload]
		public static readonly StorePropertyDefinition IsSpecificMessageReply = InternalSchema.IsSpecificMessageReply;

		[Autoload]
		public static readonly StorePropertyDefinition IsSpecificMessageReplyStamped = InternalSchema.IsSpecificMessageReplyStamped;

		[Autoload]
		public static readonly StorePropertyDefinition RelyOnConversationIndex = InternalSchema.RelyOnConversationIndex;

		[Autoload]
		public static readonly StorePropertyDefinition IsClutterOverridden = InternalSchema.IsClutterOverridden;

		[Autoload]
		public static readonly StorePropertyDefinition IsGroupEscalationMessage = InternalSchema.IsGroupEscalationMessage;

		public static readonly StorePropertyDefinition NeedGroupExpansion = InternalSchema.NeedGroupExpansion;

		public static readonly StorePropertyDefinition GroupExpansionRecipients = InternalSchema.GroupExpansionRecipients;

		public static readonly StorePropertyDefinition ToGroupExpansionRecipients = InternalSchema.ToGroupExpansionRecipients;

		public static readonly StorePropertyDefinition CcGroupExpansionRecipients = InternalSchema.CcGroupExpansionRecipients;

		public static readonly StorePropertyDefinition BccGroupExpansionRecipients = InternalSchema.BccGroupExpansionRecipients;

		public static readonly StorePropertyDefinition GroupExpansionError = InternalSchema.GroupExpansionError;

		public static readonly StorePropertyDefinition InferenceMessageIdentifier = InternalSchema.InferenceMessageIdentifier;

		public static readonly StorePropertyDefinition SenderRelevanceScore = InternalSchema.SenderRelevanceScore;

		public static readonly StorePropertyDefinition SenderClass = InternalSchema.SenderClass;

		public static readonly StorePropertyDefinition CurrentFolderReason = InternalSchema.CurrentFolderReason;

		public static readonly PropertyDefinition[] SingleRecipientProperties = new PropertyDefinition[]
		{
			ItemSchema.From,
			ItemSchema.Sender,
			MessageItemSchema.ReceivedRepresenting,
			MessageItemSchema.ReceivedBy
		};

		private static MessageItemSchema instance = null;
	}
}
