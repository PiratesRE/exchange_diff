using System;

namespace Microsoft.Exchange.Server.Storage.PropTags
{
	public static class NumPropTag
	{
		public static uint GetTag(ushort propId, PropertyType propType)
		{
			return (uint)((ushort)((uint)propId << 16) + propType);
		}

		public const uint NULL = 4354U;

		public const uint AcknowledgementMode = 65539U;

		public const uint TestTest = 65794U;

		public const uint AlternateRecipientAllowed = 131083U;

		public const uint AuthorizingUsers = 196866U;

		public const uint AutoForwardComment = 262175U;

		public const uint AutoForwarded = 327691U;

		public const uint ContentConfidentialityAlgorithmId = 393474U;

		public const uint ContentCorrelator = 459010U;

		public const uint ContentIdentifier = 524319U;

		public const uint ContentLength = 589827U;

		public const uint ContentReturnRequested = 655371U;

		public const uint ConversationKey = 721154U;

		public const uint ConversionEits = 786690U;

		public const uint ConversionWithLossProhibited = 851979U;

		public const uint ConvertedEits = 917762U;

		public const uint DeferredDeliveryTime = 983104U;

		public const uint DeliverTime = 1048640U;

		public const uint DiscardReason = 1114115U;

		public const uint DisclosureOfRecipients = 1179659U;

		public const uint DLExpansionHistory = 1245442U;

		public const uint DLExpansionProhibited = 1310731U;

		public const uint ExpiryTime = 1376320U;

		public const uint ImplicitConversionProhibited = 1441803U;

		public const uint Importance = 1507331U;

		public const uint IPMID = 1573122U;

		public const uint LatestDeliveryTime = 1638464U;

		public const uint MessageClass = 1703967U;

		public const uint MessageDeliveryId = 1769730U;

		public const uint MessageSecurityLabel = 1966338U;

		public const uint ObsoletedIPMS = 2031874U;

		public const uint OriginallyIntendedRecipientName = 2097410U;

		public const uint OriginalEITS = 2162946U;

		public const uint OriginatorCertificate = 2228482U;

		public const uint DeliveryReportRequested = 2293771U;

		public const uint OriginatorReturnAddress = 2359554U;

		public const uint ParentKey = 2425090U;

		public const uint Priority = 2490371U;

		public const uint OriginCheck = 2556162U;

		public const uint ProofOfSubmissionRequested = 2621451U;

		public const uint ReadReceiptRequested = 2686987U;

		public const uint ReceiptTime = 2752576U;

		public const uint RecipientReassignmentProhibited = 2818059U;

		public const uint RedirectionHistory = 2883842U;

		public const uint RelatedIPMS = 2949378U;

		public const uint OriginalSensitivity = 3014659U;

		public const uint Languages = 3080223U;

		public const uint ReplyTime = 3145792U;

		public const uint ReportTag = 3211522U;

		public const uint ReportTime = 3276864U;

		public const uint ReturnedIPM = 3342347U;

		public const uint Security = 3407875U;

		public const uint IncompleteCopy = 3473419U;

		public const uint Sensitivity = 3538947U;

		public const uint Subject = 3604511U;

		public const uint SubjectIPM = 3670274U;

		public const uint ClientSubmitTime = 3735616U;

		public const uint ReportName = 3801119U;

		public const uint SentRepresentingSearchKey = 3866882U;

		public const uint X400ContentType = 3932418U;

		public const uint SubjectPrefix = 3997727U;

		public const uint NonReceiptReason = 4063235U;

		public const uint ReceivedByEntryId = 4129026U;

		public const uint ReceivedByName = 4194335U;

		public const uint SentRepresentingEntryId = 4260098U;

		public const uint SentRepresentingName = 4325407U;

		public const uint ReceivedRepresentingEntryId = 4391170U;

		public const uint ReceivedRepresentingName = 4456479U;

		public const uint ReportEntryId = 4522242U;

		public const uint ReadReceiptEntryId = 4587778U;

		public const uint MessageSubmissionId = 4653314U;

		public const uint ProviderSubmitTime = 4718656U;

		public const uint OriginalSubject = 4784159U;

		public const uint DiscVal = 4849675U;

		public const uint OriginalMessageClass = 4915231U;

		public const uint OriginalAuthorEntryId = 4980994U;

		public const uint OriginalAuthorName = 5046303U;

		public const uint OriginalSubmitTime = 5111872U;

		public const uint ReplyRecipientEntries = 5177602U;

		public const uint ReplyRecipientNames = 5242911U;

		public const uint ReceivedBySearchKey = 5308674U;

		public const uint ReceivedRepresentingSearchKey = 5374210U;

		public const uint ReadReceiptSearchKey = 5439746U;

		public const uint ReportSearchKey = 5505282U;

		public const uint OriginalDeliveryTime = 5570624U;

		public const uint OriginalAuthorSearchKey = 5636354U;

		public const uint MessageToMe = 5701643U;

		public const uint MessageCCMe = 5767179U;

		public const uint MessageRecipMe = 5832715U;

		public const uint OriginalSenderName = 5898271U;

		public const uint OriginalSenderEntryId = 5964034U;

		public const uint OriginalSenderSearchKey = 6029570U;

		public const uint OriginalSentRepresentingName = 6094879U;

		public const uint OriginalSentRepresentingEntryId = 6160642U;

		public const uint OriginalSentRepresentingSearchKey = 6226178U;

		public const uint StartDate = 6291520U;

		public const uint EndDate = 6357056U;

		public const uint OwnerApptId = 6422531U;

		public const uint ResponseRequested = 6488075U;

		public const uint SentRepresentingAddressType = 6553631U;

		public const uint SentRepresentingEmailAddress = 6619167U;

		public const uint OriginalSenderAddressType = 6684703U;

		public const uint OriginalSenderEmailAddress = 6750239U;

		public const uint OriginalSentRepresentingAddressType = 6815775U;

		public const uint OriginalSentRepresentingEmailAddress = 6881311U;

		public const uint ConversationTopic = 7340063U;

		public const uint ConversationIndex = 7405826U;

		public const uint OriginalDisplayBcc = 7471135U;

		public const uint OriginalDisplayCc = 7536671U;

		public const uint OriginalDisplayTo = 7602207U;

		public const uint ReceivedByAddressType = 7667743U;

		public const uint ReceivedByEmailAddress = 7733279U;

		public const uint ReceivedRepresentingAddressType = 7798815U;

		public const uint ReceivedRepresentingEmailAddress = 7864351U;

		public const uint OriginalAuthorAddressType = 7929887U;

		public const uint OriginalAuthorEmailAddress = 7995423U;

		public const uint OriginallyIntendedRecipientAddressType = 8126495U;

		public const uint TransportMessageHeaders = 8192031U;

		public const uint Delegation = 8257794U;

		public const uint TNEFCorrelationKey = 8323330U;

		public const uint ReportDisposition = 8388639U;

		public const uint ReportDispositionMode = 8454175U;

		public const uint ReportOriginalSender = 8519711U;

		public const uint ReportDispositionToNames = 8585247U;

		public const uint ReportDispositionToEmailAddress = 8650783U;

		public const uint ReportDispositionOptions = 8716319U;

		public const uint RichContent = 8781826U;

		public const uint AdministratorEMail = 16781343U;

		public const uint ContentIntegrityCheck = 201326850U;

		public const uint ExplicitConversion = 201392131U;

		public const uint ReturnRequested = 201457675U;

		public const uint MessageToken = 201523458U;

		public const uint NDRReasonCode = 201588739U;

		public const uint NDRDiagCode = 201654275U;

		public const uint NonReceiptNotificationRequested = 201719819U;

		public const uint DeliveryPoint = 201785347U;

		public const uint NonDeliveryReportRequested = 201850891U;

		public const uint OriginatorRequestedAlterateRecipient = 201916674U;

		public const uint PhysicalDeliveryBureauFaxDelivery = 201981963U;

		public const uint PhysicalDeliveryMode = 202047491U;

		public const uint PhysicalDeliveryReportRequest = 202113027U;

		public const uint PhysicalForwardingAddress = 202178818U;

		public const uint PhysicalForwardingAddressRequested = 202244107U;

		public const uint PhysicalForwardingProhibited = 202309643U;

		public const uint PhysicalRenditionAttributes = 202375426U;

		public const uint ProofOfDelivery = 202440962U;

		public const uint ProofOfDeliveryRequested = 202506251U;

		public const uint RecipientCertificate = 202572034U;

		public const uint RecipientNumberForAdvice = 202637343U;

		public const uint RecipientType = 202702851U;

		public const uint RegisteredMailType = 202768387U;

		public const uint ReplyRequested = 202833931U;

		public const uint RequestedDeliveryMethod = 202899459U;

		public const uint SenderEntryId = 202965250U;

		public const uint SenderName = 203030559U;

		public const uint SupplementaryInfo = 203096095U;

		public const uint TypeOfMTSUser = 203161603U;

		public const uint SenderSearchKey = 203227394U;

		public const uint SenderAddressType = 203292703U;

		public const uint SenderEmailAddress = 203358239U;

		public const uint ParticipantSID = 203686146U;

		public const uint ParticipantGuid = 203751682U;

		public const uint ToGroupExpansionRecipients = 203816991U;

		public const uint CcGroupExpansionRecipients = 203882527U;

		public const uint BccGroupExpansionRecipients = 203948063U;

		public const uint CurrentVersion = 234881044U;

		public const uint DeleteAfterSubmit = 234946571U;

		public const uint DisplayBcc = 235012127U;

		public const uint DisplayCc = 235077663U;

		public const uint DisplayTo = 235143199U;

		public const uint ParentDisplay = 235208735U;

		public const uint MessageDeliveryTime = 235274304U;

		public const uint MessageFlags = 235339779U;

		public const uint MessageSize = 235405332U;

		public const uint MessageSize32 = 235405315U;

		public const uint ParentEntryId = 235471106U;

		public const uint ParentEntryIdSvrEid = 235471099U;

		public const uint SentMailEntryId = 235536642U;

		public const uint Correlate = 235667467U;

		public const uint CorrelateMTSID = 235733250U;

		public const uint DiscreteValues = 235798539U;

		public const uint Responsibility = 235864075U;

		public const uint SpoolerStatus = 235929603U;

		public const uint TransportStatus = 235995139U;

		public const uint MessageRecipients = 236060685U;

		public const uint MessageRecipientsMVBin = 236065026U;

		public const uint MessageAttachments = 236126221U;

		public const uint ItemSubobjectsBin = 236126466U;

		public const uint SubmitFlags = 236191747U;

		public const uint RecipientStatus = 236257283U;

		public const uint TransportKey = 236322819U;

		public const uint MsgStatus = 236388355U;

		public const uint MessageDownloadTime = 236453891U;

		public const uint CreationVersion = 236519444U;

		public const uint ModifyVersion = 236584980U;

		public const uint HasAttach = 236650507U;

		public const uint BodyCRC = 236716035U;

		public const uint NormalizedSubject = 236781599U;

		public const uint RTFInSync = 236912651U;

		public const uint AttachSize = 236978179U;

		public const uint AttachSizeInt64 = 236978196U;

		public const uint AttachNum = 237043715U;

		public const uint Preprocess = 237109259U;

		public const uint FolderInternetId = 237174787U;

		public const uint HighestFolderInternetId = 237174787U;

		public const uint InternetArticleNumber = 237174787U;

		public const uint OriginatingMTACertificate = 237306114U;

		public const uint ProofOfSubmission = 237371650U;

		public const uint NTSecurityDescriptor = 237437186U;

		public const uint PrimarySendAccount = 237502495U;

		public const uint NextSendAccount = 237568031U;

		public const uint TodoItemFlags = 237699075U;

		public const uint SwappedTODOStore = 237764866U;

		public const uint SwappedTODOData = 237830402U;

		public const uint IMAPId = 237961219U;

		public const uint OriginalSourceServerVersion = 238092290U;

		public const uint ReplFlags = 238551043U;

		public const uint MessageDeepAttachments = 238682125U;

		public const uint AclTableAndSecurityDescriptor = 239010050U;

		public const uint SenderGuid = 239075586U;

		public const uint SentRepresentingGuid = 239141122U;

		public const uint OriginalSenderGuid = 239206658U;

		public const uint OriginalSentRepresentingGuid = 239272194U;

		public const uint ReadReceiptGuid = 239337730U;

		public const uint ReportGuid = 239403266U;

		public const uint OriginatorGuid = 239468802U;

		public const uint ReportDestinationGuid = 239534338U;

		public const uint OriginalAuthorGuid = 239599874U;

		public const uint ReceivedByGuid = 239665410U;

		public const uint ReceivedRepresentingGuid = 239730946U;

		public const uint CreatorGuid = 239796482U;

		public const uint LastModifierGuid = 239862018U;

		public const uint SenderSID = 239927554U;

		public const uint SentRepresentingSID = 239993090U;

