using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Logging.MessageTracking
{
	internal interface IMessageTrackingLog
	{
		void Start();

		void Start(string logFilePrefix);

		void Stop();

		void TrackLoadedMessage(MessageTrackingSource trackingSource, MessageTrackingEvent trackingEvent, TransportMailItem mailItem);

		void TrackPoisonMessage(MessageTrackingSource source, IReadOnlyMailItem mailItem);

		void TrackPoisonMessage(MessageTrackingSource source, MsgTrackPoisonInfo msgTrackingPoisonInfo);

		void TrackPoisonMessage(MessageTrackingSource source, TransportMailItem mailItem, string messageId, MsgTrackReceiveInfo msgTrackInfo);

		void TrackReceiveForApprovalRelease(TransportMailItem mailItem, string approver, string initiationMessageId);

		void TrackReceiveByAgent(ITransportMailItemFacade mailItem, string sourceContext, string connectorId, long? relatedMailItemId);

		void TrackReceive(MessageTrackingSource source, TransportMailItem mailItem, MsgTrackReceiveInfo msgTrackInfo);

		void TrackReceive(MessageTrackingSource source, TransportMailItem mailItem, string messageId, MsgTrackReceiveInfo msgTrackInfo);

		void TrackNotify(MsgTrackMapiSubmitInfo msgTrackInfo, bool isShadowSubmission);

		void TrackMapiSubmit(MsgTrackMapiSubmitInfo msgTrackInfo);

		void TrackResubmitCancelled(MessageTrackingSource source, IReadOnlyMailItem mailItem, ICollection recipients, SmtpResponse response, LatencyFormatter latencyFormatter);

		void TrackDuplicateDelivery(MessageTrackingSource source, IReadOnlyMailItem mailItem, ICollection recipients, ICollection recipientsData, string clientHostname, string serverName, LatencyFormatter latencyFormatter, string sourceContext, List<KeyValuePair<string, string>> extraEventData);

		void TrackDelivered(MessageTrackingSource source, IReadOnlyMailItem mailItem, ICollection recipients, ICollection recipientsData, string clientHostname, string serverName, LatencyFormatter latencyFormatter, string sourceContext, List<KeyValuePair<string, string>> extraEventData);

		void TrackExpiredMessageDropped(MessageTrackingSource source, IReadOnlyMailItem mailItem, ICollection recipients, SmtpResponse response);

		void TrackProcessed(MessageTrackingSource source, IReadOnlyMailItem mailItem, ICollection recipients, ICollection recipientsSource, LatencyFormatter latencyFormatter, string sourceContext, List<KeyValuePair<string, string>> extraEventData);

		void TrackRecipientAdd(MessageTrackingSource source, TransportMailItem mailItem, RoutingAddress recipEmail, RecipientP2Type? recipientType, string agentName);

		void TrackRecipientAddByAgent(ITransportMailItemFacade mailItem, string recipEmail, RecipientP2Type recipientType, string agentName);

		void TrackFailedRecipients(MessageTrackingSource source, string sourceContext, IReadOnlyMailItem mailItem, string relatedRecipientAddress, ICollection<MailRecipient> recipients, SmtpResponse smtpResponse, LatencyFormatter latencyFormatter);

		void TrackRecipientFail(MessageTrackingSource source, TransportMailItem mailItem, RoutingAddress recipEmail, SmtpResponse smtpResponse, string sourceContext, LatencyFormatter latencyFormatter);

		void TrackRecipientDrop(MessageTrackingSource source, TransportMailItem mailItem, RoutingAddress recipEmail, SmtpResponse smtpResponse, string sourceContext, LatencyFormatter latencyFormatter);

		void TrackAgentInfo(TransportMailItem mailItem);

		void TrackPoisonMessageDeleted(MessageTrackingSource source, string sourceContext, IReadOnlyMailItem item);

		void TrackRejectCommand(MessageTrackingSource source, string sourceContext, AckDetails ackDetails, SmtpResponse smtpResponse);

		void TrackRelayedAndFailed(MessageTrackingSource source, IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipients, AckDetails ackDetails);

		void TrackRelayedAndFailed(MessageTrackingSource source, string sourceContext, IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipients, AckDetails ackDetails, SmtpResponse smtpResponse, LatencyFormatter latencyFormatter);

		void TrackDSN(TransportMailItem dsnMailItem, MsgTrackDSNInfo dsnInfo);

		void TrackAgentGeneratedMessageRejected(MessageTrackingSource source, bool loopDetectionEnabled, IReadOnlyMailItem mailItem);

		void TrackBadmail(MessageTrackingSource source, MsgTrackReceiveInfo msgTrackInfo, IReadOnlyMailItem mailItem, string badmailReason);

		void TrackDefer(MessageTrackingSource source, IReadOnlyMailItem mailItem, string sourceContext);

		void TrackDefer(MessageTrackingSource source, string messageTrackingSourceContext, IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipientsToTrack, AckDetails ackDetails);

		void TrackThrottle(MessageTrackingSource source, MsgTrackMapiSubmitInfo msgTrackInfo, string senderAddress, string messageId);

		void TrackThrottle(MessageTrackingSource source, MsgTrackMapiSubmitInfo msgTrackInfo, string senderAddress, string messageId, MailDirectionality direction);

		void TrackThrottle(MessageTrackingSource source, IReadOnlyMailItem mailItem, IPAddress serverIPAddress, string sourceContext, string reference, ProxyAddress recipient, string recipientData);

		void TrackResolve(MessageTrackingSource source, IReadOnlyMailItem mailItem, MsgTrackResolveInfo msgTrackInfo);

		void TrackExpand<T>(MessageTrackingSource source, IReadOnlyMailItem mailItem, MsgTrackExpandInfo msgTrackInfo, ICollection<T> recipients);

		void TrackExpandEvent<T>(MessageTrackingSource source, IReadOnlyMailItem mailItem, MsgTrackExpandInfo msgTrackInfo, ICollection<T> recipients, MessageTrackingEvent trackingEvent);

		void TrackRedirect(MessageTrackingSource source, IReadOnlyMailItem mailItem, MsgTrackRedirectInfo msgTrackInfo);

		void TrackRedirectEvent(MessageTrackingSource source, IReadOnlyMailItem mailItem, MsgTrackRedirectInfo msgTrackInfo, MessageTrackingEvent redirectEvent);

		void TrackRedirectToDomain(MessageTrackingSource source, IReadOnlyMailItem mailItem, MsgTrackRedirectInfo msgTrackInfo, MailRecipient mailRecipient);

		void TrackTransfer(MessageTrackingSource source, IReadOnlyMailItem mailItem, long relatedMailItemId, string sourceContext);

		void TrackHighAvailabilityRedirect(MessageTrackingSource source, IReadOnlyMailItem mailItem, ICollection<MailRecipient> redirectedRecipients, string sourceContext);

		void TrackHighAvailabilityRedirect(MessageTrackingSource source, IReadOnlyMailItem mailItem, string sourceContext);

		void TrackHighAvailabilityRedirectFail(MessageTrackingSource source, IReadOnlyMailItem mailItem, string sourceContext);

		void TrackHighAvailabilityReceive(MessageTrackingSource source, string primaryServerFqdn, IReadOnlyMailItem mailItem);

		void TrackHighAvailabilityDiscard(MessageTrackingSource source, IReadOnlyMailItem mailItem, string reason);

		void TrackResubmit(MessageTrackingSource source, TransportMailItem newMailItem, IReadOnlyMailItem originalMailItem, string sourceContext);

		void TrackInitMessageCreated(MessageTrackingSource source, ICollection<MailRecipient> moderatedRecipients, IReadOnlyMailItem originalMailItem, TransportMailItem initiationMailItem, string initiationMessageIdentifier, string sourceContext);

		void TrackModeratorsAllNdr(MessageTrackingSource source, string initiationMessageId, string originalMessageId, string originalSenderAddress, ICollection<string> recipientAddresses, OrganizationId organizationId);

		void TrackModeratorExpired(MessageTrackingSource source, string initiationMessageId, string originalMessageId, string originalSenderAddress, ICollection<string> recipientAddresses, OrganizationId organizationId, bool isNotificationSent);

		void TrackModeratorDecision(MessageTrackingSource source, string initiationMessageId, string originalMessageId, string originalSenderAddress, ICollection<string> recipientAddresses, bool isApproved, OrganizationId organizationId);

		void TrackMeetingMessage(string internetMessageID, string clientName, OrganizationId organizationID, List<KeyValuePair<string, string>> extraEventData);

		void Configure(Server serverConfig);

		void FlushBuffer();
	}
}
