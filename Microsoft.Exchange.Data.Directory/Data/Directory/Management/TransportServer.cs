using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public sealed class TransportServer : ADPresentationObject
	{
		public TransportServer()
		{
		}

		public TransportServer(Server dataObject) : base(dataObject)
		{
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				if (TopologyProvider.IsAdamTopology())
				{
					return TransportServer.schema;
				}
				return TransportServer.adSchema;
			}
		}

		public new string Name
		{
			get
			{
				return (string)this[ADObjectSchema.Name];
			}
		}

		[Parameter(Mandatory = false)]
		public bool AntispamAgentsEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.AntispamAgentsEnabled];
			}
			set
			{
				this[TransportServerSchema.AntispamAgentsEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ConnectivityLogEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.ConnectivityLogEnabled];
			}
			set
			{
				this[TransportServerSchema.ConnectivityLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ConnectivityLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.ConnectivityLogMaxAge];
			}
			set
			{
				this[TransportServerSchema.ConnectivityLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ConnectivityLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[TransportServerSchema.ConnectivityLogMaxDirectorySize];
			}
			set
			{
				this[TransportServerSchema.ConnectivityLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ConnectivityLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[TransportServerSchema.ConnectivityLogMaxFileSize];
			}
			set
			{
				this[TransportServerSchema.ConnectivityLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ConnectivityLogPath
		{
			get
			{
				return (LocalLongFullPath)this[TransportServerSchema.ConnectivityLogPath];
			}
			set
			{
				this[TransportServerSchema.ConnectivityLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan DelayNotificationTimeout
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.DelayNotificationTimeout];
			}
			set
			{
				this[TransportServerSchema.DelayNotificationTimeout] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExternalDNSAdapterEnabled
		{
			get
			{
				return !(bool)this[TransportServerSchema.ExternalDNSAdapterDisabled];
			}
			set
			{
				this[TransportServerSchema.ExternalDNSAdapterDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid ExternalDNSAdapterGuid
		{
			get
			{
				return (Guid)this[TransportServerSchema.ExternalDNSAdapterGuid];
			}
			set
			{
				this[TransportServerSchema.ExternalDNSAdapterGuid] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolOption ExternalDNSProtocolOption
		{
			get
			{
				return (ProtocolOption)this[TransportServerSchema.ExternalDNSProtocolOption];
			}
			set
			{
				this[TransportServerSchema.ExternalDNSProtocolOption] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPAddress> ExternalDNSServers
		{
			get
			{
				return (MultiValuedProperty<IPAddress>)this[TransportServerSchema.ExternalDNSServers];
			}
			set
			{
				this[TransportServerSchema.ExternalDNSServers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IPAddress ExternalIPAddress
		{
			get
			{
				return (IPAddress)this[TransportServerSchema.ExternalIPAddress];
			}
			set
			{
				this[TransportServerSchema.ExternalIPAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InternalDNSAdapterEnabled
		{
			get
			{
				return !(bool)this[TransportServerSchema.InternalDNSAdapterDisabled];
			}
			set
			{
				this[TransportServerSchema.InternalDNSAdapterDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid InternalDNSAdapterGuid
		{
			get
			{
				return (Guid)this[TransportServerSchema.InternalDNSAdapterGuid];
			}
			set
			{
				this[TransportServerSchema.InternalDNSAdapterGuid] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolOption InternalDNSProtocolOption
		{
			get
			{
				return (ProtocolOption)this[TransportServerSchema.InternalDNSProtocolOption];
			}
			set
			{
				this[TransportServerSchema.InternalDNSProtocolOption] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPAddress> InternalDNSServers
		{
			get
			{
				return (MultiValuedProperty<IPAddress>)this[TransportServerSchema.InternalDNSServers];
			}
			set
			{
				this[TransportServerSchema.InternalDNSServers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxConcurrentMailboxDeliveries
		{
			get
			{
				return (int)this[TransportServerSchema.MaxConcurrentMailboxDeliveries];
			}
			set
			{
				this[TransportServerSchema.MaxConcurrentMailboxDeliveries] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxConcurrentMailboxSubmissions
		{
			get
			{
				return (int)this[ADTransportServerSchema.MaxConcurrentMailboxSubmissions];
			}
			set
			{
				this[ADTransportServerSchema.MaxConcurrentMailboxSubmissions] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxConnectionRatePerMinute
		{
			get
			{
				return (int)this[TransportServerSchema.MaxConnectionRatePerMinute];
			}
			set
			{
				this[TransportServerSchema.MaxConnectionRatePerMinute] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxOutboundConnections
		{
			get
			{
				return (Unlimited<int>)this[TransportServerSchema.MaxOutboundConnections];
			}
			set
			{
				this[TransportServerSchema.MaxOutboundConnections] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxPerDomainOutboundConnections
		{
			get
			{
				return (Unlimited<int>)this[TransportServerSchema.MaxPerDomainOutboundConnections];
			}
			set
			{
				this[TransportServerSchema.MaxPerDomainOutboundConnections] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MessageExpirationTimeout
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.MessageExpirationTimeout];
			}
			set
			{
				this[TransportServerSchema.MessageExpirationTimeout] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MessageRetryInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.MessageRetryInterval];
			}
			set
			{
				this[TransportServerSchema.MessageRetryInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MessageTrackingLogEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.MessageTrackingLogEnabled];
			}
			set
			{
				this[TransportServerSchema.MessageTrackingLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MessageTrackingLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.MessageTrackingLogMaxAge];
			}
			set
			{
				this[TransportServerSchema.MessageTrackingLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> MessageTrackingLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[TransportServerSchema.MessageTrackingLogMaxDirectorySize];
			}
			set
			{
				this[TransportServerSchema.MessageTrackingLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize MessageTrackingLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportServerSchema.MessageTrackingLogMaxFileSize];
			}
			set
			{
				this[TransportServerSchema.MessageTrackingLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath MessageTrackingLogPath
		{
			get
			{
				return (LocalLongFullPath)this[TransportServerSchema.MessageTrackingLogPath];
			}
			set
			{
				this[TransportServerSchema.MessageTrackingLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IrmLogEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.IrmLogEnabled];
			}
			set
			{
				this[TransportServerSchema.IrmLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan IrmLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.IrmLogMaxAge];
			}
			set
			{
				this[TransportServerSchema.IrmLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> IrmLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[TransportServerSchema.IrmLogMaxDirectorySize];
			}
			set
			{
				this[TransportServerSchema.IrmLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize IrmLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportServerSchema.IrmLogMaxFileSize];
			}
			set
			{
				this[TransportServerSchema.IrmLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath IrmLogPath
		{
			get
			{
				return (LocalLongFullPath)this[TransportServerSchema.IrmLogPath];
			}
			set
			{
				this[TransportServerSchema.IrmLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ActiveUserStatisticsLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.ActiveUserStatisticsLogMaxAge];
			}
			set
			{
				this[TransportServerSchema.ActiveUserStatisticsLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize ActiveUserStatisticsLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportServerSchema.ActiveUserStatisticsLogMaxDirectorySize];
			}
			set
			{
				this[TransportServerSchema.ActiveUserStatisticsLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize ActiveUserStatisticsLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportServerSchema.ActiveUserStatisticsLogMaxFileSize];
			}
			set
			{
				this[TransportServerSchema.ActiveUserStatisticsLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ActiveUserStatisticsLogPath
		{
			get
			{
				return (LocalLongFullPath)this[TransportServerSchema.ActiveUserStatisticsLogPath];
			}
			set
			{
				this[TransportServerSchema.ActiveUserStatisticsLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ServerStatisticsLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.ServerStatisticsLogMaxAge];
			}
			set
			{
				this[TransportServerSchema.ServerStatisticsLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize ServerStatisticsLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportServerSchema.ServerStatisticsLogMaxDirectorySize];
			}
			set
			{
				this[TransportServerSchema.ServerStatisticsLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize ServerStatisticsLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportServerSchema.ServerStatisticsLogMaxFileSize];
			}
			set
			{
				this[TransportServerSchema.ServerStatisticsLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ServerStatisticsLogPath
		{
			get
			{
				return (LocalLongFullPath)this[TransportServerSchema.ServerStatisticsLogPath];
			}
			set
			{
				this[TransportServerSchema.ServerStatisticsLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MessageTrackingLogSubjectLoggingEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.MessageTrackingLogSubjectLoggingEnabled];
			}
			set
			{
				this[TransportServerSchema.MessageTrackingLogSubjectLoggingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan OutboundConnectionFailureRetryInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.OutboundConnectionFailureRetryInterval];
			}
			set
			{
				this[TransportServerSchema.OutboundConnectionFailureRetryInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolLoggingLevel IntraOrgConnectorProtocolLoggingLevel
		{
			get
			{
				return (ProtocolLoggingLevel)this[TransportServerSchema.IntraOrgConnectorProtocolLoggingLevel];
			}
			set
			{
				this[TransportServerSchema.IntraOrgConnectorProtocolLoggingLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize PickupDirectoryMaxHeaderSize
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportServerSchema.PickupDirectoryMaxHeaderSize];
			}
			set
			{
				this[TransportServerSchema.PickupDirectoryMaxHeaderSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int PickupDirectoryMaxMessagesPerMinute
		{
			get
			{
				return (int)this[TransportServerSchema.PickupDirectoryMaxMessagesPerMinute];
			}
			set
			{
				this[TransportServerSchema.PickupDirectoryMaxMessagesPerMinute] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int PickupDirectoryMaxRecipientsPerMessage
		{
			get
			{
				return (int)this[TransportServerSchema.PickupDirectoryMaxRecipientsPerMessage];
			}
			set
			{
				this[TransportServerSchema.PickupDirectoryMaxRecipientsPerMessage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath PickupDirectoryPath
		{
			get
			{
				return (LocalLongFullPath)this[TransportServerSchema.PickupDirectoryPath];
			}
			set
			{
				this[TransportServerSchema.PickupDirectoryPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PipelineTracingEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.PipelineTracingEnabled];
			}
			set
			{
				this[TransportServerSchema.PipelineTracingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ContentConversionTracingEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.ContentConversionTracingEnabled];
			}
			set
			{
				this[TransportServerSchema.ContentConversionTracingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath PipelineTracingPath
		{
			get
			{
				return (LocalLongFullPath)this[TransportServerSchema.PipelineTracingPath];
			}
			set
			{
				this[TransportServerSchema.PipelineTracingPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress? PipelineTracingSenderAddress
		{
			get
			{
				return (SmtpAddress?)this[TransportServerSchema.PipelineTracingSenderAddress];
			}
			set
			{
				this[TransportServerSchema.PipelineTracingSenderAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PoisonMessageDetectionEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.PoisonMessageDetectionEnabled];
			}
			set
			{
				this[TransportServerSchema.PoisonMessageDetectionEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int PoisonThreshold
		{
			get
			{
				return (int)this[TransportServerSchema.PoisonThreshold];
			}
			set
			{
				this[TransportServerSchema.PoisonThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan QueueMaxIdleTime
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.QueueMaxIdleTime];
			}
			set
			{
				this[TransportServerSchema.QueueMaxIdleTime] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ReceiveProtocolLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.ReceiveProtocolLogMaxAge];
			}
			set
			{
				this[TransportServerSchema.ReceiveProtocolLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ReceiveProtocolLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[TransportServerSchema.ReceiveProtocolLogMaxDirectorySize];
			}
			set
			{
				this[TransportServerSchema.ReceiveProtocolLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ReceiveProtocolLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[TransportServerSchema.ReceiveProtocolLogMaxFileSize];
			}
			set
			{
				this[TransportServerSchema.ReceiveProtocolLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ReceiveProtocolLogPath
		{
			get
			{
				return (LocalLongFullPath)this[TransportServerSchema.ReceiveProtocolLogPath];
			}
			set
			{
				this[TransportServerSchema.ReceiveProtocolLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RecipientValidationCacheEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.RecipientValidationCacheEnabled];
			}
			set
			{
				this[TransportServerSchema.RecipientValidationCacheEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ReplayDirectoryPath
		{
			get
			{
				return (LocalLongFullPath)this[TransportServerSchema.ReplayDirectoryPath];
			}
			set
			{
				this[TransportServerSchema.ReplayDirectoryPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string RootDropDirectoryPath
		{
			get
			{
				return (string)this[TransportServerSchema.RootDropDirectoryPath];
			}
			set
			{
				this[TransportServerSchema.RootDropDirectoryPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan RoutingTableLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.RoutingTableLogMaxAge];
			}
			set
			{
				this[TransportServerSchema.RoutingTableLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> RoutingTableLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[TransportServerSchema.RoutingTableLogMaxDirectorySize];
			}
			set
			{
				this[TransportServerSchema.RoutingTableLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath RoutingTableLogPath
		{
			get
			{
				return (LocalLongFullPath)this[TransportServerSchema.RoutingTableLogPath];
			}
			set
			{
				this[TransportServerSchema.RoutingTableLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan SendProtocolLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.SendProtocolLogMaxAge];
			}
			set
			{
				this[TransportServerSchema.SendProtocolLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> SendProtocolLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[TransportServerSchema.SendProtocolLogMaxDirectorySize];
			}
			set
			{
				this[TransportServerSchema.SendProtocolLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> SendProtocolLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[TransportServerSchema.SendProtocolLogMaxFileSize];
			}
			set
			{
				this[TransportServerSchema.SendProtocolLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath SendProtocolLogPath
		{
			get
			{
				return (LocalLongFullPath)this[TransportServerSchema.SendProtocolLogPath];
			}
			set
			{
				this[TransportServerSchema.SendProtocolLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TransientFailureRetryCount
		{
			get
			{
				return (int)this[TransportServerSchema.TransientFailureRetryCount];
			}
			set
			{
				this[TransportServerSchema.TransientFailureRetryCount] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TransientFailureRetryInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.TransientFailureRetryInterval];
			}
			set
			{
				this[TransportServerSchema.TransientFailureRetryInterval] = value;
			}
		}

		public bool AntispamUpdatesEnabled
		{
			get
			{
				return (bool)this[ServerSchema.AntispamUpdatesEnabled];
			}
		}

		public string InternalTransportCertificateThumbprint
		{
			get
			{
				return (string)this[TransportServerSchema.InternalTransportCertificateThumbprint];
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.TransportSyncEnabled];
			}
			set
			{
				this[TransportServerSchema.TransportSyncEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncPopEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.TransportSyncPopEnabled];
			}
			set
			{
				this[TransportServerSchema.TransportSyncPopEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool WindowsLiveHotmailTransportSyncEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.WindowsLiveHotmailTransportSyncEnabled];
			}
			set
			{
				this[TransportServerSchema.WindowsLiveHotmailTransportSyncEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncExchangeEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.TransportSyncExchangeEnabled];
			}
			set
			{
				this[TransportServerSchema.TransportSyncExchangeEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncImapEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.TransportSyncImapEnabled];
			}
			set
			{
				this[TransportServerSchema.TransportSyncImapEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxNumberOfTransportSyncAttempts
		{
			get
			{
				return (int)this[TransportServerSchema.MaxNumberOfTransportSyncAttempts];
			}
			set
			{
				this[TransportServerSchema.MaxNumberOfTransportSyncAttempts] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxActiveTransportSyncJobsPerProcessor
		{
			get
			{
				return (int)this[TransportServerSchema.MaxActiveTransportSyncJobsPerProcessor];
			}
			set
			{
				this[TransportServerSchema.MaxActiveTransportSyncJobsPerProcessor] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string HttpTransportSyncProxyServer
		{
			get
			{
				return (string)this[TransportServerSchema.HttpTransportSyncProxyServer];
			}
			set
			{
				this[TransportServerSchema.HttpTransportSyncProxyServer] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool HttpProtocolLogEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.HttpProtocolLogEnabled];
			}
			set
			{
				this[TransportServerSchema.HttpProtocolLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath HttpProtocolLogFilePath
		{
			get
			{
				return (LocalLongFullPath)this[TransportServerSchema.HttpProtocolLogFilePath];
			}
			set
			{
				this[TransportServerSchema.HttpProtocolLogFilePath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan HttpProtocolLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.HttpProtocolLogMaxAge];
			}
			set
			{
				this[TransportServerSchema.HttpProtocolLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize HttpProtocolLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportServerSchema.HttpProtocolLogMaxDirectorySize];
			}
			set
			{
				this[TransportServerSchema.HttpProtocolLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize HttpProtocolLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportServerSchema.HttpProtocolLogMaxFileSize];
			}
			set
			{
				this[TransportServerSchema.HttpProtocolLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolLoggingLevel HttpProtocolLogLoggingLevel
		{
			get
			{
				return (ProtocolLoggingLevel)this[TransportServerSchema.HttpProtocolLogLoggingLevel];
			}
			set
			{
				this[TransportServerSchema.HttpProtocolLogLoggingLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncLogEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.TransportSyncLogEnabled];
			}
			set
			{
				this[TransportServerSchema.TransportSyncLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath TransportSyncLogFilePath
		{
			get
			{
				return (LocalLongFullPath)this[TransportServerSchema.TransportSyncLogFilePath];
			}
			set
			{
				this[TransportServerSchema.TransportSyncLogFilePath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SyncLoggingLevel TransportSyncLogLoggingLevel
		{
			get
			{
				return (SyncLoggingLevel)this[TransportServerSchema.TransportSyncLogLoggingLevel];
			}
			set
			{
				this[TransportServerSchema.TransportSyncLogLoggingLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TransportSyncLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.TransportSyncLogMaxAge];
			}
			set
			{
				this[TransportServerSchema.TransportSyncLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportServerSchema.TransportSyncLogMaxDirectorySize];
			}
			set
			{
				this[TransportServerSchema.TransportSyncLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportServerSchema.TransportSyncLogMaxFileSize];
			}
			set
			{
				this[TransportServerSchema.TransportSyncLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncHubHealthLogEnabled
		{
			get
			{
				return (bool)this[ADTransportServerSchema.TransportSyncHubHealthLogEnabled];
			}
			set
			{
				this[ADTransportServerSchema.TransportSyncHubHealthLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath TransportSyncHubHealthLogFilePath
		{
			get
			{
				return (LocalLongFullPath)this[ADTransportServerSchema.TransportSyncHubHealthLogFilePath];
			}
			set
			{
				this[ADTransportServerSchema.TransportSyncHubHealthLogFilePath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TransportSyncHubHealthLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ADTransportServerSchema.TransportSyncHubHealthLogMaxAge];
			}
			set
			{
				this[ADTransportServerSchema.TransportSyncHubHealthLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncHubHealthLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[ADTransportServerSchema.TransportSyncHubHealthLogMaxDirectorySize];
			}
			set
			{
				this[ADTransportServerSchema.TransportSyncHubHealthLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncHubHealthLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ADTransportServerSchema.TransportSyncHubHealthLogMaxFileSize];
			}
			set
			{
				this[ADTransportServerSchema.TransportSyncHubHealthLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncAccountsPoisonDetectionEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.TransportSyncAccountsPoisonDetectionEnabled];
			}
			set
			{
				this[TransportServerSchema.TransportSyncAccountsPoisonDetectionEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TransportSyncAccountsPoisonAccountThreshold
		{
			get
			{
				return (int)this[TransportServerSchema.TransportSyncAccountsPoisonAccountThreshold];
			}
			set
			{
				this[TransportServerSchema.TransportSyncAccountsPoisonAccountThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TransportSyncAccountsPoisonItemThreshold
		{
			get
			{
				return (int)this[TransportServerSchema.TransportSyncAccountsPoisonItemThreshold];
			}
			set
			{
				this[TransportServerSchema.TransportSyncAccountsPoisonItemThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TransportSyncAccountsSuccessivePoisonItemThreshold
		{
			get
			{
				return (int)this[ADTransportServerSchema.TransportSyncAccountsSuccessivePoisonItemThreshold];
			}
			set
			{
				this[ADTransportServerSchema.TransportSyncAccountsSuccessivePoisonItemThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TransportSyncRemoteConnectionTimeout
		{
			get
			{
				return (EnhancedTimeSpan)this[TransportServerSchema.TransportSyncRemoteConnectionTimeout];
			}
			set
			{
				this[TransportServerSchema.TransportSyncRemoteConnectionTimeout] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncMaxDownloadSizePerItem
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportServerSchema.TransportSyncMaxDownloadSizePerItem];
			}
			set
			{
				this[TransportServerSchema.TransportSyncMaxDownloadSizePerItem] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncMaxDownloadSizePerConnection
		{
			get
			{
				return (ByteQuantifiedSize)this[TransportServerSchema.TransportSyncMaxDownloadSizePerConnection];
			}
			set
			{
				this[TransportServerSchema.TransportSyncMaxDownloadSizePerConnection] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TransportSyncMaxDownloadItemsPerConnection
		{
			get
			{
				return (int)this[TransportServerSchema.TransportSyncMaxDownloadItemsPerConnection];
			}
			set
			{
				this[TransportServerSchema.TransportSyncMaxDownloadItemsPerConnection] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string DeltaSyncClientCertificateThumbprint
		{
			get
			{
				return (string)this[TransportServerSchema.DeltaSyncClientCertificateThumbprint];
			}
			set
			{
				this[TransportServerSchema.DeltaSyncClientCertificateThumbprint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UseDowngradedExchangeServerAuth
		{
			get
			{
				return (bool)this[ADTransportServerSchema.UseDowngradedExchangeServerAuth];
			}
			set
			{
				this[ADTransportServerSchema.UseDowngradedExchangeServerAuth] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int IntraOrgConnectorSmtpMaxMessagesPerConnection
		{
			get
			{
				return (int)this[TransportServerSchema.IntraOrgConnectorSmtpMaxMessagesPerConnection];
			}
			set
			{
				this[TransportServerSchema.IntraOrgConnectorSmtpMaxMessagesPerConnection] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncLinkedInEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.TransportSyncLinkedInEnabled];
			}
			set
			{
				this[TransportServerSchema.TransportSyncLinkedInEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncFacebookEnabled
		{
			get
			{
				return (bool)this[TransportServerSchema.TransportSyncFacebookEnabled];
			}
			set
			{
				this[TransportServerSchema.TransportSyncFacebookEnabled] = value;
			}
		}

		public EnhancedTimeSpan QueueLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.QueueLogMaxAge];
			}
		}

		public Unlimited<ByteQuantifiedSize> QueueLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.QueueLogMaxDirectorySize];
			}
		}

		public Unlimited<ByteQuantifiedSize> QueueLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.QueueLogMaxFileSize];
			}
		}

		public LocalLongFullPath QueueLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.QueueLogPath];
			}
		}

		public EnhancedTimeSpan WlmLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.WlmLogMaxAge];
			}
		}

		public Unlimited<ByteQuantifiedSize> WlmLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.WlmLogMaxDirectorySize];
			}
		}

		public Unlimited<ByteQuantifiedSize> WlmLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.WlmLogMaxFileSize];
			}
		}

		public LocalLongFullPath WlmLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.WlmLogPath];
			}
		}

		public EnhancedTimeSpan AgentLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.AgentLogMaxAge];
			}
		}

		public Unlimited<ByteQuantifiedSize> AgentLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.AgentLogMaxDirectorySize];
			}
		}

		public Unlimited<ByteQuantifiedSize> AgentLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.AgentLogMaxFileSize];
			}
		}

		public LocalLongFullPath AgentLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.AgentLogPath];
			}
		}

		public bool AgentLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.AgentLogEnabled];
			}
		}

		public EnhancedTimeSpan FlowControlLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.FlowControlLogMaxAge];
			}
		}

		public Unlimited<ByteQuantifiedSize> FlowControlLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.FlowControlLogMaxDirectorySize];
			}
		}

		public Unlimited<ByteQuantifiedSize> FlowControlLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.FlowControlLogMaxFileSize];
			}
		}

		public LocalLongFullPath FlowControlLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.FlowControlLogPath];
			}
		}

		public bool FlowControlLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.FlowControlLogEnabled];
			}
		}

		public EnhancedTimeSpan ProcessingSchedulerLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.ProcessingSchedulerLogMaxAge];
			}
		}

		public Unlimited<ByteQuantifiedSize> ProcessingSchedulerLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.ProcessingSchedulerLogMaxDirectorySize];
			}
		}

		public Unlimited<ByteQuantifiedSize> ProcessingSchedulerLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.ProcessingSchedulerLogMaxFileSize];
			}
		}

		public LocalLongFullPath ProcessingSchedulerLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.ProcessingSchedulerLogPath];
			}
		}

		public bool ProcessingSchedulerLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.ProcessingSchedulerLogEnabled];
			}
		}

		public EnhancedTimeSpan ResourceLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.ResourceLogMaxAge];
			}
		}

		public Unlimited<ByteQuantifiedSize> ResourceLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.ResourceLogMaxDirectorySize];
			}
		}

		public Unlimited<ByteQuantifiedSize> ResourceLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.ResourceLogMaxFileSize];
			}
		}

		public LocalLongFullPath ResourceLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.ResourceLogPath];
			}
		}

		public bool ResourceLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.ResourceLogEnabled];
			}
		}

		public EnhancedTimeSpan DnsLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.DnsLogMaxAge];
			}
		}

		public Unlimited<ByteQuantifiedSize> DnsLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.DnsLogMaxDirectorySize];
			}
		}

		public Unlimited<ByteQuantifiedSize> DnsLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.DnsLogMaxFileSize];
			}
		}

		public LocalLongFullPath DnsLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.DnsLogPath];
			}
		}

		public bool DnsLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.DnsLogEnabled];
			}
		}

		public EnhancedTimeSpan JournalLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.JournalLogMaxAge];
			}
		}

		public Unlimited<ByteQuantifiedSize> JournalLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.JournalLogMaxDirectorySize];
			}
		}

		public Unlimited<ByteQuantifiedSize> JournalLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.JournalLogMaxFileSize];
			}
		}

		public LocalLongFullPath JournalLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.JournalLogPath];
			}
		}

		public bool JournalLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.JournalLogEnabled];
			}
		}

		public EnhancedTimeSpan TransportMaintenanceLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.TransportMaintenanceLogMaxAge];
			}
		}

		public Unlimited<ByteQuantifiedSize> TransportMaintenanceLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.TransportMaintenanceLogMaxDirectorySize];
			}
		}

		public Unlimited<ByteQuantifiedSize> TransportMaintenanceLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ServerSchema.TransportMaintenanceLogMaxFileSize];
			}
		}

		public LocalLongFullPath TransportMaintenanceLogPath
		{
			get
			{
				return (LocalLongFullPath)this[ServerSchema.TransportMaintenanceLogPath];
			}
		}

		public bool TransportMaintenanceLogEnabled
		{
			get
			{
				return (bool)this[ServerSchema.TransportMaintenanceLogEnabled];
			}
		}

		private static TransportServerSchema schema = ObjectSchema.GetInstance<TransportServerSchema>();

		private static ADTransportServerSchema adSchema = ObjectSchema.GetInstance<ADTransportServerSchema>();
	}
}
