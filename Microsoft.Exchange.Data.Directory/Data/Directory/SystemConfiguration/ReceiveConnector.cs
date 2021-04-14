using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public class ReceiveConnector : ADConfigurationObject
	{
		public ReceiveConnector()
		{
			this.Bindings = new MultiValuedProperty<IPBinding>(false, ReceiveConnectorSchema.Bindings, new IPBinding[]
			{
				IPBinding.Parse("0.0.0.0:25")
			});
			this.RemoteIPRanges = new MultiValuedProperty<IPRange>(false, ReceiveConnectorSchema.RemoteIPRanges, new IPRange[]
			{
				IPRange.Parse("0.0.0.0-255.255.255.255")
			});
			base.ResetChangeTracking();
		}

		public ReceiveConnector(string name, ADObjectId connectorCollectionId) : this()
		{
			base.SetId(connectorCollectionId.GetChildId(name));
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ReceiveConnector.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchSmtpReceiveConnector";
			}
		}

		[Parameter(Mandatory = false)]
		public AuthMechanisms AuthMechanism
		{
			get
			{
				return (AuthMechanisms)this[ReceiveConnectorSchema.SecurityFlags];
			}
			set
			{
				this[ReceiveConnectorSchema.SecurityFlags] = (int)value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Banner
		{
			get
			{
				return (string)this[ReceiveConnectorSchema.Banner];
			}
			set
			{
				this[ReceiveConnectorSchema.Banner] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BinaryMimeEnabled
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.BinaryMimeEnabled];
			}
			set
			{
				this[ReceiveConnectorSchema.BinaryMimeEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPBinding> Bindings
		{
			get
			{
				return (MultiValuedProperty<IPBinding>)this[ReceiveConnectorSchema.Bindings];
			}
			set
			{
				this[ReceiveConnectorSchema.Bindings] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ChunkingEnabled
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.ChunkingEnabled];
			}
			set
			{
				this[ReceiveConnectorSchema.ChunkingEnabled] = value;
			}
		}

		public ADObjectId DefaultDomain
		{
			get
			{
				return (ADObjectId)this[ReceiveConnectorSchema.DefaultDomain];
			}
			set
			{
				this[ReceiveConnectorSchema.DefaultDomain] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DeliveryStatusNotificationEnabled
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.DeliveryStatusNotificationEnabled];
			}
			set
			{
				this[ReceiveConnectorSchema.DeliveryStatusNotificationEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EightBitMimeEnabled
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.EightBitMimeEnabled];
			}
			set
			{
				this[ReceiveConnectorSchema.EightBitMimeEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SmtpUtf8Enabled
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.SmtpUtf8Enabled];
			}
			set
			{
				this[ReceiveConnectorSchema.SmtpUtf8Enabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BareLinefeedRejectionEnabled
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.BareLinefeedRejectionEnabled];
			}
			set
			{
				this[ReceiveConnectorSchema.BareLinefeedRejectionEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DomainSecureEnabled
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.DomainSecureEnabled];
			}
			set
			{
				this[ReceiveConnectorSchema.DomainSecureEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EnhancedStatusCodesEnabled
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.EnhancedStatusCodesEnabled];
			}
			set
			{
				this[ReceiveConnectorSchema.EnhancedStatusCodesEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LongAddressesEnabled
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.LongAddressesEnabled];
			}
			set
			{
				this[ReceiveConnectorSchema.LongAddressesEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OrarEnabled
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.OrarEnabled];
			}
			set
			{
				this[ReceiveConnectorSchema.OrarEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SuppressXAnonymousTls
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.SuppressXAnonymousTls];
			}
			set
			{
				this[ReceiveConnectorSchema.SuppressXAnonymousTls] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ProxyEnabled
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.ProxyEnabled];
			}
			set
			{
				this[ReceiveConnectorSchema.ProxyEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AdvertiseClientSettings
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.AdvertiseClientSettings];
			}
			set
			{
				this[ReceiveConnectorSchema.AdvertiseClientSettings] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Fqdn Fqdn
		{
			get
			{
				return (Fqdn)this[ReceiveConnectorSchema.Fqdn];
			}
			set
			{
				this[ReceiveConnectorSchema.Fqdn] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Fqdn ServiceDiscoveryFqdn
		{
			get
			{
				return (Fqdn)this[ReceiveConnectorSchema.ServiceDiscoveryFqdn];
			}
			set
			{
				this[ReceiveConnectorSchema.ServiceDiscoveryFqdn] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpX509Identifier TlsCertificateName
		{
			get
			{
				return (SmtpX509Identifier)this[ReceiveConnectorSchema.TlsCertificateName];
			}
			set
			{
				this[ReceiveConnectorSchema.TlsCertificateName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Comment
		{
			get
			{
				return (string)this[ReceiveConnectorSchema.Comment];
			}
			set
			{
				this[ReceiveConnectorSchema.Comment] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.Enabled];
			}
			set
			{
				this[ReceiveConnectorSchema.Enabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ConnectionTimeout
		{
			get
			{
				return (EnhancedTimeSpan)this[ReceiveConnectorSchema.ConnectionTimeout];
			}
			set
			{
				this[ReceiveConnectorSchema.ConnectionTimeout] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ConnectionInactivityTimeout
		{
			get
			{
				return (EnhancedTimeSpan)this[ReceiveConnectorSchema.ConnectionInactivityTimeout];
			}
			set
			{
				this[ReceiveConnectorSchema.ConnectionInactivityTimeout] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MessageRateLimit
		{
			get
			{
				return (Unlimited<int>)this[ReceiveConnectorSchema.MessageRateLimit];
			}
			set
			{
				this[ReceiveConnectorSchema.MessageRateLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MessageRateSourceFlags MessageRateSource
		{
			get
			{
				return (MessageRateSourceFlags)this[ReceiveConnectorSchema.MessageRateSource];
			}
			set
			{
				this[ReceiveConnectorSchema.MessageRateSource] = (int)value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxInboundConnection
		{
			get
			{
				return (Unlimited<int>)this[ReceiveConnectorSchema.MaxInboundConnection];
			}
			set
			{
				this[ReceiveConnectorSchema.MaxInboundConnection] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxInboundConnectionPerSource
		{
			get
			{
				return (Unlimited<int>)this[ReceiveConnectorSchema.MaxInboundConnectionPerSource];
			}
			set
			{
				this[ReceiveConnectorSchema.MaxInboundConnectionPerSource] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxInboundConnectionPercentagePerSource
		{
			get
			{
				return (int)this[ReceiveConnectorSchema.MaxInboundConnectionPercentagePerSource];
			}
			set
			{
				this[ReceiveConnectorSchema.MaxInboundConnectionPercentagePerSource] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize MaxHeaderSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ReceiveConnectorSchema.MaxHeaderSize];
			}
			set
			{
				this[ReceiveConnectorSchema.MaxHeaderSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxHopCount
		{
			get
			{
				return (int)this[ReceiveConnectorSchema.MaxHopCount];
			}
			set
			{
				this[ReceiveConnectorSchema.MaxHopCount] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxLocalHopCount
		{
			get
			{
				return (int)this[ReceiveConnectorSchema.MaxLocalHopCount];
			}
			set
			{
				this[ReceiveConnectorSchema.MaxLocalHopCount] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxLogonFailures
		{
			get
			{
				return (int)this[ReceiveConnectorSchema.MaxLogonFailures];
			}
			set
			{
				this[ReceiveConnectorSchema.MaxLogonFailures] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize MaxMessageSize
		{
			get
			{
				return (ByteQuantifiedSize)this[ReceiveConnectorSchema.MaxMessageSize];
			}
			set
			{
				this[ReceiveConnectorSchema.MaxMessageSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> MaxProtocolErrors
		{
			get
			{
				return (Unlimited<int>)this[ReceiveConnectorSchema.MaxProtocolErrors];
			}
			set
			{
				this[ReceiveConnectorSchema.MaxProtocolErrors] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxRecipientsPerMessage
		{
			get
			{
				return (int)this[ReceiveConnectorSchema.MaxRecipientsPerMessage];
			}
			set
			{
				this[ReceiveConnectorSchema.MaxRecipientsPerMessage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PermissionGroups PermissionGroups
		{
			get
			{
				return (PermissionGroups)this[ReceiveConnectorSchema.PermissionGroups];
			}
			set
			{
				this[ReceiveConnectorSchema.PermissionGroups] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PipeliningEnabled
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.PipeliningEnabled];
			}
			set
			{
				this[ReceiveConnectorSchema.PipeliningEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolLoggingLevel ProtocolLoggingLevel
		{
			get
			{
				return (ProtocolLoggingLevel)this[ReceiveConnectorSchema.ProtocolLoggingLevel];
			}
			set
			{
				this[ReceiveConnectorSchema.ProtocolLoggingLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPRange> RemoteIPRanges
		{
			get
			{
				return (MultiValuedProperty<IPRange>)this[ReceiveConnectorSchema.RemoteIPRanges];
			}
			set
			{
				this[ReceiveConnectorSchema.RemoteIPRanges] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireEHLODomain
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.RequireEHLODomain];
			}
			set
			{
				this[ReceiveConnectorSchema.RequireEHLODomain] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireTLS
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.RequireTLS];
			}
			set
			{
				this[ReceiveConnectorSchema.RequireTLS] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool EnableAuthGSSAPI
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.EnableAuthGSSAPI];
			}
			set
			{
				this[ReceiveConnectorSchema.EnableAuthGSSAPI] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExtendedProtectionPolicySetting ExtendedProtectionPolicy
		{
			get
			{
				return (ExtendedProtectionPolicySetting)this[ReceiveConnectorSchema.ExtendedProtectionPolicy];
			}
			set
			{
				this[ReceiveConnectorSchema.ExtendedProtectionPolicy] = (int)value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool LiveCredentialEnabled
		{
			get
			{
				return (bool)this[ReceiveConnectorSchema.LiveCredentialEnabled];
			}
			set
			{
				this[ReceiveConnectorSchema.LiveCredentialEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmtpReceiveDomainCapabilities> TlsDomainCapabilities
		{
			get
			{
				return (MultiValuedProperty<SmtpReceiveDomainCapabilities>)this[ReceiveConnectorSchema.TlsDomainCapabilities];
			}
			set
			{
				this[ReceiveConnectorSchema.TlsDomainCapabilities] = value;
			}
		}

		public ADObjectId Server
		{
			get
			{
				return (ADObjectId)this[ReceiveConnectorSchema.Server];
			}
		}

		[Parameter(Mandatory = false)]
		public ServerRole TransportRole
		{
			get
			{
				return (ServerRole)this[ReceiveConnectorSchema.TransportRole];
			}
			set
			{
				this[ReceiveConnectorSchema.TransportRole] = (int)value;
			}
		}

		internal bool HasNoAuthMechanisms
		{
			get
			{
				return this.AuthMechanism == AuthMechanisms.None;
			}
		}

		internal bool HasTlsAuthMechanism
		{
			get
			{
				return (this.AuthMechanism & AuthMechanisms.Tls) != AuthMechanisms.None;
			}
		}

		internal bool HasIntegratedAuthMechanism
		{
			get
			{
				return (this.AuthMechanism & AuthMechanisms.Integrated) != AuthMechanisms.None;
			}
		}

		internal bool HasBasicAuthAuthMechanism
		{
			get
			{
				return (this.AuthMechanism & AuthMechanisms.BasicAuth) != AuthMechanisms.None;
			}
		}

		internal bool HasBasicAuthRequireTlsAuthMechanism
		{
			get
			{
				return (this.AuthMechanism & AuthMechanisms.BasicAuthRequireTLS) != AuthMechanisms.None;
			}
		}

		internal bool HasExchangeServerAuthMechanism
		{
			get
			{
				return (this.AuthMechanism & AuthMechanisms.ExchangeServer) != AuthMechanisms.None;
			}
		}

		internal bool HasExternalAuthoritativeAuthMechanism
		{
			get
			{
				return (this.AuthMechanism & AuthMechanisms.ExternalAuthoritative) != AuthMechanisms.None;
			}
		}

		internal void SetPermissionGroupsBasedOnSecurityDescriptor(Server server)
		{
			RawSecurityDescriptor rsd = base.Session.ReadSecurityDescriptor(base.Id);
			ReceiveConnector.PermissionGroupInfo[] permissionGroupInfos = ReceiveConnector.PermissionGroupPermissions.GetPermissionGroupInfos(server);
			this.PermissionGroups = ReceiveConnector.GetPermissionGroups(rsd, permissionGroupInfos, server);
		}

		internal static object ServerGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
				if (adobjectId == null)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Server", string.Empty), ReceiveConnectorSchema.Server, propertyBag[ADObjectSchema.Id]), null);
				}
				result = adobjectId.DescendantDN(8);
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Server", ex.Message), ReceiveConnectorSchema.Server, propertyBag[ADObjectSchema.Id]), ex);
			}
			return result;
		}

		internal void SaveNewSecurityDescriptor(Server server)
		{
			if (!base.IsModified(ReceiveConnectorSchema.PermissionGroups))
			{
				return;
			}
			ReceiveConnector.PermissionGroupInfo[] permissionGroupInfos = ReceiveConnector.PermissionGroupPermissions.GetPermissionGroupInfos(server);
			PrincipalPermissionList principalPermissionList = new PrincipalPermissionList();
			PrincipalPermissionList principalPermissionList2 = new PrincipalPermissionList();
			foreach (ReceiveConnector.PermissionGroupInfo permissionGroupInfo in permissionGroupInfos)
			{
				for (int j = 0; j < permissionGroupInfo.Sids.Length; j++)
				{
					principalPermissionList2.Add(permissionGroupInfo.Sids[j], permissionGroupInfo.DefaultPermission[j]);
				}
				if ((this.PermissionGroups & permissionGroupInfo.PermissionGroup) != PermissionGroups.None)
				{
					for (int k = 0; k < permissionGroupInfo.Sids.Length; k++)
					{
						principalPermissionList.Add(permissionGroupInfo.Sids[k], permissionGroupInfo.DefaultPermission[k]);
						if (permissionGroupInfo.PermissionGroup == PermissionGroups.ExchangeLegacyServers)
						{
							principalPermissionList.AddDeny(permissionGroupInfo.Sids[k], Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders);
						}
					}
				}
			}
			RawSecurityDescriptor rawSecurityDescriptor = base.ReadSecurityDescriptor();
			if (rawSecurityDescriptor != null)
			{
				rawSecurityDescriptor = principalPermissionList2.RemoveExtendedRightsFromSecurityDescriptor(rawSecurityDescriptor);
				rawSecurityDescriptor = principalPermissionList.AddExtendedRightsToSecurityDescriptor(rawSecurityDescriptor);
				base.SaveSecurityDescriptor(rawSecurityDescriptor);
				return;
			}
			if (principalPermissionList.Count != 0)
			{
				SecurityIdentifier principal = principalPermissionList[0].Principal;
				SecurityIdentifier group = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
				rawSecurityDescriptor = principalPermissionList.CreateExtendedRightsSecurityDescriptor(principal, group);
				rawSecurityDescriptor = principalPermissionList.AddExtendedRightsToSecurityDescriptor(rawSecurityDescriptor);
				base.SaveSecurityDescriptor(rawSecurityDescriptor);
			}
		}

		internal virtual RawSecurityDescriptor GetSecurityDescriptor()
		{
			if (this.securityDescriptor == null)
			{
				this.securityDescriptor = base.ReadSecurityDescriptor();
			}
			return this.securityDescriptor;
		}

		private static PermissionGroups GetPermissionGroups(RawSecurityDescriptor rsd, ReceiveConnector.PermissionGroupInfo[] permissionGroupInfos, Server server)
		{
			PermissionGroups permissionGroups = PermissionGroups.None;
			if (rsd == null)
			{
				return permissionGroups;
			}
			Dictionary<SecurityIdentifier, bool> dictionary = new Dictionary<SecurityIdentifier, bool>();
			Permission permission = Permission.SMTPSubmitForMLS | Permission.SMTPSendEXCH50 | Permission.SendRoutingHeaders | Permission.SendForestHeaders | Permission.SendOrganizationHeaders | Permission.SendAs | Permission.SMTPSendXShadow;
			if (!server.IsE14OrLater)
			{
				permission |= Permission.SMTPAcceptXShadow;
			}
			Permission permission2 = ~permission;
			foreach (ReceiveConnector.PermissionGroupInfo permissionGroupInfo in permissionGroupInfos)
			{
				bool flag = true;
				for (int j = 0; j < permissionGroupInfo.Sids.Length; j++)
				{
					Permission permission3;
					try
					{
						permission3 = AuthzAuthorization.CheckPermissions(permissionGroupInfo.Sids[j], rsd, null);
					}
					catch (Win32Exception ex)
					{
						throw new LocalizedException(DirectoryStrings.ExceptionWin32OperationFailed(ex.NativeErrorCode, ex.Message), ex);
					}
					permission3 &= permission2;
					Permission permission4 = permissionGroupInfo.DefaultPermission[j];
					if (!server.IsE14OrLater)
					{
						permission4 &= ~Permission.SMTPAcceptXShadow;
					}
					if ((permission3 & permission4) != permission4)
					{
						flag = false;
					}
					if (permission3 != Permission.None && permission3 != permission4)
					{
						permissionGroups |= PermissionGroups.Custom;
					}
					dictionary.Add(permissionGroupInfo.Sids[j], true);
				}
				if (flag)
				{
					permissionGroups |= permissionGroupInfo.PermissionGroup;
				}
			}
			if ((permissionGroups & PermissionGroups.Custom) != PermissionGroups.Custom)
			{
				ActiveDirectorySecurity activeDirectorySecurity = new ActiveDirectorySecurity();
				activeDirectorySecurity.SetSecurityDescriptorSddlForm(rsd.GetSddlForm(AccessControlSections.All));
				foreach (object obj in activeDirectorySecurity.GetAccessRules(true, true, typeof(SecurityIdentifier)))
				{
					AuthorizationRule authorizationRule = (AuthorizationRule)obj;
					if (!authorizationRule.IsInherited && authorizationRule is AccessRule)
					{
						SecurityIdentifier securityIdentifier = authorizationRule.IdentityReference as SecurityIdentifier;
						if (securityIdentifier != null && !dictionary.ContainsKey(securityIdentifier))
						{
							Permission permission5 = AuthzAuthorization.CheckPermissions(securityIdentifier, rsd, null);
							if (permission5 != Permission.None && !ReceiveConnector.IsPredefinedPermissionGroup(permission5))
							{
								permissionGroups |= PermissionGroups.Custom;
								break;
							}
							dictionary.Add(securityIdentifier, true);
						}
					}
				}
			}
			return permissionGroups;
		}

		private static bool IsPredefinedPermissionGroup(Permission permission)
		{
			return (Permission.SMTPSubmit | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.AcceptRoutingHeaders) == permission || (Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders | Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders | Permission.SMTPAcceptXShadow | Permission.SMTPAcceptXProxyFrom | Permission.SMTPAcceptXSessionParams | Permission.SMTPAcceptXMessageContextADRecipientCache | Permission.SMTPAcceptXMessageContextExtendedProperties | Permission.SMTPAcceptXMessageContextFastIndex | Permission.SMTPAcceptXAttr | Permission.SMTPAcceptXSysProbe) == permission || (Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders | Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders) == permission || (Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders | Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders | Permission.SMTPAcceptXShadow) == permission || (Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.BypassAntiSpam | Permission.AcceptRoutingHeaders) == permission || (Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders) == permission || (Permission.SMTPSubmit | Permission.AcceptRoutingHeaders) == permission;
		}

		[Parameter(Mandatory = false)]
		public SizeMode SizeEnabled
		{
			get
			{
				return (SizeMode)this[ReceiveConnectorSchema.SizeEnabled];
			}
			set
			{
				this[ReceiveConnectorSchema.SizeEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TarpitInterval
		{
			get
			{
				return (EnhancedTimeSpan)this[ReceiveConnectorSchema.TarpitInterval];
			}
			set
			{
				this[ReceiveConnectorSchema.TarpitInterval] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MaxAcknowledgementDelay
		{
			get
			{
				return (EnhancedTimeSpan)this[ReceiveConnectorSchema.MaxAcknowledgementDelay];
			}
			set
			{
				this[ReceiveConnectorSchema.MaxAcknowledgementDelay] = value;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			foreach (IPBinding ipbinding in this.Bindings)
			{
				IPvxAddress pvxAddress = new IPvxAddress(ipbinding.Address);
				if (pvxAddress.IsMulticast || pvxAddress.IsBroadcast)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidBindingAddressSetting, ReceiveConnectorSchema.Bindings, this));
					break;
				}
			}
			if (!string.IsNullOrEmpty(this.Banner))
			{
				SmtpResponse smtpResponse;
				if (!SmtpResponse.TryParse(this.Banner, out smtpResponse) || !string.Equals("220", smtpResponse.StatusCode, StringComparison.Ordinal))
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.InvalidBannerSetting, ReceiveConnectorSchema.Banner, this));
				}
				else if (smtpResponse.StatusText != null)
				{
					bool flag = smtpResponse.StatusText.Length <= 50;
					if (flag)
					{
						foreach (string text in smtpResponse.StatusText)
						{
							if (text.Length > 2000)
							{
								flag = false;
								break;
							}
						}
					}
					if (!flag)
					{
						errors.Add(new PropertyValidationError(DirectoryStrings.InvalidBannerSetting, ReceiveConnectorSchema.Banner, this));
					}
				}
			}
			bool flag2 = (this.AuthMechanism & AuthMechanisms.ExternalAuthoritative) != AuthMechanisms.None;
			bool flag3 = (this.AuthMechanism & AuthMechanisms.BasicAuth) != AuthMechanisms.None;
			bool flag4 = (this.AuthMechanism & AuthMechanisms.BasicAuthRequireTLS) != AuthMechanisms.None;
			bool flag5 = (this.AuthMechanism & AuthMechanisms.ExchangeServer) != AuthMechanisms.None;
			bool flag6 = (this.AuthMechanism & AuthMechanisms.Integrated) != AuthMechanisms.None;
			bool flag7 = (this.AuthMechanism & AuthMechanisms.Tls) != AuthMechanisms.None;
			if (flag4 && !flag7)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.BasicAfterTLSWithoutTLS, ReceiveConnectorSchema.SecurityFlags, this));
			}
			if (flag4 && !flag3)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.BasicAfterTLSWithoutBasic, ReceiveConnectorSchema.SecurityFlags, this));
			}
			if (this.RequireTLS && !flag7)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.RequireTLSWithoutTLS, ReceiveConnectorSchema.SecurityFlags, this));
			}
			if (flag2 && (flag3 || flag4 || flag5 || flag6))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExternalAndAuthSet, ReceiveConnectorSchema.SecurityFlags, this));
			}
			if (this.LiveCredentialEnabled && !flag3 && !flag4)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.LiveCredentialWithoutBasic, ReceiveConnectorSchema.SecurityFlags, this));
			}
			if (EnhancedTimeSpan.Compare(this.ConnectionInactivityTimeout, this.ConnectionTimeout) > 0)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ConnectionTimeoutLessThanInactivityTimeout, ReceiveConnectorSchema.ConnectionTimeout, this));
			}
			if (this.DomainSecureEnabled)
			{
				if (!flag7)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.DomainSecureEnabledWithoutTls, ReceiveConnectorSchema.DomainSecureEnabled, this));
				}
				if (flag2)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.DomainSecureEnabledWithExternalAuthoritative, ReceiveConnectorSchema.DomainSecureEnabled, this));
				}
			}
			if (flag2 && (this.PermissionGroups & PermissionGroups.ExchangeServers) == PermissionGroups.None)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExternalAuthoritativeWithoutExchangeServerPermission, ReceiveConnectorSchema.DomainSecureEnabled, this));
			}
			if (this.ExtendedProtectionPolicy != ExtendedProtectionPolicySetting.None && !this.RequireTLS)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ExtendedProtectionNonTlsTerminatingProxyScenarioRequireTls, ReceiveConnectorSchema.ExtendedProtectionPolicy, this));
			}
			this.ValidateTlsDomainCapabilities(errors);
		}

		private void ValidateTlsDomainCapabilities(List<ValidationError> errors)
		{
			HashSet<SmtpDomainWithSubdomains> hashSet = new HashSet<SmtpDomainWithSubdomains>();
			foreach (SmtpReceiveDomainCapabilities smtpReceiveDomainCapabilities in this.TlsDomainCapabilities)
			{
				if (smtpReceiveDomainCapabilities.Domain.SmtpDomain == null)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.StarTlsDomainCapabilitiesNotAllowed, ReceiveConnectorSchema.TlsDomainCapabilities, this));
				}
				else if (!hashSet.Add(smtpReceiveDomainCapabilities.Domain))
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.DuplicateTlsDomainCapabilitiesNotAllowed(smtpReceiveDomainCapabilities.Domain), ReceiveConnectorSchema.TlsDomainCapabilities, this));
				}
			}
		}

		public const PermissionGroups UnsupportedEdgePermissionGroups = PermissionGroups.ExchangeLegacyServers;

		public const string MostDerivedClass = "msExchSmtpReceiveConnector";

		internal static readonly string DefaultName = "SMTP Receive Connectors";

		private static ReceiveConnectorSchema schema = ObjectSchema.GetInstance<ReceiveConnectorSchema>();

		[NonSerialized]
		private RawSecurityDescriptor securityDescriptor;

		internal static class PermissionGroupPermissions
		{
			internal static ReceiveConnector.PermissionGroupInfo[] GetPermissionGroupInfos(Server server)
			{
				ReceiveConnector.PermissionGroupInfo[] array;
				if (server.IsEdgeServer)
				{
					array = new ReceiveConnector.PermissionGroupInfo[4];
				}
				else
				{
					array = new ReceiveConnector.PermissionGroupInfo[5];
				}
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1215, "GetPermissionGroupInfos", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ReceiveConnector.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 1220, "GetPermissionGroupInfos", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ReceiveConnector.cs");
				array[0] = new ReceiveConnector.PermissionGroupInfo(PermissionGroups.AnonymousUsers, new SecurityIdentifier(WellKnownSidType.AnonymousSid, null), Permission.SMTPSubmit | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.AcceptRoutingHeaders);
				array[1] = new ReceiveConnector.PermissionGroupInfo(PermissionGroups.ExchangeUsers, new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null), Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.BypassAntiSpam | Permission.AcceptRoutingHeaders);
				SecurityIdentifier[] array2;
				Permission[] array3;
				if (server.IsEdgeServer)
				{
					array2 = new SecurityIdentifier[3];
					array3 = new Permission[3];
				}
				else
				{
					array2 = new SecurityIdentifier[4];
					array3 = new Permission[4];
					array2[3] = ReceiveConnector.PermissionGroupPermissions.GetSidForExchangeKnownGuid(tenantOrRootOrgRecipientSession, WellKnownGuid.ExSWkGuid, tenantOrTopologyConfigurationSession.ConfigurationNamingContext.DistinguishedName);
					array3[3] = (Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders | Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders | Permission.SMTPAcceptXShadow | Permission.SMTPAcceptXProxyFrom | Permission.SMTPAcceptXSessionParams | Permission.SMTPAcceptXMessageContextADRecipientCache | Permission.SMTPAcceptXMessageContextExtendedProperties | Permission.SMTPAcceptXMessageContextFastIndex | Permission.SMTPAcceptXAttr | Permission.SMTPAcceptXSysProbe);
				}
				Permission permission;
				if (server.IsE15OrLater)
				{
					permission = (Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders | Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders | Permission.SMTPAcceptXShadow | Permission.SMTPAcceptXProxyFrom | Permission.SMTPAcceptXSessionParams | Permission.SMTPAcceptXMessageContextADRecipientCache | Permission.SMTPAcceptXMessageContextExtendedProperties | Permission.SMTPAcceptXMessageContextFastIndex | Permission.SMTPAcceptXAttr | Permission.SMTPAcceptXSysProbe);
				}
				else if (server.IsE14OrLater)
				{
					permission = (Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders | Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders | Permission.SMTPAcceptXShadow);
				}
				else if (server.IsExchange2007OrLater)
				{
					permission = (Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders | Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders);
				}
				else
				{
					permission = (Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders | Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders | Permission.SMTPAcceptXShadow | Permission.SMTPAcceptXProxyFrom | Permission.SMTPAcceptXSessionParams | Permission.SMTPAcceptXMessageContextADRecipientCache | Permission.SMTPAcceptXMessageContextExtendedProperties | Permission.SMTPAcceptXMessageContextFastIndex | Permission.SMTPAcceptXAttr | Permission.SMTPAcceptXSysProbe);
				}
				array2[0] = WellKnownSids.EdgeTransportServers;
				array3[0] = permission;
				array2[1] = WellKnownSids.ExternallySecuredServers;
				array3[1] = (Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders);
				array2[2] = WellKnownSids.HubTransportServers;
				array3[2] = permission;
				array[2] = new ReceiveConnector.PermissionGroupInfo(PermissionGroups.ExchangeServers, array2, array3);
				array[3] = new ReceiveConnector.PermissionGroupInfo(PermissionGroups.Partners, WellKnownSids.PartnerServers, Permission.SMTPSubmit | Permission.AcceptRoutingHeaders);
				if (!server.IsEdgeServer)
				{
					SecurityIdentifier sidForExchangeKnownGuid = ReceiveConnector.PermissionGroupPermissions.GetSidForExchangeKnownGuid(tenantOrRootOrgRecipientSession, WellKnownGuid.E3iWkGuid, tenantOrTopologyConfigurationSession.ConfigurationNamingContext.DistinguishedName);
					array[4] = new ReceiveConnector.PermissionGroupInfo(PermissionGroups.ExchangeLegacyServers, sidForExchangeKnownGuid, Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders);
				}
				return array;
			}

			internal static SecurityIdentifier GetSidForExchangeKnownGuid(IRecipientSession session, Guid knownGuid, string containerDN)
			{
				ADGroup exchangeAccount = null;
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
				{
					exchangeAccount = session.ResolveWellKnownGuid<ADGroup>(knownGuid, containerDN);
				}, 3);
				if (!adoperationResult.Succeeded || exchangeAccount == null)
				{
					throw new ErrorExchangeGroupNotFoundException(knownGuid, adoperationResult.Exception);
				}
				return exchangeAccount.Sid;
			}

			public const Permission Anonymous = Permission.SMTPSubmit | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.AcceptRoutingHeaders;

			public const Permission ExchangeUsers = Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.BypassAntiSpam | Permission.AcceptRoutingHeaders;

			public const Permission ExchangeServersE12 = Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders | Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders;

			public const Permission ExchangeServersE14 = Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders | Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders | Permission.SMTPAcceptXShadow;

			public const Permission ExchangeServers = Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders | Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders | Permission.SMTPAcceptXShadow | Permission.SMTPAcceptXProxyFrom | Permission.SMTPAcceptXSessionParams | Permission.SMTPAcceptXMessageContextADRecipientCache | Permission.SMTPAcceptXMessageContextExtendedProperties | Permission.SMTPAcceptXMessageContextFastIndex | Permission.SMTPAcceptXAttr | Permission.SMTPAcceptXSysProbe;

			public const Permission ExternallySecuredServers = Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders;

			public const Permission ExchangeLegacyServers = Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders;

			public const Permission ExchangeLegacyServersExplicitDeny = Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders;

			public const Permission Partners = Permission.SMTPSubmit | Permission.AcceptRoutingHeaders;

			public const Permission AcceptCrossForestEmail = Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders | Permission.AcceptOrganizationHeaders | Permission.SMTPAcceptXAttr | Permission.SMTPAcceptXSysProbe;

			public const Permission AcceptCloudServicesEmail = Permission.SMTPSubmit | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.AcceptRoutingHeaders;

			public const Permission AllowSubmit = Permission.SMTPSubmit | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.AcceptRoutingHeaders;
		}

		internal class PermissionGroupInfo
		{
			public PermissionGroupInfo(PermissionGroups permissionGroup, SecurityIdentifier sid, Permission defaultPermission)
			{
				this.PermissionGroup = permissionGroup;
				this.Sids = new SecurityIdentifier[]
				{
					sid
				};
				this.DefaultPermission = new Permission[]
				{
					defaultPermission
				};
			}

			public PermissionGroupInfo(PermissionGroups permissionGroup, SecurityIdentifier[] sid, Permission[] defaultPermission)
			{
				if (sid.Length != defaultPermission.Length)
				{
					throw new ArgumentException("Bug: number of sid must match number of defaultPermission");
				}
				this.PermissionGroup = permissionGroup;
				this.Sids = sid;
				this.DefaultPermission = defaultPermission;
			}

			public readonly PermissionGroups PermissionGroup;

			public readonly SecurityIdentifier[] Sids;

			public readonly Permission[] DefaultPermission;
		}
	}
}