		public const uint OriginalSenderSid = 240058626U;

		public const uint OriginalSentRepresentingSid = 240124162U;

		public const uint ReadReceiptSid = 240189698U;

		public const uint ReportSid = 240255234U;

		public const uint OriginatorSid = 240320770U;

		public const uint ReportDestinationSid = 240386306U;

		public const uint OriginalAuthorSid = 240451842U;

		public const uint RcvdBySid = 240517378U;

		public const uint RcvdRepresentingSid = 240582914U;

		public const uint CreatorSID = 240648450U;

		public const uint LastModifierSid = 240713986U;

		public const uint RecipientCAI = 240779522U;

		public const uint ConversationCreatorSID = 240845058U;

		public const uint Catalog = 240845058U;

		public const uint CISearchEnabled = 240910347U;

		public const uint CINotificationEnabled = 240975883U;

		public const uint MaxIndices = 241041411U;

		public const uint SourceFid = 241106964U;

		public const uint PFContactsGuid = 241172738U;

		public const uint URLCompNamePostfix = 241238019U;

		public const uint URLCompNameSet = 241303563U;

		public const uint SubfolderCount = 241369091U;

		public const uint DeletedSubfolderCt = 241434627U;

		public const uint MaxCachedViews = 241696771U;

		public const uint Read = 241762315U;

		public const uint NTSecurityDescriptorAsXML = 241827871U;

		public const uint AdminNTSecurityDescriptorAsXML = 241893407U;

		public const uint CreatorSidAsXML = 241958943U;

		public const uint LastModifierSidAsXML = 242024479U;

		public const uint SenderSIDAsXML = 242090015U;

		public const uint SentRepresentingSidAsXML = 242155551U;

		public const uint OriginalSenderSIDAsXML = 242221087U;

		public const uint OriginalSentRepresentingSIDAsXML = 242286623U;

		public const uint ReadReceiptSIDAsXML = 242352159U;

		public const uint ReportSIDAsXML = 242417695U;

		public const uint OriginatorSidAsXML = 242483231U;

		public const uint ReportDestinationSIDAsXML = 242548767U;

		public const uint OriginalAuthorSIDAsXML = 242614303U;

		public const uint ReceivedBySIDAsXML = 242679839U;

		public const uint ReceivedRepersentingSIDAsXML = 242745375U;

		public const uint TrustSender = 242810883U;

		public const uint MergeMidsetDeleted = 242876674U;

		public const uint ReserveRangeOfIDs = 242942210U;

		public const uint SenderSMTPAddress = 243859487U;

		public const uint SentRepresentingSMTPAddress = 243925023U;

		public const uint OriginalSenderSMTPAddress = 243990559U;

		public const uint OriginalSentRepresentingSMTPAddress = 244056095U;

		public const uint ReadReceiptSMTPAddress = 244121631U;

		public const uint ReportSMTPAddress = 244187167U;

		public const uint OriginatorSMTPAddress = 244252703U;

		public const uint ReportDestinationSMTPAddress = 244318239U;

		public const uint OriginalAuthorSMTPAddress = 244383775U;

		public const uint ReceivedBySMTPAddress = 244449311U;

		public const uint ReceivedRepresentingSMTPAddress = 244514847U;

		public const uint CreatorSMTPAddress = 244580383U;

		public const uint LastModifierSMTPAddress = 244645919U;

		public const uint VirusScannerStamp = 244711682U;

		public const uint VirusTransportStamp = 244715551U;

		public const uint AddrTo = 244776991U;

		public const uint AddrCc = 244842527U;

		public const uint ExtendedRuleActions = 244908290U;

		public const uint ExtendedRuleCondition = 244973826U;

		public const uint ExtendedRuleSizeLimit = 245039107U;

		public const uint EntourageSentHistory = 245305375U;

		public const uint ProofInProgress = 245497859U;

		public const uint SearchAttachmentsOLK = 245694495U;

		public const uint SearchRecipEmailTo = 245760031U;

		public const uint SearchRecipEmailCc = 245825567U;

		public const uint SearchRecipEmailBcc = 245891103U;

		public const uint SFGAOFlags = 246022147U;

		public const uint SearchFullTextSubject = 246153247U;

		public const uint SearchFullTextBody = 246218783U;

		public const uint FullTextConversationIndex = 246284319U;

		public const uint SearchAllIndexedProps = 246349855U;

		public const uint SearchRecipients = 246480927U;

		public const uint SearchRecipientsTo = 246546463U;

		public const uint SearchRecipientsCc = 246611999U;

		public const uint SearchRecipientsBcc = 246677535U;

		public const uint SearchAccountTo = 246743071U;

		public const uint SearchAccountCc = 246808607U;

		public const uint SearchAccountBcc = 246874143U;

		public const uint SearchEmailAddressTo = 246939679U;

		public const uint SearchEmailAddressCc = 247005215U;

		public const uint SearchEmailAddressBcc = 247070751U;

		public const uint SearchSmtpAddressTo = 247136287U;

		public const uint SearchSmtpAddressCc = 247201823U;

		public const uint SearchSmtpAddressBcc = 247267359U;

		public const uint SearchSender = 247332895U;

		public const uint IsIRMMessage = 248315915U;

		public const uint SearchIsPartiallyIndexed = 248381451U;

		public const uint FreeBusyNTSD = 251658498U;

		public const uint RenewTime = 251723840U;

		public const uint DeliveryOrRenewTime = 251789376U;

		public const uint ConversationFamilyId = 251855106U;

		public const uint LikeCount = 251920387U;

		public const uint RichContentDeprecated = 251985922U;

		public const uint PeopleCentricConversationId = 252051459U;

		public const uint DiscoveryAnnotation = 252575775U;

		public const uint Access = 267649027U;

		public const uint RowType = 267714563U;

		public const uint InstanceKey = 267780354U;

		public const uint InstanceKeySvrEid = 267780347U;

		public const uint AccessLevel = 267845635U;

		public const uint MappingSignature = 267911426U;

		public const uint RecordKey = 267976962U;

		public const uint RecordKeySvrEid = 267976955U;

		public const uint StoreRecordKey = 268042498U;

		public const uint StoreEntryId = 268108034U;

		public const uint MiniIcon = 268173570U;

		public const uint Icon = 268239106U;

		public const uint ObjectType = 268304387U;

		public const uint EntryId = 268370178U;

		public const uint EntryIdSvrEid = 268370171U;

		public const uint BodyUnicode = 268435487U;

		public const uint IsIntegJobMailboxGuid = 268435528U;

		public const uint ReportText = 268501023U;

		public const uint IsIntegJobGuid = 268501064U;

		public const uint OriginatorAndDLExpansionHistory = 268566786U;

		public const uint IsIntegJobFlags = 268566531U;

		public const uint ReportingDLName = 268632322U;

		public const uint IsIntegJobTask = 268632067U;

		public const uint ReportingMTACertificate = 268697858U;

		public const uint IsIntegJobState = 268697602U;

		public const uint IsIntegJobCreationTime = 268763200U;

		public const uint RtfSyncBodyCrc = 268828675U;

		public const uint IsIntegJobCompletedTime = 268828736U;

		public const uint RtfSyncBodyCount = 268894211U;

		public const uint IsIntegJobLastExecutionTime = 268894272U;

		public const uint RtfSyncBodyTag = 268959775U;

		public const uint IsIntegJobCorruptionsDetected = 268959747U;

		public const uint RtfCompressed = 269025538U;

		public const uint IsIntegJobCorruptionsFixed = 269025283U;

		public const uint AlternateBestBody = 269091074U;

		public const uint IsIntegJobRequestGuid = 269090888U;

		public const uint IsIntegJobProgress = 269156354U;

		public const uint IsIntegJobCorruptions = 269222146U;

		public const uint IsIntegJobSource = 269287426U;

		public const uint IsIntegJobPriority = 269352962U;

		public const uint IsIntegJobTimeInServer = 269418501U;

		public const uint RtfSyncPrefixCount = 269484035U;

		public const uint IsIntegJobMailboxNumber = 269484035U;

		public const uint RtfSyncTrailingCount = 269549571U;

		public const uint IsIntegJobError = 269549571U;

		public const uint OriginallyIntendedRecipientEntryId = 269615362U;

		public const uint BodyHtml = 269680898U;

		public const uint BodyHtmlUnicode = 269680671U;

		public const uint BodyContentLocation = 269746207U;

		public const uint BodyContentId = 269811743U;

		public const uint NativeBodyInfo = 269877251U;

		public const uint NativeBodyType = 269877250U;

		public const uint NativeBody = 269877506U;

		public const uint AnnotationToken = 269943042U;

		public const uint InternetApproved = 271581215U;

		public const uint InternetFollowupTo = 271777823U;

		public const uint InternetMessageId = 271908895U;

		public const uint InetNewsgroups = 271974431U;

		public const uint InternetReferences = 272171039U;

		public const uint PostReplyFolderEntries = 272433410U;

		public const uint NNTPXRef = 272629791U;

		public const uint InReplyToId = 272760863U;

		public const uint OriginalInternetMessageId = 273023007U;

		public const uint IconIndex = 276824067U;

		public const uint LastVerbExecuted = 276889603U;

		public const uint LastVerbExecutionTime = 276955200U;

		public const uint Relevance = 277086211U;

		public const uint FlagStatus = 277872643U;

		public const uint FlagCompleteTime = 277938240U;

		public const uint FormatPT = 278003715U;

		public const uint FollowupIcon = 278200323U;

		public const uint BlockStatus = 278265859U;

		public const uint ItemTempFlags = 278331395U;

		public const uint SMTPTempTblData = 281018626U;

		public const uint SMTPTempTblData2 = 281083907U;

		public const uint SMTPTempTblData3 = 281149698U;

		public const uint DAVSubmitData = 281411615U;

		public const uint ImapCachedMsgSize = 284164354U;

		public const uint DisableFullFidelity = 284295179U;

		public const uint URLCompName = 284360735U;

		public const uint AttrHidden = 284426251U;

		public const uint AttrSystem = 284491787U;

		public const uint AttrReadOnly = 284557323U;

		public const uint PredictedActions = 302256130U;

		public const uint GroupingActions = 302321666U;

		public const uint PredictedActionsSummary = 302383107U;

		public const uint PredictedActionsThresholds = 302448898U;

		public const uint IsClutter = 302448651U;

		public const uint InferencePredictedReplyForwardReasons = 302514434U;

		public const uint InferencePredictedDeleteReasons = 302579970U;

		public const uint InferencePredictedIgnoreReasons = 302645506U;

		public const uint OriginalDeliveryFolderInfo = 302711042U;

		public const uint RowId = 805306371U;

		public const uint UserInformationGuid = 805306440U;

		public const uint DisplayName = 805371935U;

		public const uint UserInformationDisplayName = 805371935U;

		public const uint AddressType = 805437471U;

		public const uint UserInformationCreationTime = 805437504U;

		public const uint EmailAddress = 805503007U;

		public const uint UserInformationLastModificationTime = 805503040U;

		public const uint Comment = 805568543U;

		public const uint UserInformationChangeNumber = 805568532U;

		public const uint Depth = 805634051U;

		public const uint UserInformationLastInteractiveLogonTime = 805634112U;

		public const uint ProviderDisplay = 805699615U;

		public const uint UserInformationActiveSyncAllowedDeviceIDs = 805703711U;

		public const uint CreationTime = 805765184U;

		public const uint UserInformationActiveSyncBlockedDeviceIDs = 805769247U;

		public const uint LastModificationTime = 805830720U;

		public const uint UserInformationActiveSyncDebugLogging = 805830659U;

		public const uint ResourceFlags = 805896195U;

		public const uint UserInformationActiveSyncEnabled = 805896203U;

		public const uint ProviderDllName = 805961759U;

		public const uint UserInformationAdminDisplayName = 805961759U;

		public const uint SearchKey = 806027522U;

		public const uint SearchKeySvrEid = 806027515U;

		public const uint UserInformationAggregationSubscriptionCredential = 806031391U;

		public const uint ProviderUID = 806093058U;

		public const uint UserInformationAllowArchiveAddressSync = 806092811U;

		public const uint ProviderOrdinal = 806158339U;

		public const uint UserInformationAltitude = 806158339U;

		public const uint UserInformationAntispamBypassEnabled = 806223883U;

		public const uint UserInformationArchiveDomain = 806289439U;

		public const uint TargetEntryId = 806355202U;

		public const uint UserInformationArchiveGuid = 806355016U;

		public const uint UserInformationArchiveName = 806424607U;

		public const uint UserInformationArchiveQuota = 806486047U;

		public const uint ConversationId = 806551810U;

		public const uint UserInformationArchiveRelease = 806551583U;

		public const uint BodyTag = 806617346U;

		public const uint UserInformationArchiveStatus = 806617091U;

		public const uint ConversationIndexTrackingObsolete = 806682644U;

