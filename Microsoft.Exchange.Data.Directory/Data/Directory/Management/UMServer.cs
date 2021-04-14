using System;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class UMServer : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return UMServer.schema;
			}
		}

		public UMServer()
		{
		}

		public UMServer(Server dataObject) : base(dataObject)
		{
		}

		public new string Name
		{
			get
			{
				return (string)this[ADObjectSchema.Name];
			}
		}

		[Parameter(Mandatory = false)]
		public int? MaxCallsAllowed
		{
			get
			{
				return (int?)this[UMServerSchema.MaxCallsAllowed];
			}
			set
			{
				this[UMServerSchema.MaxCallsAllowed] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ServerStatus Status
		{
			get
			{
				return (ServerStatus)this[UMServerSchema.Status];
			}
			set
			{
				this[UMServerSchema.Status] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SipTcpListeningPort
		{
			get
			{
				return (int)this[UMServerSchema.SipTcpListeningPort];
			}
			set
			{
				this[UMServerSchema.SipTcpListeningPort] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SipTlsListeningPort
		{
			get
			{
				return (int)this[UMServerSchema.SipTlsListeningPort];
			}
			set
			{
				this[UMServerSchema.SipTlsListeningPort] = value;
			}
		}

		public MultiValuedProperty<UMLanguage> Languages
		{
			get
			{
				return (MultiValuedProperty<UMLanguage>)this[UMServerSchema.Languages];
			}
			internal set
			{
				this[UMServerSchema.Languages] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> DialPlans
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[UMServerSchema.DialPlans];
			}
			set
			{
				this[UMServerSchema.DialPlans] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ScheduleInterval[] GrammarGenerationSchedule
		{
			get
			{
				return (ScheduleInterval[])this[UMServerSchema.GrammarGenerationSchedule];
			}
			set
			{
				this[UMServerSchema.GrammarGenerationSchedule] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMSmartHost ExternalHostFqdn
		{
			get
			{
				return (UMSmartHost)this[UMServerSchema.ExternalHostFqdn];
			}
			set
			{
				this[UMServerSchema.ExternalHostFqdn] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMSmartHost ExternalServiceFqdn
		{
			get
			{
				return (UMSmartHost)this[UMServerSchema.ExternalServiceFqdn];
			}
			set
			{
				this[UMServerSchema.ExternalServiceFqdn] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string UMPodRedirectTemplate
		{
			get
			{
				return (string)this[UMServerSchema.UMPodRedirectTemplate];
			}
			set
			{
				this[UMServerSchema.UMPodRedirectTemplate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string UMForwardingAddressTemplate
		{
			get
			{
				return (string)this[UMServerSchema.UMForwardingAddressTemplate];
			}
			set
			{
				this[UMServerSchema.UMForwardingAddressTemplate] = value;
			}
		}

		public string UMCertificateThumbprint
		{
			get
			{
				return (string)this[UMServerSchema.UMCertificateThumbprint];
			}
			internal set
			{
				this[UMServerSchema.UMCertificateThumbprint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolConnectionSettings SIPAccessService
		{
			get
			{
				return (ProtocolConnectionSettings)this[UMServerSchema.SIPAccessService];
			}
			set
			{
				this[UMServerSchema.SIPAccessService] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMStartupMode UMStartupMode
		{
			get
			{
				return (UMStartupMode)this[UMServerSchema.UMStartupMode];
			}
			set
			{
				this[UMServerSchema.UMStartupMode] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IrmLogEnabled
		{
			get
			{
				return (bool)this[UMServerSchema.IrmLogEnabled];
			}
			set
			{
				this[UMServerSchema.IrmLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan IrmLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[UMServerSchema.IrmLogMaxAge];
			}
			set
			{
				this[UMServerSchema.IrmLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> IrmLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[UMServerSchema.IrmLogMaxDirectorySize];
			}
			set
			{
				this[UMServerSchema.IrmLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize IrmLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[UMServerSchema.IrmLogMaxFileSize];
			}
			set
			{
				this[UMServerSchema.IrmLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath IrmLogPath
		{
			get
			{
				return (LocalLongFullPath)this[UMServerSchema.IrmLogPath];
			}
			set
			{
				this[UMServerSchema.IrmLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IPAddressFamilyConfigurable
		{
			get
			{
				return (bool)this[UMServerSchema.IPAddressFamilyConfigurable];
			}
			set
			{
				this[UMServerSchema.IPAddressFamilyConfigurable] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IPAddressFamily IPAddressFamily
		{
			get
			{
				return (IPAddressFamily)this[UMServerSchema.IPAddressFamily];
			}
			set
			{
				this[UMServerSchema.IPAddressFamily] = value;
			}
		}

		private static UMServerSchema schema = ObjectSchema.GetInstance<UMServerSchema>();
	}
}
