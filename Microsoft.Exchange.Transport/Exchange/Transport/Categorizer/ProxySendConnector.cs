using System;
using System.Security.AccessControl;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ProxySendConnector : SmtpSendConnectorConfig
	{
		public ProxySendConnector(string name, Server localServerConfig, ADObjectId localRoutingGroupId, string fqdn)
		{
			if (localServerConfig == null)
			{
				throw new ArgumentNullException("localServerConfig");
			}
			if (localRoutingGroupId == null)
			{
				throw new ArgumentNullException("localRoutingGroupId");
			}
			ADObjectId childId = localRoutingGroupId.GetChildId("Connections").GetChildId(name);
			base.SetId(childId);
			base.Name = name;
			string text = fqdn;
			if (string.IsNullOrEmpty(text))
			{
				text = localServerConfig.Fqdn;
				if (string.IsNullOrEmpty(text))
				{
					text = "local";
				}
			}
			base.Fqdn = new Fqdn(text);
			base.SmtpMaxMessagesPerConnection = localServerConfig.IntraOrgConnectorSmtpMaxMessagesPerConnection;
			base.ProtocolLoggingLevel = localServerConfig.IntraOrgConnectorProtocolLoggingLevel;
			this.PopulateCalculatedProperties();
		}

		public ProxySendConnector(string name, Server localServerConfig, ADObjectId localRoutingGroupId, bool internalConnector, bool requireTls, TlsAuthLevel? tlsAuthLevel, SmtpDomainWithSubdomains tlsDomain, bool useExternalDnsServer, int port, string fqdn, string certificateSubject) : this(name, localServerConfig, localRoutingGroupId, fqdn)
		{
			base.Port = port;
			if (internalConnector)
			{
				base.SmartHostAuthMechanism = SmtpSendConnectorConfig.AuthMechanisms.ExchangeServer;
			}
			else
			{
				base.RequireTLS = requireTls;
				base.TlsAuthLevel = tlsAuthLevel;
				base.TlsDomain = tlsDomain;
				base.UseExternalDNSServersEnabled = useExternalDnsServer;
				base.CertificateSubject = certificateSubject;
			}
			this.PopulateCalculatedProperties();
		}

		internal override RawSecurityDescriptor GetSecurityDescriptor()
		{
			return EnterpriseRelaySendConnector.SecurityDescriptor;
		}

		private void PopulateCalculatedProperties()
		{
			foreach (PropertyDefinition propertyDefinition in this.Schema.AllProperties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				if (adpropertyDefinition.IsCalculated && !base.ExchangeVersion.IsOlderThan(adpropertyDefinition.VersionAdded))
				{
					object obj = this.propertyBag[adpropertyDefinition];
				}
			}
		}
	}
}
