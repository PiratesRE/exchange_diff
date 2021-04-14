using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport
{
	internal interface IReadOnlyMailItem : ISystemProbeTraceable
	{
		ADRecipientCache<TransportMiniRecipient> ADRecipientCache { get; }

		string Auth { get; }

		MultilevelAuthMechanism AuthMethod { get; }

		BodyType BodyType { get; }

		DateTime DateReceived { get; }

		DeferReason DeferReason { get; }

		MailDirectionality Directionality { get; }

		DsnFormat DsnFormat { get; }

		DsnParameters DsnParameters { get; }

		bool SuppressBodyInDsn { get; set; }

		string EnvId { get; }

		IReadOnlyExtendedPropertyCollection ExtendedProperties { get; }

		TimeSpan ExtensionToExpiryDuration { get; }

		RoutingAddress From { get; }

		string HeloDomain { get; }

		string InternetMessageId { get; }

		Guid NetworkMessageId { get; }

		bool IsActive { get; }

		bool IsHeartbeat { get; }

		LatencyTracker LatencyTracker { get; }

		byte[] LegacyXexch50Blob { get; }

		IEnumerable<string> LockReasonHistory { get; }

		EmailMessage Message { get; }

		string Oorg { get; }

		Guid ExternalOrganizationId { get; }

		string ExoAccountForest { get; }

		string ExoTenantContainer { get; }

		bool IsProbe { get; }

		string ProbeName { get; }

		bool PersistProbeTrace { get; }

		RiskLevel RiskLevel { get; }

		DeliveryPriority Priority { get; }

		string PrioritizationReason { get; }

		MimeDocument MimeDocument { get; }

		string MimeFrom { get; }

		RoutingAddress MimeSender { get; }

		long MimeSize { get; }

		OrganizationId OrganizationId { get; }

		RoutingAddress OriginalFrom { get; }

		int PoisonCount { get; }

		int PoisonForRemoteCount { get; }

		bool IsPoison { get; }

		string ReceiveConnectorName { get; }

		IReadOnlyMailRecipientCollection Recipients { get; }

		long RecordId { get; }

		bool RetryDeliveryIfRejected { get; }

		MimePart RootPart { get; }

		int Scl { get; }

		Guid ShadowMessageId { get; }

		string ShadowServerContext { get; }

		string ShadowServerDiscardId { get; }

		IPAddress SourceIPAddress { get; }

		string Subject { get; }

		PerTenantTransportSettings TransportSettings { get; }

		LazyBytes FastIndexBlob { get; }

		void CacheTransportSettings();

		bool IsJournalReport();

		bool IsPfReplica();

		bool IsShadowed();

		bool IsDelayedAck();

		TransportMailItem NewCloneWithoutRecipients(bool shareRecipientCache);

		Stream OpenMimeReadStream();

		Stream OpenMimeReadStream(bool downConvert);

		void TrackSuccessfulConnectLatency(LatencyComponent connectComponent);

		void AddDsnParameters(string key, object value);

		void IncrementPoisonForRemoteCount();
	}
}
