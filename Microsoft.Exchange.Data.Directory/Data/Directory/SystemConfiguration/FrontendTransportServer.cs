using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public class FrontendTransportServer : ADLegacyVersionableObject
	{
		public ServerVersion AdminDisplayVersion
		{
			get
			{
				return (ServerVersion)this[FrontendTransportServerSchema.AdminDisplayVersion];
			}
			internal set
			{
				this[FrontendTransportServerSchema.AdminDisplayVersion] = value;
			}
		}

		public ServerEditionType Edition
		{
			get
			{
				return (ServerEditionType)this[FrontendTransportServerSchema.Edition];
			}
			internal set
			{
				this[FrontendTransportServerSchema.Edition] = value;
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return (string)this[FrontendTransportServerSchema.ExchangeLegacyDN];
			}
			internal set
			{
				this[FrontendTransportServerSchema.ExchangeLegacyDN] = value;
			}
		}

		public bool IsFrontendTransportServer
		{
			get
			{
				return (bool)this[FrontendTransportServerSchema.IsFrontendTransportServer];
			}
			internal set
			{
				this[FrontendTransportServerSchema.IsFrontendTransportServer] = value;
			}
		}

		public bool IsProvisionedServer
		{
			get
			{
				return (bool)this[FrontendTransportServerSchema.IsProvisionedServer];
			}
			internal set
			{
				this[FrontendTransportServerSchema.IsProvisionedServer] = value;
			}
		}

		public NetworkAddressCollection NetworkAddress
		{
			get
			{
				return (NetworkAddressCollection)this[FrontendTransportServerSchema.NetworkAddress];
			}
			internal set
			{
				this[FrontendTransportServerSchema.NetworkAddress] = value;
			}
		}

		public int VersionNumber
		{
			get
			{
				return (int)this[FrontendTransportServerSchema.VersionNumber];
			}
			internal set
			{
				this[FrontendTransportServerSchema.VersionNumber] = value;
			}
		}

		public EnhancedTimeSpan AgentLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[FrontendTransportServerSchema.AgentLogMaxAge];
			}
			set
			{
				this[FrontendTransportServerSchema.AgentLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> AgentLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[FrontendTransportServerSchema.AgentLogMaxDirectorySize];
			}
			set
			{
				this[FrontendTransportServerSchema.AgentLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> AgentLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[FrontendTransportServerSchema.AgentLogMaxFileSize];
			}
			set
			{
				this[FrontendTransportServerSchema.AgentLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath AgentLogPath
		{
			get
			{
				return (LocalLongFullPath)this[FrontendTransportServerSchema.AgentLogPath];
			}
			set
			{
				this[FrontendTransportServerSchema.AgentLogPath] = value;
			}
		}

		public bool AgentLogEnabled
		{
			get
			{
				return (bool)this[FrontendTransportServerSchema.AgentLogEnabled];
			}
			set
			{
				this[FrontendTransportServerSchema.AgentLogEnabled] = value;
			}
		}

		public EnhancedTimeSpan DnsLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[FrontendTransportServerSchema.DnsLogMaxAge];
			}
			set
			{
				this[FrontendTransportServerSchema.DnsLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> DnsLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[FrontendTransportServerSchema.DnsLogMaxDirectorySize];
			}
			set
			{
				this[FrontendTransportServerSchema.DnsLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> DnsLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[FrontendTransportServerSchema.DnsLogMaxFileSize];
			}
			set
			{
				this[FrontendTransportServerSchema.DnsLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath DnsLogPath
		{
			get
			{
				return (LocalLongFullPath)this[FrontendTransportServerSchema.DnsLogPath];
			}
			set
			{
				this[FrontendTransportServerSchema.DnsLogPath] = value;
			}
		}

		public bool DnsLogEnabled
		{
			get
			{
				return (bool)this[FrontendTransportServerSchema.DnsLogEnabled];
			}
			set
			{
				this[FrontendTransportServerSchema.DnsLogEnabled] = value;
			}
		}

		public EnhancedTimeSpan ResourceLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[FrontendTransportServerSchema.ResourceLogMaxAge];
			}
			set
			{
				this[FrontendTransportServerSchema.ResourceLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ResourceLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[FrontendTransportServerSchema.ResourceLogMaxDirectorySize];
			}
			set
			{
				this[FrontendTransportServerSchema.ResourceLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> ResourceLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[FrontendTransportServerSchema.ResourceLogMaxFileSize];
			}
			set
			{
				this[FrontendTransportServerSchema.ResourceLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath ResourceLogPath
		{
			get
			{
				return (LocalLongFullPath)this[FrontendTransportServerSchema.ResourceLogPath];
			}
			set
			{
				this[FrontendTransportServerSchema.ResourceLogPath] = value;
			}
		}

		public bool ResourceLogEnabled
		{
			get
			{
				return (bool)this[FrontendTransportServerSchema.ResourceLogEnabled];
			}
			set
			{
				this[FrontendTransportServerSchema.ResourceLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AntispamAgentsEnabled
		{
			get
			{
				return (bool)this[FrontendTransportServerSchema.AntispamAgentsEnabled];
			}
			set
			{
				this[FrontendTransportServerSchema.AntispamAgentsEnabled] = value;
			}
		}

		public EnhancedTimeSpan AttributionLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[FrontendTransportServerSchema.AttributionLogMaxAge];
			}
			set
			{
				this[FrontendTransportServerSchema.AttributionLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> AttributionLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[FrontendTransportServerSchema.AttributionLogMaxDirectorySize];
			}
			set
			{
				this[FrontendTransportServerSchema.AttributionLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> AttributionLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[FrontendTransportServerSchema.AttributionLogMaxFileSize];
			}
			set
			{
				this[FrontendTransportServerSchema.AttributionLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath AttributionLogPath
		{
			get
			{
				return (LocalLongFullPath)this[FrontendTransportServerSchema.AttributionLogPath];
			}
			set
			{
				this[FrontendTransportServerSchema.AttributionLogPath] = value;
			}
		}

		public bool AttributionLogEnabled
		{
			get
			{
				return (bool)this[FrontendTransportServerSchema.AttributionLogEnabled];
			}
			set
			{
				this[FrontendTransportServerSchema.AttributionLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ConnectivityLogEnabled
		{
			get
			{
				return (bool)this[FrontendTransportServerSchema.ConnectivityLogEnabled];
			}
			set
			{
				this[FrontendTransportServerSchema.ConnectivityLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ConnectivityLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[FrontendTransportServerSchema.ConnectivityLogMaxAge];
			}
			set
			{
				this[FrontendTransportServerSchema.ConnectivityLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ConnectivityLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[FrontendTransportServerSchema.ConnectivityLogMaxDirectorySize];
			}
			set
			{
				this[FrontendTransportServerSchema.ConnectivityLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ConnectivityLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[FrontendTransportServerSchema.ConnectivityLogMaxFileSize];
			}
			set
			{
				this[FrontendTransportServerSchema.ConnectivityLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ConnectivityLogPath
		{
			get
			{
				return (LocalLongFullPath)this[FrontendTransportServerSchema.ConnectivityLogPath];
			}
			set
			{
				this[FrontendTransportServerSchema.ConnectivityLogPath] = value;
			}
		}

		public int MaxReceiveTlsRatePerMinute
		{
			get
			{
				return (int)this[FrontendTransportServerSchema.MaxReceiveTlsRatePerMinute];
			}
			set
			{
				this[FrontendTransportServerSchema.MaxReceiveTlsRatePerMinute] = value;
			}
		}

		public ServerRole CurrentServerRole
		{
			get
			{
				return (ServerRole)this[FrontendTransportServerSchema.CurrentServerRole];
			}
			internal set
			{
				this[FrontendTransportServerSchema.CurrentServerRole] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExternalDNSAdapterEnabled
		{
			get
			{
				return !(bool)this[FrontendTransportServerSchema.ExternalDNSAdapterDisabled];
			}
			set
			{
				this[FrontendTransportServerSchema.ExternalDNSAdapterDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid ExternalDNSAdapterGuid
		{
			get
			{
				return (Guid)this[FrontendTransportServerSchema.ExternalDNSAdapterGuid];
			}
			set
			{
				this[FrontendTransportServerSchema.ExternalDNSAdapterGuid] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPAddress> ExternalDNSServers
		{
			get
			{
				return (MultiValuedProperty<IPAddress>)this[FrontendTransportServerSchema.ExternalDNSServers];
			}
			set
			{
				this[FrontendTransportServerSchema.ExternalDNSServers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IPAddress ExternalIPAddress
		{
			get
			{
				return (IPAddress)this[FrontendTransportServerSchema.ExternalIPAddress];
			}
			set
			{
				this[FrontendTransportServerSchema.ExternalIPAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolOption ExternalDNSProtocolOption
		{
			get
			{
				return (ProtocolOption)this[FrontendTransportServerSchema.ExternalDNSProtocolOption];
			}
			set
			{
				this[FrontendTransportServerSchema.ExternalDNSProtocolOption] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InternalDNSAdapterEnabled
		{
			get
			{
				return !(bool)this[FrontendTransportServerSchema.InternalDNSAdapterDisabled];
			}
			set
			{
				this[FrontendTransportServerSchema.InternalDNSAdapterDisabled] = !value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid InternalDNSAdapterGuid
		{
			get
			{
				return (Guid)this[FrontendTransportServerSchema.InternalDNSAdapterGuid];
			}
			set
			{
				this[FrontendTransportServerSchema.InternalDNSAdapterGuid] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPAddress> InternalDNSServers
		{
			get
			{
				return (MultiValuedProperty<IPAddress>)this[FrontendTransportServerSchema.InternalDNSServers];
			}
			set
			{
				this[FrontendTransportServerSchema.InternalDNSServers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolOption InternalDNSProtocolOption
		{
			get
			{
				return (ProtocolOption)this[FrontendTransportServerSchema.InternalDNSProtocolOption];
			}
			set
			{
				this[FrontendTransportServerSchema.InternalDNSProtocolOption] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolLoggingLevel IntraOrgConnectorProtocolLoggingLevel
		{
			get
			{
				return (ProtocolLoggingLevel)this[FrontendTransportServerSchema.IntraOrgConnectorProtocolLoggingLevel];
			}
			set
			{
				this[FrontendTransportServerSchema.IntraOrgConnectorProtocolLoggingLevel] = value;
			}
		}

		public int IntraOrgConnectorSmtpMaxMessagesPerConnection
		{
			get
			{
				return (int)this[FrontendTransportServerSchema.IntraOrgConnectorSmtpMaxMessagesPerConnection];
			}
			internal set
			{
				this[FrontendTransportServerSchema.IntraOrgConnectorSmtpMaxMessagesPerConnection] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxConnectionRatePerMinute
		{
			get
			{
				return (int)this[FrontendTransportServerSchema.MaxConnectionRatePerMinute];
			}
			set
			{
				this[FrontendTransportServerSchema.MaxConnectionRatePerMinute] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxOutboundConnections
		{
			get
			{
				return (Unlimited<int>)this[FrontendTransportServerSchema.MaxOutboundConnections];
			}
			set
			{
				this[FrontendTransportServerSchema.MaxOutboundConnections] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxPerDomainOutboundConnections
		{
			get
			{
				return (Unlimited<int>)this[FrontendTransportServerSchema.MaxPerDomainOutboundConnections];
			}
			set
			{
				this[FrontendTransportServerSchema.MaxPerDomainOutboundConnections] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ReceiveProtocolLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[FrontendTransportServerSchema.ReceiveProtocolLogMaxAge];
			}
			set
			{
				this[FrontendTransportServerSchema.ReceiveProtocolLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ReceiveProtocolLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[FrontendTransportServerSchema.ReceiveProtocolLogMaxDirectorySize];
			}
			set
			{
				this[FrontendTransportServerSchema.ReceiveProtocolLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ReceiveProtocolLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[FrontendTransportServerSchema.ReceiveProtocolLogMaxFileSize];
			}
			set
			{
				this[FrontendTransportServerSchema.ReceiveProtocolLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ReceiveProtocolLogPath
		{
			get
			{
				return (LocalLongFullPath)this[FrontendTransportServerSchema.ReceiveProtocolLogPath];
			}
			set
			{
				this[FrontendTransportServerSchema.ReceiveProtocolLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan SendProtocolLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[FrontendTransportServerSchema.SendProtocolLogMaxAge];
			}
			set
			{
				this[FrontendTransportServerSchema.SendProtocolLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> SendProtocolLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[FrontendTransportServerSchema.SendProtocolLogMaxDirectorySize];
			}
			set
			{
				this[FrontendTransportServerSchema.SendProtocolLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> SendProtocolLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[FrontendTransportServerSchema.SendProtocolLogMaxFileSize];
			}
			set
			{
				this[FrontendTransportServerSchema.SendProtocolLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath SendProtocolLogPath
		{
			get
			{
				return (LocalLongFullPath)this[FrontendTransportServerSchema.SendProtocolLogPath];
			}
			set
			{
				this[FrontendTransportServerSchema.SendProtocolLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int TransientFailureRetryCount
		{
			get
			{
				return (int)this[FrontendTransportServerSchema.TransientFailureRetryCount];
			}
			set
			{
				this[FrontendTransportServerSchema.TransientFailureRetryCount] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TransientFailureRetryInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[FrontendTransportServerSchema.TransientFailureRetryInterval];
			}
			set
			{
				this[FrontendTransportServerSchema.TransientFailureRetryInterval] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				if (this.schema == null)
				{
					this.schema = ObjectSchema.GetInstance<FrontendTransportServerADSchema>();
				}
				return this.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return FrontendTransportServer.mostDerivedClass;
			}
		}

		internal Container GetParentContainer()
		{
			return base.Session.Read<Container>(base.Id.Parent);
		}

		internal const string FrontendTransportServerADObjectName = "Frontend";

		private static string mostDerivedClass = "msExchExchangeTransportServer";

		[NonSerialized]
		private ADObjectSchema schema;
	}
}
