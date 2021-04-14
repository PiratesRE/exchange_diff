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

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal sealed class IrmProbeMailItem : IReadOnlyMailItem, ISystemProbeTraceable
	{
		public static IrmProbeMailItem CreateFromStream(Stream stream)
		{
			return new IrmProbeMailItem(stream);
		}

		public ADRecipientCache<TransportMiniRecipient> ADRecipientCache
		{
			get
			{
				return null;
			}
		}

		public string Auth
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public MultilevelAuthMechanism AuthMethod
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public BodyType BodyType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public DateTime DateReceived
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public DeferReason DeferReason
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public MailDirectionality Directionality
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public DsnFormat DsnFormat
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public DsnParameters DsnParameters
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool SuppressBodyInDsn
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string EnvId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IReadOnlyExtendedPropertyCollection ExtendedProperties
		{
			get
			{
				return this.extendedPropertyDictionary;
			}
		}

		public TimeSpan ExtensionToExpiryDuration
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public RoutingAddress From
		{
			get
			{
				return RoutingAddress.Empty;
			}
		}

		public string HeloDomain
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string InternetMessageId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Guid NetworkMessageId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsActive
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsHeartbeat
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public LatencyTracker LatencyTracker
		{
			get
			{
				return null;
			}
		}

		public byte[] LegacyXexch50Blob
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IEnumerable<string> LockReasonHistory
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public EmailMessage Message
		{
			get
			{
				return this.emailMessage;
			}
		}

		public string Oorg
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string ExoAccountForest
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string ExoTenantContainer
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Guid ExternalOrganizationId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public RiskLevel RiskLevel
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public DeliveryPriority Priority
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string PrioritizationReason
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public MimeDocument MimeDocument
		{
			get
			{
				return null;
			}
		}

		public string MimeFrom
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public RoutingAddress MimeSender
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public long MimeSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public RoutingAddress OriginalFrom
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int PoisonCount
		{
			get
			{
				throw new NotImplementedException();
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
				throw new NotImplementedException();
			}
		}

		public string ReceiveConnectorName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IReadOnlyMailRecipientCollection Recipients
		{
			get
			{
				return null;
			}
		}

		public long RecordId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool RetryDeliveryIfRejected
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public MimePart RootPart
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int Scl
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Guid ShadowMessageId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string ShadowServerContext
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string ShadowServerDiscardId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IPAddress SourceIPAddress
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string Subject
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public PerTenantTransportSettings TransportSettings
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public LazyBytes FastIndexBlob
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void CacheTransportSettings()
		{
			throw new NotImplementedException();
		}

		public bool IsJournalReport()
		{
			throw new NotImplementedException();
		}

		public bool IsPfReplica()
		{
			throw new NotImplementedException();
		}

		public bool IsShadowed()
		{
			throw new NotImplementedException();
		}

		public bool IsDelayedAck()
		{
			throw new NotImplementedException();
		}

		public TransportMailItem NewCloneWithoutRecipients(bool shareRecipientCache)
		{
			throw new NotImplementedException();
		}

		public Stream OpenMimeReadStream()
		{
			return this.emailMessage.MimeDocument.RootPart.GetContentReadStream();
		}

		public Stream OpenMimeReadStream(bool downConvert)
		{
			throw new NotImplementedException();
		}

		public void TrackSuccessfulConnectLatency(LatencyComponent connectComponent)
		{
			throw new NotImplementedException();
		}

		public void AddDsnParameters(string key, object value)
		{
			throw new NotImplementedException();
		}

		public void IncrementPoisonForRemoteCount()
		{
			throw new NotImplementedException();
		}

		public Guid SystemProbeId
		{
			get
			{
				return Guid.Empty;
			}
		}

		public bool IsProbe
		{
			get
			{
				return true;
			}
		}

		public string ProbeName
		{
			get
			{
				return string.Empty;
			}
		}

		public bool PersistProbeTrace
		{
			get
			{
				return false;
			}
		}

		private IrmProbeMailItem(Stream stream)
		{
			this.emailMessage = EmailMessage.Create(stream);
		}

		private readonly EmailMessage emailMessage;

		private readonly ExtendedPropertyDictionary extendedPropertyDictionary = new ExtendedPropertyDictionary();
	}
}