		public const uint UserInformationArchiveWarningQuota = 806682655U;

		public const uint ConversationIndexTracking = 806748171U;

		public const uint UserInformationAssistantName = 806748191U;

		public const uint UserInformationBirthdate = 806813760U;

		public const uint ArchiveTag = 806879490U;

		public const uint UserInformationBypassNestedModerationEnabled = 806879243U;

		public const uint PolicyTag = 806945026U;

		public const uint UserInformationC = 806944799U;

		public const uint RetentionPeriod = 807010307U;

		public const uint UserInformationCalendarLoggingQuota = 807010335U;

		public const uint StartDateEtc = 807076098U;

		public const uint UserInformationCalendarRepairDisabled = 807075851U;

		public const uint RetentionDate = 807141440U;

		public const uint UserInformationCalendarVersionStoreDisabled = 807141387U;

		public const uint RetentionFlags = 807206915U;

		public const uint UserInformationCity = 807206943U;

		public const uint ArchivePeriod = 807272451U;

		public const uint UserInformationCountry = 807272479U;

		public const uint ArchiveDate = 807338048U;

		public const uint UserInformationCountryCode = 807337987U;

		public const uint UserInformationCountryOrRegion = 807403551U;

		public const uint UserInformationDefaultMailTip = 807469087U;

		public const uint UserInformationDeliverToMailboxAndForward = 807534603U;

		public const uint UserInformationDescription = 807604255U;

		public const uint UserInformationDisabledArchiveGuid = 807665736U;

		public const uint UserInformationDowngradeHighPriorityMessagesEnabled = 807731211U;

		public const uint UserInformationECPEnabled = 807796747U;

		public const uint UserInformationEmailAddressPolicyEnabled = 807862283U;

		public const uint UserInformationEwsAllowEntourage = 807927819U;

		public const uint UserInformationEwsAllowMacOutlook = 807993355U;

		public const uint UserInformationEwsAllowOutlook = 808058891U;

		public const uint UserInformationEwsApplicationAccessPolicy = 808124419U;

		public const uint UserInformationEwsEnabled = 808189955U;

		public const uint UserInformationEwsExceptions = 808259615U;

		public const uint UserInformationEwsWellKnownApplicationAccessPolicies = 808325151U;

		public const uint UserInformationExchangeGuid = 808386632U;

		public const uint UserInformationExternalOofOptions = 808452099U;

		public const uint UserInformationFirstName = 808517663U;

		public const uint UserInformationForwardingSmtpAddress = 808583199U;

		public const uint UserInformationGender = 808648735U;

		public const uint UserInformationGenericForwardingAddress = 808714271U;

		public const uint UserInformationGeoCoordinates = 808779807U;

		public const uint UserInformationHABSeniorityIndex = 808845315U;

		public const uint UserInformationHasActiveSyncDevicePartnership = 808910859U;

		public const uint UserInformationHiddenFromAddressListsEnabled = 808976395U;

		public const uint UserInformationHiddenFromAddressListsValue = 809041931U;

		public const uint UserInformationHomePhone = 809107487U;

		public const uint UserInformationImapEnabled = 809173003U;

		public const uint UserInformationImapEnableExactRFC822Size = 809238539U;

		public const uint UserInformationImapForceICalForCalendarRetrievalOption = 809304075U;

		public const uint UserInformationImapMessagesRetrievalMimeFormat = 809369603U;

		public const uint UserInformationImapProtocolLoggingEnabled = 809435139U;

		public const uint UserInformationImapSuppressReadReceipt = 809500683U;

		public const uint UserInformationImapUseProtocolDefaults = 809566219U;

		public const uint UserInformationIncludeInGarbageCollection = 809631755U;

		public const uint UserInformationInitials = 809697311U;

		public const uint UserInformationInPlaceHolds = 809766943U;

		public const uint UserInformationInternalOnly = 809828363U;

		public const uint UserInformationInternalUsageLocation = 809893919U;

		public const uint UserInformationInternetEncoding = 809959427U;

		public const uint UserInformationIsCalculatedTargetAddress = 810024971U;

		public const uint UserInformationIsExcludedFromServingHierarchy = 810090507U;

		public const uint UserInformationIsHierarchyReady = 810156043U;

		public const uint UserInformationIsInactiveMailbox = 810221579U;

		public const uint UserInformationIsSoftDeletedByDisable = 810287115U;

		public const uint UserInformationIsSoftDeletedByRemove = 810352651U;

		public const uint UserInformationIssueWarningQuota = 810418207U;

		public const uint UserInformationJournalArchiveAddress = 810483743U;

		public const uint UserInformationLanguages = 810553375U;

		public const uint UserInformationLastExchangeChangedTime = 810614848U;

		public const uint UserInformationLastName = 810680351U;

		public const uint UserInformationLatitude = 810745859U;

		public const uint UserInformationLEOEnabled = 810811403U;

		public const uint UserInformationLocaleID = 810881027U;

		public const uint UserInformationLongitude = 810942467U;

		public const uint UserInformationMacAttachmentFormat = 811008003U;

		public const uint UserInformationMailboxContainerGuid = 811073608U;

		public const uint UserInformationMailboxMoveBatchName = 811139103U;

		public const uint UserInformationMailboxMoveRemoteHostName = 811204639U;

		public const uint UserInformationMailboxMoveStatus = 811270147U;

		public const uint UserInformationMailboxRelease = 811335711U;

		public const uint UserInformationMailTipTranslations = 811405343U;

		public const uint UserInformationMAPIBlockOutlookNonCachedMode = 811466763U;

		public const uint UserInformationMAPIBlockOutlookRpcHttp = 811532299U;

		public const uint UserInformationMAPIBlockOutlookVersions = 811597855U;

		public const uint UserInformationMAPIEnabled = 811663371U;

		public const uint UserInformationMapiRecipient = 811728907U;

		public const uint UserInformationMaxBlockedSenders = 811794435U;

		public const uint UserInformationMaxReceiveSize = 811859999U;

		public const uint UserInformationMaxSafeSenders = 811925507U;

		public const uint UserInformationMaxSendSize = 811991071U;

		public const uint UserInformationMemberName = 812056607U;

		public const uint UserInformationMessageBodyFormat = 812122115U;

		public const uint UserInformationMessageFormat = 812187651U;

		public const uint UserInformationMessageTrackingReadStatusDisabled = 812253195U;

		public const uint UserInformationMobileFeaturesEnabled = 812318723U;

		public const uint UserInformationMobilePhone = 812384287U;

		public const uint UserInformationModerationFlags = 812449795U;

		public const uint UserInformationNotes = 812515359U;

		public const uint UserInformationOccupation = 812580895U;

		public const uint UserInformationOpenDomainRoutingDisabled = 812646411U;

		public const uint UserInformationOtherHomePhone = 812716063U;

		public const uint UserInformationOtherMobile = 812781599U;

		public const uint UserInformationOtherTelephone = 812847135U;

		public const uint UserInformationOWAEnabled = 812908555U;

		public const uint UserInformationOWAforDevicesEnabled = 812974091U;

		public const uint UserInformationPager = 813039647U;

		public const uint UserInformationPersistedCapabilities = 813109251U;

		public const uint UserInformationPhone = 813170719U;

		public const uint UserInformationPhoneProviderId = 813236255U;

		public const uint UserInformationPopEnabled = 813301771U;

		public const uint UserInformationPopEnableExactRFC822Size = 813367307U;

		public const uint UserInformationPopForceICalForCalendarRetrievalOption = 813432843U;

		public const uint UserInformationPopMessagesRetrievalMimeFormat = 813498371U;

		public const uint UserInformationPopProtocolLoggingEnabled = 813563907U;

		public const uint UserInformationPopSuppressReadReceipt = 813629451U;

		public const uint UserInformationPopUseProtocolDefaults = 813694987U;

		public const uint UserInformationPostalCode = 813760543U;

		public const uint UserInformationPostOfficeBox = 813830175U;

		public const uint UserInformationPreviousExchangeGuid = 813891656U;

		public const uint UserInformationPreviousRecipientTypeDetails = 813957123U;

		public const uint UserInformationProhibitSendQuota = 814022687U;

		public const uint UserInformationProhibitSendReceiveQuota = 814088223U;

		public const uint UserInformationQueryBaseDNRestrictionEnabled = 814153739U;

		public const uint UserInformationRecipientDisplayType = 814219267U;

		public const uint UserInformationRecipientLimits = 814284831U;

		public const uint UserInformationRecipientSoftDeletedStatus = 814350339U;

		public const uint UserInformationRecoverableItemsQuota = 814415903U;

		public const uint UserInformationRecoverableItemsWarningQuota = 814481439U;

		public const uint UserInformationRegion = 814546975U;

		public const uint UserInformationRemotePowerShellEnabled = 814612491U;

		public const uint UserInformationRemoteRecipientType = 814678019U;

		public const uint UserInformationRequireAllSendersAreAuthenticated = 814743563U;

		public const uint UserInformationResetPasswordOnNextLogon = 814809099U;

		public const uint UserInformationRetainDeletedItemsFor = 814874644U;

		public const uint UserInformationRetainDeletedItemsUntilBackup = 814940171U;

		public const uint UserInformationRulesQuota = 815005727U;

		public const uint UserInformationShouldUseDefaultRetentionPolicy = 815071243U;

		public const uint UserInformationSimpleDisplayName = 815136799U;

		public const uint UserInformationSingleItemRecoveryEnabled = 815202315U;

		public const uint UserInformationStateOrProvince = 815267871U;

		public const uint UserInformationStreetAddress = 815333407U;

		public const uint UserInformationSubscriberAccessEnabled = 815398923U;

		public const uint UserInformationTextEncodedORAddress = 815464479U;

		public const uint UserInformationTextMessagingState = 815534111U;

		public const uint UserInformationTimezone = 815595551U;

		public const uint UserInformationUCSImListMigrationCompleted = 815661067U;

		public const uint UserInformationUpgradeDetails = 815726623U;

		public const uint UserInformationUpgradeMessage = 815792159U;

		public const uint UserInformationUpgradeRequest = 815857667U;

		public const uint UserInformationUpgradeStage = 815923203U;

		public const uint UserInformationUpgradeStageTimeStamp = 815988800U;

		public const uint UserInformationUpgradeStatus = 816054275U;

		public const uint UserInformationUsageLocation = 816119839U;

		public const uint UserInformationUseMapiRichTextFormat = 816185347U;

		public const uint UserInformationUsePreferMessageFormat = 816250891U;

		public const uint UserInformationUseUCCAuditConfig = 816316427U;

		public const uint UserInformationWebPage = 816381983U;

		public const uint UserInformationWhenMailboxCreated = 816447552U;

		public const uint UserInformationWhenSoftDeleted = 816513088U;

		public const uint UserInformationBirthdayPrecision = 816578591U;

		public const uint UserInformationNameVersion = 816644127U;

		public const uint UserInformationOptInUser = 816709643U;

		public const uint UserInformationIsMigratedConsumerMailbox = 816775179U;

		public const uint UserInformationMigrationDryRun = 816840715U;

		public const uint UserInformationIsPremiumConsumerMailbox = 816906251U;

		public const uint UserInformationAlternateSupportEmailAddresses = 816971807U;

		public const uint UserInformationEmailAddresses = 817041439U;

		public const uint UserInformationMapiHttpEnabled = 819331083U;

		public const uint UserInformationMAPIBlockOutlookExternalConnectivity = 819396619U;

		public const uint FormVersion = 855703583U;

		public const uint FormCLSID = 855769160U;

		public const uint FormContactName = 855834655U;

		public const uint FormCategory = 855900191U;

		public const uint FormCategorySub = 855965727U;

		public const uint FormHidden = 856096779U;

		public const uint FormDesignerName = 856162335U;

		public const uint FormDesignerGuid = 856227912U;

		public const uint FormMessageBehavior = 856293379U;

		public const uint MessageTableTotalPages = 872480771U;

		public const uint MessageTableAvailablePages = 872546307U;

		public const uint OtherTablesTotalPages = 872611843U;

		public const uint OtherTablesAvailablePages = 872677379U;

		public const uint AttachmentTableTotalPages = 872742915U;

		public const uint AttachmentTableAvailablePages = 872808451U;

		public const uint MailboxTypeVersion = 872873987U;

		public const uint MailboxPartitionMailboxGuids = 872943688U;

		public const uint StoreSupportMask = 873267203U;

		public const uint StoreState = 873332739U;

		public const uint IPMSubtreeSearchKey = 873464066U;

		public const uint IPMOutboxSearchKey = 873529602U;

		public const uint IPMWastebasketSearchKey = 873595138U;

		public const uint IPMSentmailSearchKey = 873660674U;

		public const uint MdbProvider = 873726210U;

		public const uint ReceiveFolderSettings = 873791501U;

		public const uint LocalDirectoryEntryID = 873857282U;

		public const uint ProviderDisplayIcon = 873922591U;

