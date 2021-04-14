using System;
using System.Management.Automation;
using System.Net;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class SmtpSendConnectorConfigSchema : MailGatewaySchema
	{
		internal static GetterDelegate AdvertisedDomainGetterDelegate()
		{
			return (IPropertyBag bag) => (SmtpDomain)bag[SmtpSendConnectorConfigSchema.Fqdn];
		}

		internal static SetterDelegate AdvertisedDomainSetterDelegate()
		{
			return delegate(object value, IPropertyBag bag)
			{
				if (value == null)
				{
					bag[SmtpSendConnectorConfigSchema.Fqdn] = null;
					return;
				}
				bag[SmtpSendConnectorConfigSchema.Fqdn] = new Fqdn(((SmtpDomain)value).Domain);
			};
		}

		private const int UsernameMaxLength = 256;

		private const int PasswordMaxLength = 256;

		public static readonly ADPropertyDefinition SmartHostsString = new ADPropertyDefinition("SmartHostsString", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchSmtpSmartHost", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Port = new ADPropertyDefinition("Port", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpSendPort", ADPropertyDefinitionFlags.PersistDefaultValue, 25, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 65535)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConnectionInactivityTimeout = new ADPropertyDefinition("ConnectionInactivityTimeout", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchSmtpSendConnectionTimeout", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromMinutes(10.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.OneSecond, EnhancedTimeSpan.OneDay),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SendConnectorSecurityFlags = new ADPropertyDefinition("SendConnectorSecurityFlags", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchSmtpOutboundSecurityFlag", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Enabled = new ADPropertyDefinition("Enabled", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchSmtpSendEnabled", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TlsDomain = new ADPropertyDefinition("TlsDomain", ExchangeObjectVersion.Exchange2007, typeof(SmtpDomainWithSubdomains), "msExchSmtpSendTlsDomain", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new DisallowStarSmtpDomainWithSubdomainsConstraint()
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ErrorPoliciesBase = new ADPropertyDefinition("ErrorPolicies", ExchangeObjectVersion.Exchange2007, typeof(ErrorPolicies), "msExchSmtpSendNdrLevel", ADPropertyDefinitionFlags.None, Microsoft.Exchange.Data.ErrorPolicies.Default, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ErrorPolicies = new ADPropertyDefinition("CalculatedErrorPolicies", ExchangeObjectVersion.Exchange2007, typeof(ErrorPolicies), null, ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.ErrorPolicies.Default, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SmtpSendConnectorConfigSchema.ErrorPoliciesBase
		}, null, delegate(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[SmtpSendConnectorConfigSchema.ErrorPoliciesBase];
			num &= -3;
			return (ErrorPolicies)num;
		}, delegate(object value, IPropertyBag propertyBag)
		{
			propertyBag[SmtpSendConnectorConfigSchema.ErrorPoliciesBase] = (ErrorPolicies)value;
		}, null, null);

		public static readonly ADPropertyDefinition ProtocolLoggingLevel = new ADPropertyDefinition("ProtocolLoggingLevel", ExchangeObjectVersion.Exchange2007, typeof(ProtocolLoggingLevel), "msExchSmtpSendProtocolLoggingLevel", ADPropertyDefinitionFlags.None, Microsoft.Exchange.Data.ProtocolLoggingLevel.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AuthenticationUserName = new ADPropertyDefinition("AuthenticationUserName", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchSmtpOutboundSecurityUserName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 256)
		}, null, null);

		public static readonly ADPropertyDefinition EncryptedAuthenticationPassword = new ADPropertyDefinition("EncryptedAuthenticationPassword", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchSmtpOutboundSecurityPassword", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 256)
		}, null, null);

		public static readonly ADPropertyDefinition Fqdn = new ADPropertyDefinition("Fqdn", ExchangeObjectVersion.Exchange2007, typeof(Fqdn), "msExchSMTPSendConnectorFQDN", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition TlsCertificateName = new ADPropertyDefinition("TlsCertificateName", ExchangeObjectVersion.Exchange2007, typeof(SmtpX509Identifier), "msExchSmtpTLSCertificate", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SendConnectorFlags = new ADPropertyDefinition("SendConnectorFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpSendFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SourceIPAddress = new ADPropertyDefinition("SourceIPAddress", ExchangeObjectVersion.Exchange2007, typeof(IPAddress), "msExchSmtpSendBindingIPAddress", ADPropertyDefinitionFlags.None, IPAddress.Any, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SmtpMaxMessagesPerConnection = new ADPropertyDefinition("SmtpMaxMessagesPerConnection", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchSmtpMaxMessagesPerConnection", ADPropertyDefinitionFlags.PersistDefaultValue, 20, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition DNSRoutingEnabled = new ADPropertyDefinition("DNSRoutingEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool?), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SmartHosts = new ADPropertyDefinition("SmartHosts", ExchangeObjectVersion.Exchange2003, typeof(SmartHost), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SmtpSendConnectorConfigSchema.SmartHostsString
		}, null, new GetterDelegate(SmtpSendConnectorConfig.SmartHostsGetter), new SetterDelegate(SmtpSendConnectorConfig.SmartHostsSetter), null, null);

		public static readonly ADPropertyDefinition ForceHELO = new ADPropertyDefinition("ForceHELO", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SmtpSendConnectorConfigSchema.SendConnectorSecurityFlags
		}, null, ADObject.FlagGetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorSecurityFlags, 4194304), ADObject.FlagSetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorSecurityFlags, 4194304), null, null);

		public static readonly ADPropertyDefinition FrontendProxyEnabled = new ADPropertyDefinition("FrontendProxyEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SmtpSendConnectorConfigSchema.SendConnectorFlags
		}, null, ADObject.FlagGetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorFlags, 8192), ADObject.FlagSetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorFlags, 8192), null, null);

		public static readonly ADPropertyDefinition IgnoreSTARTTLS = new ADPropertyDefinition("IgnoreSTARTTLS", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SmtpSendConnectorConfigSchema.SendConnectorSecurityFlags
		}, null, ADObject.FlagGetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorSecurityFlags, 8388608), ADObject.FlagSetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorSecurityFlags, 8388608), null, null);

		public static readonly ADPropertyDefinition CloudServicesMailEnabled = new ADPropertyDefinition("CloudServicesMailEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SmtpSendConnectorConfigSchema.SendConnectorFlags
		}, null, ADObject.FlagGetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorFlags, 131072), ADObject.FlagSetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorFlags, 131072), null, null);

		public static readonly ADPropertyDefinition RequireOorg = new ADPropertyDefinition("RequireOorg", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SmtpSendConnectorConfigSchema.SendConnectorFlags
		}, null, ADObject.FlagGetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorFlags, 8), ADObject.FlagSetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorFlags, 8), null, null);

		public static readonly ADPropertyDefinition RequireTLS = new ADPropertyDefinition("RequireTLS", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SmtpSendConnectorConfigSchema.SendConnectorSecurityFlags
		}, null, ADObject.FlagGetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorSecurityFlags, 4), ADObject.FlagSetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorSecurityFlags, 4), null, null);

		public static readonly ADPropertyDefinition AuthenticationCredential = new ADPropertyDefinition("AuthenticationCredential", ExchangeObjectVersion.Exchange2003, typeof(PSCredential), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SmtpSendConnectorConfigSchema.AuthenticationUserName,
			SmtpSendConnectorConfigSchema.EncryptedAuthenticationPassword
		}, null, new GetterDelegate(SmtpSendConnectorConfig.AuthenticationCredentialGetter), new SetterDelegate(SmtpSendConnectorConfig.AuthenticationCredentialSetter), null, null);

		public static readonly ADPropertyDefinition UseExternalDNSServersEnabled = new ADPropertyDefinition("UseExternalDNSServersEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SmtpSendConnectorConfigSchema.SendConnectorFlags
		}, null, ADObject.FlagGetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorFlags, 2), ADObject.FlagSetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorFlags, 2), null, null);

		public static readonly ADPropertyDefinition DomainSecureEnabled = new ADPropertyDefinition("DomainSecureEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SmtpSendConnectorConfigSchema.SendConnectorFlags
		}, null, ADObject.FlagGetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorFlags, 4), ADObject.FlagSetterDelegate(SmtpSendConnectorConfigSchema.SendConnectorFlags, 4), null, null);

		public static readonly ADPropertyDefinition SmartHostAuthMechanism = new ADPropertyDefinition("SmartHostAuthMechanism", ExchangeObjectVersion.Exchange2007, typeof(SmtpSendConnectorConfig.AuthMechanisms), null, ADPropertyDefinitionFlags.Calculated, SmtpSendConnectorConfig.AuthMechanisms.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SmtpSendConnectorConfigSchema.SendConnectorFlags
		}, null, new GetterDelegate(SmtpSendConnectorConfig.SmartHostAuthMechanismGetter), new SetterDelegate(SmtpSendConnectorConfig.SmartHostAuthMechanismSetter), null, null);

		public static readonly ADPropertyDefinition TlsAuthLevel = new ADPropertyDefinition("TlsAuthLevel", ExchangeObjectVersion.Exchange2007, typeof(TlsAuthLevel?), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SmtpSendConnectorConfigSchema.SendConnectorFlags
		}, null, new GetterDelegate(SmtpSendConnectorConfig.TlsAuthLevelGetter), new SetterDelegate(SmtpSendConnectorConfig.TlsAuthLevelSetter), null, null);
	}
}
