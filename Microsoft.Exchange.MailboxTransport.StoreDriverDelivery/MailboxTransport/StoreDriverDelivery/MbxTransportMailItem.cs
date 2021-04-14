using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class MbxTransportMailItem : IReadOnlyMailItem, ISystemProbeTraceable, IReadOnlyMailRecipientCollection, IEnumerable<MailRecipient>, IEnumerable
	{
		public MbxTransportMailItem(TransportMailItem mailItem)
		{
			this.mailItem = mailItem;
			this.Initialize();
		}

		public int MailItemRecipientCount
		{
			get
			{
				return this.mailItem.Recipients.Count;
			}
		}

		public DateTime RoutingTimeStamp
		{
			get
			{
				return this.mailItem.RoutingTimeStamp;
			}
		}

		public DateTime ClientSubmitTime
		{
			get
			{
				DateTime result;
				this.mailItem.Message.TryGetMapiProperty<DateTime>(TnefPropertyTag.ClientSubmitTime, out result);
				return result;
			}
		}

		public ADRecipientCache<TransportMiniRecipient> ADRecipientCache
		{
			get
			{
				return this.mailItem.ADRecipientCache;
			}
		}

		public string Auth
		{
			get
			{
				return this.mailItem.Auth;
			}
		}

		public MultilevelAuthMechanism AuthMethod
		{
			get
			{
				return this.mailItem.AuthMethod;
			}
		}

		public BodyType BodyType
		{
			get
			{
				return this.mailItem.BodyType;
			}
		}

		public DateTime DateReceived
		{
			get
			{
				return this.mailItem.DateReceived;
			}
		}

		public DeferReason DeferReason
		{
			get
			{
				throw new NotImplementedException("MbxTransportMailItem.DeferReason");
			}
		}

		public MailDirectionality Directionality
		{
			get
			{
				return this.mailItem.Directionality;
			}
		}

		public string ExoAccountForest
		{
			get
			{
				return this.mailItem.ExoAccountForest;
			}
		}

		public string ExoTenantContainer
		{
			get
			{
				return this.mailItem.ExoTenantContainer;
			}
		}

		public Guid ExternalOrganizationId
		{
			get
			{
				return this.mailItem.ExternalOrganizationId;
			}
		}

		public DsnFormat DsnFormat
		{
			get
			{
				return this.mailItem.DsnFormat;
			}
		}

		public DsnParameters DsnParameters
		{
			get
			{
				return null;
			}
		}

		public string EnvId
		{
			get
			{
				return this.mailItem.EnvId;
			}
		}

		public IReadOnlyExtendedPropertyCollection ExtendedProperties
		{
			get
			{
				return this.mailItem.ExtendedProperties;
			}
		}

		public TimeSpan ExtensionToExpiryDuration
		{
			get
			{
				return this.mailItem.ExtensionToExpiryDuration;
			}
		}

		public RoutingAddress From
		{
			get
			{
				return this.mailItem.From;
			}
		}

		public string HeloDomain
		{
			get
			{
				return this.mailItem.HeloDomain;
			}
		}

		public string InternetMessageId
		{
			get
			{
				return this.mailItem.InternetMessageId;
			}
		}

		public Guid NetworkMessageId
		{
			get
			{
				return this.mailItem.NetworkMessageId;
			}
		}

		public Guid SystemProbeId
		{
			get
			{
				return this.mailItem.SystemProbeId;
			}
		}

		public bool IsProbe
		{
			get
			{
				return this.mailItem.IsProbe;
			}
		}

		public string ProbeName
		{
			get
			{
				return this.mailItem.ProbeName;
			}
		}

		public bool PersistProbeTrace
		{
			get
			{
				return this.mailItem.PersistProbeTrace;
			}
		}

		public bool IsHeartbeat
		{
			get
			{
				throw new NotImplementedException("MbxTransportMailItem.IsHeartbeat");
			}
		}

		public bool IsActive
		{
			get
			{
				return this.mailItem.IsActive;
			}
		}

		public IEnumerable<string> LockReasonHistory
		{
			get
			{
				return this.mailItem.LockReasonHistory;
			}
		}

		public LatencyTracker LatencyTracker
		{
			get
			{
				return this.mailItem.LatencyTracker;
			}
		}

		public byte[] LegacyXexch50Blob
		{
			get
			{
				return this.mailItem.LegacyXexch50Blob;
			}
		}

		public EmailMessage Message
		{
			get
			{
				return this.mailItem.Message;
			}
		}

		public string Oorg
		{
			get
			{
				return this.mailItem.Oorg;
			}
		}

		public RiskLevel RiskLevel
		{
			get
			{
				return this.mailItem.RiskLevel;
			}
		}

		public DeliveryPriority Priority
		{
			get
			{
				return this.mailItem.Priority;
			}
		}

		public string PrioritizationReason
		{
			get
			{
				return this.mailItem.PrioritizationReason;
			}
		}

		public MimeDocument MimeDocument
		{
			get
			{
				return this.mailItem.MimeDocument;
			}
		}

		public string MimeFrom
		{
			get
			{
				return this.mailItem.MimeFrom;
			}
		}

		public RoutingAddress MimeSender
		{
			get
			{
				return this.mailItem.MimeSender;
			}
		}

		public long MimeSize
		{
			get
			{
				return this.mailItem.MimeSize;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.mailItem.OrganizationId;
			}
		}

		public RoutingAddress OriginalFrom
		{
			get
			{
				return this.mailItem.OriginalFrom;
			}
		}

		public int PoisonCount
		{
			get
			{
				return this.mailItem.PoisonCount;
			}
		}

		public int PoisonForRemoteCount
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsPoison
		{
			get
			{
				return this.mailItem.IsPoison;
			}
		}

		public string ReceiveConnectorName
		{
			get
			{
				return this.mailItem.ReceiveConnectorName;
			}
		}

		public IReadOnlyMailRecipientCollection Recipients
		{
			get
			{
				return this;
			}
		}

		public long RecordId
		{
			get
			{
				return this.recordId;
			}
		}

		public bool RetryDeliveryIfRejected
		{
			get
			{
				return this.mailItem.RetryDeliveryIfRejected;
			}
		}

		public MimePart RootPart
		{
			get
			{
				return this.mailItem.RootPart;
			}
		}

		public int Scl
		{
			get
			{
				return this.mailItem.Scl;
			}
		}

		public Guid ShadowMessageId
		{
			get
			{
				throw new NotImplementedException("MbxTransportMailItem.ShadowMessageId");
			}
		}

		public string ShadowServerContext
		{
			get
			{
				throw new NotImplementedException("MbxTransportMailItem.ShadowServerContext");
			}
		}

		public string ShadowServerDiscardId
		{
			get
			{
				throw new NotImplementedException("MbxTransportMailItem.ShadowServerDiscardId");
			}
		}

		public IPAddress SourceIPAddress
		{
			get
			{
				return this.mailItem.SourceIPAddress;
			}
		}

		public string Subject
		{
			get
			{
				return this.mailItem.Subject;
			}
		}

		public PerTenantTransportSettings TransportSettings
		{
			get
			{
				return this.mailItem.TransportSettings;
			}
		}

		public bool Deferred
		{
			get
			{
				throw new NotImplementedException("MbxTransportMailItem.Deferred");
			}
		}

		public bool SuppressBodyInDsn
		{
			get
			{
				throw new NotImplementedException("MbxTransportMailItem.SuppressBodyInDsn");
			}
			set
			{
				throw new NotImplementedException("MbxTransportMailItem.SuppressBodyInDsn");
			}
		}

		int IReadOnlyMailRecipientCollection.Count
		{
			get
			{
				return this.mailItem.Recipients.Count;
			}
		}

		internal DateTime SessionStartTime { get; set; }

		internal Guid DatabaseGuid { get; set; }

		internal string DatabaseName { get; set; }

		internal MessageAction MessageLevelAction { get; set; }

		MailRecipient IReadOnlyMailRecipientCollection.this[int index]
		{
			get
			{
				return this.mailItem.Recipients[index];
			}
		}

		IEnumerable<MailRecipient> IReadOnlyMailRecipientCollection.All
		{
			get
			{
				return this.mailItem.Recipients;
			}
		}

		IEnumerable<MailRecipient> IReadOnlyMailRecipientCollection.AllUnprocessed
		{
			get
			{
				return this.mailItem.Recipients.AllUnprocessed;
			}
		}

		public LazyBytes FastIndexBlob
		{
			get
			{
				throw new NotSupportedException("This should never be called");
			}
		}

		public void CacheTransportSettings()
		{
			this.mailItem.CacheTransportSettings();
		}

		public void IncrementPoisonForRemoteCount()
		{
			throw new NotImplementedException();
		}

		public bool IsJournalReport()
		{
			return this.mailItem.IsJournalReport();
		}

		public bool IsPfReplica()
		{
			return this.mailItem.IsPfReplica();
		}

		public bool IsShadowed()
		{
			return this.mailItem.IsShadowed();
		}

		public bool IsDelayedAck()
		{
			return this.mailItem.IsDelayedAck();
		}

		public TransportMailItem NewCloneWithoutRecipients(bool shareRecipientCache)
		{
			TransportMailItem result;
			lock (this.mailItem)
			{
				result = this.mailItem.NewCloneWithoutRecipients(shareRecipientCache, this.mailItem.LatencyTracker, null);
			}
			return result;
		}

		public Stream OpenMimeReadStream()
		{
			return this.mailItem.OpenMimeReadStream();
		}

		public void FinalizeDeliveryLatencyTracking(LatencyComponent deliveryComponent)
		{
			LatencyTracker.EndTrackLatency(LatencyComponent.Delivery, deliveryComponent, this.mailItem.LatencyTracker);
		}

		public void TrackSuccessfulConnectLatency(LatencyComponent connectComponent)
		{
			LatencyTracker.EndAndBeginTrackLatency(connectComponent, LatencyComponent.Delivery, this.mailItem.LatencyTracker);
		}

		public Stream OpenMimeReadStream(bool downConvert)
		{
			return this.mailItem.OpenMimeReadStream(downConvert);
		}

		public void AddDsnParameters(string key, object value)
		{
			throw new ArgumentException("Not implemented in MbxTransportMailItem");
		}

		public virtual void AckRecipient(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			this.recipientResponses.Enqueue(new AckStatusAndResponse(ackStatus, smtpResponse));
		}

		public IEnumerator<MailRecipient> GetEnumerator()
		{
			return this.mailItem.Recipients.GetEnumerator();
		}

		public void AddAgentInfo(string agentName, string eventName, List<KeyValuePair<string, string>> data)
		{
			this.mailItem.AddAgentInfo(agentName, eventName, data);
		}

		public void TrackAgentInfo()
		{
			MessageTrackingLog.TrackAgentInfo(this.mailItem);
		}

		public SmtpResponse Response
		{
			get
			{
				return this.mailItemResponse;
			}
		}

		bool IReadOnlyMailRecipientCollection.Contains(MailRecipient item)
		{
			return this.mailItem.Recipients.Contains(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal virtual MailRecipient GetNextRecipient()
		{
			Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.FaultInjectionTracer.TraceTest(2883988797U);
			if (this.recipientEnumerator.MoveNext())
			{
				return this.recipientEnumerator.Current;
			}
			return null;
		}

		internal IList<MailRecipient> GetRecipients()
		{
			List<MailRecipient> list = new List<MailRecipient>(this.Recipients.Count);
			foreach (MailRecipient item in this.Recipients)
			{
				list.Add(item);
			}
			return list;
		}

		internal virtual void AckMailItem(AckStatus ackStatus, SmtpResponse smtpResponse, AckDetails ackDetails, TimeSpan? retryInterval, string messageTrackingSourceContext)
		{
			if (ackStatus == AckStatus.Retry && smtpResponse.SmtpResponseType != SmtpResponseType.TransientError)
			{
				smtpResponse = TransportMailItem.ReplaceFailWithRetryResponse(smtpResponse);
			}
			this.mailItem.Ack(ackStatus, smtpResponse, this.GetRecipients(), this.recipientResponses);
			this.FinalizeDeliveryLatencyTracking(LatencyComponent.StoreDriverDelivery);
			this.mailItemResponse = SmtpResponseGenerator.GenerateResponse(this.MessageLevelAction, this.Recipients, smtpResponse, retryInterval);
			foreach (MailRecipient mailRecipient in this.Recipients)
			{
				if (mailRecipient.ExtendedProperties.Contains("ExceptionAgentName"))
				{
					mailRecipient.ExtendedProperties.Remove("ExceptionAgentName");
				}
			}
		}

		private void Initialize()
		{
			this.recipientResponses = new Queue<AckStatusAndResponse>();
			this.recipientEnumerator = this.mailItem.Recipients.GetEnumerator();
			this.mailItemResponse = SmtpResponse.Empty;
			this.RestoreInternalMessageId();
			this.MessageLevelAction = MessageAction.Success;
		}

		private void RestoreInternalMessageId()
		{
			ulong num;
			if (this.ExtendedProperties.TryGetValue<ulong>("Microsoft.Exchange.Transport.MailboxTransport.InternalMessageId", out num))
			{
				this.recordId = (long)num;
				return;
			}
			MbxTransportMailItem.Diag.TraceWarning<string>(0L, "Failed to get RecordId from Extended properties for message {0}.", this.InternetMessageId);
			this.recordId = 0L;
		}

		private static readonly Trace Diag = Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery.ExTraceGlobals.StoreDriverDeliveryTracer;

		private IEnumerator<MailRecipient> recipientEnumerator;

		private Queue<AckStatusAndResponse> recipientResponses;

		private TransportMailItem mailItem;

		private SmtpResponse mailItemResponse;

		private long recordId;
	}
}
