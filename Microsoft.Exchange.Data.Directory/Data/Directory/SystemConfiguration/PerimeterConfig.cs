using System;
using System.Management.Automation;
using System.Net;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class PerimeterConfig : ADContainer
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return PerimeterConfig.adSchema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchTenantPerimeterSettings";
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return PerimeterConfig.parentPath;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		[Parameter(Mandatory = false)]
		public string PerimeterOrgId
		{
			get
			{
				return (string)this[PerimeterConfigSchema.PerimeterOrgId];
			}
			set
			{
				this[PerimeterConfigSchema.PerimeterOrgId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SyncToHotmailEnabled
		{
			get
			{
				return (bool)this[PerimeterConfigSchema.SyncToHotmailEnabled];
			}
			set
			{
				this[PerimeterConfigSchema.SyncToHotmailEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RouteOutboundViaEhfEnabled
		{
			get
			{
				return (bool)this[PerimeterConfigSchema.RouteOutboundViaEhfEnabled];
			}
			set
			{
				this[PerimeterConfigSchema.RouteOutboundViaEhfEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IPSkiplistingEnabled
		{
			get
			{
				return (bool)this[PerimeterConfigSchema.IPSkiplistingEnabled];
			}
			set
			{
				this[PerimeterConfigSchema.IPSkiplistingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EhfConfigSyncEnabled
		{
			get
			{
				return (bool)this[PerimeterConfigSchema.EhfConfigSyncEnabled];
			}
			set
			{
				this[PerimeterConfigSchema.EhfConfigSyncEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EhfAdminAccountSyncEnabled
		{
			get
			{
				return (bool)this[PerimeterConfigSchema.EhfAdminAccountSyncEnabled];
			}
			set
			{
				this[PerimeterConfigSchema.EhfAdminAccountSyncEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IPSafelistingSyncEnabled
		{
			get
			{
				return (bool)this[PerimeterConfigSchema.IPSafelistingSyncEnabled];
			}
			set
			{
				this[PerimeterConfigSchema.IPSafelistingSyncEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MigrationInProgress
		{
			get
			{
				return (bool)this[PerimeterConfigSchema.MigrationInProgress];
			}
			set
			{
				this[PerimeterConfigSchema.MigrationInProgress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RouteOutboundViaFfoFrontendEnabled
		{
			get
			{
				return (bool)this[PerimeterConfigSchema.RouteOutboundViaFfoFrontendEnabled];
			}
			set
			{
				this[PerimeterConfigSchema.RouteOutboundViaFfoFrontendEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EheEnabled
		{
			get
			{
				return (bool)this[PerimeterConfigSchema.EheEnabled];
			}
			set
			{
				this[PerimeterConfigSchema.EheEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RMSOFwdSyncEnabled
		{
			get
			{
				return (bool)this[PerimeterConfigSchema.RMSOFwdSyncEnabled];
			}
			set
			{
				this[PerimeterConfigSchema.RMSOFwdSyncEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EheDecryptEnabled
		{
			get
			{
				return (bool)this[PerimeterConfigSchema.EheDecryptEnabled];
			}
			set
			{
				this[PerimeterConfigSchema.EheDecryptEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPAddress> GatewayIPAddresses
		{
			get
			{
				return (MultiValuedProperty<IPAddress>)this[PerimeterConfigSchema.GatewayIPAddresses];
			}
			set
			{
				this[PerimeterConfigSchema.GatewayIPAddresses] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPAddress> InternalServerIPAddresses
		{
			get
			{
				return (MultiValuedProperty<IPAddress>)this[PerimeterConfigSchema.InternalServerIPAddresses];
			}
			set
			{
				this[PerimeterConfigSchema.InternalServerIPAddresses] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpDomain PartnerRoutingDomain
		{
			get
			{
				return (SmtpDomain)this[PerimeterConfigSchema.PartnerRoutingDomain];
			}
			set
			{
				this[PerimeterConfigSchema.PartnerRoutingDomain] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpDomain PartnerConnectorDomain
		{
			get
			{
				return (SmtpDomain)this[PerimeterConfigSchema.PartnerConnectorDomain];
			}
			set
			{
				this[PerimeterConfigSchema.PartnerConnectorDomain] = value;
			}
		}

		public ADObjectId MailFlowPartner
		{
			get
			{
				return (ADObjectId)this[PerimeterConfigSchema.MailFlowPartner];
			}
			set
			{
				this[PerimeterConfigSchema.MailFlowPartner] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SafelistingUIMode SafelistingUIMode
		{
			get
			{
				return (SafelistingUIMode)((int)this[PerimeterConfigSchema.SafelistingUIMode]);
			}
			set
			{
				this[PerimeterConfigSchema.SafelistingUIMode] = (int)value;
			}
		}

		private const string MostDerivedClassInternal = "msExchTenantPerimeterSettings";

		private static readonly PerimeterConfigSchema adSchema = ObjectSchema.GetInstance<PerimeterConfigSchema>();

		private static readonly ADObjectId parentPath = new ADObjectId("CN=Transport Settings");
	}
}
