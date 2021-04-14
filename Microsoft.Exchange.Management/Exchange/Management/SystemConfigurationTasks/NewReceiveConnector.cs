using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "ReceiveConnector", SupportsShouldProcess = true, DefaultParameterSetName = "Custom")]
	public sealed class NewReceiveConnector : NewSystemConfigurationObjectTask<ReceiveConnector>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewReceiveConnector(base.Name, base.FormatMultiValuedProperty(this.Bindings), base.FormatMultiValuedProperty(this.RemoteIPRanges));
			}
		}

		internal static NewReceiveConnector.UsageType[] TransportUsage()
		{
			return new NewReceiveConnector.UsageType[]
			{
				NewReceiveConnector.UsageType.Custom,
				NewReceiveConnector.UsageType.Internet,
				NewReceiveConnector.UsageType.Internal,
				NewReceiveConnector.UsageType.Client,
				NewReceiveConnector.UsageType.Partner
			};
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(ParameterSetName = "Internet", Mandatory = true)]
		public SwitchParameter Internet
		{
			internal get
			{
				return new SwitchParameter(this.Usage == NewReceiveConnector.UsageType.Internet);
			}
			set
			{
				this.Usage = NewReceiveConnector.UsageType.Internet;
			}
		}

		[Parameter(ParameterSetName = "Internal", Mandatory = true)]
		public SwitchParameter Internal
		{
			internal get
			{
				return new SwitchParameter(this.Usage == NewReceiveConnector.UsageType.Internal);
			}
			set
			{
				this.Usage = NewReceiveConnector.UsageType.Internal;
			}
		}

		[Parameter(ParameterSetName = "Client", Mandatory = true)]
		public SwitchParameter Client
		{
			internal get
			{
				return new SwitchParameter(this.Usage == NewReceiveConnector.UsageType.Client);
			}
			set
			{
				this.Usage = NewReceiveConnector.UsageType.Client;
			}
		}

		[Parameter(ParameterSetName = "Partner", Mandatory = true)]
		public SwitchParameter Partner
		{
			internal get
			{
				return new SwitchParameter(this.Usage == NewReceiveConnector.UsageType.Partner);
			}
			set
			{
				this.Usage = NewReceiveConnector.UsageType.Partner;
			}
		}

		[Parameter(ParameterSetName = "Custom", Mandatory = false)]
		public SwitchParameter Custom
		{
			internal get
			{
				return new SwitchParameter(this.Usage == NewReceiveConnector.UsageType.Custom);
			}
			set
			{
				this.Usage = NewReceiveConnector.UsageType.Custom;
			}
		}

		[Parameter(ParameterSetName = "UsageType", Mandatory = true)]
		public NewReceiveConnector.UsageType Usage
		{
			internal get
			{
				return this.usage;
			}
			set
			{
				this.usage = value;
				this.isUsageSet = true;
			}
		}

		[Parameter(Mandatory = false)]
		public AuthMechanisms AuthMechanism
		{
			get
			{
				return this.DataObject.AuthMechanism;
			}
			set
			{
				this.DataObject.AuthMechanism = value;
			}
		}

		[Parameter(ParameterSetName = "Custom", Mandatory = true)]
		[Parameter(ParameterSetName = "Internet", Mandatory = true)]
		[Parameter(Mandatory = false)]
		[Parameter(ParameterSetName = "Partner", Mandatory = true)]
		[Parameter(ParameterSetName = "UsageType", Mandatory = false)]
		public MultiValuedProperty<IPBinding> Bindings
		{
			get
			{
				return this.DataObject.Bindings;
			}
			set
			{
				this.DataObject.Bindings = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
		public bool RequireEHLODomain
		{
			get
			{
				return this.DataObject.RequireEHLODomain;
			}
			set
			{
				this.DataObject.RequireEHLODomain = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ConnectionTimeout
		{
			get
			{
				return this.DataObject.ConnectionTimeout;
			}
			set
			{
				this.DataObject.ConnectionTimeout = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ConnectionInactivityTimeout
		{
			get
			{
				return this.DataObject.ConnectionInactivityTimeout;
			}
			set
			{
				this.DataObject.ConnectionInactivityTimeout = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AcceptedDomainIdParameter DefaultDomain
		{
			get
			{
				return (AcceptedDomainIdParameter)base.Fields["DefaultDomain"];
			}
			set
			{
				base.Fields["DefaultDomain"] = value;
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
		public Fqdn ServiceDiscoveryFqdn
		{
			get
			{
				return this.DataObject.ServiceDiscoveryFqdn;
			}
			set
			{
				this.DataObject.ServiceDiscoveryFqdn = value;
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

		[Parameter(Mandatory = false)]
		public Unlimited<int> MessageRateLimit
		{
			get
			{
				return this.DataObject.MessageRateLimit;
			}
			set
			{
				this.DataObject.MessageRateLimit = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MessageRateSourceFlags MessageRateSource
		{
			get
			{
				return this.DataObject.MessageRateSource;
			}
			set
			{
				this.DataObject.MessageRateSource = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxInboundConnection
		{
			get
			{
				return this.DataObject.MaxInboundConnection;
			}
			set
			{
				this.DataObject.MaxInboundConnection = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxInboundConnectionPerSource
		{
			get
			{
				return this.DataObject.MaxInboundConnectionPerSource;
			}
			set
			{
				this.DataObject.MaxInboundConnectionPerSource = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize MaxHeaderSize
		{
			get
			{
				return this.DataObject.MaxHeaderSize;
			}
			set
			{
				this.DataObject.MaxHeaderSize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxHopCount
		{
			get
			{
				return this.DataObject.MaxHopCount;
			}
			set
			{
				this.DataObject.MaxHopCount = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxLocalHopCount
		{
			get
			{
				return this.DataObject.MaxLocalHopCount;
			}
			set
			{
				this.DataObject.MaxLocalHopCount = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxLogonFailures
		{
			get
			{
				return this.DataObject.MaxLogonFailures;
			}
			set
			{
				this.DataObject.MaxLogonFailures = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize MaxMessageSize
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
		public int MaxInboundConnectionPercentagePerSource
		{
			get
			{
				return this.DataObject.MaxInboundConnectionPercentagePerSource;
			}
			set
			{
				this.DataObject.MaxInboundConnectionPercentagePerSource = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxProtocolErrors
		{
			get
			{
				return this.DataObject.MaxProtocolErrors;
			}
			set
			{
				this.DataObject.MaxProtocolErrors = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxRecipientsPerMessage
		{
			get
			{
				return this.DataObject.MaxRecipientsPerMessage;
			}
			set
			{
				this.DataObject.MaxRecipientsPerMessage = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PermissionGroups PermissionGroups
		{
			get
			{
				return this.DataObject.PermissionGroups;
			}
			set
			{
				this.DataObject.PermissionGroups = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		[Parameter(ParameterSetName = "UsageType", Mandatory = false)]
		[Parameter(ParameterSetName = "Partner", Mandatory = true)]
		[Parameter(Mandatory = false)]
		[Parameter(ParameterSetName = "Internal", Mandatory = true)]
		[Parameter(ParameterSetName = "Custom", Mandatory = true)]
		[Parameter(ParameterSetName = "Client", Mandatory = true)]
		public MultiValuedProperty<IPRange> RemoteIPRanges
		{
			get
			{
				return this.DataObject.RemoteIPRanges;
			}
			set
			{
				this.DataObject.RemoteIPRanges = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EightBitMimeEnabled
		{
			get
			{
				return this.DataObject.EightBitMimeEnabled;
			}
			set
			{
				this.DataObject.EightBitMimeEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Banner
		{
			get
			{
				return this.DataObject.Banner;
			}
			set
			{
				this.DataObject.Banner = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BinaryMimeEnabled
		{
			get
			{
				return this.DataObject.BinaryMimeEnabled;
			}
			set
			{
				this.DataObject.BinaryMimeEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ChunkingEnabled
		{
			get
			{
				return this.DataObject.ChunkingEnabled;
			}
			set
			{
				this.DataObject.ChunkingEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DeliveryStatusNotificationEnabled
		{
			get
			{
				return this.DataObject.DeliveryStatusNotificationEnabled;
			}
			set
			{
				this.DataObject.DeliveryStatusNotificationEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EnhancedStatusCodesEnabled
		{
			get
			{
				return this.DataObject.EnhancedStatusCodesEnabled;
			}
			set
			{
				this.DataObject.EnhancedStatusCodesEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SizeMode SizeEnabled
		{
			get
			{
				return this.DataObject.SizeEnabled;
			}
			set
			{
				this.DataObject.SizeEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PipeliningEnabled
		{
			get
			{
				return this.DataObject.PipeliningEnabled;
			}
			set
			{
				this.DataObject.PipeliningEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TarpitInterval
		{
			get
			{
				return this.DataObject.TarpitInterval;
			}
			set
			{
				this.DataObject.TarpitInterval = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MaxAcknowledgementDelay
		{
			get
			{
				return this.DataObject.MaxAcknowledgementDelay;
			}
			set
			{
				this.DataObject.MaxAcknowledgementDelay = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
		public bool EnableAuthGSSAPI
		{
			get
			{
				return this.DataObject.EnableAuthGSSAPI;
			}
			set
			{
				this.DataObject.EnableAuthGSSAPI = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExtendedProtectionPolicySetting ExtendedProtectionPolicy
		{
			get
			{
				return this.DataObject.ExtendedProtectionPolicy;
			}
			set
			{
				this.DataObject.ExtendedProtectionPolicy = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LiveCredentialEnabled
		{
			get
			{
				return this.DataObject.LiveCredentialEnabled;
			}
			set
			{
				this.DataObject.LiveCredentialEnabled = value;
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
			}
		}

		[Parameter(Mandatory = false)]
		public bool LongAddressesEnabled
		{
			get
			{
				return this.DataObject.LongAddressesEnabled;
			}
			set
			{
				this.DataObject.LongAddressesEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OrarEnabled
		{
			get
			{
				return this.DataObject.OrarEnabled;
			}
			set
			{
				this.DataObject.OrarEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SuppressXAnonymousTls
		{
			get
			{
				return this.DataObject.SuppressXAnonymousTls;
			}
			set
			{
				this.DataObject.SuppressXAnonymousTls = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AdvertiseClientSettings
		{
			get
			{
				return this.DataObject.AdvertiseClientSettings;
			}
			set
			{
				this.DataObject.AdvertiseClientSettings = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ProxyEnabled
		{
			get
			{
				return this.DataObject.ProxyEnabled;
			}
			set
			{
				this.DataObject.ProxyEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmtpReceiveDomainCapabilities> TlsDomainCapabilities
		{
			get
			{
				return this.DataObject.TlsDomainCapabilities;
			}
			set
			{
				this.DataObject.TlsDomainCapabilities = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ServerRole TransportRole
		{
			get
			{
				return this.DataObject.TransportRole;
			}
			set
			{
				this.DataObject.TransportRole = value;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			ReceiveConnector receiveConnector = dataObject as ReceiveConnector;
			if (receiveConnector != null && !receiveConnector.IsReadOnly)
			{
				receiveConnector.PermissionGroups = this.DataObject.PermissionGroups;
			}
			receiveConnector.ResetChangeTracking();
			base.WriteResult(dataObject);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter(new object[]
			{
				base.GetType().FullName
			});
			this.CheckServerAndSetReceiveConnectorID();
			this.CheckParameters();
			this.InitializeDefaults();
			this.CalculateProperties();
			base.InternalValidate();
			if (this.AdvertiseClientSettings && (this.PermissionGroups & PermissionGroups.ExchangeUsers) != PermissionGroups.ExchangeUsers)
			{
				base.WriteError(new AdvertiseClientSettingsWithoutExchangeUsersPermissionGroupsException(), ErrorCategory.InvalidOperation, this.DataObject);
			}
			if (base.HasErrors)
			{
				return;
			}
			LocalizedException exception;
			if (!ReceiveConnectorNoMappingConflictCondition.Verify(this.DataObject, base.DataSession as IConfigurationSession, out exception))
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, this.DataObject);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			ReceiveConnector receiveConnector = null;
			try
			{
				receiveConnector = configurationSession.Read<ReceiveConnector>(this.DataObject.OriginalId);
				base.WriteVerbose(Strings.NewReceiveConnectorAddingPermissionsMsg);
				receiveConnector.PermissionGroups = this.DataObject.PermissionGroups;
				try
				{
					receiveConnector.SaveNewSecurityDescriptor(this.serverObject);
				}
				catch (OverflowException ex)
				{
					base.WriteDebug(ex.ToString());
					throw new ReceiveConnectorAclOverflowException(ex.Message);
				}
				base.WriteVerbose(Strings.NewReceiveConnectorAddingPermissionsDoneMsg);
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, receiveConnector);
			}
		}

		internal static bool ValidataName(string connectorName, out string exceptionString)
		{
			if (!string.IsNullOrEmpty(connectorName) && connectorName.Contains("\\"))
			{
				exceptionString = Strings.ErrorInvalidCharactersInParameterValue("Name", connectorName, string.Format("{0}{1}{2}", "{ '", "\\", "' }"));
				return false;
			}
			exceptionString = null;
			return true;
		}

		private void CheckServerAndSetReceiveConnectorID()
		{
			string message;
			if (!NewReceiveConnector.ValidataName(this.DataObject.Name, out message))
			{
				base.WriteError(new ArgumentException(message), ErrorCategory.InvalidArgument, null);
			}
			if (this.Server == null)
			{
				try
				{
					this.serverObject = ((ITopologyConfigurationSession)base.DataSession).FindLocalServer();
					goto IL_AD;
				}
				catch (LocalServerNotFoundException)
				{
					base.WriteError(new NeedToSpecifyServerObjectException(), ErrorCategory.InvalidOperation, this.DataObject);
					goto IL_AD;
				}
			}
			this.serverObject = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, this.RootId, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
			if (this.serverObject == null)
			{
				return;
			}
			IL_AD:
			this.isEdgeRole = this.serverObject.IsEdgeServer;
			if (!this.isEdgeRole && !this.serverObject.IsHubTransportServer && !this.serverObject.IsFrontendTransportServer && !this.serverObject.IsMailboxServer)
			{
				base.WriteError(new ServerNotHubOrEdgeException(), ErrorCategory.InvalidOperation, this.serverObject);
			}
			ADObjectId id = this.serverObject.Id;
			ADObjectId childId = id.GetChildId("Protocols").GetChildId("SMTP Receive Connectors").GetChildId(this.DataObject.Name);
			this.DataObject.SetId(childId);
		}

		private void CalculateProperties()
		{
			AcceptedDomainIdParameter defaultDomain = this.DefaultDomain;
			if (defaultDomain != null)
			{
				AcceptedDomain acceptedDomain = (AcceptedDomain)base.GetDataObject<AcceptedDomain>(defaultDomain, base.DataSession, this.RootId, new LocalizedString?(Strings.ErrorDefaultDomainNotFound(defaultDomain)), new LocalizedString?(Strings.ErrorDefaultDomainNotUnique(defaultDomain)));
				this.DataObject.DefaultDomain = acceptedDomain.Id;
			}
		}

		private void CheckParameters()
		{
			if (this.usage == NewReceiveConnector.UsageType.Internet && !this.DataObject.IsModified(ReceiveConnectorSchema.Bindings))
			{
				base.WriteError(new ParameterErrorForInternetUsageException(), ErrorCategory.InvalidOperation, this.DataObject);
			}
			else if (this.usage == NewReceiveConnector.UsageType.Internal && !this.DataObject.IsModified(ReceiveConnectorSchema.RemoteIPRanges))
			{
				base.WriteError(new ParameterErrorForInternalUsageException(), ErrorCategory.InvalidOperation, this.DataObject);
			}
			else if (this.usage == NewReceiveConnector.UsageType.Custom && (!this.DataObject.IsModified(ReceiveConnectorSchema.RemoteIPRanges) || !this.DataObject.IsModified(ReceiveConnectorSchema.Bindings)))
			{
				base.WriteError(new ParameterErrorForDefaultUsageException(), ErrorCategory.InvalidOperation, this.DataObject);
			}
			if (this.DataObject.IsModified(ReceiveConnectorSchema.PermissionGroups))
			{
				if (this.isEdgeRole && (this.DataObject.PermissionGroups & PermissionGroups.ExchangeLegacyServers) != PermissionGroups.None)
				{
					base.WriteError(new UnSupportedPermissionGroupsForEdgeException(), ErrorCategory.InvalidOperation, this.DataObject);
				}
				if ((this.DataObject.PermissionGroups & PermissionGroups.Custom) != PermissionGroups.None)
				{
					base.WriteError(new CustomCannotBeSetForPermissionGroupsException(), ErrorCategory.InvalidOperation, this.DataObject);
				}
			}
			if (this.DataObject.IsModified(ReceiveConnectorSchema.ChunkingEnabled) && !this.DataObject.ChunkingEnabled && this.DataObject.BinaryMimeEnabled)
			{
				base.WriteError(new ChunkingEnabledSettingConflictException(), ErrorCategory.InvalidOperation, this.DataObject);
			}
			if (this.DataObject.IsModified(ReceiveConnectorSchema.LongAddressesEnabled) && this.DataObject.LongAddressesEnabled && this.isEdgeRole)
			{
				base.WriteError(new LongAddressesEnabledOnEdgeException(), ErrorCategory.InvalidOperation, this.DataObject);
			}
			if (this.DataObject.IsModified(ReceiveConnectorSchema.SuppressXAnonymousTls))
			{
				if (this.DataObject.SuppressXAnonymousTls && this.serverObject.IsEdgeServer)
				{
					base.WriteError(new SuppressXAnonymousTlsEnabledOnEdgeException(), ErrorCategory.InvalidOperation, this.DataObject);
				}
				if (this.DataObject.SuppressXAnonymousTls && !this.serverObject.UseDowngradedExchangeServerAuth)
				{
					base.WriteError(new SuppressXAnonymousTlsEnabledWithoutDowngradedExchangeAuthException(), ErrorCategory.InvalidOperation, this.DataObject);
				}
			}
			if (this.DataObject.IsModified(ReceiveConnectorSchema.TransportRole) && (this.DataObject.TransportRole & (ServerRole.HubTransport | ServerRole.Edge | ServerRole.FrontendTransport)) == ServerRole.None)
			{
				base.WriteError(new InvalidTransportRoleOnReceiveConnectorException(), ErrorCategory.InvalidData, this.DataObject);
			}
		}

		private void InitializeDefaults()
		{
			if (!this.DataObject.IsModified(ReceiveConnectorSchema.MaxInboundConnection))
			{
				this.MaxInboundConnection = 5000;
			}
			if (!this.DataObject.IsModified(ReceiveConnectorSchema.MaxInboundConnectionPerSource))
			{
				this.MaxInboundConnectionPerSource = 20;
			}
			if (!this.DataObject.IsModified(ReceiveConnectorSchema.MaxProtocolErrors))
			{
				this.MaxProtocolErrors = 5;
			}
			if (!this.DataObject.IsModified(ReceiveConnectorSchema.Fqdn))
			{
				string fqdn = this.serverObject.Fqdn;
				SmtpDomain smtpDomain;
				if (SmtpDomain.TryParse(fqdn, out smtpDomain))
				{
					this.Fqdn = new Fqdn(fqdn);
				}
				else if (SmtpDomain.TryParse(this.serverObject.Name, out smtpDomain))
				{
					this.Fqdn = new Fqdn(this.serverObject.Name);
				}
				else
				{
					base.WriteError(new InvalidFqdnException(), ErrorCategory.InvalidOperation, this.DataObject);
				}
			}
			if (!this.DataObject.IsModified(ReceiveConnectorSchema.TransportRole))
			{
				if ((this.serverObject.CurrentServerRole & ServerRole.Edge) != ServerRole.None)
				{
					this.TransportRole = ServerRole.HubTransport;
				}
				else if ((this.serverObject.CurrentServerRole & ServerRole.HubTransport) != ServerRole.None)
				{
					this.TransportRole = ServerRole.HubTransport;
				}
				else if ((this.serverObject.CurrentServerRole & ServerRole.FrontendTransport) != ServerRole.None)
				{
					this.TransportRole = ServerRole.FrontendTransport;
				}
				else
				{
					this.TransportRole = ServerRole.HubTransport;
				}
			}
			if (this.isEdgeRole)
			{
				if (!this.DataObject.IsModified(ReceiveConnectorSchema.ConnectionTimeout))
				{
					this.DataObject.ConnectionTimeout = EnhancedTimeSpan.FromMinutes(5.0);
				}
				if (!this.DataObject.IsModified(ReceiveConnectorSchema.ConnectionInactivityTimeout))
				{
					this.DataObject.ConnectionInactivityTimeout = EnhancedTimeSpan.OneMinute;
				}
				if (!this.DataObject.IsModified(ReceiveConnectorSchema.MessageRateLimit))
				{
					this.DataObject.MessageRateLimit = 600;
				}
				if (!this.DataObject.IsModified(ReceiveConnectorSchema.MessageRateSource))
				{
					this.DataObject.MessageRateSource = MessageRateSourceFlags.IPAddress;
				}
			}
			if (!this.DataObject.IsModified(ReceiveConnectorSchema.PermissionGroups))
			{
				this.SetPermissionGroups();
			}
			if (!this.DataObject.IsModified(ReceiveConnectorSchema.SecurityFlags))
			{
				this.SetAuthMechanism();
			}
			if (this.isUsageSet && this.usage == NewReceiveConnector.UsageType.Internal)
			{
				this.SetUsageInternalProperties();
			}
			if (!this.DataObject.IsModified(ReceiveConnectorSchema.Bindings) && this.usage == NewReceiveConnector.UsageType.Client)
			{
				this.DataObject.Bindings[0].Port = 587;
			}
			LocalizedException exception;
			if (!this.isEdgeRole && (this.AuthMechanism & AuthMechanisms.ExchangeServer) != AuthMechanisms.None && !ReceiveConnectorFqdnCondition.Verify(this.DataObject, this.serverObject, out exception))
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, this.DataObject);
			}
		}

		private void SetPermissionGroups()
		{
			if (this.isUsageSet)
			{
				switch (this.usage)
				{
				case NewReceiveConnector.UsageType.Custom:
					this.DataObject.PermissionGroups = PermissionGroups.None;
					return;
				case NewReceiveConnector.UsageType.Internet:
					this.DataObject.PermissionGroups = PermissionGroups.AnonymousUsers;
					return;
				case NewReceiveConnector.UsageType.Internal:
					this.DataObject.PermissionGroups = (PermissionGroups.ExchangeServers | PermissionGroups.ExchangeLegacyServers);
					return;
				case NewReceiveConnector.UsageType.Client:
					this.DataObject.PermissionGroups = PermissionGroups.ExchangeUsers;
					return;
				case NewReceiveConnector.UsageType.Partner:
					this.DataObject.PermissionGroups = PermissionGroups.Partners;
					return;
				default:
					return;
				}
			}
			else
			{
				if (this.isEdgeRole)
				{
					this.DataObject.PermissionGroups = (PermissionGroups.AnonymousUsers | PermissionGroups.ExchangeServers | PermissionGroups.Partners);
					return;
				}
				this.DataObject.PermissionGroups = (PermissionGroups.ExchangeUsers | PermissionGroups.ExchangeServers | PermissionGroups.ExchangeLegacyServers);
				return;
			}
		}

		private void SetAuthMechanism()
		{
			if (this.isUsageSet)
			{
				switch (this.usage)
				{
				case NewReceiveConnector.UsageType.Custom:
					this.DataObject.AuthMechanism = AuthMechanisms.Tls;
					return;
				case NewReceiveConnector.UsageType.Internet:
					this.DataObject.AuthMechanism = AuthMechanisms.Tls;
					return;
				case NewReceiveConnector.UsageType.Internal:
					this.DataObject.AuthMechanism = (AuthMechanisms.Tls | AuthMechanisms.ExchangeServer);
					return;
				case NewReceiveConnector.UsageType.Client:
					this.DataObject.AuthMechanism = (AuthMechanisms.Tls | AuthMechanisms.Integrated | AuthMechanisms.BasicAuth | AuthMechanisms.BasicAuthRequireTLS);
					return;
				case NewReceiveConnector.UsageType.Partner:
					this.DataObject.AuthMechanism = AuthMechanisms.Tls;
					this.DataObject.DomainSecureEnabled = true;
					return;
				default:
					return;
				}
			}
			else
			{
				if (this.isEdgeRole)
				{
					this.DataObject.AuthMechanism = (AuthMechanisms.Tls | AuthMechanisms.ExchangeServer);
					return;
				}
				this.DataObject.AuthMechanism = (AuthMechanisms.Tls | AuthMechanisms.Integrated | AuthMechanisms.BasicAuth | AuthMechanisms.BasicAuthRequireTLS | AuthMechanisms.ExchangeServer);
				return;
			}
		}

		private void SetUsageInternalProperties()
		{
			if (!this.DataObject.IsModified(ReceiveConnectorSchema.SizeEnabled) && !this.isEdgeRole)
			{
				this.DataObject.SizeEnabled = SizeMode.EnabledWithoutValue;
			}
			if (!this.DataObject.IsModified(ReceiveConnectorSchema.MaxInboundConnectionPercentagePerSource) && !this.isEdgeRole)
			{
				this.MaxInboundConnectionPercentagePerSource = 100;
			}
		}

		private const string CustomParameterSetName = "Custom";

		private const string InternetParameterSetName = "Internet";

		private const string InternalParameterSetName = "Internal";

		private const string ClientParameterSetName = "Client";

		private const string PartnerParameterSetName = "Partner";

		private const string UsageTypeParameterSetName = "UsageType";

		private const string CommonNameSeperatorChar = "\\";

		private const string protocolsContainer = "Protocols";

		private const string smtpContainer = "SMTP Receive Connectors";

		private NewReceiveConnector.UsageType usage;

		private bool isUsageSet;

		private bool isEdgeRole = true;

		private Server serverObject;

		public enum UsageType
		{
			[LocDescription(Strings.IDs.UsageTypeCustom)]
			Custom,
			[LocDescription(Strings.IDs.UsageTypeInternet)]
			Internet,
			[LocDescription(Strings.IDs.UsageTypeInternal)]
			Internal,
			[LocDescription(Strings.IDs.UsageTypeClient)]
			Client,
			[LocDescription(Strings.IDs.UsageTypePartner)]
			Partner
		}
	}
}
