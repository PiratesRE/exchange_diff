using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public sealed class MailboxTransportServerPresentationObject : ADPresentationObject
	{
		public MailboxTransportServerPresentationObject()
		{
		}

		public MailboxTransportServerPresentationObject(MailboxTransportServer dataObject) : base(dataObject)
		{
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return MailboxTransportServerPresentationObject.schema;
			}
		}

		public new string Name
		{
			get
			{
				return ((ADObjectId)this[ADObjectSchema.Id]).Parent.Parent.Name;
			}
		}

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
		public ProtocolLoggingLevel MailboxDeliveryConnectorProtocolLoggingLevel
		{
			get
			{
				return (ProtocolLoggingLevel)this[MailboxTransportServerSchema.MailboxDeliveryConnectorProtocolLoggingLevel];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxDeliveryConnectorProtocolLoggingLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MailboxDeliveryConnectorSmtpUtf8Enabled
		{
			get
			{
				return (bool)this[MailboxTransportServerSchema.MailboxDeliveryConnectorSmtpUtf8Enabled];
			}
			set
			{
				this[MailboxTransportServerSchema.MailboxDeliveryConnectorSmtpUtf8Enabled] = value;
			}
		}

		public EnhancedTimeSpan MailboxSubmissionAgentLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxTransportServerSchema.MailboxSubmissionAgentLogMaxAge];
			}
		}

		public Unlimited<ByteQuantifiedSize> MailboxSubmissionAgentLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.MailboxSubmissionAgentLogMaxDirectorySize];
			}
		}

		public Unlimited<ByteQuantifiedSize> MailboxSubmissionAgentLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.MailboxSubmissionAgentLogMaxFileSize];
			}
		}

		public LocalLongFullPath MailboxSubmissionAgentLogPath
		{
			get
			{
				return (LocalLongFullPath)this[MailboxTransportServerSchema.MailboxSubmissionAgentLogPath];
			}
		}

		public bool MailboxSubmissionAgentLogEnabled
		{
			get
			{
				return (bool)this[MailboxTransportServerSchema.MailboxSubmissionAgentLogEnabled];
			}
		}

		public EnhancedTimeSpan MailboxDeliveryAgentLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxTransportServerSchema.MailboxDeliveryAgentLogMaxAge];
			}
		}

		public Unlimited<ByteQuantifiedSize> MailboxDeliveryAgentLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.MailboxDeliveryAgentLogMaxDirectorySize];
			}
		}

		public Unlimited<ByteQuantifiedSize> MailboxDeliveryAgentLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.MailboxDeliveryAgentLogMaxFileSize];
			}
		}

		public LocalLongFullPath MailboxDeliveryAgentLogPath
		{
			get
			{
				return (LocalLongFullPath)this[MailboxTransportServerSchema.MailboxDeliveryAgentLogPath];
			}
		}

		public bool MailboxDeliveryAgentLogEnabled
		{
			get
			{
				return (bool)this[MailboxTransportServerSchema.MailboxDeliveryAgentLogEnabled];
			}
		}

		public bool MailboxDeliveryThrottlingLogEnabled
		{
			get
			{
				return (bool)this[MailboxTransportServerSchema.MailboxDeliveryThrottlingLogEnabled];
			}
		}

		public EnhancedTimeSpan MailboxDeliveryThrottlingLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxTransportServerSchema.MailboxDeliveryThrottlingLogMaxAge];
			}
		}

		public Unlimited<ByteQuantifiedSize> MailboxDeliveryThrottlingLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.MailboxDeliveryThrottlingLogMaxDirectorySize];
			}
		}

		public Unlimited<ByteQuantifiedSize> MailboxDeliveryThrottlingLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxTransportServerSchema.MailboxDeliveryThrottlingLogMaxFileSize];
			}
		}

		public LocalLongFullPath MailboxDeliveryThrottlingLogPath
		{
			get
			{
				return (LocalLongFullPath)this[MailboxTransportServerSchema.MailboxDeliveryThrottlingLogPath];
			}
		}

		public ServerRole ServerRole
		{
			get
			{
				return (ServerRole)this[MailboxTransportServerSchema.CurrentServerRole];
			}
		}

		private static MailboxTransportServerSchema schema = ObjectSchema.GetInstance<MailboxTransportServerSchema>();
	}
}
