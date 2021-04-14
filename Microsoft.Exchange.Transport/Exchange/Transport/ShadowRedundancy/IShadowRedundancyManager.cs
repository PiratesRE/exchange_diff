using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal interface IShadowRedundancyManager : IShadowRedundancyManagerFacade
	{
		Guid DatabaseId { get; }

		string DatabaseIdForTransmit { get; }

		IShadowRedundancyConfigurationSource Configuration { get; }

		object SyncQueues { get; }

		IPrimaryServerInfoMap PrimaryServerInfos { get; }

		ShadowRedundancyEventLogger EventLogger { get; }

		void Start(bool initiallyPaused, ServiceState targetRunningState);

		void Stop();

		void Pause();

		void Continue();

		TransportMailItem MoveUndeliveredRecipientsToNewClone(TransportMailItem mailItem);

		void AddDiagnosticInfoTo(XElement componentElement, bool showBasic, bool showVerbose);

		bool IsVersion(ShadowRedundancyCompatibilityVersion version);

		string GetShadowContextForInboundSession();

		string GetShadowContext(IEhloOptions ehloOptions, NextHopSolutionKey key);

		bool ShouldShadowMailItem(IReadOnlyMailItem mailItem);

		IShadowSession GetShadowSession(ISmtpInSession inSession, bool isBdat);

		void SetSmtpInEhloOptions(EhloOptions ehloOptions, ReceiveConnector receiveConnector);

		void SetDelayedAckCompletedHandler(DelayedAckItemHandler delayedAckCompleted);

		void ApplyDiscardEvents(string serverFqdn, NextHopSolutionKey solutionKey, ICollection<string> messageIds, out int invalid, out int notFound);

		string[] QueryDiscardEvents(string queryingServerContext, int maxDiscardEvents);

		void NotifyBootLoaderDone();

		void NotifyShuttingDown();

		void NotifyMailItemBifurcated(string shadowServerContext, string shadowServerDiscardId);

		void NotifyMailItemPreEnqueuing(IReadOnlyMailItem mailItem, TransportMessageQueue queue);

		void NotifyMailItemDeferred(IReadOnlyMailItem mailItem, TransportMessageQueue queue, DateTime deferUntil);

		void LinkSideEffectMailItem(IReadOnlyMailItem originalMailItem, TransportMailItem sideEffectMailItem);

		void NotifyMailItemDelivered(TransportMailItem mailItem);

		void NotifyMailItemPoison(TransportMailItem mailItem);

		void NotifyMailItemReleased(TransportMailItem mailItem);

		void NotifyPrimaryServerState(string serverFqdn, string state, ShadowRedundancyCompatibilityVersion version);

		void NotifyServerViolatedSmtpContract(string serverFqdnOrContext);

		void EnqueueDelayedAckMessage(Guid shadowMessageId, object state, DateTime startTime, TimeSpan maxDelayDuration);

		bool ShouldDelayAck();

		bool ShouldSmtpOutSendXQDiscard(string serverFqdn);

		bool ShouldSmtpOutSendXShadow(Permission sessionPermissions, DeliveryType nextHopDeliveryType, IEhloOptions advertisedEhloOptions, SmtpSendConnectorConfig connector);

		bool EnqueueShadowMailItem(TransportMailItem mailItem, NextHopSolution originalMessageSolution, string primaryServer, bool shadowedMailItem, AckStatus ackStatus);

		bool EnqueuePeerShadowMailItem(TransportMailItem mailItem, string primaryServer);

		void EnqueueDiscardPendingMailItem(TransportMailItem mailItem);

		void UpdateQueues();

		ShadowMessageQueue GetQueue(NextHopSolutionKey key);

		ShadowMessageQueue[] GetQueueArray();

		TransportMailItem GetMailItem(long mailItemId);

		void ProcessMailItemOnStartup(TransportMailItem mailItem);

		void VisitMailItems(Func<TransportMailItem, bool> visitor);

		List<ShadowMessageQueue> FindByQueueIdentity(QueueIdentity queueIdentity);

		void LoadQueue(RoutedQueueBase queueStorage);

		void EvaluateHeartbeatAttempt(NextHopSolutionKey key, out bool sendHeartbeat, out bool abortHeartbeat);

		void NotifyHeartbeatConfigChanged(NextHopSolutionKey key, out bool abortHeartbeat);

		void NotifyHeartbeatRetry(NextHopSolutionKey key, out bool abortHeartbeat);

		void NotifyHeartbeatStatus(string serverFqdn, NextHopSolutionKey solutionKey, bool heartbeatSucceeded);

		IEnumerable<PrimaryServerInfo> GetPrimaryServersForMessageResubmission();
	}
}
