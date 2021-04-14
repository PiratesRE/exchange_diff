using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.ShadowRedundancy;
using Microsoft.Exchange.Transport.Storage;
using Microsoft.Exchange.Transport.Storage.Messaging;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport
{
	internal class TransportMailItem : IReadOnlyMailItem, ISystemProbeTraceable, ITransportMailItemFacade, ILockableItem, IQueueItem, IQueueQuotaMailItem
	{
		public event Action<TransportMailItem> OnReleaseFromActive;

		private TransportMailItem(LatencyComponent sourceComponent) : this(null, sourceComponent, null, MailDirectionality.Undefined, default(Guid))
		{
		}

		private TransportMailItem(ADRecipientCache<TransportMiniRecipient> recipientCache, LatencyComponent sourceComponent, MailDirectionality directionality, Guid externalOrgId) : this(recipientCache, sourceComponent, null, directionality, externalOrgId)
		{
		}

		private TransportMailItem(ADRecipientCache<TransportMiniRecipient> recipientCache, LatencyComponent sourceComponent, IMailItemStorage storage, MailDirectionality directionality = MailDirectionality.Undefined, Guid externalOrgId = default(Guid))
		{
			this.audit = new Breadcrumbs(8);
			this.deferUntil = DateTime.MinValue;
			this.nextHopSolutionTable = new Dictionary<NextHopSolutionKey, NextHopSolution>();
			this.exposeMessage = true;
			this.exposeMessageHeaders = true;
			this.queueQuotaTrackingBits = new QueueQuotaTrackingBits();
			this.latencyStartTime = DateTime.MinValue;
			base..ctor();
			this.storage = (storage ?? TransportMailItem.Database.NewMailItemStorage(true));
			this.storage.Recipients = new MailRecipientCollection(this);
			this.routeForHighAvailability = true;
			this.routingTimeStamp = DateTime.MinValue;
			this.snapshotState = new SnapshotWriterState();
			this.latencyTracker = LatencyTracker.CreateInstance(sourceComponent);
			this.loadedInRestart = (sourceComponent == LatencyComponent.ServiceRestart);
			this.recipientCache = recipientCache;
			if (directionality != MailDirectionality.Undefined)
			{
				this.Directionality = directionality;
			}
			else if (storage != null)
			{
				this.Directionality = storage.Directionality;
			}
			if (MultiTenantTransport.MultiTenancyEnabled)
			{
				if (externalOrgId != Guid.Empty)
				{
					this.ExternalOrganizationId = externalOrgId;
				}
				else if (storage != null)
				{
					this.ExternalOrganizationId = storage.ExternalOrganizationId;
				}
			}
			if (sourceComponent == LatencyComponent.Heartbeat)
			{
				this.ShadowMessageId = Guid.Empty;
			}
			if (storage == null)
			{
				this.SetMimeDefaultEncoding();
			}
		}

		private TransportMailItem(TransportMailItem rhs, bool shareRecipientCache, LatencyTracker latencyTrackerToClone)
		{
			this.audit = new Breadcrumbs(8);
			this.deferUntil = DateTime.MinValue;
			this.nextHopSolutionTable = new Dictionary<NextHopSolutionKey, NextHopSolution>();
			this.exposeMessage = true;
			this.exposeMessageHeaders = true;
			this.queueQuotaTrackingBits = new QueueQuotaTrackingBits();
			this.latencyStartTime = DateTime.MinValue;
			base..ctor();
			this.storage = rhs.storage.Clone();
			this.storage.Recipients = new MailRecipientCollection(this);
			this.RemoveFirewalledProperties(this.ExtendedPropertyDictionary);
			this.CurrentCondition = rhs.CurrentCondition;
			if (rhs.ThrottlingContext != null)
			{
				this.ThrottlingContext = rhs.ThrottlingContext;
				this.ThrottlingContext.AddMemoryCost(new ByteQuantifiedSize((ulong)this.MimeSize));
			}
			if (rhs.messageTrackingAgentInfo != null)
			{
				this.messageTrackingAgentInfo = new List<List<KeyValuePair<string, string>>>(rhs.messageTrackingAgentInfo);
			}
			this.audit.Drop(Breadcrumb.CloneItem);
			this.snapshotState = new SnapshotWriterState();
			if (shareRecipientCache)
			{
				this.recipientCache = rhs.ADRecipientCache;
			}
			else if (rhs.ADRecipientCache != null)
			{
				this.recipientCache = new ADRecipientCache<TransportMiniRecipient>(TransportMiniRecipientSchema.Properties, 0, rhs.ADRecipientCache.OrganizationId);
			}
			this.Directionality = rhs.Directionality;
			this.ExternalOrganizationId = rhs.ExternalOrganizationId;
			this.latencyStartTime = rhs.latencyStartTime;
			this.transportSettings = rhs.transportSettings;
			this.routeForHighAvailability = rhs.routeForHighAvailability;
			this.routingTimeStamp = rhs.routingTimeStamp;
			this.dsnParameters = rhs.DsnParameters;
			if (rhs.PipelineTracingEnabled)
			{
				this.pipelineTracingPath = rhs.PipelineTracingPath;
				this.pipelineTracingEnabled = new bool?(rhs.PipelineTracingEnabled);
				this.snapshotState.IsOriginalWritten = true;
			}
			this.latencyTracker = LatencyTracker.Clone(latencyTrackerToClone);
			if (rhs.lockReasonHistory != null)
			{
				this.lockReasonHistory = new List<string>();
				this.lockReasonHistory.AddRange(rhs.lockReasonHistory);
			}
			this.FallbackToRawOverride = rhs.FallbackToRawOverride;
		}

		private TransportMailItem(TransportMailItem rhs, bool shareRecipientCache, LatencyTracker latencyTrackerToClone, ForkCount transportRulesForkCount, bool copyStorage = false)
		{
			this.audit = new Breadcrumbs(8);
			this.deferUntil = DateTime.MinValue;
			this.nextHopSolutionTable = new Dictionary<NextHopSolutionKey, NextHopSolution>();
			this.exposeMessage = true;
			this.exposeMessageHeaders = true;
			this.queueQuotaTrackingBits = new QueueQuotaTrackingBits();
			this.latencyStartTime = DateTime.MinValue;
			base..ctor();
			this.transportRulesForkCount = transportRulesForkCount;
			this.CurrentCondition = rhs.CurrentCondition;
			if (rhs.messageTrackingAgentInfo != null)
			{
				this.messageTrackingAgentInfo = new List<List<KeyValuePair<string, string>>>(rhs.messageTrackingAgentInfo);
			}
			this.snapshotState = new SnapshotWriterState();
			if (shareRecipientCache)
			{
				this.recipientCache = rhs.ADRecipientCache;
			}
			else if (rhs.ADRecipientCache != null)
			{
				this.recipientCache = new ADRecipientCache<TransportMiniRecipient>(TransportMiniRecipientSchema.Properties, 0, rhs.ADRecipientCache.OrganizationId);
			}
			this.latencyStartTime = rhs.latencyStartTime;
			this.transportSettings = rhs.transportSettings;
			this.routeForHighAvailability = rhs.routeForHighAvailability;
			this.routingTimeStamp = rhs.routingTimeStamp;
			this.dsnParameters = rhs.DsnParameters;
			if (rhs.PipelineTracingEnabled)
			{
				this.pipelineTracingPath = rhs.PipelineTracingPath;
				this.pipelineTracingEnabled = new bool?(rhs.PipelineTracingEnabled);
				this.snapshotState.IsOriginalWritten = true;
			}
			this.latencyTracker = LatencyTracker.Clone(latencyTrackerToClone);
			if (rhs.lockReasonHistory != null)
			{
				this.lockReasonHistory = new List<string>();
				this.lockReasonHistory.AddRange(rhs.lockReasonHistory);
			}
			this.audit.Drop(Breadcrumb.CloneItem);
			if (copyStorage)
			{
				this.storage = rhs.storage.CopyCommitted(delegate(IMailItemStorage copy)
				{
					copy.Recipients = new MailRecipientCollection(this);
					this.RemoveFirewalledProperties(copy.ExtendedProperties as IDictionary<string, object>);
				});
			}
			else
			{
				this.storage = rhs.storage.CloneCommitted(delegate(IMailItemStorage clone)
				{
					clone.Recipients = new MailRecipientCollection(this);
					this.RemoveFirewalledProperties(clone.ExtendedProperties as IDictionary<string, object>);
				});
			}
			if (rhs.ThrottlingContext != null)
			{
				this.ThrottlingContext = rhs.ThrottlingContext;
				this.ThrottlingContext.AddMemoryCost(new ByteQuantifiedSize((ulong)this.MimeSize));
			}
			this.FallbackToRawOverride = rhs.FallbackToRawOverride;
		}

		public static IMessagingDatabase Database
		{
			get
			{
				return TransportMailItem.database;
			}
		}

		public bool LoadedInRestart
		{
			get
			{
				return this.loadedInRestart;
			}
		}

		public ADRecipientCache<TransportMiniRecipient> ADRecipientCache
		{
			get
			{
				return this.recipientCache;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.recipientCache = value;
			}
		}

		public DeliveryPriority Priority
		{
			get
			{
				if (this.storage.Priority == null)
				{
					return DeliveryPriority.Normal;
				}
				return this.storage.Priority.Value;
			}
			set
			{
				this.storage.Priority = new DeliveryPriority?(value);
				if (TransportMailItem.configuration.AppConfig.DeliveryQueuePrioritizationConfiguration.PriorityHeaderPromotionEnabled)
				{
					Util.EncodeAndSetPriorityAsHeader(this.RootPart.Headers, value, this.PrioritizationReason);
				}
			}
		}

		public DeliveryPriority BootloadingPriority
		{
			get
			{
				return this.storage.BootloadingPriority;
			}
			set
			{
				this.storage.BootloadingPriority = value;
			}
		}

		public bool RouteForHighAvailability
		{
			get
			{
				return this.routeForHighAvailability;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				this.routeForHighAvailability = value;
			}
		}

		public bool IsReplayMessage
		{
			get
			{
				return this.RootPart != null && this.RootPart.Headers != null && this.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-ResubmittedMessage") != null;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				if (this.recipientCache != null)
				{
					return this.recipientCache.OrganizationId;
				}
				return null;
			}
		}

		public PerTenantTransportSettings TransportSettings
		{
			get
			{
				if (this.transportSettings == null)
				{
					throw new InvalidOperationException("Trying to access TransportSettings before the value is set. This field should be accessed only after calling CacheTransportSettings method");
				}
				return this.transportSettings;
			}
		}

		public LatencyTracker LatencyTracker
		{
			get
			{
				return this.latencyTracker;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				this.latencyTracker = value;
			}
		}

		public IActivityScope ActivityScope
		{
			get
			{
				return this.activityScope;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				this.activityScope = value;
			}
		}

		public DateTime DateReceived
		{
			get
			{
				return this.storage.DateReceived;
			}
			set
			{
				this.storage.DateReceived = value;
			}
		}

		public TimeSpan ExtensionToExpiryDuration
		{
			get
			{
				return this.storage.ExtensionToExpiryDuration;
			}
			private set
			{
				this.storage.ExtensionToExpiryDuration = value;
			}
		}

		public string PerfCounterAttribution
		{
			get
			{
				return this.storage.PerfCounterAttribution;
			}
			set
			{
				this.storage.PerfCounterAttribution = value;
			}
		}

		public MultilevelAuthMechanism AuthMethod
		{
			get
			{
				return this.authMethod;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				this.authMethod = value;
			}
		}

		public string MessageTrackingSecurityInfo
		{
			get
			{
				return this.messageTrackingSecurityInfo;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				this.messageTrackingSecurityInfo = value;
			}
		}

		public bool ExposeMessage
		{
			get
			{
				return this.exposeMessage;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				this.exposeMessage = value;
			}
		}

		public bool ExposeMessageHeaders
		{
			get
			{
				return this.exposeMessageHeaders;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				this.exposeMessageHeaders = value;
			}
		}

		public RoutingAddress From
		{
			get
			{
				return new RoutingAddress(this.FromAddress);
			}
			set
			{
				if (value == RoutingAddress.Empty)
				{
					throw new ArgumentException("From property cannot be set to an empty RoutingAddress value.");
				}
				if (!value.IsValid)
				{
					throw new FormatException("Can not set From property to an invalid RoutingAddress value.");
				}
				if (string.Equals("<>", value.LocalPart) && !string.IsNullOrEmpty(value.DomainPart))
				{
					throw new ArgumentException("The Domain part is not empty and the local part is <>", value.ToString());
				}
				this.FromAddress = value.ToString();
				this.SetMimeDefaultEncoding();
				if (this.pipelineTracingEnabled == null && TransportMailItem.configuration != null)
				{
					Server transportServer = TransportMailItem.configuration.LocalServer.TransportServer;
					if (transportServer != null && transportServer.PipelineTracingEnabled && transportServer.PipelineTracingSenderAddress != null && transportServer.PipelineTracingPath != null && !string.IsNullOrEmpty(transportServer.PipelineTracingPath.PathName))
					{
						this.pipelineTracingEnabled = new bool?(!transportServer.PipelineTracingSenderAddress.Equals(SmtpAddress.Empty) && transportServer.PipelineTracingSenderAddress.ToString().Equals(this.FromAddress));
						if (this.pipelineTracingEnabled.Value)
						{
							this.pipelineTracingPath = transportServer.PipelineTracingPath.PathName;
							TransportMailItem.Logger.LogEvent(TransportEventLogConstants.Tuple_PipelineTracingActive, "TransportPipelineTracingActive", new object[0]);
							return;
						}
					}
					else
					{
						this.pipelineTracingEnabled = new bool?(false);
					}
				}
			}
		}

		public RoutingAddress OriginalFrom
		{
			get
			{
				string attributedFromAddress = this.storage.AttributedFromAddress;
				if (!string.IsNullOrEmpty(attributedFromAddress))
				{
					return new RoutingAddress(attributedFromAddress);
				}
				if (!string.IsNullOrEmpty(this.FromAddress))
				{
					return new RoutingAddress(this.FromAddress);
				}
				return RoutingAddress.Empty;
			}
		}

		public string OriginalFromAddress
		{
			get
			{
				return this.OriginalFrom.ToString();
			}
		}

		public string MimeFrom
		{
			get
			{
				return this.storage.MimeFrom;
			}
			set
			{
				this.storage.MimeFrom = value;
			}
		}

		public RemoteDomainEntry FromDomainConfig
		{
			get
			{
				PerTenantRemoteDomainTable orgRemoteDomains = this.GetOrgRemoteDomains();
				if (orgRemoteDomains == null)
				{
					return null;
				}
				return orgRemoteDomains.RemoteDomainTable.GetDomainEntry(SmtpDomain.GetDomainPart(this.From));
			}
		}

		public RoutingAddress MimeSender
		{
			get
			{
				return new RoutingAddress(this.MimeSenderAddress);
			}
			set
			{
				if (value != RoutingAddress.Empty && !value.IsValid)
				{
					throw new FormatException("Can not set the Mime Sender property to an invalid RoutingAddress value.");
				}
				this.MimeSenderAddress = value.ToString();
			}
		}

		public DsnFormat DsnFormat
		{
			get
			{
				return this.storage.DsnFormat;
			}
			set
			{
				this.storage.DsnFormat = value;
			}
		}

		public DsnParameters DsnParameters
		{
			get
			{
				return this.dsnParameters;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				this.dsnParameters = value;
			}
		}

		public bool SuppressBodyInDsn
		{
			get
			{
				return this.DsnFormat == DsnFormat.Headers;
			}
			set
			{
				this.DsnFormat = (value ? DsnFormat.Headers : DsnFormat.Full);
			}
		}

		public bool IsDiscardPending
		{
			get
			{
				return this.storage.IsDiscardPending;
			}
			set
			{
				this.storage.IsDiscardPending = value;
			}
		}

		public int Scl
		{
			get
			{
				return this.storage.Scl;
			}
			set
			{
				if (value < -1 || value > 10)
				{
					throw new ArgumentException("Invalid Scl value:" + value);
				}
				this.storage.Scl = value;
			}
		}

		public MailDirectionality Directionality
		{
			get
			{
				return this.storage.Directionality;
			}
			set
			{
				this.storage.Directionality = value;
			}
		}

		public IExtendedPropertyCollection ExtendedProperties
		{
			get
			{
				return this.storage.ExtendedProperties;
			}
		}

		public IDictionary<string, object> ExtendedPropertyDictionary
		{
			get
			{
				this.ThrowIfDeleted();
				return this.ExtendedProperties as IDictionary<string, object>;
			}
		}

		public Guid? OrganizationScope
		{
			get
			{
				Guid value;
				if (this.ExtendedProperties.TryGetValue<Guid>("Microsoft.Exchange.Transport.MailRecipient.OrganizationScope", out value))
				{
					return new Guid?(value);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.ExtendedProperties.Remove("Microsoft.Exchange.Transport.MailRecipient.OrganizationScope");
					return;
				}
				this.ExtendedProperties.SetValue<Guid>("Microsoft.Exchange.Transport.MailRecipient.OrganizationScope", value.Value);
			}
		}

		public Status Status
		{
			get
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				foreach (MailRecipient mailRecipient in this.Recipients)
				{
					switch (mailRecipient.Status)
					{
					case Status.Ready:
						return Status.Ready;
					case Status.Retry:
						flag2 = true;
						break;
					case Status.Handled:
						flag3 = true;
						break;
					case Status.Locked:
						flag = true;
						break;
					}
				}
				if (flag)
				{
					return Status.Locked;
				}
				if (flag2)
				{
					return Status.Retry;
				}
				if (flag3)
				{
					return Status.Handled;
				}
				return Status.Complete;
			}
		}

		public bool QueuedForDelivery
		{
			get
			{
				return this.queuedForDelivery;
			}
		}

		public string HeloDomain
		{
			get
			{
				return this.storage.HeloDomain;
			}
			set
			{
				if (value != null && !RoutingAddress.IsValidDomain(value) && !RoutingAddress.IsDomainIPLiteral(value) && !HeloCommandEventArgs.IsValidIpv6WindowsAddress(value))
				{
					throw new FormatException("HeloDomain property has an invalid SMTP domain value: " + value);
				}
				this.storage.HeloDomain = value;
			}
		}

		public string Auth
		{
			get
			{
				return this.storage.Auth;
			}
			set
			{
				if (value != null && value.Length > 500)
				{
					throw new ArgumentException(Strings.ValueIsTooLarge(value.Length, 500));
				}
				this.storage.Auth = value;
			}
		}

		public string EnvId
		{
			get
			{
				return this.storage.EnvId;
			}
			set
			{
				if (value != null && value.Length > 100)
				{
					throw new ArgumentException(Strings.ValueIsTooLarge(value.Length, 100));
				}
				this.storage.EnvId = value;
			}
		}

		public BodyType BodyType
		{
			get
			{
				return this.storage.BodyType;
			}
			set
			{
				this.storage.BodyType = value;
			}
		}

		public string Oorg
		{
			get
			{
				return this.storage.Oorg;
			}
			set
			{
				if (!string.IsNullOrEmpty(value) && !RoutingAddress.IsValidDomain(value))
				{
					throw new ArgumentException(string.Format("Invalid originator organization value '{0}'. Originator organizations should be valid SMTP domains, like 'contoso.com'", value));
				}
				this.storage.Oorg = value;
				if (this.exposeMessageHeaders)
				{
					Util.SetOorgHeaders(this.RootPart.Headers, value);
				}
			}
		}

		public string ReceiveConnectorName
		{
			get
			{
				return this.storage.ReceiveConnectorName;
			}
			set
			{
				this.storage.ReceiveConnectorName = value;
			}
		}

		public bool IsInAsyncCommit
		{
			get
			{
				return this.storage.IsInAsyncCommit;
			}
		}

		public int PoisonCount
		{
			get
			{
				return this.storage.PoisonCount;
			}
			set
			{
				this.storage.PoisonCount = value;
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
				return this.PoisonCount >= Components.Configuration.LocalServer.TransportServer.PoisonThreshold;
			}
		}

		public Dictionary<NextHopSolutionKey, NextHopSolution> NextHopSolutions
		{
			get
			{
				return this.nextHopSolutionTable;
			}
			set
			{
				this.nextHopSolutionTable = value;
			}
		}

		public MimeDocument MimeDocument
		{
			get
			{
				return this.storage.MimeDocument;
			}
			set
			{
				this.storage.MimeDocument = value;
			}
		}

		public EmailMessage Message
		{
			get
			{
				if (!this.ExposeMessage)
				{
					throw new InvalidOperationException("Message can not be exposed here");
				}
				return this.storage.Message;
			}
		}

		public string PrioritizationReason
		{
			get
			{
				return this.storage.PrioritizationReason;
			}
			set
			{
				this.storage.PrioritizationReason = value;
			}
		}

		public MimePart RootPart
		{
			get
			{
				return this.storage.RootPart;
			}
		}

		public byte[] LegacyXexch50Blob
		{
			get
			{
				return this.storage.XExch50Blob.Value;
			}
			set
			{
				this.storage.XExch50Blob.Value = value;
			}
		}

		public DateTime RoutingTimeStamp
		{
			get
			{
				return this.routingTimeStamp;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				this.routingTimeStamp = value;
			}
		}

		public IPAddress SourceIPAddress
		{
			get
			{
				return this.storage.SourceIPAddress;
			}
			set
			{
				this.storage.SourceIPAddress = value;
			}
		}

		public long MimeSize
		{
			get
			{
				return this.storage.MimeSize;
			}
			set
			{
				this.storage.MimeSize = value;
			}
		}

		public bool MimeNotSequential
		{
			get
			{
				return this.storage.MimeNotSequential;
			}
			set
			{
				this.storage.MimeNotSequential = value;
			}
		}

		public bool FallbackToRawOverride
		{
			get
			{
				return this.storage.FallbackToRawOverride;
			}
			set
			{
				this.storage.FallbackToRawOverride = value;
			}
		}

		public string Subject
		{
			get
			{
				return this.storage.Subject;
			}
			set
			{
				this.storage.Subject = value;
			}
		}

		public string InternetMessageId
		{
			get
			{
				return this.storage.InternetMessageId;
			}
			set
			{
				this.storage.InternetMessageId = value;
			}
		}

		public Guid NetworkMessageId
		{
			get
			{
				return this.storage.NetworkMessageId;
			}
			set
			{
				this.storage.NetworkMessageId = value;
			}
		}

		public string ExoAccountForest
		{
			get
			{
				if (!string.IsNullOrEmpty(this.storage.ExoAccountForest))
				{
					return this.storage.ExoAccountForest;
				}
				if (this.OrganizationId != null)
				{
					if (!this.IsReadOnly)
					{
						this.storage.ExoAccountForest = this.OrganizationId.PartitionId.ForestFQDN;
					}
					return this.OrganizationId.PartitionId.ForestFQDN;
				}
				return null;
			}
			set
			{
				this.storage.ExoAccountForest = value;
			}
		}

		public string ExoTenantContainer
		{
			get
			{
				return this.storage.ExoTenantContainer;
			}
			set
			{
				this.storage.ExoTenantContainer = value;
			}
		}

		public Guid ExternalOrganizationId
		{
			get
			{
				return this.storage.ExternalOrganizationId;
			}
			set
			{
				this.storage.ExternalOrganizationId = value;
			}
		}

		public Guid SystemProbeId
		{
			get
			{
				return this.storage.SystemProbeId;
			}
			set
			{
				this.storage.SystemProbeId = value;
			}
		}

		public bool IsProbe
		{
			get
			{
				return this.SystemProbeId != Guid.Empty || !string.IsNullOrEmpty(this.storage.ProbeName);
			}
		}

		public string ProbeName
		{
			get
			{
				return this.storage.ProbeName;
			}
			set
			{
				this.storage.ProbeName = value;
			}
		}

		public bool PersistProbeTrace
		{
			get
			{
				return this.storage.PersistProbeTrace;
			}
			set
			{
				this.storage.PersistProbeTrace = value;
			}
		}

		public Guid ShadowMessageId
		{
			get
			{
				return this.storage.ShadowMessageId;
			}
			set
			{
				this.storage.ShadowMessageId = value;
			}
		}

		public string ShadowServerContext
		{
			get
			{
				return this.storage.ShadowServerContext;
			}
			set
			{
				this.storage.ShadowServerContext = value;
			}
		}

		public string ShadowServerDiscardId
		{
			get
			{
				return this.storage.ShadowServerDiscardId;
			}
			set
			{
				this.storage.ShadowServerDiscardId = value;
			}
		}

		public SnapshotWriterState SnapshotWriterState
		{
			get
			{
				return this.snapshotState;
			}
		}

		public bool PipelineTracingEnabled
		{
			get
			{
				return this.pipelineTracingEnabled != null && this.pipelineTracingEnabled.Value;
			}
		}

		public string PipelineTracingPath
		{
			get
			{
				return this.pipelineTracingPath;
			}
		}

		public bool RetryDeliveryIfRejected
		{
			get
			{
				return this.storage.RetryDeliveryIfRejected;
			}
		}

		public bool IsHeartbeat
		{
			get
			{
				return this.ShadowMessageId == Guid.Empty;
			}
		}

		public DeferReason DeferReason
		{
			get
			{
				return this.deferReason;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				this.deferReason = value;
			}
		}

		public DateTime Expiry
		{
			get
			{
				return this.GetExpiryTime(true);
			}
		}

		public MailRecipientCollection Recipients
		{
			get
			{
				return this.storage.Recipients as MailRecipientCollection;
			}
			private set
			{
				this.storage.Recipients = value;
			}
		}

		public long MsgId
		{
			get
			{
				return this.storage.MsgId;
			}
		}

		public long RecordId
		{
			get
			{
				return this.MsgId;
			}
		}

		public bool MimeWriteStreamOpen
		{
			get
			{
				return this.storage.MimeWriteStreamOpen;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.storage.IsReadOnly;
			}
		}

		public bool IsRowDeleted
		{
			get
			{
				return this.storage.IsDeleted;
			}
		}

		public AccessToken AccessToken
		{
			get
			{
				return this.accessToken;
			}
			internal set
			{
				this.accessToken = value;
			}
		}

		public ThrottlingContext ThrottlingContext
		{
			get
			{
				return this.throttlingContext;
			}
			set
			{
				this.throttlingContext = value;
			}
		}

		public WaitCondition CurrentCondition
		{
			get
			{
				return this.waitCondition;
			}
			set
			{
				this.waitCondition = value;
			}
		}

		public QueuedRecipientsByAgeToken QueuedRecipientsByAgeToken
		{
			get
			{
				return this.queuedRecipientsByAgeToken;
			}
			set
			{
				this.queuedRecipientsByAgeToken = value;
			}
		}

		public DateTimeOffset LockExpirationTime
		{
			get
			{
				return this.lockExpirationTime;
			}
			set
			{
				this.lockExpirationTime = value;
			}
		}

		public string LockReason
		{
			get
			{
				return this.lockReason;
			}
			set
			{
				this.lockReason = value;
				if (!string.IsNullOrEmpty(value))
				{
					if (this.lockReasonHistory == null)
					{
						this.lockReasonHistory = new List<string>();
					}
					this.lockReasonHistory.Add(value);
				}
			}
		}

		public IEnumerable<string> LockReasonHistory
		{
			get
			{
				return this.lockReasonHistory;
			}
			internal set
			{
				this.lockReasonHistory = ((value == null) ? null : new List<string>(value));
			}
		}

		public List<string> MoveToHosts
		{
			get
			{
				return this.storage.MoveToHosts;
			}
			set
			{
				this.storage.MoveToHosts = value;
			}
		}

		public DateTime DeferUntil
		{
			get
			{
				return this.deferUntil;
			}
			set
			{
				this.ThrowIfDeletedOrReadOnly();
				this.ThrowIfInAsyncCommit();
				this.deferUntil = value;
			}
		}

		public RiskLevel RiskLevel
		{
			get
			{
				return this.storage.RiskLevel;
			}
			set
			{
				this.storage.RiskLevel = value;
				Util.SetAsciiHeader(this.RootPart.Headers, "X-MS-Exchange-Organization-Spam-Filter-Enumerated-Risk", value.ToString());
			}
		}

		public ForkCount TransportRulesForkCount
		{
			get
			{
				return this.transportRulesForkCount;
			}
			set
			{
				this.transportRulesForkCount = value;
			}
		}

		private static ExEventLog Logger
		{
			get
			{
				if (TransportMailItem.eventLogger == null)
				{
					ExEventLog value = new ExEventLog(TransportMailItem.componentGuid, TransportEventLog.GetEventSource());
					Interlocked.CompareExchange<ExEventLog>(ref TransportMailItem.eventLogger, value, null);
				}
				return TransportMailItem.eventLogger;
			}
		}

		object ITransportMailItemFacade.ADRecipientCacheAsObject
		{
			get
			{
				return this.recipientCache;
			}
		}

		object ITransportMailItemFacade.OrganizationIdAsObject
		{
			get
			{
				return this.OrganizationId;
			}
		}

		bool ITransportMailItemFacade.IsOriginating
		{
			get
			{
				return this.Directionality == MailDirectionality.Originating;
			}
		}

		bool ITransportMailItemFacade.FallbackToRawOverride
		{
			get
			{
				return this.FallbackToRawOverride;
			}
			set
			{
				this.FallbackToRawOverride = value;
			}
		}

		ITransportSettingsFacade ITransportMailItemFacade.TransportSettings
		{
			get
			{
				return this.transportSettings;
			}
		}

		IReadOnlyExtendedPropertyCollection IReadOnlyMailItem.ExtendedProperties
		{
			get
			{
				return this.ExtendedProperties;
			}
		}

		IMailRecipientCollectionFacade ITransportMailItemFacade.Recipients
		{
			get
			{
				return this.Recipients;
			}
		}

		DeliveryPriority IQueueItem.Priority
		{
			get
			{
				if (this.storage.Priority == null)
				{
					return DeliveryPriority.Normal;
				}
				return this.storage.Priority.Value;
			}
			set
			{
				this.storage.Priority = new DeliveryPriority?(value);
			}
		}

		IReadOnlyMailRecipientCollection IReadOnlyMailItem.Recipients
		{
			get
			{
				return this.Recipients;
			}
		}

		MimeDocument ITransportMailItemFacade.MimeDocument
		{
			set
			{
				this.MimeDocument = value;
			}
		}

		QueueQuotaTrackingBits IQueueQuotaMailItem.QueueQuotaTrackingBits
		{
			get
			{
				return this.queueQuotaTrackingBits;
			}
		}

		private string FromAddress
		{
			get
			{
				return this.storage.FromAddress;
			}
			set
			{
				this.storage.FromAddress = value;
				if (this.storage.AttributedFromAddress == null)
				{
					this.storage.AttributedFromAddress = value;
				}
			}
		}

		private string MimeSenderAddress
		{
			get
			{
				return this.storage.MimeSenderAddress;
			}
			set
			{
				this.storage.MimeSenderAddress = value;
			}
		}

		private bool IsInDelivery
		{
			get
			{
				foreach (KeyValuePair<NextHopSolutionKey, NextHopSolution> keyValuePair in this.nextHopSolutionTable)
				{
					if (keyValuePair.Value.DeliveryStatus == DeliveryStatus.InDelivery)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsActive
		{
			get
			{
				return this.storage.IsActive;
			}
		}

		public bool IsNew
		{
			get
			{
				return this.storage.IsNew;
			}
		}

		public bool PendingDatabaseUpdates
		{
			get
			{
				return this.storage.PendingDatabaseUpdates;
			}
		}

		public LazyBytes FastIndexBlob
		{
			get
			{
				return this.storage.FastIndexBlob;
			}
		}

		public DateTime LatencyStartTime
		{
			get
			{
				if (this.latencyStartTime == DateTime.MinValue)
				{
					DateTime dateTime;
					if (Util.TryGetLastResubmitTime(this, out dateTime) || Util.TryGetOrganizationalMessageArrivalTime(this, out dateTime))
					{
						this.latencyStartTime = dateTime.ToUniversalTime();
					}
					else
					{
						this.latencyStartTime = DateTime.UtcNow;
					}
				}
				return this.latencyStartTime;
			}
		}

		public static SmtpResponse ReplaceFailWithRetryResponse(SmtpResponse originalResponse)
		{
			string text = "The server responded with: " + originalResponse + ". The failure was replaced by a retry response because the message was marked for retry if rejected.";
			return new SmtpResponse("400", "4.4.7", new string[]
			{
				text
			});
		}

		public static TransportMailItem NewMailItem(LatencyComponent sourceComponent)
		{
			return TransportMailItem.NewMailItem(null, sourceComponent, MailDirectionality.Undefined, Guid.Empty);
		}

		public static TransportMailItem NewMailItem(OrganizationId organizationId, LatencyComponent sourceComponent, MailDirectionality directionality = MailDirectionality.Undefined, Guid externalOrgId = default(Guid))
		{
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			return TransportMailItem.NewMailItem(new ADRecipientCache<TransportMiniRecipient>(TransportMiniRecipientSchema.Properties, 0, organizationId), sourceComponent, directionality, externalOrgId);
		}

		public static TransportMailItem NewMailItem(ADRecipientCache<TransportMiniRecipient> recipientCache, LatencyComponent sourceComponent, MailDirectionality directionality = MailDirectionality.Undefined, Guid externalOrgId = default(Guid))
		{
			TransportMailItem transportMailItem = new TransportMailItem(recipientCache, sourceComponent, directionality, externalOrgId);
			transportMailItem.audit.Drop(Breadcrumb.NewItem);
			return transportMailItem;
		}

		public static TransportMailItem NewMailItem(IMailItemStorage storage, LatencyComponent sourceComponent)
		{
			ArgumentValidator.ThrowIfNull("storage", storage);
			return new TransportMailItem(null, sourceComponent, storage, MailDirectionality.Undefined, default(Guid));
		}

		public static TransportMailItem NewAgentMailItem(ITransportMailItemFacade originalMailItem)
		{
			ArgumentValidator.ThrowIfNull("originalMailItem", originalMailItem);
			return TransportMailItem.NewSideEffectMailItem(null, LatencyComponent.Agent, MailDirectionality.Originating, originalMailItem.ExternalOrganizationId, originalMailItem.OriginalFrom);
		}

		public static TransportMailItem NewSideEffectMailItem(IReadOnlyMailItem originalMailItem)
		{
			if (originalMailItem == null)
			{
				throw new ArgumentNullException("originalMailItem");
			}
			ADRecipientCache<TransportMiniRecipient> adrecipientCache = null;
			if (originalMailItem.OrganizationId != null)
			{
				adrecipientCache = new ADRecipientCache<TransportMiniRecipient>(TransportMiniRecipientSchema.Properties, 0, originalMailItem.OrganizationId);
			}
			return TransportMailItem.NewSideEffectMailItem(adrecipientCache, LatencyComponent.Agent, MailDirectionality.Originating, originalMailItem.ExternalOrganizationId, originalMailItem.OriginalFrom);
		}

		public static TransportMailItem NewSideEffectMailItem(IReadOnlyMailItem originalMailItem, OrganizationId organizationId, LatencyComponent sourceComponent, MailDirectionality directionality = MailDirectionality.Undefined, Guid externalOrgId = default(Guid))
		{
			if (originalMailItem == null)
			{
				throw new ArgumentNullException("originalMailItem");
			}
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			return TransportMailItem.NewSideEffectMailItem(new ADRecipientCache<TransportMiniRecipient>(TransportMiniRecipientSchema.Properties, 0, organizationId), sourceComponent, directionality, externalOrgId, originalMailItem.OriginalFrom);
		}

		public static TransportMailItem LoadFromMsgId(long msgId, LatencyComponent sourceComponent)
		{
			IMailItemStorage mailItemStorage = TransportMailItem.Database.LoadMailItemFromId(msgId);
			if (mailItemStorage == null)
			{
				return null;
			}
			TransportMailItem transportMailItem = new TransportMailItem(null, sourceComponent, mailItemStorage, MailDirectionality.Undefined, default(Guid));
			foreach (IMailRecipientStorage recipStorage in TransportMailItem.Database.LoadMailRecipientsFromMessageId(msgId))
			{
				transportMailItem.AddRecipient(recipStorage);
			}
			return transportMailItem;
		}

		public static TransportMailItem FromMessageEnvelope(MessageEnvelope messageEnvelope, LatencyComponent sourceComponent)
		{
			return TransportMailItem.LoadFromMsgId(messageEnvelope.MsgId, sourceComponent);
		}

		public static void SetComponents(ITransportConfiguration configuration, ResourceManager resourceManager, ShadowRedundancyComponent shadowRedundancyComponent, IMessagingDatabase messagingDatabase)
		{
			TransportMailItem.configuration = configuration;
			TransportMailItem.resourceManager = resourceManager;
			TransportMailItem.shadowRedundancyComponent = shadowRedundancyComponent;
			TransportMailItem.database = messagingDatabase;
			if (TransportMailItem.configuration != null)
			{
				MimeCache.SetConfig(TransportMailItem.configuration.AppConfig.DeliveryQueuePrioritizationConfiguration.PriorityHeaderPromotionEnabled);
			}
		}

		public static void TrackAsyncMessage(string internetMessageId)
		{
			PoisonMessage.AddAsyncMessage(internetMessageId);
		}

		public static void TrackAsyncMessageCompleted(string internetMessageId)
		{
			PoisonMessage.RemoveAsyncMessage(internetMessageId);
		}

		public static bool SetPoisonContext(long recordId, string internetMessageId, MessageProcessingSource source)
		{
			if (PoisonMessage.Context != null)
			{
				MessageContext messageContext = PoisonMessage.Context as MessageContext;
				if (messageContext != null && messageContext.MessageId == recordId && ((messageContext.InternetMessageId != null && messageContext.InternetMessageId.Equals(internetMessageId)) || messageContext.InternetMessageId == internetMessageId) && messageContext.Source == source)
				{
					return false;
				}
			}
			PoisonMessage.Context = new MessageContext(recordId, internetMessageId, source);
			return true;
		}

		private static TransportMailItem NewSideEffectMailItem(ADRecipientCache<TransportMiniRecipient> recipientCache, LatencyComponent sourceComponent, MailDirectionality directionality, Guid externalOrgId, RoutingAddress originalFromAddress)
		{
			TransportMailItem transportMailItem = new TransportMailItem(recipientCache, sourceComponent, directionality, externalOrgId);
			transportMailItem.audit.Drop(Breadcrumb.NewSideEffectItem);
			transportMailItem.ExternalOrganizationId = externalOrgId;
			transportMailItem.Directionality = directionality;
			transportMailItem.storage.AttributedFromAddress = originalFromAddress.ToString();
			if (originalFromAddress == RoutingAddress.Empty)
			{
				ExTraceGlobals.GeneralTracer.TraceError<long>(0L, "Side-effect message with id {0} created without an original from address", transportMailItem.MsgId);
			}
			return transportMailItem;
		}

		public WaitCondition GetCondition()
		{
			if (this.CurrentCondition != null)
			{
				return this.CurrentCondition;
			}
			WaitCondition waitCondition;
			if (this.IsReplayMessage)
			{
				waitCondition = TransportMailItem.MessageRepositoryResubmitterCondition;
			}
			else if (Components.TransportAppConfig.ThrottlingConfig.CategorizerTenantThrottlingEnabled)
			{
				waitCondition = new TenantBasedCondition(this.ExternalOrganizationId);
			}
			else
			{
				if (!Components.TransportAppConfig.ThrottlingConfig.CategorizerSenderThrottlingEnabled)
				{
					return null;
				}
				waitCondition = new SenderBasedCondition(TransportMailItem.GetSourceID(this));
			}
			this.CurrentCondition = waitCondition;
			return waitCondition;
		}

		public MessageEnvelope GetMessageEnvelope()
		{
			List<string> recipients = (from mailRecipient in this.Recipients
			select mailRecipient.ToString()).ToList<string>();
			MessageEnvelope messageEnvelope = new MessageEnvelope(this.Priority, this.ExternalOrganizationId, this.DateReceived, this.From, this.Directionality, this.MimeDocument, this.MimeSize, this.Subject, this.MsgId, recipients);
			if (this.ExoAccountForest != null)
			{
				messageEnvelope.AddProperty<string>(MessageEnvelope.AccountForestProperty, this.ExoAccountForest);
			}
			return messageEnvelope;
		}

		public bool IsJournalReport()
		{
			return this.storage.IsJournalReport;
		}

		public bool IsPfReplica()
		{
			int num;
			return this.ExtendedProperties.TryGetValue<int>("Microsoft.Exchange.Transport.DirectoryData.RecipientType", out num) && num == 12;
		}

		public void PrepareJournalReport()
		{
			this.ThrowIfDeletedOrReadOnly();
			this.ThrowIfInAsyncCommit();
			if (this.From.Equals(RoutingAddress.NullReversePath) && VariantConfiguration.InvariantNoFlightingSnapshot.Transport.SetMustDeliverJournalReport.Enabled)
			{
				this.SetMustDeliver();
			}
			this.ExtendedProperties.SetValue<string>("Microsoft.Exchange.ContentIdentifier", "EXJournalData");
			HeaderList headers = this.RootPart.Headers;
			if (headers.FindFirst("X-MS-Exchange-Organization-Journal-Report") == null)
			{
				headers.AppendChild(new TextHeader("X-MS-Exchange-Organization-Journal-Report", null));
			}
			if (headers.FindFirst("X-MS-Journal-Report") == null)
			{
				headers.AppendChild(new TextHeader("X-MS-Journal-Report", null));
			}
			if (this.Priority == DeliveryPriority.Normal)
			{
				this.PrioritizationReason = "JournalReport";
				this.Priority = DeliveryPriority.Low;
			}
		}

		public void SetMustDeliver()
		{
			this.ThrowIfDeletedOrReadOnly();
			this.ThrowIfInAsyncCommit();
			this.storage.RetryDeliveryIfRejected = true;
			this.storage.TransportPropertiesHeader.SetBoolValue("MustDeliver", true);
		}

		public void SetQueuedForDelivery(bool value)
		{
			if (this.queuedForDelivery == value)
			{
				return;
			}
			this.storage.IsReadOnly = value;
			this.queuedForDelivery = value;
		}

		public bool ValidateDeliveryPriority(out SmtpResponse failureResponse)
		{
			this.ThrowIfDeletedOrReadOnly();
			this.ThrowIfInAsyncCommit();
			failureResponse = SmtpResponse.Empty;
			if (!TransportMailItem.configuration.LocalServer.IsBridgehead)
			{
				return true;
			}
			if (!TransportMailItem.configuration.AppConfig.RemoteDelivery.PriorityQueuingEnabled)
			{
				return true;
			}
			this.CheckDeliveryPriority();
			if (((IQueueItem)this).Priority == DeliveryPriority.High && TransportMailItem.configuration.AppConfig.RemoteDelivery.MaxHighPriorityMessageSize.ToBytes() < (ulong)this.MimeSize)
			{
				((IQueueItem)this).Priority = DeliveryPriority.Normal;
				ExTraceGlobals.GeneralTracer.TraceDebug<long>(0L, "Message with Id {0} exceeds the maximum size allowed for high-priority messages. Priority downgraded to Normal.", this.RecordId);
			}
			return true;
		}

		public Stream OpenMimeReadStream()
		{
			return this.OpenMimeReadStream(false);
		}

		public Stream OpenMimeReadStream(bool downConvert)
		{
			return this.storage.OpenMimeReadStream(downConvert);
		}

		public Stream OpenMimeWriteStream()
		{
			return this.OpenMimeWriteStream(MimeLimits.Unlimited, true);
		}

		public Stream OpenMimeWriteStream(MimeLimits mimeLimits)
		{
			return this.OpenMimeWriteStream(mimeLimits, true);
		}

		public Stream OpenMimeWriteStream(MimeLimits mimeLimits, bool expectBinaryContent)
		{
			return this.storage.OpenMimeWriteStream(mimeLimits, expectBinaryContent);
		}

		public long GetCurrrentMimeSize()
		{
			return this.storage.GetCurrrentMimeSize();
		}

		public long RefreshMimeSize()
		{
			return this.storage.RefreshMimeSize();
		}

		public void ResetMimeParserEohCallback()
		{
			this.storage.ResetMimeParserEohCallback();
		}

		public void AddDsnParameters(string key, object value)
		{
			this.ThrowIfDeletedOrReadOnly();
			this.ThrowIfInAsyncCommit();
			if (this.dsnParameters == null)
			{
				DsnParameters value2 = new DsnParameters();
				Interlocked.CompareExchange<DsnParameters>(ref this.dsnParameters, value2, null);
			}
			lock (this.dsnParameters)
			{
				this.dsnParameters[key] = value;
			}
		}

		public void IncrementPoisonForRemoteCount()
		{
			throw new NotImplementedException();
		}

		public void Ack(AckStatus ackStatus, SmtpResponse smtpResponse, IEnumerable<MailRecipient> solutionRecipients, Queue<AckStatusAndResponse> recipientResponses)
		{
			IList<MailRecipient> recipientsToBeResubmitted = new List<MailRecipient>();
			this.audit.Drop(Breadcrumb.AcknowledgeA);
			bool flag;
			bool flag2;
			this.Ack(ackStatus, smtpResponse, null, solutionRecipients, AdminActionStatus.None, recipientResponses, null, recipientsToBeResubmitted, out flag, out flag2);
		}

		public bool TryCreateExportStream(out Stream stream)
		{
			return ExportStream.TryCreate(this, this.Recipients, false, out stream);
		}

		public void Ack(AckStatus messageAckStatus, SmtpResponse smtpResponse, AckDetails ackDetails, IEnumerable<MailRecipient> recipients, AdminActionStatus adminActionStatus, Queue<AckStatusAndResponse> recipientResponses, Destination deliveredDestination, IList<MailRecipient> recipientsToBeResubmitted, out bool shouldEnqueueActive, out bool shouldEnqueueRetry)
		{
			this.audit.Drop(Breadcrumb.AcknowledgeB);
			shouldEnqueueActive = false;
			shouldEnqueueRetry = false;
			bool flag = this.RetryDeliveryIfRejected && adminActionStatus != AdminActionStatus.PendingDeleteWithOutNDR && adminActionStatus != AdminActionStatus.PendingDeleteWithNDR;
			if (flag && messageAckStatus == AckStatus.Fail)
			{
				messageAckStatus = AckStatus.Retry;
				smtpResponse = TransportMailItem.ReplaceFailWithRetryResponse(smtpResponse);
				if (recipientResponses == null)
				{
					recipientResponses = new Queue<AckStatusAndResponse>(0);
				}
			}
			if (recipientResponses == null && messageAckStatus != AckStatus.Fail)
			{
				throw new ArgumentException("ackStatus must be fail when recipientResponses is null");
			}
			if (adminActionStatus == AdminActionStatus.PendingDeleteWithOutNDR)
			{
				foreach (MailRecipient mailRecipient in recipients)
				{
					if (mailRecipient.Status == Status.Ready || mailRecipient.Status == Status.Retry || mailRecipient.Status == Status.Locked)
					{
						mailRecipient.DsnRequested = DsnRequestedFlags.Never;
					}
				}
			}
			bool flag2 = recipientResponses != null && recipientResponses.Count > 0;
			bool flag5;
			if (messageAckStatus == AckStatus.Pending || messageAckStatus == AckStatus.Retry)
			{
				bool flag3 = messageAckStatus == AckStatus.Pending && smtpResponse.Equals(SmtpResponse.Empty) && (recipientResponses == null || recipientResponses.Count == 0);
				IEnumerator<MailRecipient> enumerator2 = recipients.GetEnumerator();
				bool flag4 = messageAckStatus == AckStatus.Pending;
				TransportMailItem.DetermineEnqueueActiveOrRetry(flag4, !flag4, flag, out shouldEnqueueActive, out shouldEnqueueRetry, out flag5);
				IL_1EB:
				while (recipientResponses != null)
				{
					if (recipientResponses.Count <= 0)
					{
						break;
					}
					AckStatusAndResponse ackStatusAndResponse = recipientResponses.Dequeue();
					while (enumerator2.MoveNext())
					{
						MailRecipient mailRecipient2 = enumerator2.Current;
						if (mailRecipient2.Status == Status.Ready)
						{
							if (ackStatusAndResponse.AckStatus == AckStatus.Resubmit)
							{
								recipientsToBeResubmitted.Add(mailRecipient2);
								mailRecipient2.Ack(ackStatusAndResponse.AckStatus, ackStatusAndResponse.SmtpResponse);
								goto IL_1EB;
							}
							if (ackStatusAndResponse.AckStatus == AckStatus.Fail)
							{
								if (flag)
								{
									mailRecipient2.Ack(AckStatus.Retry, TransportMailItem.ReplaceFailWithRetryResponse(ackStatusAndResponse.SmtpResponse));
									goto IL_1EB;
								}
								mailRecipient2.Ack(ackStatusAndResponse.AckStatus, ackStatusAndResponse.SmtpResponse);
								goto IL_1EB;
							}
							else
							{
								if (ackStatusAndResponse.AckStatus == AckStatus.Retry && ackStatusAndResponse.SmtpResponse.SmtpResponseType != SmtpResponseType.Success && messageAckStatus != AckStatus.Pending)
								{
									mailRecipient2.Ack(ackStatusAndResponse.AckStatus, ackStatusAndResponse.SmtpResponse);
									goto IL_1EB;
								}
								mailRecipient2.Ack(messageAckStatus, smtpResponse);
								goto IL_1EB;
							}
						}
					}
					throw new InvalidOperationException("recipient collection must have changed");
				}
				while (enumerator2.MoveNext())
				{
					MailRecipient mailRecipient2 = enumerator2.Current;
					if (mailRecipient2.Status == Status.Ready && !flag3)
					{
						mailRecipient2.Ack(messageAckStatus, smtpResponse);
					}
				}
			}
			else
			{
				if (messageAckStatus == AckStatus.Success)
				{
					TransportMailItem.DetermineEnqueueActiveOrRetry(recipientResponses, flag, out shouldEnqueueActive, out shouldEnqueueRetry, out flag5);
					IEnumerator<MailRecipient> enumerator3 = recipients.GetEnumerator();
					IL_318:
					while (recipientResponses.Count > 0)
					{
						AckStatusAndResponse ackStatusAndResponse2 = recipientResponses.Dequeue();
						while (enumerator3.MoveNext())
						{
							MailRecipient mailRecipient3 = enumerator3.Current;
							if (mailRecipient3.Status == Status.Ready)
							{
								if (flag && ackStatusAndResponse2.AckStatus == AckStatus.Fail)
								{
									ackStatusAndResponse2 = new AckStatusAndResponse(AckStatus.Retry, TransportMailItem.ReplaceFailWithRetryResponse(ackStatusAndResponse2.SmtpResponse));
								}
								if (ackStatusAndResponse2.AckStatus == AckStatus.Resubmit)
								{
									recipientsToBeResubmitted.Add(mailRecipient3);
									mailRecipient3.Ack(ackStatusAndResponse2.AckStatus, ackStatusAndResponse2.SmtpResponse);
									goto IL_318;
								}
								if (ackStatusAndResponse2.AckStatus == AckStatus.Retry && shouldEnqueueActive)
								{
									mailRecipient3.Ack(AckStatus.Pending, smtpResponse);
									goto IL_318;
								}
								if ((ackStatusAndResponse2.AckStatus == AckStatus.Success || ackStatusAndResponse2.AckStatus == AckStatus.SuccessNoDsn) && deliveredDestination != null)
								{
									mailRecipient3.DeliveredDestination = deliveredDestination;
								}
								mailRecipient3.Ack(ackStatusAndResponse2.AckStatus, ackStatusAndResponse2.SmtpResponse);
								goto IL_318;
							}
						}
						throw new InvalidOperationException("recipient collection must have changed");
					}
					if (flag2)
					{
						while (enumerator3.MoveNext())
						{
							MailRecipient mailRecipient3 = enumerator3.Current;
							if (mailRecipient3.Status == Status.Ready)
							{
								mailRecipient3.Ack(AckStatus.Pending, smtpResponse);
								shouldEnqueueActive = true;
							}
						}
					}
					if (!this.IsHeartbeat)
					{
						goto IL_4E7;
					}
					using (IEnumerator<MailRecipient> enumerator4 = this.Recipients.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							MailRecipient mailRecipient4 = enumerator4.Current;
							mailRecipient4.Ack(AckStatus.SuccessNoDsn, SmtpResponse.Empty);
						}
						goto IL_4E7;
					}
				}
				bool flag6 = smtpResponse.Equals(SmtpResponse.NoRecipientSucceeded);
				TransportMailItem.DetermineEnqueueActiveOrRetry(recipientResponses, flag, out shouldEnqueueActive, out shouldEnqueueRetry, out flag5);
				foreach (MailRecipient mailRecipient5 in recipients)
				{
					if (mailRecipient5.Status == Status.Ready)
					{
						if (recipientResponses != null && recipientResponses.Count > 0)
						{
							AckStatusAndResponse ackStatusAndResponse3 = recipientResponses.Dequeue();
							if (ackStatusAndResponse3.AckStatus == AckStatus.Resubmit)
							{
								recipientsToBeResubmitted.Add(mailRecipient5);
								mailRecipient5.Ack(ackStatusAndResponse3.AckStatus, ackStatusAndResponse3.SmtpResponse);
							}
							else if (flag6 || ackStatusAndResponse3.AckStatus == AckStatus.Fail)
							{
								mailRecipient5.Ack(ackStatusAndResponse3.AckStatus, ackStatusAndResponse3.SmtpResponse);
							}
							else if (ackStatusAndResponse3.AckStatus == AckStatus.Pending || ackStatusAndResponse3.AckStatus == AckStatus.Retry)
							{
								if (shouldEnqueueActive && shouldEnqueueRetry)
								{
									mailRecipient5.Ack(AckStatus.Pending, smtpResponse);
								}
								else
								{
									mailRecipient5.Ack(ackStatusAndResponse3.AckStatus, ackStatusAndResponse3.SmtpResponse);
								}
							}
							else
							{
								mailRecipient5.Ack(AckStatus.Fail, smtpResponse);
							}
						}
						else if (flag2)
						{
							mailRecipient5.Ack(AckStatus.Pending, smtpResponse);
						}
						else
						{
							mailRecipient5.Ack(AckStatus.Fail, smtpResponse);
						}
					}
					else if (mailRecipient5.Status == Status.Retry || mailRecipient5.Status == Status.Locked)
					{
						mailRecipient5.Ack(AckStatus.Fail, smtpResponse);
					}
				}
			}
			IL_4E7:
			if (flag5)
			{
				TransportMailItem.Logger.LogEvent(TransportEventLogConstants.Tuple_RetryDeliveryIfRejected, "RetryDeliveryIfRejected", new object[]
				{
					this.MsgId
				});
			}
			if ((shouldEnqueueActive || shouldEnqueueRetry) && (adminActionStatus == AdminActionStatus.PendingDeleteWithNDR || adminActionStatus == AdminActionStatus.PendingDeleteWithOutNDR))
			{
				shouldEnqueueActive = false;
				shouldEnqueueRetry = false;
				foreach (MailRecipient mailRecipient6 in recipients)
				{
					if (mailRecipient6.Status == Status.Ready || mailRecipient6.Status == Status.Retry || mailRecipient6.Status == Status.Locked)
					{
						mailRecipient6.Ack(AckStatus.Fail, AckReason.MessageDelayedDeleteByAdmin);
					}
				}
			}
		}

		public void MinimizeMemory()
		{
			this.storage.MinimizeMemory();
			if (this.ADRecipientCache != null)
			{
				this.ADRecipientCache.Clear();
			}
			TransportMailItem.resourceManager.HintGCCollectCouldBeEffective();
		}

		public void CommitLazyAndDehydrate(Breadcrumb breadcrumb)
		{
			this.audit.Drop(breadcrumb);
			this.CommitAndDehydrate(TransactionCommitMode.MediumLatencyLazy);
		}

		public void CommitLazyAndDehydrateMessageIfPossible(Breadcrumb breadcrumb)
		{
			this.audit.Drop(breadcrumb);
			this.CommitAndDehydrateIfPossible(TransactionCommitMode.MediumLatencyLazy);
		}

		public void CommitLazyAndDehydrate(Breadcrumb breadcrumb, NextHopSolution solution)
		{
			if (this.IsRowDeleted && solution != null)
			{
				throw new InvalidOperationException("Commit and dehydration for a specific solution cannot be performed when deleting a mail item");
			}
			this.RunScopedDatabaseOperation(solution, delegate
			{
				this.CommitLazyAndDehydrate(breadcrumb);
			});
		}

		public void CommitLazyAndDehydrateMessageIfPossible(Breadcrumb breadcrumb, NextHopSolution solution)
		{
			if (this.IsRowDeleted && solution != null)
			{
				throw new InvalidOperationException("Commit and dehydration if possible for a specific solution cannot be performed when deleting a mail item");
			}
			this.RunScopedDatabaseOperation(solution, delegate
			{
				this.CommitLazyAndDehydrateMessageIfPossible(breadcrumb);
			});
		}

		public void UpdateNextHopSolutionTable(NextHopSolutionKey key, MailRecipient recipient)
		{
			NextHopSolution nextHopSolution;
			if (!this.nextHopSolutionTable.TryGetValue(key, out nextHopSolution))
			{
				nextHopSolution = ((key.NextHopType != NextHopType.Unreachable) ? new NextHopSolution(key) : new UnreachableSolution(key));
				this.nextHopSolutionTable.Add(key, nextHopSolution);
			}
			nextHopSolution.AddRecipient(recipient);
			UnreachableSolution unreachableSolution = nextHopSolution as UnreachableSolution;
			if (unreachableSolution == null)
			{
				return;
			}
			if (recipient.UnreachableReason != UnreachableReason.None)
			{
				unreachableSolution.AddUnreachableReason(recipient.UnreachableReason);
				return;
			}
			throw new InvalidOperationException("Recipient does not have unreachable reason");
		}

		public NextHopSolution UpdateNextHopSolutionTable(NextHopSolutionKey key, IEnumerable<MailRecipient> recipients, bool shouldCloneSolutionsTable)
		{
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			NextHopSolution nextHopSolution;
			if (!this.nextHopSolutionTable.TryGetValue(key, out nextHopSolution))
			{
				nextHopSolution = ((key.NextHopType != NextHopType.Unreachable) ? new NextHopSolution(key) : new UnreachableSolution(key));
				if (shouldCloneSolutionsTable)
				{
					this.nextHopSolutionTable = new Dictionary<NextHopSolutionKey, NextHopSolution>(this.NextHopSolutions)
					{
						{
							key,
							nextHopSolution
						}
					};
				}
				else
				{
					this.nextHopSolutionTable.Add(key, nextHopSolution);
				}
			}
			nextHopSolution.AddRecipients(recipients);
			UnreachableSolution unreachableSolution = nextHopSolution as UnreachableSolution;
			if (unreachableSolution != null)
			{
				foreach (MailRecipient mailRecipient in recipients)
				{
					if (mailRecipient.UnreachableReason == UnreachableReason.None)
					{
						throw new InvalidOperationException("Recipient does not have unreachable reason");
					}
					unreachableSolution.AddUnreachableReason(mailRecipient.UnreachableReason);
				}
			}
			return nextHopSolution;
		}

		public void BumpExpirationTime()
		{
			this.ExtensionToExpiryDuration = DateTime.UtcNow - this.DateReceived;
		}

		public void SetExpirationTime(DateTime requestedExpiryTime)
		{
			DateTime expiry = this.Expiry;
			if (expiry != DateTime.MaxValue)
			{
				this.ExtensionToExpiryDuration = requestedExpiryTime - expiry + this.ExtensionToExpiryDuration;
			}
		}

		void IQueueItem.Update()
		{
			this.CommitLazyAndDehydrate(Breadcrumb.DehydrateOnMailItemUpdate);
		}

		MessageContext IQueueItem.GetMessageContext(MessageProcessingSource source)
		{
			return new MessageContext(this.RecordId, this.InternetMessageId, source);
		}

		public DateTime GetExpiryTime(bool honorRetryIfNotDelivered)
		{
			if (honorRetryIfNotDelivered && this.storage.RetryDeliveryIfRejected)
			{
				return DateTime.MaxValue;
			}
			TimeSpan t;
			if (TransportMailItem.configuration.AppConfig.RemoteDelivery.PriorityQueuingEnabled)
			{
				t = TransportMailItem.configuration.AppConfig.RemoteDelivery.MessageExpirationTimeout(((IQueueItem)this).Priority);
			}
			else
			{
				t = TransportMailItem.configuration.LocalServer.TransportServer.MessageExpirationTimeout;
			}
			return this.DateReceived + this.ExtensionToExpiryDuration + t;
		}

		public void CommitImmediate()
		{
			this.Commit(TransactionCommitMode.Immediate);
		}

		public void CommitLazy()
		{
			this.Commit(TransactionCommitMode.MediumLatencyLazy);
		}

		public void CommitLazy(NextHopSolution solution)
		{
			if (this.IsRowDeleted && solution != null)
			{
				throw new InvalidOperationException("Commit for a specific solution cannot be performed when deleting a mail item");
			}
			this.RunScopedDatabaseOperation(solution, delegate
			{
				this.CommitLazy();
			});
		}

		public IAsyncResult BeginCommitForReceive(AsyncCallback asyncCallback, object asyncState)
		{
			if (this.IsInAsyncCommit)
			{
				throw new InvalidOperationException("This mail item is already in an async commit.");
			}
			this.audit.Drop(Breadcrumb.CommitForReceive);
			return this.storage.BeginCommit(asyncCallback, asyncState);
		}

		public bool EndCommitForReceive(IAsyncResult ar, out Exception exception)
		{
			return this.storage.EndCommit(ar, out exception);
		}

		IAsyncResult ITransportMailItemFacade.BeginCommitForReceive(AsyncCallback asyncCallback, object asyncState)
		{
			return this.BeginCommitForReceive(asyncCallback, asyncState);
		}

		bool ITransportMailItemFacade.EndCommitForReceive(IAsyncResult ar, out Exception exception)
		{
			return this.EndCommitForReceive(ar, out exception);
		}

		public TransportMailItem NewCloneWithoutRecipients()
		{
			return this.NewCloneWithoutRecipients(true, this.latencyTracker);
		}

		public TransportMailItem NewCloneWithoutRecipients(bool shareRecipientCache)
		{
			return this.NewCloneWithoutRecipients(shareRecipientCache, this.latencyTracker);
		}

		public TransportMailItem NewCloneWithoutRecipients(bool shareRecipientCache, LatencyTracker latencyTrackerToClone)
		{
			this.audit.Drop(Breadcrumb.CommitNow);
			TransportMailItem transportMailItem;
			lock (this)
			{
				if (Components.TransportAppConfig.QueueDatabase.CloneInOriginalGeneration)
				{
					transportMailItem = new TransportMailItem(this, shareRecipientCache, latencyTrackerToClone, this.transportRulesForkCount, false);
				}
				else
				{
					using (Transaction transaction = TransportMailItem.Database.BeginNewTransaction())
					{
						this.storage.Materialize(transaction);
						transportMailItem = new TransportMailItem(this, shareRecipientCache, latencyTrackerToClone);
						transportMailItem.audit.Drop(Breadcrumb.CommitNow);
						transportMailItem.transportRulesForkCount = this.transportRulesForkCount;
						transportMailItem.storage.Materialize(transaction);
						transaction.Commit();
					}
				}
			}
			if (this.IsShadowed() && TransportMailItem.shadowRedundancyComponent != null)
			{
				TransportMailItem.shadowRedundancyComponent.ShadowRedundancyManager.NotifyMailItemBifurcated(this.ShadowServerContext, this.ShadowServerDiscardId);
			}
			return transportMailItem;
		}

		public TransportMailItem CreateNewCopyWithoutRecipients(NextHopSolution scopingSolution)
		{
			TransportMailItem copy = null;
			this.RunScopedDatabaseOperation(scopingSolution, delegate
			{
				copy = this.CreateNewCopyWithoutRecipients();
			});
			return copy;
		}

		public TransportMailItem CreateNewCopyWithoutRecipients()
		{
			this.audit.Drop(Breadcrumb.CommitNow);
			TransportMailItem result;
			lock (this)
			{
				result = new TransportMailItem(this, false, null, this.transportRulesForkCount, true);
			}
			if (this.IsShadowed() && TransportMailItem.shadowRedundancyComponent != null)
			{
				TransportMailItem.shadowRedundancyComponent.ShadowRedundancyManager.NotifyMailItemBifurcated(this.ShadowServerContext, this.ShadowServerDiscardId);
			}
			return result;
		}

		public TransportMailItem NewCloneWithoutRecipients(bool shareRecipientCache, LatencyTracker latencyTrackerToClone, NextHopSolution scopingSolution)
		{
			TransportMailItem clone = null;
			this.RunScopedDatabaseOperation(scopingSolution, delegate
			{
				clone = this.NewCloneWithoutRecipients(shareRecipientCache, latencyTrackerToClone);
			});
			return clone;
		}

		public void UpdateDirectionalityAndScopeHeaders()
		{
			string value;
			switch (this.Directionality)
			{
			case MailDirectionality.Undefined:
				throw new InvalidOperationException("Cannot set directionality to Undefined");
			case MailDirectionality.Originating:
				value = MultiTenantTransport.OriginatingStr;
				break;
			case MailDirectionality.Incoming:
				value = MultiTenantTransport.IncomingStr;
				break;
			default:
				throw new InvalidOperationException(string.Format("Unexpected directionality: {0}", this.Directionality));
			}
			Util.SetAsciiHeader(this.RootPart.Headers, "X-MS-Exchange-Organization-MessageDirectionality", value);
			Util.SetAsciiHeader(this.RootPart.Headers, "X-MS-Exchange-Organization-Id", this.ExternalOrganizationId.ToString());
			Util.SetScopeHeaders(this.RootPart.Headers, this.OrganizationScope);
		}

		public void ReleaseFromActiveMaterializedLazy()
		{
			this.audit.Drop(Breadcrumb.MailItemDeleted);
			if (this.IsShadowed() && TransportMailItem.shadowRedundancyComponent != null)
			{
				TransportMailItem.shadowRedundancyComponent.ShadowRedundancyManager.NotifyMailItemReleased(this);
			}
			this.ReleaseFromActive();
			this.CommitLazy();
		}

		public void ReleaseFromActive()
		{
			this.Recipients.Clear();
			if (this.IsActive)
			{
				this.storage.ReleaseFromActive();
				if (this.OnReleaseFromActive != null)
				{
					this.OnReleaseFromActive(this);
				}
			}
		}

		public void ResetToActive()
		{
			if (!this.IsActive)
			{
				this.storage.IsActive = true;
			}
		}

		public void ReleaseFromShadowRedundancy()
		{
			if (this.IsShadow())
			{
				throw new InvalidOperationException("ReleaseFromShadowRedundancy() is called on MailItem while IsShadow() is true.");
			}
			bool flag = this.Status == Status.Complete || this.NextHopSolutions.Count == 0;
			if ((this.IsHeartbeat || flag) && this.IsActive)
			{
				this.ReleaseFromActive();
			}
			if (flag)
			{
				this.CommitLazyAndDehydrate(Breadcrumb.DehydrateOnReleaseFromShadowRedundancy, null);
				return;
			}
			this.CommitLazy();
		}

		public void ReleaseFromRemoteDelivery()
		{
			if (this.Status != Status.Complete)
			{
				throw new InvalidOperationException("ReleaseFromRemoteDelivery() is called on MailItem while Status != Status.Complete.");
			}
			this.audit.Drop(Breadcrumb.MailItemDelivered);
			this.SetQueuedForDelivery(false);
			if (this.IsShadowed() && TransportMailItem.shadowRedundancyComponent != null)
			{
				TransportMailItem.shadowRedundancyComponent.ShadowRedundancyManager.NotifyMailItemDelivered(this);
			}
			if (!this.IsShadow() && this.IsActive && !this.IsPoison)
			{
				this.ReleaseFromActive();
			}
			this.CommitLazyAndDehydrate(Breadcrumb.DehydrateOnReleaseFromRemoteDelivery);
		}

		public bool IsShadow()
		{
			if (this.NextHopSolutions.Count > 0)
			{
				using (Dictionary<NextHopSolutionKey, NextHopSolution>.KeyCollection.Enumerator enumerator = this.NextHopSolutions.Keys.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						NextHopSolutionKey nextHopSolutionKey = enumerator.Current;
						if (nextHopSolutionKey.NextHopType.DeliveryType == DeliveryType.ShadowRedundancy)
						{
							return true;
						}
					}
					return false;
				}
			}
			foreach (MailRecipient mailRecipient in this.Recipients)
			{
				if (!string.IsNullOrEmpty(mailRecipient.PrimaryServerFqdnGuid))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsShadowed()
		{
			return !string.IsNullOrEmpty(this.ShadowServerContext);
		}

		public bool IsDelayedAck()
		{
			return this.IsShadowed() && string.Equals(this.ShadowServerContext, "$localhost$", StringComparison.OrdinalIgnoreCase);
		}

		public bool IsShadowedByXShadow()
		{
			return this.IsShadowed() && !this.IsDelayedAck();
		}

		public void AddRecipient(IMailRecipientStorage recipStorage)
		{
			this.ThrowIfDeletedOrReadOnly();
			this.ThrowIfInAsyncCommit();
			MailRecipient mailRecipient = MailRecipient.NewMessageRecipient(this, recipStorage);
			if (this.LoadedInRestart && (mailRecipient.Status == Status.Retry || mailRecipient.Status == Status.Locked))
			{
				mailRecipient.Status = Status.Ready;
			}
			if (mailRecipient.AdminActionStatus == AdminActionStatus.SuspendedInSubmissionQueue)
			{
				((IQueueItem)this).DeferUntil = DateTime.MaxValue;
				if (this.Recipients.Count > 0)
				{
					this.Recipients.Prepend(mailRecipient);
					return;
				}
			}
			this.Recipients.Add(mailRecipient);
		}

		public void CacheTransportSettings()
		{
			if (this.transportSettings == null)
			{
				if (this.recipientCache == null)
				{
					throw new InvalidOperationException("ADRecipient cache should be set before calling this method");
				}
				Interlocked.Exchange<PerTenantTransportSettings>(ref this.transportSettings, Components.Configuration.GetTransportSettings(this.recipientCache.OrganizationId));
			}
		}

		public void ClearTransportSettings()
		{
			this.ThrowIfDeletedOrReadOnly();
			this.ThrowIfInAsyncCommit();
			this.transportSettings = null;
		}

		public void RestoreLastSavedMime(string agentName, string eventName)
		{
			this.storage.RestoreLastSavedMime();
			string messageId = this.Message.MessageId;
			TransportMailItem.Logger.LogEvent(TransportEventLogConstants.Tuple_AgentDidNotCloseMimeStream, null, new object[]
			{
				agentName,
				eventName,
				messageId
			});
		}

		public void Suspend()
		{
			if (this.Recipients[0].AdminActionStatus != AdminActionStatus.SuspendedInSubmissionQueue)
			{
				this.DeferUntil = DateTime.MaxValue;
				this.Recipients[0].AdminActionStatus = AdminActionStatus.SuspendedInSubmissionQueue;
				this.CommitLazy();
			}
		}

		public void Resume()
		{
			if (this.Recipients[0].AdminActionStatus == AdminActionStatus.SuspendedInSubmissionQueue)
			{
				this.DeferUntil = DateTime.MinValue;
				this.Recipients[0].AdminActionStatus = AdminActionStatus.None;
				this.CommitLazy();
			}
		}

		public void UpdateCachedHeaders()
		{
			this.storage.UpdateCachedHeaders();
		}

		public string GetReceiveConnectorName()
		{
			string text = this.ReceiveConnectorName;
			string text2 = "Replay:";
			if (text.StartsWith(text2, StringComparison.Ordinal) && text.Length > text2.Length)
			{
				text = text.Substring(text2.Length);
			}
			text2 = "SMTP:";
			if (text.StartsWith(text2, StringComparison.Ordinal) && text.Length > text2.Length)
			{
				text = text.Substring(text2.Length);
			}
			return text;
		}

		internal PerTenantRemoteDomainTable GetOrgRemoteDomains()
		{
			PerTenantRemoteDomainTable result = null;
			if (TransportMailItem.configuration == null)
			{
				return result;
			}
			if (this.OrganizationId == null || !TransportMailItem.configuration.TryGetRemoteDomainTable(this.OrganizationId, out result))
			{
				result = TransportMailItem.configuration.GetRemoteDomainTable(OrganizationId.ForestWideOrgId);
			}
			return result;
		}

		internal RemoteDomainEntry GetDefaultDomain()
		{
			PerTenantRemoteDomainTable orgRemoteDomains = this.GetOrgRemoteDomains();
			if (orgRemoteDomains == null)
			{
				return null;
			}
			return orgRemoteDomains.RemoteDomainTable.Star;
		}

		public void TrackSuccessfulConnectLatency(LatencyComponent connectComponent)
		{
			throw new NotImplementedException();
		}

		public void DropBreadcrumb(Breadcrumb breadcrumb)
		{
			this.audit.Drop(breadcrumb);
		}

		public void DropCatBreadcrumb(CategorizerBreadcrumb breadcrumb)
		{
			if (this.throttlingContext == null)
			{
				return;
			}
			this.throttlingContext.AddBreadcrumb(breadcrumb);
		}

		internal static LatencyComponent GetDeferLatencyComponent(DeferReason deferReason)
		{
			LatencyComponent result;
			if (deferReason != DeferReason.ReroutedByStoreDriver)
			{
				if (deferReason != DeferReason.RecipientThreadLimitExceeded)
				{
					result = LatencyComponent.Deferral;
				}
				else
				{
					result = LatencyComponent.DeliveryQueueMailboxRecipientThreadLimitExceeded;
				}
			}
			else
			{
				result = LatencyComponent.MailboxMove;
			}
			return result;
		}

		internal static string GetSourceID(TransportMailItem mailItem)
		{
			HeaderList headers = mailItem.RootPart.Headers;
			Header header = headers.FindFirst("X-MS-Exchange-Organization-AuthAs");
			bool flag = header != null && "Internal" == header.Value;
			string result;
			if (RoutingAddress.NullReversePath != mailItem.From && RoutingAddress.Empty != mailItem.From)
			{
				result = string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[]
				{
					mailItem.From.ToString().ToLowerInvariant(),
					flag ? null : "-u"
				});
			}
			else
			{
				Header header2 = headers.FindFirst(HeaderId.From);
				if (header2 == null || string.IsNullOrEmpty(header2.Value))
				{
					header2 = headers.FindFirst(HeaderId.Sender);
				}
				if (header2 != null && !string.IsNullOrEmpty(header2.Value))
				{
					result = string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[]
					{
						header2.Value.ToLowerInvariant(),
						flag ? null : "-u"
					});
				}
				else
				{
					result = ((mailItem.InternetMessageId == null) ? "NoMessageID" : mailItem.InternetMessageId.ToLowerInvariant());
				}
			}
			return result;
		}

		internal void AddAgentInfo(string agentName, string eventName, List<KeyValuePair<string, string>> data)
		{
			if (string.IsNullOrEmpty(agentName))
			{
				throw new ArgumentNullException("agentName");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (this.messageTrackingAgentInfo == null)
			{
				this.messageTrackingAgentInfo = new List<List<KeyValuePair<string, string>>>();
			}
			data.Insert(0, new KeyValuePair<string, string>(agentName, eventName));
			this.messageTrackingAgentInfo.Add(data);
		}

		internal List<List<KeyValuePair<string, string>>> ClaimAgentInfo()
		{
			this.MoveComponentCostToAgentInfo();
			List<List<KeyValuePair<string, string>>> result = this.messageTrackingAgentInfo;
			this.messageTrackingAgentInfo = null;
			return result;
		}

		internal void SetMimeDefaultEncoding()
		{
			RemoteDomainEntry fromDomainConfig = this.FromDomainConfig;
			RemoteDomainEntry defaultDomain = this.GetDefaultDomain();
			string text = null;
			string arg = string.Empty;
			if (fromDomainConfig != null && fromDomainConfig.NonMimeCharacterSet != null)
			{
				text = fromDomainConfig.NonMimeCharacterSet;
				arg = fromDomainConfig.DomainName.ToString();
			}
			else if (defaultDomain != null && defaultDomain.NonMimeCharacterSet != null)
			{
				text = defaultDomain.NonMimeCharacterSet;
				arg = defaultDomain.DomainName.ToString();
			}
			Encoding encoding;
			if (text != null && Charset.TryGetEncoding(text, out encoding))
			{
				Encoding mimeDefaultEncoding = this.storage.MimeDefaultEncoding;
				MimeDocument mimeDocument = this.MimeDocument;
				if (!encoding.Equals(mimeDefaultEncoding))
				{
					ExTraceGlobals.GeneralTracer.TraceDebug<string, Encoding>(0L, "Default encoding for content from domain {0} was configured as {1}", arg, encoding);
					this.storage.MimeDefaultEncoding = encoding;
				}
				if (mimeDocument != null && !encoding.Equals(mimeDocument.HeaderDecodingOptions.CharsetEncoding))
				{
					DecodingOptions decodingOptions = new DecodingOptions(mimeDocument.HeaderDecodingOptions.DecodingFlags, encoding);
					MimeInternalHelpers.SetDocumentDecodingOptions(mimeDocument, decodingOptions);
				}
			}
		}

		private static void CheckPendingAndRetryRecipients(Queue<AckStatusAndResponse> recipientResponses, bool mustDeliverJournalReport, out bool hasPendingRecipient, out bool hasRetryRecipient)
		{
			hasPendingRecipient = false;
			hasRetryRecipient = false;
			if (recipientResponses == null || recipientResponses.Count == 0)
			{
				return;
			}
			foreach (object obj in recipientResponses)
			{
				AckStatusAndResponse ackStatusAndResponse = (AckStatusAndResponse)obj;
				if (ackStatusAndResponse.AckStatus == AckStatus.Pending)
				{
					hasPendingRecipient = true;
				}
				else if (ackStatusAndResponse.AckStatus == AckStatus.Retry || (mustDeliverJournalReport && ackStatusAndResponse.AckStatus == AckStatus.Fail))
				{
					hasRetryRecipient = true;
				}
			}
		}

		private static void DetermineEnqueueActiveOrRetry(bool hasPendingRecipient, bool hasRetryRecipient, bool mustDeliverJournalReport, out bool shouldEnqueueActive, out bool shouldEnqueueRetry, out bool specialMessageGoesInRetry)
		{
			shouldEnqueueActive = false;
			shouldEnqueueRetry = false;
			specialMessageGoesInRetry = false;
			if (hasPendingRecipient)
			{
				shouldEnqueueActive = true;
				return;
			}
			if (hasRetryRecipient)
			{
				shouldEnqueueRetry = true;
				if (mustDeliverJournalReport)
				{
					specialMessageGoesInRetry = true;
				}
			}
		}

		private static void DetermineEnqueueActiveOrRetry(Queue<AckStatusAndResponse> recipientResponses, bool mustDeliverJournalReport, out bool shouldEnqueueActive, out bool shouldEnqueueRetry, out bool specialMessageGoesInRetry)
		{
			bool hasPendingRecipient;
			bool hasRetryRecipient;
			TransportMailItem.CheckPendingAndRetryRecipients(recipientResponses, mustDeliverJournalReport, out hasPendingRecipient, out hasRetryRecipient);
			TransportMailItem.DetermineEnqueueActiveOrRetry(hasPendingRecipient, hasRetryRecipient, mustDeliverJournalReport, out shouldEnqueueActive, out shouldEnqueueRetry, out specialMessageGoesInRetry);
		}

		private void MoveComponentCostToAgentInfo()
		{
			int length = "Microsoft.Exchange.Transport.TransportMailItem.NonforkingComponentCost.".Length;
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			List<string> list2 = new List<string>();
			foreach (KeyValuePair<string, object> keyValuePair in this.ExtendedPropertyDictionary)
			{
				if (keyValuePair.Key.StartsWith("Microsoft.Exchange.Transport.TransportMailItem.NonforkingComponentCost."))
				{
					list.Add(new KeyValuePair<string, string>(keyValuePair.Key.Substring(length), ((long)keyValuePair.Value).ToString()));
					list2.Add(keyValuePair.Key);
				}
			}
			if (list.Count > 0)
			{
				this.AddAgentInfo("CompCost", string.Empty, list);
				foreach (string key in list2)
				{
					this.ExtendedPropertyDictionary.Remove(key);
				}
			}
		}

		private DeliveryPriority EmailImportanceToDeliveryPriority()
		{
			switch (this.Message.Importance)
			{
			case Importance.Normal:
				return DeliveryPriority.Normal;
			case Importance.High:
				return DeliveryPriority.High;
			case Importance.Low:
				return DeliveryPriority.Low;
			default:
				throw new ArgumentException("Invalid EmailMessage.Importance value:" + this.Message.Importance);
			}
		}

		private DeliveryPriority EmailPriorityToDeliveryPriority()
		{
			switch (this.Message.Priority)
			{
			case Microsoft.Exchange.Data.Transport.Email.Priority.Normal:
				return DeliveryPriority.Normal;
			case Microsoft.Exchange.Data.Transport.Email.Priority.Urgent:
				return DeliveryPriority.High;
			case Microsoft.Exchange.Data.Transport.Email.Priority.NonUrgent:
				return DeliveryPriority.Low;
			default:
				throw new ArgumentException("Invalid EmailMessage.Priority value:" + this.Message.Priority);
			}
		}

		private DeliveryPriority GetRequestedMessagePriority()
		{
			DeliveryPriority deliveryPriority = this.EmailImportanceToDeliveryPriority();
			DeliveryPriority deliveryPriority2 = this.EmailPriorityToDeliveryPriority();
			if (deliveryPriority >= deliveryPriority2)
			{
				return deliveryPriority2;
			}
			return deliveryPriority;
		}

		private void CheckDeliveryPriority()
		{
			this.ThrowIfDeletedOrReadOnly();
			this.ThrowIfInAsyncCommit();
			if (this.storage.Priority != null)
			{
				ExTraceGlobals.GeneralTracer.TraceDebug<long>(0L, "Message with Id {0} has the internal priority already set.", this.RecordId);
				return;
			}
			DeliveryPriority requestedMessagePriority = this.GetRequestedMessagePriority();
			if (requestedMessagePriority != DeliveryPriority.High)
			{
				((IQueueItem)this).Priority = requestedMessagePriority;
				ExTraceGlobals.GeneralTracer.TraceDebug<long, DeliveryPriority>(0L, "Message with Id {0} has {1} priority. No other checks are needed.", this.RecordId, requestedMessagePriority);
				return;
			}
			if (!MultilevelAuth.IsAuthenticated(this))
			{
				((IQueueItem)this).Priority = DeliveryPriority.Normal;
				ExTraceGlobals.GeneralTracer.TraceDebug<long>(0L, "Message with Id {0} requested High delivery priority but was not sent from an authenticated source. Priority downgraded to Normal.", this.RecordId);
				return;
			}
			RoutingAddress outer;
			if (!Util.TryGetP2Sender(this.RootPart.Headers, out outer))
			{
				((IQueueItem)this).Priority = DeliveryPriority.Normal;
				ExTraceGlobals.GeneralTracer.TraceDebug<long>(0L, "Message with Id {0} requested High delivery priority but had no valid P2 sender. Priority downgraded to Normal.", this.RecordId);
				return;
			}
			ProxyAddress innermostAddress = Sender.GetInnermostAddress(outer);
			try
			{
				Result<TransportMiniRecipient> result = this.ADRecipientCache.FindAndCacheRecipient(innermostAddress);
				if (result.Data == null)
				{
					((IQueueItem)this).Priority = requestedMessagePriority;
					ExTraceGlobals.GeneralTracer.TraceDebug<ProxyAddress, long>(0L, "Sender {0} of message with Id {1} not found in AD. The message was sent from an authenticated source; the High delivery priority is honored.", innermostAddress, this.RecordId);
				}
				else
				{
					bool downgradeHighPriorityMessagesEnabled = result.Data.DowngradeHighPriorityMessagesEnabled;
					if (downgradeHighPriorityMessagesEnabled)
					{
						((IQueueItem)this).Priority = DeliveryPriority.Normal;
						ExTraceGlobals.GeneralTracer.TraceDebug<ProxyAddress, long>(0L, "Sender {0} of message with Id {1} does not have permissions to send High-priority messages. Priority downgraded to Normal.", innermostAddress, this.RecordId);
					}
					else
					{
						((IQueueItem)this).Priority = requestedMessagePriority;
						ExTraceGlobals.GeneralTracer.TraceDebug<ProxyAddress, long>(0L, "Sender {0}  of message with Id {1} has permissions to send High-priority messages.", innermostAddress, this.RecordId);
					}
				}
			}
			catch (ADTransientException)
			{
				ExTraceGlobals.GeneralTracer.TraceError<ProxyAddress, long>(0L, "Cannot verify if sender {0} of message {1} has permission to send High-priority email because of transient AD errors. Priority downgraded to Normal.", innermostAddress, this.RecordId);
				((IQueueItem)this).Priority = DeliveryPriority.Normal;
			}
		}

		private void RunScopedDatabaseOperation(NextHopSolution scopingSolution, Action operation)
		{
			if (scopingSolution == null)
			{
				operation();
				return;
			}
			MailRecipientCollection recipients = new MailRecipientCollection(this, scopingSolution.Recipients);
			MailRecipientCollection recipients2 = this.Recipients;
			this.Recipients = recipients;
			this.audit.Drop(Breadcrumb.ScopedRecipients);
			try
			{
				operation();
			}
			finally
			{
				this.Recipients = recipients2;
				this.audit.Drop(Breadcrumb.UnscopedRecipients);
			}
		}

		private void Commit(TransactionCommitMode commitMode)
		{
			this.audit.Drop((commitMode == TransactionCommitMode.Lazy) ? Breadcrumb.CommitLazy : Breadcrumb.CommitNow);
			lock (this)
			{
				this.storage.Commit(commitMode);
			}
		}

		private void CommitAndDehydrateIfPossible(TransactionCommitMode commitMode)
		{
			if (Monitor.TryEnter(this))
			{
				try
				{
					if (this.IsInDelivery)
					{
						this.audit.Drop(Breadcrumb.DehydrationSkippedItemInDelivery);
					}
					else
					{
						this.CommitAndDehydrate(commitMode);
					}
					return;
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
			this.audit.Drop(Breadcrumb.DehydrationSkippedItemLock);
		}

		private void CommitAndDehydrate(TransactionCommitMode commitMode)
		{
			lock (this)
			{
				this.Commit(commitMode);
				if (!this.IsRowDeleted)
				{
					this.MinimizeMemory();
					this.audit.Drop(Breadcrumb.DehydrateMinimizedMemory);
				}
			}
		}

		private void RemoveFirewalledProperties(IDictionary<string, object> extendedProperties)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, object> keyValuePair in extendedProperties)
			{
				if (keyValuePair.Key.StartsWith("Microsoft.Exchange.Transport.TransportMailItem.Nonforking"))
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (string key in list)
			{
				extendedProperties.Remove(key);
			}
		}

		private void ThrowIfReadOnly()
		{
			if (this.storage.IsReadOnly)
			{
				throw new InvalidOperationException("This operation cannot be performed after mail item has been queued for delivery.");
			}
		}

		private void ThrowIfInAsyncCommit()
		{
			if (this.IsInAsyncCommit)
			{
				throw new InvalidOperationException("This operation cannot be performed when mail item is in Async Commit");
			}
		}

		private void ThrowIfDeletedOrReadOnly()
		{
			this.ThrowIfDeleted();
			this.ThrowIfReadOnly();
		}

		private void ThrowIfDeleted()
		{
			if (this.IsRowDeleted)
			{
				throw new InvalidOperationException("operations not allowed on a deleted mail item");
			}
		}

		public const string SmtpInReceiveConnectorNamePrefix = "SMTP:";

		public const string ReplayReceiveConnectorNamePrefix = "Replay:";

		public const string AgentReceiveConnectorNamePrefix = "Agent:";

		public const string DeliveryPriorityStamped = "Microsoft.Exchange.Transport.TransportMailItem.DeliveryPriority";

		public const string PrioritizationReasonLabel = "Microsoft.Exchange.Transport.TransportMailItem.PrioritizationReason";

		public const string RiskLevelLabel = "Microsoft.Exchange.Transport.TransportMailItem.RiskLevel";

		public const string ExoAccountForestLabel = "Microsoft.Exchange.Transport.TransportMailItem.ExoAccountForest";

		public const string ExoTenantContainerLabel = "Microsoft.Exchange.Transport.TransportMailItem.ExoTenantContainer";

		public const string ExternalOrganizationIdLabel = "Microsoft.Exchange.Transport.TransportMailItem.ExternalOrganizationId";

		public const string SystemProbeIdLabel = "Microsoft.Exchange.Transport.TransportMailItem.SystemProbeId";

		public const string InboundProxySequenceNumberLabel = "Microsoft.Exchange.Transport.TransportMailItem.InboundProxySequenceNumber";

		public const string AttributedFromAddressLabel = "Microsoft.Exchange.Transport.TransportMailItem.AttributedFromAddress";

		public const string ComponentCostLabel = "Microsoft.Exchange.Transport.TransportMailItem.NonforkingComponentCost.";

		private const string NonforkingPropertyPrefix = "Microsoft.Exchange.Transport.TransportMailItem.Nonforking";

		protected bool loadedInRestart;

		protected Breadcrumbs audit;

		private static readonly Guid componentGuid = new Guid("{b53361f0-113c-4cfa-b12f-70781b6f187d}");

		private static readonly ResubmitterBasedCondition MessageRepositoryResubmitterCondition = new ResubmitterBasedCondition("MessageRepository");

		private static IMessagingDatabase database;

		private static ITransportConfiguration configuration;

		private static ResourceManager resourceManager;

		private static ShadowRedundancyComponent shadowRedundancyComponent;

		private static ExEventLog eventLogger;

		private IMailItemStorage storage;

		private QueuedRecipientsByAgeToken queuedRecipientsByAgeToken;

		private DateTime deferUntil;

		private DeferReason deferReason;

		private Dictionary<NextHopSolutionKey, NextHopSolution> nextHopSolutionTable;

		private SnapshotWriterState snapshotState;

		private bool? pipelineTracingEnabled;

		private string pipelineTracingPath;

		private ADRecipientCache<TransportMiniRecipient> recipientCache;

		private bool exposeMessage;

		private bool exposeMessageHeaders;

		private MultilevelAuthMechanism authMethod;

		private string messageTrackingSecurityInfo;

		private List<List<KeyValuePair<string, string>>> messageTrackingAgentInfo;

		private bool queuedForDelivery;

		private DateTime routingTimeStamp;

		private LatencyTracker latencyTracker;

		private IActivityScope activityScope;

		private PerTenantTransportSettings transportSettings;

		private DsnParameters dsnParameters;

		private bool routeForHighAvailability;

		private AccessToken accessToken;

		private DateTimeOffset lockExpirationTime;

		private ThrottlingContext throttlingContext;

		private WaitCondition waitCondition;

		private string lockReason;

		private List<string> lockReasonHistory;

		private ForkCount transportRulesForkCount;

		private readonly QueueQuotaTrackingBits queueQuotaTrackingBits;

		private DateTime latencyStartTime;
	}
}
