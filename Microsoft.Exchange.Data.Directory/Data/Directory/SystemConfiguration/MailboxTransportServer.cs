using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public class MailboxTransportServer : ADLegacyVersionableObject
	{
		public ServerVersion AdminDisplayVersion
		{
			get
			{
				return (ServerVersion)this[MailboxTransportServerSchema.AdminDisplayVersion];
			}
			internal set
			{
				this[MailboxTransportServerSchema.AdminDisplayVersion] = value;
			}
		}

		public ServerEditionType Edition
		{
			get
			{
				return (ServerEditionType)this[MailboxTransportServerSchema.Edition];
			}
			internal set
			{
				this[MailboxTransportServerSchema.Edition] = value;
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return (string)this[MailboxTransportServerSchema.ExchangeLegacyDN];
			}
			internal set
			{
				this[MailboxTransportServerSchema.ExchangeLegacyDN] = value;
			}
		}

		public bool IsMailboxServer
		{
			get
			{
				return (bool)this[MailboxTransportServerSchema.IsMailboxServer];
			}
			internal set
			{
				this[MailboxTransportServerSchema.IsMailboxServer] = value;
			}
		}

		public bool IsProvisionedServer
		{
			get
			{
				return (bool)this[MailboxTransportServerSchema.IsProvisionedServer];
			}
			internal set
			{
				this[MailboxTransportServerSchema.IsProvisionedServer] = value;
			}
		}

		public NetworkAddressCollection NetworkAddress
		{
			get
			{
				return (NetworkAddressCollection)this[MailboxTransportServerSchema.NetworkAddress];
			}
			internal set
			{
				this[MailboxTransportServerSchema.NetworkAddress] = value;
			}
		}

		public int VersionNumber
		{
			get
			{
				return (int)this[MailboxTransportServerSchema.VersionNumber];
			}
			internal set
			{
				this[MailboxTransportServerSchema.VersionNumber] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ConnectivityLogEnabled
		{
			get
			{
				return (bool)this[MailboxTransportServerSchema.ConnectivityLogEnabled];
			}
			set
			{
				this[MailboxTransportServerSchema.ConnectivityLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ConnectivityLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxTransportServerSchema.ConnectivityLogMaxAge];
			}
			set
			{
				this[MailboxTransportServerSchema.ConnectivityLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ConnectivityLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.ConnectivityLogMaxDirectorySize];
			}
			set
			{
				this[MailboxTransportServerSchema.ConnectivityLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ConnectivityLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.ConnectivityLogMaxFileSize];
			}
			set
			{
				this[MailboxTransportServerSchema.ConnectivityLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ConnectivityLogPath
		{
			get
			{
				return (LocalLongFullPath)this[MailboxTransportServerSchema.ConnectivityLogPath];
			}
			set
			{
				this[MailboxTransportServerSchema.ConnectivityLogPath] = value;
			}
		}

		public bool ContentConversionTracingEnabled
		{
			get
			{
				return (bool)this[MailboxTransportServerSchema.ContentConversionTracingEnabled];
			}
			internal set
			{
				this[MailboxTransportServerSchema.ContentConversionTracingEnabled] = value;
			}
		}

		public ServerRole CurrentServerRole
		{
			get
			{
				return (ServerRole)this[MailboxTransportServerSchema.CurrentServerRole];
			}
			internal set
			{
				this[MailboxTransportServerSchema.CurrentServerRole] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolLoggingLevel InMemoryReceiveConnectorProtocolLoggingLevel
		{
			get
			{
				return (ProtocolLoggingLevel)this[MailboxTransportServerSchema.InMemoryReceiveConnectorProtocolLoggingLevel];
			}
			set
			{
				this[MailboxTransportServerSchema.InMemoryReceiveConnectorProtocolLoggingLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InMemoryReceiveConnectorSmtpUtf8Enabled
		{
			get
			{
				return (bool)this[MailboxTransportServerSchema.InMemoryReceiveConnectorSmtpUtf8Enabled];
			}
			set
			{
				this[MailboxTransportServerSchema.InMemoryReceiveConnectorSmtpUtf8Enabled] = value;
			}
		}

		public EnhancedTimeSpan MailboxDeliveryAgentLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxTransportServerSchema.MailboxDeliveryAgentLogMaxAge];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxDeliveryAgentLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> MailboxDeliveryAgentLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.MailboxDeliveryAgentLogMaxDirectorySize];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxDeliveryAgentLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> MailboxDeliveryAgentLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.MailboxDeliveryAgentLogMaxFileSize];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxDeliveryAgentLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath MailboxDeliveryAgentLogPath
		{
			get
			{
				return (LocalLongFullPath)this[MailboxTransportServerSchema.MailboxDeliveryAgentLogPath];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxDeliveryAgentLogPath] = value;
			}
		}

		public bool MailboxDeliveryAgentLogEnabled
		{
			get
			{
				return (bool)this[MailboxTransportServerSchema.MailboxDeliveryAgentLogEnabled];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxDeliveryAgentLogEnabled] = value;
			}
		}

		public EnhancedTimeSpan MailboxSubmissionAgentLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxTransportServerSchema.MailboxSubmissionAgentLogMaxAge];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxSubmissionAgentLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> MailboxSubmissionAgentLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.MailboxSubmissionAgentLogMaxDirectorySize];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxSubmissionAgentLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> MailboxSubmissionAgentLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.MailboxSubmissionAgentLogMaxFileSize];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxSubmissionAgentLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath MailboxSubmissionAgentLogPath
		{
			get
			{
				return (LocalLongFullPath)this[MailboxTransportServerSchema.MailboxSubmissionAgentLogPath];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxSubmissionAgentLogPath] = value;
			}
		}

		public bool MailboxSubmissionAgentLogEnabled
		{
			get
			{
				return (bool)this[MailboxTransportServerSchema.MailboxSubmissionAgentLogEnabled];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxSubmissionAgentLogEnabled] = value;
			}
		}

		public bool MailboxDeliveryThrottlingLogEnabled
		{
			get
			{
				return (bool)this[MailboxTransportServerSchema.MailboxDeliveryThrottlingLogEnabled];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxDeliveryThrottlingLogEnabled] = value;
			}
		}

		public EnhancedTimeSpan MailboxDeliveryThrottlingLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxTransportServerSchema.MailboxDeliveryThrottlingLogMaxAge];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxDeliveryThrottlingLogMaxAge] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> MailboxDeliveryThrottlingLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.MailboxDeliveryThrottlingLogMaxDirectorySize];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxDeliveryThrottlingLogMaxDirectorySize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> MailboxDeliveryThrottlingLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.MailboxDeliveryThrottlingLogMaxFileSize];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxDeliveryThrottlingLogMaxFileSize] = value;
			}
		}

		public LocalLongFullPath MailboxDeliveryThrottlingLogPath
		{
			get
			{
				return (LocalLongFullPath)this[MailboxTransportServerSchema.MailboxDeliveryThrottlingLogPath];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxDeliveryThrottlingLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxConcurrentMailboxDeliveries
		{
			get
			{
				return (int)this[MailboxTransportServerSchema.MaxConcurrentMailboxDeliveries];
			}
			set
			{
				this[MailboxTransportServerSchema.MaxConcurrentMailboxDeliveries] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxConcurrentMailboxSubmissions
		{
			get
			{
				return (int)this[MailboxTransportServerSchema.MaxConcurrentMailboxSubmissions];
			}
			set
			{
				this[MailboxTransportServerSchema.MaxConcurrentMailboxSubmissions] = value;
			}
		}

		public bool PipelineTracingEnabled
		{
			get
			{
				return (bool)this[MailboxTransportServerSchema.PipelineTracingEnabled];
			}
			internal set
			{
				this[MailboxTransportServerSchema.PipelineTracingEnabled] = value;
			}
		}

		public LocalLongFullPath PipelineTracingPath
		{
			get
			{
				return (LocalLongFullPath)this[MailboxTransportServerSchema.PipelineTracingPath];
			}
			internal set
			{
				this[MailboxTransportServerSchema.PipelineTracingPath] = value;
			}
		}

		public SmtpAddress? PipelineTracingSenderAddress
		{
			get
			{
				return (SmtpAddress?)this[MailboxTransportServerSchema.PipelineTracingSenderAddress];
			}
			internal set
			{
				this[MailboxTransportServerSchema.PipelineTracingSenderAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ReceiveProtocolLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxTransportServerSchema.ReceiveProtocolLogMaxAge];
			}
			set
			{
				this[MailboxTransportServerSchema.ReceiveProtocolLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ReceiveProtocolLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.ReceiveProtocolLogMaxDirectorySize];
			}
			set
			{
				this[MailboxTransportServerSchema.ReceiveProtocolLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ReceiveProtocolLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.ReceiveProtocolLogMaxFileSize];
			}
			set
			{
				this[MailboxTransportServerSchema.ReceiveProtocolLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ReceiveProtocolLogPath
		{
			get
			{
				return (LocalLongFullPath)this[MailboxTransportServerSchema.ReceiveProtocolLogPath];
			}
			set
			{
				this[MailboxTransportServerSchema.ReceiveProtocolLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan SendProtocolLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxTransportServerSchema.SendProtocolLogMaxAge];
			}
			set
			{
				this[MailboxTransportServerSchema.SendProtocolLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> SendProtocolLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.SendProtocolLogMaxDirectorySize];
			}
			set
			{
				this[MailboxTransportServerSchema.SendProtocolLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> SendProtocolLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.SendProtocolLogMaxFileSize];
			}
			set
			{
				this[MailboxTransportServerSchema.SendProtocolLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath SendProtocolLogPath
		{
			get
			{
				return (LocalLongFullPath)this[MailboxTransportServerSchema.SendProtocolLogPath];
			}
			set
			{
				this[MailboxTransportServerSchema.SendProtocolLogPath] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				if (this.schema == null)
				{
					this.schema = ObjectSchema.GetInstance<MailboxTransportServerADSchema>();
				}
				return this.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MailboxTransportServer.mostDerivedClass;
			}
		}

		internal Container GetParentContainer()
		{
			return base.Session.Read<Container>(base.Id.Parent);
		}

		internal const string MailboxTransportServerADObjectName = "Mailbox";

		private static string mostDerivedClass = "msExchExchangeTransportServer";

		[NonSerialized]
		private ADObjectSchema schema;
	}
}
