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
	internal class ShadowRoutedMailItem : IReadOnlyMailItem, ISystemProbeTraceable
	{
		public ShadowRoutedMailItem(TransportMailItem mailItem)
		{
			this.mailItem = mailItem;
			this.recipients = new MailRecipientCollection(mailItem, new List<MailRecipient>(mailItem.Recipients.All));
			this.bodyType = this.mailItem.BodyType;
		}

		public ADRecipientCache<TransportMiniRecipient> ADRecipientCache
		{
			get
			{
				throw new NotSupportedException("This should never be called");
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
				return this.bodyType;
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
				throw new NotSupportedException("This should never be called");
			}
			set
			{
				throw new NotSupportedException("This should never be called");
			}
		}

		public MailDirectionality Directionality
		{
			get
			{
				return this.mailItem.Directionality;
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
				return this.mailItem.DsnParameters;
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
				return this.mailItem.IsHeartbeat;
			}
		}

		public bool IsActive
		{
			get
			{
				return this.mailItem.IsActive;
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

		public IEnumerable<string> LockReasonHistory
		{
			get
			{
				return null;
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
				return 0L;
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
				return this.recipients;
			}
		}

		public IEnumerable<MailRecipient> RecipientList
		{
			get
			{
				return this.recipients.All;
			}
		}

		public long RecordId
		{
			get
			{
				return this.mailItem.RecordId;
			}
		}

		public bool RetryDeliveryIfRejected
		{
			get
			{
				throw new NotSupportedException("This should never be called");
			}
		}

		public MimePart RootPart
		{
			get
			{
				throw new NotSupportedException("This should never be called");
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
				return this.mailItem.ShadowMessageId;
			}
		}

		public string ShadowServerContext
		{
			get
			{
				throw new NotSupportedException("This should never be called");
			}
		}

		public string ShadowServerDiscardId
		{
			get
			{
				throw new NotSupportedException("This should never be called");
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

		public bool SuppressBodyInDsn
		{
			get
			{
				throw new NotSupportedException("This should never be called");
			}
			set
			{
			}
		}

		public PerTenantTransportSettings TransportSettings
		{
			get
			{
				throw new NotSupportedException("This should never be called");
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
			throw new NotSupportedException("This should never be called");
		}

		public bool IsJournalReport()
		{
			throw new NotSupportedException("This should never be called");
		}

		public bool IsPfReplica()
		{
			return this.mailItem.IsPfReplica();
		}

		public bool IsShadowed()
		{
			throw new NotSupportedException("This should never be called");
		}

		public bool IsDelayedAck()
		{
			throw new NotSupportedException("This should never be called");
		}

		public TransportMailItem NewCloneWithoutRecipients(bool shareRecipientCache)
		{
			throw new NotSupportedException("This should never be called");
		}

		public Stream OpenMimeReadStream()
		{
			throw new NotSupportedException("This should never be called");
		}

		public Stream OpenMimeReadStream(bool downConvert)
		{
			throw new NotSupportedException("This should never be called");
		}

		public void TrackSuccessfulConnectLatency(LatencyComponent connectComponent)
		{
			LatencyTracker.EndTrackLatency(LatencyComponent.Delivery, connectComponent, this.LatencyTracker);
			LatencyTracker.BeginTrackLatency(LatencyComponent.Delivery, this.LatencyTracker);
		}

		public void AddDsnParameters(string key, object value)
		{
		}

		public void IncrementPoisonForRemoteCount()
		{
			throw new NotImplementedException();
		}

		private TransportMailItem mailItem;

		private MailRecipientCollection recipients;

		private BodyType bodyType;
	}
}