		public const uint ProviderDisplayName = 873988127U;

		public const uint ControlDataForCalendarRepairAssistant = 874512642U;

		public const uint ControlDataForSharingPolicyAssistant = 874578178U;

		public const uint ControlDataForElcAssistant = 874643714U;

		public const uint ControlDataForTopNWordsAssistant = 874709250U;

		public const uint ControlDataForJunkEmailAssistant = 874774786U;

		public const uint ControlDataForCalendarSyncAssistant = 874840322U;

		public const uint ExternalSharingCalendarSubscriptionCount = 874905603U;

		public const uint ControlDataForUMReportingAssistant = 874971394U;

		public const uint HasUMReportData = 875036683U;

		public const uint InternetCalendarSubscriptionCount = 875102211U;

		public const uint ExternalSharingContactSubscriptionCount = 875167747U;

		public const uint JunkEmailSafeListDirty = 875233283U;

		public const uint IsTopNEnabled = 875298827U;

		public const uint LastSharingPolicyAppliedId = 875364610U;

		public const uint LastSharingPolicyAppliedHash = 875430146U;

		public const uint LastSharingPolicyAppliedTime = 875495488U;

		public const uint OofScheduleStart = 875561024U;

		public const uint OofScheduleEnd = 875626560U;

		public const uint ControlDataForDirectoryProcessorAssistant = 875692290U;

		public const uint NeedsDirectoryProcessor = 875757579U;

		public const uint RetentionQueryIds = 875827231U;

		public const uint RetentionQueryInfo = 875888660U;

		public const uint ControlDataForPublicFolderAssistant = 876019970U;

		public const uint ControlDataForInferenceTrainingAssistant = 876085506U;

		public const uint InferenceEnabled = 876150795U;

		public const uint ControlDataForContactLinkingAssistant = 876216578U;

		public const uint ContactLinking = 876281859U;

		public const uint ControlDataForOABGeneratorAssistant = 876347650U;

		public const uint ContactSaveVersion = 876412931U;

		public const uint ControlDataForOrgContactsSyncAssistant = 876478722U;

		public const uint OrgContactsSyncTimestamp = 876544064U;

		public const uint PushNotificationSubscriptionType = 876609794U;

		public const uint OrgContactsSyncADWatermark = 876675136U;

		public const uint ControlDataForInferenceDataCollectionAssistant = 876740866U;

		public const uint InferenceDataCollectionProcessingState = 876806402U;

		public const uint ControlDataForPeopleRelevanceAssistant = 876871938U;

		public const uint SiteMailboxInternalState = 876937219U;

		public const uint ControlDataForSiteMailboxAssistant = 877003010U;

		public const uint InferenceTrainingLastContentCount = 877068291U;

		public const uint InferenceTrainingLastAttemptTimestamp = 877133888U;

		public const uint InferenceTrainingLastSuccessTimestamp = 877199424U;

		public const uint InferenceUserCapabilityFlags = 877264899U;

		public const uint ControlDataForMailboxAssociationReplicationAssistant = 877330690U;

		public const uint MailboxAssociationNextReplicationTime = 877396032U;

		public const uint MailboxAssociationProcessingFlags = 877461507U;

		public const uint ControlDataForSharePointSignalStoreAssistant = 877527298U;

		public const uint ControlDataForPeopleCentricTriageAssistant = 877592834U;

		public const uint NotificationBrokerSubscriptions = 877658115U;

		public const uint GroupMailboxPermissionsVersion = 877723651U;

		public const uint ElcLastRunTotalProcessingTime = 877789204U;

		public const uint ElcLastRunSubAssistantProcessingTime = 877854740U;

		public const uint ElcLastRunUpdatedFolderCount = 877920276U;

		public const uint ElcLastRunTaggedFolderCount = 877985812U;

		public const uint ElcLastRunUpdatedItemCount = 878051348U;

		public const uint ElcLastRunTaggedWithArchiveItemCount = 878116884U;

		public const uint ElcLastRunTaggedWithExpiryItemCount = 878182420U;

		public const uint ElcLastRunDeletedFromRootItemCount = 878247956U;

		public const uint ElcLastRunDeletedFromDumpsterItemCount = 878313492U;

		public const uint ElcLastRunArchivedFromRootItemCount = 878379028U;

		public const uint ElcLastRunArchivedFromDumpsterItemCount = 878444564U;

		public const uint ScheduledISIntegLastFinished = 878510144U;

		public const uint ControlDataForSearchIndexRepairAssistant = 878575874U;

		public const uint ELCLastSuccessTimestamp = 878641216U;

		public const uint EventEmailReminderTimer = 878706752U;

		public const uint InferenceTruthLoggingLastAttemptTimestamp = 878772288U;

		public const uint InferenceTruthLoggingLastSuccessTimestamp = 878837824U;

		public const uint ControlDataForGroupMailboxAssistant = 878903554U;

		public const uint ItemsPendingUpgrade = 878968835U;

		public const uint ConsumerSharingCalendarSubscriptionCount = 879034371U;

		public const uint GroupMailboxGeneratedPhotoVersion = 879099907U;

		public const uint GroupMailboxGeneratedPhotoSignature = 879165698U;

		public const uint GroupMailboxExchangeResourcesPublishedVersion = 879230979U;

		public const uint ValidFolderMask = 903806979U;

		public const uint IPMSubtreeEntryId = 903872770U;

		public const uint IPMOutboxEntryId = 904003842U;

		public const uint IPMWastebasketEntryId = 904069378U;

		public const uint IPMSentmailEntryId = 904134914U;

		public const uint IPMViewsEntryId = 904200450U;

		public const uint IPMCommonViewsEntryId = 904265986U;

		public const uint IPMConversationsEntryId = 904659202U;

		public const uint IPMAllItemsEntryId = 904790274U;

		public const uint IPMSharingEntryId = 904855810U;

		public const uint AdminDataEntryId = 905773314U;

		public const uint UnsearchableItems = 905838850U;

		public const uint ContainerFlags = 905969667U;

		public const uint IPMFinderEntryId = 905969922U;

		public const uint FolderType = 906035203U;

		public const uint ContentCount = 906100739U;

		public const uint ContentCountInt64 = 906100756U;

		public const uint UnreadCount = 906166275U;

		public const uint UnreadCountInt64 = 906166292U;

		public const uint DetailsTable = 906297357U;

		public const uint Search = 906428429U;

		public const uint Selectable = 906559499U;

		public const uint Subfolders = 906625035U;

		public const uint FolderStatus = 906690563U;

		public const uint AmbiguousNameResolution = 906756127U;

		public const uint ContentsSortOrder = 906825731U;

		public const uint ContainerHierarchy = 906887181U;

		public const uint ContainerContents = 906952717U;

		public const uint FolderAssociatedContents = 907018253U;

		public const uint ContainerClass = 907214879U;

		public const uint ContainerModifyVersion = 907280404U;

		public const uint ABProviderId = 907346178U;

		public const uint DefaultViewEntryId = 907411714U;

		public const uint AssociatedContentCount = 907476995U;

		public const uint AssociatedContentCountInt64 = 907477012U;

		public const uint PackedNamedProps = 907804930U;

		public const uint AllowAgeOut = 908001291U;

		public const uint SearchFolderMsgCount = 910426115U;

		public const uint PartOfContentIndexing = 910491659U;

		public const uint OwnerLogonUserConfigurationCache = 910557442U;

		public const uint SearchFolderAgeOutTimeout = 910622723U;

		public const uint SearchFolderPopulationResult = 910688259U;

		public const uint SearchFolderPopulationDiagnostics = 910754050U;

		public const uint ConversationTopicHashEntries = 912261378U;

		public const uint ContentAggregationFlags = 915341315U;

		public const uint TransportRulesSnapshot = 915407106U;

		public const uint TransportRulesSnapshotId = 915472456U;

		public const uint CurrentIPMWasteBasketContainerEntryId = 919535874U;

		public const uint IPMAppointmentEntryId = 919601410U;

		public const uint IPMContactEntryId = 919666946U;

		public const uint IPMJournalEntryId = 919732482U;

		public const uint IPMNoteEntryId = 919798018U;

		public const uint IPMTaskEntryId = 919863554U;

		public const uint REMOnlineEntryId = 919929090U;

		public const uint IPMOfflineEntryId = 919994626U;

		public const uint IPMDraftsEntryId = 920060162U;

		public const uint AdditionalRENEntryIds = 920129794U;

		public const uint AdditionalRENEntryIdsExtended = 920191234U;

		public const uint AdditionalRENEntryIdsExtendedMV = 920195330U;

		public const uint ExtendedFolderFlags = 920256770U;

		public const uint ContainerTimestamp = 920322112U;

		public const uint AppointmentColorName = 920387842U;

		public const uint INetUnread = 920453123U;

		public const uint NetFolderFlags = 920518659U;

		public const uint FolderWebViewInfo = 920584450U;

		public const uint FolderWebViewInfoExtended = 920649986U;

		public const uint FolderViewFlags = 920715267U;

		public const uint FreeBusyEntryIds = 920916226U;

		public const uint DefaultPostMsgClass = 920977439U;

		public const uint DefaultPostDisplayName = 921042975U;

		public const uint FolderViewList = 921370882U;

		public const uint AgingPeriod = 921436163U;

		public const uint AgingGranularity = 921567235U;

		public const uint DefaultFoldersLocaleId = 921698307U;

		public const uint InternalAccess = 921763851U;

		public const uint AttachmentX400Parameters = 922747138U;

		public const uint Content = 922812674U;

		public const uint ContentObj = 922812429U;

		public const uint AttachmentEncoding = 922878210U;

		public const uint ContentId = 922943519U;

		public const uint ContentType = 923009055U;

		public const uint AttachMethod = 923074563U;

		public const uint MimeUrl = 923205663U;

		public const uint AttachmentPathName = 923271199U;

		public const uint AttachRendering = 923336962U;

		public const uint AttachTag = 923402498U;

		public const uint RenderingPosition = 923467779U;

		public const uint AttachTransportName = 923533343U;

		public const uint AttachmentLongPathName = 923598879U;

		public const uint AttachmentMimeTag = 923664415U;

		public const uint AttachAdditionalInfo = 923730178U;

		public const uint AttachmentMimeSequence = 923795459U;

		public const uint AttachContentBase = 923861023U;

		public const uint AttachContentId = 923926559U;

		public const uint AttachContentLocation = 923992095U;

		public const uint AttachmentFlags = 924057603U;

		public const uint AttachDisposition = 924188703U;

		public const uint AttachPayloadProviderGuidString = 924385311U;

		public const uint AttachPayloadClass = 924450847U;

		public const uint TextAttachmentCharset = 924516383U;

		public const uint SyncEventSuppressGuid = 947912962U;

		public const uint DisplayType = 956301315U;

		public const uint TemplateId = 956432642U;

		public const uint CapabilitiesTable = 956498178U;

		public const uint PrimaryCapability = 956563714U;

		public const uint EMSABDisplayTypeEx = 956628995U;

		public const uint SmtpAddress = 972947487U;

		public const uint EMSABDisplayNamePrintable = 973013023U;

		public const uint SimpleDisplayName = 973013023U;

		public const uint Account = 973078559U;

		public const uint AlternateRecipient = 973144322U;

		public const uint CallbackTelephoneNumber = 973209631U;

		public const uint ConversionProhibited = 973275147U;

		public const uint Generation = 973406239U;

		public const uint GivenName = 973471775U;

		public const uint GovernmentIDNumber = 973537311U;

		public const uint BusinessTelephoneNumber = 973602847U;

		public const uint HomeTelephoneNumber = 973668383U;

		public const uint Initials = 973733919U;

		public const uint Keyword = 973799455U;

		public const uint Language = 973864991U;

		public const uint Location = 973930527U;

		public const uint MailPermission = 973996043U;

		public const uint MHSCommonName = 974061599U;

		public const uint OrganizationalIDNumber = 974127135U;

		public const uint SurName = 974192671U;

		public const uint OriginalEntryId = 974258434U;

		public const uint OriginalDisplayName = 974323743U;

		public const uint OriginalSearchKey = 974389506U;

		public const uint PostalAddress = 974454815U;

		public const uint CompanyName = 974520351U;

		public const uint Title = 974585887U;

		public const uint DepartmentName = 974651423U;

		public const uint OfficeLocation = 974716959U;

		public const uint PrimaryTelephoneNumber = 974782495U;

		public const uint Business2TelephoneNumber = 974848031U;

		public const uint Business2TelephoneNumberMv = 974852127U;

		public const uint MobileTelephoneNumber = 974913567U;

		public const uint RadioTelephoneNumber = 974979103U;

		public const uint CarTelephoneNumber = 975044639U;

		public const uint OtherTelephoneNumber = 975110175U;

		public const uint TransmitableDisplayName = 975175711U;

		public const uint PagerTelephoneNumber = 975241247U;

