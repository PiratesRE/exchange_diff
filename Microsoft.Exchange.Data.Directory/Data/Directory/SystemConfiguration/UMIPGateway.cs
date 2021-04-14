using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class UMIPGateway : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return UMIPGateway.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return UMIPGateway.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return UMIPGateway.parentPath;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = false)]
		public UMSmartHost Address
		{
			get
			{
				return (UMSmartHost)this[UMIPGatewaySchema.Address];
			}
			set
			{
				this[UMIPGatewaySchema.Address] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OutcallsAllowed
		{
			get
			{
				return (bool)this[UMIPGatewaySchema.OutcallsAllowed];
			}
			set
			{
				this[UMIPGatewaySchema.OutcallsAllowed] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public GatewayStatus Status
		{
			get
			{
				return (GatewayStatus)this[UMIPGatewaySchema.Status];
			}
			set
			{
				this[UMIPGatewaySchema.Status] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int Port
		{
			get
			{
				return (int)this[UMIPGatewaySchema.Port];
			}
			set
			{
				this[UMIPGatewaySchema.Port] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Simulator
		{
			get
			{
				return (bool)this[UMIPGatewaySchema.Simulator];
			}
			set
			{
				this[UMIPGatewaySchema.Simulator] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IPAddressFamily IPAddressFamily
		{
			get
			{
				return (IPAddressFamily)this[UMIPGatewaySchema.IPAddressFamily];
			}
			set
			{
				this[UMIPGatewaySchema.IPAddressFamily] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DelayedSourcePartyInfoEnabled
		{
			get
			{
				return (bool)this[UMIPGatewaySchema.DelayedSourcePartyInfoEnabled];
			}
			set
			{
				this[UMIPGatewaySchema.DelayedSourcePartyInfoEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MessageWaitingIndicatorAllowed
		{
			get
			{
				return (bool)this[UMIPGatewaySchema.MessageWaitingIndicatorAllowed];
			}
			set
			{
				this[UMIPGatewaySchema.MessageWaitingIndicatorAllowed] = value;
			}
		}

		public MultiValuedProperty<UMHuntGroup> HuntGroups
		{
			get
			{
				return (MultiValuedProperty<UMHuntGroup>)this[UMIPGatewaySchema.HuntGroups];
			}
			private set
			{
				this[UMIPGatewaySchema.HuntGroups] = value;
			}
		}

		public UMGlobalCallRoutingScheme GlobalCallRoutingScheme
		{
			get
			{
				return (UMGlobalCallRoutingScheme)this[UMIPGatewaySchema.GlobalCallRoutingScheme];
			}
			set
			{
				this[UMIPGatewaySchema.GlobalCallRoutingScheme] = value;
			}
		}

		public string ForwardingAddress
		{
			get
			{
				return this.forwardingAddress;
			}
			internal set
			{
				this.forwardingAddress = value;
			}
		}

		internal override void Initialize()
		{
			base.Initialize();
			IConfigurationSession session = base.Session;
			UMHuntGroup[] values = session.FindPaged<UMHuntGroup>(base.Id, QueryScope.OneLevel, null, null, 0).ReadAllPages();
			this.HuntGroups = new MultiValuedProperty<UMHuntGroup>(true, null, values);
		}

		private static UMIPGatewaySchema schema = ObjectSchema.GetInstance<UMIPGatewaySchema>();

		private static string mostDerivedClass = "msExchUMIPGateway";

		private static ADObjectId parentPath = new ADObjectId("CN=UM IPGateway Container");

		private string forwardingAddress = string.Empty;
	}
}
