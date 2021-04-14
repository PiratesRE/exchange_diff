using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport
{
	internal static class AckReason
	{
		public static SmtpResponse MimeToMimeInvalidContent(string errorDetails)
		{
			return new SmtpResponse("550", "5.6.0", new string[]
			{
				errorDetails
			});
		}

		public static SmtpResponse MimeToMimeStorageError(string errorDetails)
		{
			return new SmtpResponse("550", "5.6.0", new string[]
			{
				errorDetails
			});
		}

		public static SmtpResponse UnreachableMessageExpired(string unreachableReason)
		{
			return new SmtpResponse("550", "4.4.7", new string[]
			{
				"QUEUE.Expired; message expired in unreachable destination queue. Reason: " + unreachableReason
			});
		}

		public static SmtpResponse InboundPoisonMessage(int crashCount)
		{
			return new SmtpResponse("550", "5.7.0", new string[]
			{
				string.Format("STOREDRV.Deliver; message is treated as poison. Crash count = '{0}'", crashCount)
			});
		}

		public static bool IsMailboxTransportDeliveryPoisonMessageResponse(SmtpResponse smtpResponse)
		{
			return string.Equals(smtpResponse.StatusCode, "550", StringComparison.Ordinal) && string.Equals(smtpResponse.EnhancedStatusCode, "5.7.0", StringComparison.Ordinal);
		}

		public const string MailboxTransportPoisonResponseStatusCode = "550";

		public const string InboundPoisonMessageEnhancedStatusCode = "5.7.0";

		public static readonly SmtpResponse PickupInvalidAddress = new SmtpResponse("550", "5.1.3", new string[]
		{
			"PICKUP.InvalidAddress; invalid address"
		});

		public static readonly SmtpResponse PickupMessageTooLarge = new SmtpResponse("550", "5.3.4", new string[]
		{
			"PICKUP.MessageSize; message size exceeds a fixed maximum size limit"
		});

		public static readonly SmtpResponse PickupHeaderTooLarge = new SmtpResponse("550", "5.3.4", new string[]
		{
			"PICKUP.HeaderSize; header size exceeds a fixed maximum size limit"
		});

		public static readonly SmtpResponse PickupTooManyRecipients = new SmtpResponse("550", "5.5.3", new string[]
		{
			"PICKUP.RecipLimit; too many recipients"
		});

		public static readonly SmtpResponse MessageDelayedDeleteByAdmin = new SmtpResponse("550", "4.3.2", new string[]
		{
			"QUEUE.DDAdmin; message deleted by administrative action"
		});

		public static readonly SmtpResponse MessageDeletedByAdmin = new SmtpResponse("550", "4.3.2", new string[]
		{
			"QUEUE.Admin; message deleted by administrative action"
		});

		public static readonly SmtpResponse MessageDeletedByTransportAgent = new SmtpResponse("550", "4.3.2", new string[]
		{
			"QUEUE.TransportAgent; message deleted by transport agent"
		});

		public static readonly SmtpResponse PoisonMessageDeletedByAdmin = new SmtpResponse("550", "4.3.2", new string[]
		{
			"QUEUE.PoisonAdmin; message deleted by administrative action"
		});

		public static readonly SmtpResponse MessageExpired = new SmtpResponse("550", "4.4.7", new string[]
		{
			"QUEUE.Expired; message expired"
		});

		public static readonly SmtpResponse MessageTooLargeForHighPriority = new SmtpResponse("550", "5.2.3", new string[]
		{
			"QUEUE.Priority; message too large to be sent with high priority"
		});

		public static readonly SmtpResponse ReplayMessageTooLarge = new SmtpResponse("550", "5.3.4", new string[]
		{
			"REPLAY.MessageSize; message size exceeds a fixed maximum size limit"
		});

		public static readonly SmtpResponse AmbiguousAddressPermanent = new SmtpResponse("550", "5.1.4", new string[]
		{
			"RESOLVER.ADR.Ambiguous; ambiguous address"
		});

		public static readonly SmtpResponse AmbiguousAddressTransient = new SmtpResponse("420", "4.2.0", new string[]
		{
			"RESOLVER.ADR.Ambiguous; ambiguous address"
		});

		public static readonly SmtpResponse BadOrMissingPrimarySmtpAddress = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.ADR.BadPrimary; recipient primary SMTP address is missing or invalid"
		});

		public static readonly SmtpResponse EncapsulatedSmtpAddress = new SmtpResponse("550", "5.1.0", new string[]
		{
			"RESOLVER.ADR.SmtpInSmtp; encapsulated SMTP address inside an SMTP address (IMCEASMTP-)"
		});

		public static readonly SmtpResponse EncapsulatedX500Address = new SmtpResponse("550", "5.1.0", new string[]
		{
			"RESOLVER.ADR.X500InSmtp; encapsulated X.500 address inside an SMTP address (IMCEAX500-)"
		});

		public static readonly SmtpResponse EncapsulatedInvalidAddress = new SmtpResponse("550", "5.1.0", new string[]
		{
			"RESOLVER.ADR.InvalidInSmtp; encapsulated INVALID address inside an SMTP address (IMCEAINVALID-)"
		});

		public static readonly SmtpResponse InvalidDirectoryObject = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.ADR.Invalid; the recipient's directory object is misconfigured"
		});

		public static readonly SmtpResponse InvalidObjectOnSearch = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.ADR.InvalidResult; the recipient's directory object is misconfigured"
		});

		public static readonly SmtpResponse LocalRecipientAddressUnknown = new SmtpResponse("550", "5.1.1", new string[]
		{
			"RESOLVER.ADR.RecipNotFound; not found"
		});

		public static readonly SmtpResponse LocalRecipientExAddressUnknown = new SmtpResponse("550", "5.1.1", new string[]
		{
			"RESOLVER.ADR.ExRecipNotFound; not found"
		});

		public static readonly SmtpResponse LocalRecipientX400AddressUnknown = new SmtpResponse("550", "5.1.1", new string[]
		{
			"RESOLVER.ADR.X400RecipNotFound; not found"
		});

		public static readonly SmtpResponse UnencapsulatableTargetAddress = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.CON.UnencapTarget; contact's configured external address is unroutable"
		});

		public static readonly SmtpResponse ContactChainNotMailEnabled = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.CON.ChainDisabled; misconfigured forwarded contact"
		});

		public static readonly SmtpResponse ContactChainHandled = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.CON.Chained; contact forwarded"
		});

		public static readonly SmtpResponse ContactChainAmbiguousPermanent = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.CON.Ambiguous; contact has an ambiguous forwarding address"
		});

		public static readonly SmtpResponse ContactChainAmbiguousTransient = new SmtpResponse("420", "4.2.0", new string[]
		{
			"RESOLVER.CON.Ambiguous; contact has an ambiguous forwarding address"
		});

		public static readonly SmtpResponse ContactChainInvalid = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.CON.ChainInvalid; contact forwards to an invalid object"
		});

		public static readonly SmtpResponse ContactInvalidTargetAddress = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.CON.InvalidTarget; contact's target address is invalid"
		});

		public static readonly SmtpResponse ContactMissingTargetAddress = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.CON.NoTarget; contact's external address is missing"
		});

		public static readonly SmtpResponse ForwardedToAlternateRecipient = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.FWD.Forwarded; recipient forwarded"
		});

		public static readonly SmtpResponse AlternateRecipientInvalid = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.FWD.Invalid; misconfigured forwarding address"
		});

		public static readonly SmtpResponse AlternateRecipientNotFound = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.FWD.NotFound; misconfigured forwarding address"
		});

		public static readonly SmtpResponse MigratedPublicFolderInvalidTargetAddress = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.PF.InvalidTarget; migrated public folder's external address is invalid"
		});

		public static readonly SmtpResponse PublicFolderMailboxesInTransit = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.PF.InTransit; public folders unavailable due to ongoing migration"
		});

		public static readonly SmtpResponse ContentMailboxInvalid = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.PF.Invalid; misconfigured public folder mailbox"
		});

		public static readonly SmtpResponse ContentMailboxRecipientNotFound = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.PF.NotFound; public folder mailbox recipient not found"
		});

		public static readonly SmtpResponse AlternateRecipientBlockedBySender = new SmtpResponse("550", "5.7.300", new string[]
		{
			"RESOLVER.FWD.Blocked; the sender prohibited alt recipient redirection on this message"
		});

		public static readonly SmtpResponse DDLMisconfiguration = new SmtpResponse("550", "5.2.4", new string[]
		{
			"RESOLVER.GRP.DDLQuery; the dynamic distribution list has a misconfigured query"
		});

		public static readonly SmtpResponse DLBlockedMessageType = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.GRP.BlockedMessage; messages of this type are not delivered to groups"
		});

		public static readonly SmtpResponse DLBlockedRedirectableMessageType = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.GRP.NotRedirected; messages of this type are not delivered to groups"
		});

		public static readonly SmtpResponse DLExpansionBlockedBySender = new SmtpResponse("550", "5.7.100", new string[]
		{
			"RESOLVER.GRP.Blocked; the sender prohibited DL expansion on this message"
		});

		public static readonly SmtpResponse DLExpansionBlockedNeedsSenderRestrictions = new SmtpResponse("550", "5.7.1", new string[]
		{
			"RESOLVER.GRP.Blocked.NeedsSenderRestrictions; DL expansion needs sender restrictions or message approval configured"
		});

		public static readonly SmtpResponse DLExpanded = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.GRP.Expanded; distribution list expanded"
		});

		public static readonly SmtpResponse DLExpandedSilently = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.GRP.Expanded; distribution list expanded"
		});

		public static readonly SmtpResponse DLRedirectedToManager = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.GRP.ToOwner; message redirected to group owner"
		});

		public static readonly SmtpResponse DLRedirectManagerNotFound = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.GRP.NoOwner; no group owner to redirect to"
		});

		public static readonly SmtpResponse DLRedirectManagerNotValid = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.GRP.OwnerInvalid; group owner to redirect to is misconfigured"
		});

		public static readonly SmtpResponse InvalidGroupForExpansion = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.GRP.InvalidObject; the group's directory object is misconfigured"
		});

		public static readonly SmtpResponse ExpansionLoopDetected = new SmtpResponse("550", "5.4.6", new string[]
		{
			"RESOLVER.FWD.Loop; there is a forwarding loop configured in the directory"
		});

		public static readonly SmtpResponse SilentExpansionLoopDetected = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.ADR.AlreadyExpanded; already expanded this recipient"
		});

		public static readonly SmtpResponse DuplicateRecipient = new SmtpResponse("250", "2.1.5", new string[]
		{
			"CAT.DuplicateRecipient; recipient already present"
		});

		public static readonly SmtpResponse BlockExternalOofToInternalUser = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.OOF.ExtToInt; handled external OOF addressed to internal recipient"
		});

		public static readonly SmtpResponse BlockLegacyOofToExternalUser = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.OOF.LegacyToExt; handled legacy OOF addressed to external recipient"
		});

		public static readonly SmtpResponse BlockInternalOofToExternalUser = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.OOF.IntToExt; handled internal OOF addressed to external recipient"
		});

		public static readonly SmtpResponse BlockInternalOofToInternalOpenDomainUser = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.OOF.ExtToOpenDomainInt; handled external OOF addressed to internal recipient of an open domain"
		});

		public static readonly SmtpResponse BlockExternalOofToExternalUser = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.OOF.SuppressExternal; external OOFs are suppressed"
		});

		public static readonly SmtpResponse BlockDRToExternalUser = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.MSGTYPE.DR; handled DR addressed to external recipient"
		});

		public static readonly SmtpResponse BlockNDRToExternalUser = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.MSGTYPE.NDR; handled NDR addressed to external recipient"
		});

		public static readonly SmtpResponse BlockMFNToExternalUser = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.MSGTYPE.NDR; handled MFN addressed to external recipient"
		});

		public static readonly SmtpResponse BlockARToExternalUser = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.MSGTYPE.AR; handled AutoReply addressed to external recipient"
		});

		public static readonly SmtpResponse BlockAFToExternalUser = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.MSGTYPE.AF; handled AutoForward addressed to external recipient"
		});

		public static readonly SmtpResponse RecipientPermissionRestricted = new SmtpResponse("550", "5.7.1", new string[]
		{
			"RESOLVER.RST.NotAuthorized; not authorized"
		});

		public static readonly SmtpResponse RecipientPermissionRestrictedToGroup = new SmtpResponse("550", "5.7.1", new string[]
		{
			"RESOLVER.RST.NotAuthorizedToGroup; not authorized to send to the distribution list"
		});

		public static readonly SmtpResponse JournalReportFromUnauthorizedSender = new SmtpResponse("550", "5.7.1", new string[]
		{
			"RESOLVER.RST.JournalReportFromUnauthorizedSender; not authorized to send journal reports"
		});

		public static readonly SmtpResponse MessageTooLargeForReceiver = new SmtpResponse("550", "5.2.3", new string[]
		{
			"RESOLVER.RST.RecipSizeLimit; message too large for this recipient"
		});

		public static readonly SmtpResponse MessageTooLargeForSender = new SmtpResponse("550", "5.2.3", new string[]
		{
			"RESOLVER.RST.SendSizeLimit.Sender; message too large for this sender"
		});

		public static readonly SmtpResponse MessageTooLargeForOrganization = new SmtpResponse("550", "5.2.3", new string[]
		{
			"RESOLVER.RST.SendSizeLimit.Org; message too large for this organization"
		});

		public static readonly SmtpResponse MessageTooLargeForDistributionList = new SmtpResponse("550", "5.2.3", new string[]
		{
			"RESOLVER.RST.RecipSizeLimit.DL; message length exceeds administrative limit"
		});

		public static readonly SmtpResponse RecipientDiscarded = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.RST.RecipientDiscarded; recipient discarded due to failed distribution group limits check"
		});

		public static readonly SmtpResponse RecipientLimitExceeded = new SmtpResponse("550", "5.5.3", new string[]
		{
			"RESOLVER.ADR.RecipLimit; too many recipients"
		});

		public static readonly SmtpResponse RevertDueToRecipientLimit = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.ADR.RecipOverLimit; too many recipients"
		});

		public static readonly SmtpResponse NotAuthenticated = new SmtpResponse("550", "5.7.1", new string[]
		{
			"RESOLVER.RST.AuthRequired; authentication required"
		});

		public static readonly SmtpResponse InvalidDirectoryObjectForRestrictionCheck = new SmtpResponse("550", "5.2.0", new string[]
		{
			"RESOLVER.RST.InvalidObjectForRestrictionCheck; unable to perform restriction check because a directory object is misconfigured"
		});

		public static readonly SmtpResponse MicrosoftExchangeRecipientSuppressed = new SmtpResponse("250", "2.1.5", new string[]
		{
			"RESOLVER.MER.Suppressed; recipient suppressed"
		});

		public static readonly SmtpResponse ModerationStarted = new SmtpResponse("250", "2.1.5", new string[]
		{
			"Resolver.MT.StartModeration; recipient is moderated"
		});

		public static readonly SmtpResponse ModerationInitFailed = new SmtpResponse("554", "5.6.0", new string[]
		{
			"Resolver.MT.CannotStartModeration; recipient needs moderation, but moderation initiation failed due to content"
		});

		public static readonly SmtpResponse ModerationReencrptionFailed = new SmtpResponse("554", "5.6.0", new string[]
		{
			"Resolver.MT.ReencryptionFailed; Re-encryption failed during the moderation of a protected message."
		});

		public static readonly SmtpResponse ModerationNoArbitrationAddress = new SmtpResponse("550", "5.2.0", new string[]
		{
			"Resolver.MT.NoArbitrationAddress; No arbitration address for approval process"
		});

		public static readonly SmtpResponse NoModeratorAddresses = new SmtpResponse("550", "5.2.0", new string[]
		{
			"Resolver.MT.NoModeratorAddresses; No moderator addresses for approval process"
		});

		public static readonly SmtpResponse ModerationLoop = new SmtpResponse("550", "5.2.0", new string[]
		{
			"Resolver.MT.ModerationLoop; Loop in approval process"
		});

		public static readonly SmtpResponse MessageTooLargeForRoute = new SmtpResponse("550", "5.3.4", new string[]
		{
			"ROUTING.SizeLimit; message size exceeds fixed maximum size for route"
		});

		public static readonly SmtpResponse InvalidAddressForRouting = new SmtpResponse("550", "5.1.2", new string[]
		{
			"ROUTING.InvalidAddress; invalid address domain"
		});

		public static readonly SmtpResponse InvalidX400AddressForRouting = new SmtpResponse("550", "5.1.2", new string[]
		{
			"ROUTING.InvalidX400Address; invalid x400 address"
		});

		public static readonly SmtpResponse NoNextHop = new SmtpResponse("550", "5.4.4", new string[]
		{
			"ROUTING.NoNextHop; unable to route"
		});

		public static readonly SmtpResponse NoConnectorForAddressType = new SmtpResponse("550", "5.4.4", new string[]
		{
			"ROUTING.NoConnectorForAddressType; unable to route for address type"
		});

		public static readonly SmtpResponse QuarantineDisabled = new SmtpResponse("550", "5.2.1", new string[]
		{
			"DSNGENERATION.Quarantine; unable to quarantine"
		});

		public static readonly SmtpResponse ProbeMessageDropped = new SmtpResponse("250", "2.1.6", new string[]
		{
			"ROUTING.ProbeMessageDropped; probe message dropped"
		});

		public static readonly SmtpResponse MessageIsPoisonForRemoteServer = new SmtpResponse("550", "5.5.0", new string[]
		{
			"SMTPSEND.PoisonForRemote; message might be crashing the remote server"
		});

		public static readonly SmtpResponse SuspiciousRemoteServerError = new SmtpResponse("451", "4.4.0", new string[]
		{
			"SMTPSEND.SuspiciousRemoteServerError; remote server disconnected abruptly; retry will be delayed"
		});

		public static readonly SmtpResponse BareLinefeedsAreIllegal = new SmtpResponse("550", "5.6.2", new string[]
		{
			"SMTPSEND.BareLinefeedsAreIllegal; message contains bare linefeeds, which cannot be sent via DATA"
		});

		public static readonly SmtpResponse BdatOverAdvertisedSizeLimit = new SmtpResponse("550", "5.3.4", new string[]
		{
			"SMTPSEND.BDATOverAdvertisedSize; message size exceeds fixed maximum size"
		});

		public static readonly SmtpResponse DataOverAdvertisedSizeLimit = new SmtpResponse("550", "5.3.4", new string[]
		{
			"SMTPSEND.DATAOverAdvertisedSize; message size exceeds fixed maximum size"
		});

		public static readonly SmtpResponse OverAdvertisedSizeLimit = new SmtpResponse("550", "5.3.4", new string[]
		{
			"SMTPSEND.OverAdvertisedSize; message size exceeds fixed maximum size"
		});

		public static readonly SmtpResponse SmtpSendLongRecipientAddress = new SmtpResponse("550", "5.1.2", new string[]
		{
			"SMTPSEND.LongRecipientAddress; long recipient address"
		});

		public static readonly SmtpResponse SmtpSendInvalidLongRecipientAddress = new SmtpResponse("550", "5.1.2", new string[]
		{
			"SMTPSEND.InvalidLongRecipientAddress; invalid long recipient address"
		});

		public static readonly SmtpResponse SmtpSendLongSenderAddress = new SmtpResponse("550", "5.1.7", new string[]
		{
			"SMTPSEND.LongSenderAddress; long sender address"
		});

		public static readonly SmtpResponse SmtpSendInvalidLongSenderAddress = new SmtpResponse("550", "5.1.7", new string[]
		{
			"SMTPSEND.InvalidLongSenderAddress; invalid long sender address"
		});

		public static readonly SmtpResponse SmtpSendUtf8RecipientAddress = new SmtpResponse("550", "5.1.8", new string[]
		{
			"SMTPSEND.Utf8RecipientAddress; UTF-8 recipient address not supported."
		});

		public static readonly SmtpResponse SmtpSendUtf8SenderAddress = new SmtpResponse("550", "5.1.9", new string[]
		{
			"SMTPSEND.Utf8SenderAddress; UTF-8 sender address not supported."
		});

		public static readonly SmtpResponse SmtpSendOrarNotTransmittable = new SmtpResponse("550", "5.6.0", new string[]
		{
			"SMTPSEND.SmtpSendOrarNotTransmittable; unable to transmit ORAR"
		});

		public static readonly SmtpResponse SmtpSendLongOrarNotTransmittable = new SmtpResponse("550", "5.6.0", new string[]
		{
			"SMTPSEND.SmtpSendLongOrarNotTransmittable; unable to transmit long ORAR"
		});

		public static readonly SmtpResponse SmtpSendRDstNotTransmittable = new SmtpResponse("550", "5.6.0", new string[]
		{
			"SMTPSEND.SmtpSendRDstNotTransmittable; unable to transmit RDST"
		});

		public static readonly SmtpResponse DnsNonExistentDomain = new SmtpResponse("554", "5.4.4", new string[]
		{
			"SMTPSEND.DNS.NonExistentDomain; nonexistent domain"
		});

		public static readonly SmtpResponse DnsMxLoopback = new SmtpResponse("554", "5.4.4", new string[]
		{
			"SMTPSEND.DNS.MxLoopback; DNS records for this domain are configured in a loop"
		});

		public static readonly SmtpResponse DnsInvalidData = new SmtpResponse("554", "5.4.4", new string[]
		{
			"SMTPSEND.DNS.InvalidData; DNS returned an invalid response for this domain"
		});

		public static readonly SmtpResponse NoOutboundFrontendServers = new SmtpResponse("451", "4.4.0", new string[]
		{
			"SMTPSEND.DNS.NoOutboundFrontendServers; no outbound frontend servers to proxy through"
		});

		public static readonly SmtpResponse DnsNonExistentDomainForOutboundFrontend = new SmtpResponse("554", "5.4.4", new string[]
		{
			"SMTPSEND.DNS.NonExistentDomain; nonexistent domain while resolving the outbound proxy frontend server fqdn"
		});

		public static readonly SmtpResponse DnsMxLoopbackForOutboundFrontend = new SmtpResponse("554", "5.4.4", new string[]
		{
			"SMTPSEND.DNS.MxLoopback; DNS records for this domain are configured in a loop for the outbound proxy frontend servers fqdn"
		});

		public static readonly SmtpResponse DnsInvalidDataOutboundFrontend = new SmtpResponse("554", "5.4.4", new string[]
		{
			"SMTPSEND.DNS.InvalidData; DNS returned an invalid response nonexistent domain while resolving the outbound proxy frontend server fqdn"
		});

		public static readonly SmtpResponse SendingError = new SmtpResponse("451", "4.4.0", new string[]
		{
			"SMTPSEND.SendingError; Error while sending message"
		});

		public static readonly SmtpResponse UnexpectedException = new SmtpResponse("444", "4.4.4", new string[]
		{
			"NONSMTPGW.UnexpectedException Unexpected exception occurred opening a new non SMTP gateway connection."
		});

		public static readonly SmtpResponse GWInvalidSourceBH = new SmtpResponse("444", "4.4.4", new string[]
		{
			"NONSMTPGW.InvalidSourceBH This BH is not a source for this connector."
		});

		public static readonly SmtpResponse GWConnectorDeleted = new SmtpResponse("444", "4.4.4", new string[]
		{
			"NONSMTPGW.ConnectorDeleted The Connector has been deleted or disabled."
		});

		public static readonly SmtpResponse GWConnectorInvalid = new SmtpResponse("444", "4.4.4", new string[]
		{
			"NONSMTPGW.ConnectorInvalid The Connector object is invalid."
		});

		public static readonly SmtpResponse GWPathTooLongException = new SmtpResponse("444", "4.4.4", new string[]
		{
			"NONSMTPGW.PathTooLongException writing to the Foreign Connector Drop Directory."
		});

		public static readonly SmtpResponse GWIOException = new SmtpResponse("444", "4.4.4", new string[]
		{
			"NONSMTPGW.IOException writing to the Foreign Connector Drop Directory."
		});

		public static readonly SmtpResponse GWUnauthorizedAccess = new SmtpResponse("444", "4.4.4", new string[]
		{
			"NONSMTPGW.UnauthorizedAccess The access requested is not permitted by the OS for the specified path."
		});

		public static readonly SmtpResponse GWNoDropDirectory = new SmtpResponse("444", "4.4.4", new string[]
		{
			"NONSMTPGW.NoDropDirectory The Drop Directory does not exist or has invalid ACLs."
		});

		public static readonly SmtpResponse GWQuotaExceeded = new SmtpResponse("444", "4.4.4", new string[]
		{
			"NONSMTPGW.QuotaExceeded Drop Directory Quota is exceeded."
		});

		public static readonly SmtpResponse DeliveryAgentInvalidSourceBH = new SmtpResponse("444", "4.4.4", new string[]
		{
			"DELIVERYAGENT.InvalidSourceBH This BH is not a source for this connector."
		});

		public static readonly SmtpResponse DeliveryAgentConnectorDeleted = new SmtpResponse("444", "4.4.4", new string[]
		{
			"DELIVERYAGENT.ConnectorDeleted The Connector has been deleted or disabled."
		});

		public static readonly SmtpResponse DeliveryAgentConnectorInvalid = new SmtpResponse("444", "4.4.4", new string[]
		{
			"DELIVERYAGENT.ConnectorInvalid The Connector object is invalid."
		});

		public static readonly SmtpResponse InboundInvalidDirectoryData = new SmtpResponse("550", "5.2.0", new string[]
		{
			"STOREDRV.Deliver; directory misconfiguration."
		});

		public static readonly SmtpResponse OutboundInvalidDirectoryData = new SmtpResponse("550", "5.2.0", new string[]
		{
			"STOREDRV.Submit; directory misconfiguration."
		});

		public static readonly SmtpResponse PublicFolderReplicaServerMisconfiguration = new SmtpResponse("550", "5.2.0", new string[]
		{
			"STOREDRV.Deliver; PF replica server misconfiguration"
		});

		public static readonly SmtpResponse PublicFolderRoute = new SmtpResponse("420", "4.2.0", new string[]
		{
			"STOREDRV.Deliver; No local replica for public folder"
		});

		public static readonly SmtpResponse RecipientMailboxIsRemote = new SmtpResponse("420", "4.2.0", new string[]
		{
			"STOREDRV.Deliver; recipient mailbox is remote"
		});

		public static readonly SmtpResponse RecipientMailboxLocationInfoNotAvailable = new SmtpResponse("420", "4.2.0", new string[]
		{
			"STOREDRV.Deliver; recipient mailbox location information is not available"
		});

		public static readonly SmtpResponse SubmissionCancelled = new SmtpResponse("420", "4.2.0", new string[]
		{
			"STOREDRV.Submit; Submission is cancelled"
		});

		public static readonly SmtpResponse SubmissionCancelledRecipientMdbNotTargetted = new SmtpResponse("420", "4.2.0", new string[]
		{
			"STOREDRV.Submit; Submission is cancelled; recipient mailbox mdb not targetted for resubmission"
		});

		public static readonly SmtpResponse SubmissionCancelledProbeRequest = new SmtpResponse("420", "4.2.0", new string[]
		{
			"STOREDRV.Submit; Submission is cancelled; probe resubmit request"
		});

		public static readonly SmtpResponse OutboundInvalidAddress = new SmtpResponse("550", "5.1.3", new string[]
		{
			"STOREDRV.Submit; invalid recipient address"
		});

		public static readonly SmtpResponse InboundInvalidContent = new SmtpResponse("554", "5.6.0", new string[]
		{
			"STOREDRV.Deliver; invalid message content"
		});

		public static readonly SmtpResponse OutboundInvalidContent = new SmtpResponse("554", "5.6.0", new string[]
		{
			"STOREDRV.Submit; invalid message content"
		});

		public static readonly SmtpResponse MailboxDiskFull = new SmtpResponse("431", "4.3.1", new string[]
		{
			"STOREDRV; mailbox disk is full"
		});

		public static readonly SmtpResponse DiskFull = new SmtpResponse("431", "4.3.1", new string[]
		{
			"STOREDRV; disk is full"
		});

		public static readonly SmtpResponse MailboxIOError = new SmtpResponse("430", "4.3.0", new string[]
		{
			"STOREDRV; mailbox database IO error"
		});

		public static readonly SmtpResponse MailboxServerOffline = new SmtpResponse("432", "4.3.2", new string[]
		{
			"STOREDRV; mailbox server is offline"
		});

		public static readonly SmtpResponse MDBOffline = new SmtpResponse("432", "4.3.2", new string[]
		{
			"STOREDRV; mailbox database is offline"
		});

		public static readonly SmtpResponse MapiNoAccessFailure = new SmtpResponse("432", "4.3.2", new string[]
		{
			"STOREDRV.Deliver; retryable mailbox access denied failure"
		});

		public static readonly SmtpResponse MailboxServerTooBusy = new SmtpResponse("432", "4.3.2", new string[]
		{
			"STOREDRV.Storage; mailbox server is too busy"
		});

		public static readonly SmtpResponse MailboxMapiSessionLimit = new SmtpResponse("432", "4.3.2", new string[]
		{
			"STOREDRV.Storage; mailbox server mapi session limit exceeded"
		});

		public static readonly SmtpResponse MailboxServerNotEnoughMemory = new SmtpResponse("432", "4.3.2", new string[]
		{
			"STOREDRV.Storage; not enough memory for mapi operation"
		});

		public static readonly SmtpResponse MailboxServerMaxThreadsPerMdbExceeded = new SmtpResponse("432", "4.3.2", new string[]
		{
			"STOREDRV.Storage; max threads per mdb exceeded"
		});

		public static readonly SmtpResponse MapiExceptionMaxThreadsPerSCTExceeded = new SmtpResponse("432", "4.3.2", new string[]
		{
			"STOREDRV.Storage; max threads per sct exceeded"
		});

		public static readonly SmtpResponse MailboxServerThreadLimitExceeded = new SmtpResponse("432", "4.3.2", new string[]
		{
			"STOREDRV.Deliver; mailbox server thread limit exceeded"
		});

		public static readonly SmtpResponse MailboxDatabaseThreadLimitExceeded = new SmtpResponse("432", "4.3.2", new string[]
		{
			"STOREDRV.Deliver; mailbox database thread limit exceeded"
		});

		public static readonly SmtpResponse RecipientThreadLimitExceeded = new SmtpResponse("432", "4.3.2", new string[]
		{
			"STOREDRV.Deliver; recipient thread limit exceeded"
		});

		public static readonly SmtpResponse DeliverySourceThreadLimitExceeded = new SmtpResponse("432", "4.3.2", new string[]
		{
			"STOREDRV.Deliver; delivery source thread limit exceeded"
		});

		public static readonly SmtpResponse DynamicMailboxDatabaseThrottlingLimitExceeded = new SmtpResponse("432", "4.3.2", new string[]
		{
			"STOREDRV.Deliver; dynamic mailbox database throttling limit exceeded."
		});

		public static readonly SmtpResponse MaxConcurrentMessageSizeLimitExceeded = new SmtpResponse("432", "4.3.2", new string[]
		{
			"STOREDRV.Deliver; max concurrent message size limit exceeded."
		});

		public static readonly SmtpResponse LogonFailure = new SmtpResponse("430", "4.2.0", new string[]
		{
			"STOREDRV; mailbox logon failure"
		});

		public static readonly SmtpResponse UnrecognizedClassification = new SmtpResponse("550", "5.7.3", new string[]
		{
			"Message classification was not recognized"
		});

		public static readonly SmtpResponse NoLegacyDN = new SmtpResponse("420", "4.2.0", new string[]
		{
			"STOREDRV.Deliver; legacyExchangeDN attribute value is missing for recipient"
		});

		public static readonly SmtpResponse SubscriptionNotFound = new SmtpResponse("550", "5.6.2", new string[]
		{
			"STOREDRV.Submit; subscription not found"
		});

		public static readonly SmtpResponse SubscriptionDisabled = new SmtpResponse("550", "5.6.2", new string[]
		{
			"STOREDRV.Submit; subscription is disabled"
		});

		public static readonly SmtpResponse SubscriptionNotEnabledForSendAs = new SmtpResponse("550", "5.6.2", new string[]
		{
			"STOREDRV.Submit; subscription not enabled for send as"
		});

		public static readonly SmtpResponse AmbiguousSubscription = new SmtpResponse("550", "5.6.2", new string[]
		{
			"STOREDRV.Submit; ambiguous send as subscription found"
		});

		public static readonly SmtpResponse InvalidSendAsProperties = new SmtpResponse("550", "5.6.2", new string[]
		{
			"STOREDRV.Submit; invalid send as properties"
		});

		public static readonly SmtpResponse UnrecognizedSendAsMessage = new SmtpResponse("550", "5.6.2", new string[]
		{
			"STOREDRV.Submit; unrecognized send as message"
		});

		public static readonly SmtpResponse AbortResubmissionDueToContentChange = new SmtpResponse("550", "5.2.0", new string[]
		{
			"STOREDRV.Submit; shadow re-submission aborted due to content change"
		});

		public static readonly SmtpResponse UnableToDetermineTargetPublicFolderMailbox = new SmtpResponse("550", "5.8.1", new string[]
		{
			"STOREDRV.Deliver; unable to determine target mailbox for public folder recipient"
		});

		public static readonly SmtpResponse PublicFolderMailboxNotFound = new SmtpResponse("420", "4.8.1", new string[]
		{
			"STOREDRV.Deliver; public folder mailbox not found"
		});

		public static readonly SmtpResponse MissingMdbProperties = new SmtpResponse("532", "5.3.2", new string[]
		{
			"STOREDRV.Deliver; Missing or bad StoreDriver MDB properties"
		});

		public static readonly SmtpResponse NotResolvedRecipient = new SmtpResponse("432", "4.3.3", new string[]
		{
			"STOREDRV.Deliver; Recipient could not be resolved"
		});

		public static readonly SmtpResponse ExtendedPropertiesNotAvailable = new SmtpResponse("432", "4.3.3", new string[]
		{
			"STOREDRV.Deliver; Extended Properties not available"
		});

		public static readonly SmtpResponse DeliverAgentTransientFailure = new SmtpResponse("432", "4.2.0", new string[]
		{
			"STOREDRV.Deliver; Agent transient failure during message resubmission"
		});

		public static readonly SmtpResponse OutboundPoisonMessage = new SmtpResponse("550", "5.6.0", new string[]
		{
			"STOREDRV.Submit; message is treated as poison."
		});

		public static readonly SmtpResponse RecipientMailboxQuarantined = new SmtpResponse("550", "5.7.9", new string[]
		{
			"STOREDRV.Deliver; Recipient mailbox quarantined."
		});

		public static readonly SmtpResponse SenderMailboxQuarantined = new SmtpResponse("550", "5.7.10", new string[]
		{
			"STOREDRV.Submit; Sender mailbox quarantined."
		});

		public static readonly SmtpResponse PublicFolderSenderValidationFailed = new SmtpResponse("550", "5.7.1", new string[]
		{
			"STOREDRV.Deliver; Unable to determine the sender's permission on public folder"
		});

		public static readonly SmtpResponse ApprovalInvalidMessage = new SmtpResponse("550", "5.7.1", new string[]
		{
			"APPROVAL.InvalidContent; Invalid content."
		});

		public static readonly SmtpResponse ApprovalCannotReadExpiryPolicy = new SmtpResponse("550", "5.6.0", new string[]
		{
			"APPROVAL.InvalidExpiry; Cannot read expiry policy."
		});

		public static readonly SmtpResponse ApprovalUnAuthorizedMessage = new SmtpResponse("550", "5.7.1", new string[]
		{
			"APPROVAL.NotAuthorized; message cannot be delivered."
		});

		public static readonly SmtpResponse ApprovalDuplicateInitiation = new SmtpResponse("250", "2.1.5", new string[]
		{
			"APPROVAL.DuplicationInitiation; duplicate initiation ignored."
		});

		public static readonly SmtpResponse ApprovalDecisionSuccsess = new SmtpResponse("250", "2.1.5", new string[]
		{
			"APPROVAL.DecisionMarked; decision marked OK."
		});

		public static readonly SmtpResponse ApprovalUpdateSuccess = new SmtpResponse("250", "2.1.5", new string[]
		{
			"APPROVAL.ApprovalRequestUpdated; approval request updated successfully"
		});

		public static readonly SmtpResponse ApprovalNdrOofUpdateSuccess = new SmtpResponse("250", "2.1.5", new string[]
		{
			"APPROVAL.NdrOofUpdated; NDR or OOF update successfully"
		});

		public static readonly SmtpResponse ApprovalResubmitSuccess = new SmtpResponse("500", "5.0.0", new string[]
		{
			"APPROVAL.ModeratedMessageResubmitted; message resubmitted successfully"
		});

		public static readonly SmtpResponse MeetingMessageParkedSuccess = new SmtpResponse("250", "2.1.5", new string[]
		{
			"PARKINGLOT.MeetingMessageParked; Message was parked successfully."
		});

		public static readonly SmtpResponse UMPartnerMessageInvalidHeaders = new SmtpResponse("554", "5.6.5", new string[]
		{
			"UMPMSG.Deliver; missing or invalid x-ms-exchange-um-* headers."
		});

		public static readonly SmtpResponse UMPartnerMessageInvalidAttachments = new SmtpResponse("554", "5.6.5", new string[]
		{
			"UMPMSG.Deliver; missing or invalid attachments. The message must have 2 attachments of type audio/wav and text/xml."
		});

		public static readonly SmtpResponse UMFaxPartnerMessageInvalidAttachments = new SmtpResponse("554", "5.6.5", new string[]
		{
			"UMPMSG.Deliver; missing or invalid Fax attachments. The message must have only 1 attachment of type image/tiff."
		});

		public static readonly SmtpResponse UMPartnerMessageInvalidTranscriptionDocument = new SmtpResponse("554", "5.6.5", new string[]
		{
			"UMPMSG.Deliver; invalid transcription document. The transcription document failed schema validation."
		});

		public static readonly SmtpResponse UMPartnerMessageMessageArrivedTooLate = new SmtpResponse("554", "5.6.5", new string[]
		{
			"UMPMSG.Deliver; message arrived too late. A copy without transcription has already been delivered to the user."
		});

		public static readonly SmtpResponse UMPartnerMessageMessageSenderIdCheckFailed = new SmtpResponse("554", "5.6.5", new string[]
		{
			"UMPMSG.Deliver; sender ID check failed. Anonymous x-ms-exchange-um-partner messages must pass the Sender ID check."
		});

		public static readonly SmtpResponse UMPartnerMessageMessageInvalidPartnerDomain = new SmtpResponse("554", "5.6.5", new string[]
		{
			"UMPMSG.Deliver; unrecognized partner domain. The recipient's UMMailboxPolicy does not allow the sender domain."
		});

		public static readonly SmtpResponse UMPartnerMessageMessageCannotReadPolicy = new SmtpResponse("554", "5.6.5", new string[]
		{
			"UMPMSG.Deliver; cannot read the recipient's UMMailboxPolicy."
		});

		public static readonly SmtpResponse UMPartnerMessageMessageCannotReadRecipient = new SmtpResponse("554", "5.6.5", new string[]
		{
			"UMPMSG.Deliver; cannot read the recipient object."
		});

		public static readonly SmtpResponse UMTranscriptionRequestSuccess = new SmtpResponse("250", "2.1.5", new string[]
		{
			"UMPMSG.Deliver; transcription request processed succesfully."
		});

		public static readonly SmtpResponse TransportRulesAgentDeferCountExceeded = new SmtpResponse("550", "5.7.1", new string[]
		{
			"TRA: Policy processing failed: transport rule evaluation timed out."
		});

		public static readonly SmtpResponse TransportRuleReject = new SmtpResponse("550", "5.7.1", new string[]
		{
			"TRANSPORT.RULES.RejectMessage; the message was rejected by organization policy"
		});

		public static readonly SmtpResponse SenderRecipientThrottlingMessageRejected = new SmtpResponse("450", "4.5.0", new string[]
		{
			"Message rejected.  Excessive message rate from sender to recipient."
		});

		public static readonly SmtpResponse MessageTooOld = new SmtpResponse("550", "4.4.7", new string[]
		{
			"Message dropped. Message too old"
		});

		public static readonly SmtpResponse MessageNotActive = new SmtpResponse("550", "4.4.7", new string[]
		{
			"Message dropped. Message not active"
		});

		public static readonly SmtpResponse PoisonMessageExpired = new SmtpResponse("550", "4.4.7", new string[]
		{
			"Message dropped. Poison message too old"
		});

		public static readonly Dictionary<SmtpResponse, LocalizedString> EnhancedTextGetter = new Dictionary<SmtpResponse, LocalizedString>(new AckReason.SmtpResponseComparer())
		{
			{
				AckReason.MessageTooLargeForHighPriority,
				SystemMessages.DSNEnhanced_5_2_3_QUEUE_Priority
			},
			{
				AckReason.MessageTooLargeForReceiver,
				SystemMessages.DSNEnhanced_5_2_3_RESOLVER_RST_RecipSizeLimit
			},
			{
				AckReason.MessageTooLargeForSender,
				SystemMessages.DSNEnhanced_5_2_3_RESOLVER_RST_SendSizeLimit_Sender
			},
			{
				AckReason.MessageTooLargeForOrganization,
				SystemMessages.DSNEnhanced_5_2_3_RESOLVER_RST_SendSizeLimit_Org
			},
			{
				AckReason.MessageTooLargeForDistributionList,
				SystemMessages.DSNEnhanced_5_2_3_RESOLVER_RST_RecipSizeLimit_DL
			},
			{
				AckReason.NoNextHop,
				SystemMessages.DSNEnhanced_5_4_4_ROUTING_NoNextHop
			},
			{
				AckReason.DnsNonExistentDomain,
				SystemMessages.DSNEnhanced_5_4_4_SMTPSEND_DNS_NonExistentDomain
			},
			{
				AckReason.ApprovalUnAuthorizedMessage,
				SystemMessages.DSNEnhanced_5_7_1_APPROVAL_NotAuthorized
			},
			{
				AckReason.NotAuthenticated,
				SystemMessages.DSNEnhanced_5_7_1_RESOLVER_RST_AuthRequired
			},
			{
				AckReason.RecipientPermissionRestricted,
				SystemMessages.DSNEnhanced_5_7_1_RESOLVER_RST_NotAuthorized
			},
			{
				AckReason.RecipientPermissionRestrictedToGroup,
				SystemMessages.DSNEnhanced_5_7_1_RESOLVER_RST_NotAuthorizedToGroup
			},
			{
				AckReason.ModerationReencrptionFailed,
				SystemMessages.DSNEnhanced_5_6_0_RESOLVER_MT_ModerationReencrptionFailed
			},
			{
				AckReason.DLExpansionBlockedNeedsSenderRestrictions,
				SystemMessages.DSNEnhanced_5_7_1_RESOLVER_RST_DLNeedsSenderRestrictions
			},
			{
				AckReason.TransportRuleReject,
				SystemMessages.DSNEnhanced_5_7_1_TRANSPORT_RULES_RejectMessage
			}
		};

		internal static readonly SmtpResponse MailboxDatabaseMoved = new SmtpResponse("421", "4.2.1", new string[]
		{
			"STOREDRV.Deliver; mailbox database has moved."
		});

		internal class SmtpResponseComparer : IEqualityComparer<SmtpResponse>
		{
			public bool Equals(SmtpResponse x, SmtpResponse y)
			{
				if (string.Equals(x.StatusCode, y.StatusCode, StringComparison.Ordinal) && string.Equals(x.EnhancedStatusCode, y.EnhancedStatusCode, StringComparison.Ordinal) && x.StatusText != null && y.StatusText != null && x.StatusText.Length > 0 && y.StatusText.Length > 0 && !string.IsNullOrEmpty(x.StatusText[0]) && !string.IsNullOrEmpty(y.StatusText[0]))
				{
					int num = x.StatusText[0].IndexOf(';');
					int num2 = y.StatusText[0].IndexOf(';');
					return num == num2 && num > 0 && string.Equals(x.StatusText[0].Substring(0, num), y.StatusText[0].Substring(0, num2), StringComparison.Ordinal);
				}
				return false;
			}

			public int GetHashCode(SmtpResponse smtpResponse)
			{
				int num = smtpResponse.GetHashCode();
				if (smtpResponse.StatusText != null && smtpResponse.StatusText.Length > 0 && !string.IsNullOrEmpty(smtpResponse.StatusText[0]))
				{
					int num2 = smtpResponse.StatusText[0].IndexOf(';');
					if (num2 > 0)
					{
						num ^= smtpResponse.StatusText[0].Substring(0, num2).GetHashCode();
					}
				}
				return num;
			}

			private const char SemiColon = ';';
		}
	}
}