		public const uint UserCertificate = 975307010U;

		public const uint PrimaryFaxNumber = 975372319U;

		public const uint BusinessFaxNumber = 975437855U;

		public const uint HomeFaxNumber = 975503391U;

		public const uint Country = 975568927U;

		public const uint Locality = 975634463U;

		public const uint StateOrProvince = 975699999U;

		public const uint StreetAddress = 975765535U;

		public const uint PostalCode = 975831071U;

		public const uint PostOfficeBox = 975896607U;

		public const uint TelexNumber = 975962143U;

		public const uint ISDNNumber = 976027679U;

		public const uint AssistantTelephoneNumber = 976093215U;

		public const uint Home2TelephoneNumber = 976158751U;

		public const uint Home2TelephoneNumberMv = 976162847U;

		public const uint Assistant = 976224287U;

		public const uint SendRichInfo = 977272843U;

		public const uint WeddingAnniversary = 977338432U;

		public const uint Birthday = 977403968U;

		public const uint Hobbies = 977469471U;

		public const uint MiddleName = 977535007U;

		public const uint DisplayNamePrefix = 977600543U;

		public const uint Profession = 977666079U;

		public const uint ReferredByName = 977731615U;

		public const uint SpouseName = 977797151U;

		public const uint ComputerNetworkName = 977862687U;

		public const uint CustomerId = 977928223U;

		public const uint TTYTDDPhoneNumber = 977993759U;

		public const uint FTPSite = 978059295U;

		public const uint Gender = 978124802U;

		public const uint ManagerName = 978190367U;

		public const uint NickName = 978255903U;

		public const uint PersonalHomePage = 978321439U;

		public const uint BusinessHomePage = 978386975U;

		public const uint ContactVersion = 978452552U;

		public const uint ContactEntryIds = 978522370U;

		public const uint ContactAddressTypes = 978587679U;

		public const uint ContactDefaultAddressIndex = 978649091U;

		public const uint ContactEmailAddress = 978718751U;

		public const uint CompanyMainPhoneNumber = 978780191U;

		public const uint ChildrensNames = 978849823U;

		public const uint HomeAddressCity = 978911263U;

		public const uint HomeAddressCountry = 978976799U;

		public const uint HomeAddressPostalCode = 979042335U;

		public const uint HomeAddressStateOrProvince = 979107871U;

		public const uint HomeAddressStreet = 979173407U;

		public const uint HomeAddressPostOfficeBox = 979238943U;

		public const uint OtherAddressCity = 979304479U;

		public const uint OtherAddressCountry = 979370015U;

		public const uint OtherAddressPostalCode = 979435551U;

		public const uint OtherAddressStateOrProvince = 979501087U;

		public const uint OtherAddressStreet = 979566623U;

		public const uint OtherAddressPostOfficeBox = 979632159U;

		public const uint UserX509CertificateABSearchPath = 980422914U;

		public const uint SendInternetEncoding = 980484099U;

		public const uint PartnerNetworkId = 980811807U;

		public const uint PartnerNetworkUserId = 980877343U;

		public const uint PartnerNetworkThumbnailPhotoUrl = 980942879U;

		public const uint PartnerNetworkProfilePhotoUrl = 981008415U;

		public const uint PartnerNetworkContactType = 981073951U;

		public const uint RelevanceScore = 981139459U;

		public const uint IsDistributionListContact = 981205003U;

		public const uint IsPromotedContact = 981270539U;

		public const uint OrgUnitName = 1006501919U;

		public const uint OrganizationName = 1006567455U;

		public const uint TestBlobProperty = 1023410196U;

		public const uint StoreProviders = 1023410434U;

		public const uint AddressBookProviders = 1023475970U;

		public const uint TransportProviders = 1023541506U;

		public const uint FilteringHooks = 1023934722U;

		public const uint ServiceName = 1024000031U;

		public const uint ServiceDLLName = 1024065567U;

		public const uint ServiceEntryName = 1024131103U;

		public const uint ServiceUid = 1024196866U;

		public const uint ServiceExtraUid = 1024262402U;

		public const uint Services = 1024327938U;

		public const uint ServiceSupportFiles = 1024397343U;

		public const uint ServiceDeleteFiles = 1024462879U;

		public const uint ProfileName = 1024589855U;

		public const uint AdminSecurityDescriptor = 1025573122U;

		public const uint Win32NTSecurityDescriptor = 1025638658U;

		public const uint NonWin32ACL = 1025703947U;

		public const uint ItemLevelACL = 1025769483U;

		public const uint ICSGid = 1026425090U;

		public const uint SystemFolderFlags = 1026490371U;

		public const uint MaterializedRestrictionSearchRoot = 1033634050U;

		public const uint ScheduledISIntegCorruptionCount = 1033699331U;

		public const uint ScheduledISIntegExecutionTime = 1033764867U;

		public const uint MailboxPartitionNumber = 1033830403U;

		public const uint MailboxNumberInternal = 1033895939U;

		public const uint QueryCriteriaInternal = 1033961730U;

		public const uint LastQuotaNotificationTime = 1034027072U;

		public const uint PropertyPromotionInProgressHiddenItems = 1034092555U;

		public const uint PropertyPromotionInProgressNormalItems = 1034158091U;

		public const uint VirtualParentDisplay = 1034223647U;

		public const uint MailboxTypeDetail = 1034289155U;

		public const uint InternalTenantHint = 1034354946U;

		public const uint InternalConversationIndexTracking = 1034420235U;

		public const uint InternalConversationIndex = 1034486018U;

		public const uint ConversationItemConversationId = 1034551554U;

		public const uint VirtualUnreadMessageCount = 1034616852U;

		public const uint VirtualIsRead = 1034682379U;

		public const uint IsReadColumn = 1034747915U;

		public const uint TenantHint = 1034813698U;

		public const uint Internal9ByteChangeNumber = 1034879234U;

		public const uint Internal9ByteReadCnNew = 1034944770U;

		public const uint CategoryHeaderLevelStub1 = 1035010059U;

		public const uint CategoryHeaderLevelStub2 = 1035075595U;

		public const uint CategoryHeaderLevelStub3 = 1035141131U;

		public const uint CategoryHeaderAggregateProp0 = 1035206914U;

		public const uint CategoryHeaderAggregateProp1 = 1035272450U;

		public const uint CategoryHeaderAggregateProp2 = 1035337986U;

		public const uint CategoryHeaderAggregateProp3 = 1035403522U;

		public const uint MaintenanceId = 1035665480U;

		public const uint MailboxType = 1035730947U;

		public const uint MessageFlagsActual = 1035796483U;

		public const uint InternalChangeKey = 1035862274U;

		public const uint InternalSourceKey = 1035927810U;

		public const uint CorrelationId = 1037107272U;

		public const uint IdentityDisplay = 1040187423U;

		public const uint IdentityEntryId = 1040253186U;

		public const uint ResourceMethods = 1040318467U;

		public const uint ResourceType = 1040384003U;

		public const uint StatusCode = 1040449539U;

		public const uint IdentitySearchKey = 1040515330U;

		public const uint OwnStoreEntryId = 1040580866U;

		public const uint ResourcePath = 1040646175U;

		public const uint StatusString = 1040711711U;

		public const uint X400DeferredDeliveryCancel = 1040777227U;

		public const uint HeaderFolderEntryId = 1040843010U;

		public const uint RemoteProgress = 1040908291U;

		public const uint RemoteProgressText = 1040973855U;

		public const uint RemoteValidateOK = 1041039371U;

		public const uint ControlFlags = 1056964611U;

		public const uint ControlStructure = 1057030402U;

		public const uint ControlType = 1057095683U;

		public const uint DeltaX = 1057161219U;

		public const uint DeltaY = 1057226755U;

		public const uint XPos = 1057292291U;

		public const uint YPos = 1057357827U;

		public const uint ControlId = 1057423618U;

		public const uint InitialDetailsPane = 1057488899U;

		public const uint AttachmentId = 1065877524U;

		public const uint AttachmentIdBin = 1065877762U;

		public const uint VID = 1065877524U;

		public const uint GVid = 1065943298U;

		public const uint GDID = 1066008834U;

		public const uint XVid = 1066729730U;

		public const uint GDefVid = 1066795266U;

		public const uint PrimaryMailboxOverQuota = 1069678603U;

		public const uint ReplicaChangeNumber = 1070072066U;

		public const uint LastConflict = 1070137602U;

		public const uint RMI = 1070858498U;

		public const uint InternalPostReply = 1070924034U;

		public const uint NTSDModificationTime = 1070989376U;

		public const uint ACLDataChecksum = 1071054851U;

		public const uint PreviewUnread = 1071120415U;

		public const uint Preview = 1071185951U;

		public const uint InternetCPID = 1071513603U;

		public const uint AutoResponseSuppress = 1071579139U;

		public const uint ACLData = 1071644930U;

		public const uint ACLTable = 1071644685U;

		public const uint RulesData = 1071710466U;

		public const uint RulesTable = 1071710221U;

		public const uint OofHistory = 1071841538U;

		public const uint DesignInProgress = 1071906827U;

		public const uint SecureOrigination = 1071972363U;

		public const uint PublishInAddressBook = 1072037899U;

		public const uint ResolveMethod = 1072103427U;

		public const uint AddressBookDisplayName = 1072168991U;

		public const uint EFormsLocaleId = 1072234499U;

		public const uint HasDAMs = 1072300043U;

		public const uint DeferredSendNumber = 1072365571U;

		public const uint DeferredSendUnits = 1072431107U;

		public const uint ExpiryNumber = 1072496643U;

		public const uint ExpiryUnits = 1072562179U;

		public const uint DeferredSendTime = 1072627776U;

		public const uint BackfillTimeout = 1072693506U;

		public const uint MessageLocaleId = 1072758787U;

		public const uint RuleTriggerHistory = 1072824578U;

		public const uint MoveToStoreEid = 1072890114U;

		public const uint MoveToFolderEid = 1072955650U;

		public const uint StorageQuotaLimit = 1073020931U;

		public const uint ExcessStorageUsed = 1073086467U;

		public const uint ServerGeneratingQuotaMsg = 1073152031U;

		public const uint CreatorName = 1073217567U;

		public const uint CreatorEntryId = 1073283330U;

		public const uint LastModifierName = 1073348639U;

		public const uint LastModifierEntryId = 1073414402U;

		public const uint MessageCodePage = 1073545219U;

		public const uint QuotaType = 1073610755U;

		public const uint ExtendedACLData = 1073611010U;

		public const uint RulesSize = 1073676291U;

		public const uint IsPublicFolderQuotaMessage = 1073676299U;

		public const uint NewAttach = 1073741827U;

		public const uint StartEmbed = 1073807363U;

		public const uint EndEmbed = 1073872899U;

		public const uint StartRecip = 1073938435U;

		public const uint EndRecip = 1074003971U;

		public const uint EndCcRecip = 1074069507U;

		public const uint EndBccRecip = 1074135043U;

		public const uint EndP1Recip = 1074200579U;

		public const uint DNPrefix = 1074266143U;

		public const uint StartTopFolder = 1074331651U;

		public const uint StartSubFolder = 1074397187U;

		public const uint EndFolder = 1074462723U;

		public const uint StartMessage = 1074528259U;

		public const uint EndMessage = 1074593795U;

		public const uint EndAttach = 1074659331U;

		public const uint EcWarning = 1074724867U;

		public const uint StartFAIMessage = 1074790403U;

		public const uint NewFXFolder = 1074856194U;

		public const uint IncrSyncChange = 1074921475U;

		public const uint IncrSyncDelete = 1074987011U;

		public const uint IncrSyncEnd = 1075052547U;

		public const uint IncrSyncMessage = 1075118083U;

		public const uint FastTransferDelProp = 1075183619U;

		public const uint IdsetGiven = 1075249410U;

		public const uint IdsetGivenInt32 = 1075249155U;

		public const uint FastTransferErrorInfo = 1075314691U;

		public const uint SenderFlags = 1075380227U;

		public const uint SentRepresentingFlags = 1075445763U;

		public const uint RcvdByFlags = 1075511299U;

		public const uint RcvdRepresentingFlags = 1075576835U;

		public const uint OriginalSenderFlags = 1075642371U;

		public const uint OriginalSentRepresentingFlags = 1075707907U;

		public const uint ReportFlags = 1075773443U;

		public const uint ReadReceiptFlags = 1075838979U;

		public const uint SoftDeletes = 1075904770U;

		public const uint CreatorAddressType = 1075970079U;

		public const uint CreatorEmailAddr = 1076035615U;

		public const uint LastModifierAddressType = 1076101151U;

		public const uint LastModifierEmailAddr = 1076166687U;

		public const uint ReportAddressType = 1076232223U;

		public const uint ReportEmailAddress = 1076297759U;

		public const uint ReportDisplayName = 1076363295U;

		public const uint ReadReceiptAddressType = 1076428831U;

