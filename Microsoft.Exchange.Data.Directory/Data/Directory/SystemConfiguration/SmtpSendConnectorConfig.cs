using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management.Automation;
using System.Net;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class SmtpSendConnectorConfig : MailGateway
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return SmtpSendConnectorConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return SmtpSendConnectorConfig.MostDerivedClass;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DNSRoutingEnabled
		{
			get
			{
				if (this[SmtpSendConnectorConfigSchema.DNSRoutingEnabled] == null)
				{
					return string.IsNullOrEmpty(this.SmartHostsString);
				}
				return (bool)this[SmtpSendConnectorConfigSchema.DNSRoutingEnabled];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.DNSRoutingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpDomainWithSubdomains TlsDomain
		{
			get
			{
				return (SmtpDomainWithSubdomains)this[SmtpSendConnectorConfigSchema.TlsDomain];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.TlsDomain] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public TlsAuthLevel? TlsAuthLevel
		{
			get
			{
				return (TlsAuthLevel?)this[SmtpSendConnectorConfigSchema.TlsAuthLevel];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.TlsAuthLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ErrorPolicies ErrorPolicies
		{
			get
			{
				return (ErrorPolicies)this[SmtpSendConnectorConfigSchema.ErrorPolicies];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.ErrorPolicies] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmartHost> SmartHosts
		{
			get
			{
				return (MultiValuedProperty<SmartHost>)this[SmtpSendConnectorConfigSchema.SmartHosts];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.SmartHosts] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int Port
		{
			get
			{
				return (int)this[SmtpSendConnectorConfigSchema.Port];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.Port] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ConnectionInactivityTimeOut
		{
			get
			{
				return (EnhancedTimeSpan)this[SmtpSendConnectorConfigSchema.ConnectionInactivityTimeout];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.ConnectionInactivityTimeout] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ForceHELO
		{
			get
			{
				return (bool)this[SmtpSendConnectorConfigSchema.ForceHELO];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.ForceHELO] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool FrontendProxyEnabled
		{
			get
			{
				return (bool)this[SmtpSendConnectorConfigSchema.FrontendProxyEnabled];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.FrontendProxyEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IgnoreSTARTTLS
		{
			get
			{
				return (bool)this[SmtpSendConnectorConfigSchema.IgnoreSTARTTLS];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.IgnoreSTARTTLS] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CloudServicesMailEnabled
		{
			get
			{
				return (bool)this[SmtpSendConnectorConfigSchema.CloudServicesMailEnabled];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.CloudServicesMailEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Fqdn Fqdn
		{
			get
			{
				return (Fqdn)this[SmtpSendConnectorConfigSchema.Fqdn];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.Fqdn] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpX509Identifier TlsCertificateName
		{
			get
			{
				return (SmtpX509Identifier)this[SmtpSendConnectorConfigSchema.TlsCertificateName];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.TlsCertificateName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireTLS
		{
			get
			{
				return (bool)this[SmtpSendConnectorConfigSchema.RequireTLS];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.RequireTLS] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RequireOorg
		{
			get
			{
				return (bool)this[SmtpSendConnectorConfigSchema.RequireOorg];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.RequireOorg] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override bool Enabled
		{
			get
			{
				return (bool)this[SmtpSendConnectorConfigSchema.Enabled];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.Enabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProtocolLoggingLevel ProtocolLoggingLevel
		{
			get
			{
				return (ProtocolLoggingLevel)this[SmtpSendConnectorConfigSchema.ProtocolLoggingLevel];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.ProtocolLoggingLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpSendConnectorConfig.AuthMechanisms SmartHostAuthMechanism
		{
			get
			{
				return (SmtpSendConnectorConfig.AuthMechanisms)this[SmtpSendConnectorConfigSchema.SmartHostAuthMechanism];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.SmartHostAuthMechanism] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PSCredential AuthenticationCredential
		{
			get
			{
				return (PSCredential)this[SmtpSendConnectorConfigSchema.AuthenticationCredential];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.AuthenticationCredential] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UseExternalDNSServersEnabled
		{
			get
			{
				return (bool)this[SmtpSendConnectorConfigSchema.UseExternalDNSServersEnabled];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.UseExternalDNSServersEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DomainSecureEnabled
		{
			get
			{
				return (bool)this[SmtpSendConnectorConfigSchema.DomainSecureEnabled];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.DomainSecureEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IPAddress SourceIPAddress
		{
			get
			{
				return (IPAddress)this[SmtpSendConnectorConfigSchema.SourceIPAddress];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.SourceIPAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SmtpMaxMessagesPerConnection
		{
			get
			{
				return (int)this[SmtpSendConnectorConfigSchema.SmtpMaxMessagesPerConnection];
			}
			set
			{
				this[SmtpSendConnectorConfigSchema.SmtpMaxMessagesPerConnection] = value;
			}
		}

		public string SmartHostsString
		{
			get
			{
				return (string)this[SmtpSendConnectorConfigSchema.SmartHostsString];
			}
		}

		public string CertificateSubject
		{
			get
			{
				return this.certificateSubject;
			}
			set
			{
				this.certificateSubject = value;
			}
		}

		internal static object SmartHostsGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[SmtpSendConnectorConfigSchema.SmartHostsString];
			if (string.IsNullOrEmpty(text))
			{
				return new MultiValuedProperty<SmartHost>(false, SmtpSendConnectorConfigSchema.SmartHosts, new SmartHost[0]);
			}
			List<SmartHost> routingHostsFromString = RoutingHost.GetRoutingHostsFromString<SmartHost>(text, (RoutingHost routingHost) => new SmartHost(routingHost));
			return new MultiValuedProperty<SmartHost>(false, SmtpSendConnectorConfigSchema.SmartHosts, routingHostsFromString);
		}

		internal static void SmartHostsSetter(object value, IPropertyBag propertyBag)
		{
			if (value == null)
			{
				propertyBag[SmtpSendConnectorConfigSchema.SmartHostsString] = string.Empty;
				return;
			}
			MultiValuedProperty<SmartHost> routingHostWrappers = (MultiValuedProperty<SmartHost>)value;
			string value2 = RoutingHost.ConvertRoutingHostsToString<SmartHost>(routingHostWrappers, (SmartHost host) => host.InnerRoutingHost);
			propertyBag[SmtpSendConnectorConfigSchema.SmartHostsString] = value2;
		}

		internal static object AuthenticationCredentialGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[SmtpSendConnectorConfigSchema.AuthenticationUserName];
			string text2 = (string)propertyBag[SmtpSendConnectorConfigSchema.EncryptedAuthenticationPassword];
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				SecureString secureString = new SecureString();
				foreach (char c in text2.ToCharArray())
				{
					secureString.AppendChar(c);
				}
				return new PSCredential(text, secureString);
			}
			return null;
		}

		internal static void AuthenticationCredentialSetter(object value, IPropertyBag propertyBag)
		{
			PSCredential pscredential = value as PSCredential;
			if (pscredential == null)
			{
				propertyBag[SmtpSendConnectorConfigSchema.AuthenticationUserName] = null;
				propertyBag[SmtpSendConnectorConfigSchema.EncryptedAuthenticationPassword] = null;
				return;
			}
			string value2 = string.Empty;
			if (pscredential.Password == null || pscredential.Password.Length == 0)
			{
				return;
			}
			value2 = pscredential.Password.ConvertToUnsecureString();
			propertyBag[SmtpSendConnectorConfigSchema.AuthenticationUserName] = pscredential.UserName;
			propertyBag[SmtpSendConnectorConfigSchema.EncryptedAuthenticationPassword] = value2;
		}

		internal static object SmartHostAuthMechanismGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[SmtpSendConnectorConfigSchema.SendConnectorFlags];
			return (SmtpSendConnectorConfig.AuthMechanisms)(((long)num & (long)((ulong)-1048576)) >> 20);
		}

		internal static void SmartHostAuthMechanismSetter(object value, IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[SmtpSendConnectorConfigSchema.SendConnectorFlags];
			num = (((int)value & 4095) << 20 | (num & 1048575));
			propertyBag[SmtpSendConnectorConfigSchema.SendConnectorFlags] = num;
		}

		internal static object TlsAuthLevelGetter(IPropertyBag propertyBag)
		{
			return SmtpSendConnectorConfig.TlsAuthLevelGetter(propertyBag, SmtpSendConnectorConfigSchema.SendConnectorFlags);
		}

		internal static object TlsAuthLevelGetter(IPropertyBag propertyBag, ADPropertyDefinition flagsProperty)
		{
			int num = (int)propertyBag[flagsProperty];
			TlsAuthLevel tlsAuthLevel = (TlsAuthLevel)((num & 4080) >> 4);
			return (tlsAuthLevel == (TlsAuthLevel)0) ? null : new TlsAuthLevel?(tlsAuthLevel);
		}

		internal static void TlsAuthLevelSetter(object value, IPropertyBag propertyBag)
		{
			SmtpSendConnectorConfig.TlsAuthLevelSetter(value, propertyBag, SmtpSendConnectorConfigSchema.SendConnectorFlags);
		}

		internal static void TlsAuthLevelSetter(object value, IPropertyBag propertyBag, ADPropertyDefinition flagsProperty)
		{
			if (value == null)
			{
				value = 0;
			}
			int num = (int)propertyBag[flagsProperty];
			num = (((int)value & 255) << 4 | (int)((uint)((long)num & (long)((ulong)-4081))));
			propertyBag[flagsProperty] = num;
		}

		internal string GetAuthenticationUserName()
		{
			return (string)this[SmtpSendConnectorConfigSchema.AuthenticationUserName];
		}

		internal bool IsInitialSendConnector()
		{
			MultiValuedProperty<AddressSpace> addressSpaces = base.AddressSpaces;
			if (addressSpaces != null)
			{
				foreach (AddressSpace addressSpace in addressSpaces)
				{
					if (addressSpace.Address.Equals("--"))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		internal SecureString GetSmartHostPassword()
		{
			SecureString secureString = new SecureString();
			string text = (string)this[SmtpSendConnectorConfigSchema.EncryptedAuthenticationPassword];
			foreach (char c in text.ToCharArray())
			{
				secureString.AppendChar(c);
			}
			return secureString;
		}

		internal virtual RawSecurityDescriptor GetSecurityDescriptor()
		{
			if (this.securityDescriptor == null)
			{
				this.securityDescriptor = base.ReadSecurityDescriptor();
			}
			return this.securityDescriptor;
		}

		internal Permission GetAnonymousPermissions()
		{
			if (!this.anonymousPermissionsSet)
			{
				this.DetermineAnonymousPermissions();
			}
			return this.anonymousPermissions;
		}

		internal Permission GetPartnerPermissions()
		{
			if (!this.partnerPermissionsSet)
			{
				this.DeterminePartnerPermissions();
			}
			return this.partnerPermissions;
		}

		private static int GetFlagsValue(IPropertyBag propertyBag, ProviderPropertyDefinition property, int flags)
		{
			int num = (int)propertyBag[property];
			return num & flags;
		}

		private Permission DeterminePermissions(SecurityIdentifier sid)
		{
			Permission result = Permission.None;
			RawSecurityDescriptor rawSecurityDescriptor = this.GetSecurityDescriptor();
			try
			{
				if (rawSecurityDescriptor != null)
				{
					result = AuthzAuthorization.CheckPermissions(sid, rawSecurityDescriptor, null);
				}
			}
			catch (Win32Exception)
			{
			}
			return result;
		}

		private void DetermineAnonymousPermissions()
		{
			this.anonymousPermissions = this.DeterminePermissions(SmtpSendConnectorConfig.AnonymousSecurityIdentifier);
			this.anonymousPermissionsSet = true;
		}

		private void DeterminePartnerPermissions()
		{
			this.partnerPermissions = this.DeterminePermissions(WellKnownSids.PartnerServers);
			this.partnerPermissionsSet = true;
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (MultiValuedPropertyBase.IsNullOrEmpty(base.ConnectedDomains) && MultiValuedPropertyBase.IsNullOrEmpty(base.AddressSpaces))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.NoAddressSpaces, MailGatewaySchema.AddressSpaces, this));
			}
			if (!this.DNSRoutingEnabled && MultiValuedPropertyBase.IsNullOrEmpty(this.SmartHosts))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.SmartHostNotSet, SmtpSendConnectorConfigSchema.SmartHosts, this));
			}
			IPvxAddress pvxAddress = new IPvxAddress(this.SourceIPAddress);
			if (pvxAddress.IsMulticast || pvxAddress.IsBroadcast)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.InvalidSourceAddressSetting, SmtpSendConnectorConfigSchema.SourceIPAddress, this));
			}
			if ((this.SmartHostAuthMechanism == SmtpSendConnectorConfig.AuthMechanisms.BasicAuth || this.SmartHostAuthMechanism == SmtpSendConnectorConfig.AuthMechanisms.BasicAuthRequireTLS) && this.AuthenticationCredential == null)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.AuthenticationCredentialNotSet, SmtpSendConnectorConfigSchema.AuthenticationCredential, this));
			}
			if (this.DomainSecureEnabled)
			{
				if (!this.DNSRoutingEnabled)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.DomainSecureWithoutDNSRoutingEnabled, SmtpSendConnectorConfigSchema.DNSRoutingEnabled, this));
				}
				if (this.IgnoreSTARTTLS)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.DomainSecureWithIgnoreStartTLSEnabled, SmtpSendConnectorConfigSchema.IgnoreSTARTTLS, this));
				}
			}
			if (this.TlsAuthLevel != null)
			{
				if (!this.RequireTLS)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.TlsAuthLevelWithRequireTlsDisabled, SmtpSendConnectorConfigSchema.TlsAuthLevel, this));
				}
				if (this.DomainSecureEnabled)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.TlsAuthLevelWithDomainSecureEnabled, SmtpSendConnectorConfigSchema.TlsAuthLevel, this));
				}
			}
			if (this.TlsDomain != null)
			{
				if (this.TlsAuthLevel == null || this.TlsAuthLevel.Value != Microsoft.Exchange.Data.TlsAuthLevel.DomainValidation)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.TlsDomainWithIncorrectTlsAuthLevel, SmtpSendConnectorConfigSchema.TlsDomain, this));
					return;
				}
			}
			else if (!this.DNSRoutingEnabled && this.TlsAuthLevel != null && this.TlsAuthLevel.Value == Microsoft.Exchange.Data.TlsAuthLevel.DomainValidation)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.TlsAuthLevelWithNoDomainOnSmartHost, SmtpSendConnectorConfigSchema.TlsDomain, this));
			}
		}

		public new static string MostDerivedClass = "msExchRoutingSMTPConnector";

		private static SmtpSendConnectorConfigSchema schema = ObjectSchema.GetInstance<SmtpSendConnectorConfigSchema>();

		protected static readonly SecurityIdentifier AnonymousSecurityIdentifier = new SecurityIdentifier(WellKnownSidType.AnonymousSid, null);

		[NonSerialized]
		private RawSecurityDescriptor securityDescriptor;

		[NonSerialized]
		private Permission anonymousPermissions;

		private bool anonymousPermissionsSet;

		[NonSerialized]
		private Permission partnerPermissions;

		private bool partnerPermissionsSet;

		private string certificateSubject;

		public enum AuthMechanisms
		{
			[LocDescription(DirectoryStrings.IDs.SendAuthMechanismNone)]
			None,
			[LocDescription(DirectoryStrings.IDs.SendAuthMechanismBasicAuth)]
			BasicAuth = 2,
			[LocDescription(DirectoryStrings.IDs.SendAuthMechanismBasicAuthPlusTls)]
			BasicAuthRequireTLS = 4,
			[LocDescription(DirectoryStrings.IDs.SendAuthMechanismExchangeServer)]
			ExchangeServer = 8,
			[LocDescription(DirectoryStrings.IDs.SendAuthMechanismExternalAuthoritative)]
			ExternalAuthoritative = 16
		}
	}
}
