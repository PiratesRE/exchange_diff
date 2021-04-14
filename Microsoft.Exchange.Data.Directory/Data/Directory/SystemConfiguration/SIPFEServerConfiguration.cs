using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public sealed class SIPFEServerConfiguration : ADConfigurationObject
	{
		public new string Name
		{
			get
			{
				return "UMCallRouterSettings";
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return SIPFEServerConfiguration.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchProtocolCfgSIPFEServer";
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return SIPFEServerConfiguration.parentPath;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MaxCallsAllowed
		{
			get
			{
				return (int?)this[SIPFEServerConfigurationSchema.MaxCallsAllowed];
			}
			set
			{
				this[SIPFEServerConfigurationSchema.MaxCallsAllowed] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SipTcpListeningPort
		{
			get
			{
				return (int)this[SIPFEServerConfigurationSchema.SipTcpListeningPort];
			}
			set
			{
				this[SIPFEServerConfigurationSchema.SipTcpListeningPort] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SipTlsListeningPort
		{
			get
			{
				return (int)this[SIPFEServerConfigurationSchema.SipTlsListeningPort];
			}
			set
			{
				this[SIPFEServerConfigurationSchema.SipTlsListeningPort] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMSmartHost ExternalHostFqdn
		{
			get
			{
				return (UMSmartHost)this[SIPFEServerConfigurationSchema.ExternalHostFqdn];
			}
			set
			{
				this[SIPFEServerConfigurationSchema.ExternalHostFqdn] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMSmartHost ExternalServiceFqdn
		{
			get
			{
				return (UMSmartHost)this[SIPFEServerConfigurationSchema.ExternalServiceFqdn];
			}
			set
			{
				this[SIPFEServerConfigurationSchema.ExternalServiceFqdn] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string UMPodRedirectTemplate
		{
			get
			{
				return (string)this[SIPFEServerConfigurationSchema.UMPodRedirectTemplate];
			}
			set
			{
				this[SIPFEServerConfigurationSchema.UMPodRedirectTemplate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string UMForwardingAddressTemplate
		{
			get
			{
				return (string)this[SIPFEServerConfigurationSchema.UMForwardingAddressTemplate];
			}
			set
			{
				this[SIPFEServerConfigurationSchema.UMForwardingAddressTemplate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public UMStartupMode UMStartupMode
		{
			get
			{
				return (UMStartupMode)this[SIPFEServerConfigurationSchema.UMStartupMode];
			}
			set
			{
				this[SIPFEServerConfigurationSchema.UMStartupMode] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> DialPlans
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[SIPFEServerConfigurationSchema.DialPlans];
			}
			set
			{
				this[SIPFEServerConfigurationSchema.DialPlans] = value;
			}
		}

		public string UMCertificateThumbprint
		{
			get
			{
				return (string)this[SIPFEServerConfigurationSchema.UMCertificateThumbprint];
			}
			internal set
			{
				this[SIPFEServerConfigurationSchema.UMCertificateThumbprint] = value;
			}
		}

		public NetworkAddressCollection NetworkAddress
		{
			get
			{
				return (NetworkAddressCollection)this[SIPFEServerConfigurationSchema.NetworkAddress];
			}
			internal set
			{
				this[SIPFEServerConfigurationSchema.NetworkAddress] = value;
			}
		}

		public int VersionNumber
		{
			get
			{
				return (int)this[SIPFEServerConfigurationSchema.VersionNumber];
			}
			internal set
			{
				this[SIPFEServerConfigurationSchema.VersionNumber] = value;
			}
		}

		public ServerRole CurrentServerRole
		{
			get
			{
				return (ServerRole)this[SIPFEServerConfigurationSchema.CurrentServerRole];
			}
			internal set
			{
				this[SIPFEServerConfigurationSchema.CurrentServerRole] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IPAddressFamilyConfigurable
		{
			get
			{
				return (bool)this[SIPFEServerConfigurationSchema.IPAddressFamilyConfigurable];
			}
			set
			{
				this[SIPFEServerConfigurationSchema.IPAddressFamilyConfigurable] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IPAddressFamily IPAddressFamily
		{
			get
			{
				return (IPAddressFamily)this[SIPFEServerConfigurationSchema.IPAddressFamily];
			}
			set
			{
				this[SIPFEServerConfigurationSchema.IPAddressFamily] = value;
			}
		}

		internal static ObjectId GetRootId(Server server)
		{
			return server.Id.GetChildId("Protocols").GetChildId("SIP");
		}

		internal static SIPFEServerConfiguration Find()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 563, "Find", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\SIPFEServerConfiguration.cs");
			return SIPFEServerConfiguration.Find(LocalServer.GetServer(), tenantOrTopologyConfigurationSession);
		}

		internal static SIPFEServerConfiguration Find(Server server, IConfigurationSession adSession)
		{
			if (server == null || adSession == null)
			{
				return null;
			}
			ObjectId rootId = SIPFEServerConfiguration.GetRootId(server);
			if (rootId == null)
			{
				return null;
			}
			SIPFEServerConfiguration[] array = adSession.Find<SIPFEServerConfiguration>(rootId as ADObjectId, QueryScope.OneLevel, null, null, 2);
			if (array == null || array.Length <= 0)
			{
				return null;
			}
			return array[0];
		}

		private const string MostDerivedClass = "msExchProtocolCfgSIPFEServer";

		private const string Protocols = "Protocols";

		public const string ProtocolName = "SIP";

		private static readonly SIPFEServerConfigurationSchema schema = ObjectSchema.GetInstance<SIPFEServerConfigurationSchema>();

		private static readonly ADObjectId parentPath = new ADObjectId("CN=SIP");
	}
}
