using System;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "SendConnector", SupportsShouldProcess = true, DefaultParameterSetName = "AddressSpaces")]
	public sealed class NewSendConnector : NewSystemConfigurationObjectTask<SmtpSendConnectorConfig>
	{
		[Parameter(Mandatory = false)]
		public NewSendConnector.UsageType Usage
		{
			internal get
			{
				return this.usage;
			}
			set
			{
				this.usage = value;
				this.usageSetCount++;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Internet
		{
			internal get
			{
				return new SwitchParameter(this.Usage == NewSendConnector.UsageType.Internet);
			}
			set
			{
				this.Usage = NewSendConnector.UsageType.Internet;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Internal
		{
			internal get
			{
				return new SwitchParameter(this.Usage == NewSendConnector.UsageType.Internal);
			}
			set
			{
				this.Usage = NewSendConnector.UsageType.Internal;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Partner
		{
			internal get
			{
				return new SwitchParameter(this.Usage == NewSendConnector.UsageType.Partner);
			}
			set
			{
				this.Usage = NewSendConnector.UsageType.Partner;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Custom
		{
			internal get
			{
				return new SwitchParameter(this.Usage == NewSendConnector.UsageType.Custom);
			}
			set
			{
				this.Usage = NewSendConnector.UsageType.Custom;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			internal get
			{
				return this.force;
			}
			set
			{
				this.force = value;
			}
		}

		[Parameter(ParameterSetName = "AddressSpaces", Mandatory = true)]
		public MultiValuedProperty<AddressSpace> AddressSpaces
		{
			get
			{
				return this.DataObject.AddressSpaces;
			}
			set
			{
				this.DataObject.AddressSpaces = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DomainSecureEnabled
		{
			get
			{
				return this.DataObject.DomainSecureEnabled;
			}
			set
			{
				this.DataObject.DomainSecureEnabled = value;
				this.isDomainSecureEnabledSet = true;
			}
		}

		[Parameter]
		public bool DNSRoutingEnabled
		{
			get
			{
				return this.DataObject.DNSRoutingEnabled;
			}
			set
			{
				this.DataObject.DNSRoutingEnabled = value;
				this.isDnsRoutingEnabledSet = true;
			}
		}

		[Parameter]
		public MultiValuedProperty<SmartHost> SmartHosts
		{
			get
			{
				return this.DataObject.SmartHosts;
			}
			set
			{
				this.DataObject.SmartHosts = value;
			}
		}

		[Parameter]
		public int Port
		{
			get
			{
				return this.DataObject.Port;
			}
			set
			{
				this.DataObject.Port = value;
			}
		}

		[Parameter]
		public EnhancedTimeSpan ConnectionInactivityTimeOut
		{
			get
			{
				return this.DataObject.ConnectionInactivityTimeOut;
			}
			set
			{
				this.DataObject.ConnectionInactivityTimeOut = value;
			}
		}

		[Parameter]
		public Unlimited<ByteQuantifiedSize> MaxMessageSize
		{
			get
			{
				return this.DataObject.MaxMessageSize;
			}
			set
			{
				this.DataObject.MaxMessageSize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Fqdn Fqdn
		{
			get
			{
				return this.DataObject.Fqdn;
			}
			set
			{
				this.DataObject.Fqdn = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpX509Identifier TlsCertificateName
		{
			get
			{
				return this.DataObject.TlsCertificateName;
			}
			set
			{
				this.DataObject.TlsCertificateName = value;
			}
		}

		[Parameter]
		public bool ForceHELO
		{
			get
			{
				return this.DataObject.ForceHELO;
			}
			set
			{
				this.DataObject.ForceHELO = value;
			}
		}

		[Parameter]
		public bool FrontendProxyEnabled
		{
			get
			{
				return this.DataObject.FrontendProxyEnabled;
			}
			set
			{
				this.DataObject.FrontendProxyEnabled = value;
			}
		}

		[Parameter]
		public bool IgnoreSTARTTLS
		{
			get
			{
				return this.DataObject.IgnoreSTARTTLS;
			}
			set
			{
				this.DataObject.IgnoreSTARTTLS = value;
			}
		}

		[Parameter]
		public bool CloudServicesMailEnabled
		{
			get
			{
				return this.DataObject.CloudServicesMailEnabled;
			}
			set
			{
				this.DataObject.CloudServicesMailEnabled = value;
			}
		}

		[Parameter]
		public bool RequireOorg
		{
			get
			{
				return this.DataObject.RequireOorg;
			}
			set
			{
				this.DataObject.RequireOorg = value;
			}
		}

		[Parameter]
		public bool RequireTLS
		{
			get
			{
				return this.DataObject.RequireTLS;
			}
			set
			{
				this.DataObject.RequireTLS = value;
			}
		}

		[Parameter]
		public bool Enabled
		{
			get
			{
				return this.DataObject.Enabled;
			}
			set
			{
				this.DataObject.Enabled = value;
			}
		}

		[Parameter]
		public ProtocolLoggingLevel ProtocolLoggingLevel
		{
			get
			{
				return this.DataObject.ProtocolLoggingLevel;
			}
			set
			{
				this.DataObject.ProtocolLoggingLevel = value;
			}
		}

		[Parameter]
		public SmtpSendConnectorConfig.AuthMechanisms SmartHostAuthMechanism
		{
			get
			{
				return this.DataObject.SmartHostAuthMechanism;
			}
			set
			{
				this.DataObject.SmartHostAuthMechanism = value;
			}
		}

		[Parameter]
		public bool UseExternalDNSServersEnabled
		{
			get
			{
				return this.DataObject.UseExternalDNSServersEnabled;
			}
			set
			{
				this.DataObject.UseExternalDNSServersEnabled = value;
			}
		}

		[Parameter]
		public string Comment
		{
			get
			{
				return this.DataObject.Comment;
			}
			set
			{
				this.DataObject.Comment = value;
			}
		}

		[Parameter]
		public IPAddress SourceIPAddress
		{
			get
			{
				return this.DataObject.SourceIPAddress;
			}
			set
			{
				this.DataObject.SourceIPAddress = value;
			}
		}

		[Parameter]
		public int SmtpMaxMessagesPerConnection
		{
			get
			{
				return this.DataObject.SmtpMaxMessagesPerConnection;
			}
			set
			{
				this.DataObject.SmtpMaxMessagesPerConnection = value;
			}
		}

		[Parameter]
		public PSCredential AuthenticationCredential
		{
			get
			{
				return this.DataObject.AuthenticationCredential;
			}
			set
			{
				this.DataObject.AuthenticationCredential = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<ServerIdParameter> SourceTransportServers
		{
			get
			{
				return (MultiValuedProperty<ServerIdParameter>)base.Fields["SourceTransportServers"];
			}
			set
			{
				base.Fields["SourceTransportServers"] = value;
			}
		}

		[Parameter(ParameterSetName = "AddressSpaces", Mandatory = false)]
		public bool IsScopedConnector
		{
			get
			{
				return this.DataObject.IsScopedConnector;
			}
			set
			{
				this.DataObject.IsScopedConnector = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpDomainWithSubdomains TlsDomain
		{
			get
			{
				return this.DataObject.TlsDomain;
			}
			set
			{
				this.DataObject.TlsDomain = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TlsAuthLevel? TlsAuthLevel
		{
			get
			{
				return this.DataObject.TlsAuthLevel;
			}
			set
			{
				this.DataObject.TlsAuthLevel = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ErrorPolicies ErrorPolicies
		{
			get
			{
				return this.DataObject.ErrorPolicies;
			}
			set
			{
				this.DataObject.ErrorPolicies = value;
			}
		}

		private bool IsUsageSet
		{
			get
			{
				if (this.usageSetCount > 1)
				{
					throw new ApplicationException("usage parameters should be checked and tasks execution ended before using this method.");
				}
				return this.usageSetCount > 0;
			}
		}

		internal static MultiValuedProperty<ADObjectId> GetDefaultSourceTransportServers()
		{
			Server server = null;
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 516, "GetDefaultSourceTransportServers", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Transport\\NewSendConnector.cs");
			try
			{
				server = topologyConfigurationSession.FindLocalServer();
			}
			catch (ComputerNameNotCurrentlyAvailableException)
			{
			}
			catch (LocalServerNotFoundException)
			{
			}
			MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>();
			if (server != null && server.IsHubTransportServer)
			{
				multiValuedProperty.Add(server.Id);
			}
			return multiValuedProperty;
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewSendConnectorAddressSpaces(base.Name, base.FormatMultiValuedProperty(this.AddressSpaces));
			}
		}

		internal static LocalizedException CrossObjectValidate(SmtpSendConnectorConfig connector, IConfigurationSession session, Server localServer, Task task, out bool multiSiteConnector)
		{
			bool flag;
			LocalizedException ex = NewSendConnector.ValidateSourceServers(connector, session, localServer, task, out flag, out multiSiteConnector);
			if (ex != null)
			{
				return ex;
			}
			if (!MultiValuedPropertyBase.IsNullOrEmpty(connector.AddressSpaces))
			{
				foreach (AddressSpace addressSpace in connector.AddressSpaces)
				{
					if (!addressSpace.IsSmtpType)
					{
						if (flag)
						{
							return new SendConnectorNonSmtpAddressSpaceOnEdgeException(addressSpace.ToString());
						}
						if (connector.DNSRoutingEnabled)
						{
							return new SendConnectorNonSmtpAddressSpaceOnDNSConnectorException(addressSpace.ToString());
						}
					}
					if (addressSpace.Address != null && addressSpace.Address.Equals("--", StringComparison.InvariantCulture))
					{
						if (!flag)
						{
							return new SendConnectorDashdashAddressSpaceNotOffEdgeException();
						}
						if (connector.AddressSpaces.Count > 1)
						{
							return new SendConnectorDashdashAddressSpaceNotUniqueException();
						}
					}
				}
			}
			if (!MultiValuedPropertyBase.IsNullOrEmpty(connector.SmartHosts))
			{
				foreach (SmartHost smartHost in connector.SmartHosts)
				{
					if (smartHost.Domain != null && smartHost.Domain.ToString().Equals("--", StringComparison.InvariantCulture))
					{
						if (!flag)
						{
							return new SendConnectorDashdashSmarthostNotOffEdgeException();
						}
						if (connector.SmartHosts.Count > 1)
						{
							return new SendConnectorDashdashSmarthostNotUniqueException();
						}
					}
				}
			}
			if (flag && connector.FrontendProxyEnabled)
			{
				return new SendConnectorProxyEnabledOnEdgeException();
			}
			if (localServer != null && localServer.IsEdgeServer)
			{
				ex = NewSendConnector.ValidateSourceIPAddress(connector);
				if (ex != null)
				{
					return ex;
				}
			}
			return null;
		}

		internal static Exception CheckDNSAndSmartHostParameters(SmtpSendConnectorConfig sendConnector)
		{
			if (sendConnector.IsModified(SmtpSendConnectorConfigSchema.SmartHostsString) && !string.IsNullOrEmpty(sendConnector.SmartHostsString))
			{
				return new InvalidOperationException(Strings.DNSRoutingEnabledMustNotBeSpecified);
			}
			return null;
		}

		internal static Exception CheckDNSAndSmartHostAuthMechanismParameters(SmtpSendConnectorConfig sendConnector)
		{
			if (sendConnector.SmartHostAuthMechanism == SmtpSendConnectorConfig.AuthMechanisms.None)
			{
				return null;
			}
			if (sendConnector.IsModified(SmtpSendConnectorConfigSchema.SmartHostAuthMechanism) && !sendConnector.IsModified(SmtpSendConnectorConfigSchema.DNSRoutingEnabled))
			{
				return new InvalidOperationException(Strings.DNSRoutingEnabledMustBeSpecifiedForAuthMechanism);
			}
			return null;
		}

		internal static Exception CheckTLSParameters(SmtpSendConnectorConfig sendConnector)
		{
			if (sendConnector.RequireTLS && sendConnector.IgnoreSTARTTLS)
			{
				return new InvalidOperationException(Strings.IgnoreRequireTLSConflict);
			}
			return null;
		}

		internal static void ClearSmartHostsListIfNecessary(SmtpSendConnectorConfig sendConnector)
		{
			if (sendConnector.DNSRoutingEnabled && sendConnector.SmartHosts != null && sendConnector.SmartHosts.Count != 0)
			{
				sendConnector.SmartHosts = null;
			}
		}

		internal static void SetSmartHostAuthMechanismIfNecessary(SmtpSendConnectorConfig sendConnector)
		{
			if (sendConnector.DNSRoutingEnabled && sendConnector.SmartHostAuthMechanism != SmtpSendConnectorConfig.AuthMechanisms.None)
			{
				sendConnector.SmartHostAuthMechanism = SmtpSendConnectorConfig.AuthMechanisms.None;
				sendConnector.AuthenticationCredential = null;
			}
		}

		private RawSecurityDescriptor ConfigureDefaultSecurityDescriptor(RawSecurityDescriptor originalSecurityDescriptor)
		{
			PrincipalPermissionList defaultPermission = NewSendConnector.GetDefaultPermission(this.isHubTransportServer);
			return defaultPermission.AddExtendedRightsToSecurityDescriptor(originalSecurityDescriptor);
		}

		protected override IConfigurable PrepareDataObject()
		{
			SmtpSendConnectorConfig smtpSendConnectorConfig = (SmtpSendConnectorConfig)base.PrepareDataObject();
			try
			{
				this.localServer = ((ITopologyConfigurationSession)base.DataSession).ReadLocalServer();
			}
			catch (TransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ResourceUnavailable, this.DataObject);
			}
			this.isHubTransportServer = (this.localServer != null && this.localServer.IsHubTransportServer);
			bool isEdgeConnector = this.localServer != null && this.localServer.IsEdgeServer;
			if (this.SourceTransportServers != null)
			{
				smtpSendConnectorConfig.SourceTransportServers = base.ResolveIdParameterCollection<ServerIdParameter, Server, ADObjectId>(this.SourceTransportServers, base.DataSession, this.RootId, null, (ExchangeErrorCategory)0, new Func<IIdentityParameter, LocalizedString>(Strings.ErrorServerNotFound), new Func<IIdentityParameter, LocalizedString>(Strings.ErrorServerNotUnique), null, delegate(IConfigurable configObject)
				{
					Server server = (Server)configObject;
					isEdgeConnector |= server.IsEdgeServer;
					return server;
				});
			}
			MultiValuedProperty<ADObjectId> multiValuedProperty = this.DataObject.SourceTransportServers;
			if (this.localServer != null && this.localServer.IsHubTransportServer && (multiValuedProperty == null || multiValuedProperty.Count == 0))
			{
				multiValuedProperty = new MultiValuedProperty<ADObjectId>(false, SendConnectorSchema.SourceTransportServers, new ADObjectId[]
				{
					this.localServer.Id
				});
				this.DataObject.SourceTransportServers = multiValuedProperty;
			}
			if (multiValuedProperty != null && multiValuedProperty.Count > 0)
			{
				ManageSendConnectors.SetConnectorHomeMta(this.DataObject, (IConfigurationSession)base.DataSession);
			}
			if (!this.DataObject.IsModified(SendConnectorSchema.MaxMessageSize))
			{
				if (this.IsUsageSet && this.usage == NewSendConnector.UsageType.Internal)
				{
					this.MaxMessageSize = Unlimited<ByteQuantifiedSize>.UnlimitedValue;
				}
				else
				{
					this.MaxMessageSize = ByteQuantifiedSize.FromMB(35UL);
				}
			}
			if (!this.DataObject.IsModified(SmtpSendConnectorConfigSchema.UseExternalDNSServersEnabled) && isEdgeConnector)
			{
				this.UseExternalDNSServersEnabled = true;
			}
			ManageSendConnectors.SetConnectorId(smtpSendConnectorConfig, ((ITopologyConfigurationSession)base.DataSession).GetRoutingGroupId());
			return smtpSendConnectorConfig;
		}

		protected override void InternalProcessRecord()
		{
			SmtpSendConnectorConfig dataObject = this.DataObject;
			if (!TopologyProvider.IsAdamTopology() && !dataObject.Enabled && !this.force && !base.ShouldContinue(Strings.ConfirmationMessageDisableSendConnector))
			{
				return;
			}
			Exception ex = null;
			if (dataObject.DNSRoutingEnabled)
			{
				ex = NewSendConnector.CheckDNSAndSmartHostParameters(this.DataObject);
				if (ex == null)
				{
					ex = NewSendConnector.CheckDNSAndSmartHostAuthMechanismParameters(this.DataObject);
				}
			}
			if (ex == null)
			{
				ex = NewSendConnector.CheckTLSParameters(this.DataObject);
			}
			if (ex != null)
			{
				base.WriteError(ex, ErrorCategory.InvalidOperation, this.DataObject.Identity);
				return;
			}
			NewSendConnector.ClearSmartHostsListIfNecessary(this.DataObject);
			NewSendConnector.SetSmartHostAuthMechanismIfNecessary(this.DataObject);
			if (dataObject.IsScopedConnector)
			{
				ManageSendConnectors.AdjustAddressSpaces(dataObject);
			}
			base.InternalProcessRecord();
			ITopologyConfigurationSession topologyConfigurationSession = (ITopologyConfigurationSession)base.DataSession;
			SmtpSendConnectorConfig smtpSendConnectorConfig = null;
			try
			{
				smtpSendConnectorConfig = topologyConfigurationSession.Read<SmtpSendConnectorConfig>(this.DataObject.OriginalId);
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, this.DataObject);
				return;
			}
			RawSecurityDescriptor originalSecurityDescriptor = smtpSendConnectorConfig.ReadSecurityDescriptor();
			RawSecurityDescriptor rawSecurityDescriptor = null;
			try
			{
				rawSecurityDescriptor = this.ConfigureDefaultSecurityDescriptor(originalSecurityDescriptor);
			}
			catch (LocalizedException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidOperation, this.DataObject);
				return;
			}
			if (rawSecurityDescriptor != null)
			{
				topologyConfigurationSession.SaveSecurityDescriptor(this.DataObject.OriginalId, rawSecurityDescriptor);
			}
			if (!TopologyProvider.IsAdamTopology())
			{
				ManageSendConnectors.UpdateGwartLastModified(topologyConfigurationSession, this.DataObject.SourceRoutingGroup, new ManageSendConnectors.ThrowTerminatingErrorDelegate(base.ThrowTerminatingError));
			}
		}

		protected override void InternalValidate()
		{
			if (Server.IsSubscribedGateway(base.GlobalConfigSession))
			{
				base.WriteError(new CannotRunOnSubscribedEdgeException(), ErrorCategory.InvalidOperation, null);
			}
			if (this.usageSetCount > 1)
			{
				base.WriteError(new NewSendConnectorIncorrectUsageParametersException(), ErrorCategory.InvalidOperation, null);
			}
			if (this.IsUsageSet && this.usage == NewSendConnector.UsageType.Partner)
			{
				if (!this.isDomainSecureEnabledSet)
				{
					this.DataObject.DomainSecureEnabled = true;
				}
				if (!this.isDnsRoutingEnabledSet)
				{
					this.DataObject.DNSRoutingEnabled = true;
				}
			}
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			bool flag;
			LocalizedException ex = NewSendConnector.CrossObjectValidate(this.DataObject, (IConfigurationSession)base.DataSession, this.localServer, this, out flag);
			if (ex != null)
			{
				base.WriteError(ex, ErrorCategory.InvalidOperation, this.DataObject);
				return;
			}
			if (flag)
			{
				this.WriteWarning(Strings.WarningMultiSiteSourceServers);
			}
		}

		private static LocalizedException ValidateSourceServers(SmtpSendConnectorConfig connector, IConfigurationSession session, Server localServer, Task task, out bool edgeConnector, out bool multiSiteConnector)
		{
			edgeConnector = false;
			multiSiteConnector = false;
			MultiValuedProperty<ADObjectId> sourceTransportServers = connector.SourceTransportServers;
			if (localServer == null || !localServer.IsEdgeServer)
			{
				ADObjectId sourceRoutingGroup = connector.SourceRoutingGroup;
				return ManageSendConnectors.ValidateTransportServers(session, connector, ref sourceRoutingGroup, true, true, task, out edgeConnector, out multiSiteConnector);
			}
			if (sourceTransportServers != null && sourceTransportServers.Count > 0)
			{
				return new SendConnectorSourceServersSetForEdgeException();
			}
			edgeConnector = true;
			return null;
		}

		private static LocalizedException ValidateSourceIPAddress(SmtpSendConnectorConfig connector)
		{
			IPAddress sourceIPAddress = connector.SourceIPAddress;
			if (sourceIPAddress.AddressFamily != AddressFamily.InterNetwork && sourceIPAddress.AddressFamily != AddressFamily.InterNetworkV6)
			{
				return new SendConnectorInvalidSourceIPAddressException();
			}
			if (sourceIPAddress.Equals(IPAddress.Any) || sourceIPAddress.Equals(IPAddress.IPv6Any) || IPAddress.IsLoopback(sourceIPAddress))
			{
				return null;
			}
			bool flag = false;
			IPHostEntry hostEntry = Dns.GetHostEntry(string.Empty);
			for (int i = 0; i < hostEntry.AddressList.Length; i++)
			{
				if (sourceIPAddress.Equals(hostEntry.AddressList[i]))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return new SendConnectorInvalidSourceIPAddressException();
			}
			return null;
		}

		internal static PrincipalPermissionList GetDefaultPermission(bool isHubTransportServer)
		{
			PrincipalPermissionList principalPermissionList = new PrincipalPermissionList(5);
			principalPermissionList.Add(new SecurityIdentifier(WellKnownSidType.AnonymousSid, null), Permission.SendRoutingHeaders);
			principalPermissionList.Add(WellKnownSids.PartnerServers, Permission.SendRoutingHeaders);
			principalPermissionList.Add(WellKnownSids.LegacyExchangeServers, Permission.SMTPSendEXCH50 | Permission.SendRoutingHeaders);
			principalPermissionList.Add(WellKnownSids.HubTransportServers, Permission.SMTPSendEXCH50 | Permission.SendRoutingHeaders | Permission.SendForestHeaders | Permission.SendOrganizationHeaders | Permission.SMTPSendXShadow);
			principalPermissionList.Add(WellKnownSids.EdgeTransportServers, Permission.SMTPSendEXCH50 | Permission.SendRoutingHeaders | Permission.SendForestHeaders | Permission.SendOrganizationHeaders | Permission.SMTPSendXShadow);
			principalPermissionList.Add(WellKnownSids.ExternallySecuredServers, Permission.SMTPSendEXCH50 | Permission.SendRoutingHeaders);
			if (isHubTransportServer)
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 1215, "GetDefaultPermission", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Transport\\NewSendConnector.cs");
				IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1219, "GetDefaultPermission", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Transport\\NewSendConnector.cs");
				configurationSession.UseConfigNC = false;
				principalPermissionList.Add(NewEdgeSubscription.GetSidForExchangeKnownGuid(tenantOrRootOrgRecipientSession, WellKnownGuid.ExSWkGuid, configurationSession.ConfigurationNamingContext.DistinguishedName), Permission.SMTPSendEXCH50 | Permission.SendRoutingHeaders | Permission.SendForestHeaders | Permission.SendOrganizationHeaders | Permission.SMTPSendXShadow);
			}
			return principalPermissionList;
		}

		private const string AddressSpacesParameterSetName = "AddressSpaces";

		private const Permission FullSendPermission = Permission.SMTPSendEXCH50 | Permission.SendRoutingHeaders | Permission.SendForestHeaders | Permission.SendOrganizationHeaders | Permission.SMTPSendXShadow;

		private Server localServer;

		private bool isHubTransportServer;

		private NewSendConnector.UsageType usage;

		private int usageSetCount;

		private bool isDomainSecureEnabledSet;

		private bool isDnsRoutingEnabledSet;

		private SwitchParameter force;

		public enum UsageType
		{
			[LocDescription(Strings.IDs.UsageTypeCustom)]
			Custom,
			[LocDescription(Strings.IDs.UsageTypeInternal)]
			Internal,
			[LocDescription(Strings.IDs.UsageTypeInternet)]
			Internet,
			[LocDescription(Strings.IDs.UsageTypePartner)]
			Partner
		}
	}
}