		public const uint ReadReceiptEmailAddress = 1076494367U;

		public const uint ReadReceiptDisplayName = 1076559903U;

		public const uint IdsetRead = 1076691202U;

		public const uint IdsetUnread = 1076756738U;

		public const uint IncrSyncRead = 1076822019U;

		public const uint SenderSimpleDisplayName = 1076887583U;

		public const uint SentRepresentingSimpleDisplayName = 1076953119U;

		public const uint OriginalSenderSimpleDisplayName = 1077018655U;

		public const uint OriginalSentRepresentingSimpleDisplayName = 1077084191U;

		public const uint ReceivedBySimpleDisplayName = 1077149727U;

		public const uint ReceivedRepresentingSimpleDisplayName = 1077215263U;

		public const uint ReadReceiptSimpleDisplayName = 1077280799U;

		public const uint ReportSimpleDisplayName = 1077346335U;

		public const uint CreatorSimpleDisplayName = 1077411871U;

		public const uint LastModifierSimpleDisplayName = 1077477407U;

		public const uint IncrSyncStateBegin = 1077542915U;

		public const uint IncrSyncStateEnd = 1077608451U;

		public const uint IncrSyncImailStream = 1077673987U;

		public const uint SenderOrgAddressType = 1077870623U;

		public const uint SenderOrgEmailAddr = 1077936159U;

		public const uint SentRepresentingOrgAddressType = 1078001695U;

		public const uint SentRepresentingOrgEmailAddr = 1078067231U;

		public const uint OriginalSenderOrgAddressType = 1078132767U;

		public const uint OriginalSenderOrgEmailAddr = 1078198303U;

		public const uint OriginalSentRepresentingOrgAddressType = 1078263839U;

		public const uint OriginalSentRepresentingOrgEmailAddr = 1078329375U;

		public const uint RcvdByOrgAddressType = 1078394911U;

		public const uint RcvdByOrgEmailAddr = 1078460447U;

		public const uint RcvdRepresentingOrgAddressType = 1078525983U;

		public const uint RcvdRepresentingOrgEmailAddr = 1078591519U;

		public const uint ReadReceiptOrgAddressType = 1078657055U;

		public const uint ReadReceiptOrgEmailAddr = 1078722591U;

		public const uint ReportOrgAddressType = 1078788127U;

		public const uint ReportOrgEmailAddr = 1078853663U;

		public const uint CreatorOrgAddressType = 1078919199U;

		public const uint CreatorOrgEmailAddr = 1078984735U;

		public const uint LastModifierOrgAddressType = 1079050271U;

		public const uint LastModifierOrgEmailAddr = 1079115807U;

		public const uint OriginatorOrgAddressType = 1079181343U;

		public const uint OriginatorOrgEmailAddr = 1079246879U;

		public const uint ReportDestinationOrgEmailType = 1079312415U;

		public const uint ReportDestinationOrgEmailAddr = 1079377951U;

		public const uint OriginalAuthorOrgAddressType = 1079443487U;

		public const uint OriginalAuthorOrgEmailAddr = 1079509023U;

		public const uint CreatorFlags = 1079574531U;

		public const uint LastModifierFlags = 1079640067U;

		public const uint OriginatorFlags = 1079705603U;

		public const uint ReportDestinationFlags = 1079771139U;

		public const uint OriginalAuthorFlags = 1079836675U;

		public const uint OriginatorSimpleDisplayName = 1079902239U;

		public const uint ReportDestinationSimpleDisplayName = 1079967775U;

		public const uint OriginalAuthorSimpleDispName = 1080033311U;

		public const uint OriginatorSearchKey = 1080099074U;

		public const uint ReportDestinationAddressType = 1080164383U;

		public const uint ReportDestinationEmailAddress = 1080229919U;

		public const uint ReportDestinationSearchKey = 1080295682U;

		public const uint IncrSyncImailStreamContinue = 1080426499U;

		public const uint IncrSyncImailStreamCancel = 1080492035U;

		public const uint IncrSyncImailStream2Continue = 1081147395U;

		public const uint IncrSyncProgressMode = 1081344011U;

		public const uint SyncProgressPerMsg = 1081409547U;

		public const uint ContentFilterSCL = 1081475075U;

		public const uint IncrSyncMsgPartial = 1081737219U;

		public const uint IncrSyncGroupInfo = 1081802755U;

		public const uint IncrSyncGroupId = 1081868291U;

		public const uint IncrSyncChangePartial = 1081933827U;

		public const uint HierRev = 1082261568U;

		public const uint ContentFilterPCL = 1082392579U;

		public const uint DeliverAsRead = 1476788235U;

		public const uint InetMailOverrideFormat = 1493303299U;

		public const uint MessageEditorFormat = 1493762051U;

		public const uint SenderSMTPAddressXSO = 1560346655U;

		public const uint SentRepresentingSMTPAddressXSO = 1560412191U;

		public const uint OriginalSenderSMTPAddressXSO = 1560477727U;

		public const uint OriginalSentRepresentingSMTPAddressXSO = 1560543263U;

		public const uint ReadReceiptSMTPAddressXSO = 1560608799U;

		public const uint OriginalAuthorSMTPAddressXSO = 1560674335U;

		public const uint ReceivedBySMTPAddressXSO = 1560739871U;

		public const uint ReceivedRepresentingSMTPAddressXSO = 1560805407U;

		public const uint RecipientOrder = 1608450051U;

		public const uint RecipientSipUri = 1608843295U;

		public const uint RecipientDisplayName = 1609957407U;

		public const uint RecipientEntryId = 1610023170U;

		public const uint RecipientFlags = 1610416131U;

		public const uint RecipientTrackStatus = 1610547203U;

		public const uint DotStuffState = 1610678303U;

		public const uint InternetMessageIdHash = 1644167171U;

		public const uint ConversationTopicHash = 1644232707U;

		public const uint MimeSkeleton = 1693450498U;

		public const uint ReplyTemplateId = 1707213058U;

		public const uint SecureSubmitFlags = 1707474947U;

		public const uint SourceKey = 1709179138U;

		public const uint ParentSourceKey = 1709244674U;

		public const uint ChangeKey = 1709310210U;

		public const uint PredecessorChangeList = 1709375746U;

		public const uint RuleMsgState = 1709768707U;

		public const uint RuleMsgUserFlags = 1709834243U;

		public const uint RuleMsgProvider = 1709899807U;

		public const uint RuleMsgName = 1709965343U;

		public const uint RuleMsgLevel = 1710030851U;

		public const uint RuleMsgProviderData = 1710096642U;

		public const uint RuleMsgActions = 1710162178U;

		public const uint RuleMsgCondition = 1710227714U;

		public const uint RuleMsgConditionLCID = 1710292995U;

		public const uint RuleMsgVersion = 1710358530U;

		public const uint RuleMsgSequence = 1710424067U;

		public const uint PreventMsgCreate = 1710489611U;

		public const uint IMAPSubscribeList = 1710624799U;

		public const uint LISSD = 1710817538U;

		public const uint ProfileVersion = 1711276035U;

		public const uint ProfileConfigFlags = 1711341571U;

		public const uint ProfileHomeServer = 1711407135U;

		public const uint ProfileUser = 1711472671U;

		public const uint ProfileConnectFlags = 1711538179U;

		public const uint ProfileTransportFlags = 1711603715U;

		public const uint ProfileUIState = 1711669251U;

		public const uint ProfileUnresolvedName = 1711734815U;

		public const uint ProfileUnresolvedServer = 1711800351U;

		public const uint ProfileBindingOrder = 1711865887U;

		public const uint ProfileMaxRestrict = 1712128003U;

		public const uint ProfileABFilesPath = 1712193567U;

		public const uint ProfileFavFolderDisplayName = 1712259103U;

		public const uint ProfileOfflineStorePath = 1712324639U;

		public const uint ProfileOfflineInfo = 1712390402U;

		public const uint ProfileHomeServerDN = 1712455711U;

		public const uint ProfileHomeServerAddrs = 1712525343U;

		public const uint ProfileServerDN = 1712586783U;

		public const uint ProfileAllPubDisplayName = 1712717855U;

		public const uint ProfileAllPubComment = 1712783391U;

		public const uint InTransitState = 1712848907U;

		public const uint InTransitStatus = 1712848899U;

		public const uint UserEntryId = 1712914690U;

		public const uint UserName = 1712979999U;

		public const uint MailboxOwnerEntryId = 1713045762U;

		public const uint MailboxOwnerName = 1713111071U;

		public const uint OofState = 1713176587U;

		public const uint TestLineSpeed = 1714094338U;

		public const uint FavoritesDefaultName = 1714749471U;

		public const uint FolderChildCount = 1714946051U;

		public const uint FolderChildCountInt64 = 1714946068U;

		public const uint SerializedReplidGuidMap = 1714946306U;

		public const uint Rights = 1715011587U;

		public const uint HasRules = 1715077131U;

		public const uint AddressBookEntryId = 1715142914U;

		public const uint HierarchyChangeNumber = 1715339267U;

		public const uint HasModeratorRules = 1715404811U;

		public const uint ModeratorRuleCount = 1715404803U;

		public const uint DeletedMsgCount = 1715470339U;

		public const uint DeletedMsgCountInt64 = 1715470356U;

		public const uint DeletedFolderCount = 1715535875U;

		public const uint DeletedAssocMsgCount = 1715666947U;

		public const uint DeletedAssocMsgCountInt64 = 1715666964U;

		public const uint ReplicaServer = 1715732511U;

		public const uint PromotedProperties = 1715798274U;

		public const uint HiddenPromotedProperties = 1715863810U;

		public const uint DAMOriginalEntryId = 1715863810U;

		public const uint LinkedSiteAuthorityUrl = 1715929119U;

		public const uint HasNamedProperties = 1716125707U;

		public const uint FidMid = 1716257026U;

		public const uint ActiveUserEntryId = 1716650242U;

		public const uint ICSChangeKey = 1716846850U;

		public const uint SetPropsCondition = 1716977922U;

		public const uint InternetContent = 1717108994U;

		public const uint OriginatorName = 1717239839U;

		public const uint OriginatorEmailAddress = 1717305375U;

		public const uint OriginatorAddressType = 1717370911U;

		public const uint OriginatorEntryId = 1717436674U;

		public const uint RecipientNumber = 1717698563U;

		public const uint ReportDestinationName = 1717829663U;

		public const uint ReportDestinationEntryId = 1717895426U;

		public const uint ProhibitReceiveQuota = 1718222851U;

		public const uint MaxSubmitMessageSize = 1718419459U;

		public const uint SearchAttachments = 1718419487U;

		public const uint ProhibitSendQuota = 1718484995U;

		public const uint SubmittedByAdmin = 1718550539U;

		public const uint LongTermEntryIdFromTable = 1718616322U;

		public const uint MemberId = 1718681620U;

		public const uint MemberName = 1718747167U;

		public const uint MemberRights = 1718812675U;

		public const uint MemberSecurityIdentifier = 1718878466U;

		public const uint RuleId = 1718878228U;

		public const uint MemberIsGroup = 1718943755U;

		public const uint RuleIds = 1718944002U;

		public const uint RuleSequence = 1719009283U;

		public const uint RuleState = 1719074819U;

		public const uint RuleUserFlags = 1719140355U;

		public const uint RuleCondition = 1719206141U;

		public const uint RuleMsgConditionOld = 1719206146U;

		public const uint RuleActions = 1719664898U;

		public const uint RuleMsgActionsOld = 1719664898U;

		public const uint RuleProvider = 1719730207U;

		public const uint RuleName = 1719795743U;

		public const uint RuleLevel = 1719861251U;

		public const uint RuleProviderData = 1719927042U;

		public const uint DeletedOn = 1720647744U;

		public const uint ReplicationStyle = 1720713219U;

		public const uint ReplicationTIB = 1720779010U;

		public const uint ReplicationMsgPriority = 1720844291U;

		public const uint WorkerProcessId = 1721171971U;

		public const uint MinimumDatabaseSchemaVersion = 1721237507U;

		public const uint ReplicaList = 1721237762U;

		public const uint OverallAgeLimit = 1721303043U;

		public const uint MaximumDatabaseSchemaVersion = 1721303043U;

		public const uint CurrentDatabaseSchemaVersion = 1721368579U;

		public const uint MailboxDatabaseVersion = 1721368579U;

		public const uint DeletedMessageSize = 1721434132U;

		public const uint DeletedMessageSize32 = 1721434115U;

		public const uint RequestedDatabaseSchemaVersion = 1721434115U;

		public const uint DeletedNormalMessageSize = 1721499668U;

		public const uint DeletedNormalMessageSize32 = 1721499651U;

		public const uint DeletedAssociatedMessageSize = 1721565204U;

		public const uint DeletedAssociatedMessageSize32 = 1721565187U;

		public const uint SecureInSite = 1721630731U;

		public const uint NTUsername = 1721761823U;

