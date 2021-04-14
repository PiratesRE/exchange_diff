using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class ExchangeServer : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return ExchangeServer.schema;
			}
		}

		public ExchangeServer()
		{
		}

		public ExchangeServer(Server dataObject) : base(dataObject)
		{
		}

		public new string Name
		{
			get
			{
				return (string)this[ADObjectSchema.Name];
			}
		}

		public LocalLongFullPath DataPath
		{
			get
			{
				return (LocalLongFullPath)this[ExchangeServerSchema.DataPath];
			}
		}

		public string Domain
		{
			get
			{
				return (string)this[ExchangeServerSchema.Domain];
			}
		}

		public ServerEditionType Edition
		{
			get
			{
				return (ServerEditionType)this[ExchangeServerSchema.Edition];
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return (string)this[ExchangeServerSchema.ExchangeLegacyDN];
			}
		}

		public int ExchangeLegacyServerRole
		{
			get
			{
				return (int)this[ExchangeServerSchema.ExchangeLegacyServerRole];
			}
		}

		public string Fqdn
		{
			get
			{
				return (string)this[ExchangeServerSchema.Fqdn];
			}
		}

		[Parameter(Mandatory = false)]
		public bool? CustomerFeedbackEnabled
		{
			get
			{
				return (bool?)this[ExchangeServerSchema.CustomerFeedbackEnabled];
			}
			set
			{
				this[ExchangeServerSchema.CustomerFeedbackEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri InternetWebProxy
		{
			get
			{
				return (Uri)this[ExchangeServerSchema.InternetWebProxy];
			}
			set
			{
				this[ExchangeServerSchema.InternetWebProxy] = value;
			}
		}

		public bool IsHubTransportServer
		{
			get
			{
				return (bool)this[ExchangeServerSchema.IsHubTransportServer];
			}
		}

		public bool IsClientAccessServer
		{
			get
			{
				if (!this.IsE15OrLater)
				{
					return (bool)this[ExchangeServerSchema.IsClientAccessServer];
				}
				return (bool)this[ExchangeServerSchema.IsCafeServer];
			}
		}

		public bool IsExchange2007OrLater
		{
			get
			{
				return (bool)this[ExchangeServerSchema.IsExchange2007OrLater];
			}
		}

		public bool IsEdgeServer
		{
			get
			{
				return (bool)this[ExchangeServerSchema.IsEdgeServer];
			}
		}

		public bool IsMailboxServer
		{
			get
			{
				return (bool)this[ExchangeServerSchema.IsMailboxServer];
			}
		}

		public bool IsE14OrLater
		{
			get
			{
				return (bool)this[ExchangeServerSchema.IsE14OrLater];
			}
		}

		public bool IsE15OrLater
		{
			get
			{
				return (bool)this[ExchangeServerSchema.IsE15OrLater];
			}
		}

		public bool IsProvisionedServer
		{
			get
			{
				return (bool)this[ExchangeServerSchema.IsProvisionedServer];
			}
		}

		public bool IsUnifiedMessagingServer
		{
			get
			{
				return (bool)this[ExchangeServerSchema.IsUnifiedMessagingServer];
			}
		}

		internal bool IsCafeServer
		{
			get
			{
				return (bool)this[ExchangeServerSchema.IsCafeServer];
			}
		}

		public bool IsFrontendTransportServer
		{
			get
			{
				return (bool)this[ExchangeServerSchema.IsFrontendTransportServer];
			}
		}

		public NetworkAddressCollection NetworkAddress
		{
			get
			{
				return (NetworkAddressCollection)this[ExchangeServerSchema.NetworkAddress];
			}
		}

		public string OrganizationalUnit
		{
			get
			{
				return (string)this[ExchangeServerSchema.OrganizationalUnit];
			}
		}

		public ServerVersion AdminDisplayVersion
		{
			get
			{
				return (ServerVersion)this[ExchangeServerSchema.AdminDisplayVersion];
			}
		}

		public ADObjectId Site
		{
			get
			{
				return (ADObjectId)this[ExchangeServerSchema.Site];
			}
		}

		public ServerRole ServerRole
		{
			get
			{
				ServerRole serverRole = (ServerRole)this[ExchangeServerSchema.CurrentServerRole];
				if (!this.IsE15OrLater)
				{
					return serverRole;
				}
				return ExchangeServer.ConvertE15ServerRoleToOutput(serverRole);
			}
		}

		public bool? ErrorReportingEnabled
		{
			get
			{
				return (bool?)this[ServerSchema.ErrorReportingEnabled];
			}
			set
			{
				this[ServerSchema.ErrorReportingEnabled] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> StaticDomainControllers
		{
			get
			{
				return (MultiValuedProperty<string>)this[ServerSchema.StaticDomainControllers];
			}
			set
			{
				this[ServerSchema.StaticDomainControllers] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> StaticGlobalCatalogs
		{
			get
			{
				return (MultiValuedProperty<string>)this[ServerSchema.StaticGlobalCatalogs];
			}
			set
			{
				this[ServerSchema.StaticGlobalCatalogs] = value;
			}
		}

		[Parameter]
		public string StaticConfigDomainController
		{
			get
			{
				return (string)this[ServerSchema.StaticConfigDomainController];
			}
			set
			{
				this[ServerSchema.StaticConfigDomainController] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> StaticExcludedDomainControllers
		{
			get
			{
				return (MultiValuedProperty<string>)this[ServerSchema.StaticExcludedDomainControllers];
			}
			set
			{
				this[ServerSchema.StaticExcludedDomainControllers] = value;
			}
		}

		[Parameter]
		public string MonitoringGroup
		{
			get
			{
				return (string)this[ExchangeServerSchema.MonitoringGroup];
			}
			set
			{
				this[ExchangeServerSchema.MonitoringGroup] = value;
			}
		}

		public MultiValuedProperty<string> CurrentDomainControllers
		{
			get
			{
				return (MultiValuedProperty<string>)this[ServerSchema.CurrentDomainControllers];
			}
			private set
			{
				this[ServerSchema.CurrentDomainControllers] = value;
			}
		}

		public MultiValuedProperty<string> CurrentGlobalCatalogs
		{
			get
			{
				return (MultiValuedProperty<string>)this[ServerSchema.CurrentGlobalCatalogs];
			}
			private set
			{
				this[ServerSchema.CurrentGlobalCatalogs] = value;
			}
		}

		public string CurrentConfigDomainController
		{
			get
			{
				return (string)this[ServerSchema.CurrentConfigDomainController];
			}
			private set
			{
				this[ServerSchema.CurrentConfigDomainController] = value;
			}
		}

		public string ProductID
		{
			get
			{
				return (string)this[ExchangeServerSchema.ProductID];
			}
		}

		public bool IsExchangeTrialEdition
		{
			get
			{
				return (bool)this[ExchangeServerSchema.IsExchangeTrialEdition];
			}
		}

		public bool IsExpiredExchangeTrialEdition
		{
			get
			{
				return (bool)this[ExchangeServerSchema.IsExpiredExchangeTrialEdition];
			}
		}

		public MailboxProvisioningAttributes MailboxProvisioningAttributes
		{
			get
			{
				return this[ServerSchema.MailboxProvisioningAttributes] as MailboxProvisioningAttributes;
			}
		}

		public EnhancedTimeSpan RemainingTrialPeriod
		{
			get
			{
				return (EnhancedTimeSpan)this[ServerSchema.RemainingTrialPeriod];
			}
		}

		internal static ServerRole ConvertE15ServerRoleToOutput(ServerRole serverRole)
		{
			if (serverRole == ServerRole.Edge)
			{
				return serverRole;
			}
			bool flag = (serverRole & ServerRole.Cafe) != ServerRole.None;
			bool flag2 = (serverRole & ServerRole.Mailbox) != ServerRole.None;
			ServerRole serverRole2 = ServerRole.Mailbox;
			if (Globals.IsDatacenter)
			{
				if (!flag)
				{
					serverRole2 |= ServerRole.FrontendTransport;
				}
				if (!flag2)
				{
					serverRole2 |= ServerRole.HubTransport;
				}
			}
			serverRole &= serverRole2;
			if (flag)
			{
				serverRole |= ServerRole.ClientAccess;
			}
			return serverRole;
		}

		internal void RefreshDsAccessData()
		{
			if (this.IsEdgeServer)
			{
				this.CurrentDomainControllers = new MultiValuedProperty<string>(new string[]
				{
					this.Fqdn
				});
				this.CurrentGlobalCatalogs = new MultiValuedProperty<string>(new string[]
				{
					this.Fqdn
				});
				this.CurrentConfigDomainController = this.Fqdn;
				return;
			}
			using (ServiceTopologyProvider serviceTopologyProvider = new ServiceTopologyProvider())
			{
				string partitionFqdn = (this.m_Session != null && this.m_Session.SessionSettings.PartitionId != null) ? this.m_Session.SessionSettings.PartitionId.ForestFQDN : TopologyProvider.LocalForestFqdn;
				this.CurrentDomainControllers = new MultiValuedProperty<string>(serviceTopologyProvider.GetCurrentDCs(partitionFqdn));
				this.CurrentGlobalCatalogs = new MultiValuedProperty<string>(serviceTopologyProvider.GetCurrentGCs(partitionFqdn));
				this.CurrentConfigDomainController = (serviceTopologyProvider.GetConfigDC(partitionFqdn, true) ?? string.Empty);
			}
		}

		private static ExchangeServerSchema schema = ObjectSchema.GetInstance<ExchangeServerSchema>();
	}
}
