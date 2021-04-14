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
	internal sealed class MessageTrackingLogWrapper : IMessageTrackingLog
	{
		public void Start()
		{
			MessageTrackingLog.Start();
		}

		public void Start(string logFilePrefix)
		{
			MessageTrackingLog.Start(logFilePrefix);
		}

		public void Stop()
		{
			MessageTrackingLog.Stop();
		}

		public void TrackLoadedMessage(MessageTrackingSource trackingSource, MessageTrackingEvent trackingEvent, TransportMailItem mailItem)
		{
			MessageTrackingLog.TrackLoadedMessage(trackingSource, trackingEvent, mailItem);
		}

		public void TrackPoisonMessage(MessageTrackingSource source, IReadOnlyMailItem mailItem)
		{
			MessageTrackingLog.TrackPoisonMessage(source, mailItem);
		}

		public void TrackPoisonMessage(MessageTrackingSource source, MsgTrackPoisonInfo msgTrackingPoisonInfo)
		{
			MessageTrackingLog.TrackPoisonMessage(source, msgTrackingPoisonInfo);
		}

		public void TrackPoisonMessage(MessageTrackingSource source, TransportMailItem mailItem, string messageId, MsgTrackReceiveInfo msgTrackInfo)
		{
			MessageTrackingLog.TrackPoisonMessage(source, mailItem, messageId, msgTrackInfo);
		}

		public void TrackReceiveForApprovalRelease(TransportMailItem mailItem, string approver, string initiationMessageId)
		{
			MessageTrackingLog.TrackReceiveForApprovalRelease(mailItem, approver, initiationMessageId);
		}

		public void TrackReceiveByAgent(ITransportMailItemFacade mailItem, string sourceContext, string connectorId, long? relatedMailItemId)
		{
			MessageTrackingLog.TrackReceiveByAgent(mailItem, sourceContext, connectorId, relatedMailItemId);
		}

		public void TrackReceive(MessageTrackingSource source, TransportMailItem mailItem, MsgTrackReceiveInfo msgTrackInfo)
		{
			MessageTrackingLog.TrackReceive(source, mailItem, msgTrackInfo);
		}

		public void TrackReceive(MessageTrackingSource source, TransportMailItem mailItem, string messageId, MsgTrackReceiveInfo msgTrackInfo)
		{
			MessageTrackingLog.TrackReceive(source, mailItem, messageId, msgTrackInfo);
		}

		public void TrackNotify(MsgTrackMapiSubmitInfo msgTrackInfo, bool isShadowSubmission)
		{
			MessageTrackingLog.TrackNotify(msgTrackInfo, isShadowSubmission);
		}

		public void TrackMapiSubmit(MsgTrackMapiSubmitInfo msgTrackInfo)
		{
			MessageTrackingLog.TrackMapiSubmit(msgTrackInfo);
		}

		public void TrackResubmitCancelled(MessageTrackingSource source, IReadOnlyMailItem mailItem, ICollection recipients, SmtpResponse response, LatencyFormatter latencyFormatter)
		{
			MessageTrackingLog.TrackResubmitCancelled(source, mailItem, recipients, response, latencyFormatter);
		}

		public void TrackDuplicateDelivery(MessageTrackingSource source, IReadOnlyMailItem mailItem, ICollection recipients, ICollection recipientsData, string clientHostname, string serverName, LatencyFormatter latencyFormatter, string sourceContext, List<KeyValuePair<string, string>> extraEventData)
		{
			MessageTrackingLog.TrackDuplicateDelivery(source, mailItem, recipients, recipientsData, clientHostname, serverName, latencyFormatter, sourceContext, extraEventData);
		}

		public void TrackDelivered(MessageTrackingSource source, IReadOnlyMailItem mailItem, ICollection recipients, ICollection recipientsData, string clientHostname, string serverName, LatencyFormatter latencyFormatter, string sourceContext, List<KeyValuePair<string, string>> extraEventData)
		{
			MessageTrackingLog.TrackDelivered(source, mailItem, recipients, recipientsData, clientHostname, serverName, latencyFormatter, sourceContext, extraEventData);
		}

		public void TrackExpiredMessageDropped(MessageTrackingSource source, IReadOnlyMailItem mailItem, ICollection recipients, SmtpResponse response)
		{
			MessageTrackingLog.TrackExpiredMessageDropped(source, mailItem, recipients, response);
		}

		public void TrackProcessed(MessageTrackingSource source, IReadOnlyMailItem mailItem, ICollection recipients, ICollection recipientsSource, LatencyFormatter latencyFormatter, string sourceContext, List<KeyValuePair<string, string>> extraEventData)
		{
			MessageTrackingLog.TrackProcessed(source, mailItem, recipients, recipientsSource, latencyFormatter, sourceContext, extraEventData);
		}

		public void TrackRecipientAdd(MessageTrackingSource source, TransportMailItem mailItem, RoutingAddress recipEmail, RecipientP2Type? recipientType, string agentName)
		{
			MessageTrackingLog.TrackRecipientAdd(source, mailItem, recipEmail, recipientType, agentName);
		}

		public void TrackRecipientAddByAgent(ITransportMailItemFacade mailItem, string recipEmail, RecipientP2Type recipientType, string agentName)
		{
			MessageTrackingLog.TrackRecipientAddByAgent(mailItem, recipEmail, recipientType, agentName);
		}

		public void TrackFailedRecipients(MessageTrackingSource source, string sourceContext, IReadOnlyMailItem mailItem, string relatedRecipientAddress, ICollection<MailRecipient> recipients, SmtpResponse smtpResponse, LatencyFormatter latencyFormatter)
		{
			MessageTrackingLog.TrackFailedRecipients(source, sourceContext, mailItem, relatedRecipientAddress, recipients, smtpResponse, latencyFormatter);
		}

		public void TrackRecipientFail(MessageTrackingSource source, TransportMailItem mailItem, RoutingAddress recipEmail, SmtpResponse smtpResponse, string sourceContext, LatencyFormatter latencyFormatter)
		{
			MessageTrackingLog.TrackRecipientFail(source, mailItem, recipEmail, smtpResponse, sourceContext, latencyFormatter);
		}

		public void TrackRecipientDrop(MessageTrackingSource source, TransportMailItem mailItem, RoutingAddress recipEmail, SmtpResponse smtpResponse, string sourceContext, LatencyFormatter latencyFormatter)
		{
			MessageTrackingLog.TrackRecipientDrop(source, mailItem, recipEmail, smtpResponse, sourceContext, latencyFormatter);
		}

		public void TrackAgentInfo(TransportMailItem mailItem)
		{
			MessageTrackingLog.TrackAgentInfo(mailItem);
		}

		public void TrackPoisonMessageDeleted(MessageTrackingSource source, string sourceContext, IReadOnlyMailItem item)
		{
			MessageTrackingLog.TrackPoisonMessageDeleted(source, sourceContext, item);
		}

		public void TrackRejectCommand(MessageTrackingSource source, string sourceContext, AckDetails ackDetails, SmtpResponse smtpResponse)
		{
			MessageTrackingLog.TrackRejectCommand(source, sourceContext, ackDetails, smtpResponse);
		}

		public void TrackRelayedAndFailed(MessageTrackingSource source, IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipients, AckDetails ackDetails)
		{
			MessageTrackingLog.TrackRelayedAndFailed(source, mailItem, recipients, ackDetails);
		}

		public void TrackRelayedAndFailed(MessageTrackingSource source, string sourceContext, IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipients, AckDetails ackDetails, SmtpResponse smtpResponse, LatencyFormatter latencyFormatter)
		{
			MessageTrackingLog.TrackRelayedAndFailed(source, sourceContext, mailItem, recipients, ackDetails, smtpResponse, latencyFormatter);
		}

		public void TrackDSN(TransportMailItem dsnMailItem, MsgTrackDSNInfo dsnInfo)
		{
			MessageTrackingLog.TrackDSN(dsnMailItem, dsnInfo);
		}

		public void TrackAgentGeneratedMessageRejected(MessageTrackingSource source, bool loopDetectionEnabled, IReadOnlyMailItem mailItem)
		{
			MessageTrackingLog.TrackAgentGeneratedMessageRejected(source, loopDetectionEnabled, mailItem);
		}

		public void TrackBadmail(MessageTrackingSource source, MsgTrackReceiveInfo msgTrackInfo, IReadOnlyMailItem mailItem, string badmailReason)
		{
			MessageTrackingLog.TrackBadmail(source, msgTrackInfo, mailItem, badmailReason);
		}

		public void TrackDefer(MessageTrackingSource source, IReadOnlyMailItem mailItem, string sourceContext)
		{
			MessageTrackingLog.TrackDefer(source, mailItem, sourceContext);
		}

		public void TrackDefer(MessageTrackingSource source, string messageTrackingSourceContext, IReadOnlyMailItem mailItem, IEnumerable<MailRecipient> recipientsToTrack, AckDetails ackDetails)
		{
			MessageTrackingLog.TrackDefer(source, messageTrackingSourceContext, mailItem, recipientsToTrack, ackDetails);
		}

		public void TrackThrottle(MessageTrackingSource source, MsgTrackMapiSubmitInfo msgTrackInfo, string senderAddress, string messageId)
		{
			MessageTrackingLog.TrackThrottle(source, msgTrackInfo, senderAddress, messageId);
		}

		public void TrackThrottle(MessageTrackingSource source, MsgTrackMapiSubmitInfo msgTrackInfo, string senderAddress, string messageId, MailDirectionality direction)
		{
			MessageTrackingLog.TrackThrottle(source, msgTrackInfo, senderAddress, messageId, direction);
		}

		public void TrackThrottle(MessageTrackingSource source, IReadOnlyMailItem mailItem, IPAddress serverIPAddress, string sourceContext, string reference, ProxyAddress recipient, string recipientData)
		{
			MessageTrackingLog.TrackThrottle(source, mailItem, serverIPAddress, sourceContext, reference, recipient, recipientData);
		}

		public void TrackResolve(MessageTrackingSource source, IReadOnlyMailItem mailItem, MsgTrackResolveInfo msgTrackInfo)
		{
			MessageTrackingLog.TrackResolve(source, mailItem, msgTrackInfo);
		}

		public void TrackExpand<T>(MessageTrackingSource source, IReadOnlyMailItem mailItem, MsgTrackExpandInfo msgTrackInfo, ICollection<T> recipients)
		{
			MessageTrackingLog.TrackExpand<T>(source, mailItem, msgTrackInfo, recipients);
		}

		public void TrackExpandEvent<T>(MessageTrackingSource source, IReadOnlyMailItem mailItem, MsgTrackExpandInfo msgTrackInfo, ICollection<T> recipients, MessageTrackingEvent trackingEvent)
		{
			MessageTrackingLog.TrackExpandEvent<T>(source, mailItem, msgTrackInfo, recipients, trackingEvent);
		}

		public void TrackRedirect(MessageTrackingSource source, IReadOnlyMailItem mailItem, MsgTrackRedirectInfo msgTrackInfo)
		{
			MessageTrackingLog.TrackRedirect(source, mailItem, msgTrackInfo);
		}

		public void TrackRedirectEvent(MessageTrackingSource source, IReadOnlyMailItem mailItem, MsgTrackRedirectInfo msgTrackInfo, MessageTrackingEvent redirectEvent)
		{
			MessageTrackingLog.TrackRedirectEvent(source, mailItem, msgTrackInfo, redirectEvent);
		}

		public void TrackRedirectToDomain(MessageTrackingSource source, IReadOnlyMailItem mailItem, MsgTrackRedirectInfo msgTrackInfo, MailRecipient mailRecipient)
		{
			MessageTrackingLog.TrackRedirectToDomain(source, mailItem, msgTrackInfo, mailRecipient);
		}

		public void TrackTransfer(MessageTrackingSource source, IReadOnlyMailItem mailItem, long relatedMailItemId, string sourceContext)
		{
			MessageTrackingLog.TrackTransfer(source, mailItem, relatedMailItemId, sourceContext);
		}

		public void TrackHighAvailabilityRedirect(MessageTrackingSource source, IReadOnlyMailItem mailItem, ICollection<MailRecipient> redirectedRecipients, string sourceContext)
		{
			MessageTrackingLog.TrackHighAvailabilityRedirect(source, mailItem, redirectedRecipients, sourceContext);
		}

		public void TrackHighAvailabilityRedirect(MessageTrackingSource source, IReadOnlyMailItem mailItem, string sourceContext)
		{
			MessageTrackingLog.TrackHighAvailabilityRedirect(source, mailItem, sourceContext);
		}

		public void TrackHighAvailabilityRedirectFail(MessageTrackingSource source, IReadOnlyMailItem mailItem, string sourceContext)
		{
			MessageTrackingLog.TrackHighAvailabilityRedirectFail(source, mailItem, sourceContext);
		}

		public void TrackHighAvailabilityReceive(MessageTrackingSource source, string primaryServerFqdn, IReadOnlyMailItem mailItem)
		{
			MessageTrackingLog.TrackHighAvailabilityReceive(source, primaryServerFqdn, mailItem);
		}

		public void TrackHighAvailabilityDiscard(MessageTrackingSource source, IReadOnlyMailItem mailItem, string reason)
		{
			MessageTrackingLog.TrackHighAvailabilityDiscard(source, mailItem, reason);
		}

		public void TrackResubmit(MessageTrackingSource source, TransportMailItem newMailItem, IReadOnlyMailItem originalMailItem, string sourceContext)
		{
			MessageTrackingLog.TrackResubmit(source, newMailItem, originalMailItem, sourceContext);
		}

		public void TrackInitMessageCreated(MessageTrackingSource source, ICollection<MailRecipient> moderatedRecipients, IReadOnlyMailItem originalMailItem, TransportMailItem initiationMailItem, string initiationMessageIdentifier, string sourceContext)
		{
			MessageTrackingLog.TrackInitMessageCreated(source, moderatedRecipients, originalMailItem, initiationMailItem, initiationMessageIdentifier, sourceContext);
		}

		public void TrackModeratorsAllNdr(MessageTrackingSource source, string initiationMessageId, string originalMessageId, string originalSenderAddress, ICollection<string> recipientAddresses, OrganizationId organizationId)
		{
			MessageTrackingLog.TrackModeratorsAllNdr(source, initiationMessageId, originalMessageId, originalSenderAddress, recipientAddresses, organizationId);
		}

		public void TrackModeratorExpired(MessageTrackingSource source, string initiationMessageId, string originalMessageId, string originalSenderAddress, ICollection<string> recipientAddresses, OrganizationId organizationId, bool isNotificationSent)
		{
			MessageTrackingLog.TrackModeratorExpired(source, initiationMessageId, originalMessageId, originalSenderAddress, recipientAddresses, organizationId, isNotificationSent);
		}

		public void TrackModeratorDecision(MessageTrackingSource source, string initiationMessageId, string originalMessageId, string originalSenderAddress, ICollection<string> recipientAddresses, bool isApproved, OrganizationId organizationId)
		{
			MessageTrackingLog.TrackModeratorDecision(source, initiationMessageId, originalMessageId, originalSenderAddress, recipientAddresses, isApproved, organizationId);
		}

		public void TrackMeetingMessage(string internetMessageID, string clientName, OrganizationId organizationID, List<KeyValuePair<string, string>> extraEventData)
		{
			MessageTrackingLog.TrackMeetingMessage(internetMessageID, clientName, organizationID, extraEventData);
		}

		public void Configure(Server serverConfig)
		{
			MessageTrackingLog.Configure(serverConfig);
		}

		public void FlushBuffer()
		{
			MessageTrackingLog.FlushBuffer();
		}
	}
}