		public const uint NTUserSid = 1721762050U;

		public const uint LocaleId = 1721827331U;

		public const uint LastLogonTime = 1721892928U;

		public const uint LastLogoffTime = 1721958464U;

		public const uint StorageLimitInformation = 1722023939U;

		public const uint InternetMdns = 1722089483U;

		public const uint MailboxStatus = 1722089474U;

		public const uint MailboxFlags = 1722220547U;

		public const uint FolderFlags = 1722286083U;

		public const uint PreservingMailboxSignature = 1722286091U;

		public const uint MRSPreservingMailboxSignature = 1722351627U;

		public const uint LastAccessTime = 1722351680U;

		public const uint MailboxMessagesPerFolderCountWarningQuota = 1722482691U;

		public const uint MailboxMessagesPerFolderCountReceiveQuota = 1722548227U;

		public const uint NormalMsgWithAttachCount = 1722613763U;

		public const uint NormalMsgWithAttachCountInt64 = 1722613780U;

		public const uint DumpsterMessagesPerFolderCountWarningQuota = 1722613763U;

		public const uint AssocMsgWithAttachCount = 1722679299U;

		public const uint AssocMsgWithAttachCountInt64 = 1722679316U;

		public const uint DumpsterMessagesPerFolderCountReceiveQuota = 1722679299U;

		public const uint RecipientOnNormalMsgCount = 1722744835U;

		public const uint RecipientOnNormalMsgCountInt64 = 1722744852U;

		public const uint FolderHierarchyChildrenCountWarningQuota = 1722744835U;

		public const uint RecipientOnAssocMsgCount = 1722810371U;

		public const uint RecipientOnAssocMsgCountInt64 = 1722810388U;

		public const uint FolderHierarchyChildrenCountReceiveQuota = 1722810371U;

		public const uint AttachOnNormalMsgCt = 1722875907U;

		public const uint AttachOnNormalMsgCtInt64 = 1722875924U;

		public const uint FolderHierarchyDepthWarningQuota = 1722875907U;

		public const uint AttachOnAssocMsgCt = 1722941443U;

		public const uint AttachOnAssocMsgCtInt64 = 1722941460U;

		public const uint FolderHierarchyDepthReceiveQuota = 1722941443U;

		public const uint NormalMessageSize = 1723006996U;

		public const uint NormalMessageSize32 = 1723006979U;

		public const uint AssociatedMessageSize = 1723072532U;

		public const uint AssociatedMessageSize32 = 1723072515U;

		public const uint FolderPathName = 1723138079U;

		public const uint FoldersCountWarningQuota = 1723138051U;

		public const uint OwnerCount = 1723203587U;

		public const uint FoldersCountReceiveQuota = 1723203587U;

		public const uint ContactCount = 1723269123U;

		public const uint NamedPropertiesCountQuota = 1723269123U;

		public const uint CodePageId = 1724055555U;

		public const uint RetentionAgeLimit = 1724121091U;

		public const uint DisablePerUserRead = 1724186635U;

		public const uint UserDN = 1724514335U;

		public const uint UserDisplayName = 1724579871U;

		public const uint ServerDN = 1725956127U;

		public const uint BackfillRanking = 1726021635U;

		public const uint LastTransmissionTime = 1726087171U;

		public const uint StatusSendTime = 1726152768U;

		public const uint BackfillEntryCount = 1726218243U;

		public const uint NextBroadcastTime = 1726283840U;

		public const uint NextBackfillTime = 1726349376U;

		public const uint LastCNBroadcast = 1726415106U;

		public const uint BackfillId = 1726677250U;

		public const uint LastShortCNBroadcast = 1727267074U;

		public const uint AverageTransmissionTime = 1727725632U;

		public const uint ReplicationStatus = 1727791124U;

		public const uint LastDataReceivalTime = 1727856704U;

		public const uint AdminDisplayName = 1727922207U;

		public const uint WizardNoPSTPage = 1728053259U;

		public const uint WizardNoPABPage = 1728118795U;

		public const uint SortLocaleId = 1728380931U;

		public const uint MailboxDSGuid = 1728512258U;

		public const uint MailboxDSGuidGuid = 1728512072U;

		public const uint URLName = 1728512031U;

		public const uint DateDiscoveredAbsentInDS = 1728577600U;

		public const uint UnifiedMailboxGuidGuid = 1728577608U;

		public const uint LocalCommitTime = 1728643136U;

		public const uint LocalCommitTimeMax = 1728708672U;

		public const uint DeletedCountTotal = 1728774147U;

		public const uint DeletedCountTotalInt64 = 1728774164U;

		public const uint AutoReset = 1728843848U;

		public const uint ScopeFIDs = 1729233154U;

		public const uint ELCAutoCopyTag = 1729495298U;

		public const uint ELCMoveDate = 1729560834U;

		public const uint PFAdminDescription = 1729560607U;

		public const uint PFProxy = 1729954050U;

		public const uint PFPlatinumHomeMdb = 1730019339U;

		public const uint PFProxyRequired = 1730084875U;

		public const uint PFOverHardQuotaLimit = 1730215939U;

		public const uint QuotaWarningThreshold = 1730215939U;

		public const uint PFMsgSizeLimit = 1730281475U;

		public const uint QuotaSendThreshold = 1730281475U;

		public const uint PFDisallowMdbWideExpiry = 1730347019U;

		public const uint QuotaReceiveThreshold = 1730347011U;

		public const uint FolderAdminFlags = 1731002371U;

		public const uint TimeInServer = 1731002371U;

		public const uint TimeInCpu = 1731067907U;

		public const uint ProvisionedFID = 1731133460U;

		public const uint RopCount = 1731133443U;

		public const uint ELCFolderSize = 1731198996U;

		public const uint PageRead = 1731198979U;

		public const uint ELCFolderQuota = 1731264515U;

		public const uint PagePreread = 1731264515U;

		public const uint ELCPolicyId = 1731330079U;

		public const uint LogRecordCount = 1731330051U;

		public const uint ELCPolicyComment = 1731395615U;

		public const uint LogRecordBytes = 1731395587U;

		public const uint PropertyGroupMappingId = 1731461123U;

		public const uint LdapReads = 1731461123U;

		public const uint LdapSearches = 1731526659U;

		public const uint DigestCategory = 1731592223U;

		public const uint SampleId = 1731657731U;

		public const uint SampleTime = 1731723328U;

		public const uint PropGroupInfo = 1732116738U;

		public const uint PropertyGroupChangeMask = 1732116483U;

		public const uint ReadCnNewExport = 1732182274U;

		public const uint SentMailSvrEID = 1732247803U;

		public const uint SentMailSvrEIDBin = 1732247810U;

		public const uint LocallyDelivered = 1732575490U;

		public const uint MimeSize = 1732640788U;

		public const uint MimeSize32 = 1732640771U;

		public const uint FileSize = 1732706324U;

		public const uint FileSize32 = 1732706307U;

		public const uint Fid = 1732771860U;

		public const uint FidBin = 1732772098U;

		public const uint ParentFid = 1732837396U;

		public const uint ParentFidBin = 1732837634U;

		public const uint Mid = 1732902932U;

		public const uint MidBin = 1732903170U;

		public const uint CategID = 1732968468U;

		public const uint ParentCategID = 1733034004U;

		public const uint InstanceId = 1733099540U;

		public const uint InstanceNum = 1733165059U;

		public const uint ChangeType = 1733296130U;

		public const uint ArticleNumNext = 1733361667U;

		public const uint RequiresRefResolve = 1733361675U;

		public const uint ImapLastArticleId = 1733427203U;

		public const uint LTID = 1733820674U;

		public const uint CnExport = 1733886210U;

		public const uint PclExport = 1733951746U;

		public const uint CnMvExport = 1734017282U;

		public const uint MidsetDeletedExport = 1734082818U;

		public const uint ArticleNumMic = 1734148099U;

		public const uint ArticleNumMost = 1734213635U;

		public const uint RulesSync = 1734344707U;

		public const uint ReplicaListR = 1734410498U;

		public const uint SortOrder = 1734410498U;

		public const uint LocalIdNext = 1734410498U;

		public const uint ReplicaListRC = 1734476034U;

		public const uint ReplicaListRBUG = 1734541570U;

		public const uint RootFid = 1734606868U;

		public const uint FIDC = 1734738178U;

		public const uint EventMailboxGuid = 1735000322U;

		public const uint MdbDSGuid = 1735000322U;

		public const uint MailboxOwnerDN = 1735065631U;

		public const uint MailboxGuid = 1735131394U;

		public const uint MapiEntryIdGuid = 1735131394U;

		public const uint MapiEntryIdGuidGuid = 1735131208U;

		public const uint ImapCachedBodystructure = 1735196930U;

		public const uint Localized = 1735196683U;

		public const uint LCID = 1735262211U;

		public const uint AltRecipientDN = 1735327775U;

		public const uint NoLocalDelivery = 1735393291U;

		public const uint SoftDeleted = 1735393291U;

		public const uint DeliveryContentLength = 1735458819U;

		public const uint AutoReply = 1735524363U;

		public const uint MailboxOwnerDisplayName = 1735589919U;

		public const uint MailboxLastUpdated = 1735655488U;

		public const uint AdminSurName = 1735720991U;

		public const uint AdminGivenName = 1735786527U;

		public const uint ActiveSearchCount = 1735852035U;

		public const uint AdminNickname = 1735917599U;

		public const uint QuotaStyle = 1735983107U;

		public const uint OverQuotaLimit = 1736048643U;

		public const uint StorageQuota = 1736114179U;

		public const uint SubmitContentLength = 1736179715U;

		public const uint ReservedIdCounterRangeUpperLimit = 1736310804U;

		public const uint FolderPropTagArray = 1736311042U;

		public const uint ReservedCnCounterRangeUpperLimit = 1736376340U;

		public const uint MsgFolderPropTagArray = 1736376578U;

		public const uint SetReceiveCount = 1736441859U;

		public const uint SubmittedCount = 1736572931U;

		public const uint CreatorToken = 1736638722U;

		public const uint SearchState = 1736638467U;

		public const uint SearchRestriction = 1736704258U;

		public const uint SearchFIDs = 1736769794U;

		public const uint RecursiveSearchFIDs = 1736835330U;

		public const uint SearchBacklinks = 1736900866U;

		public const uint LCIDRestriction = 1736966147U;

		public const uint CategFIDs = 1737097474U;

		public const uint FolderCDN = 1737294082U;

		public const uint MidSegmentStart = 1737555988U;

		public const uint MidsetDeleted = 1737621762U;

		public const uint FolderIdsetIn = 1737621762U;

		public const uint MidsetExpired = 1737687298U;

		public const uint CnsetIn = 1737752834U;

		public const uint CnsetBackfill = 1737883906U;

		public const uint CnsetSeen = 1737883906U;

		public const uint MidsetTombstones = 1738014978U;

		public const uint GWFolder = 1738145803U;

		public const uint IPMFolder = 1738211339U;

		public const uint PublicFolderPath = 1738276895U;

		public const uint MidSegmentIndex = 1738473474U;

		public const uint MidSegmentSize = 1738539010U;

		public const uint CnSegmentStart = 1738604546U;

		public const uint CnSegmentIndex = 1738670082U;

		public const uint CnSegmentSize = 1738735618U;

		public const uint ChangeNumber = 1738801172U;

		public const uint ChangeNumberBin = 1738801410U;

		public const uint PCL = 1738866946U;

		public const uint CnMv = 1738936340U;

		public const uint FolderTreeRootFID = 1738997780U;

		public const uint SourceEntryId = 1739063554U;

		public const uint MailFlags = 1739128834U;

		public const uint Associated = 1739194379U;

		public const uint SubmitResponsibility = 1739259907U;

		public const uint SharedReceiptHandling = 1739390987U;

		public const uint Inid = 1739587842U;

		public const uint ViewRestriction = 1739587837U;

		public const uint MessageAttachList = 1739784450U;

		public const uint SenderCAI = 1739915522U;

		public const uint SentRepresentingCAI = 1739981058U;

		public const uint OriginalSenderCAI = 1740046594U;

		public const uint OriginalSentRepresentingCAI = 1740112130U;

		public const uint ReceivedByCAI = 1740177666U;

		public const uint ReceivedRepresentingCAI = 1740243202U;

		public const uint ReadReceiptCAI = 1740308738U;

		public const uint ReportCAI = 1740374274U;

		public const uint CreatorCAI = 1740439810U;

		public const uint LastModifierCAI = 1740505346U;

		public const uint AnonymousRights = 1740898306U;

		public const uint SearchGUID = 1741553922U;

		public const uint CnsetRead = 1741816066U;

		public const uint CnsetBackfillFAI = 1742340354U;

		public const uint CnsetSeenFAI = 1742340354U;

		public const uint ReplMsgVersion = 1742602243U;

		public const uint IdSetDeleted = 1743061250U;

		public const uint FolderMessages = 1743126786U;

		public const uint SenderReplid = 1743192322U;

		public const uint CnMin = 1743257620U;

		public const uint CnMax = 1743323156U;

		public const uint ReplMsgType = 1743388675U;

		public const uint RgszDNResponders = 1743454466U;

		public const uint ViewCoveringPropertyTags = 1743917059U;

		public const uint ViewAccessTime = 1743978560U;

		public const uint ICSViewFilter = 1744044043U;

		public const uint ModifiedCount = 1744175107U;

		public const uint DeletedState = 1744240643U;

		public const uint OriginatorCAI = 1744306434U;

		public const uint ReportDestinationCAI = 1744371970U;

		public const uint OriginalAuthorCAI = 1744437506U;

		public const uint ReadCnNew = 1744699412U;

		public const uint ReadCnNewBin = 1744699650U;

		public const uint SenderTelephoneNumber = 1744961567U;

		public const uint ShutoffQuota = 1745092611U;

		public const uint VoiceMessageAttachmentOrder = 1745158175U;

		public const uint MailboxMiscFlags = 1745223683U;

		public const uint EventCounter = 1745289236U;

		public const uint EventMask = 1745354755U;

		public const uint EventFid = 1745420546U;

		public const uint EventMid = 1745486082U;

		public const uint EventFidParent = 1745551618U;

		public const uint MailboxInCreation = 1745551371U;

		public const uint EventFidOld = 1745617154U;

		public const uint EventMidOld = 1745682690U;

		public const uint ObjectClassFlags = 1745682435U;

		public const uint EventFidOldParent = 1745748226U;

		public const uint ptagMsgHeaderTableFID = 1745747988U;

		public const uint EventCreatedTime = 1745813568U;

		public const uint EventMessageClass = 1745879071U;

		public const uint OOFStateEx = 1745879043U;

		public const uint EventItemCount = 1745944579U;

		public const uint EventFidRoot = 1746010370U;

		public const uint EventUnreadCount = 1746075651U;

		public const uint OofStateUserChangeTime = 1746075712U;

		public const uint EventTransacId = 1746141187U;

		public const uint UserOofSettingsItemId = 1746141442U;

		public const uint DocumentId = 1746206723U;

		public const uint EventFlags = 1746206723U;

		public const uint EventExtendedFlags = 1746403348U;

		public const uint EventClientType = 1746468867U;

		public const uint SoftDeletedFilter = 1746468875U;

		public const uint EventSid = 1746534658U;

		public const uint AssociatedFilter = 1746534411U;

		public const uint MailboxQuarantined = 1746534411U;

		public const uint EventDocId = 1746599939U;

		public const uint ConversationsFilter = 1746599947U;

		public const uint MailboxQuarantineDescription = 1746599967U;

		public const uint MailboxQuarantineLastCrash = 1746665536U;

		public const uint InstanceGuid = 1746731080U;

		public const uint MailboxQuarantineEnd = 1746731072U;

		public const uint MailboxNum = 1746862083U;

		public const uint InferenceActivityId = 1746927619U;

		public const uint InferenceClientId = 1746993155U;

		public const uint InferenceItemId = 1747058946U;

		public const uint InferenceTimeStamp = 1747124288U;

		public const uint InferenceWindowId = 1747189832U;

		public const uint InferenceSessionId = 1747255368U;

		public const uint InferenceFolderId = 1747321090U;

		public const uint ConversationDocumentId = 1747320835U;

		public const uint ConversationIdHash = 1747386371U;

		public const uint InferenceOofEnabled = 1747386379U;

		public const uint InferenceDeleteType = 1747451907U;

		public const uint LocalDirectoryBlob = 1747452162U;

		public const uint InferenceBrowser = 1747517471U;

		public const uint MemberEmail = 1747517471U;

		public const uint InferenceLocaleId = 1747582979U;

		public const uint MemberExternalId = 1747583007U;

		public const uint InferenceLocation = 1747648543U;

		public const uint MemberSID = 1747648770U;

		public const uint InferenceConversationId = 1747714306U;

		public const uint InferenceIpAddress = 1747779615U;

		public const uint MaxMessageSize = 1747779587U;

		public const uint InferenceTimeZone = 1747845151U;

		public const uint InferenceCategory = 1747910687U;

		public const uint InferenceAttachmentId = 1747976450U;

		public const uint LastUserAccessTime = 1747976256U;

		public const uint InferenceGlobalObjectId = 1748041986U;

		public const uint LastUserModificationTime = 1748041792U;

		public const uint InferenceModuleSelected = 1748107267U;

		public const uint InferenceLayoutType = 1748172831U;

		public const uint ViewStyle = 1748238339U;

		public const uint InferenceClientActivityFlags = 1748238339U;

		public const uint InferenceOWAUserActivityLoggingEnabledDeprecated = 1748303883U;

		public const uint InferenceOLKUserActivityLoggingEnabled = 1748369419U;

		public const uint FreebusyEMA = 1749614623U;

		public const uint WunderbarLinkEntryID = 1749811458U;

		public const uint WunderbarLinkStoreEntryId = 1749942530U;

		public const uint SchdInfoFreebusyMerged = 1750077698U;

		public const uint WunderbarLinkGroupClsId = 1750073602U;

		public const uint WunderbarLinkGroupName = 1750138911U;

		public const uint WunderbarLinkSection = 1750204419U;

		public const uint NavigationNodeCalendarColor = 1750269955U;

		public const uint NavigationNodeAddressbookEntryId = 1750335746U;

		public const uint AgingDeleteItems = 1750401027U;

		public const uint AgingFileName9AndPrev = 1750466591U;

		public const uint AgingAgeFolder = 1750532107U;

		public const uint AgingDontAgeMe = 1750597643U;

		public const uint AgingFileNameAfter9 = 1750663199U;

		public const uint AgingWhenDeletedOnServer = 1750794251U;

		public const uint AgingWaitUntilExpired = 1750859787U;

		public const uint InferenceTrainedModelVersionBreadCrumb = 1752367362U;

		public const uint ConversationMvFrom = 1753223199U;

		public const uint ConversationMvFromMailboxWide = 1753288735U;

		public const uint ConversationMvTo = 1753354271U;

		public const uint ConversationMvToMailboxWide = 1753419807U;

		public const uint ConversationMessageDeliveryTime = 1753481280U;

		public const uint ConversationMessageDeliveryTimeMailboxWide = 1753546816U;

		public const uint ConversationCategories = 1753616415U;

		public const uint ConversationCategoriesMailboxWide = 1753681951U;

		public const uint ConversationFlagStatus = 1753743363U;

		public const uint ConversationFlagStatusMailboxWide = 1753808899U;

		public const uint ConversationFlagCompleteTime = 1753874496U;

		public const uint ConversationFlagCompleteTimeMailboxWide = 1753940032U;

		public const uint ConversationHasAttach = 1754005515U;

		public const uint ConversationHasAttachMailboxWide = 1754071051U;

		public const uint ConversationContentCount = 1754136579U;

		public const uint ConversationContentCountMailboxWide = 1754202115U;

		public const uint ConversationContentUnread = 1754267651U;

		public const uint ConversationContentUnreadMailboxWide = 1754333187U;

		public const uint ConversationMessageSize = 1754398723U;

		public const uint ConversationMessageSizeMailboxWide = 1754464259U;

		public const uint ConversationMessageClasses = 1754533919U;

		public const uint ConversationMessageClassesMailboxWide = 1754599455U;

		public const uint ConversationReplyForwardState = 1754660867U;

		public const uint ConversationReplyForwardStateMailboxWide = 1754726403U;

		public const uint ConversationImportance = 1754791939U;

		public const uint ConversationImportanceMailboxWide = 1754857475U;

		public const uint ConversationMvFromUnread = 1754927135U;

		public const uint ConversationMvFromUnreadMailboxWide = 1754992671U;

		public const uint CategCount = 1755185155U;

		public const uint ConversationMvItemIds = 1755320578U;

		public const uint ConversationMvItemIdsMailboxWide = 1755386114U;

		public const uint ConversationHasIrm = 1755447307U;

		public const uint ConversationHasIrmMailboxWide = 1755512843U;

		public const uint PersonCompanyNameMailboxWide = 1755578399U;

		public const uint PersonDisplayNameMailboxWide = 1755643935U;

		public const uint PersonGivenNameMailboxWide = 1755709471U;

		public const uint PersonSurnameMailboxWide = 1755775007U;

		public const uint PersonPhotoContactEntryIdMailboxWide = 1755840770U;

		public const uint ConversationInferredImportanceInternal = 1755971589U;

		public const uint ConversationInferredImportanceOverride = 1756037123U;

		public const uint ConversationInferredUnimportanceInternal = 1756102661U;

		public const uint ConversationInferredImportanceInternalMailboxWide = 1756168197U;

		public const uint ConversationInferredImportanceOverrideMailboxWide = 1756233731U;

		public const uint ConversationInferredUnimportanceInternalMailboxWide = 1756299269U;

		public const uint PersonFileAsMailboxWide = 1756364831U;

		public const uint PersonRelevanceScoreMailboxWide = 1756430339U;

		public const uint PersonIsDistributionListMailboxWide = 1756495883U;

		public const uint PersonHomeCityMailboxWide = 1756561439U;

		public const uint PersonCreationTimeMailboxWide = 1756627008U;

		public const uint PersonGALLinkIDMailboxWide = 1756823624U;

		public const uint PersonMvEmailAddressMailboxWide = 1757024287U;

		public const uint PersonMvEmailDisplayNameMailboxWide = 1757089823U;

		public const uint PersonMvEmailRoutingTypeMailboxWide = 1757155359U;

		public const uint PersonImAddressMailboxWide = 1757216799U;

		public const uint PersonWorkCityMailboxWide = 1757282335U;

		public const uint ConversationGroupingActions = 1757286402U;

		public const uint PersonDisplayNameFirstLastMailboxWide = 1757347871U;

		public const uint ConversationGroupingActionsMailboxWide = 1757351938U;

		public const uint PersonDisplayNameLastFirstMailboxWide = 1757413407U;

		public const uint ConversationPredictedActionsSummary = 1757413379U;

		public const uint ConversationPredictedActionsSummaryMailboxWide = 1757478915U;

		public const uint ConversationHasClutter = 1757544459U;

		public const uint ConversationHasClutterMailboxWide = 1757609995U;

		public const uint ConversationLastMemberDocumentId = 1761607683U;

		public const uint ConversationPreview = 1761673247U;

		public const uint ConversationLastMemberDocumentIdMailboxWide = 1761738755U;

		public const uint ConversationInitialMemberDocumentId = 1761804291U;

		public const uint ConversationMemberDocumentIds = 1761873923U;

		public const uint ConversationMessageDeliveryOrRenewTimeMailboxWide = 1761935424U;

		public const uint NDRFromName = 1761935391U;

		public const uint FamilyId = 1762001154U;

		public const uint ConversationMessageRichContentMailboxWide = 1762070530U;

		public const uint ConversationPreviewMailboxWide = 1762131999U;

		public const uint ConversationMessageDeliveryOrRenewTime = 1762197568U;

		public const uint AttachEXCLIVersion = 1762197507U;

		public const uint ConversationWorkingSetSourcePartition = 1762263071U;

		public const uint SecurityFlags = 1845559299U;

		public const uint SecurityReceiptRequestProcessed = 1845755915U;

		public const uint FavoriteDisplayName = 2080374815U;

		public const uint FavoriteDisplayAlias = 2080440351U;

		public const uint FavPublicSourceKey = 2080506114U;

		public const uint SyncCustomState = 2080506114U;

		public const uint SyncFolderSourceKey = 2080571650U;

		public const uint SyncFolderChangeKey = 2080637186U;

		public const uint SyncFolderLastModificationTime = 2080702528U;

		public const uint UserConfigurationDataType = 2080768003U;

		public const uint UserConfigurationXmlStream = 2080899330U;

		public const uint UserConfigurationStream = 2080964866U;

		public const uint ptagSyncState = 2081030402U;

		public const uint ReplyFwdStatus = 2081095711U;

		public const uint UserPhotoCacheId = 2082078723U;

		public const uint UserPhotoPreviewCacheId = 2082144259U;

		public const uint OscSyncEnabledOnServer = 2082734091U;

		public const uint Processed = 2097217547U;

		public const uint FavLevelMask = 2097348611U;

		public const uint HasDlpDetectedAttachmentClassifications = 2146959391U;

		public const uint SExceptionReplaceTime = 2147024960U;

		public const uint AttachmentLinkId = 2147090435U;

		public const uint ExceptionStartTime = 2147155975U;

		public const uint ExceptionEndTime = 2147221511U;

		public const uint AttachmentFlags2 = 2147287043U;

		public const uint AttachmentHidden = 2147352587U;

		public const uint AttachmentContactPhoto = 2147418123U;
	}
}
